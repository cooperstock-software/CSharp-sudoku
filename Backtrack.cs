using System.Windows.Forms;

namespace SudokuProject
{
    /// <summary>
    /// The fast recursive backtracking solver.
    /// </summary>
    /// <remarks>
    /// rowMasks[i] is a bitmask, with information about row number i, with the j-th right-most
    /// bit being either a 0 (if the digit j doesn't already appear in that row) or a 1
    /// (if the digit j does already appear in that row).
    /// So the right-most bit is for digit 1, the next right-most bit for digit 2, etc.
    /// Similarly, colMasks[i] refers to column number i, and boxMasks[i] refers to box number i.
    /// These values can be:
    ///    tested with (maskValue & bit(digit)) == 0, which returns true if the appropriate digit is a 0,
    ///    set to mark them as being already present with maskValue |= bit(digit), or
    ///    cleared to mark them as not already present with maskValue &= ~bit(digit).
    /// Arrays are 0-based unlike the Delphi source it was converted from.
    /// </remarks>
    internal sealed class Backtrack: BacktrackBase
    {
        private readonly uint[] rowMasks = new uint[9];
        private readonly uint[] colMasks = new uint[9];
        private readonly uint[] boxMasks = new uint[9];

        private readonly int[,] board = new int[9, 9];

        public override bool DoAll()
        {
            SetupBoard();

            InitMasks();

            if (!SolveSudoku())
            {
                Dialogs.ShowMessage("The backtracking method was not successful.");
                return false;
            }

            // transfer the results back to the puzzle
            TransferBack();
            return true;
        }

        // Set up the initial Board array of current values, 0 for no value.
        private void SetupBoard()
        {
            for (int row = 0; row < numRowsCols; row++)
            {
                for (int col = 0; col < numRowsCols; col++)
                {
                    board[row, col] = 0;
                    Cell cell = pCells[row, col];
                    if (cell.IsResultVisible)
                    {
                        string s = cell.ResultText();
                        if (s.Length > 0)
                            board[row, col] = int.Parse(s);
                    }
                }
            }
        }

        // Initialize masks from the starting board
        private void InitMasks()
        {
            for (int row = 0; row < numRowsCols; row++)
            {
                rowMasks[row] = 0;
                colMasks[row] = 0;
                boxMasks[row] = 0;
            }

            for (int row = 0; row < numRowsCols; row++)
            {
                for (int col = 0; col < numRowsCols; col++)
                {
                    int digit = board[row, col];
                    if (digit > 0)
                    {
                        int box = pCells[row, col].BoxNum;
                        uint b = Bit(digit);
                        rowMasks[row] |= b;
                        colMasks[col] |= b;
                        boxMasks[box] |= b;
                    }
                }
            }
        }

        // Main new backtracking solving function - recursive!
        // Return true for success, false for failure.
        private bool SolveSudoku()
        {
            if (!FindEmpty(out int row, out int col))
                return true;   // solved

            Cell cell = pCells[row, col];
            int box = cell.BoxNum;

            for (int digit = 1; digit <= numRowsCols; digit++)
            {
                uint b = Bit(digit);

                if ((rowMasks[row] & b) == 0 &&
                    (colMasks[col] & b) == 0 &&
                    (boxMasks[box] & b) == 0)
                {
                    // Place digit
                    board[row, col] = digit;
                    // Can take up to 10 times longer if we include the SetResult calls here!
                    if (Globals.ShowAttempts)
                    {
                        cell.SetResult(digit.ToString(), Globals.Solved);
                        Application.DoEvents();
                    }

                    rowMasks[row] |= b;
                    colMasks[col] |= b;
                    boxMasks[box] |= b;

                    if (SolveSudoku())
                        return true;

                    // Undo
                    board[row, col] = 0;
                    if (Globals.ShowAttempts)
                    {
                        cell.SetResult("", Globals.Solved);
                        Application.DoEvents();
                    }
                    rowMasks[row] &= ~b;
                    colMasks[col] &= ~b;
                    boxMasks[box] &= ~b;
                }
            }

            return false;
        }

        // Find next empty cell and fill it into foundRow and foundCol.
        // Return true if there is one, false if not.
        private bool FindEmpty(out int foundRow, out int foundCol)
        {
            for (int row = 0; row < numRowsCols; row++)
            {
                for (int col = 0; col < numRowsCols; col++)
                {
                    if (board[row, col] == 0)
                    {
                        foundRow = row;
                        foundCol = col;
                        return true;
                    }
                }
            }

            foundRow = 0;
            foundCol = 0;
            return false;
        }

        // Transfer results back to our UI from the solved board.
        private void TransferBack()
        {
            for (int row = 0; row < numRowsCols; row++)
            {
                for (int col = 0; col < numRowsCols; col++)
                {
                    Cell cell = pCells[row, col];
                    if (!cell.IsResultVisible)
                        cell.SetResult(board[row, col].ToString(), Globals.Solved);
                }
            }
        }

        // Return a bit mask with the bit for the given digit set.
        private static uint Bit(int d) => 1u << (d - 1);
    }
}
