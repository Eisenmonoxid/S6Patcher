using S6Patcher.Source.Helpers;
using S6Patcher.Source.Patcher.Mappings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace S6Patcher.Source.Patcher
{
    internal partial class Patcher
    {
        private readonly FileStream GlobalStream;
        private readonly MappingBase GlobalMappings;
        private readonly Mod GlobalMod;
        public readonly execID GlobalID;

        public event Action<string> ShowMessage;
        public event Action<bool> PatchingFinished;

        public Patcher(string FilePath)
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

            GlobalMappings = MappingBase.GetMappingsByID(GlobalID);
            if (GlobalMappings == null)
            {
                IOFileHandler.Instance.CloseStream(GlobalStream);
                throw new ArgumentException("Erroneous arguments to Patcher ctor.");
            }

            GlobalMod = new Mod(GlobalID, IOFileHandler.Instance.GetModLoaderDirectory(GlobalID, GlobalStream.Name));
            GlobalMod.Finished += Success => PatchingFinished?.Invoke(Success);
            GlobalMod.ShowErrorMessage += Message => ShowMessage?.Invoke(Message);

            Logger.Instance.Log("Patcher ctor(): ID: " + GlobalID.ToString() + ", Stream: " + GlobalStream.Name);
        }

        public void PatchByControlFeatures(List<string> Features)
        {
            List<Dictionary<long, byte[]>> Results = 
                [.. from Entry in GlobalMappings?.GetMapping()
                    from Name in Features
                    where Name == Entry.Name
                    select Entry.Mapping];

            foreach (var Element in Results)
            {
                WriteMappingToFile(Element);
            }
        }

        public void SetTextureResolution(string ResolutionText)
        {
            Logger.Instance.Log("SetTextureResolution(): Called with " + ResolutionText);

            if (uint.TryParse(ResolutionText, out uint Resolution) == false)
            {
                ShowMessage?.Invoke("Invalid texture resolution value: " + ResolutionText);
                return;
            }

            WriteMappingToFile(GlobalMappings.GetTextureResolutionMapping(Resolution));
            IOFileHandler.Instance.UpdateEntryInOptionsFile("[Display]", "TextureResolution", 3);
            IOFileHandler.Instance.UpdateEntryInOptionsFile("[Display]", "Terrain", 2);
        }

        public void SetAutosaveTimer(string AutosaveText)
        {
            Logger.Instance.Log("SetAutosaveTimer(): Called with " + AutosaveText);

            if (double.TryParse(AutosaveText, out double Timer) == false)
            {
                ShowMessage?.Invoke("Invalid autosave timer value: " + AutosaveText);
                return;
            }

            WriteMappingToFile(GlobalMappings.GetAutoSaveMapping(Timer));
        }

        public void SetZoomLevel(string ZoomText)
        {
            Logger.Instance.Log("SetZoomLevel(): Called with " + ZoomText);

            if (double.TryParse(ZoomText, out double Level) == false || float.TryParse(ZoomText, out float Distance) == false)
            {
                ShowMessage?.Invoke("Invalid zoom level value: " + ZoomText);
                return;
            }

            WriteMappingToFile(GlobalMappings.GetZoomLevelMapping(Level, Distance));
        }

        public void SetModLoader(bool UseBugFixMod)
        {
            Logger.Instance.Log("SetModLoader(): Called with " + UseBugFixMod.ToString());
            WriteMappingToFile(GlobalMappings.GetModloaderMapping());
            GlobalMod.CreateModLoader(UseBugFixMod);
        }

        public void SetLargeAddressAwareFlag()
        {
            Logger.Instance.Log("SetLargeAddressAwareFlag(): Called.");

            // Partially adapted from: https://stackoverflow.com/questions/9054469
            const int IMAGE_FILE_LARGE_ADDRESS_AWARE = 0x20;
            BinaryReader Reader = new(GlobalStream);

            Reader.BaseStream.Position = 0x3C;
            Reader.BaseStream.Position = Reader.ReadInt32();

            if (Reader.ReadInt32() != 0x4550)
            {
                Logger.Instance.Log("SetLargeAddressAwareFlag(): Error - Not at expected offset!");
                return;
            }

            Reader.BaseStream.Position += 0x12;
            long CurrentPosition = Reader.BaseStream.Position;

            short Flag = Reader.ReadInt16();
            if ((Flag & IMAGE_FILE_LARGE_ADDRESS_AWARE) != IMAGE_FILE_LARGE_ADDRESS_AWARE)
            {
                WriteBytes(CurrentPosition, BitConverter.GetBytes(Flag |= IMAGE_FILE_LARGE_ADDRESS_AWARE));
            }

            Logger.Instance.Log("SetLargeAddressAwareFlag(): Finished successfully.");
        }

        public void SetEntryInOptionsFile(string Entry, bool Checked) => 
            IOFileHandler.Instance.UpdateEntryInOptionsFile("[S6Patcher]", Entry, Checked == true ? 1U : 0U);
        public void SetEasyDebug() => WriteMappingToFile(GlobalMappings.GetEasyDebugMapping());

        public void WriteScriptFilesToFolder()
        {
            WriteMappingToFile(GlobalMappings.GetScriptFileMapping());
            UserScriptHandler.Instance.CreateUserScriptFiles();

            foreach (var Element in ScriptFeatures.Features)
            {
                SetEntryInOptionsFile(Element, true);
            }
        }

        private void WriteMappingToFile(Dictionary<long, byte[]> Mapping)
        {
            foreach (var Entry in Mapping)
            {
                WriteBytes(Entry.Key, Entry.Value);
                Logger.Instance.Log("WriteMappingToFile(): Patching Element: 0x" + $"{Entry.Key:X}");
            }
        }

        private void WriteBytes(long Position, byte[] Bytes)
        {
            GlobalStream.Position = Position;
            try
            {
                GlobalStream.Write(Bytes, 0, Bytes.Length);
            }
            catch (Exception ex)
            {
                ShowMessage?.Invoke(ex.ToString());
                Logger.Instance.Log(ex.ToString());
            }
        }

        public void RaiseFinishedEvent(bool Success) => PatchingFinished.Invoke(Success);

        public void Dispose(bool FinishWithPEHeader = false)
        {
            string Name = GlobalStream.Name;
            long Size = GlobalStream.Length;
            IOFileHandler.Instance.CloseStream(GlobalStream);

            if (FinishWithPEHeader)
            {
                new CheckSumCalculator().WritePEHeaderFileCheckSum(Name, Size);
            }
        }
    }
}
