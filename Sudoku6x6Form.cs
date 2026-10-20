using System;
using System.Windows.Forms;

namespace SudokuProject
{
    /// <summary>
    /// The 6x6 Sudoku window (Delphi: TSudoku6x6Form in Sudoku6x6.pas / Sudoku6x6.dfm).
    /// Inherits all the controls and behaviour of the 9x9 form, and re-positions the controls
    /// in Sudoku6x6Form.Designer.cs, like the "inherited" form in the .dfm.
    /// </summary>
    public partial class Sudoku6x6Form : SudokuBaseForm
    {
        public Sudoku6x6Form()
        {
            InitializeComponent();
        }

        // Delphi: FormCreate6x6. In the .dfm this replaced the inherited form's OnCreate handler
        // (OnCreate = FormCreate6x6), so here it overrides the base form's Load handler instead.
        protected override void FormCreate(object sender, EventArgs e)
        {
            NumRowsCols = 6;
            FinishCreate();
        }

        // Delphi: OnClose = FormClose
        private void FormClose(object sender, FormClosedEventArgs e)
        {
            Program.MainForm.Close();   // otherwise the hidden 9x9 form stays running invisibly
        }

        // Convert a row and column number to a box number and box position - both values 1 to NumRowsCols.
        // Rules for this a 6x6 Sudoku:
        //     Box number 1 is top left 3 columns x top 2 rows, box 2 is the 3 columns to
        //     the right of that, box 3 is the left 3 columns and rows 3-4, etc.
        // Box position is the sequential position 1-NumRowsCols within the box.
        protected override void RowColToBox(int row, int col, out int boxNum, out int boxPos)
        {
            // first calc the box number from two parts
            int rowPart = (row + 1) / 2;   // gives 1, 2 or 3 for top 2 rows, centre 2 rows, and bottom 2 rows respectively
            int colPart = (col + 2) / 3;   // gives 1 or 2 for left 3 columns and right 3 columns respectively

            boxNum = (rowPart - 1) * 2 + colPart;

            // now calc the position within the box from two parts
            rowPart = (((row - 1) % 2) + 1) * 3 - 2;   // gives 1 for rows 1, 3 and 5, and 4 for rows 2, 4 and 6
            colPart = ((col - 1) % 3) + 1;             // gives 1 for columns 1 and 4, 2 for cols 2 and 5, and 3 for cols 3 and 6

            boxPos = rowPart + (colPart - 1);
        }
    }
}
