namespace MilSpace.Core.ModalWindows
{
    partial class PointsListModalWindow
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
            if(disposing && (components != null))
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
            this.titlePanel = new System.Windows.Forms.Panel();
            this.lblLayer = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnChoosePoint = new System.Windows.Forms.Button();
            this.lvPoints = new System.Windows.Forms.ListView();
            this.titlePanel.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // titlePanel
            // 
            this.titlePanel.Controls.Add(this.lblLayer);
            this.titlePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.titlePanel.Location = new System.Drawing.Point(0, 0);
            this.titlePanel.Name = "titlePanel";
            this.titlePanel.Size = new System.Drawing.Size(257, 32);
            this.titlePanel.TabIndex = 0;
            // 
            // lblLayer
            // 
            this.lblLayer.AutoSize = true;
            this.lblLayer.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.lblLayer.Location = new System.Drawing.Point(0, 7);
            this.lblLayer.Name = "lblLayer";
            this.lblLayer.Size = new System.Drawing.Size(52, 20);
            this.lblLayer.TabIndex = 0;
            this.lblLayer.Text = "Слой";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnChoosePoint);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 322);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(257, 28);
            this.panel1.TabIndex = 1;
            // 
            // btnChoosePoint
            // 
            this.btnChoosePoint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnChoosePoint.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnChoosePoint.Location = new System.Drawing.Point(179, 2);
            this.btnChoosePoint.Name = "btnChoosePoint";
            this.btnChoosePoint.Size = new System.Drawing.Size(75, 23);
            this.btnChoosePoint.TabIndex = 0;
            this.btnChoosePoint.Text = "Выбрать";
            this.btnChoosePoint.UseVisualStyleBackColor = true;
            this.btnChoosePoint.Click += new System.EventHandler(this.BtnChoosePoint_Click);
            // 
            // lvPoints
            // 
            this.lvPoints.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lvPoints.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvPoints.FullRowSelect = true;
            this.lvPoints.GridLines = true;
            this.lvPoints.HideSelection = false;
            this.lvPoints.Location = new System.Drawing.Point(0, 32);
            this.lvPoints.MultiSelect = false;
            this.lvPoints.Name = "lvPoints";
            this.lvPoints.Size = new System.Drawing.Size(257, 290);
            this.lvPoints.TabIndex = 2;
            this.lvPoints.UseCompatibleStateImageBehavior = false;
            // 
            // PointsListModalWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(257, 350);
            this.Controls.Add(this.lvPoints);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.titlePanel);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PointsListModalWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "PointsListModalWindow";
            this.titlePanel.ResumeLayout(false);
            this.titlePanel.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel titlePanel;
        private System.Windows.Forms.Label lblLayer;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnChoosePoint;
        private System.Windows.Forms.ListView lvPoints;
    }
}