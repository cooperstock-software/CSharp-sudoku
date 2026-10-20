namespace SudokuProject
{
    partial class OptionsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// (Converted from Options.dfm)
        /// </summary>
        private void InitializeComponent()
        {
            SleepsCheckBox = new System.Windows.Forms.CheckBox();
            ReportDurationCheckBox = new System.Windows.Forms.CheckBox();
            ShowSmallDigitsCheckBox = new System.Windows.Forms.CheckBox();
            ShowAttemptsCheckBox = new System.Windows.Forms.CheckBox();
            MethodRadioGroup = new System.Windows.Forms.GroupBox();
            MethodRadioButton3 = new System.Windows.Forms.RadioButton();
            MethodRadioButton2 = new System.Windows.Forms.RadioButton();
            MethodRadioButton1 = new System.Windows.Forms.RadioButton();
            SaveOptionsButton = new System.Windows.Forms.Button();
            CancelOptionsButton = new System.Windows.Forms.Button();
            MethodRadioGroup.SuspendLayout();
            SuspendLayout();
            // 
            // SleepsCheckBox
            // 
            SleepsCheckBox.Font = new System.Drawing.Font("Tahoma", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, 0);
            SleepsCheckBox.Location = new System.Drawing.Point(20, 18);
            SleepsCheckBox.Name = "SleepsCheckBox";
            SleepsCheckBox.Size = new System.Drawing.Size(619, 26);
            SleepsCheckBox.TabIndex = 0;
            SleepsCheckBox.Text = "&Do short pauses after solving squares";
            SleepsCheckBox.UseVisualStyleBackColor = true;
            // 
            // ReportDurationCheckBox
            // 
            ReportDurationCheckBox.Font = new System.Drawing.Font("Tahoma", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, 0);
            ReportDurationCheckBox.Location = new System.Drawing.Point(20, 46);
            ReportDurationCheckBox.Name = "ReportDurationCheckBox";
            ReportDurationCheckBox.Size = new System.Drawing.Size(619, 26);
            ReportDurationCheckBox.TabIndex = 1;
            ReportDurationCheckBox.Text = "&Report the time taken to solve the puzzle, ignoring pauses";
            ReportDurationCheckBox.UseVisualStyleBackColor = true;
            // 
            // ShowSmallDigitsCheckBox
            // 
            ShowSmallDigitsCheckBox.Font = new System.Drawing.Font("Tahoma", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, 0);
            ShowSmallDigitsCheckBox.Location = new System.Drawing.Point(20, 74);
            ShowSmallDigitsCheckBox.Name = "ShowSmallDigitsCheckBox";
            ShowSmallDigitsCheckBox.Size = new System.Drawing.Size(619, 26);
            ShowSmallDigitsCheckBox.TabIndex = 2;
            ShowSmallDigitsCheckBox.Text = "S&how small helper digits in each square during Manual Play or Simple Solving";
            ShowSmallDigitsCheckBox.UseVisualStyleBackColor = true;
            // 
            // ShowAttemptsCheckBox
            // 
            ShowAttemptsCheckBox.Font = new System.Drawing.Font("Tahoma", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, 0);
            ShowAttemptsCheckBox.Location = new System.Drawing.Point(20, 234);
            ShowAttemptsCheckBox.Name = "ShowAttemptsCheckBox";
            ShowAttemptsCheckBox.Size = new System.Drawing.Size(619, 26);
            ShowAttemptsCheckBox.TabIndex = 4;
            ShowAttemptsCheckBox.Text = "In fast backtracking &approach, show digits being tried";
            ShowAttemptsCheckBox.UseVisualStyleBackColor = true;
            // 
            // MethodRadioGroup
            // 
            MethodRadioGroup.Controls.Add(MethodRadioButton3);
            MethodRadioGroup.Controls.Add(MethodRadioButton2);
            MethodRadioGroup.Controls.Add(MethodRadioButton1);
            MethodRadioGroup.Font = new System.Drawing.Font("Tahoma", 17F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, 0);
            MethodRadioGroup.Location = new System.Drawing.Point(12, 114);
            MethodRadioGroup.Name = "MethodRadioGroup";
            MethodRadioGroup.Size = new System.Drawing.Size(619, 114);
            MethodRadioGroup.TabIndex = 3;
            MethodRadioGroup.TabStop = false;
            MethodRadioGroup.Text = "Solving Method to Use";
            // 
            // MethodRadioButton3
            // 
            MethodRadioButton3.Font = new System.Drawing.Font("Tahoma", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, 0);
            MethodRadioButton3.Location = new System.Drawing.Point(12, 76);
            MethodRadioButton3.Name = "MethodRadioButton3";
            MethodRadioButton3.Size = new System.Drawing.Size(595, 26);
            MethodRadioButton3.TabIndex = 2;
            MethodRadioButton3.Text = "&Only use fast backtracking approach";
            MethodRadioButton3.UseVisualStyleBackColor = true;
            // 
            // MethodRadioButton2
            // 
            MethodRadioButton2.Font = new System.Drawing.Font("Tahoma", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, 0);
            MethodRadioButton2.Location = new System.Drawing.Point(12, 49);
            MethodRadioButton2.Name = "MethodRadioButton2";
            MethodRadioButton2.Size = new System.Drawing.Size(595, 26);
            MethodRadioButton2.TabIndex = 1;
            MethodRadioButton2.Text = "Si&mple solving rules, automatically use fast backtracking approach if needed";
            MethodRadioButton2.UseVisualStyleBackColor = true;
            // 
            // MethodRadioButton1
            // 
            MethodRadioButton1.Font = new System.Drawing.Font("Tahoma", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, 0);
            MethodRadioButton1.Location = new System.Drawing.Point(12, 22);
            MethodRadioButton1.Name = "MethodRadioButton1";
            MethodRadioButton1.Size = new System.Drawing.Size(595, 26);
            MethodRadioButton1.TabIndex = 0;
            MethodRadioButton1.Text = "S&imple solving rules, plus prompt to use fast backtracking approach if needed";
            MethodRadioButton1.UseVisualStyleBackColor = true;
            // 
            // SaveOptionsButton
            // 
            SaveOptionsButton.Font = new System.Drawing.Font("Tahoma", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, 0);
            SaveOptionsButton.Location = new System.Drawing.Point(20, 275);
            SaveOptionsButton.Name = "SaveOptionsButton";
            SaveOptionsButton.Size = new System.Drawing.Size(75, 28);
            SaveOptionsButton.TabIndex = 5;
            SaveOptionsButton.Text = "&Save";
            SaveOptionsButton.UseVisualStyleBackColor = true;
            SaveOptionsButton.Click += SaveButtonClick;
            // 
            // CancelOptionsButton
            // 
            CancelOptionsButton.Font = new System.Drawing.Font("Tahoma", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, 0);
            CancelOptionsButton.Location = new System.Drawing.Point(128, 275);
            CancelOptionsButton.Name = "CancelOptionsButton";
            CancelOptionsButton.Size = new System.Drawing.Size(75, 28);
            CancelOptionsButton.TabIndex = 6;
            CancelOptionsButton.Text = "&Cancel";
            CancelOptionsButton.UseVisualStyleBackColor = true;
            CancelOptionsButton.Click += CancelButtonClick;
            // 
            // OptionsForm
            // 
            AcceptButton = SaveOptionsButton;
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            CancelButton = CancelOptionsButton;
            ClientSize = new System.Drawing.Size(655, 316);
            Controls.Add(CancelOptionsButton);
            Controls.Add(SaveOptionsButton);
            Controls.Add(MethodRadioGroup);
            Controls.Add(ShowAttemptsCheckBox);
            Controls.Add(ShowSmallDigitsCheckBox);
            Controls.Add(ReportDurationCheckBox);
            Controls.Add(SleepsCheckBox);
            Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "OptionsForm";
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "Options";
            Load += FormCreate;
            MethodRadioGroup.ResumeLayout(false);
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox SleepsCheckBox;
        private System.Windows.Forms.CheckBox ReportDurationCheckBox;
        private System.Windows.Forms.CheckBox ShowSmallDigitsCheckBox;
        private System.Windows.Forms.CheckBox ShowAttemptsCheckBox;
        private System.Windows.Forms.GroupBox MethodRadioGroup;
        private System.Windows.Forms.RadioButton MethodRadioButton1;
        private System.Windows.Forms.RadioButton MethodRadioButton2;
        private System.Windows.Forms.RadioButton MethodRadioButton3;
        private System.Windows.Forms.Button SaveOptionsButton;
        private System.Windows.Forms.Button CancelOptionsButton;
    }
}
