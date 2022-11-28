using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        //static void Main(string[] args)
        //{
        //    Application.EnableVisualStyles();
        //    Application.SetCompatibleTextRenderingDefault(false);
        //    Application.Run(new Graph(args[0])); //cannot run it directly, i.e. called from Unity
        //    //Application.Run(new Graph("[Gerard, Chair-on|Book-under|Laptop-right_of]"));
        //}
    }
}