using S6Patcher.Source.Forms;
using System;
using System.Windows.Forms;

namespace S6Patcher.Source
{
    internal static class Program
    {
        internal static bool IsMono {get;} = Type.GetType("Mono.Runtime") != null; // Testing
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new mainFrm());
        }
    }
}