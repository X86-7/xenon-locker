using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Xenon
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            string pass = Properties.Settings.Default.passSet;
            if (pass == "on")
            {
                Application.Run(new Form1());
            }
            else
            {
                Application.Run(new Setup());
            }

        }
    }
}
