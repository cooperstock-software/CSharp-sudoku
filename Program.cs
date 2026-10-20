using System;
using System.Windows.Forms;

namespace SudokuProject
{
    /// <summary>
    /// Application entry point. Replaces SudokuProject.dpr.
    /// </summary>
    internal static class Program
    {
        // In Delphi these were the global form variables created by Application.CreateForm:
        //   SudokuBaseForm, Sudoku6x6Form and OptionsForm.
        internal static SudokuBaseForm MainForm { get; private set; }
        internal static Sudoku6x6Form SixBySixForm { get; private set; }
        internal static OptionsForm OptionsDialog { get; private set; }

        [STAThread]
        private static void Main()
        {
            // The Delphi layout is built from fixed pixel positions (the grid cells are placed
            // at hard-coded coordinates), so run DPI-unaware to keep that layout exact on
            // high-DPI displays. Windows scales the whole window for us.
            // Change to HighDpiMode.SystemAware if you prefer crisp text and rework the layout.
            Application.SetHighDpiMode(HighDpiMode.DpiUnaware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Delphi called ReadIni from each form's FormCreate; once at startup is equivalent.
            Globals.ReadIni();

            // Application.CreateForm(TSudokuBaseForm, SudokuBaseForm);
            // Application.CreateForm(TSudoku6x6Form, Sudoku6x6Form);
            // Application.CreateForm(TOptionsForm, OptionsForm);
            MainForm = new SudokuBaseForm();
            SixBySixForm = new Sudoku6x6Form();
            OptionsDialog = new OptionsForm();

            // The first form created is the main form, so closing it ends the program.
            Application.Run(MainForm);
        }
    }
}
