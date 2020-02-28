namespace MilSpace.Profile.ModalWindows
{
    partial class ObservPointsForFunToPointsModalWindow
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
            this.dgvPoints = new System.Windows.Forms.DataGridView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnChoosePoint = new System.Windows.Forms.Button();
            this.titlePanel = new System.Windows.Forms.Panel();
            this.lblLayer = new System.Windows.Forms.Label();
            this.chckAllPoints = new System.Windows.Forms.CheckBox();
            this.ChooseCol = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.IdCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TitleCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.XCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.YCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPoints)).BeginInit();
            this.panel1.SuspendLayout();
            this.titlePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvPoints
            // 
            this.dgvPoints.AllowUserToAddRows = false;
            this.dgvPoints.AllowUserToDeleteRows = false;
            this.dgvPoints.AllowUserToResizeColumns = false;
            this.dgvPoints.AllowUserToResizeRows = false;
            this.dgvPoints.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dgvPoints.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvPoints.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ChooseCol,
            this.IdCol,
            this.TitleCol,
            this.XCol,
            this.YCol});
            this.dgvPoints.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvPoints.Location = new System.Drawing.Point(0, 28);
            this.dgvPoints.MultiSelect = false;
            this.dgvPoints.Name = "dgvPoints";
            this.dgvPoints.RowHeadersVisible = false;
            this.dgvPoints.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvPoints.Size = new System.Drawing.Size(283, 295);
            this.dgvPoints.TabIndex = 8;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnChoosePoint);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 323);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(283, 28);
            this.panel1.TabIndex = 7;
            // 
            // btnChoosePoint
            // 
            this.btnChoosePoint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnChoosePoint.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnChoosePoint.Location = new System.Drawing.Point(205, 3);
            this.btnChoosePoint.Name = "btnChoosePoint";
            this.btnChoosePoint.Size = new System.Drawing.Size(75, 23);
            this.btnChoosePoint.TabIndex = 0;
            this.btnChoosePoint.Text = "Выбрать";
            this.btnChoosePoint.UseVisualStyleBackColor = true;
            this.btnChoosePoint.Click += new System.EventHandler(this.BtnChoosePoint_Click);
            // 
            // titlePanel
            // 
            this.titlePanel.Controls.Add(this.lblLayer);
            this.titlePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.titlePanel.Location = new System.Drawing.Point(0, 0);
            this.titlePanel.Name = "titlePanel";
            this.titlePanel.Size = new System.Drawing.Size(283, 28);
            this.titlePanel.TabIndex = 6;
            // 
            // lblLayer
            // 
            this.lblLayer.AutoSize = true;
            this.lblLayer.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblLayer.Location = new System.Drawing.Point(3, 6);
            this.lblLayer.Name = "lblLayer";
            this.lblLayer.Size = new System.Drawing.Size(41, 17);
            this.lblLayer.TabIndex = 0;
            this.lblLayer.Text = "Слой";
            // 
            // chckAllPoints
            // 
            this.chckAllPoints.AutoSize = true;
            this.chckAllPoints.Location = new System.Drawing.Point(7, 33);
            this.chckAllPoints.Name = "chckAllPoints";
            this.chckAllPoints.Size = new System.Drawing.Size(15, 14);
            this.chckAllPoints.TabIndex = 9;
            this.chckAllPoints.UseVisualStyleBackColor = true;
            this.chckAllPoints.CheckedChanged += new System.EventHandler(this.ChckAllPoints_CheckedChanged);
            // 
            // ChooseCol
            // 
            this.ChooseCol.HeaderText = "";
            this.ChooseCol.MinimumWidth = 25;
            this.ChooseCol.Name = "ChooseCol";
            this.ChooseCol.Width = 25;
            // 
            // IdCol
            // 
            this.IdCol.HeaderText = "ObjectId";
            this.IdCol.Name = "IdCol";
            this.IdCol.ReadOnly = true;
            this.IdCol.Width = 70;
            // 
            // TitleCol
            // 
            this.TitleCol.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.TitleCol.HeaderText = "TitleOp";
            this.TitleCol.Name = "TitleCol";
            this.TitleCol.ReadOnly = true;
            // 
            // XCol
            // 
            this.XCol.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.XCol.HeaderText = "X";
            this.XCol.Name = "XCol";
            this.XCol.ReadOnly = true;
            this.XCol.Width = 39;
            // 
            // YCol
            // 
            this.YCol.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.YCol.HeaderText = "Y";
            this.YCol.Name = "YCol";
            this.YCol.ReadOnly = true;
            this.YCol.Width = 39;
            // 
            // ObservPointsForFunToPointsModalWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(283, 351);
            this.Controls.Add(this.chckAllPoints);
            this.Controls.Add(this.dgvPoints);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.titlePanel);
            this.Name = "ObservPointsForFunToPointsModalWindow";
            this.Text = "ObservPointsForFunToPointsModalWindow";
            ((System.ComponentModel.ISupportInitialize)(this.dgvPoints)).EndInit();
            this.panel1.ResumeLayout(false);
            this.titlePanel.ResumeLayout(false);
            this.titlePanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvPoints;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnChoosePoint;
        private System.Windows.Forms.Panel titlePanel;
        private System.Windows.Forms.Label lblLayer;
        private System.Windows.Forms.CheckBox chckAllPoints;
        private System.Windows.Forms.DataGridViewCheckBoxColumn ChooseCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn IdCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn TitleCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn XCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn YCol;
    }
}