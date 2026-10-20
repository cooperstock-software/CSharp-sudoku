namespace SudokuProject
{
    partial class SudokuBaseForm
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
        /// (Converted from SudokuBase.dfm)
        /// </summary>
        private void InitializeComponent()
        {
            this.Shape1 = new System.Windows.Forms.Panel();
            this.ManualPlayButton = new System.Windows.Forms.Button();
            this.SolveButton = new System.Windows.Forms.Button();
            this.LoadButton = new System.Windows.Forms.Button();
            this.SaveButton = new System.Windows.Forms.Button();
            this.ClearButton = new System.Windows.Forms.Button();
            this.SwitchButton = new System.Windows.Forms.Button();
            this.OptionsButton = new System.Windows.Forms.Button();
            this.HelpFileButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            //
            // Shape1
            //
            this.Shape1.BackColor = System.Drawing.Color.Black;
            this.Shape1.Enabled = false;
            this.Shape1.Location = new System.Drawing.Point(8, 8);
            this.Shape1.Name = "Shape1";
            this.Shape1.Size = new System.Drawing.Size(599, 599);
            this.Shape1.TabIndex = 8;
            //
            // ManualPlayButton
            //
            this.ManualPlayButton.Font = new System.Drawing.Font("Tahoma", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.ManualPlayButton.Location = new System.Drawing.Point(9, 628);
            this.ManualPlayButton.Name = "ManualPlayButton";
            this.ManualPlayButton.Size = new System.Drawing.Size(98, 30);
            this.ManualPlayButton.TabIndex = 0;
            this.ManualPlayButton.Text = "&Manual Play";
            this.ManualPlayButton.UseVisualStyleBackColor = true;
            this.ManualPlayButton.Click += new System.EventHandler(this.ManualPlayButtonClick);
            //
            // SolveButton
            //
            this.SolveButton.Font = new System.Drawing.Font("Tahoma", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.SolveButton.Location = new System.Drawing.Point(122, 628);
            this.SolveButton.Name = "SolveButton";
            this.SolveButton.Size = new System.Drawing.Size(94, 30);
            this.SolveButton.TabIndex = 1;
            this.SolveButton.Text = "&Solve";
            this.SolveButton.UseVisualStyleBackColor = true;
            this.SolveButton.Click += new System.EventHandler(this.SolveButtonClick);
            //
            // LoadButton
            //
            this.LoadButton.Font = new System.Drawing.Font("Tahoma", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.LoadButton.Location = new System.Drawing.Point(285, 628);
            this.LoadButton.Name = "LoadButton";
            this.LoadButton.Size = new System.Drawing.Size(92, 30);
            this.LoadButton.TabIndex = 2;
            this.LoadButton.Text = "&Load";
            this.LoadButton.UseVisualStyleBackColor = true;
            this.LoadButton.Click += new System.EventHandler(this.LoadButtonClick);
            //
            // SaveButton
            //
            this.SaveButton.Font = new System.Drawing.Font("Tahoma", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.SaveButton.Location = new System.Drawing.Point(397, 628);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(92, 30);
            this.SaveButton.TabIndex = 3;
            this.SaveButton.Text = "S&ave";
            this.SaveButton.UseVisualStyleBackColor = true;
            this.SaveButton.Click += new System.EventHandler(this.SaveButtonClick);
            //
            // ClearButton
            //
            this.ClearButton.Font = new System.Drawing.Font("Tahoma", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.ClearButton.Location = new System.Drawing.Point(9, 672);
            this.ClearButton.Name = "ClearButton";
            this.ClearButton.Size = new System.Drawing.Size(98, 30);
            this.ClearButton.TabIndex = 4;
            this.ClearButton.Text = "&Clear";
            this.ClearButton.UseVisualStyleBackColor = true;
            this.ClearButton.Click += new System.EventHandler(this.ClearButtonClick);
            //
            // SwitchButton
            //
            this.SwitchButton.Font = new System.Drawing.Font("Tahoma", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.SwitchButton.Location = new System.Drawing.Point(285, 672);
            this.SwitchButton.Name = "SwitchButton";
            this.SwitchButton.Size = new System.Drawing.Size(206, 30);
            this.SwitchButton.TabIndex = 6;
            this.SwitchButton.Text = "S&witch to 6x6 Sudoku";
            this.SwitchButton.UseVisualStyleBackColor = true;
            this.SwitchButton.Click += new System.EventHandler(this.SwitchButtonClick);
            //
            // OptionsButton
            //
            this.OptionsButton.Font = new System.Drawing.Font("Tahoma", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.OptionsButton.Location = new System.Drawing.Point(122, 672);
            this.OptionsButton.Name = "OptionsButton";
            this.OptionsButton.Size = new System.Drawing.Size(94, 30);
            this.OptionsButton.TabIndex = 5;
            this.OptionsButton.Text = "&Options";
            this.OptionsButton.UseVisualStyleBackColor = true;
            this.OptionsButton.Click += new System.EventHandler(this.OptionsButtonClick);
            //
            // HelpFileButton
            //
            this.HelpFileButton.Font = new System.Drawing.Font("Tahoma", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.HelpFileButton.Location = new System.Drawing.Point(512, 627);
            this.HelpFileButton.Name = "HelpFileButton";
            this.HelpFileButton.Size = new System.Drawing.Size(94, 31);
            this.HelpFileButton.TabIndex = 7;
            this.HelpFileButton.Text = "&Help";
            this.HelpFileButton.UseVisualStyleBackColor = true;
            this.HelpFileButton.Click += new System.EventHandler(this.HelpButtonClick);
            //
            // SudokuBaseForm
            //
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(615, 716);
            this.Controls.Add(this.HelpFileButton);
            this.Controls.Add(this.OptionsButton);
            this.Controls.Add(this.SwitchButton);
            this.Controls.Add(this.ClearButton);
            this.Controls.Add(this.SaveButton);
            this.Controls.Add(this.LoadButton);
            this.Controls.Add(this.SolveButton);
            this.Controls.Add(this.ManualPlayButton);
            this.Controls.Add(this.Shape1);
            this.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(1)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.Name = "SudokuBaseForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Sudoku";
            this.Load += new System.EventHandler(this.FormCreate);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.FormKeyPress);
            this.ResumeLayout(false);

        }

        #endregion

        // These are protected (not private) because the 6x6 form inherits them and moves them around,
        // just as the inherited controls in Sudoku6x6.dfm did.
        protected System.Windows.Forms.Panel Shape1;
        protected System.Windows.Forms.Button ManualPlayButton;
        protected System.Windows.Forms.Button SolveButton;
        protected System.Windows.Forms.Button LoadButton;
        protected System.Windows.Forms.Button SaveButton;
        protected System.Windows.Forms.Button ClearButton;
        protected System.Windows.Forms.Button SwitchButton;
        protected System.Windows.Forms.Button OptionsButton;
        protected System.Windows.Forms.Button HelpFileButton;
    }
}
