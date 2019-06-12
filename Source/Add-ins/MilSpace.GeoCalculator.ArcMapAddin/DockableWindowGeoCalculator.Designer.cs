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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DockableWindowGeoCalculator));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle12 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
            this.ProjectionsGroup = new System.Windows.Forms.GroupBox();
            this.GridToolStrip = new System.Windows.Forms.ToolStrip();
            this.ClearGridButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.CopyGridPointButton = new System.Windows.Forms.ToolStripButton();
            this.SaveGridPointsButton = new System.Windows.Forms.ToolStripButton();
            this.GeneralToolStrip = new System.Windows.Forms.ToolStrip();
            this.MapPointToolButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.MoveToCenterButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.CopyButton = new System.Windows.Forms.ToolStripButton();
            this.SaveButton = new System.Windows.Forms.ToolStripButton();
            this.panel4 = new System.Windows.Forms.Panel();
            this.MgrsNotationLabel = new System.Windows.Forms.Label();
            this.MgrsNotationTextBox = new System.Windows.Forms.TextBox();
            this.UTMNotationTextBox = new System.Windows.Forms.TextBox();
            this.UTMNotationLabel = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.ukraineDMSYTextBox = new System.Windows.Forms.TextBox();
            this.ukraineDMSXTextBox = new System.Windows.Forms.TextBox();
            this.UkraineGeoLabel = new System.Windows.Forms.Label();
            this.UkraineProjectedLabel = new System.Windows.Forms.Label();
            this.UkraineCoordinatesLabel = new System.Windows.Forms.Label();
            this.UkraineXCoordinateTextBox = new System.Windows.Forms.TextBox();
            this.UkraineYCoordinateTextBox = new System.Windows.Forms.TextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.pulkovoDMSYTextBox = new System.Windows.Forms.TextBox();
            this.pulkovoDMSXTextBox = new System.Windows.Forms.TextBox();
            this.PulkovoGeoLabel = new System.Windows.Forms.Label();
            this.PulkovoProjectedLabel = new System.Windows.Forms.Label();
            this.PulkovoCoordinatesLabel = new System.Windows.Forms.Label();
            this.PulkovoXCoordinateTextBox = new System.Windows.Forms.TextBox();
            this.PulkovoYCoordinateTextBox = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.wgsDMSYTextBox = new System.Windows.Forms.TextBox();
            this.wgsDMSXTextBox = new System.Windows.Forms.TextBox();
            this.WgsGeoLabel = new System.Windows.Forms.Label();
            this.wgsProjectedLabel = new System.Windows.Forms.Label();
            this.WgsCoordinatesLabel = new System.Windows.Forms.Label();
            this.WgsXCoordinateTextBox = new System.Windows.Forms.TextBox();
            this.WgsYCoordinateTextBox = new System.Windows.Forms.TextBox();
            this.CurrentCoordinatesPanel = new System.Windows.Forms.Panel();
            this.CurrentMapLabel = new System.Windows.Forms.Label();
            this.XCoordinateTextBox = new System.Windows.Forms.TextBox();
            this.YCoordinateTextBox = new System.Windows.Forms.TextBox();
            this.PointsGridView = new System.Windows.Forms.DataGridView();
            this.NumberColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.HighlightColumn = new System.Windows.Forms.DataGridViewImageColumn();
            this.XCoordColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.YCoordColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DeleteColumn = new System.Windows.Forms.DataGridViewImageColumn();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.mgrsToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.utmToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.ProjectionsGroup.SuspendLayout();
            this.GridToolStrip.SuspendLayout();
            this.GeneralToolStrip.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.CurrentCoordinatesPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PointsGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // ProjectionsGroup
            // 
            this.ProjectionsGroup.Controls.Add(this.GridToolStrip);
            this.ProjectionsGroup.Controls.Add(this.GeneralToolStrip);
            this.ProjectionsGroup.Controls.Add(this.panel4);
            this.ProjectionsGroup.Controls.Add(this.panel3);
            this.ProjectionsGroup.Controls.Add(this.panel2);
            this.ProjectionsGroup.Controls.Add(this.panel1);
            this.ProjectionsGroup.Controls.Add(this.CurrentCoordinatesPanel);
            this.ProjectionsGroup.Controls.Add(this.PointsGridView);
            this.ProjectionsGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ProjectionsGroup.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ProjectionsGroup.Location = new System.Drawing.Point(0, 0);
            this.ProjectionsGroup.Name = "ProjectionsGroup";
            this.ProjectionsGroup.Size = new System.Drawing.Size(256, 708);
            this.ProjectionsGroup.TabIndex = 0;
            this.ProjectionsGroup.TabStop = false;
            this.ProjectionsGroup.Text = "Projections";
            // 
            // GridToolStrip
            // 
            this.GridToolStrip.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.GridToolStrip.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.GridToolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.GridToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.GridToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ClearGridButton,
            this.toolStripSeparator3,
            this.CopyGridPointButton,
            this.SaveGridPointsButton});
            this.GridToolStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.GridToolStrip.Location = new System.Drawing.Point(6, 592);
            this.GridToolStrip.Name = "GridToolStrip";
            this.GridToolStrip.Size = new System.Drawing.Size(76, 23);
            this.GridToolStrip.TabIndex = 47;
            this.GridToolStrip.Text = "toolStrip2";
            // 
            // ClearGridButton
            // 
            this.ClearGridButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClearGridButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ClearGridButton.Image = ((System.Drawing.Image)(resources.GetObject("ClearGridButton.Image")));
            this.ClearGridButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ClearGridButton.Name = "ClearGridButton";
            this.ClearGridButton.Size = new System.Drawing.Size(23, 20);
            this.ClearGridButton.Click += new System.EventHandler(this.ClearGridButton_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 23);
            // 
            // CopyGridPointButton
            // 
            this.CopyGridPointButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.CopyGridPointButton.Image = ((System.Drawing.Image)(resources.GetObject("CopyGridPointButton.Image")));
            this.CopyGridPointButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.CopyGridPointButton.Name = "CopyGridPointButton";
            this.CopyGridPointButton.Size = new System.Drawing.Size(23, 20);
            this.CopyGridPointButton.Click += new System.EventHandler(this.CopyGridPointButton_Click);
            // 
            // SaveGridPointsButton
            // 
            this.SaveGridPointsButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.SaveGridPointsButton.Image = ((System.Drawing.Image)(resources.GetObject("SaveGridPointsButton.Image")));
            this.SaveGridPointsButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.SaveGridPointsButton.Name = "SaveGridPointsButton";
            this.SaveGridPointsButton.Size = new System.Drawing.Size(23, 20);
            this.SaveGridPointsButton.Click += new System.EventHandler(this.SaveGridPointsButton_Click);
            // 
            // GeneralToolStrip
            // 
            this.GeneralToolStrip.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.GeneralToolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.GeneralToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.GeneralToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MapPointToolButton,
            this.toolStripSeparator1,
            this.MoveToCenterButton,
            this.toolStripSeparator2,
            this.CopyButton,
            this.SaveButton});
            this.GeneralToolStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.GeneralToolStrip.Location = new System.Drawing.Point(6, 22);
            this.GeneralToolStrip.Name = "GeneralToolStrip";
            this.GeneralToolStrip.Size = new System.Drawing.Size(105, 23);
            this.GeneralToolStrip.TabIndex = 46;
            this.GeneralToolStrip.Text = "toolStrip1";
            // 
            // MapPointToolButton
            // 
            this.MapPointToolButton.Checked = true;
            this.MapPointToolButton.CheckOnClick = true;
            this.MapPointToolButton.CheckState = System.Windows.Forms.CheckState.Checked;
            this.MapPointToolButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.MapPointToolButton.Image = ((System.Drawing.Image)(resources.GetObject("MapPointToolButton.Image")));
            this.MapPointToolButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.MapPointToolButton.Name = "MapPointToolButton";
            this.MapPointToolButton.Size = new System.Drawing.Size(23, 20);
            this.MapPointToolButton.Text = "toolStripButton1";
            this.MapPointToolButton.Click += new System.EventHandler(this.MapPointToolButton_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 23);
            // 
            // MoveToCenterButton
            // 
            this.MoveToCenterButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.MoveToCenterButton.Image = ((System.Drawing.Image)(resources.GetObject("MoveToCenterButton.Image")));
            this.MoveToCenterButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.MoveToCenterButton.Name = "MoveToCenterButton";
            this.MoveToCenterButton.Size = new System.Drawing.Size(23, 20);
            this.MoveToCenterButton.Click += new System.EventHandler(this.MoveToCenterButton_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 23);
            // 
            // CopyButton
            // 
            this.CopyButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.CopyButton.Image = ((System.Drawing.Image)(resources.GetObject("CopyButton.Image")));
            this.CopyButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.CopyButton.Name = "CopyButton";
            this.CopyButton.Size = new System.Drawing.Size(23, 20);
            this.CopyButton.Click += new System.EventHandler(this.CopyButton_Click);
            // 
            // SaveButton
            // 
            this.SaveButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.SaveButton.Image = ((System.Drawing.Image)(resources.GetObject("SaveButton.Image")));
            this.SaveButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(23, 20);
            this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // panel4
            // 
            this.panel4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel4.Controls.Add(this.MgrsNotationLabel);
            this.panel4.Controls.Add(this.MgrsNotationTextBox);
            this.panel4.Controls.Add(this.UTMNotationTextBox);
            this.panel4.Controls.Add(this.UTMNotationLabel);
            this.panel4.Location = new System.Drawing.Point(6, 517);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(245, 72);
            this.panel4.TabIndex = 45;
            // 
            // MgrsNotationLabel
            // 
            this.MgrsNotationLabel.AutoSize = true;
            this.MgrsNotationLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.MgrsNotationLabel.Location = new System.Drawing.Point(6, 10);
            this.MgrsNotationLabel.Name = "MgrsNotationLabel";
            this.MgrsNotationLabel.Size = new System.Drawing.Size(47, 15);
            this.MgrsNotationLabel.TabIndex = 30;
            this.MgrsNotationLabel.Text = "MGRS:";
            // 
            // MgrsNotationTextBox
            // 
            this.MgrsNotationTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.MgrsNotationTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.MgrsNotationTextBox.Location = new System.Drawing.Point(112, 10);
            this.MgrsNotationTextBox.Name = "MgrsNotationTextBox";
            this.MgrsNotationTextBox.Size = new System.Drawing.Size(130, 21);
            this.MgrsNotationTextBox.TabIndex = 18;
            this.MgrsNotationTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MgrsNotationTextBox_KeyDown);
            this.MgrsNotationTextBox.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.MgrsNotationTextBox_MouseDoubleClick);
            // 
            // UTMNotationTextBox
            // 
            this.UTMNotationTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.UTMNotationTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.UTMNotationTextBox.Location = new System.Drawing.Point(112, 38);
            this.UTMNotationTextBox.Name = "UTMNotationTextBox";
            this.UTMNotationTextBox.Size = new System.Drawing.Size(130, 21);
            this.UTMNotationTextBox.TabIndex = 31;
            this.UTMNotationTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.UTMNotationTextBox_KeyDown);
            this.UTMNotationTextBox.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.UTMNotationTextBox_MouseDoubleClick);
            // 
            // UTMNotationLabel
            // 
            this.UTMNotationLabel.AutoSize = true;
            this.UTMNotationLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.UTMNotationLabel.Location = new System.Drawing.Point(6, 38);
            this.UTMNotationLabel.Name = "UTMNotationLabel";
            this.UTMNotationLabel.Size = new System.Drawing.Size(37, 15);
            this.UTMNotationLabel.TabIndex = 34;
            this.UTMNotationLabel.Text = "UTM:";
            // 
            // panel3
            // 
            this.panel3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel3.Controls.Add(this.ukraineDMSYTextBox);
            this.panel3.Controls.Add(this.ukraineDMSXTextBox);
            this.panel3.Controls.Add(this.UkraineGeoLabel);
            this.panel3.Controls.Add(this.UkraineProjectedLabel);
            this.panel3.Controls.Add(this.UkraineCoordinatesLabel);
            this.panel3.Controls.Add(this.UkraineXCoordinateTextBox);
            this.panel3.Controls.Add(this.UkraineYCoordinateTextBox);
            this.panel3.Location = new System.Drawing.Point(6, 387);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(245, 124);
            this.panel3.TabIndex = 44;
            // 
            // ukraineDMSYTextBox
            // 
            this.ukraineDMSYTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ukraineDMSYTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ukraineDMSYTextBox.Location = new System.Drawing.Point(125, 90);
            this.ukraineDMSYTextBox.Name = "ukraineDMSYTextBox";
            this.ukraineDMSYTextBox.Size = new System.Drawing.Size(110, 21);
            this.ukraineDMSYTextBox.TabIndex = 26;
            // 
            // ukraineDMSXTextBox
            // 
            this.ukraineDMSXTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ukraineDMSXTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ukraineDMSXTextBox.Location = new System.Drawing.Point(9, 90);
            this.ukraineDMSXTextBox.Name = "ukraineDMSXTextBox";
            this.ukraineDMSXTextBox.Size = new System.Drawing.Size(110, 21);
            this.ukraineDMSXTextBox.TabIndex = 25;
            // 
            // UkraineGeoLabel
            // 
            this.UkraineGeoLabel.AutoSize = true;
            this.UkraineGeoLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.UkraineGeoLabel.Location = new System.Drawing.Point(4, 68);
            this.UkraineGeoLabel.Name = "UkraineGeoLabel";
            this.UkraineGeoLabel.Size = new System.Drawing.Size(107, 15);
            this.UkraineGeoLabel.TabIndex = 22;
            this.UkraineGeoLabel.Text = "Ukraine 2000 Geo";
            // 
            // UkraineProjectedLabel
            // 
            this.UkraineProjectedLabel.AutoSize = true;
            this.UkraineProjectedLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.UkraineProjectedLabel.Location = new System.Drawing.Point(4, 21);
            this.UkraineProjectedLabel.Name = "UkraineProjectedLabel";
            this.UkraineProjectedLabel.Size = new System.Drawing.Size(136, 15);
            this.UkraineProjectedLabel.TabIndex = 21;
            this.UkraineProjectedLabel.Text = "Ukraine 2000 Projected";
            // 
            // UkraineCoordinatesLabel
            // 
            this.UkraineCoordinatesLabel.AutoSize = true;
            this.UkraineCoordinatesLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.UkraineCoordinatesLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.UkraineCoordinatesLabel.Location = new System.Drawing.Point(0, 0);
            this.UkraineCoordinatesLabel.Name = "UkraineCoordinatesLabel";
            this.UkraineCoordinatesLabel.Size = new System.Drawing.Size(78, 15);
            this.UkraineCoordinatesLabel.TabIndex = 26;
            this.UkraineCoordinatesLabel.Text = "Ukraine2000";
            // 
            // UkraineXCoordinateTextBox
            // 
            this.UkraineXCoordinateTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.UkraineXCoordinateTextBox.Location = new System.Drawing.Point(9, 43);
            this.UkraineXCoordinateTextBox.Name = "UkraineXCoordinateTextBox";
            this.UkraineXCoordinateTextBox.Size = new System.Drawing.Size(110, 21);
            this.UkraineXCoordinateTextBox.TabIndex = 11;
            this.UkraineXCoordinateTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.UkraineXCoordinateTextBox_KeyDown);
            // 
            // UkraineYCoordinateTextBox
            // 
            this.UkraineYCoordinateTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.UkraineYCoordinateTextBox.Location = new System.Drawing.Point(125, 43);
            this.UkraineYCoordinateTextBox.Name = "UkraineYCoordinateTextBox";
            this.UkraineYCoordinateTextBox.Size = new System.Drawing.Size(110, 21);
            this.UkraineYCoordinateTextBox.TabIndex = 12;
            this.UkraineYCoordinateTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.UkraineYCoordinateTextBox_KeyDown);
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.Controls.Add(this.pulkovoDMSYTextBox);
            this.panel2.Controls.Add(this.pulkovoDMSXTextBox);
            this.panel2.Controls.Add(this.PulkovoGeoLabel);
            this.panel2.Controls.Add(this.PulkovoProjectedLabel);
            this.panel2.Controls.Add(this.PulkovoCoordinatesLabel);
            this.panel2.Controls.Add(this.PulkovoXCoordinateTextBox);
            this.panel2.Controls.Add(this.PulkovoYCoordinateTextBox);
            this.panel2.Location = new System.Drawing.Point(6, 257);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(245, 124);
            this.panel2.TabIndex = 43;
            // 
            // pulkovoDMSYTextBox
            // 
            this.pulkovoDMSYTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pulkovoDMSYTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.pulkovoDMSYTextBox.Location = new System.Drawing.Point(125, 91);
            this.pulkovoDMSYTextBox.Name = "pulkovoDMSYTextBox";
            this.pulkovoDMSYTextBox.Size = new System.Drawing.Size(110, 21);
            this.pulkovoDMSYTextBox.TabIndex = 26;
            // 
            // pulkovoDMSXTextBox
            // 
            this.pulkovoDMSXTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.pulkovoDMSXTextBox.Location = new System.Drawing.Point(9, 91);
            this.pulkovoDMSXTextBox.Name = "pulkovoDMSXTextBox";
            this.pulkovoDMSXTextBox.Size = new System.Drawing.Size(110, 21);
            this.pulkovoDMSXTextBox.TabIndex = 25;
            // 
            // PulkovoGeoLabel
            // 
            this.PulkovoGeoLabel.AutoSize = true;
            this.PulkovoGeoLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.PulkovoGeoLabel.Location = new System.Drawing.Point(3, 68);
            this.PulkovoGeoLabel.Name = "PulkovoGeoLabel";
            this.PulkovoGeoLabel.Size = new System.Drawing.Size(93, 15);
            this.PulkovoGeoLabel.TabIndex = 22;
            this.PulkovoGeoLabel.Text = "Pulkovo 42 Geo";
            // 
            // PulkovoProjectedLabel
            // 
            this.PulkovoProjectedLabel.AutoSize = true;
            this.PulkovoProjectedLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.PulkovoProjectedLabel.Location = new System.Drawing.Point(4, 21);
            this.PulkovoProjectedLabel.Name = "PulkovoProjectedLabel";
            this.PulkovoProjectedLabel.Size = new System.Drawing.Size(122, 15);
            this.PulkovoProjectedLabel.TabIndex = 21;
            this.PulkovoProjectedLabel.Text = "Pulkovo 42 Projected";
            // 
            // PulkovoCoordinatesLabel
            // 
            this.PulkovoCoordinatesLabel.AutoSize = true;
            this.PulkovoCoordinatesLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.PulkovoCoordinatesLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.PulkovoCoordinatesLabel.Location = new System.Drawing.Point(0, 0);
            this.PulkovoCoordinatesLabel.Name = "PulkovoCoordinatesLabel";
            this.PulkovoCoordinatesLabel.Size = new System.Drawing.Size(78, 15);
            this.PulkovoCoordinatesLabel.TabIndex = 24;
            this.PulkovoCoordinatesLabel.Text = "Pulkovo1942";
            // 
            // PulkovoXCoordinateTextBox
            // 
            this.PulkovoXCoordinateTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.PulkovoXCoordinateTextBox.Location = new System.Drawing.Point(9, 43);
            this.PulkovoXCoordinateTextBox.Name = "PulkovoXCoordinateTextBox";
            this.PulkovoXCoordinateTextBox.Size = new System.Drawing.Size(110, 21);
            this.PulkovoXCoordinateTextBox.TabIndex = 10;
            this.PulkovoXCoordinateTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.PulkovoXCoordinateTextBox_KeyDown);
            // 
            // PulkovoYCoordinateTextBox
            // 
            this.PulkovoYCoordinateTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.PulkovoYCoordinateTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.PulkovoYCoordinateTextBox.Location = new System.Drawing.Point(125, 43);
            this.PulkovoYCoordinateTextBox.Name = "PulkovoYCoordinateTextBox";
            this.PulkovoYCoordinateTextBox.Size = new System.Drawing.Size(110, 21);
            this.PulkovoYCoordinateTextBox.TabIndex = 8;
            this.PulkovoYCoordinateTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.PulkovoYCoordinateTextBox_KeyDown);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.wgsDMSYTextBox);
            this.panel1.Controls.Add(this.wgsDMSXTextBox);
            this.panel1.Controls.Add(this.WgsGeoLabel);
            this.panel1.Controls.Add(this.wgsProjectedLabel);
            this.panel1.Controls.Add(this.WgsCoordinatesLabel);
            this.panel1.Controls.Add(this.WgsXCoordinateTextBox);
            this.panel1.Controls.Add(this.WgsYCoordinateTextBox);
            this.panel1.Location = new System.Drawing.Point(6, 127);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(245, 124);
            this.panel1.TabIndex = 42;
            // 
            // wgsDMSYTextBox
            // 
            this.wgsDMSYTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.wgsDMSYTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.wgsDMSYTextBox.Location = new System.Drawing.Point(125, 91);
            this.wgsDMSYTextBox.Name = "wgsDMSYTextBox";
            this.wgsDMSYTextBox.Size = new System.Drawing.Size(110, 21);
            this.wgsDMSYTextBox.TabIndex = 26;
            // 
            // wgsDMSXTextBox
            // 
            this.wgsDMSXTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.wgsDMSXTextBox.Location = new System.Drawing.Point(9, 91);
            this.wgsDMSXTextBox.Name = "wgsDMSXTextBox";
            this.wgsDMSXTextBox.Size = new System.Drawing.Size(110, 21);
            this.wgsDMSXTextBox.TabIndex = 25;
            // 
            // WgsGeoLabel
            // 
            this.WgsGeoLabel.AutoSize = true;
            this.WgsGeoLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.WgsGeoLabel.Location = new System.Drawing.Point(6, 68);
            this.WgsGeoLabel.Name = "WgsGeoLabel";
            this.WgsGeoLabel.Size = new System.Drawing.Size(75, 15);
            this.WgsGeoLabel.TabIndex = 22;
            this.WgsGeoLabel.Text = "WGS84 Geo";
            // 
            // wgsProjectedLabel
            // 
            this.wgsProjectedLabel.AutoSize = true;
            this.wgsProjectedLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.wgsProjectedLabel.Location = new System.Drawing.Point(6, 21);
            this.wgsProjectedLabel.Name = "wgsProjectedLabel";
            this.wgsProjectedLabel.Size = new System.Drawing.Size(104, 15);
            this.wgsProjectedLabel.TabIndex = 21;
            this.wgsProjectedLabel.Text = "WGS84 Projected";
            // 
            // WgsCoordinatesLabel
            // 
            this.WgsCoordinatesLabel.AutoSize = true;
            this.WgsCoordinatesLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.WgsCoordinatesLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.WgsCoordinatesLabel.Location = new System.Drawing.Point(0, 0);
            this.WgsCoordinatesLabel.Name = "WgsCoordinatesLabel";
            this.WgsCoordinatesLabel.Size = new System.Drawing.Size(63, 15);
            this.WgsCoordinatesLabel.TabIndex = 22;
            this.WgsCoordinatesLabel.Text = "WGS1984";
            // 
            // WgsXCoordinateTextBox
            // 
            this.WgsXCoordinateTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.WgsXCoordinateTextBox.Location = new System.Drawing.Point(9, 43);
            this.WgsXCoordinateTextBox.Name = "WgsXCoordinateTextBox";
            this.WgsXCoordinateTextBox.Size = new System.Drawing.Size(110, 21);
            this.WgsXCoordinateTextBox.TabIndex = 2;
            this.WgsXCoordinateTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.WgsXCoordinateTextBox_KeyDown);
            // 
            // WgsYCoordinateTextBox
            // 
            this.WgsYCoordinateTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.WgsYCoordinateTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.WgsYCoordinateTextBox.Location = new System.Drawing.Point(125, 43);
            this.WgsYCoordinateTextBox.Name = "WgsYCoordinateTextBox";
            this.WgsYCoordinateTextBox.Size = new System.Drawing.Size(110, 21);
            this.WgsYCoordinateTextBox.TabIndex = 3;
            this.WgsYCoordinateTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.WgsYCoordinateTextBox_KeyDown);
            // 
            // CurrentCoordinatesPanel
            // 
            this.CurrentCoordinatesPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.CurrentCoordinatesPanel.Controls.Add(this.CurrentMapLabel);
            this.CurrentCoordinatesPanel.Controls.Add(this.XCoordinateTextBox);
            this.CurrentCoordinatesPanel.Controls.Add(this.YCoordinateTextBox);
            this.CurrentCoordinatesPanel.Location = new System.Drawing.Point(6, 57);
            this.CurrentCoordinatesPanel.Name = "CurrentCoordinatesPanel";
            this.CurrentCoordinatesPanel.Size = new System.Drawing.Size(245, 64);
            this.CurrentCoordinatesPanel.TabIndex = 41;
            // 
            // CurrentMapLabel
            // 
            this.CurrentMapLabel.AutoSize = true;
            this.CurrentMapLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.CurrentMapLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.CurrentMapLabel.Location = new System.Drawing.Point(0, 0);
            this.CurrentMapLabel.Name = "CurrentMapLabel";
            this.CurrentMapLabel.Size = new System.Drawing.Size(144, 15);
            this.CurrentMapLabel.TabIndex = 20;
            this.CurrentMapLabel.Text = "Current Map Coordinates";
            // 
            // XCoordinateTextBox
            // 
            this.XCoordinateTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.XCoordinateTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.XCoordinateTextBox.Location = new System.Drawing.Point(9, 32);
            this.XCoordinateTextBox.Name = "XCoordinateTextBox";
            this.XCoordinateTextBox.Size = new System.Drawing.Size(110, 21);
            this.XCoordinateTextBox.TabIndex = 5;
            this.XCoordinateTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.XCoordinateTextBox_KeyDown);
            // 
            // YCoordinateTextBox
            // 
            this.YCoordinateTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.YCoordinateTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.YCoordinateTextBox.Location = new System.Drawing.Point(125, 32);
            this.YCoordinateTextBox.Name = "YCoordinateTextBox";
            this.YCoordinateTextBox.Size = new System.Drawing.Size(110, 21);
            this.YCoordinateTextBox.TabIndex = 7;
            this.YCoordinateTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.YCoordinateTextBox_KeyDown);
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
            dataGridViewCellStyle12.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle12.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle12.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle12.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle12.SelectionBackColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle12.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle12.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.PointsGridView.DefaultCellStyle = dataGridViewCellStyle12;
            this.PointsGridView.GridColor = System.Drawing.SystemColors.Window;
            this.PointsGridView.Location = new System.Drawing.Point(6, 620);
            this.PointsGridView.Name = "PointsGridView";
            this.PointsGridView.ReadOnly = true;
            this.PointsGridView.RowHeadersVisible = false;
            this.PointsGridView.Size = new System.Drawing.Size(245, 84);
            this.PointsGridView.TabIndex = 40;
            this.PointsGridView.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.PointsGridView_CellContentClick);
            // 
            // NumberColumn
            // 
            this.NumberColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle9.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.NumberColumn.DefaultCellStyle = dataGridViewCellStyle9;
            this.NumberColumn.HeaderText = "";
            this.NumberColumn.Name = "NumberColumn";
            this.NumberColumn.ReadOnly = true;
            this.NumberColumn.Width = 5;
            // 
            // HighlightColumn
            // 
            this.HighlightColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.HighlightColumn.HeaderText = "";
            this.HighlightColumn.Name = "HighlightColumn";
            this.HighlightColumn.ReadOnly = true;
            this.HighlightColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.HighlightColumn.Width = 23;
            // 
            // XCoordColumn
            // 
            this.XCoordColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle10.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle10.SelectionBackColor = System.Drawing.Color.Transparent;
            dataGridViewCellStyle10.SelectionForeColor = System.Drawing.Color.Black;
            this.XCoordColumn.DefaultCellStyle = dataGridViewCellStyle10;
            this.XCoordColumn.HeaderText = "";
            this.XCoordColumn.Name = "XCoordColumn";
            this.XCoordColumn.ReadOnly = true;
            this.XCoordColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.XCoordColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // YCoordColumn
            // 
            this.YCoordColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle11.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle11.SelectionBackColor = System.Drawing.Color.Transparent;
            dataGridViewCellStyle11.SelectionForeColor = System.Drawing.Color.Black;
            this.YCoordColumn.DefaultCellStyle = dataGridViewCellStyle11;
            this.YCoordColumn.HeaderText = "";
            this.YCoordColumn.Name = "YCoordColumn";
            this.YCoordColumn.ReadOnly = true;
            this.YCoordColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.YCoordColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // DeleteColumn
            // 
            this.DeleteColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.DeleteColumn.HeaderText = "";
            this.DeleteColumn.Name = "DeleteColumn";
            this.DeleteColumn.ReadOnly = true;
            this.DeleteColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.DeleteColumn.Width = 23;
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.FileName = "Coordinates.xml";
            // 
            // DockableWindowGeoCalculator
            // 
            this.Controls.Add(this.ProjectionsGroup);
            this.Name = "DockableWindowGeoCalculator";
            this.Size = new System.Drawing.Size(256, 708);
            this.ProjectionsGroup.ResumeLayout(false);
            this.ProjectionsGroup.PerformLayout();
            this.GridToolStrip.ResumeLayout(false);
            this.GridToolStrip.PerformLayout();
            this.GeneralToolStrip.ResumeLayout(false);
            this.GeneralToolStrip.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.CurrentCoordinatesPanel.ResumeLayout(false);
            this.CurrentCoordinatesPanel.PerformLayout();
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
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.DataGridView PointsGridView;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.TextBox ukraineDMSYTextBox;
        private System.Windows.Forms.TextBox ukraineDMSXTextBox;
        private System.Windows.Forms.Label UkraineGeoLabel;
        private System.Windows.Forms.Label UkraineProjectedLabel;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TextBox pulkovoDMSYTextBox;
        private System.Windows.Forms.TextBox pulkovoDMSXTextBox;
        private System.Windows.Forms.Label PulkovoGeoLabel;
        private System.Windows.Forms.Label PulkovoProjectedLabel;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox wgsDMSYTextBox;
        private System.Windows.Forms.TextBox wgsDMSXTextBox;
        private System.Windows.Forms.Label WgsGeoLabel;
        private System.Windows.Forms.Label wgsProjectedLabel;
        private System.Windows.Forms.Panel CurrentCoordinatesPanel;
        private System.Windows.Forms.ToolStrip GeneralToolStrip;
        private System.Windows.Forms.ToolStripButton MapPointToolButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton MoveToCenterButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton SaveButton;
        private System.Windows.Forms.ToolStripButton CopyButton;
        private System.Windows.Forms.ToolStrip GridToolStrip;
        private System.Windows.Forms.ToolStripButton CopyGridPointButton;
        private System.Windows.Forms.ToolStripButton SaveGridPointsButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton ClearGridButton;
        private System.Windows.Forms.DataGridViewTextBoxColumn NumberColumn;
        private System.Windows.Forms.DataGridViewImageColumn HighlightColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn XCoordColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn YCoordColumn;
        private System.Windows.Forms.DataGridViewImageColumn DeleteColumn;
        private System.Windows.Forms.ToolTip mgrsToolTip;
        private System.Windows.Forms.ToolTip utmToolTip;
    }
}
