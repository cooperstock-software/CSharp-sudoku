namespace SudokuProject
{
    partial class Sudoku6x6Form
    {
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// (Converted from Sudoku6x6.dfm, an "inherited" form: only the properties that differ
        /// from SudokuBaseForm are listed here.)
        /// </summary>
        private void InitializeComponent()
        {
            SuspendLayout();
            // 
            // Shape1
            // 
            Shape1.Size = new System.Drawing.Size(402, 402);
            // 
            // ManualPlayButton
            // 
            ManualPlayButton.Location = new System.Drawing.Point(9, 439);
            ManualPlayButton.Size = new System.Drawing.Size(98, 32);
            // 
            // SolveButton
            // 
            SolveButton.Location = new System.Drawing.Point(122, 439);
            SolveButton.Size = new System.Drawing.Size(94, 32);
            // 
            // LoadButton
            // 
            LoadButton.Location = new System.Drawing.Point(9, 479);
            LoadButton.Size = new System.Drawing.Size(92, 32);
            LoadButton.TabIndex = 3;
            // 
            // SaveButton
            // 
            SaveButton.Location = new System.Drawing.Point(122, 479);
            SaveButton.TabIndex = 4;
            // 
            // ClearButton
            // 
            ClearButton.Location = new System.Drawing.Point(316, 439);
            ClearButton.Size = new System.Drawing.Size(94, 30);
            ClearButton.TabIndex = 2;
            // 
            // SwitchButton
            // 
            SwitchButton.Location = new System.Drawing.Point(9, 527);
            SwitchButton.Text = "S&witch to 9x9 Sudoku";
            // 
            // OptionsButton
            // 
            OptionsButton.Location = new System.Drawing.Point(316, 478);
            // 
            // HelpFileButton
            // 
            HelpFileButton.Location = new System.Drawing.Point(319, 530);
            // 
            // Sudoku6x6Form
            // 
            ClientSize = new System.Drawing.Size(423, 571);
            Name = "Sudoku6x6Form";
            Text = "Sudoku 6x6";
            FormClosed += FormClose;
            ResumeLayout(false);

        }
    }
}
