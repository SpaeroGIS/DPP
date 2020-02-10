namespace MilSpace.GeoCalculator
{
    partial class ExportForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExportForm));
            this.OkButton = new System.Windows.Forms.Button();
            this.XmlFileRadio = new System.Windows.Forms.RadioButton();
            this.CsvFileRadio = new System.Windows.Forms.RadioButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.LayerRadio = new System.Windows.Forms.RadioButton();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // OkButton
            // 
            this.OkButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OkButton.Location = new System.Drawing.Point(12, 82);
            this.OkButton.Name = "OkButton";
            this.OkButton.Size = new System.Drawing.Size(95, 23);
            this.OkButton.TabIndex = 0;
            this.OkButton.Text = "OK";
            this.OkButton.UseVisualStyleBackColor = true;
            // 
            // XmlFileRadio
            // 
            this.XmlFileRadio.AutoSize = true;
            this.XmlFileRadio.Checked = true;
            this.XmlFileRadio.Location = new System.Drawing.Point(0, 3);
            this.XmlFileRadio.Name = "XmlFileRadio";
            this.XmlFileRadio.Size = new System.Drawing.Size(47, 17);
            this.XmlFileRadio.TabIndex = 1;
            this.XmlFileRadio.TabStop = true;
            this.XmlFileRadio.Text = "XML";
            this.XmlFileRadio.UseVisualStyleBackColor = true;
            this.XmlFileRadio.CheckedChanged += new System.EventHandler(this.XmlFileRadion_CheckedChanged);
            // 
            // CsvFileRadio
            // 
            this.CsvFileRadio.AutoSize = true;
            this.CsvFileRadio.Location = new System.Drawing.Point(0, 23);
            this.CsvFileRadio.Name = "CsvFileRadio";
            this.CsvFileRadio.Size = new System.Drawing.Size(46, 17);
            this.CsvFileRadio.TabIndex = 2;
            this.CsvFileRadio.Text = "CSV";
            this.CsvFileRadio.UseVisualStyleBackColor = true;
            this.CsvFileRadio.CheckedChanged += new System.EventHandler(this.CsvFileRadio_CheckedChanged);
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel1.Controls.Add(this.LayerRadio);
            this.panel1.Controls.Add(this.XmlFileRadio);
            this.panel1.Controls.Add(this.CsvFileRadio);
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(88, 64);
            this.panel1.TabIndex = 3;
            // 
            // LayerRadio
            // 
            this.LayerRadio.AutoSize = true;
            this.LayerRadio.Location = new System.Drawing.Point(0, 44);
            this.LayerRadio.Name = "LayerRadio";
            this.LayerRadio.Size = new System.Drawing.Size(85, 17);
            this.LayerRadio.TabIndex = 3;
            this.LayerRadio.TabStop = true;
            this.LayerRadio.Text = "radioButton1";
            this.LayerRadio.UseVisualStyleBackColor = true;
            this.LayerRadio.CheckedChanged += new System.EventHandler(this.LayerRadio_CheckedChanged);
            // 
            // ExportForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(127, 114);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.OkButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ExportForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Save As";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button OkButton;
        private System.Windows.Forms.RadioButton XmlFileRadio;
        private System.Windows.Forms.RadioButton CsvFileRadio;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton LayerRadio;
    }
}