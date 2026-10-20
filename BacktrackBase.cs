namespace SudokuProject
{
    /// <summary>
    /// Common state and setup for the solvers that work on the puzzle form's squares.
    /// Descendants implement DoAll.
    /// </summary>
    internal abstract class BacktrackBase
    {
        protected SudokuBaseForm form;
        protected Cell[,] pCells;        // 1-based [row, col], shared with the form
        protected int numRowsCols;      // 9 or 6

        // Virtual so a descendant can do its own extra setup (call base.Initialize first).
        public void Initialize(SudokuBaseForm aForm)
        {
            form = aForm;
            pCells = form.Cells;
            numRowsCols = form.NumRowsCols;
        }

        /// <summary>Solve the puzzle and transfer the result to the form. Returns true on success.</summary>
        public abstract bool DoAll();
    }
}