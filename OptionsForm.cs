using System;
using System.Windows.Forms;

namespace SudokuProject
{
    /// <summary>
    /// The Options dialog (Delphi: TOptionsForm in Options.pas / Options.dfm).
    /// Controls and event wiring live in OptionsForm.Designer.cs.
    /// </summary>
    public partial class OptionsForm : Form
    {
        public OptionsForm()
        {
            InitializeComponent();
        }

        // Delphi's TRadioGroup is a group box with one radio button per item and an ItemIndex.
        // This provides the same ItemIndex over the three radio buttons (-1 means none selected).
        private int MethodItemIndex
        {
            get
            {
                RadioButton[] items = MethodRadioButtons;
                for (int i = 0; i < items.Length; i++)
                    if (items[i].Checked) return i;
                return -1;
            }
            set
            {
                RadioButton[] items = MethodRadioButtons;
                for (int i = 0; i < items.Length; i++)
                    items[i].Checked = (i == value);
            }
        }

        private RadioButton[] MethodRadioButtons
        {
            get { return new[] { MethodRadioButton1, MethodRadioButton2, MethodRadioButton3 }; }
        }

        // Initialize the controls from our globals
        private void FormCreate(object sender, EventArgs e)
        {
            if (DesignMode) return;

            SleepsCheckBox.Checked = Globals.DoSleeps;
            ReportDurationCheckBox.Checked = Globals.ReportDuration;
            ShowSmallDigitsCheckBox.Checked = Globals.ShowSmallDigits;
            ShowAttemptsCheckBox.Checked = Globals.ShowAttempts;

            MethodItemIndex = (int)Globals.Method - 1;
        }

        // Set our globals from the controls, then Save to the INI file
        private void SaveButtonClick(object sender, EventArgs e)
        {
            Globals.DoSleeps = SleepsCheckBox.Checked;
            Globals.ReportDuration = ReportDurationCheckBox.Checked;
            Globals.ShowSmallDigits = ShowSmallDigitsCheckBox.Checked;
            Globals.ShowAttempts = ShowAttemptsCheckBox.Checked;

            Globals.Method = (SolveMethod)(MethodItemIndex + 1);

            Globals.WriteIni();

            DialogResult = DialogResult.OK;   // Delphi: ModalResult := mrOk
        }

        // Just cancel without saving
        private void CancelButtonClick(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;   // Delphi: ModalResult := mrCancel
        }
    }
}
