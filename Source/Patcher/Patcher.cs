using S6Patcher.Source.Helpers;
using S6Patcher.Source.Patcher.Mappings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace S6Patcher.Source.Patcher
{
    internal partial class Patcher
    {
        private readonly BinaryParser GlobalParser;
        private readonly FileStream GlobalStream;
        private readonly MappingBase GlobalMappings;
        private readonly Mod GlobalMod;
        public readonly execID GlobalID;
        private readonly Lock WriteLock = new();
        public event Action<string> ShowMessage;

        public Patcher(string FilePath, Stream DefinitionStream)
        {
            if (FilePath == null)
            {
                throw new ArgumentException("No file chosen or file does not exist!\nAborting ...");
            }

            try
            {
                GlobalStream = new ExecutableHandler(FilePath).OpenExecutableFile();
            }
            catch (Exception)
            {
                throw;
            }

            Validator Validator = new(GlobalStream);
            try
            {
                GlobalID = Validator.Validate();
            }
            catch (Exception)
            {
                IOFileHandler.Instance.CloseStream(GlobalStream);
                throw;
            }

            try
            {
                GlobalParser = new BinaryParser(DefinitionStream);
            }
            catch (Exception)
            {
                IOFileHandler.Instance.CloseStream(GlobalStream);
                throw;
            }

            GlobalMappings = MappingBase.GetMappingsByID(GlobalID, GlobalParser);
            if (GlobalMappings == null)
            {
                IOFileHandler.Instance.CloseStream(GlobalStream);
                throw new ArgumentException("Erroneous arguments to Patcher ctor.");
            }

            GlobalMod = new Mod(GlobalID, IOFileHandler.Instance.GetModLoaderDirectory(GlobalID, GlobalStream.Name));
            GlobalMod.ShowMessage += Message => ShowMessage.Invoke(Message);

            Logger.Instance.Log("ID: " + GlobalID.ToString() + ", Stream: " + GlobalStream.Name);
        }

        public void PatchByControlFeatures(List<string> Features) => WriteMappingToFile(GlobalMappings.GetMapping(Features));

        public void SetTextureResolution(string ResolutionText)
        {
            Logger.Instance.Log("Called with " + ResolutionText);

            if (uint.TryParse(ResolutionText, out uint Resolution) == false)
            {
                string Text = "Invalid texture resolution value: " + ResolutionText;
                ShowMessage.Invoke(Text);
                Logger.Instance.Log(Text);
                return;
            }

            WriteMappingToFile(GlobalMappings.GetTextureResolutionMapping(Resolution));
            IOFileHandler.Instance.UpdateEntryInOptionsFile("[Display]", "TextureResolution", 3);
            IOFileHandler.Instance.UpdateEntryInOptionsFile("[Display]", "Terrain", 2);
        }

        public void SetAutosaveTimer(string AutosaveText)
        {
            Logger.Instance.Log("Called with " + AutosaveText);

            if (double.TryParse(AutosaveText, out double Timer) == false)
            {
                string Text = "Invalid autosave timer value: " + AutosaveText;
                ShowMessage.Invoke(Text);
                Logger.Instance.Log(Text);
                return;
            }

            WriteMappingToFile(GlobalMappings.GetAutoSaveMapping(Timer));
        }

        public void SetZoomLevel(string ZoomText)
        {
            Logger.Instance.Log("Called with " + ZoomText);

            if (double.TryParse(ZoomText, out double Level) == false || float.TryParse(ZoomText, out float Distance) == false)
            {
                string Text = "Invalid zoom level value: " + ZoomText;
                ShowMessage.Invoke(Text);
                Logger.Instance.Log(Text);
                return;
            }

            WriteMappingToFile(GlobalMappings.GetZoomLevelMapping(Level, Distance));
        }

        public async Task SetModLoader(bool UseBugFixMod)
        {
            Logger.Instance.Log("Called with " + UseBugFixMod.ToString());
            WriteMappingToFile(GlobalMappings.GetModloaderMapping());
            SetEntryInOptionsFile("SpecialKnightsAvailable", UseBugFixMod);
            await GlobalMod.CreateModLoader(UseBugFixMod);
        }

        public void SetLargeAddressAwareFlag()
        {
            Logger.Instance.Log("Called.");

            // Partially adapted from: https://stackoverflow.com/questions/9054469
            const int IMAGE_FILE_LARGE_ADDRESS_AWARE = 0x20;
            BinaryReader Reader = new(GlobalStream);

            Reader.BaseStream.Position = 0x3C;
            Reader.BaseStream.Position = Reader.ReadInt32();

            if (Reader.ReadInt32() != 0x4550)
            {
                Logger.Instance.Log("Error - Not at expected offset!");
                return;
            }

            Reader.BaseStream.Position += 0x12;
            long CurrentPosition = Reader.BaseStream.Position;

            short Flag = Reader.ReadInt16();
            if ((Flag & IMAGE_FILE_LARGE_ADDRESS_AWARE) != IMAGE_FILE_LARGE_ADDRESS_AWARE)
            {
                WriteBytes(CurrentPosition, BitConverter.GetBytes(Flag |= IMAGE_FILE_LARGE_ADDRESS_AWARE));
            }

            Logger.Instance.Log("Finished successfully.");
        }

        public void SetEntryInOptionsFile(string Entry, bool Checked) => 
            IOFileHandler.Instance.UpdateEntryInOptionsFile("[S6Patcher]", Entry, Checked == true ? 1U : 0U);
        public void SetEasyDebug() => WriteMappingToFile(GlobalMappings.GetEasyDebugMapping());

        public async Task WriteScriptFilesToFolder()
        {
            WriteMappingToFile(GlobalMappings.GetScriptFileMapping());
            foreach (var Element in ScriptFeatures.Features)
            {
                SetEntryInOptionsFile(Element.Key, Element.Value);
            }

            await UserScriptHandler.Instance.CreateUserScriptFiles();
        }

        private void WriteMappingToFile(Dictionary<UInt32, byte[]> Mapping)
        {
            foreach (var Entry in Mapping)
            {
                WriteBytes(Entry.Key, Entry.Value);
                Logger.Instance.Log("Patching Element: 0x" + $"{Entry.Key:X}");
            }
        }

        private void WriteBytes(long Position, byte[] Bytes)
        {
            lock (WriteLock)
            {
                GlobalStream.Position = Position;
                try
                {
                    GlobalStream.Write(Bytes, 0, Bytes.Length);
                }
                catch (Exception ex)
                {
                    ShowMessage.Invoke(ex.ToString());
                    Logger.Instance.Log(ex.ToString());
                }
            }
        }

        public void Dispose(bool FinishWithPEHeader = false)
        {
            string Name = GlobalStream.Name;
            long Size = GlobalStream.Length;

            lock (WriteLock)
            {
                IOFileHandler.Instance.CloseStream(GlobalStream);
            }

            if (FinishWithPEHeader)
            {
                new CheckSumCalculator().WritePEHeaderFileCheckSum(Name, Size);
            }
        }
    }
}
