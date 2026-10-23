using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace SudokuProject
{
    /// <summary>
    /// The 9x9 Sudoku window.
    /// The 6x6 window (Sudoku6x6Form) inherits from this one.
    /// Controls and event wiring live in SudokuBaseForm.Designer.cs.
    /// </summary>
    /// <remarks>
    /// Arrays are 0-based unlike the Delphi source it was converted from.
    /// </remarks>
    public partial class SudokuBaseForm : Form
    {
        private static string CurrentDir;          // the directory we started in

        protected string saveFile;                // last loaded file, for the Save / Load dialogs

        // Public state. These are fields, not properties, so the Windows
        // Forms designer never tries to serialize them.
        public int ClickedRow = -1, ClickedCol = -1;        // the highlighted square, -1 if none
        public long PausesDuration;               // milliseconds spent paused / waiting for the user
        public Cell[,] Cells = new Cell[9, 9];      // [row, col]
        public Cell[,] Columns = new Cell[9, 9];    // [col, row]
        public Cell[,] Boxes = new Cell[9, 9];      // [boxNum, position within box]
        public bool IsStarted, Stepping;
        public int NumRowsCols;                   // 9 or 6, for the different puzzle sizes

        public SudokuBaseForm()
        {
            InitializeComponent();
        }

        // ------------------------------------------------------------------------------------
        // Creation
        // ------------------------------------------------------------------------------------

        // Form create handler. The 6x6 form replaces it (see Sudoku6x6Form.FormCreate).
        protected virtual void FormCreate(object sender, EventArgs e)
        {
            NumRowsCols = 9;
            FinishCreate();
        }

        protected void FinishCreate()
        {
            if (DesignMode) return;   // don't build the grid inside the Visual Studio designer

            // (ReadIni now happens once in Program.Main.)
            Globals.UseC = false;     // use our C DLL for the backtracking (not sure I will code for this)

            IsStarted = false;

            // (KeyPreview = true, so the form sees KeyPress events before any controls, is set
            // in the designer.)

            CurrentDir = Directory.GetCurrentDirectory();

            for (int row = 0; row < NumRowsCols; row++)
            {
                for (int col = 0; col < NumRowsCols; col++)
                {
                    RowColToBox(row, col, out int boxNum, out int boxPos);
                    Cell cell = new(this, row, col, boxNum);

                    Cells[row, col] = cell;
                    Columns[col, row] = cell;
                    try
                    {
                        Boxes[boxNum, boxPos] = cell;
                    }
                    catch (IndexOutOfRangeException)
                    {
                        Dialogs.ShowMessage("error");
                    }
                }
            }
        }

        // ------------------------------------------------------------------------------------
        // Marking results
        // ------------------------------------------------------------------------------------

        // After entering a valid result digit key in square row, col, bubble it out: remove it from the
        // 1 to NumRowsCols digits that are considered possible in all other squares in its row, column and block
        public void BubbleResult(int row, int col, string key)
        {
            if (key.Length == 0) return;

            int digit = int.Parse(key);

            // check in this row
            for (int i = 0; i < NumRowsCols; i++)
            {
                Cell cell = Cells[row, i];
                if (i != col)
                    cell.SetPossible(digit, Globals.Impossible);
            }

            // check in this column
            for (int i = 0; i < NumRowsCols; i++)
            {
                Cell cell = Cells[i, col];
                if (i != row)
                    cell.SetPossible(digit, Globals.Impossible);
            }

            // check in this 3x3 or 2x3 box
            int boxNum = Cells[row, col].BoxNum;
            for (int i = 0; i < NumRowsCols; i++)
            {
                Cell cell = Boxes[boxNum, i];
                if (cell.Row != row && cell.Col != col)
                    cell.SetPossible(digit, Globals.Impossible);
            }
        }

        // Check whether key is a valid result in the square row, col - i.e. it
        // isn't already present in that square's row, column or 3x3 box.
        // If verbose, give a message if it's not allowed.
        // Return true if it's allowed, false if not.
        public bool CheckAllowedResult(int row, int col, string key, bool verbose)
        {
            if (key.Length == 0)   // clearing is always allowed
                return true;

            // check in this row
            for (int i = 0; i < NumRowsCols; i++)
            {
                Cell cell = Cells[row, i];
                if (i != col && cell.IsResultVisible && cell.ResultText() == key)
                {
                    if (verbose)
                        Dialogs.ShowMessage("You cannot put a " + key +
                            " in this square, because it already occurs in another " +
                            "square in the same row.");
                    return false;
                }
            }

            // check in this column
            for (int i = 0; i < NumRowsCols; i++)
            {
                Cell cell = Cells[i, col];
                if (i != row && cell.IsResultVisible && cell.ResultText() == key)
                {
                    if (verbose)
                        Dialogs.ShowMessage("You cannot put a " + key +
                            " in this square, because it already occurs in another " +
                            "square in the same column.");
                    return false;
                }
            }

            // check in this 3x3 or 3x2 box
            int boxNum = Cells[row, col].BoxNum;
            for (int i = 0; i < NumRowsCols; i++)
            {
                Cell cell = Boxes[boxNum, i];
                if (cell.Row != row && cell.Col != col && cell.IsResultVisible &&
                    cell.ResultText() == key)
                {
                    if (verbose)
                        Dialogs.ShowMessage("You cannot put a " + key +
                            " in this square, because it already occurs in another " +
                            "square in the same " +
                            (NumRowsCols == 9 ? "3 x 3" : "3 x 2") + " box.");
                    return false;
                }
            }

            return true;   // it's OK!
        }

        // Clear the selected colour in whichever square (if any) is currently highlighted
        public void ClearSelection()
        {
            if (ClickedRow >= 0 && ClickedCol >= 0)
                Cells[ClickedRow, ClickedCol].ResetColor();
        }

        // ------------------------------------------------------------------------------------
        // Keyboard
        // ------------------------------------------------------------------------------------

        // This sees keys before any control does, so it can process them, because normal controls on
        // the form like buttons otherwise swallow and act on keys like Tab.
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (FormShortCut(keyData))
                return true; // this says the key has been handled

            return base.ProcessCmdKey(ref msg, keyData);
        }

        // Handle navigation keys. Returns true if the key was handled.
        private bool FormShortCut(Keys keyData)
        {
            Keys c = keyData & Keys.KeyCode;
            bool shifted = (keyData & Keys.Shift) == Keys.Shift;

            if (c is not 
                (Keys.Tab or Keys.Down or Keys.Up or Keys.Right or Keys.Left or
                 Keys.PageDown or Keys.PageUp or Keys.Home or Keys.End or Keys.Delete))
                return false;

            // the first key press just selects square 0,0.
            if (ClickedRow < 0 || ClickedCol < 0)
            {
                if (c != Keys.Delete)
                    Cells[0, 0].FrameClick(this, EventArgs.Empty);
                return true;
            }

            int newRow = ClickedRow;
            int newCol = ClickedCol;

            switch (c)
            {
                // Delete: same as backspace, clear the result in the square
                case Keys.Delete:
                    Cell cell = Cells[ClickedRow, ClickedCol];
                    if (IsStarted)
                    {
                        if (!cell.IsResultVisible) return true;          // only small digits showing; nothing to clear
                        if (cell.IsOriginal) { Beep(); return true; }    // same rule as typing over an original digit
                    }
                    cell.SetResult("", !(IsStarted || cell.AreDigitsVisible));
                    return true;

                // Tab: walk forward through the rows and columns, going from the end of one row
                //      to the start of the next, and from the end of the last row to the top.
                // Shift-Tab: Same in reverse.
                case Keys.Tab:
                    if (!shifted)
                    {
                        newCol++;
                        if (newCol >= NumRowsCols)
                        {
                            newCol = 0;
                            newRow++;
                            if (newRow >= NumRowsCols)
                                newRow = 0;
                        }
                    }
                    else
                    {
                        newCol--;
                        if (newCol < 0)
                        {
                            newCol = NumRowsCols - 1;
                            newRow--;
                            if (newRow < 0)
                                newRow = NumRowsCols - 1;
                        }
                    }
                    Cells[newRow, newCol].FrameClick(this, EventArgs.Empty);
                    return true;

                // Arrow keys move in expected directions but don't wrap
                case Keys.Down:
                    if (newRow < NumRowsCols - 1)
                    {
                        newRow++;
                        Cells[newRow, newCol].FrameClick(this, EventArgs.Empty);
                    }
                    else
                        Beep();
                    return true;

                case Keys.Up:
                    if (newRow > 0)
                    {
                        newRow--;
                        Cells[newRow, newCol].FrameClick(this, EventArgs.Empty);
                    }
                    else
                        Beep();
                    return true;

                case Keys.Right:
                    if (newCol < NumRowsCols - 1)
                    {
                        newCol++;
                        Cells[newRow, newCol].FrameClick(this, EventArgs.Empty);
                    }
                    else
                        Beep();
                    return true;

                case Keys.Left:
                    if (newCol > 0)
                    {
                        newCol--;
                        Cells[newRow, newCol].FrameClick(this, EventArgs.Empty);
                    }
                    else
                        Beep();
                    return true;

                // PgUp/PgDn go to top or bottom
                case Keys.PageDown:
                    newRow = NumRowsCols - 1;
                    Cells[newRow, newCol].FrameClick(this, EventArgs.Empty);
                    return true;

                case Keys.PageUp:
                    newRow = 0;
                    Cells[newRow, newCol].FrameClick(this, EventArgs.Empty);
                    return true;

                // Home/End go to start or end of line
                case Keys.End:
                    newCol = NumRowsCols - 1;
                    Cells[newRow, newCol].FrameClick(this, EventArgs.Empty);
                    return true;

                case Keys.Home:
                    newCol = 0;
                    Cells[newRow, newCol].FrameClick(this, EventArgs.Empty);
                    return true;
            }

            return false;
        }

        private static void Beep()
        {
            System.Media.SystemSounds.Beep.Play();
        }

        // Catch digits being typed to put them into a cell. Do it here, and pass it through to the cell,
        // because the cell control doesn't get the key events (the form has KeyPreview on).
        // Also handles backspace to clear a result digit.
        private void FormKeyPress(object sender, KeyPressEventArgs e)
        {
            char key = e.KeyChar;

            if (ClickedRow < 0 || ClickedCol < 0) return;

            Cell cell = Cells[ClickedRow, ClickedCol];

            // Can't change original values once started
            if (IsStarted && cell.IsResultVisible && cell.IsOriginal)
            {
                Beep();
                return;
            }

            if (key >= '1' && (key <= '6' || (NumRowsCols == 9 && key <= '9')))
            {
                string sKey = key.ToString();
                if (CheckAllowedResult(ClickedRow, ClickedCol, sKey, Globals.Verbose))
                {
                    cell.SetResult(sKey, !(IsStarted || cell.AreDigitsVisible));
                    Application.DoEvents();
                    if (IsPuzzleSolved())
                        Dialogs.ShowMessage("You have solved the puzzle!");
                }
            }
            else if (key == '\b') // Backspace - clear the result
            {
                cell.SetResult("", !(IsStarted || cell.AreDigitsVisible));
            }
        }

        // ------------------------------------------------------------------------------------
        // Solving
        // ------------------------------------------------------------------------------------

        // Follow a bunch of steps to solve the puzzle.
        // Optionally stop between each step with messages, if stepping.
        private void FullSolve()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            Cursor.Current = Cursors.WaitCursor;
            Application.UseWaitCursor = true;
            try
            {
                bool backtrack = false;
                bool done = false;
                bool stop = false;
                bool nonePossible = false;

                PrepareForSolve();

                if (Stepping)
                {
                    if (!Dialogs.Confirm("Did preparation step - reset possible and impossible " +
                            "digits based on currently solved squares." +
                            "\n\n" + "Continue stepping?"))
                        return;
                }

                if (Globals.Method != SolveMethod.OnlyBacktrack)
                    MainSolveSteps(ref done, ref stop, ref nonePossible);

                if (!(done || stop))
                {
                    if (Globals.Method != SolveMethod.SimplePromptBacktrack) backtrack = true;
                    if (!backtrack)
                    {
                        Stopwatch stopwatch2 = Stopwatch.StartNew();
                        backtrack = Dialogs.Confirm(
                            "Sorry, the simple solving rules in this program were not able to completely solve the puzzle." +
                            "\n\n" +
                            "Would you like to have the program use its backtracking approach of trying all remaining possibilities?");
                        stopwatch2.Stop();
                        PausesDuration += stopwatch2.ElapsedMilliseconds;
                    }
                    if (backtrack)
                    {
                        Stepping = false;

                        if (!Globals.UseC && Globals.ShowAttempts)
                        {
                            Stopwatch stopwatch2 = Stopwatch.StartNew();
                            if (!Dialogs.Confirm(
                                "Showing the attempted digits when using the code for the backtracking " +
                                "approach can make solving it extremely slow!" + "\n\n" +
                                "Are you sure you want to show the attempted digits?"))
                                Globals.ShowAttempts = false;
                            stopwatch2.Stop();
                            PausesDuration += stopwatch2.ElapsedMilliseconds;
                        }

                        BacktrackBase backtracker;
                        backtracker = new Backtrack(); // or new Backtrack_C() if you want to use the C DLL version
                        backtracker.Initialize(this);
                        backtracker.DoAll();
                    }
                    else
                    {
                        Dialogs.ShowMessage("You can try solving the rest of it manually.");
                        Globals.ReportDuration = false;
                    }
                }

                if (IsPuzzleSolved())
                {
                    stopwatch.Stop();
                    long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
                    long duration = elapsedMilliseconds - PausesDuration;
                    string message = "The puzzle has been successfully solved.";
                    if (Globals.ReportDuration && !Stepping)
                    {
                        message += "\n\n" + "That took " + (duration / 1000.0).ToString("0.00") + " seconds";
                        if (PausesDuration > 0) message += ", ignoring any pauses";
                        message += ".";
                    }
                    Dialogs.ShowMessage(message);
                }
            }
            finally
            {
                Application.UseWaitCursor = false;
                Cursor.Current = Cursors.Default;
            }
        }

        // Do the two regular solving steps in a loop, until nothing further can be done.
        // Return whether anything was solved during these steps, and set all of done,
        // stop and nonePossible.
        // Return immediately if stop or nonePossible are set.
        private bool MainSolveSteps(ref bool done, ref bool stop, ref bool nonePossible)
        {
            bool solvedSomething;

            do
            {
                solvedSomething = SolveSingletonSquares(out nonePossible);
                done = IsPuzzleSolved();
                if (!done && Stepping)
                {
                    if (!Dialogs.Confirm("Did step to solve any squares with only one possible digit in them." +
                            "\n\n" + "Continue stepping?"))
                        return solvedSomething;
                }

                if (done || nonePossible)
                    return solvedSomething;

                if (SolveRowsColsBoxes(out stop))
                    solvedSomething = true;
                done = IsPuzzleSolved();
                if (done || stop || nonePossible)
                    return solvedSomething;
            } while (solvedSomething);

            return solvedSomething;
        }

        // Show the 1-NumRowsCols digits in all squares that don't have a result digit in
        // them or already have the 1-NumRowsCols digits in them
        private void ManualPlayButtonClick(object sender, EventArgs e)
        {
            for (int row = 0; row < NumRowsCols; row++)
            {
                for (int col = 0; col < NumRowsCols; col++)
                {
                    Cell cell = Cells[row, col];
                    if (!(cell.IsResultVisible || cell.AreDigitsVisible))
                        cell.ShowDigits();
                }
            }

            PrepareForSolve();   // re-bubble all of the results

            // TODO
            SolveButton.Enabled = true;
            //cb_step.Enabled = TRUE;

            IsStarted = true;
        }

        private void OptionsButtonClick(object sender, EventArgs e)
        {
            Program.OptionsDialog.ShowDialog(this);
        }

        // Return one of the three arrays, based on a passed-in type value
        private Cell[,] GetArrayOfType(ArrayType aType)
        {
            return aType switch
            {
                ArrayType.RowType => Cells,
                ArrayType.ColType => Columns,
                ArrayType.BoxType => Boxes,
                _ => null,// can't really happen
            };
        }

        // Reset the colours on all squares to the standard white or silver
        private void ResetBackColors()
        {
            for (int row = 0; row < NumRowsCols; row++)
                for (int col = 0; col < NumRowsCols; col++)
                    Cells[row, col].ResetColor();
        }

        // Convert a row and column number to a box number and box position - both values 0 to NumRowsCols - 1.
        // Rules for this a 9x9 Sudoku:
        //    Box number 0 is the top-left 9 squares, box 1 is the top-centre 9,
        //    box 2 is the top-right 9, box 3 is the middle-left 9, etc.
        // Box position is the sequential position 0 to NumRowsCols - 1 within the box.
        protected virtual void RowColToBox(int row, int col, out int boxNum, out int boxPos)
        {
            // first calc the box number from two parts
            int rowPart = row / 3;   // gives 0, 1 or 2 for top 3 rows, centre 3 rows, and bottom 3 rows respectively
            int colPart = col / 3;   // gives 0, 1 or 2 for left 3 columns, centre 3 columns and right 3 columns respectively

            boxNum = rowPart * 3 + colPart;

            // now calc the position within the box from two parts
            rowPart = (row % 3) * 3; // gives 0 for rows 0,3,6, 3 for rows 1,4,7, and 6 for rows 2,5,8
            colPart = col % 3; // gives 0 for columns 0,3,6, 1 for cols 1,4,7, and 2 for cols 2,5,8

            boxPos = rowPart + colPart;
        }

        // Follow a bunch of steps to solve the puzzle
        private void SolveButtonClick(object sender, EventArgs e)
        {
            Stepping = false;

            if (!IsStarted) ManualPlayButtonClick(sender, e);

            FullSolve();
        }

        // When passed in an array where the first index is rows, columns, or boxes, work
        // through row/col/box number firstIndex, looking for an unsolved square where
        // the digit only occurs in one of the NumRowsCols squares, and if found, solve it.
        // Return whether or not anything was solved.
        private bool SolveRowColBox(Cell[,] anArray, int firstIndex, int digit)
        {
            bool solvedOne = false;
            int numOccurrences = 0;
            int occurrenceIndex = 0;

            for (int secondIndex = 0; secondIndex < NumRowsCols; secondIndex++)
            {
                Cell cell = anArray[firstIndex, secondIndex];
                if (!cell.AreDigitsVisible) continue;
                if (cell.IsPossible(digit))
                {
                    numOccurrences++;
                    if (numOccurrences > 1) break;
                    occurrenceIndex = secondIndex;
                }
            }

            if (numOccurrences == 1)
            {
                Cell cell = anArray[firstIndex, occurrenceIndex];
                cell.FrameClick(this, EventArgs.Empty);
                cell.SetResult(digit.ToString(), Globals.Solved);
                Application.DoEvents();
                PauseAfterSolving();

                solvedOne = true;
            }

            return solvedOne;
        }

        // Short pause so the person can follow along (the "short pauses" option); the time spent
        // doesn't count towards the reported solving time.
        private void PauseAfterSolving()
        {
            if (!Globals.DoSleeps) return;

            int milliseconds = Stepping ? 500 : 200;
            Thread.Sleep(milliseconds);
            PausesDuration += milliseconds;
        }

        // For all rows, all columns, and all boxes, look for an unsolved digit in that
        // row/col/box that only occurs once, and solve it.
        // Return whether any were solved.
        // Set stop to true if you are stepping, you give progress message, and they ask to stop.
        private bool SolveRowsColsBoxes(out bool stop)
        {
            string[] typeNames = [null, "row", "column", "box"];
            string[] extraText = [null, "", "", " (counting across the boxes then down)"];

            stop = false;
            bool solvedAny = false;

            for (int t = (int)ArrayType.RowType; t <= (int)ArrayType.BoxType; t++)
            {
                ArrayType aType = (ArrayType)t;
                Cell[,] myCells = GetArrayOfType(aType);
                for (int firstIndex = 0; firstIndex < NumRowsCols; firstIndex++)
                {
                    for (int digit = 1; digit <= NumRowsCols; digit++)
                        if (SolveRowColBox(myCells, firstIndex, digit))
                            solvedAny = true;

                    if (!IsPuzzleSolved() && Stepping)
                    {
                        if (!Dialogs.Confirm("Did step to solve any squares where one digit was only " +
                                "possible in one square of " + typeNames[t] + " " +
                                (firstIndex + 1) + extraText[t] +
                                ".\n\nContinue stepping?"))
                        {
                            stop = true;
                            return solvedAny;
                        }
                    }
                }
            }

            return solvedAny;
        }

        // Solve any squares showing the 1-NumRowsCols digits, where only one of those
        // digits is bold (i.e. possible).
        // Also set nonePossible and return false if you hit a square with no possible
        // values (can occur when doing backtracking).
        // Return whether or not any squares were solved.
        private bool SolveSingletonSquares(out bool nonePossible)
        {
            nonePossible = false;
            bool solvedOne = false;

            for (int row = 0; row < NumRowsCols; row++)
            {
                for (int col = 0; col < NumRowsCols; col++)
                {
                    Cell cell = Cells[row, col];
                    if (!cell.AreDigitsVisible) continue;

                    int numVisible = 0;
                    int visibleOne = 0;
                    for (int digit = 1; digit <= NumRowsCols; digit++)
                    {
                        if (cell.IsPossible(digit))
                        {
                            numVisible++;
                            if (numVisible > 1) break;
                            visibleOne = digit;
                        }
                    }

                    if (numVisible == 1)
                    {
                        cell.FrameClick(this, EventArgs.Empty);
                        cell.SetResult(visibleOne.ToString(), Globals.Solved);
                        Application.DoEvents();
                        PauseAfterSolving();
                        solvedOne = true;
                    }
                    else if (numVisible == 0)
                    {
                        nonePossible = true;
                        return false;
                    }
                }
            }

            return solvedOne;
        }

        // ------------------------------------------------------------------------------------
        // Buttons
        // ------------------------------------------------------------------------------------

        // Switch to the other size of sudoku (after confirmation, if the puzzle isn't empty)
        private void SwitchButtonClick(object sender, EventArgs e)
        {
            if (!IsPuzzleClear())
            {
                if (!Dialogs.Confirm(
                    "There are already results in the puzzle. Are you sure you want to switch to the other size?"))
                    return;
            }

            if (NumRowsCols == 9)
            {
                Program.MainForm.Hide();
                Program.SixBySixForm.Show();
            }
            else
            {
                Program.SixBySixForm.Hide();
                Program.MainForm.Show();
            }
        }

        // Return true if no squares of the puzzle have anything in them (it's "clear"), false otherwise.
        private bool IsPuzzleClear()
        {
            for (int row = 0; row < NumRowsCols; row++)
            {
                for (int col = 0; col < NumRowsCols; col++)
                {
                    if (Cells[row, col].IsResultVisible)
                        return false;
                }
            }

            return true;
        }

        // Return whether or not the puzzle is solved
        public bool IsPuzzleSolved()
        {
            for (int row = 0; row < NumRowsCols; row++)
            {
                for (int col = 0; col < NumRowsCols; col++)
                {
                    Cell cell = Cells[row, col];
                    if (!(cell.IsResultVisible && cell.ResultText().Length > 0))
                        return false;
                }
            }

            return true;
        }

        // Save the currently selected results, in NumRowsCols rows of a user-selected text file,
        //   surrounded by border and separator rows.
        // Each row is NumRowsCols consecutive results from the cells in that row of the puzzle,
        //   returned from the cell's SaveValue method,
        //    with vertical bar separators before the first one, and after each one.
        // The cell value is three spaces if it doesn't have a result digit in it,
        //   or the result digit 3 times, if it's bold (an original puzzle digit), or the result
        //   digit surrounded by two spaces if not.
        private void SaveButtonClick(object sender, EventArgs e)
        {
            string pathName;

            using (SaveFileDialog saveDialog = new())
            {
                // default to saving a previous-saved (or loaded) one
                if (!string.IsNullOrEmpty(saveFile))
                {
                    saveDialog.FileName = Path.GetFileName(saveFile);
                    saveDialog.InitialDirectory = Path.GetDirectoryName(saveFile);
                }
                else
                {
                    saveDialog.FileName = "save.sud";
                    saveDialog.InitialDirectory = CurrentDir;
                }

                saveDialog.Filter = "Sudoku Files (*.sud)|*.sud";
                saveDialog.RestoreDirectory = true;    // Delphi: ofNoChangeDir
                saveDialog.OverwritePrompt = true;     // Delphi: ofOverwritePrompt
                if (saveDialog.ShowDialog(this) == DialogResult.OK)
                    pathName = saveDialog.FileName;
                else
                    return;
            }

            // build the output
            string vertical = "|";
            string borderLine = "+" + string.Concat(Enumerable.Repeat("---+", NumRowsCols));
            StringBuilder s = new();

            for (int row = 0; row < NumRowsCols; row++)
            {
                s.Append(borderLine).Append('\n');
                for (int col = 0; col < NumRowsCols; col++)
                    s.Append(vertical).Append(Cells[row, col].SaveValue());
                s.Append(vertical).Append('\n');
            }
            s.Append(borderLine).Append('\n');

            // write it out
            try
            {
                File.WriteAllText(pathName, s.ToString(), Encoding.Latin1);
                // save for re-loads or re-saves, after successful load
                saveFile = pathName;
            }
            catch (Exception ex)
            {
                Dialogs.ShowMessage(string.Format("Unable to write to file {0}.{1}Error: {2}",
                    pathName, Environment.NewLine, ex.Message));
            }
        }

        // Load a previously-saved set of results, saved by SaveButtonClick.
        // The results in NumRowsCols rows of a user-selected text file, surrounded by border and separator rows.
        // Each row is NumRowsCols consecutive results from the cells in that row of the
        //    puzzle, returned from SaveValue(),
        //    with vertical bar separators before the first one, and after each one.
        // The cell value is three spaces if it doesn't have a result digit in it,
        //   or the result digit 3 times, if it's bold (an original puzzle digit), or the
        //   result digit surrounded by two spaces if not.
        private void LoadButtonClick(object sender, EventArgs e)
        {
            string pathName;

            // first make sure we select a file, defaulting to a previously-saved one
            using (OpenFileDialog openDialog = new())
            {
                if (!string.IsNullOrEmpty(saveFile))
                {
                    openDialog.FileName = Path.GetFileName(saveFile);
                    openDialog.InitialDirectory = Path.GetDirectoryName(saveFile);
                }
                else
                {
                    openDialog.InitialDirectory = CurrentDir;
                }

                openDialog.Filter = "Sudoku Files (*.sud)|*.sud";
                openDialog.RestoreDirectory = true;    // Delphi: ofNoChangeDir
                openDialog.CheckFileExists = true;     // Delphi: ofFileMustExist
                if (openDialog.ShowDialog(this) == DialogResult.OK)
                    pathName = openDialog.FileName;
                else
                    return;
            }

            // Check we can clear the puzzle
            if (!IsPuzzleClear())
            {
                if (!Dialogs.Confirm(
                    "Is it OK to clear whatever is currently displayed, before Loading a puzzle?"))
                    return;
            }

            LoadFile(pathName);
        }

        // Clear the puzzle and open and display the passed-in file
        private void LoadFile(string pathName)
        {
            if (!File.Exists(pathName))
            {
                Dialogs.ShowMessage("Cannot open file " + pathName + " since it doesn't seem to exist.");
                return;
            }

            ClearAll();
            bool first = true;

            try
            {
                using StreamReader reader = new(pathName, Encoding.Latin1);
                int lineNo = 0;
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    lineNo++;
                    if (lineNo % 2 == 1) continue;   // skip alternate (border) lines

                    int row = lineNo / 2 - 1;

                    // Check it is for the right size puzzle - line lengths seem to be either
                    // 4 x NumRowsCols, or one higher
                    if (first)
                    {
                        first = false;
                        int lineLen = line.Length;
                        if (lineLen % 2 == 1) lineLen--;
                        int numCells = lineLen / 4;
                        if (numCells != NumRowsCols)
                        {
                            Dialogs.ShowMessage(string.Format(
                                "This appears to be a file saved from a {0} x {0} Sudoku, " +
                                "rather than a {1} x {1} one.", numCells, NumRowsCols) +
                                "\n\n" + "It cannot be loaded into this puzzle");
                            break;
                        }
                    }

                    // parse the line and fill in the results
                    for (int col = 0; col < NumRowsCols; col++)
                    {
                        string s = line.Substring(col * 4 + 1, 3);
                        char digit = s[1];
                        if (digit == ' ') continue;   // nothing in this square
                        bool isOriginal = s[0] != ' ';
                        Cells[row, col].SetResult(digit.ToString(), isOriginal);
                    }
                }

                // save for re-loads or re-saves, after successful load
                saveFile = pathName;
            }
            catch (IOException ex)
            {
                Dialogs.ShowMessage("Error reading file " + pathName + " " + ex.Message);
            }
            catch (Exception ex)
            {
                Dialogs.ShowMessage("Error processing file " + pathName + " " + ex.Message);
            }
        }

        private void ClearAll()
        {
            for (int row = 0; row < NumRowsCols; row++)
                for (int col = 0; col < NumRowsCols; col++)
                    Cells[row, col].ClearAll();

            ResetBackColors();
            ClickedRow = -1;
            ClickedCol = -1;

            //ManualPlayButton.Enabled = false;
            //SolveButton.Enabled = false;

            IsStarted = false;
            Stepping = false;

            PausesDuration = 0;
        }

        // Clear the whole puzzle back to nothing in it and no selected square
        private void ClearButtonClick(object sender, EventArgs e)
        {
            if (!IsPuzzleClear())
            {
                if (!Dialogs.Confirm(
                    "There are already results in the puzzle. Are you sure you want to clear it?"))
                    return;
            }

            ClearAll();
        }

        // Show the PDF Help file
        private void HelpButtonClick(object sender, EventArgs e)
        {
            const string pdfFile = "sudoku.pdf";

            if (!File.Exists(pdfFile))
            {
                Dialogs.ShowMessage("Cannot find Help file " + pdfFile + ".");
                return;
            }

            try
            {
                Process.Start(new ProcessStartInfo(Path.GetFullPath(pdfFile)) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                int code = (ex as Win32Exception)?.NativeErrorCode ?? 0;
                Dialogs.ShowMessage(string.Format("Error # {0} opening Help file {1}.", code, pdfFile));
            }
        }

        // Prepare for solving the puzzle by re-bubbling all results in squares across
        // their rows, down their columns, and in their boxes.
        private void PrepareForSolve()
        {
            // First reset all squares with digits in them (or that are empty)
            for (int row = 0; row < NumRowsCols; row++)
            {
                for (int col = 0; col < NumRowsCols; col++)
                {
                    Cell cell = Cells[row, col];
                    if (!(cell.IsResultVisible && cell.ResultText().Length > 0))
                    {
                        cell.ClearAll();
                        cell.ShowDigits();
                    }
                }
            }

            // Now bubble out all values in squares with results
            for (int row = 0; row < NumRowsCols; row++)
            {
                for (int col = 0; col < NumRowsCols; col++)
                {
                    Cell cell = Cells[row, col];
                    if (cell.IsResultVisible && cell.ResultText().Length > 0)
                        BubbleResult(row, col, cell.ResultText());
                }
            }
        }
    }

    /// <summary>Which set of squares to scan for the "only one place for this digit" rule.</summary>
    internal enum ArrayType
    {
        RowType = 1,
        ColType = 2,
        BoxType = 3
    }
}
