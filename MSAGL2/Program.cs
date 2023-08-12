using System;
using System.Windows.Forms;

namespace MSAGL2
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length != 0) //if there are arguments, i.e. called from unity
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                //Application.Run(new Graph(args[0]));
                Application.Run(new Graph(args[0]));

            }
            else //if there are no arguments, i.e. start normally
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Graph());
            }
        }
    }
}