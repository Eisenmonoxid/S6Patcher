﻿using Microsoft.Win32.SafeHandles;
using S6Patcher.Source.Helpers;
using System;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;

namespace S6Patcher.Source.Patcher
{
    internal class CheckSumCalculator
    {
        [DllImport("imagehlp.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern int CheckSumMappedFile(SafeMemoryMappedViewHandle BaseAddress, uint FileLength,
            ref uint HeaderSum, ref uint CheckSum);

        private uint UpdatePEHeaderFileCheckSum(string Path, long Size)
        {
            Logger.Instance.Log("Called.");

            // This will only work on Windows
            uint CheckSum = 0x0;
            uint HeaderSum = 0x0;

            using MemoryMappedFile Mapping = MemoryMappedFile.CreateFromFile(Path);
            using MemoryMappedViewAccessor View = Mapping.CreateViewAccessor();

            int Result = CheckSumMappedFile(View.SafeMemoryMappedViewHandle, (uint)Size, ref HeaderSum, ref CheckSum);
            if (Result == 0x0)
            {
                Logger.Instance.Log("CheckSumMappedFile failed with code 0 and error: " + CheckSum.ToString());
                return 0;
            }

            Logger.Instance.Log("New CheckSum: 0x" + $"{CheckSum.ToString():X}");
            return CheckSum;
        }

        public void WritePEHeaderFileCheckSum(string Path, long Size)
        {
            uint CheckSum = UpdatePEHeaderFileCheckSum(Path, Size);
            FileStream Stream = IOFileHandler.Instance.OpenFileStream(Path);
            if (Stream == null)
            {
                return;
            }

            WritePEHeaderFileCheckSum(Stream, BitConverter.GetBytes(CheckSum));
            IOFileHandler.Instance.CloseStream(Stream);
        }

        private void WritePEHeaderFileCheckSum(FileStream Stream, byte[] CheckSum)
        {
            BinaryReader Reader = new(Stream);

            Reader.BaseStream.Position = 0x3C;
            Reader.BaseStream.Position = Reader.ReadInt32();

            if (Reader.ReadInt32() != 0x4550)
            {
                Logger.Instance.Log("CheckSum offset not found! Skipping ...");
                return;
            }

            Reader.BaseStream.Position += 0x54;
            Reader.BaseStream.Write(CheckSum, 0, CheckSum.Length);
        }
    }
}