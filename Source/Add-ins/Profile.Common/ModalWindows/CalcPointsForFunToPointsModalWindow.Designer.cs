namespace MilSpace.Profile.ModalWindows
{
    partial class CalcPointsForFunToPointsModalWindow
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
            this.chckAllPoints = new System.Windows.Forms.CheckBox();
            this.dgvPoints = new System.Windows.Forms.DataGridView();
            this.ChooseCol = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.NumCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.XCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.YCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.titlePanel = new System.Windows.Forms.Panel();
            this.lblLayer = new System.Windows.Forms.Label();
            this.btnChoosePoint = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPoints)).BeginInit();
            this.titlePanel.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // chckAllPoints
            // 
            this.chckAllPoints.AutoSize = true;
            this.chckAllPoints.Location = new System.Drawing.Point(7, 33);
            this.chckAllPoints.Name = "chckAllPoints";
            this.chckAllPoints.Size = new System.Drawing.Size(15, 14);
            this.chckAllPoints.TabIndex = 13;
            this.chckAllPoints.UseVisualStyleBackColor = true;
            this.chckAllPoints.CheckedChanged += new System.EventHandler(this.ChckAllPoints_CheckedChanged);
            // 
            // dgvPoints
            // 
            this.dgvPoints.AllowUserToAddRows = false;
            this.dgvPoints.AllowUserToDeleteRows = false;
            this.dgvPoints.AllowUserToResizeColumns = false;
            this.dgvPoints.AllowUserToResizeRows = false;
            this.dgvPoints.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dgvPoints.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvPoints.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvPoints.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ChooseCol,
            this.NumCol,
            this.XCol,
            this.YCol});
            this.dgvPoints.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvPoints.Location = new System.Drawing.Point(0, 28);
            this.dgvPoints.MultiSelect = false;
            this.dgvPoints.Name = "dgvPoints";
            this.dgvPoints.RowHeadersVisible = false;
            this.dgvPoints.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvPoints.Size = new System.Drawing.Size(226, 281);
            this.dgvPoints.TabIndex = 12;
            // 
            // ChooseCol
            // 
            this.ChooseCol.HeaderText = "";
            this.ChooseCol.MinimumWidth = 25;
            this.ChooseCol.Name = "ChooseCol";
            this.ChooseCol.Width = 25;
            // 
            // NumCol
            // 
            this.NumCol.HeaderText = "Number";
            this.NumCol.Name = "NumCol";
            this.NumCol.ReadOnly = true;
            this.NumCol.Width = 70;
            // 
            // XCol
            // 
            this.XCol.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.XCol.HeaderText = "X";
            this.XCol.Name = "XCol";
            this.XCol.ReadOnly = true;
            // 
            // YCol
            // 
            this.YCol.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.YCol.HeaderText = "Y";
            this.YCol.Name = "YCol";
            this.YCol.ReadOnly = true;
            // 
            // titlePanel
            // 
            this.titlePanel.Controls.Add(this.lblLayer);
            this.titlePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.titlePanel.Location = new System.Drawing.Point(0, 0);
            this.titlePanel.Name = "titlePanel";
            this.titlePanel.Size = new System.Drawing.Size(226, 28);
            this.titlePanel.TabIndex = 10;
            // 
            // lblLayer
            // 
            this.lblLayer.AutoSize = true;
            this.lblLayer.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblLayer.Location = new System.Drawing.Point(1, 6);
            this.lblLayer.Margin = new System.Windows.Forms.Padding(3, 0, 1, 0);
            this.lblLayer.Name = "lblLayer";
            this.lblLayer.Size = new System.Drawing.Size(41, 17);
            this.lblLayer.TabIndex = 0;
            this.lblLayer.Text = "Слой";
            // 
            // btnChoosePoint
            // 
            this.btnChoosePoint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnChoosePoint.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnChoosePoint.Location = new System.Drawing.Point(148, 3);
            this.btnChoosePoint.Name = "btnChoosePoint";
            this.btnChoosePoint.Size = new System.Drawing.Size(75, 23);
            this.btnChoosePoint.TabIndex = 0;
            this.btnChoosePoint.Text = "Выбрать";
            this.btnChoosePoint.UseVisualStyleBackColor = true;
            this.btnChoosePoint.Click += new System.EventHandler(this.BtnChoosePoint_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnChoosePoint);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 309);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(226, 28);
            this.panel1.TabIndex = 11;
            // 
            // CalcPointsForFunToPointsModalWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(226, 337);
            this.Controls.Add(this.chckAllPoints);
            this.Controls.Add(this.dgvPoints);
            this.Controls.Add(this.titlePanel);
            this.Controls.Add(this.panel1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CalcPointsForFunToPointsModalWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "CalcPointsForFunToPointsModalWindow";
            ((System.ComponentModel.ISupportInitialize)(this.dgvPoints)).EndInit();
            this.titlePanel.ResumeLayout(false);
            this.titlePanel.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chckAllPoints;
        private System.Windows.Forms.DataGridView dgvPoints;
        private System.Windows.Forms.Panel titlePanel;
        private System.Windows.Forms.Label lblLayer;
        private System.Windows.Forms.Button btnChoosePoint;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.DataGridViewCheckBoxColumn ChooseCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn NumCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn XCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn YCol;
    }
}