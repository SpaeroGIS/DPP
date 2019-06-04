namespace ArcMapAddin
{
    partial class DockableWindowGeoCalculator
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
        /// </summary>
        private void InitializeComponent()
        {
            this.LatitudeLongitudeGroup = new System.Windows.Forms.GroupBox();
            this.SaveButton = new System.Windows.Forms.Button();
            this.CopyButton = new System.Windows.Forms.Button();
            this.UTMNotationLabel = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.UTMNotationTextBox = new System.Windows.Forms.TextBox();
            this.MgrsNotationLabel = new System.Windows.Forms.Label();
            this.UkraineCoordinatesLabel = new System.Windows.Forms.Label();
            this.PulkovoCoordinatesLabel = new System.Windows.Forms.Label();
            this.WgsCoordinatesLabel = new System.Windows.Forms.Label();
            this.CurrentMapLabel = new System.Windows.Forms.Label();
            this.MgrsNotationTextBox = new System.Windows.Forms.TextBox();
            this.UkraineYCoordinateTextBox = new System.Windows.Forms.TextBox();
            this.UkraineXCoordinateTextBox = new System.Windows.Forms.TextBox();
            this.PulkovoXCoordinateTextBox = new System.Windows.Forms.TextBox();
            this.PulkovoYCoordinateTextBox = new System.Windows.Forms.TextBox();
            this.YCoordinateTextBox = new System.Windows.Forms.TextBox();
            this.XCoordinateTextBox = new System.Windows.Forms.TextBox();
            this.WgsYCoordinateTextBox = new System.Windows.Forms.TextBox();
            this.WgsXCoordinateTextBox = new System.Windows.Forms.TextBox();
            this.MoveToCenterButton = new System.Windows.Forms.Button();
            this.saveButtonFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.LatitudeLongitudeGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // LatitudeLongitudeGroup
            // 
            this.LatitudeLongitudeGroup.Controls.Add(this.MoveToCenterButton);
            this.LatitudeLongitudeGroup.Controls.Add(this.SaveButton);
            this.LatitudeLongitudeGroup.Controls.Add(this.CopyButton);
            this.LatitudeLongitudeGroup.Controls.Add(this.UTMNotationLabel);
            this.LatitudeLongitudeGroup.Controls.Add(this.textBox2);
            this.LatitudeLongitudeGroup.Controls.Add(this.UTMNotationTextBox);
            this.LatitudeLongitudeGroup.Controls.Add(this.MgrsNotationLabel);
            this.LatitudeLongitudeGroup.Controls.Add(this.UkraineCoordinatesLabel);
            this.LatitudeLongitudeGroup.Controls.Add(this.PulkovoCoordinatesLabel);
            this.LatitudeLongitudeGroup.Controls.Add(this.WgsCoordinatesLabel);
            this.LatitudeLongitudeGroup.Controls.Add(this.CurrentMapLabel);
            this.LatitudeLongitudeGroup.Controls.Add(this.MgrsNotationTextBox);
            this.LatitudeLongitudeGroup.Controls.Add(this.UkraineYCoordinateTextBox);
            this.LatitudeLongitudeGroup.Controls.Add(this.UkraineXCoordinateTextBox);
            this.LatitudeLongitudeGroup.Controls.Add(this.PulkovoXCoordinateTextBox);
            this.LatitudeLongitudeGroup.Controls.Add(this.PulkovoYCoordinateTextBox);
            this.LatitudeLongitudeGroup.Controls.Add(this.YCoordinateTextBox);
            this.LatitudeLongitudeGroup.Controls.Add(this.XCoordinateTextBox);
            this.LatitudeLongitudeGroup.Controls.Add(this.WgsYCoordinateTextBox);
            this.LatitudeLongitudeGroup.Controls.Add(this.WgsXCoordinateTextBox);
            this.LatitudeLongitudeGroup.Location = new System.Drawing.Point(4, 4);
            this.LatitudeLongitudeGroup.Name = "LatitudeLongitudeGroup";
            this.LatitudeLongitudeGroup.Size = new System.Drawing.Size(293, 393);
            this.LatitudeLongitudeGroup.TabIndex = 0;
            this.LatitudeLongitudeGroup.TabStop = false;
            this.LatitudeLongitudeGroup.Text = "Latitude/Longitude";
            // 
            // SaveButton
            // 
            this.SaveButton.Location = new System.Drawing.Point(71, 364);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(105, 23);
            this.SaveButton.TabIndex = 36;
            this.SaveButton.Text = "Save to file";
            this.SaveButton.UseVisualStyleBackColor = true;
            this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // CopyButton
            // 
            this.CopyButton.Location = new System.Drawing.Point(182, 364);
            this.CopyButton.Name = "CopyButton";
            this.CopyButton.Size = new System.Drawing.Size(105, 23);
            this.CopyButton.TabIndex = 35;
            this.CopyButton.Text = "Copy";
            this.CopyButton.UseVisualStyleBackColor = true;
            this.CopyButton.Click += new System.EventHandler(this.CopyButton_Click);
            // 
            // UTMNotationLabel
            // 
            this.UTMNotationLabel.AutoSize = true;
            this.UTMNotationLabel.Location = new System.Drawing.Point(7, 287);
            this.UTMNotationLabel.Name = "UTMNotationLabel";
            this.UTMNotationLabel.Size = new System.Drawing.Size(106, 13);
            this.UTMNotationLabel.TabIndex = 34;
            this.UTMNotationLabel.Text = "UTM Representation";
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(81, 332);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(100, 20);
            this.textBox2.TabIndex = 32;
            // 
            // UTMNotationTextBox
            // 
            this.UTMNotationTextBox.Location = new System.Drawing.Point(81, 306);
            this.UTMNotationTextBox.Name = "UTMNotationTextBox";
            this.UTMNotationTextBox.Size = new System.Drawing.Size(100, 20);
            this.UTMNotationTextBox.TabIndex = 31;
            // 
            // MgrsNotationLabel
            // 
            this.MgrsNotationLabel.AutoSize = true;
            this.MgrsNotationLabel.Location = new System.Drawing.Point(7, 235);
            this.MgrsNotationLabel.Name = "MgrsNotationLabel";
            this.MgrsNotationLabel.Size = new System.Drawing.Size(114, 13);
            this.MgrsNotationLabel.TabIndex = 30;
            this.MgrsNotationLabel.Text = "MGRS Representation";
            // 
            // UkraineCoordinatesLabel
            // 
            this.UkraineCoordinatesLabel.AutoSize = true;
            this.UkraineCoordinatesLabel.Location = new System.Drawing.Point(7, 183);
            this.UkraineCoordinatesLabel.Name = "UkraineCoordinatesLabel";
            this.UkraineCoordinatesLabel.Size = new System.Drawing.Size(68, 13);
            this.UkraineCoordinatesLabel.TabIndex = 26;
            this.UkraineCoordinatesLabel.Text = "Ukraine2000";
            // 
            // PulkovoCoordinatesLabel
            // 
            this.PulkovoCoordinatesLabel.AutoSize = true;
            this.PulkovoCoordinatesLabel.Location = new System.Drawing.Point(7, 131);
            this.PulkovoCoordinatesLabel.Name = "PulkovoCoordinatesLabel";
            this.PulkovoCoordinatesLabel.Size = new System.Drawing.Size(70, 13);
            this.PulkovoCoordinatesLabel.TabIndex = 24;
            this.PulkovoCoordinatesLabel.Text = "Pulkovo1942";
            // 
            // WgsCoordinatesLabel
            // 
            this.WgsCoordinatesLabel.AutoSize = true;
            this.WgsCoordinatesLabel.Location = new System.Drawing.Point(7, 79);
            this.WgsCoordinatesLabel.Name = "WgsCoordinatesLabel";
            this.WgsCoordinatesLabel.Size = new System.Drawing.Size(57, 13);
            this.WgsCoordinatesLabel.TabIndex = 22;
            this.WgsCoordinatesLabel.Text = "WGS1984";
            // 
            // CurrentMapLabel
            // 
            this.CurrentMapLabel.AutoSize = true;
            this.CurrentMapLabel.Location = new System.Drawing.Point(7, 26);
            this.CurrentMapLabel.Name = "CurrentMapLabel";
            this.CurrentMapLabel.Size = new System.Drawing.Size(124, 13);
            this.CurrentMapLabel.TabIndex = 20;
            this.CurrentMapLabel.Text = "Current Map Coordinates";
            // 
            // MgrsNotationTextBox
            // 
            this.MgrsNotationTextBox.Location = new System.Drawing.Point(81, 254);
            this.MgrsNotationTextBox.Name = "MgrsNotationTextBox";
            this.MgrsNotationTextBox.Size = new System.Drawing.Size(100, 20);
            this.MgrsNotationTextBox.TabIndex = 18;
            // 
            // UkraineYCoordinateTextBox
            // 
            this.UkraineYCoordinateTextBox.Location = new System.Drawing.Point(187, 202);
            this.UkraineYCoordinateTextBox.Name = "UkraineYCoordinateTextBox";
            this.UkraineYCoordinateTextBox.Size = new System.Drawing.Size(100, 20);
            this.UkraineYCoordinateTextBox.TabIndex = 12;
            // 
            // UkraineXCoordinateTextBox
            // 
            this.UkraineXCoordinateTextBox.Location = new System.Drawing.Point(81, 202);
            this.UkraineXCoordinateTextBox.Name = "UkraineXCoordinateTextBox";
            this.UkraineXCoordinateTextBox.Size = new System.Drawing.Size(100, 20);
            this.UkraineXCoordinateTextBox.TabIndex = 11;
            // 
            // PulkovoXCoordinateTextBox
            // 
            this.PulkovoXCoordinateTextBox.Location = new System.Drawing.Point(81, 150);
            this.PulkovoXCoordinateTextBox.Name = "PulkovoXCoordinateTextBox";
            this.PulkovoXCoordinateTextBox.Size = new System.Drawing.Size(100, 20);
            this.PulkovoXCoordinateTextBox.TabIndex = 10;
            // 
            // PulkovoYCoordinateTextBox
            // 
            this.PulkovoYCoordinateTextBox.Location = new System.Drawing.Point(187, 150);
            this.PulkovoYCoordinateTextBox.Name = "PulkovoYCoordinateTextBox";
            this.PulkovoYCoordinateTextBox.Size = new System.Drawing.Size(100, 20);
            this.PulkovoYCoordinateTextBox.TabIndex = 8;
            // 
            // YCoordinateTextBox
            // 
            this.YCoordinateTextBox.Location = new System.Drawing.Point(187, 46);
            this.YCoordinateTextBox.Name = "YCoordinateTextBox";
            this.YCoordinateTextBox.Size = new System.Drawing.Size(100, 20);
            this.YCoordinateTextBox.TabIndex = 7;
            // 
            // XCoordinateTextBox
            // 
            this.XCoordinateTextBox.Location = new System.Drawing.Point(81, 46);
            this.XCoordinateTextBox.Name = "XCoordinateTextBox";
            this.XCoordinateTextBox.Size = new System.Drawing.Size(100, 20);
            this.XCoordinateTextBox.TabIndex = 5;
            // 
            // WgsYCoordinateTextBox
            // 
            this.WgsYCoordinateTextBox.Location = new System.Drawing.Point(187, 98);
            this.WgsYCoordinateTextBox.Name = "WgsYCoordinateTextBox";
            this.WgsYCoordinateTextBox.Size = new System.Drawing.Size(100, 20);
            this.WgsYCoordinateTextBox.TabIndex = 3;
            // 
            // WgsXCoordinateTextBox
            // 
            this.WgsXCoordinateTextBox.Location = new System.Drawing.Point(81, 98);
            this.WgsXCoordinateTextBox.Name = "WgsXCoordinateTextBox";
            this.WgsXCoordinateTextBox.Size = new System.Drawing.Size(100, 20);
            this.WgsXCoordinateTextBox.TabIndex = 2;
            // 
            // MoveToCenterButton
            // 
            this.MoveToCenterButton.Location = new System.Drawing.Point(212, 329);
            this.MoveToCenterButton.Name = "MoveToCenterButton";
            this.MoveToCenterButton.Size = new System.Drawing.Size(75, 23);
            this.MoveToCenterButton.TabIndex = 37;
            this.MoveToCenterButton.Text = "Center";
            this.MoveToCenterButton.UseVisualStyleBackColor = true;
            this.MoveToCenterButton.Click += new System.EventHandler(this.MoveToCenterButton_Click);
            // 
            // saveButtonFileDialog
            // 
            this.saveButtonFileDialog.CheckFileExists = false;
            this.saveButtonFileDialog.FileName = "Coordinates.xml";
            // 
            // DockableWindowGeoCalculator
            // 
            this.Controls.Add(this.LatitudeLongitudeGroup);
            this.Name = "DockableWindowGeoCalculator";
            this.Size = new System.Drawing.Size(300, 400);
            this.LatitudeLongitudeGroup.ResumeLayout(false);
            this.LatitudeLongitudeGroup.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox LatitudeLongitudeGroup;
        private System.Windows.Forms.Label MgrsNotationLabel;
        private System.Windows.Forms.Label UkraineCoordinatesLabel;
        private System.Windows.Forms.Label PulkovoCoordinatesLabel;
        private System.Windows.Forms.Label WgsCoordinatesLabel;
        private System.Windows.Forms.Label CurrentMapLabel;
        private System.Windows.Forms.TextBox MgrsNotationTextBox;
        private System.Windows.Forms.TextBox UkraineYCoordinateTextBox;
        private System.Windows.Forms.TextBox UkraineXCoordinateTextBox;
        private System.Windows.Forms.TextBox PulkovoXCoordinateTextBox;
        private System.Windows.Forms.TextBox PulkovoYCoordinateTextBox;
        private System.Windows.Forms.TextBox YCoordinateTextBox;
        private System.Windows.Forms.TextBox XCoordinateTextBox;
        private System.Windows.Forms.TextBox WgsYCoordinateTextBox;
        private System.Windows.Forms.TextBox WgsXCoordinateTextBox;
        private System.Windows.Forms.Label UTMNotationLabel;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox UTMNotationTextBox;
        private System.Windows.Forms.Button SaveButton;
        private System.Windows.Forms.Button CopyButton;
        private System.Windows.Forms.Button MoveToCenterButton;
        private System.Windows.Forms.OpenFileDialog saveButtonFileDialog;
    }
}
