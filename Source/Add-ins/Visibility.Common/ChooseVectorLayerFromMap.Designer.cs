namespace MilSpace.Visibility
{
    partial class ChooseVectorLayerFromMapModalWindow
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
            this.lblTitle = new System.Windows.Forms.Label();
            this.buttonPanel = new System.Windows.Forms.Panel();
            this.btnOk = new System.Windows.Forms.Button();
            this.lbLayers = new System.Windows.Forms.ListBox();
            this.labelPanel = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.chooseTitleFieldPanel = new System.Windows.Forms.Panel();
            this.lblTiltleField = new System.Windows.Forms.Label();
            this.cmbTiltleField = new System.Windows.Forms.ComboBox();
            this.buttonPanel.SuspendLayout();
            this.labelPanel.SuspendLayout();
            this.panel1.SuspendLayout();
            this.chooseTitleFieldPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblTitle.Location = new System.Drawing.Point(11, 6);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(90, 20);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Layers list";
            // 
            // buttonPanel
            // 
            this.buttonPanel.Controls.Add(this.btnOk);
            this.buttonPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.buttonPanel.Location = new System.Drawing.Point(0, 272);
            this.buttonPanel.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.buttonPanel.Name = "buttonPanel";
            this.buttonPanel.Size = new System.Drawing.Size(240, 35);
            this.buttonPanel.TabIndex = 5;
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(152, 6);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(85, 25);
            this.btnOk.TabIndex = 0;
            this.btnOk.Text = "Choose";
            this.btnOk.UseVisualStyleBackColor = true;
            // 
            // lbLayers
            // 
            this.lbLayers.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lbLayers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbLayers.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lbLayers.ItemHeight = 15;
            this.lbLayers.Location = new System.Drawing.Point(3, 0);
            this.lbLayers.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.lbLayers.Name = "lbLayers";
            this.lbLayers.Size = new System.Drawing.Size(234, 210);
            this.lbLayers.TabIndex = 4;
            this.lbLayers.SelectedIndexChanged += new System.EventHandler(this.LbLayers_SelectedIndexChanged);
            // 
            // labelPanel
            // 
            this.labelPanel.Controls.Add(this.lblTitle);
            this.labelPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelPanel.Location = new System.Drawing.Point(0, 0);
            this.labelPanel.Name = "labelPanel";
            this.labelPanel.Size = new System.Drawing.Size(240, 32);
            this.labelPanel.TabIndex = 3;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lbLayers);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 32);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.panel1.Size = new System.Drawing.Size(240, 210);
            this.panel1.TabIndex = 6;
            // 
            // chooseTitleFieldPanel
            // 
            this.chooseTitleFieldPanel.Controls.Add(this.lblTiltleField);
            this.chooseTitleFieldPanel.Controls.Add(this.cmbTiltleField);
            this.chooseTitleFieldPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.chooseTitleFieldPanel.Location = new System.Drawing.Point(0, 242);
            this.chooseTitleFieldPanel.Name = "chooseTitleFieldPanel";
            this.chooseTitleFieldPanel.Size = new System.Drawing.Size(240, 30);
            this.chooseTitleFieldPanel.TabIndex = 5;
            // 
            // lblTiltleField
            // 
            this.lblTiltleField.AutoSize = true;
            this.lblTiltleField.Location = new System.Drawing.Point(0, 9);
            this.lblTiltleField.Name = "lblTiltleField";
            this.lblTiltleField.Size = new System.Drawing.Size(106, 13);
            this.lblTiltleField.TabIndex = 1;
            this.lblTiltleField.Text = "Ідентифікуюче поле";
            // 
            // cmbTiltleField
            // 
            this.cmbTiltleField.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbTiltleField.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTiltleField.FormattingEnabled = true;
            this.cmbTiltleField.Location = new System.Drawing.Point(112, 5);
            this.cmbTiltleField.Name = "cmbTiltleField";
            this.cmbTiltleField.Size = new System.Drawing.Size(125, 21);
            this.cmbTiltleField.TabIndex = 0;
            // 
            // ChooseVectorLayerFromMapModalWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(240, 307);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.chooseTitleFieldPanel);
            this.Controls.Add(this.buttonPanel);
            this.Controls.Add(this.labelPanel);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ChooseVectorLayerFromMapModalWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ChooseVectorLayerFromMap";
            this.buttonPanel.ResumeLayout(false);
            this.labelPanel.ResumeLayout(false);
            this.labelPanel.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.chooseTitleFieldPanel.ResumeLayout(false);
            this.chooseTitleFieldPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Panel buttonPanel;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.ListBox lbLayers;
        private System.Windows.Forms.Panel labelPanel;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel chooseTitleFieldPanel;
        private System.Windows.Forms.ComboBox cmbTiltleField;
        private System.Windows.Forms.Label lblTiltleField;
    }
}