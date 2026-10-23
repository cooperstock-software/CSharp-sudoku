using System;
using System.Windows.Forms;

namespace SudokuProject
{
    /// <summary>
    /// Application entry point. Replaces SudokuProject.dpr.
    /// </summary>
    internal static class Program
    {
        // The three forms are created once and reused, so that the user can switch between them
        // without losing their work.
        internal static SudokuBaseForm MainForm { get; private set; }
        internal static Sudoku6x6Form SixBySixForm { get; private set; }
        internal static OptionsForm OptionsDialog { get; private set; }

        [STAThread]
        private static void Main()
        {
            // The layout is built from fixed pixel positions (the grid cells are placed
            // at hard-coded coordinates), so run DPI-unaware to keep that layout exact on
            // high-DPI displays. Windows scales the whole window for us.
            // Change to HighDpiMode.SystemAware if you prefer crisp text and rework the layout.
            Application.SetHighDpiMode(HighDpiMode.DpiUnaware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Globals.ReadIni();

            MainForm = new SudokuBaseForm();
            SixBySixForm = new Sudoku6x6Form();
            OptionsDialog = new OptionsForm();

            // The first form created is the main form, so closing it ends the program.
            Application.Run(MainForm);
        }
    }
}
