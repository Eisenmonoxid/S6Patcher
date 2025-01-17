using S6Patcher.Source.Forms;
using System;
using System.Windows.Forms;

namespace S6Patcher.Source
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new mainFrm());
        }
    }
}