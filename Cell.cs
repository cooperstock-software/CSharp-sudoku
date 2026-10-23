using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SudokuProject
{
    /// <summary>
    /// One square of the puzzle.
    /// It holds the nine (or six) small "possible digit" labels plus one large result label;
    /// only one of the two groups is visible at a time.
    /// The controls and their event wiring live in Cell.Designer.cs.
    /// </summary>
    public partial class Cell : UserControl
    {
        // Left position of each column, and Top position of each row, in the 9x9 puzzle
        private static readonly int[] Positions = [ 10, 76, 142, 209, 275, 341, 408, 474, 540 ];
        // Top position of each row in the 6x6 puzzle
        private static readonly int[] YPositions6x6 = [ 10, 76, 143, 209, 276, 342 ];

        // Values for the changeOther 2nd arg to the Change* functions
        private const bool ChangeOther = true;
        private const bool NoChangeOther = false;

        // Delphi told "possible" digits from "impossible" ones by the label's font style (bold vs
        // italic), and original puzzle digits from solved ones the same way (bold vs regular).
        // Shared font objects keep that cheap: the solver changes these thousands of times.
        private static readonly Font DigitPossibleFont =
            new("Tahoma", 17F, FontStyle.Bold, GraphicsUnit.Pixel, 0);
        private static readonly Font DigitImpossibleFont =
            new("Tahoma", 17F, FontStyle.Italic, GraphicsUnit.Pixel, 0);
        private static readonly Font ResultOriginalFont =
            new("Tahoma", 37F, FontStyle.Bold, GraphicsUnit.Pixel, 0);
        private static readonly Font ResultSolvedFont =
            new("Tahoma", 37F, FontStyle.Regular, GraphicsUnit.Pixel, 0);

        // clMoneyGreen ("Mint") - the highlight colour of the selected square.
        private static readonly Color SelectedColor = Color.FromArgb(0xC0, 0xDC, 0xC0);

        private readonly Label[] digits = new Label[9];   // digits[0] is Label1
        private readonly int numRowsCols;                  // 9 or 6, for the different puzzle sizes

        [Browsable(false)] public bool IsResultVisible { get; private set; }
        [Browsable(false)] public bool AreDigitsVisible { get; private set; }
        [Browsable(false)] public bool IsOriginal { get; private set; }  // is the result part of the original puzzle
        [Browsable(false)] public int Row { get; private set; }
        [Browsable(false)] public int Col { get; private set; }
        [Browsable(false)] public int BoxNum { get; private set; }

        /// <summary>Needed by the Windows Forms designer; the game always uses the other constructor.</summary>
        public Cell()
        {
            InitializeComponent();
        }

        /// <summary>Do various initialization on construction, passing in some values.</summary>
        public Cell(SudokuBaseForm owner, int row, int col, int boxNum) : this()
        {
            Row = row;
            Col = col;
            BoxNum = boxNum;
            Name = "Cell" + row + col;

            owner.Controls.Add(this);
            BringToFront(); // sit above the black background panel (Shape1)

            numRowsCols = owner.NumRowsCols;

            Left = Positions[col];
            if (numRowsCols == 9)
            {
                Top = Positions[row];
            }
            else
            {
                Top = YPositions6x6[row];
                Label4.Top = 35;
                Label5.Top = 35;
                Label6.Top = 35;
                Label7.Visible = false;
                Label8.Visible = false;
                Label9.Visible = false;
            }

            ResetColor();

            // Make the array of the numRowsCols small digit controls, for easy indexed access
            digits[0] = Label1;
            digits[1] = Label2;
            digits[2] = Label3;
            digits[3] = Label4;
            digits[4] = Label5;
            digits[5] = Label6;

            if (numRowsCols == 9)
            {
                digits[6] = Label7;
                digits[7] = Label8;
                digits[8] = Label9;
            }

            for (int i = 0; i < numRowsCols; i++)
            {
                digits[i].Tag = 1;   // 1 means possible, 0 means not possible - start assume possible
                digits[i].Visible = false;
            }
            Result.Visible = false;
        }

        // The form this square sits on.
        private SudokuBaseForm GameForm
        {
            get { return (SudokuBaseForm)Parent; }
        }

        // Flip the visibility of the 1-numRowsCols digits to isVisible.
        // If changeOther is true, also flip the result digit to be NOT isVisible.
        private void ChangeDigitsVisibility(bool isVisible, bool changeOther)
        {
            if (Globals.ShowSmallDigits)
            {
                for (int i = 0; i < numRowsCols; i++)
                    digits[i].Visible = isVisible;
            }

            AreDigitsVisible = isVisible;

            if (changeOther)
                ChangeResultVisibility(!isVisible, NoChangeOther);
        }

        private void ChangeResultVisibility(bool isVisible, bool changeOther)
        {
            Result.Visible = isVisible;
            IsResultVisible = isVisible;
            if (AreDigitsVisible == isVisible && changeOther)
                ChangeDigitsVisibility(!isVisible, NoChangeOther);
        }

        // Handle a single click on this square; tell the window which square was clicked
        public void FrameClick(object sender, EventArgs e)
        {
            GameForm.ClearSelection();

            GameForm.ClickedRow = Row;
            GameForm.ClickedCol = Col;

            BackColor = SelectedColor;
        }

        // Clear everything in this square, leaving both parts not visible, the
        // 1-numRowsCols small digits possible, and the result original
        public void ClearAll()
        {
            if (AreDigitsVisible) ChangeDigitsVisibility(false, NoChangeOther);
            if (IsResultVisible) ChangeResultVisibility(false, NoChangeOther);

            for (int i = 0; i < numRowsCols; i++)
                SetPossible(i + 1, Globals.Possible);

            // same for the result
            SetResultOriginal(Globals.Original);
        }

        // Same as clicking the empty frame
        private void ClickResult(object sender, EventArgs e)
        {
            FrameClick(sender, e);
        }

        // On a double-click on the result digit, if it's not part of the original puzzle
        // (not bold), go back to the 1-numRowsCols digits.
        // Don't show any disallowed digits as bold (but leave non-bold ones from before
        // as non-bold).
        private void DblClickResult(object sender, EventArgs e)
        {
            if (IsResultOriginal()) return;

            Result.Text = "";
            ShowDigits();

            for (int i = 0; i < numRowsCols; i++)
            {
                if (IsPossible(i + 1))
                {
                    if (!GameForm.CheckAllowedResult(Row, Col, (i + 1).ToString(), Globals.Silent))
                        SetPossible(i + 1, Globals.Impossible);
                }
            }
        }

        // Handle a single-click on one of the 1-numRowsCols small digits.
        // If it's currently a possibility (bold) make it show as not possible.
        // If it's not a possibility, make it possible (if it's allowed!).
        // Also select the square.
        private void ClickSmallNumber(object sender, EventArgs e)
        {
            string sDigit = ((Label)sender).Text;
            int digit = int.Parse(sDigit);
            bool possible = IsPossible(digit);

            if (possible || GameForm.CheckAllowedResult(Row, Col, sDigit, Globals.Verbose))
                SetPossible(digit, !possible);

            FrameClick(sender, e);
        }

        // Handle a double-click on one of the 1-numRowsCols small digits.
        // If it's allowed in that square, switch to it as a (non-bold) result.
        // Also select this square.
        private void DblClickSmallNumber(object sender, EventArgs e)
        {
            string sDigit = ((Label)sender).Text;

            if (GameForm.CheckAllowedResult(Row, Col, sDigit, Globals.Verbose))
            {
                SetResult(sDigit, Globals.Solved);
                if (GameForm.IsPuzzleSolved())
                {
                    Dialogs.ShowMessage("You have solved the puzzle!");
                }
                else
                {
                    // In the Delphi control this was converted from, a double-click
                    // delivers OnClick, OnDblClick, OnClick; Windows Forms only
                    // delivers Click, DoubleClick. The first Click toggled this digit's
                    // possible/impossible state, so replay the trailing Click to toggle it back
                    // and leave the digit's state unchanged, as it was in the Delphi program.
                    ClickSmallNumber(sender, e);
                    return;
                }
            }

            FrameClick(sender, e);
        }

        public void ResetColor()
        {
            bool isWhite;
            if (numRowsCols == 9)
                isWhite = (BoxNum % 2) == 0;
            else
                isWhite = BoxNum == 0 || BoxNum == 3 || BoxNum == 4; // bit weird in 6x6 puzzle!

            Color newColor = isWhite ? Color.White : Color.Silver;

            if (newColor.ToArgb() != BackColor.ToArgb())
                BackColor = newColor;
        }

        // Just return the character (if any) in the result label
        public string ResultText()
        {
            return Result.Text;
        }

        // Return a value for saving from this square, which is three spaces if it
        // doesn't have a result digit in it, or the result digit 3 times, if it's bold
        // (an original puzzle digit), or the result digit surrounded by two spaces if not.
        public string SaveValue()
        {
            if (!IsResultVisible)
                return "   ";

            string s = Result.Text;

            if (IsResultOriginal())
                return string.Concat(Enumerable.Repeat(s, 3));   // DupeString(s, 3)

            return " " + s + " ";
        }

        // Set the digit in the small 1-numRowsCols digits as possible (if possible is
        // true or POSSIBLE) or impossible (if false or IMPOSSIBLE),
        // by changing its bold and italic settings
        public void SetPossible(int digit, bool possible)
        {
            if (!Globals.ShowSmallDigits)
            {
                digits[digit - 1].Tag = possible ? 1 : 0;
                return;
            }

            digits[digit - 1].Font = possible ? DigitPossibleFont : DigitImpossibleFont;
        }

        // Set the result to be displayed, with the value key.
        // If isOriginal is true (value ORIGINAL), it's part of the original entered puzzle.
        // If false (value SOLVED) it's not.
        public void SetResult(string key, bool isOriginal)
        {
            if (!IsResultVisible) ShowResult();

            SetResultOriginal(isOriginal && key.Length > 0);
            Result.Text = key;

            if (key.Length == 0 && GameForm.IsStarted) ShowDigits();

            if (key.Length > 0) GameForm.BubbleResult(Row, Col, key);
        }

        // Set whether the result is an original puzzle value (if isOriginal is true or
        // ORIGINAL) or a solved one (false or SOLVED), by changing its bold and regular settings.
        private void SetResultOriginal(bool isOriginal)
        {
            Result.Font = isOriginal ? ResultOriginalFont : ResultSolvedFont;
            IsOriginal = isOriginal;
        }

        // Return whether or not the digit is still possible in this square
        public bool IsPossible(int digit)
        {
            if (Globals.ShowSmallDigits)
                return digits[digit - 1].Font.Bold;

            return digits[digit - 1].Tag is 1;
        }

        // Return whether or not the displayed result value is an original puzzle value
        // (as opposed to a solved one), based on whether it's bold
        private bool IsResultOriginal()
        {
            return Result.Font.Bold;
        }

        // make the numRowsCols small digits visible and the result invisible
        public void ShowDigits()
        {
            ChangeDigitsVisibility(true, ChangeOther);
        }

        // make the result digit visible and the numRowsCols small digits invisible
        private void ShowResult()
        {
            ChangeResultVisibility(true, ChangeOther);
        }
    }
}
