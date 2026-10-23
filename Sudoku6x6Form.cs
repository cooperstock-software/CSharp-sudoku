using System;
using System.Windows.Forms;

namespace SudokuProject
{
    /// <summary>
    /// The 6x6 Sudoku window.
    /// Inherits all the controls and behaviour of the 9x9 form, and re-positions the controls
    /// in Sudoku6x6Form.Designer.cs, like the "inherited" form in the .dfm.
    /// </summary>
    public partial class Sudoku6x6Form : SudokuBaseForm
    {
        public Sudoku6x6Form()
        {
            InitializeComponent();
        }

        // Override the base form's Load handler to set the size.
        protected override void FormCreate(object sender, EventArgs e)
        {
            NumRowsCols = 6;
            FinishCreate();
        }

        private void FormClose(object sender, FormClosedEventArgs e)
        {
            Program.MainForm.Close();   // otherwise the hidden 9x9 form stays running invisibly
        }

        // Convert a row and column number to a box number and box position - both values 0 to NumRowsCols - 1.
        // Rules for this a 6x6 Sudoku:
        //     Box number 0 is top left 3 columns x top 2 rows, box 1 is the 3 columns to
        //     the right of that, box 2 is the left 3 columns and rows 3-4, etc.
        // Box position is the sequential position 0 to NumRowsCols - 1 within the box.
        protected override void RowColToBox(int row, int col, out int boxNum, out int boxPos)
        {
            // first calc the box number from two parts
            int rowPart = row / 2;   // gives 0, 1 or 2 for top 2 rows, centre 2 rows, and bottom 2 rows respectively
            int colPart = col / 3;   // gives 0 or 1 for left 3 columns and right 3 columns respectively

            boxNum = rowPart * 2 + colPart;

            // now calc the position within the box from two parts
            rowPart = (row % 2) * 3;   // gives 0 for rows 0, 2 and 4, and 3 for rows 1, 3 and 5
            colPart = col % 3; // gives 0 for columns 0 and 3, 1 for cols 1 and 4, and 2 for cols 2 and 5

            boxPos = rowPart + colPart;
        }
    }
}
