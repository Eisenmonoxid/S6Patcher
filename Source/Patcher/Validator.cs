using S6Patcher.Source.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace S6Patcher.Source.Patcher
{
    internal class Validator(FileStream Stream)
    {
        private const string ExpectedVersion = "1, 71, 4289, 0";
        private readonly Dictionary<uint, execID> Mapping = new()
        {
            {0x6ECADC, execID.OV},
            {0xF531A4, execID.HE_UBISOFT},
            {0xF545A4, execID.HE_STEAM},
            {0x6D06A8, execID.ED},
        };

        public execID Validate()
        {
            execID ID = ValidateExecutableID();
            if (ID == execID.NONE)
            {
                throw new Exception("Invalid executable has been chosen!\nAborting ...");
            }

            if ((ID == execID.OV || ID == execID.HE_STEAM) && !IsSteamExecutableValid(ID))
            {
                throw new Exception("Steam executable invalid, has to be unpacked with the tool \"Steamless\" before using the S6Patcher!\nAborting ...");
            }

            return ID;
        }

        private execID ValidateExecutableID()
        {
            Logger.Instance.Log("ValidateExecutableID(): Called with Stream: " + Stream.Name);

            int Size = Encoding.Unicode.GetByteCount(ExpectedVersion) + 1;
            byte[] Result = new byte[Size];

            foreach (var Element in Mapping)
            {
                if (Stream.Length < Element.Key)
                {
                    Logger.Instance.Log("ValidateExecutableID(): " + Element.Value.ToString() + 
                        " Stream Length smaller than Mapping Index: " + Stream.Length.ToString());
                    continue;
                }

                Stream.Position = Element.Key;
                Stream.ReadExactly(Result);

                string Version = Encoding.Unicode.GetString(Result)[..ExpectedVersion.Length];
                Logger.Instance.Log("ValidateExecutableID(): " + Element.Value.ToString() + " Read from File: " + Version);

                if (Version == ExpectedVersion)
                {
                    Logger.Instance.Log("ValidateExecutableID(): Valid executable! execID: " + Element.Value.ToString());
                    return Element.Value;
                };
            }

            Logger.Instance.Log("ValidateExecutableID(): No valid executable was found!");
            return execID.NONE;
        }

        private bool IsSteamExecutableValid(execID? ID)
        {
            byte[] Identifier = [0x84, 0xC0];
            uint[] Addresses = [0x00C044, 0x1E0F08]; // 0 = OV, 1 = Steam HE

            byte[] Result = new byte[Identifier.Length];
            Stream.Position = Addresses[ID == execID.OV ? 0 : 1];
            Stream.ReadExactly(Result);

            return Identifier.SequenceEqual(Result);
        }
    }
}
