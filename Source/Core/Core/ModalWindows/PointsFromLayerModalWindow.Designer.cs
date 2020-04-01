namespace MilSpace.Core.ModalWindows
{
    partial class PointsFromLayerModalWindow
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
            this.IdCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DisplayFieldCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.XCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.YCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.titlePanel = new System.Windows.Forms.Panel();
            this.lblLayer = new System.Windows.Forms.Label();
            this.btnChoosePoint = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.comboPanel = new System.Windows.Forms.Panel();
            this.lblField = new System.Windows.Forms.Label();
            this.lblChooseLayer = new System.Windows.Forms.Label();
            this.cmbFields = new System.Windows.Forms.ComboBox();
            this.cmbLayers = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPoints)).BeginInit();
            this.titlePanel.SuspendLayout();
            this.panel1.SuspendLayout();
            this.comboPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvPoints
            // 
            this.dgvPoints.AllowUserToAddRows = false;
            this.dgvPoints.AllowUserToDeleteRows = false;
            this.dgvPoints.AllowUserToResizeColumns = false;
            this.dgvPoints.AllowUserToResizeRows = false;
            this.dgvPoints.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dgvPoints.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvPoints.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPoints.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.IdCol,
            this.DisplayFieldCol,
            this.XCol,
            this.YCol});
            this.dgvPoints.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvPoints.Location = new System.Drawing.Point(0, 88);
            this.dgvPoints.MultiSelect = false;
            this.dgvPoints.Name = "dgvPoints";
            this.dgvPoints.ReadOnly = true;
            this.dgvPoints.RowHeadersVisible = false;
            this.dgvPoints.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvPoints.Size = new System.Drawing.Size(348, 243);
            this.dgvPoints.TabIndex = 8;
            // 
            // IdCol
            // 
            this.IdCol.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.IdCol.HeaderText = "ObjectId";
            this.IdCol.Name = "IdCol";
            this.IdCol.ReadOnly = true;
            // 
            // DisplayFieldCol
            // 
            this.DisplayFieldCol.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.DisplayFieldCol.HeaderText = "Display field";
            this.DisplayFieldCol.Name = "DisplayFieldCol";
            this.DisplayFieldCol.ReadOnly = true;
            this.DisplayFieldCol.Width = 88;
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
            // titlePanel
            // 
            this.titlePanel.Controls.Add(this.lblLayer);
            this.titlePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.titlePanel.Location = new System.Drawing.Point(0, 53);
            this.titlePanel.Name = "titlePanel";
            this.titlePanel.Size = new System.Drawing.Size(348, 35);
            this.titlePanel.TabIndex = 6;
            // 
            // lblLayer
            // 
            this.lblLayer.AutoSize = true;
            this.lblLayer.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.lblLayer.Location = new System.Drawing.Point(3, 9);
            this.lblLayer.Name = "lblLayer";
            this.lblLayer.Size = new System.Drawing.Size(52, 20);
            this.lblLayer.TabIndex = 0;
            this.lblLayer.Text = "Слой";
            // 
            // btnChoosePoint
            // 
            this.btnChoosePoint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnChoosePoint.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnChoosePoint.Location = new System.Drawing.Point(270, 3);
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
            this.panel1.Location = new System.Drawing.Point(0, 331);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(348, 28);
            this.panel1.TabIndex = 7;
            // 
            // comboPanel
            // 
            this.comboPanel.Controls.Add(this.lblField);
            this.comboPanel.Controls.Add(this.lblChooseLayer);
            this.comboPanel.Controls.Add(this.cmbFields);
            this.comboPanel.Controls.Add(this.cmbLayers);
            this.comboPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.comboPanel.Location = new System.Drawing.Point(0, 0);
            this.comboPanel.Name = "comboPanel";
            this.comboPanel.Size = new System.Drawing.Size(348, 53);
            this.comboPanel.TabIndex = 10;
            // 
            // lblField
            // 
            this.lblField.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblField.AutoSize = true;
            this.lblField.Location = new System.Drawing.Point(212, 9);
            this.lblField.Name = "lblField";
            this.lblField.Size = new System.Drawing.Size(111, 13);
            this.lblField.TabIndex = 3;
            this.lblField.Text = "Отображаемое поле";
            // 
            // lblChooseLayer
            // 
            this.lblChooseLayer.AutoSize = true;
            this.lblChooseLayer.Location = new System.Drawing.Point(3, 9);
            this.lblChooseLayer.Name = "lblChooseLayer";
            this.lblChooseLayer.Size = new System.Drawing.Size(32, 13);
            this.lblChooseLayer.TabIndex = 2;
            this.lblChooseLayer.Text = "Слой";
            // 
            // cmbFields
            // 
            this.cmbFields.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbFields.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFields.Location = new System.Drawing.Point(215, 27);
            this.cmbFields.Name = "cmbFields";
            this.cmbFields.Size = new System.Drawing.Size(121, 21);
            this.cmbFields.TabIndex = 1;
            this.cmbFields.SelectedIndexChanged += new System.EventHandler(this.CmbFields_SelectedIndexChanged);
            // 
            // cmbLayers
            // 
            this.cmbLayers.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbLayers.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbLayers.Location = new System.Drawing.Point(6, 27);
            this.cmbLayers.Name = "cmbLayers";
            this.cmbLayers.Size = new System.Drawing.Size(174, 21);
            this.cmbLayers.TabIndex = 0;
            this.cmbLayers.SelectedIndexChanged += new System.EventHandler(this.CmbLayers_SelectedIndexChanged);
            // 
            // PointsFromLayerModalWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(348, 359);
            this.Controls.Add(this.dgvPoints);
            this.Controls.Add(this.titlePanel);
            this.Controls.Add(this.comboPanel);
            this.Controls.Add(this.panel1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PointsFromLayerModalWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "PointsFromLayerModalWindow";
            ((System.ComponentModel.ISupportInitialize)(this.dgvPoints)).EndInit();
            this.titlePanel.ResumeLayout(false);
            this.titlePanel.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.comboPanel.ResumeLayout(false);
            this.comboPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvPoints;
        private System.Windows.Forms.Panel titlePanel;
        private System.Windows.Forms.Label lblLayer;
        private System.Windows.Forms.Button btnChoosePoint;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel comboPanel;
        private System.Windows.Forms.Label lblField;
        private System.Windows.Forms.Label lblChooseLayer;
        private System.Windows.Forms.ComboBox cmbFields;
        private System.Windows.Forms.ComboBox cmbLayers;
        private System.Windows.Forms.DataGridViewTextBoxColumn IdCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn DisplayFieldCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn XCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn YCol;
    }
}