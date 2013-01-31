using System;
using System.Windows.Forms;

namespace Snake
{
    /// <summary>
    ///     Entry point for the Snake game
    ///     Ryan Harrison 2013
    ///     www.raharrison.co.uk
    /// </summary>
    internal static class Program
    {
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}