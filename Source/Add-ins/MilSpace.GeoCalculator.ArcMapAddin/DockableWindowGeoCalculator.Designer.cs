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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DockableWindowGeoCalculator));
            this.ProjectionsGroup = new System.Windows.Forms.GroupBox();
            this.PointsGridView = new System.Windows.Forms.DataGridView();
            this.MapPointToolButton = new System.Windows.Forms.Button();
            this.MoveToCenterButton = new System.Windows.Forms.Button();
            this.SaveButton = new System.Windows.Forms.Button();
            this.CopyButton = new System.Windows.Forms.Button();
            this.UTMNotationLabel = new System.Windows.Forms.Label();
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
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.NumberColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ButtonColumn = new System.Windows.Forms.DataGridViewImageColumn();
            this.XCoordColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.YCoordColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ProjectionsGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PointsGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // ProjectionsGroup
            // 
            this.ProjectionsGroup.Controls.Add(this.PointsGridView);
            this.ProjectionsGroup.Controls.Add(this.MapPointToolButton);
            this.ProjectionsGroup.Controls.Add(this.MoveToCenterButton);
            this.ProjectionsGroup.Controls.Add(this.SaveButton);
            this.ProjectionsGroup.Controls.Add(this.CopyButton);
            this.ProjectionsGroup.Controls.Add(this.UTMNotationLabel);
            this.ProjectionsGroup.Controls.Add(this.UTMNotationTextBox);
            this.ProjectionsGroup.Controls.Add(this.MgrsNotationLabel);
            this.ProjectionsGroup.Controls.Add(this.UkraineCoordinatesLabel);
            this.ProjectionsGroup.Controls.Add(this.PulkovoCoordinatesLabel);
            this.ProjectionsGroup.Controls.Add(this.WgsCoordinatesLabel);
            this.ProjectionsGroup.Controls.Add(this.CurrentMapLabel);
            this.ProjectionsGroup.Controls.Add(this.MgrsNotationTextBox);
            this.ProjectionsGroup.Controls.Add(this.UkraineYCoordinateTextBox);
            this.ProjectionsGroup.Controls.Add(this.UkraineXCoordinateTextBox);
            this.ProjectionsGroup.Controls.Add(this.PulkovoXCoordinateTextBox);
            this.ProjectionsGroup.Controls.Add(this.PulkovoYCoordinateTextBox);
            this.ProjectionsGroup.Controls.Add(this.YCoordinateTextBox);
            this.ProjectionsGroup.Controls.Add(this.XCoordinateTextBox);
            this.ProjectionsGroup.Controls.Add(this.WgsYCoordinateTextBox);
            this.ProjectionsGroup.Controls.Add(this.WgsXCoordinateTextBox);
            this.ProjectionsGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ProjectionsGroup.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ProjectionsGroup.Location = new System.Drawing.Point(0, 0);
            this.ProjectionsGroup.Name = "ProjectionsGroup";
            this.ProjectionsGroup.Size = new System.Drawing.Size(300, 450);
            this.ProjectionsGroup.TabIndex = 0;
            this.ProjectionsGroup.TabStop = false;
            this.ProjectionsGroup.Text = "Projections";
            // 
            // PointsGridView
            // 
            this.PointsGridView.AllowUserToAddRows = false;
            this.PointsGridView.AllowUserToResizeColumns = false;
            this.PointsGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PointsGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.PointsGridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.PointsGridView.BackgroundColor = System.Drawing.SystemColors.Window;
            this.PointsGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.PointsGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.Raised;
            this.PointsGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.PointsGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.PointsGridView.ColumnHeadersVisible = false;
            this.PointsGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.NumberColumn,
            this.ButtonColumn,
            this.XCoordColumn,
            this.YCoordColumn});
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.PointsGridView.DefaultCellStyle = dataGridViewCellStyle1;
            this.PointsGridView.GridColor = System.Drawing.SystemColors.Window;
            this.PointsGridView.Location = new System.Drawing.Point(10, 338);
            this.PointsGridView.Name = "PointsGridView";
            this.PointsGridView.ReadOnly = true;
            this.PointsGridView.RowHeadersVisible = false;
            this.PointsGridView.Size = new System.Drawing.Size(277, 106);
            this.PointsGridView.TabIndex = 40;
            this.PointsGridView.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.PointsGridView_CellContentClick);
            // 
            // MapPointToolButton
            // 
            this.MapPointToolButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.MapPointToolButton.AutoSize = true;
            this.MapPointToolButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.MapPointToolButton.Image = ((System.Drawing.Image)(resources.GetObject("MapPointToolButton.Image")));
            this.MapPointToolButton.Location = new System.Drawing.Point(256, 13);
            this.MapPointToolButton.Name = "MapPointToolButton";
            this.MapPointToolButton.Size = new System.Drawing.Size(31, 31);
            this.MapPointToolButton.TabIndex = 39;
            this.MapPointToolButton.UseVisualStyleBackColor = true;
            this.MapPointToolButton.Click += new System.EventHandler(this.MapPointToolButton_Click);
            // 
            // MoveToCenterButton
            // 
            this.MoveToCenterButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.MoveToCenterButton.AutoSize = true;
            this.MoveToCenterButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.MoveToCenterButton.Image = ((System.Drawing.Image)(resources.GetObject("MoveToCenterButton.Image")));
            this.MoveToCenterButton.Location = new System.Drawing.Point(219, 13);
            this.MoveToCenterButton.Name = "MoveToCenterButton";
            this.MoveToCenterButton.Size = new System.Drawing.Size(31, 31);
            this.MoveToCenterButton.TabIndex = 37;
            this.MoveToCenterButton.UseVisualStyleBackColor = true;
            this.MoveToCenterButton.Click += new System.EventHandler(this.MoveToCenterButton_Click);
            // 
            // SaveButton
            // 
            this.SaveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.SaveButton.AutoSize = true;
            this.SaveButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.SaveButton.Image = ((System.Drawing.Image)(resources.GetObject("SaveButton.Image")));
            this.SaveButton.Location = new System.Drawing.Point(182, 13);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(31, 31);
            this.SaveButton.TabIndex = 36;
            this.SaveButton.UseVisualStyleBackColor = true;
            this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // CopyButton
            // 
            this.CopyButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.CopyButton.AutoSize = true;
            this.CopyButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.CopyButton.Image = ((System.Drawing.Image)(resources.GetObject("CopyButton.Image")));
            this.CopyButton.Location = new System.Drawing.Point(145, 13);
            this.CopyButton.Name = "CopyButton";
            this.CopyButton.Size = new System.Drawing.Size(31, 31);
            this.CopyButton.TabIndex = 35;
            this.CopyButton.UseVisualStyleBackColor = true;
            this.CopyButton.Click += new System.EventHandler(this.CopyButton_Click);
            // 
            // UTMNotationLabel
            // 
            this.UTMNotationLabel.AutoSize = true;
            this.UTMNotationLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.UTMNotationLabel.Location = new System.Drawing.Point(7, 287);
            this.UTMNotationLabel.Name = "UTMNotationLabel";
            this.UTMNotationLabel.Size = new System.Drawing.Size(133, 16);
            this.UTMNotationLabel.TabIndex = 34;
            this.UTMNotationLabel.Text = "UTM Representation";
            // 
            // UTMNotationTextBox
            // 
            this.UTMNotationTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.UTMNotationTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.UTMNotationTextBox.Location = new System.Drawing.Point(81, 306);
            this.UTMNotationTextBox.Name = "UTMNotationTextBox";
            this.UTMNotationTextBox.Size = new System.Drawing.Size(206, 22);
            this.UTMNotationTextBox.TabIndex = 31;
            this.UTMNotationTextBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.UTMNotationTextBox_MouseClick);
            this.UTMNotationTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.UTMNotationTextBox_KeyPress);
            // 
            // MgrsNotationLabel
            // 
            this.MgrsNotationLabel.AutoSize = true;
            this.MgrsNotationLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.MgrsNotationLabel.Location = new System.Drawing.Point(7, 235);
            this.MgrsNotationLabel.Name = "MgrsNotationLabel";
            this.MgrsNotationLabel.Size = new System.Drawing.Size(143, 16);
            this.MgrsNotationLabel.TabIndex = 30;
            this.MgrsNotationLabel.Text = "MGRS Representation";
            // 
            // UkraineCoordinatesLabel
            // 
            this.UkraineCoordinatesLabel.AutoSize = true;
            this.UkraineCoordinatesLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.UkraineCoordinatesLabel.Location = new System.Drawing.Point(7, 183);
            this.UkraineCoordinatesLabel.Name = "UkraineCoordinatesLabel";
            this.UkraineCoordinatesLabel.Size = new System.Drawing.Size(83, 16);
            this.UkraineCoordinatesLabel.TabIndex = 26;
            this.UkraineCoordinatesLabel.Text = "Ukraine2000";
            // 
            // PulkovoCoordinatesLabel
            // 
            this.PulkovoCoordinatesLabel.AutoSize = true;
            this.PulkovoCoordinatesLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.PulkovoCoordinatesLabel.Location = new System.Drawing.Point(7, 131);
            this.PulkovoCoordinatesLabel.Name = "PulkovoCoordinatesLabel";
            this.PulkovoCoordinatesLabel.Size = new System.Drawing.Size(85, 16);
            this.PulkovoCoordinatesLabel.TabIndex = 24;
            this.PulkovoCoordinatesLabel.Text = "Pulkovo1942";
            // 
            // WgsCoordinatesLabel
            // 
            this.WgsCoordinatesLabel.AutoSize = true;
            this.WgsCoordinatesLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.WgsCoordinatesLabel.Location = new System.Drawing.Point(7, 79);
            this.WgsCoordinatesLabel.Name = "WgsCoordinatesLabel";
            this.WgsCoordinatesLabel.Size = new System.Drawing.Size(68, 16);
            this.WgsCoordinatesLabel.TabIndex = 22;
            this.WgsCoordinatesLabel.Text = "WGS1984";
            // 
            // CurrentMapLabel
            // 
            this.CurrentMapLabel.AutoSize = true;
            this.CurrentMapLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.CurrentMapLabel.Location = new System.Drawing.Point(7, 26);
            this.CurrentMapLabel.Name = "CurrentMapLabel";
            this.CurrentMapLabel.Size = new System.Drawing.Size(156, 16);
            this.CurrentMapLabel.TabIndex = 20;
            this.CurrentMapLabel.Text = "Current Map Coordinates";
            // 
            // MgrsNotationTextBox
            // 
            this.MgrsNotationTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.MgrsNotationTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.MgrsNotationTextBox.Location = new System.Drawing.Point(81, 254);
            this.MgrsNotationTextBox.Name = "MgrsNotationTextBox";
            this.MgrsNotationTextBox.Size = new System.Drawing.Size(206, 22);
            this.MgrsNotationTextBox.TabIndex = 18;
            this.MgrsNotationTextBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.MgrsNotationTextBox_MouseClick);
            this.MgrsNotationTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.MgrsNotationTextBox_KeyPress);
            // 
            // UkraineYCoordinateTextBox
            // 
            this.UkraineYCoordinateTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.UkraineYCoordinateTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.UkraineYCoordinateTextBox.Location = new System.Drawing.Point(187, 202);
            this.UkraineYCoordinateTextBox.Name = "UkraineYCoordinateTextBox";
            this.UkraineYCoordinateTextBox.Size = new System.Drawing.Size(100, 22);
            this.UkraineYCoordinateTextBox.TabIndex = 12;
            // 
            // UkraineXCoordinateTextBox
            // 
            this.UkraineXCoordinateTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.UkraineXCoordinateTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.UkraineXCoordinateTextBox.Location = new System.Drawing.Point(81, 202);
            this.UkraineXCoordinateTextBox.Name = "UkraineXCoordinateTextBox";
            this.UkraineXCoordinateTextBox.Size = new System.Drawing.Size(100, 22);
            this.UkraineXCoordinateTextBox.TabIndex = 11;
            // 
            // PulkovoXCoordinateTextBox
            // 
            this.PulkovoXCoordinateTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.PulkovoXCoordinateTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.PulkovoXCoordinateTextBox.Location = new System.Drawing.Point(81, 150);
            this.PulkovoXCoordinateTextBox.Name = "PulkovoXCoordinateTextBox";
            this.PulkovoXCoordinateTextBox.Size = new System.Drawing.Size(100, 22);
            this.PulkovoXCoordinateTextBox.TabIndex = 10;
            // 
            // PulkovoYCoordinateTextBox
            // 
            this.PulkovoYCoordinateTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.PulkovoYCoordinateTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.PulkovoYCoordinateTextBox.Location = new System.Drawing.Point(187, 150);
            this.PulkovoYCoordinateTextBox.Name = "PulkovoYCoordinateTextBox";
            this.PulkovoYCoordinateTextBox.Size = new System.Drawing.Size(100, 22);
            this.PulkovoYCoordinateTextBox.TabIndex = 8;
            // 
            // YCoordinateTextBox
            // 
            this.YCoordinateTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.YCoordinateTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.YCoordinateTextBox.Location = new System.Drawing.Point(187, 46);
            this.YCoordinateTextBox.Name = "YCoordinateTextBox";
            this.YCoordinateTextBox.Size = new System.Drawing.Size(100, 22);
            this.YCoordinateTextBox.TabIndex = 7;
            this.YCoordinateTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.YCoordinateTextBox_KeyPress);
            // 
            // XCoordinateTextBox
            // 
            this.XCoordinateTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.XCoordinateTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.XCoordinateTextBox.Location = new System.Drawing.Point(81, 46);
            this.XCoordinateTextBox.Name = "XCoordinateTextBox";
            this.XCoordinateTextBox.Size = new System.Drawing.Size(100, 22);
            this.XCoordinateTextBox.TabIndex = 5;
            this.XCoordinateTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.XCoordinateTextBox_KeyPress);
            // 
            // WgsYCoordinateTextBox
            // 
            this.WgsYCoordinateTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.WgsYCoordinateTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.WgsYCoordinateTextBox.Location = new System.Drawing.Point(187, 98);
            this.WgsYCoordinateTextBox.Name = "WgsYCoordinateTextBox";
            this.WgsYCoordinateTextBox.Size = new System.Drawing.Size(100, 22);
            this.WgsYCoordinateTextBox.TabIndex = 3;
            // 
            // WgsXCoordinateTextBox
            // 
            this.WgsXCoordinateTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.WgsXCoordinateTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.WgsXCoordinateTextBox.Location = new System.Drawing.Point(81, 98);
            this.WgsXCoordinateTextBox.Name = "WgsXCoordinateTextBox";
            this.WgsXCoordinateTextBox.Size = new System.Drawing.Size(100, 22);
            this.WgsXCoordinateTextBox.TabIndex = 2;
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.FileName = "Coordinates.xml";
            // 
            // NumberColumn
            // 
            this.NumberColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.NumberColumn.DividerWidth = 10;
            this.NumberColumn.HeaderText = "";
            this.NumberColumn.Name = "NumberColumn";
            this.NumberColumn.ReadOnly = true;
            this.NumberColumn.Width = 5;
            // 
            // ButtonColumn
            // 
            this.ButtonColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.ButtonColumn.DividerWidth = 10;
            this.ButtonColumn.HeaderText = "";
            this.ButtonColumn.Name = "ButtonColumn";
            this.ButtonColumn.ReadOnly = true;
            this.ButtonColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.ButtonColumn.Width = 5;
            // 
            // XCoordColumn
            // 
            this.XCoordColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.XCoordColumn.DividerWidth = 10;
            this.XCoordColumn.HeaderText = "";
            this.XCoordColumn.Name = "XCoordColumn";
            this.XCoordColumn.ReadOnly = true;
            this.XCoordColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.XCoordColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // YCoordColumn
            // 
            this.YCoordColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.YCoordColumn.DividerWidth = 10;
            this.YCoordColumn.HeaderText = "";
            this.YCoordColumn.Name = "YCoordColumn";
            this.YCoordColumn.ReadOnly = true;
            this.YCoordColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.YCoordColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // DockableWindowGeoCalculator
            // 
            this.Controls.Add(this.ProjectionsGroup);
            this.Name = "DockableWindowGeoCalculator";
            this.Size = new System.Drawing.Size(300, 450);
            this.ProjectionsGroup.ResumeLayout(false);
            this.ProjectionsGroup.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PointsGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox ProjectionsGroup;
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
        private System.Windows.Forms.TextBox UTMNotationTextBox;
        private System.Windows.Forms.Button SaveButton;
        private System.Windows.Forms.Button CopyButton;
        private System.Windows.Forms.Button MoveToCenterButton;
        private System.Windows.Forms.Button MapPointToolButton;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.DataGridView PointsGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn NumberColumn;
        private System.Windows.Forms.DataGridViewImageColumn ButtonColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn XCoordColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn YCoordColumn;
    }
}
