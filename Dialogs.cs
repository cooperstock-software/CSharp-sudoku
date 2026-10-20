using System.Windows.Forms;

namespace SudokuProject
{
    /// <summary>Thin wrappers giving the Delphi ShowMessage / MessageDlg calls a one-line equivalent.</summary>
    internal static class Dialogs
    {
        /// <summary>Delphi: ShowMessage(text)</summary>
        public static void ShowMessage(string text)
        {
            MessageBox.Show(text, Application.ProductName);
        }

        /// <summary>
        /// Delphi: MessageDlg(text, mtConfirmation, [mbYes, mbNo], 0) = mrYes.
        /// Returns true if the user picked Yes.
        /// </summary>
        public static bool Confirm(string text)
        {
            return MessageBox.Show(text, Application.ProductName,
                                   MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
        }
    }
}
