namespace MilSpace.ProjectionsConverter.UI
{
    partial class CoordinatesConverter
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.XCoordinateTextBox = new System.Windows.Forms.TextBox();
            this.YCoordinateTextBox = new System.Windows.Forms.TextBox();
            this.XCoordinateLabel = new System.Windows.Forms.Label();
            this.YCoordinateLabel = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.WgsXCoordinateLabel = new System.Windows.Forms.Label();
            this.WgsYCoordinateTextBox = new System.Windows.Forms.TextBox();
            this.WgsYCoordinateLabel = new System.Windows.Forms.Label();
            this.WgsXCoordinateTextBox = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.MgrsNotationLabel = new System.Windows.Forms.Label();
            this.MgrsNotationTextBox = new System.Windows.Forms.TextBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.PulkovoXCoordinateLabel = new System.Windows.Forms.Label();
            this.PulkovoYCoordinateTextBox = new System.Windows.Forms.TextBox();
            this.PulkovoYCoordinateLabel = new System.Windows.Forms.Label();
            this.PulkovoXCoordinateTextBox = new System.Windows.Forms.TextBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.UkraineXCoordinateLabel = new System.Windows.Forms.Label();
            this.UkraineYCoordinateTextBox = new System.Windows.Forms.TextBox();
            this.UkraineYCoordinateLabel = new System.Windows.Forms.Label();
            this.UkraineXCoordinateTextBox = new System.Windows.Forms.TextBox();
            this.CloseButton = new System.Windows.Forms.Button();
            this.CopyButton = new System.Windows.Forms.Button();
            this.SaveButton = new System.Windows.Forms.Button();
            this.saveButtonFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // XCoordinateTextBox
            // 
            this.XCoordinateTextBox.Enabled = false;
            this.XCoordinateTextBox.Location = new System.Drawing.Point(80, 26);
            this.XCoordinateTextBox.Name = "XCoordinateTextBox";
            this.XCoordinateTextBox.Size = new System.Drawing.Size(225, 20);
            this.XCoordinateTextBox.TabIndex = 0;
            // 
            // YCoordinateTextBox
            // 
            this.YCoordinateTextBox.Enabled = false;
            this.YCoordinateTextBox.Location = new System.Drawing.Point(80, 52);
            this.YCoordinateTextBox.Name = "YCoordinateTextBox";
            this.YCoordinateTextBox.Size = new System.Drawing.Size(225, 20);
            this.YCoordinateTextBox.TabIndex = 1;
            // 
            // XCoordinateLabel
            // 
            this.XCoordinateLabel.AutoSize = true;
            this.XCoordinateLabel.Location = new System.Drawing.Point(6, 26);
            this.XCoordinateLabel.Name = "XCoordinateLabel";
            this.XCoordinateLabel.Size = new System.Drawing.Size(68, 13);
            this.XCoordinateLabel.TabIndex = 2;
            this.XCoordinateLabel.Text = "X Coordinate";
            // 
            // YCoordinateLabel
            // 
            this.YCoordinateLabel.AutoSize = true;
            this.YCoordinateLabel.Location = new System.Drawing.Point(6, 52);
            this.YCoordinateLabel.Name = "YCoordinateLabel";
            this.YCoordinateLabel.Size = new System.Drawing.Size(68, 13);
            this.YCoordinateLabel.TabIndex = 3;
            this.YCoordinateLabel.Text = "Y Coordinate";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.XCoordinateLabel);
            this.groupBox1.Controls.Add(this.YCoordinateTextBox);
            this.groupBox1.Controls.Add(this.YCoordinateLabel);
            this.groupBox1.Controls.Add(this.XCoordinateTextBox);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(312, 89);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Current Map Coordinate System";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.WgsXCoordinateLabel);
            this.groupBox2.Controls.Add(this.WgsYCoordinateTextBox);
            this.groupBox2.Controls.Add(this.WgsYCoordinateLabel);
            this.groupBox2.Controls.Add(this.WgsXCoordinateTextBox);
            this.groupBox2.Location = new System.Drawing.Point(12, 107);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(312, 89);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "WGS84";
            // 
            // WgsXCoordinateLabel
            // 
            this.WgsXCoordinateLabel.AutoSize = true;
            this.WgsXCoordinateLabel.Location = new System.Drawing.Point(6, 26);
            this.WgsXCoordinateLabel.Name = "WgsXCoordinateLabel";
            this.WgsXCoordinateLabel.Size = new System.Drawing.Size(68, 13);
            this.WgsXCoordinateLabel.TabIndex = 2;
            this.WgsXCoordinateLabel.Text = "X Coordinate";
            // 
            // WgsYCoordinateTextBox
            // 
            this.WgsYCoordinateTextBox.Enabled = false;
            this.WgsYCoordinateTextBox.Location = new System.Drawing.Point(80, 52);
            this.WgsYCoordinateTextBox.Name = "WgsYCoordinateTextBox";
            this.WgsYCoordinateTextBox.Size = new System.Drawing.Size(225, 20);
            this.WgsYCoordinateTextBox.TabIndex = 1;
            // 
            // WgsYCoordinateLabel
            // 
            this.WgsYCoordinateLabel.AutoSize = true;
            this.WgsYCoordinateLabel.Location = new System.Drawing.Point(6, 52);
            this.WgsYCoordinateLabel.Name = "WgsYCoordinateLabel";
            this.WgsYCoordinateLabel.Size = new System.Drawing.Size(68, 13);
            this.WgsYCoordinateLabel.TabIndex = 3;
            this.WgsYCoordinateLabel.Text = "Y Coordinate";
            // 
            // WgsXCoordinateTextBox
            // 
            this.WgsXCoordinateTextBox.Enabled = false;
            this.WgsXCoordinateTextBox.Location = new System.Drawing.Point(80, 26);
            this.WgsXCoordinateTextBox.Name = "WgsXCoordinateTextBox";
            this.WgsXCoordinateTextBox.Size = new System.Drawing.Size(225, 20);
            this.WgsXCoordinateTextBox.TabIndex = 0;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.MgrsNotationLabel);
            this.groupBox3.Controls.Add(this.MgrsNotationTextBox);
            this.groupBox3.Location = new System.Drawing.Point(12, 392);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(312, 61);
            this.groupBox3.TabIndex = 6;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "MGRS";
            // 
            // MgrsNotationLabel
            // 
            this.MgrsNotationLabel.AutoSize = true;
            this.MgrsNotationLabel.Location = new System.Drawing.Point(6, 26);
            this.MgrsNotationLabel.Name = "MgrsNotationLabel";
            this.MgrsNotationLabel.Size = new System.Drawing.Size(82, 13);
            this.MgrsNotationLabel.TabIndex = 2;
            this.MgrsNotationLabel.Text = "MGRS Notation";
            // 
            // MgrsNotationTextBox
            // 
            this.MgrsNotationTextBox.Enabled = false;
            this.MgrsNotationTextBox.Location = new System.Drawing.Point(94, 26);
            this.MgrsNotationTextBox.Name = "MgrsNotationTextBox";
            this.MgrsNotationTextBox.Size = new System.Drawing.Size(211, 20);
            this.MgrsNotationTextBox.TabIndex = 0;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.PulkovoXCoordinateLabel);
            this.groupBox4.Controls.Add(this.PulkovoYCoordinateTextBox);
            this.groupBox4.Controls.Add(this.PulkovoYCoordinateLabel);
            this.groupBox4.Controls.Add(this.PulkovoXCoordinateTextBox);
            this.groupBox4.Location = new System.Drawing.Point(14, 202);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(312, 89);
            this.groupBox4.TabIndex = 7;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Pulkovo 1984";
            // 
            // PulkovoXCoordinateLabel
            // 
            this.PulkovoXCoordinateLabel.AutoSize = true;
            this.PulkovoXCoordinateLabel.Location = new System.Drawing.Point(6, 26);
            this.PulkovoXCoordinateLabel.Name = "PulkovoXCoordinateLabel";
            this.PulkovoXCoordinateLabel.Size = new System.Drawing.Size(68, 13);
            this.PulkovoXCoordinateLabel.TabIndex = 2;
            this.PulkovoXCoordinateLabel.Text = "X Coordinate";
            // 
            // PulkovoYCoordinateTextBox
            // 
            this.PulkovoYCoordinateTextBox.Enabled = false;
            this.PulkovoYCoordinateTextBox.Location = new System.Drawing.Point(80, 52);
            this.PulkovoYCoordinateTextBox.Name = "PulkovoYCoordinateTextBox";
            this.PulkovoYCoordinateTextBox.Size = new System.Drawing.Size(225, 20);
            this.PulkovoYCoordinateTextBox.TabIndex = 1;
            // 
            // PulkovoYCoordinateLabel
            // 
            this.PulkovoYCoordinateLabel.AutoSize = true;
            this.PulkovoYCoordinateLabel.Location = new System.Drawing.Point(6, 52);
            this.PulkovoYCoordinateLabel.Name = "PulkovoYCoordinateLabel";
            this.PulkovoYCoordinateLabel.Size = new System.Drawing.Size(68, 13);
            this.PulkovoYCoordinateLabel.TabIndex = 3;
            this.PulkovoYCoordinateLabel.Text = "Y Coordinate";
            // 
            // PulkovoXCoordinateTextBox
            // 
            this.PulkovoXCoordinateTextBox.Enabled = false;
            this.PulkovoXCoordinateTextBox.Location = new System.Drawing.Point(80, 26);
            this.PulkovoXCoordinateTextBox.Name = "PulkovoXCoordinateTextBox";
            this.PulkovoXCoordinateTextBox.Size = new System.Drawing.Size(225, 20);
            this.PulkovoXCoordinateTextBox.TabIndex = 0;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.UkraineXCoordinateLabel);
            this.groupBox5.Controls.Add(this.UkraineYCoordinateTextBox);
            this.groupBox5.Controls.Add(this.UkraineYCoordinateLabel);
            this.groupBox5.Controls.Add(this.UkraineXCoordinateTextBox);
            this.groupBox5.Location = new System.Drawing.Point(12, 297);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(312, 89);
            this.groupBox5.TabIndex = 8;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Ukraine 2000";
            // 
            // UkraineXCoordinateLabel
            // 
            this.UkraineXCoordinateLabel.AutoSize = true;
            this.UkraineXCoordinateLabel.Location = new System.Drawing.Point(6, 26);
            this.UkraineXCoordinateLabel.Name = "UkraineXCoordinateLabel";
            this.UkraineXCoordinateLabel.Size = new System.Drawing.Size(68, 13);
            this.UkraineXCoordinateLabel.TabIndex = 2;
            this.UkraineXCoordinateLabel.Text = "X Coordinate";
            // 
            // UkraineYCoordinateTextBox
            // 
            this.UkraineYCoordinateTextBox.Enabled = false;
            this.UkraineYCoordinateTextBox.Location = new System.Drawing.Point(80, 52);
            this.UkraineYCoordinateTextBox.Name = "UkraineYCoordinateTextBox";
            this.UkraineYCoordinateTextBox.Size = new System.Drawing.Size(225, 20);
            this.UkraineYCoordinateTextBox.TabIndex = 1;
            // 
            // UkraineYCoordinateLabel
            // 
            this.UkraineYCoordinateLabel.AutoSize = true;
            this.UkraineYCoordinateLabel.Location = new System.Drawing.Point(6, 52);
            this.UkraineYCoordinateLabel.Name = "UkraineYCoordinateLabel";
            this.UkraineYCoordinateLabel.Size = new System.Drawing.Size(68, 13);
            this.UkraineYCoordinateLabel.TabIndex = 3;
            this.UkraineYCoordinateLabel.Text = "Y Coordinate";
            // 
            // UkraineXCoordinateTextBox
            // 
            this.UkraineXCoordinateTextBox.Enabled = false;
            this.UkraineXCoordinateTextBox.Location = new System.Drawing.Point(80, 26);
            this.UkraineXCoordinateTextBox.Name = "UkraineXCoordinateTextBox";
            this.UkraineXCoordinateTextBox.Size = new System.Drawing.Size(225, 20);
            this.UkraineXCoordinateTextBox.TabIndex = 0;
            // 
            // CloseButton
            // 
            this.CloseButton.Location = new System.Drawing.Point(251, 464);
            this.CloseButton.Name = "CloseButton";
            this.CloseButton.Size = new System.Drawing.Size(75, 23);
            this.CloseButton.TabIndex = 9;
            this.CloseButton.Text = "Close";
            this.CloseButton.UseVisualStyleBackColor = true;
            this.CloseButton.Click += new System.EventHandler(this.CloseButton_Click);
            // 
            // CopyButton
            // 
            this.CopyButton.Location = new System.Drawing.Point(123, 464);
            this.CopyButton.Name = "CopyButton";
            this.CopyButton.Size = new System.Drawing.Size(105, 23);
            this.CopyButton.TabIndex = 10;
            this.CopyButton.Text = "Copy to clipboard";
            this.CopyButton.UseVisualStyleBackColor = true;
            this.CopyButton.Click += new System.EventHandler(this.CopyButton_Click);
            // 
            // SaveButton
            // 
            this.SaveButton.Location = new System.Drawing.Point(12, 464);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(105, 23);
            this.SaveButton.TabIndex = 11;
            this.SaveButton.Text = "Save to file";
            this.SaveButton.UseVisualStyleBackColor = true;
            this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // saveButtonFileDialog
            // 
            this.saveButtonFileDialog.FileName = "Coordinates.xml";
            // 
            // CoordinatesConverter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(336, 499);
            this.Controls.Add(this.SaveButton);
            this.Controls.Add(this.CopyButton);
            this.Controls.Add(this.CloseButton);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "CoordinatesConverter";
            this.Text = "Coordinates Converter";
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.CoordinatesConverter_MouseClick);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox XCoordinateTextBox;
        private System.Windows.Forms.TextBox YCoordinateTextBox;
        private System.Windows.Forms.Label XCoordinateLabel;
        private System.Windows.Forms.Label YCoordinateLabel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label WgsXCoordinateLabel;
        private System.Windows.Forms.TextBox WgsYCoordinateTextBox;
        private System.Windows.Forms.Label WgsYCoordinateLabel;
        private System.Windows.Forms.TextBox WgsXCoordinateTextBox;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label MgrsNotationLabel;
        private System.Windows.Forms.TextBox MgrsNotationTextBox;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label PulkovoXCoordinateLabel;
        private System.Windows.Forms.TextBox PulkovoYCoordinateTextBox;
        private System.Windows.Forms.Label PulkovoYCoordinateLabel;
        private System.Windows.Forms.TextBox PulkovoXCoordinateTextBox;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Label UkraineXCoordinateLabel;
        private System.Windows.Forms.TextBox UkraineYCoordinateTextBox;
        private System.Windows.Forms.Label UkraineYCoordinateLabel;
        private System.Windows.Forms.TextBox UkraineXCoordinateTextBox;
        private System.Windows.Forms.Button CloseButton;
        private System.Windows.Forms.Button CopyButton;
        private System.Windows.Forms.Button SaveButton;
        private System.Windows.Forms.OpenFileDialog saveButtonFileDialog;
    }
}

