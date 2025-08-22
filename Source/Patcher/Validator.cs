using S6Patcher.Source.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace S6Patcher.Source.Patcher
{
    public class Validator
    {
        public execID ID {get;} = execID.NONE;
        public bool IsExecutableUnpacked {get;} = true;

        private const string ExpectedVersion = "1, 71, 4289, 0";
        private readonly Dictionary<UInt32, execID> Mapping = new()
        {
            {0x6ECADC, execID.OV},
            {0x2FCADC, execID.OV_OFFSET},
            {0xF531A4, execID.HE_UBISOFT},
            {0xF545A4, execID.HE_STEAM},
            {0x6D06A8, execID.ED},
        };

        public Validator(FileStream Stream)
        {
            if (Stream == null || Stream.CanRead == false)
            {
                throw new ArgumentException("Validator: Erroneous arguments to Validator ctor.");
            }

            ID = ValidateExecutableID(Stream);
            if (ID == execID.OV || ID == execID.HE_STEAM)
            {
                IsExecutableUnpacked = IsSteamExecutableValid(Stream, ID);
            }
        }

        private execID ValidateExecutableID(FileStream Stream)
        {
            Logger.Instance.Log("ValidateExecutableID(): Called with Stream: " + Stream.Name);

            int Size = Encoding.Unicode.GetByteCount(ExpectedVersion) + 1;
            byte[] Result = new byte[Size];

            foreach (var Element in Mapping)
            {
                if (Stream.Length < Element.Key)
                {
                    Logger.Instance.Log("ValidateExecutableID(): " + Element.Value.ToString() + " Stream Length smaller than Mapping Index: " + Stream.Length.ToString());
                    continue;
                }

                Stream.Position = Element.Key;
                Stream.Read(Result, 0, Result.Length);

                string Version = Encoding.Unicode.GetString(Result)[..ExpectedVersion.Length];
                Logger.Instance.Log("ValidateExecutableID(): Read from File: " + Version);

                if (Version == ExpectedVersion)
                {
                    Logger.Instance.Log("ValidateExecutableID(): Valid executable! execID: " + Element.Value.ToString());
                    return Element.Value;
                };
            }

            Logger.Instance.Log("ValidateExecutableID(): NO valid executable was found!");
            return execID.NONE;
        }

        private bool IsSteamExecutableValid(FileStream Stream, execID ID)
        {
            byte[] Identifier = [0x84, 0xC0];
            UInt32[] Addresses = [0x00C044, 0x1E0F08]; // 0 = OV, 1 = Steam HE

            byte[] Result = new byte[Identifier.Length];
            Stream.Position = Addresses[ID == execID.OV ? 0 : 1];
            Stream.Read(Result, 0, Result.Length);

            return Identifier.SequenceEqual(Result);
        }
    }
}
