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
            this.buttonPanel.SuspendLayout();
            this.labelPanel.SuspendLayout();
            this.panel1.SuspendLayout();
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
            this.buttonPanel.Location = new System.Drawing.Point(0, 240);
            this.buttonPanel.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.buttonPanel.Name = "buttonPanel";
            this.buttonPanel.Size = new System.Drawing.Size(221, 35);
            this.buttonPanel.TabIndex = 5;
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(133, 6);
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
            this.lbLayers.Size = new System.Drawing.Size(215, 208);
            this.lbLayers.TabIndex = 4;
            // 
            // labelPanel
            // 
            this.labelPanel.Controls.Add(this.lblTitle);
            this.labelPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelPanel.Location = new System.Drawing.Point(0, 0);
            this.labelPanel.Name = "labelPanel";
            this.labelPanel.Size = new System.Drawing.Size(221, 32);
            this.labelPanel.TabIndex = 3;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lbLayers);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 32);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.panel1.Size = new System.Drawing.Size(221, 208);
            this.panel1.TabIndex = 6;
            // 
            // ChooseVectorLayerFromMapModalWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(221, 275);
            this.Controls.Add(this.panel1);
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
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Panel buttonPanel;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.ListBox lbLayers;
        private System.Windows.Forms.Panel labelPanel;
        private System.Windows.Forms.Panel panel1;
    }
}