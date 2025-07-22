using Microsoft.Win32.SafeHandles;
using S6Patcher.Properties;
using S6Patcher.Source.Helpers;
using System;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace S6Patcher.Source.Patcher
{
    internal class CheckSumCalculator
    {
        [DllImport("imagehlp.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern int CheckSumMappedFile(SafeMemoryMappedViewHandle BaseAddress, uint FileLength, ref uint HeaderSum, ref uint CheckSum);
        private uint UpdatePEHeaderFileCheckSum(string Path, long Size)
        {
            Logger.Instance.Log("UpdatePEHeaderFileCheckSum(): Called.");

            uint CheckSum = 0x0;
            uint HeaderSum = 0x0;
            using (MemoryMappedFile Mapping = MemoryMappedFile.CreateFromFile(Path))
            {
                using (MemoryMappedViewAccessor View = Mapping.CreateViewAccessor())
                {
                    CheckSumMappedFile(View.SafeMemoryMappedViewHandle, (uint)Size, ref HeaderSum, ref CheckSum); // This will only work on Windows
                };
            };

            Logger.Instance.Log("UpdatePEHeaderFileCheckSum(): Finished successfully. New CheckSum: 0x" + $"{CheckSum.ToString():X}");
            return CheckSum;
        }

        public void WritePEHeaderFileCheckSum(string Path, long Size)
        {
            uint CheckSum = UpdatePEHeaderFileCheckSum(Path, Size);
            FileStream CurrentStream = IOFileHandler.Instance.OpenFileStream(Path);
            if (CurrentStream == null)
            {
                Logger.Instance.Log("WritePEHeaderFileCheckSum():" + Resources.ErrorInvalidExecutable);
                MessageBox.Show(Resources.ErrorInvalidExecutable, "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            byte[] CheckSumByte = BitConverter.GetBytes(CheckSum);
            CurrentStream.Position = 0x168;
            CurrentStream.Write(CheckSumByte, 0, CheckSumByte.Length);

            CurrentStream.Close();
            CurrentStream.Dispose();
        }
    }
}
