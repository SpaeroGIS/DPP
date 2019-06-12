namespace MilSpace.GeoCalculator
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DockableWindowGeoCalculator));
            this.ProjectionsGroup = new System.Windows.Forms.GroupBox();
            this.PointsGridView = new System.Windows.Forms.DataGridView();
            this.NumberColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.HighlightColumn = new System.Windows.Forms.DataGridViewImageColumn();
            this.XCoordColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.YCoordColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DeleteColumn = new System.Windows.Forms.DataGridViewImageColumn();
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
            this.CurrentCoordinatesPanel = new System.Windows.Forms.Panel();
            this.XLabel = new System.Windows.Forms.Label();
            this.YLabel = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.dmsLabel4 = new System.Windows.Forms.Label();
            this.dmsLabel3 = new System.Windows.Forms.Label();
            this.wgsDMSYTextBox = new System.Windows.Forms.TextBox();
            this.wgsDMSXTextBox = new System.Windows.Forms.TextBox();
            this.MetersLabel4 = new System.Windows.Forms.Label();
            this.MetersLabel3 = new System.Windows.Forms.Label();
            this.WgsYLabel = new System.Windows.Forms.Label();
            this.wgsXLabel = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.dmsLabel6 = new System.Windows.Forms.Label();
            this.dmsLabel5 = new System.Windows.Forms.Label();
            this.pulkovoDMSYTextBox = new System.Windows.Forms.TextBox();
            this.pulkovoDMSXTextBox = new System.Windows.Forms.TextBox();
            this.MetersLabel6 = new System.Windows.Forms.Label();
            this.MetersLabel5 = new System.Windows.Forms.Label();
            this.pulkovoYLabel = new System.Windows.Forms.Label();
            this.pulkovoXLabel = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.dmsLabel8 = new System.Windows.Forms.Label();
            this.dmsLabel7 = new System.Windows.Forms.Label();
            this.ukraineDMSYTextBox = new System.Windows.Forms.TextBox();
            this.ukraineDMSXTextBox = new System.Windows.Forms.TextBox();
            this.MetersLabel8 = new System.Windows.Forms.Label();
            this.MetersLabel7 = new System.Windows.Forms.Label();
            this.ukraineYLabel = new System.Windows.Forms.Label();
            this.ukraineXLabel = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.CurrentMapUnitsLabel1 = new System.Windows.Forms.Label();
            this.CurrentMapUnitsLabel2 = new System.Windows.Forms.Label();
            this.mapCenterButtonToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.toolButtonToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.saveButtonToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.copyButtonToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.ProjectionsGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PointsGridView)).BeginInit();
            this.CurrentCoordinatesPanel.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // ProjectionsGroup
            // 
            this.ProjectionsGroup.Controls.Add(this.panel4);
            this.ProjectionsGroup.Controls.Add(this.panel3);
            this.ProjectionsGroup.Controls.Add(this.panel2);
            this.ProjectionsGroup.Controls.Add(this.panel1);
            this.ProjectionsGroup.Controls.Add(this.CurrentCoordinatesPanel);
            this.ProjectionsGroup.Controls.Add(this.PointsGridView);
            this.ProjectionsGroup.Controls.Add(this.MapPointToolButton);
            this.ProjectionsGroup.Controls.Add(this.MoveToCenterButton);
            this.ProjectionsGroup.Controls.Add(this.SaveButton);
            this.ProjectionsGroup.Controls.Add(this.CopyButton);
            this.ProjectionsGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ProjectionsGroup.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ProjectionsGroup.Location = new System.Drawing.Point(0, 0);
            this.ProjectionsGroup.Name = "ProjectionsGroup";
            this.ProjectionsGroup.Size = new System.Drawing.Size(335, 615);
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
            this.HighlightColumn,
            this.XCoordColumn,
            this.YCoordColumn,
            this.DeleteColumn});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.PointsGridView.DefaultCellStyle = dataGridViewCellStyle2;
            this.PointsGridView.GridColor = System.Drawing.SystemColors.Window;
            this.PointsGridView.Location = new System.Drawing.Point(5, 523);
            this.PointsGridView.Name = "PointsGridView";
            this.PointsGridView.ReadOnly = true;
            this.PointsGridView.RowHeadersVisible = false;
            this.PointsGridView.Size = new System.Drawing.Size(324, 84);
            this.PointsGridView.TabIndex = 40;
            this.PointsGridView.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.PointsGridView_CellContentClick);
            // 
            // NumberColumn
            // 
            this.NumberColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.NumberColumn.HeaderText = "";
            this.NumberColumn.Name = "NumberColumn";
            this.NumberColumn.ReadOnly = true;
            this.NumberColumn.Width = 5;
            // 
            // HighlightColumn
            // 
            this.HighlightColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.HighlightColumn.HeaderText = "";
            this.HighlightColumn.Name = "HighlightColumn";
            this.HighlightColumn.ReadOnly = true;
            this.HighlightColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.HighlightColumn.Width = 5;
            // 
            // XCoordColumn
            // 
            this.XCoordColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.XCoordColumn.HeaderText = "";
            this.XCoordColumn.Name = "XCoordColumn";
            this.XCoordColumn.ReadOnly = true;
            this.XCoordColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.XCoordColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // YCoordColumn
            // 
            this.YCoordColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.YCoordColumn.HeaderText = "";
            this.YCoordColumn.Name = "YCoordColumn";
            this.YCoordColumn.ReadOnly = true;
            this.YCoordColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.YCoordColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // DeleteColumn
            // 
            this.DeleteColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.DeleteColumn.HeaderText = "";
            this.DeleteColumn.Name = "DeleteColumn";
            this.DeleteColumn.ReadOnly = true;
            this.DeleteColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.DeleteColumn.Width = 5;
            // 
            // MapPointToolButton
            // 
            this.MapPointToolButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.MapPointToolButton.AutoSize = true;
            this.MapPointToolButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.MapPointToolButton.Image = ((System.Drawing.Image)(resources.GetObject("MapPointToolButton.Image")));
            this.MapPointToolButton.Location = new System.Drawing.Point(295, 21);
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
            this.MoveToCenterButton.Location = new System.Drawing.Point(258, 21);
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
            this.SaveButton.Location = new System.Drawing.Point(258, 57);
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
            this.CopyButton.Location = new System.Drawing.Point(295, 57);
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
            this.UTMNotationLabel.Location = new System.Drawing.Point(6, 38);
            this.UTMNotationLabel.Name = "UTMNotationLabel";
            this.UTMNotationLabel.Size = new System.Drawing.Size(41, 16);
            this.UTMNotationLabel.TabIndex = 34;
            this.UTMNotationLabel.Text = "UTM:";
            // 
            // UTMNotationTextBox
            // 
            this.UTMNotationTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.UTMNotationTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.UTMNotationTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.UTMNotationTextBox.Location = new System.Drawing.Point(148, 39);
            this.UTMNotationTextBox.Name = "UTMNotationTextBox";
            this.UTMNotationTextBox.Size = new System.Drawing.Size(170, 15);
            this.UTMNotationTextBox.TabIndex = 31;
            this.UTMNotationTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.UTMNotationTextBox_KeyDown);
            this.UTMNotationTextBox.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.UTMNotationTextBox_MouseDoubleClick);
            // 
            // MgrsNotationLabel
            // 
            this.MgrsNotationLabel.AutoSize = true;
            this.MgrsNotationLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.MgrsNotationLabel.Location = new System.Drawing.Point(6, 10);
            this.MgrsNotationLabel.Name = "MgrsNotationLabel";
            this.MgrsNotationLabel.Size = new System.Drawing.Size(51, 16);
            this.MgrsNotationLabel.TabIndex = 30;
            this.MgrsNotationLabel.Text = "MGRS:";
            // 
            // UkraineCoordinatesLabel
            // 
            this.UkraineCoordinatesLabel.AutoSize = true;
            this.UkraineCoordinatesLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.UkraineCoordinatesLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.UkraineCoordinatesLabel.Location = new System.Drawing.Point(0, 0);
            this.UkraineCoordinatesLabel.Name = "UkraineCoordinatesLabel";
            this.UkraineCoordinatesLabel.Size = new System.Drawing.Size(83, 16);
            this.UkraineCoordinatesLabel.TabIndex = 26;
            this.UkraineCoordinatesLabel.Text = "Ukraine2000";
            // 
            // PulkovoCoordinatesLabel
            // 
            this.PulkovoCoordinatesLabel.AutoSize = true;
            this.PulkovoCoordinatesLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.PulkovoCoordinatesLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.PulkovoCoordinatesLabel.Location = new System.Drawing.Point(0, 0);
            this.PulkovoCoordinatesLabel.Name = "PulkovoCoordinatesLabel";
            this.PulkovoCoordinatesLabel.Size = new System.Drawing.Size(85, 16);
            this.PulkovoCoordinatesLabel.TabIndex = 24;
            this.PulkovoCoordinatesLabel.Text = "Pulkovo1942";
            // 
            // WgsCoordinatesLabel
            // 
            this.WgsCoordinatesLabel.AutoSize = true;
            this.WgsCoordinatesLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.WgsCoordinatesLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.WgsCoordinatesLabel.Location = new System.Drawing.Point(0, 0);
            this.WgsCoordinatesLabel.Name = "WgsCoordinatesLabel";
            this.WgsCoordinatesLabel.Size = new System.Drawing.Size(68, 16);
            this.WgsCoordinatesLabel.TabIndex = 22;
            this.WgsCoordinatesLabel.Text = "WGS1984";
            // 
            // CurrentMapLabel
            // 
            this.CurrentMapLabel.AutoSize = true;
            this.CurrentMapLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.CurrentMapLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.CurrentMapLabel.Location = new System.Drawing.Point(0, 0);
            this.CurrentMapLabel.Name = "CurrentMapLabel";
            this.CurrentMapLabel.Size = new System.Drawing.Size(156, 16);
            this.CurrentMapLabel.TabIndex = 20;
            this.CurrentMapLabel.Text = "Current Map Coordinates";
            // 
            // MgrsNotationTextBox
            // 
            this.MgrsNotationTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.MgrsNotationTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.MgrsNotationTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.MgrsNotationTextBox.Location = new System.Drawing.Point(148, 11);
            this.MgrsNotationTextBox.Name = "MgrsNotationTextBox";
            this.MgrsNotationTextBox.Size = new System.Drawing.Size(170, 15);
            this.MgrsNotationTextBox.TabIndex = 18;
            this.MgrsNotationTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MgrsNotationTextBox_KeyDown);
            this.MgrsNotationTextBox.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.MgrsNotationTextBox_MouseDoubleClick);
            // 
            // UkraineYCoordinateTextBox
            // 
            this.UkraineYCoordinateTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.UkraineYCoordinateTextBox.Location = new System.Drawing.Point(31, 62);
            this.UkraineYCoordinateTextBox.Name = "UkraineYCoordinateTextBox";
            this.UkraineYCoordinateTextBox.Size = new System.Drawing.Size(100, 22);
            this.UkraineYCoordinateTextBox.TabIndex = 12;
            this.UkraineYCoordinateTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.UkraineYCoordinateTextBox_KeyDown);
            // 
            // UkraineXCoordinateTextBox
            // 
            this.UkraineXCoordinateTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.UkraineXCoordinateTextBox.Location = new System.Drawing.Point(31, 34);
            this.UkraineXCoordinateTextBox.Name = "UkraineXCoordinateTextBox";
            this.UkraineXCoordinateTextBox.Size = new System.Drawing.Size(100, 22);
            this.UkraineXCoordinateTextBox.TabIndex = 11;
            this.UkraineXCoordinateTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.UkraineXCoordinateTextBox_KeyDown);
            // 
            // PulkovoXCoordinateTextBox
            // 
            this.PulkovoXCoordinateTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.PulkovoXCoordinateTextBox.Location = new System.Drawing.Point(31, 34);
            this.PulkovoXCoordinateTextBox.Name = "PulkovoXCoordinateTextBox";
            this.PulkovoXCoordinateTextBox.Size = new System.Drawing.Size(100, 22);
            this.PulkovoXCoordinateTextBox.TabIndex = 10;
            this.PulkovoXCoordinateTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.PulkovoXCoordinateTextBox_KeyDown);
            // 
            // PulkovoYCoordinateTextBox
            // 
            this.PulkovoYCoordinateTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.PulkovoYCoordinateTextBox.Location = new System.Drawing.Point(31, 62);
            this.PulkovoYCoordinateTextBox.Name = "PulkovoYCoordinateTextBox";
            this.PulkovoYCoordinateTextBox.Size = new System.Drawing.Size(100, 22);
            this.PulkovoYCoordinateTextBox.TabIndex = 8;
            this.PulkovoYCoordinateTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.PulkovoYCoordinateTextBox_KeyDown);
            // 
            // YCoordinateTextBox
            // 
            this.YCoordinateTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.YCoordinateTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.YCoordinateTextBox.Location = new System.Drawing.Point(31, 62);
            this.YCoordinateTextBox.Name = "YCoordinateTextBox";
            this.YCoordinateTextBox.Size = new System.Drawing.Size(130, 15);
            this.YCoordinateTextBox.TabIndex = 7;
            this.YCoordinateTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.YCoordinateTextBox_KeyDown);
            // 
            // XCoordinateTextBox
            // 
            this.XCoordinateTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.XCoordinateTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.XCoordinateTextBox.Location = new System.Drawing.Point(31, 34);
            this.XCoordinateTextBox.Name = "XCoordinateTextBox";
            this.XCoordinateTextBox.Size = new System.Drawing.Size(130, 15);
            this.XCoordinateTextBox.TabIndex = 5;
            this.XCoordinateTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.XCoordinateTextBox_KeyDown);
            // 
            // WgsYCoordinateTextBox
            // 
            this.WgsYCoordinateTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.WgsYCoordinateTextBox.Location = new System.Drawing.Point(31, 62);
            this.WgsYCoordinateTextBox.Name = "WgsYCoordinateTextBox";
            this.WgsYCoordinateTextBox.Size = new System.Drawing.Size(100, 22);
            this.WgsYCoordinateTextBox.TabIndex = 3;
            this.WgsYCoordinateTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.WgsYCoordinateTextBox_KeyDown);
            // 
            // WgsXCoordinateTextBox
            // 
            this.WgsXCoordinateTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.WgsXCoordinateTextBox.Location = new System.Drawing.Point(31, 34);
            this.WgsXCoordinateTextBox.Name = "WgsXCoordinateTextBox";
            this.WgsXCoordinateTextBox.Size = new System.Drawing.Size(100, 22);
            this.WgsXCoordinateTextBox.TabIndex = 2;
            this.WgsXCoordinateTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.WgsXCoordinateTextBox_KeyDown);
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.FileName = "Coordinates.xml";
            // 
            // CurrentCoordinatesPanel
            // 
            this.CurrentCoordinatesPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.CurrentCoordinatesPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.CurrentCoordinatesPanel.Controls.Add(this.CurrentMapUnitsLabel2);
            this.CurrentCoordinatesPanel.Controls.Add(this.CurrentMapUnitsLabel1);
            this.CurrentCoordinatesPanel.Controls.Add(this.YLabel);
            this.CurrentCoordinatesPanel.Controls.Add(this.XLabel);
            this.CurrentCoordinatesPanel.Controls.Add(this.CurrentMapLabel);
            this.CurrentCoordinatesPanel.Controls.Add(this.XCoordinateTextBox);
            this.CurrentCoordinatesPanel.Controls.Add(this.YCoordinateTextBox);
            this.CurrentCoordinatesPanel.Location = new System.Drawing.Point(6, 21);
            this.CurrentCoordinatesPanel.Name = "CurrentCoordinatesPanel";
            this.CurrentCoordinatesPanel.Size = new System.Drawing.Size(246, 100);
            this.CurrentCoordinatesPanel.TabIndex = 41;
            // 
            // XLabel
            // 
            this.XLabel.AutoSize = true;
            this.XLabel.Location = new System.Drawing.Point(7, 33);
            this.XLabel.Name = "XLabel";
            this.XLabel.Size = new System.Drawing.Size(19, 16);
            this.XLabel.TabIndex = 21;
            this.XLabel.Text = "X:";
            // 
            // YLabel
            // 
            this.YLabel.AutoSize = true;
            this.YLabel.Location = new System.Drawing.Point(7, 61);
            this.YLabel.Name = "YLabel";
            this.YLabel.Size = new System.Drawing.Size(20, 16);
            this.YLabel.TabIndex = 22;
            this.YLabel.Text = "Y:";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.dmsLabel4);
            this.panel1.Controls.Add(this.dmsLabel3);
            this.panel1.Controls.Add(this.wgsDMSYTextBox);
            this.panel1.Controls.Add(this.wgsDMSXTextBox);
            this.panel1.Controls.Add(this.MetersLabel4);
            this.panel1.Controls.Add(this.MetersLabel3);
            this.panel1.Controls.Add(this.WgsYLabel);
            this.panel1.Controls.Add(this.wgsXLabel);
            this.panel1.Controls.Add(this.WgsCoordinatesLabel);
            this.panel1.Controls.Add(this.WgsXCoordinateTextBox);
            this.panel1.Controls.Add(this.WgsYCoordinateTextBox);
            this.panel1.Location = new System.Drawing.Point(6, 127);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(324, 100);
            this.panel1.TabIndex = 42;
            // 
            // dmsLabel4
            // 
            this.dmsLabel4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.dmsLabel4.AutoSize = true;
            this.dmsLabel4.Location = new System.Drawing.Point(276, 65);
            this.dmsLabel4.Name = "dmsLabel4";
            this.dmsLabel4.Size = new System.Drawing.Size(32, 16);
            this.dmsLabel4.TabIndex = 28;
            this.dmsLabel4.Text = "(dd)";
            // 
            // dmsLabel3
            // 
            this.dmsLabel3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.dmsLabel3.AutoSize = true;
            this.dmsLabel3.Location = new System.Drawing.Point(276, 37);
            this.dmsLabel3.Name = "dmsLabel3";
            this.dmsLabel3.Size = new System.Drawing.Size(32, 16);
            this.dmsLabel3.TabIndex = 27;
            this.dmsLabel3.Text = "(dd)";
            // 
            // wgsDMSYTextBox
            // 
            this.wgsDMSYTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.wgsDMSYTextBox.Location = new System.Drawing.Point(170, 62);
            this.wgsDMSYTextBox.Name = "wgsDMSYTextBox";
            this.wgsDMSYTextBox.Size = new System.Drawing.Size(100, 22);
            this.wgsDMSYTextBox.TabIndex = 26;
            // 
            // wgsDMSXTextBox
            // 
            this.wgsDMSXTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.wgsDMSXTextBox.Location = new System.Drawing.Point(170, 34);
            this.wgsDMSXTextBox.Name = "wgsDMSXTextBox";
            this.wgsDMSXTextBox.Size = new System.Drawing.Size(100, 22);
            this.wgsDMSXTextBox.TabIndex = 25;
            // 
            // MetersLabel4
            // 
            this.MetersLabel4.AutoSize = true;
            this.MetersLabel4.Location = new System.Drawing.Point(137, 65);
            this.MetersLabel4.Name = "MetersLabel4";
            this.MetersLabel4.Size = new System.Drawing.Size(27, 16);
            this.MetersLabel4.TabIndex = 24;
            this.MetersLabel4.Text = "(m)";
            // 
            // MetersLabel3
            // 
            this.MetersLabel3.AutoSize = true;
            this.MetersLabel3.Location = new System.Drawing.Point(137, 37);
            this.MetersLabel3.Name = "MetersLabel3";
            this.MetersLabel3.Size = new System.Drawing.Size(27, 16);
            this.MetersLabel3.TabIndex = 23;
            this.MetersLabel3.Text = "(m)";
            // 
            // WgsYLabel
            // 
            this.WgsYLabel.AutoSize = true;
            this.WgsYLabel.Location = new System.Drawing.Point(6, 65);
            this.WgsYLabel.Name = "WgsYLabel";
            this.WgsYLabel.Size = new System.Drawing.Size(20, 16);
            this.WgsYLabel.TabIndex = 22;
            this.WgsYLabel.Text = "Y:";
            // 
            // wgsXLabel
            // 
            this.wgsXLabel.AutoSize = true;
            this.wgsXLabel.Location = new System.Drawing.Point(6, 37);
            this.wgsXLabel.Name = "wgsXLabel";
            this.wgsXLabel.Size = new System.Drawing.Size(19, 16);
            this.wgsXLabel.TabIndex = 21;
            this.wgsXLabel.Text = "X:";
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel2.Controls.Add(this.dmsLabel6);
            this.panel2.Controls.Add(this.dmsLabel5);
            this.panel2.Controls.Add(this.pulkovoDMSYTextBox);
            this.panel2.Controls.Add(this.pulkovoDMSXTextBox);
            this.panel2.Controls.Add(this.MetersLabel6);
            this.panel2.Controls.Add(this.MetersLabel5);
            this.panel2.Controls.Add(this.pulkovoYLabel);
            this.panel2.Controls.Add(this.pulkovoXLabel);
            this.panel2.Controls.Add(this.PulkovoCoordinatesLabel);
            this.panel2.Controls.Add(this.PulkovoXCoordinateTextBox);
            this.panel2.Controls.Add(this.PulkovoYCoordinateTextBox);
            this.panel2.Location = new System.Drawing.Point(6, 233);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(324, 100);
            this.panel2.TabIndex = 43;
            // 
            // dmsLabel6
            // 
            this.dmsLabel6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.dmsLabel6.AutoSize = true;
            this.dmsLabel6.Location = new System.Drawing.Point(276, 65);
            this.dmsLabel6.Name = "dmsLabel6";
            this.dmsLabel6.Size = new System.Drawing.Size(32, 16);
            this.dmsLabel6.TabIndex = 28;
            this.dmsLabel6.Text = "(dd)";
            // 
            // dmsLabel5
            // 
            this.dmsLabel5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.dmsLabel5.AutoSize = true;
            this.dmsLabel5.Location = new System.Drawing.Point(276, 37);
            this.dmsLabel5.Name = "dmsLabel5";
            this.dmsLabel5.Size = new System.Drawing.Size(32, 16);
            this.dmsLabel5.TabIndex = 27;
            this.dmsLabel5.Text = "(dd)";
            // 
            // pulkovoDMSYTextBox
            // 
            this.pulkovoDMSYTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pulkovoDMSYTextBox.Location = new System.Drawing.Point(170, 62);
            this.pulkovoDMSYTextBox.Name = "pulkovoDMSYTextBox";
            this.pulkovoDMSYTextBox.Size = new System.Drawing.Size(100, 22);
            this.pulkovoDMSYTextBox.TabIndex = 26;
            // 
            // pulkovoDMSXTextBox
            // 
            this.pulkovoDMSXTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pulkovoDMSXTextBox.Location = new System.Drawing.Point(170, 34);
            this.pulkovoDMSXTextBox.Name = "pulkovoDMSXTextBox";
            this.pulkovoDMSXTextBox.Size = new System.Drawing.Size(100, 22);
            this.pulkovoDMSXTextBox.TabIndex = 25;
            // 
            // MetersLabel6
            // 
            this.MetersLabel6.AutoSize = true;
            this.MetersLabel6.Location = new System.Drawing.Point(137, 65);
            this.MetersLabel6.Name = "MetersLabel6";
            this.MetersLabel6.Size = new System.Drawing.Size(27, 16);
            this.MetersLabel6.TabIndex = 24;
            this.MetersLabel6.Text = "(m)";
            // 
            // MetersLabel5
            // 
            this.MetersLabel5.AutoSize = true;
            this.MetersLabel5.Location = new System.Drawing.Point(137, 37);
            this.MetersLabel5.Name = "MetersLabel5";
            this.MetersLabel5.Size = new System.Drawing.Size(27, 16);
            this.MetersLabel5.TabIndex = 23;
            this.MetersLabel5.Text = "(m)";
            // 
            // pulkovoYLabel
            // 
            this.pulkovoYLabel.AutoSize = true;
            this.pulkovoYLabel.Location = new System.Drawing.Point(6, 65);
            this.pulkovoYLabel.Name = "pulkovoYLabel";
            this.pulkovoYLabel.Size = new System.Drawing.Size(20, 16);
            this.pulkovoYLabel.TabIndex = 22;
            this.pulkovoYLabel.Text = "Y:";
            // 
            // pulkovoXLabel
            // 
            this.pulkovoXLabel.AutoSize = true;
            this.pulkovoXLabel.Location = new System.Drawing.Point(6, 37);
            this.pulkovoXLabel.Name = "pulkovoXLabel";
            this.pulkovoXLabel.Size = new System.Drawing.Size(19, 16);
            this.pulkovoXLabel.TabIndex = 21;
            this.pulkovoXLabel.Text = "X:";
            // 
            // panel3
            // 
            this.panel3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel3.Controls.Add(this.dmsLabel8);
            this.panel3.Controls.Add(this.dmsLabel7);
            this.panel3.Controls.Add(this.ukraineDMSYTextBox);
            this.panel3.Controls.Add(this.ukraineDMSXTextBox);
            this.panel3.Controls.Add(this.MetersLabel8);
            this.panel3.Controls.Add(this.MetersLabel7);
            this.panel3.Controls.Add(this.ukraineYLabel);
            this.panel3.Controls.Add(this.ukraineXLabel);
            this.panel3.Controls.Add(this.UkraineCoordinatesLabel);
            this.panel3.Controls.Add(this.UkraineXCoordinateTextBox);
            this.panel3.Controls.Add(this.UkraineYCoordinateTextBox);
            this.panel3.Location = new System.Drawing.Point(6, 339);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(324, 100);
            this.panel3.TabIndex = 44;
            // 
            // dmsLabel8
            // 
            this.dmsLabel8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.dmsLabel8.AutoSize = true;
            this.dmsLabel8.Location = new System.Drawing.Point(276, 65);
            this.dmsLabel8.Name = "dmsLabel8";
            this.dmsLabel8.Size = new System.Drawing.Size(32, 16);
            this.dmsLabel8.TabIndex = 28;
            this.dmsLabel8.Text = "(dd)";
            // 
            // dmsLabel7
            // 
            this.dmsLabel7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.dmsLabel7.AutoSize = true;
            this.dmsLabel7.Location = new System.Drawing.Point(276, 37);
            this.dmsLabel7.Name = "dmsLabel7";
            this.dmsLabel7.Size = new System.Drawing.Size(32, 16);
            this.dmsLabel7.TabIndex = 27;
            this.dmsLabel7.Text = "(dd)";
            // 
            // ukraineDMSYTextBox
            // 
            this.ukraineDMSYTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ukraineDMSYTextBox.Location = new System.Drawing.Point(170, 62);
            this.ukraineDMSYTextBox.Name = "ukraineDMSYTextBox";
            this.ukraineDMSYTextBox.Size = new System.Drawing.Size(100, 22);
            this.ukraineDMSYTextBox.TabIndex = 26;
            // 
            // ukraineDMSXTextBox
            // 
            this.ukraineDMSXTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ukraineDMSXTextBox.Location = new System.Drawing.Point(170, 34);
            this.ukraineDMSXTextBox.Name = "ukraineDMSXTextBox";
            this.ukraineDMSXTextBox.Size = new System.Drawing.Size(100, 22);
            this.ukraineDMSXTextBox.TabIndex = 25;
            // 
            // MetersLabel8
            // 
            this.MetersLabel8.AutoSize = true;
            this.MetersLabel8.Location = new System.Drawing.Point(137, 65);
            this.MetersLabel8.Name = "MetersLabel8";
            this.MetersLabel8.Size = new System.Drawing.Size(27, 16);
            this.MetersLabel8.TabIndex = 24;
            this.MetersLabel8.Text = "(m)";
            // 
            // MetersLabel7
            // 
            this.MetersLabel7.AutoSize = true;
            this.MetersLabel7.Location = new System.Drawing.Point(137, 37);
            this.MetersLabel7.Name = "MetersLabel7";
            this.MetersLabel7.Size = new System.Drawing.Size(27, 16);
            this.MetersLabel7.TabIndex = 23;
            this.MetersLabel7.Text = "(m)";
            // 
            // ukraineYLabel
            // 
            this.ukraineYLabel.AutoSize = true;
            this.ukraineYLabel.Location = new System.Drawing.Point(6, 65);
            this.ukraineYLabel.Name = "ukraineYLabel";
            this.ukraineYLabel.Size = new System.Drawing.Size(20, 16);
            this.ukraineYLabel.TabIndex = 22;
            this.ukraineYLabel.Text = "Y:";
            // 
            // ukraineXLabel
            // 
            this.ukraineXLabel.AutoSize = true;
            this.ukraineXLabel.Location = new System.Drawing.Point(6, 37);
            this.ukraineXLabel.Name = "ukraineXLabel";
            this.ukraineXLabel.Size = new System.Drawing.Size(19, 16);
            this.ukraineXLabel.TabIndex = 21;
            this.ukraineXLabel.Text = "X:";
            // 
            // panel4
            // 
            this.panel4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel4.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel4.Controls.Add(this.MgrsNotationLabel);
            this.panel4.Controls.Add(this.MgrsNotationTextBox);
            this.panel4.Controls.Add(this.UTMNotationTextBox);
            this.panel4.Controls.Add(this.UTMNotationLabel);
            this.panel4.Location = new System.Drawing.Point(6, 445);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(324, 72);
            this.panel4.TabIndex = 45;
            // 
            // CurrentMapUnitsLabel1
            // 
            this.CurrentMapUnitsLabel1.AutoSize = true;
            this.CurrentMapUnitsLabel1.Location = new System.Drawing.Point(165, 34);
            this.CurrentMapUnitsLabel1.Name = "CurrentMapUnitsLabel1";
            this.CurrentMapUnitsLabel1.Size = new System.Drawing.Size(43, 16);
            this.CurrentMapUnitsLabel1.TabIndex = 23;
            this.CurrentMapUnitsLabel1.Text = "(units)";
            // 
            // CurrentMapUnitsLabel2
            // 
            this.CurrentMapUnitsLabel2.AutoSize = true;
            this.CurrentMapUnitsLabel2.Location = new System.Drawing.Point(165, 61);
            this.CurrentMapUnitsLabel2.Name = "CurrentMapUnitsLabel2";
            this.CurrentMapUnitsLabel2.Size = new System.Drawing.Size(43, 16);
            this.CurrentMapUnitsLabel2.TabIndex = 24;
            this.CurrentMapUnitsLabel2.Text = "(units)";
            // 
            // DockableWindowGeoCalculator
            // 
            this.Controls.Add(this.ProjectionsGroup);
            this.Name = "DockableWindowGeoCalculator";
            this.Size = new System.Drawing.Size(335, 615);
            this.ProjectionsGroup.ResumeLayout(false);
            this.ProjectionsGroup.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PointsGridView)).EndInit();
            this.CurrentCoordinatesPanel.ResumeLayout(false);
            this.CurrentCoordinatesPanel.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
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
        private System.Windows.Forms.DataGridViewImageColumn HighlightColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn XCoordColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn YCoordColumn;
        private System.Windows.Forms.DataGridViewImageColumn DeleteColumn;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label dmsLabel8;
        private System.Windows.Forms.Label dmsLabel7;
        private System.Windows.Forms.TextBox ukraineDMSYTextBox;
        private System.Windows.Forms.TextBox ukraineDMSXTextBox;
        private System.Windows.Forms.Label MetersLabel8;
        private System.Windows.Forms.Label MetersLabel7;
        private System.Windows.Forms.Label ukraineYLabel;
        private System.Windows.Forms.Label ukraineXLabel;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label dmsLabel6;
        private System.Windows.Forms.Label dmsLabel5;
        private System.Windows.Forms.TextBox pulkovoDMSYTextBox;
        private System.Windows.Forms.TextBox pulkovoDMSXTextBox;
        private System.Windows.Forms.Label MetersLabel6;
        private System.Windows.Forms.Label MetersLabel5;
        private System.Windows.Forms.Label pulkovoYLabel;
        private System.Windows.Forms.Label pulkovoXLabel;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label dmsLabel4;
        private System.Windows.Forms.Label dmsLabel3;
        private System.Windows.Forms.TextBox wgsDMSYTextBox;
        private System.Windows.Forms.TextBox wgsDMSXTextBox;
        private System.Windows.Forms.Label MetersLabel4;
        private System.Windows.Forms.Label MetersLabel3;
        private System.Windows.Forms.Label WgsYLabel;
        private System.Windows.Forms.Label wgsXLabel;
        private System.Windows.Forms.Panel CurrentCoordinatesPanel;
        private System.Windows.Forms.Label YLabel;
        private System.Windows.Forms.Label XLabel;
        private System.Windows.Forms.Label CurrentMapUnitsLabel2;
        private System.Windows.Forms.Label CurrentMapUnitsLabel1;
        private System.Windows.Forms.ToolTip mapCenterButtonToolTip;
        private System.Windows.Forms.ToolTip toolButtonToolTip;
        private System.Windows.Forms.ToolTip saveButtonToolTip;
        private System.Windows.Forms.ToolTip copyButtonToolTip;
    }
}
