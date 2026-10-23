using System.Windows.Forms;
using System.Runtime.InteropServices;
using System;

namespace SudokuProject
{
    internal static class NativeSolver
    {
        [DllImport("sudoku.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int sudoku_init_config(int size);

        [DllImport("sudoku.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int sudoku_solve([In, Out] int[] board);
    }

    /// <summary>
    /// Use the C-language DLL's recursive backtracking solver.
    /// </summary>
    /// <remarks>
    /// </remarks>
    internal sealed class Backtrack_C: BacktrackBase
    {
        private int[] board = new int[81]; // 0-based, 0 for no value, 0..80 for the digits in the puzzle

        // Set up and do the backtracking, and put the results back into the board.
        // Return TRUE for complete, FALSE (and a message) for failure.
        public override bool DoAll()
        {
            // send the size to the DLL
            NativeSolver.sudoku_init_config(numRowsCols);

            // Set up the board linear array of current values, 0 for no value.
            FillBoard();

            try
            {
                int result = NativeSolver.sudoku_solve(board);

                if (result == 1)
                {
                    // transfer the results back to the puzzle
                    TransferBack();
                    return true;
                }
                else
                {
                    Dialogs.ShowMessage("The backtracking method was not successful.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Dialogs.ShowMessage($"An error occurred while solving the puzzle: {ex.Message}");
                return false;
            }
        }

        // Set up the initial Board array of current values, 0 for no value.
        private void FillBoard()
        {
            int i = 0;
            string s;

            for (int row = 0; row < numRowsCols; row++)
            {
                for (int col = 0; col < numRowsCols; col++)
                {
                    board[i] = 0;
                    Cell cell = pCells[row, col];
                    if (cell.IsResultVisible)
                    {
                        s = cell.ResultText();
                        if (s.Length > 0)
                            board[i] = int.Parse(s);
                    }
                    i++;
                }
            }
        }


        // Transfer results back to our UI from the solved board.
        private void TransferBack()
        {
            int i = 0;

            for (int row = 0; row < numRowsCols; row++)
            {
                for (int col = 0; col < numRowsCols; col++)
                {
                    Cell cell = pCells[row, col];
                    if (!cell.IsResultVisible)
                        cell.SetResult(board[i].ToString(), Globals.Solved);
                    i++;
                }
            }
        }
    }
}
