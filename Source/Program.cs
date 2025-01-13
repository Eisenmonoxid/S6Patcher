using System;
using System.Windows.Forms;

namespace S6Patcher
{
    internal static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new mainFrm(args));
        }
    }
}