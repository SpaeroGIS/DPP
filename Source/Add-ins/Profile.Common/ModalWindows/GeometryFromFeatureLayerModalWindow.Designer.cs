namespace MilSpace.Profile.ModalWindows
{
    partial class GeometryFromFeatureLayerModalWindow
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
            this.dgvGeometries = new System.Windows.Forms.DataGridView();
            this.IdCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DisplayFieldCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.titlePanel = new System.Windows.Forms.Panel();
            this.lblLayer = new System.Windows.Forms.Label();
            this.comboPanel = new System.Windows.Forms.Panel();
            this.lblField = new System.Windows.Forms.Label();
            this.lblChooseLayer = new System.Windows.Forms.Label();
            this.cmbFields = new System.Windows.Forms.ComboBox();
            this.cmbLayers = new System.Windows.Forms.ComboBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnChoosePoint = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvGeometries)).BeginInit();
            this.titlePanel.SuspendLayout();
            this.comboPanel.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvGeometries
            // 
            this.dgvGeometries.AllowUserToAddRows = false;
            this.dgvGeometries.AllowUserToDeleteRows = false;
            this.dgvGeometries.AllowUserToResizeColumns = false;
            this.dgvGeometries.AllowUserToResizeRows = false;
            this.dgvGeometries.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dgvGeometries.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvGeometries.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.IdCol,
            this.DisplayFieldCol});
            this.dgvGeometries.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvGeometries.Location = new System.Drawing.Point(0, 88);
            this.dgvGeometries.MultiSelect = false;
            this.dgvGeometries.Name = "dgvGeometries";
            this.dgvGeometries.RowHeadersVisible = false;
            this.dgvGeometries.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvGeometries.Size = new System.Drawing.Size(315, 243);
            this.dgvGeometries.TabIndex = 18;
            // 
            // IdCol
            // 
            this.IdCol.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.IdCol.HeaderText = "ObjectId";
            this.IdCol.Name = "IdCol";
            this.IdCol.ReadOnly = true;
            this.IdCol.Width = 72;
            // 
            // DisplayFieldCol
            // 
            this.DisplayFieldCol.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.DisplayFieldCol.HeaderText = "Display field";
            this.DisplayFieldCol.Name = "DisplayFieldCol";
            this.DisplayFieldCol.ReadOnly = true;
            // 
            // titlePanel
            // 
            this.titlePanel.Controls.Add(this.lblLayer);
            this.titlePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.titlePanel.Location = new System.Drawing.Point(0, 53);
            this.titlePanel.Name = "titlePanel";
            this.titlePanel.Size = new System.Drawing.Size(315, 35);
            this.titlePanel.TabIndex = 15;
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
            // comboPanel
            // 
            this.comboPanel.Controls.Add(this.lblField);
            this.comboPanel.Controls.Add(this.lblChooseLayer);
            this.comboPanel.Controls.Add(this.cmbFields);
            this.comboPanel.Controls.Add(this.cmbLayers);
            this.comboPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.comboPanel.Location = new System.Drawing.Point(0, 0);
            this.comboPanel.Name = "comboPanel";
            this.comboPanel.Size = new System.Drawing.Size(315, 53);
            this.comboPanel.TabIndex = 17;
            // 
            // lblField
            // 
            this.lblField.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblField.AutoSize = true;
            this.lblField.Location = new System.Drawing.Point(179, 9);
            this.lblField.Name = "lblField";
            this.lblField.Size = new System.Drawing.Size(111, 13);
            this.lblField.TabIndex = 3;
            this.lblField.Text = "Отображаемое поле";
            // 
            // lblChooseLayer
            // 
            this.lblChooseLayer.AutoSize = true;
            this.lblChooseLayer.Location = new System.Drawing.Point(2, 9);
            this.lblChooseLayer.Name = "lblChooseLayer";
            this.lblChooseLayer.Size = new System.Drawing.Size(32, 13);
            this.lblChooseLayer.TabIndex = 2;
            this.lblChooseLayer.Text = "Слой";
            // 
            // cmbFields
            // 
            this.cmbFields.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbFields.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFields.Location = new System.Drawing.Point(182, 27);
            this.cmbFields.Name = "cmbFields";
            this.cmbFields.Size = new System.Drawing.Size(130, 21);
            this.cmbFields.TabIndex = 1;
            this.cmbFields.SelectedIndexChanged += new System.EventHandler(this.CmbFields_SelectedIndexChanged);
            // 
            // cmbLayers
            // 
            this.cmbLayers.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbLayers.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbLayers.Location = new System.Drawing.Point(5, 27);
            this.cmbLayers.Name = "cmbLayers";
            this.cmbLayers.Size = new System.Drawing.Size(171, 21);
            this.cmbLayers.TabIndex = 0;
            this.cmbLayers.SelectedIndexChanged += new System.EventHandler(this.CmbLayers_SelectedIndexChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnChoosePoint);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 331);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(315, 28);
            this.panel1.TabIndex = 16;
            // 
            // btnChoosePoint
            // 
            this.btnChoosePoint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnChoosePoint.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnChoosePoint.Location = new System.Drawing.Point(237, 3);
            this.btnChoosePoint.Name = "btnChoosePoint";
            this.btnChoosePoint.Size = new System.Drawing.Size(75, 23);
            this.btnChoosePoint.TabIndex = 0;
            this.btnChoosePoint.Text = "Выбрать";
            this.btnChoosePoint.UseVisualStyleBackColor = true;
            this.btnChoosePoint.Click += new System.EventHandler(this.BtnChoosePoint_Click);
            // 
            // GeometryFromFeatureLayerModalWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(315, 359);
            this.Controls.Add(this.dgvGeometries);
            this.Controls.Add(this.titlePanel);
            this.Controls.Add(this.comboPanel);
            this.Controls.Add(this.panel1);
            this.Name = "GeometryFromFeatureLayerModalWindow";
            this.Text = "PointsFromFeatureLayerModalWindow";
            ((System.ComponentModel.ISupportInitialize)(this.dgvGeometries)).EndInit();
            this.titlePanel.ResumeLayout(false);
            this.titlePanel.PerformLayout();
            this.comboPanel.ResumeLayout(false);
            this.comboPanel.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvGeometries;
        private System.Windows.Forms.Panel titlePanel;
        private System.Windows.Forms.Label lblLayer;
        private System.Windows.Forms.Panel comboPanel;
        private System.Windows.Forms.Label lblField;
        private System.Windows.Forms.Label lblChooseLayer;
        private System.Windows.Forms.ComboBox cmbFields;
        private System.Windows.Forms.ComboBox cmbLayers;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnChoosePoint;
        private System.Windows.Forms.DataGridViewTextBoxColumn IdCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn DisplayFieldCol;
    }
}