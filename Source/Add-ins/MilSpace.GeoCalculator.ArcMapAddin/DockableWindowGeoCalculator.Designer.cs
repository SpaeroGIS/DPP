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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.mgrsToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.utmToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.TitlePanel = new System.Windows.Forms.Panel();
            this.TitleLabel = new System.Windows.Forms.Label();
            this.GeneralToolStrip = new System.Windows.Forms.ToolStrip();
            this.MapPointToolButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.MoveToCenterButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.CopyButton = new System.Windows.Forms.ToolStripButton();
            this.SaveButton = new System.Windows.Forms.ToolStripButton();
            this.CurrentCoordinatesPanel = new System.Windows.Forms.Panel();
            this.CurrentMapLabel = new System.Windows.Forms.Label();
            this.XCoordinateTextBox = new System.Windows.Forms.TextBox();
            this.YCoordinateTextBox = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.wgsDMSYTextBox = new System.Windows.Forms.TextBox();
            this.wgsDMSXTextBox = new System.Windows.Forms.TextBox();
            this.WgsGeoLabel = new System.Windows.Forms.Label();
            this.wgsProjectedLabel = new System.Windows.Forms.Label();
            this.WgsXCoordinateTextBox = new System.Windows.Forms.TextBox();
            this.WgsYCoordinateTextBox = new System.Windows.Forms.TextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.pulkovoDMSYTextBox = new System.Windows.Forms.TextBox();
            this.pulkovoDMSXTextBox = new System.Windows.Forms.TextBox();
            this.PulkovoGeoLabel = new System.Windows.Forms.Label();
            this.PulkovoProjectedLabel = new System.Windows.Forms.Label();
            this.PulkovoXCoordinateTextBox = new System.Windows.Forms.TextBox();
            this.PulkovoYCoordinateTextBox = new System.Windows.Forms.TextBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.ukraineDMSYTextBox = new System.Windows.Forms.TextBox();
            this.ukraineDMSXTextBox = new System.Windows.Forms.TextBox();
            this.UkraineGeoLabel = new System.Windows.Forms.Label();
            this.UkraineProjectedLabel = new System.Windows.Forms.Label();
            this.UkraineXCoordinateTextBox = new System.Windows.Forms.TextBox();
            this.UkraineYCoordinateTextBox = new System.Windows.Forms.TextBox();
            this.panel4 = new System.Windows.Forms.Panel();
            this.MgrsNotationLabel = new System.Windows.Forms.Label();
            this.MgrsNotationTextBox = new System.Windows.Forms.TextBox();
            this.UTMNotationTextBox = new System.Windows.Forms.TextBox();
            this.UTMNotationLabel = new System.Windows.Forms.Label();
            this.GridToolStrip = new System.Windows.Forms.ToolStrip();
            this.ClearGridButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.CopyGridPointButton = new System.Windows.Forms.ToolStripButton();
            this.SaveGridPointsButton = new System.Windows.Forms.ToolStripButton();
            this.PointsGridView = new System.Windows.Forms.DataGridView();
            this.NumberColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.XCoordColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.YCoordColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.HighlightColumn = new System.Windows.Forms.DataGridViewImageColumn();
            this.DeleteColumn = new System.Windows.Forms.DataGridViewImageColumn();
            this.TitlePanel.SuspendLayout();
            this.GeneralToolStrip.SuspendLayout();
            this.CurrentCoordinatesPanel.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            this.GridToolStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PointsGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.DefaultExt = "txt";
            this.saveFileDialog.FileName = "Coordinates";
            // 
            // TitlePanel
            // 
            this.TitlePanel.BackColor = System.Drawing.SystemColors.ControlDark;
            this.TitlePanel.Controls.Add(this.TitleLabel);
            this.TitlePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.TitlePanel.Location = new System.Drawing.Point(0, 0);
            this.TitlePanel.Name = "TitlePanel";
            this.TitlePanel.Size = new System.Drawing.Size(230, 19);
            this.TitlePanel.TabIndex = 49;
            // 
            // TitleLabel
            // 
            this.TitleLabel.AutoSize = true;
            this.TitleLabel.BackColor = System.Drawing.SystemColors.ControlDark;
            this.TitleLabel.Dock = System.Windows.Forms.DockStyle.Right;
            this.TitleLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.TitleLabel.Location = new System.Drawing.Point(119, 0);
            this.TitleLabel.Name = "TitleLabel";
            this.TitleLabel.Size = new System.Drawing.Size(111, 16);
            this.TitleLabel.TabIndex = 0;
            this.TitleLabel.Text = "Geo Calculator";
            // 
            // GeneralToolStrip
            // 
            this.GeneralToolStrip.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.GeneralToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.GeneralToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MapPointToolButton,
            this.toolStripSeparator1,
            this.MoveToCenterButton,
            this.toolStripSeparator2,
            this.CopyButton,
            this.SaveButton});
            this.GeneralToolStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.GeneralToolStrip.Location = new System.Drawing.Point(0, 19);
            this.GeneralToolStrip.Name = "GeneralToolStrip";
            this.GeneralToolStrip.Size = new System.Drawing.Size(230, 23);
            this.GeneralToolStrip.TabIndex = 50;
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
            // CurrentCoordinatesPanel
            // 
            this.CurrentCoordinatesPanel.Controls.Add(this.CurrentMapLabel);
            this.CurrentCoordinatesPanel.Controls.Add(this.XCoordinateTextBox);
            this.CurrentCoordinatesPanel.Controls.Add(this.YCoordinateTextBox);
            this.CurrentCoordinatesPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.CurrentCoordinatesPanel.Location = new System.Drawing.Point(0, 42);
            this.CurrentCoordinatesPanel.Name = "CurrentCoordinatesPanel";
            this.CurrentCoordinatesPanel.Size = new System.Drawing.Size(230, 54);
            this.CurrentCoordinatesPanel.TabIndex = 51;
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
            this.XCoordinateTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.XCoordinateTextBox.Location = new System.Drawing.Point(17, 25);
            this.XCoordinateTextBox.Name = "XCoordinateTextBox";
            this.XCoordinateTextBox.Size = new System.Drawing.Size(80, 21);
            this.XCoordinateTextBox.TabIndex = 5;
            this.XCoordinateTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.XCoordinateTextBox_KeyDown);
            // 
            // YCoordinateTextBox
            // 
            this.YCoordinateTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.YCoordinateTextBox.Location = new System.Drawing.Point(103, 25);
            this.YCoordinateTextBox.Name = "YCoordinateTextBox";
            this.YCoordinateTextBox.Size = new System.Drawing.Size(80, 21);
            this.YCoordinateTextBox.TabIndex = 7;
            this.YCoordinateTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.YCoordinateTextBox_KeyDown);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.wgsDMSYTextBox);
            this.panel1.Controls.Add(this.wgsDMSXTextBox);
            this.panel1.Controls.Add(this.WgsGeoLabel);
            this.panel1.Controls.Add(this.wgsProjectedLabel);
            this.panel1.Controls.Add(this.WgsXCoordinateTextBox);
            this.panel1.Controls.Add(this.WgsYCoordinateTextBox);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 96);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(230, 102);
            this.panel1.TabIndex = 52;
            // 
            // wgsDMSYTextBox
            // 
            this.wgsDMSYTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.wgsDMSYTextBox.Location = new System.Drawing.Point(103, 26);
            this.wgsDMSYTextBox.Name = "wgsDMSYTextBox";
            this.wgsDMSYTextBox.Size = new System.Drawing.Size(80, 21);
            this.wgsDMSYTextBox.TabIndex = 26;
            // 
            // wgsDMSXTextBox
            // 
            this.wgsDMSXTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.wgsDMSXTextBox.Location = new System.Drawing.Point(17, 26);
            this.wgsDMSXTextBox.Name = "wgsDMSXTextBox";
            this.wgsDMSXTextBox.Size = new System.Drawing.Size(80, 21);
            this.wgsDMSXTextBox.TabIndex = 25;
            // 
            // WgsGeoLabel
            // 
            this.WgsGeoLabel.AutoSize = true;
            this.WgsGeoLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.WgsGeoLabel.Location = new System.Drawing.Point(0, 3);
            this.WgsGeoLabel.Name = "WgsGeoLabel";
            this.WgsGeoLabel.Size = new System.Drawing.Size(75, 15);
            this.WgsGeoLabel.TabIndex = 22;
            this.WgsGeoLabel.Text = "WGS84 Geo";
            // 
            // wgsProjectedLabel
            // 
            this.wgsProjectedLabel.AutoSize = true;
            this.wgsProjectedLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.wgsProjectedLabel.Location = new System.Drawing.Point(0, 50);
            this.wgsProjectedLabel.Name = "wgsProjectedLabel";
            this.wgsProjectedLabel.Size = new System.Drawing.Size(104, 15);
            this.wgsProjectedLabel.TabIndex = 21;
            this.wgsProjectedLabel.Text = "WGS84 Projected";
            // 
            // WgsXCoordinateTextBox
            // 
            this.WgsXCoordinateTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.WgsXCoordinateTextBox.Location = new System.Drawing.Point(17, 72);
            this.WgsXCoordinateTextBox.Name = "WgsXCoordinateTextBox";
            this.WgsXCoordinateTextBox.Size = new System.Drawing.Size(80, 21);
            this.WgsXCoordinateTextBox.TabIndex = 2;
            this.WgsXCoordinateTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.WgsXCoordinateTextBox_KeyDown);
            // 
            // WgsYCoordinateTextBox
            // 
            this.WgsYCoordinateTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.WgsYCoordinateTextBox.Location = new System.Drawing.Point(103, 72);
            this.WgsYCoordinateTextBox.Name = "WgsYCoordinateTextBox";
            this.WgsYCoordinateTextBox.Size = new System.Drawing.Size(80, 21);
            this.WgsYCoordinateTextBox.TabIndex = 3;
            this.WgsYCoordinateTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.WgsYCoordinateTextBox_KeyDown);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.pulkovoDMSYTextBox);
            this.panel2.Controls.Add(this.pulkovoDMSXTextBox);
            this.panel2.Controls.Add(this.PulkovoGeoLabel);
            this.panel2.Controls.Add(this.PulkovoProjectedLabel);
            this.panel2.Controls.Add(this.PulkovoXCoordinateTextBox);
            this.panel2.Controls.Add(this.PulkovoYCoordinateTextBox);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 198);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(230, 102);
            this.panel2.TabIndex = 53;
            // 
            // pulkovoDMSYTextBox
            // 
            this.pulkovoDMSYTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.pulkovoDMSYTextBox.Location = new System.Drawing.Point(103, 26);
            this.pulkovoDMSYTextBox.Name = "pulkovoDMSYTextBox";
            this.pulkovoDMSYTextBox.Size = new System.Drawing.Size(80, 21);
            this.pulkovoDMSYTextBox.TabIndex = 26;
            // 
            // pulkovoDMSXTextBox
            // 
            this.pulkovoDMSXTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.pulkovoDMSXTextBox.Location = new System.Drawing.Point(17, 26);
            this.pulkovoDMSXTextBox.Name = "pulkovoDMSXTextBox";
            this.pulkovoDMSXTextBox.Size = new System.Drawing.Size(80, 21);
            this.pulkovoDMSXTextBox.TabIndex = 25;
            // 
            // PulkovoGeoLabel
            // 
            this.PulkovoGeoLabel.AutoSize = true;
            this.PulkovoGeoLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.PulkovoGeoLabel.Location = new System.Drawing.Point(0, 3);
            this.PulkovoGeoLabel.Name = "PulkovoGeoLabel";
            this.PulkovoGeoLabel.Size = new System.Drawing.Size(93, 15);
            this.PulkovoGeoLabel.TabIndex = 22;
            this.PulkovoGeoLabel.Text = "Pulkovo 42 Geo";
            // 
            // PulkovoProjectedLabel
            // 
            this.PulkovoProjectedLabel.AutoSize = true;
            this.PulkovoProjectedLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.PulkovoProjectedLabel.Location = new System.Drawing.Point(0, 50);
            this.PulkovoProjectedLabel.Name = "PulkovoProjectedLabel";
            this.PulkovoProjectedLabel.Size = new System.Drawing.Size(122, 15);
            this.PulkovoProjectedLabel.TabIndex = 21;
            this.PulkovoProjectedLabel.Text = "Pulkovo 42 Projected";
            // 
            // PulkovoXCoordinateTextBox
            // 
            this.PulkovoXCoordinateTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.PulkovoXCoordinateTextBox.Location = new System.Drawing.Point(17, 72);
            this.PulkovoXCoordinateTextBox.Name = "PulkovoXCoordinateTextBox";
            this.PulkovoXCoordinateTextBox.Size = new System.Drawing.Size(80, 21);
            this.PulkovoXCoordinateTextBox.TabIndex = 10;
            this.PulkovoXCoordinateTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.PulkovoXCoordinateTextBox_KeyDown);
            // 
            // PulkovoYCoordinateTextBox
            // 
            this.PulkovoYCoordinateTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.PulkovoYCoordinateTextBox.Location = new System.Drawing.Point(103, 72);
            this.PulkovoYCoordinateTextBox.Name = "PulkovoYCoordinateTextBox";
            this.PulkovoYCoordinateTextBox.Size = new System.Drawing.Size(80, 21);
            this.PulkovoYCoordinateTextBox.TabIndex = 8;
            this.PulkovoYCoordinateTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.PulkovoYCoordinateTextBox_KeyDown);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.ukraineDMSYTextBox);
            this.panel3.Controls.Add(this.ukraineDMSXTextBox);
            this.panel3.Controls.Add(this.UkraineGeoLabel);
            this.panel3.Controls.Add(this.UkraineProjectedLabel);
            this.panel3.Controls.Add(this.UkraineXCoordinateTextBox);
            this.panel3.Controls.Add(this.UkraineYCoordinateTextBox);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel3.Location = new System.Drawing.Point(0, 300);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(230, 102);
            this.panel3.TabIndex = 54;
            // 
            // ukraineDMSYTextBox
            // 
            this.ukraineDMSYTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ukraineDMSYTextBox.Location = new System.Drawing.Point(103, 25);
            this.ukraineDMSYTextBox.Name = "ukraineDMSYTextBox";
            this.ukraineDMSYTextBox.Size = new System.Drawing.Size(80, 21);
            this.ukraineDMSYTextBox.TabIndex = 26;
            // 
            // ukraineDMSXTextBox
            // 
            this.ukraineDMSXTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ukraineDMSXTextBox.Location = new System.Drawing.Point(17, 25);
            this.ukraineDMSXTextBox.Name = "ukraineDMSXTextBox";
            this.ukraineDMSXTextBox.Size = new System.Drawing.Size(80, 21);
            this.ukraineDMSXTextBox.TabIndex = 25;
            // 
            // UkraineGeoLabel
            // 
            this.UkraineGeoLabel.AutoSize = true;
            this.UkraineGeoLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.UkraineGeoLabel.Location = new System.Drawing.Point(0, 3);
            this.UkraineGeoLabel.Name = "UkraineGeoLabel";
            this.UkraineGeoLabel.Size = new System.Drawing.Size(107, 15);
            this.UkraineGeoLabel.TabIndex = 22;
            this.UkraineGeoLabel.Text = "Ukraine 2000 Geo";
            // 
            // UkraineProjectedLabel
            // 
            this.UkraineProjectedLabel.AutoSize = true;
            this.UkraineProjectedLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.UkraineProjectedLabel.Location = new System.Drawing.Point(0, 49);
            this.UkraineProjectedLabel.Name = "UkraineProjectedLabel";
            this.UkraineProjectedLabel.Size = new System.Drawing.Size(136, 15);
            this.UkraineProjectedLabel.TabIndex = 21;
            this.UkraineProjectedLabel.Text = "Ukraine 2000 Projected";
            // 
            // UkraineXCoordinateTextBox
            // 
            this.UkraineXCoordinateTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.UkraineXCoordinateTextBox.Location = new System.Drawing.Point(17, 71);
            this.UkraineXCoordinateTextBox.Name = "UkraineXCoordinateTextBox";
            this.UkraineXCoordinateTextBox.Size = new System.Drawing.Size(80, 21);
            this.UkraineXCoordinateTextBox.TabIndex = 11;
            this.UkraineXCoordinateTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.UkraineXCoordinateTextBox_KeyDown);
            // 
            // UkraineYCoordinateTextBox
            // 
            this.UkraineYCoordinateTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.UkraineYCoordinateTextBox.Location = new System.Drawing.Point(103, 71);
            this.UkraineYCoordinateTextBox.Name = "UkraineYCoordinateTextBox";
            this.UkraineYCoordinateTextBox.Size = new System.Drawing.Size(80, 21);
            this.UkraineYCoordinateTextBox.TabIndex = 12;
            this.UkraineYCoordinateTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.UkraineYCoordinateTextBox_KeyDown);
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.MgrsNotationLabel);
            this.panel4.Controls.Add(this.MgrsNotationTextBox);
            this.panel4.Controls.Add(this.UTMNotationTextBox);
            this.panel4.Controls.Add(this.UTMNotationLabel);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel4.Location = new System.Drawing.Point(0, 402);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(230, 72);
            this.panel4.TabIndex = 55;
            // 
            // MgrsNotationLabel
            // 
            this.MgrsNotationLabel.AutoSize = true;
            this.MgrsNotationLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.MgrsNotationLabel.Location = new System.Drawing.Point(0, 10);
            this.MgrsNotationLabel.Name = "MgrsNotationLabel";
            this.MgrsNotationLabel.Size = new System.Drawing.Size(47, 15);
            this.MgrsNotationLabel.TabIndex = 30;
            this.MgrsNotationLabel.Text = "MGRS:";
            // 
            // MgrsNotationTextBox
            // 
            this.MgrsNotationTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.MgrsNotationTextBox.Location = new System.Drawing.Point(53, 7);
            this.MgrsNotationTextBox.Name = "MgrsNotationTextBox";
            this.MgrsNotationTextBox.Size = new System.Drawing.Size(130, 21);
            this.MgrsNotationTextBox.TabIndex = 18;
            this.MgrsNotationTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MgrsNotationTextBox_KeyDown);
            this.MgrsNotationTextBox.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.MgrsNotationTextBox_MouseDoubleClick);
            // 
            // UTMNotationTextBox
            // 
            this.UTMNotationTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.UTMNotationTextBox.Location = new System.Drawing.Point(53, 35);
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
            this.UTMNotationLabel.Location = new System.Drawing.Point(0, 38);
            this.UTMNotationLabel.Name = "UTMNotationLabel";
            this.UTMNotationLabel.Size = new System.Drawing.Size(37, 15);
            this.UTMNotationLabel.TabIndex = 34;
            this.UTMNotationLabel.Text = "UTM:";
            // 
            // GridToolStrip
            // 
            this.GridToolStrip.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.GridToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.GridToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ClearGridButton,
            this.toolStripSeparator3,
            this.CopyGridPointButton,
            this.SaveGridPointsButton});
            this.GridToolStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.GridToolStrip.Location = new System.Drawing.Point(0, 474);
            this.GridToolStrip.Name = "GridToolStrip";
            this.GridToolStrip.Size = new System.Drawing.Size(230, 23);
            this.GridToolStrip.TabIndex = 56;
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
            // PointsGridView
            // 
            this.PointsGridView.AllowUserToAddRows = false;
            this.PointsGridView.AllowUserToDeleteRows = false;
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
            this.XCoordColumn,
            this.YCoordColumn,
            this.HighlightColumn,
            this.DeleteColumn});
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.PointsGridView.DefaultCellStyle = dataGridViewCellStyle4;
            this.PointsGridView.GridColor = System.Drawing.SystemColors.Window;
            this.PointsGridView.Location = new System.Drawing.Point(0, 497);
            this.PointsGridView.Name = "PointsGridView";
            this.PointsGridView.ReadOnly = true;
            this.PointsGridView.RowHeadersVisible = false;
            this.PointsGridView.Size = new System.Drawing.Size(230, 132);
            this.PointsGridView.TabIndex = 57;
            this.PointsGridView.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.PointsGridView_CellContentClick);
            // 
            // NumberColumn
            // 
            this.NumberColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.NumberColumn.DefaultCellStyle = dataGridViewCellStyle1;
            this.NumberColumn.HeaderText = "";
            this.NumberColumn.Name = "NumberColumn";
            this.NumberColumn.ReadOnly = true;
            this.NumberColumn.Width = 5;
            // 
            // XCoordColumn
            // 
            this.XCoordColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.Transparent;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.Black;
            this.XCoordColumn.DefaultCellStyle = dataGridViewCellStyle2;
            this.XCoordColumn.HeaderText = "";
            this.XCoordColumn.Name = "XCoordColumn";
            this.XCoordColumn.ReadOnly = true;
            this.XCoordColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.XCoordColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // YCoordColumn
            // 
            this.YCoordColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.Transparent;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.Black;
            this.YCoordColumn.DefaultCellStyle = dataGridViewCellStyle3;
            this.YCoordColumn.HeaderText = "";
            this.YCoordColumn.Name = "YCoordColumn";
            this.YCoordColumn.ReadOnly = true;
            this.YCoordColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.YCoordColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
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
            // DeleteColumn
            // 
            this.DeleteColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.DeleteColumn.HeaderText = "";
            this.DeleteColumn.Name = "DeleteColumn";
            this.DeleteColumn.ReadOnly = true;
            this.DeleteColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.DeleteColumn.Width = 5;
            // 
            // DockableWindowGeoCalculator
            // 
            this.Controls.Add(this.PointsGridView);
            this.Controls.Add(this.GridToolStrip);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.CurrentCoordinatesPanel);
            this.Controls.Add(this.GeneralToolStrip);
            this.Controls.Add(this.TitlePanel);
            this.Name = "DockableWindowGeoCalculator";
            this.Size = new System.Drawing.Size(230, 628);
            this.TitlePanel.ResumeLayout(false);
            this.TitlePanel.PerformLayout();
            this.GeneralToolStrip.ResumeLayout(false);
            this.GeneralToolStrip.PerformLayout();
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
            this.GridToolStrip.ResumeLayout(false);
            this.GridToolStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PointsGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.ToolTip mgrsToolTip;
        private System.Windows.Forms.ToolTip utmToolTip;
        private System.Windows.Forms.Panel TitlePanel;
        private System.Windows.Forms.Label TitleLabel;
        private System.Windows.Forms.ToolStrip GeneralToolStrip;
        private System.Windows.Forms.ToolStripButton MapPointToolButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton MoveToCenterButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton CopyButton;
        private System.Windows.Forms.ToolStripButton SaveButton;
        private System.Windows.Forms.Panel CurrentCoordinatesPanel;
        private System.Windows.Forms.Label CurrentMapLabel;
        private System.Windows.Forms.TextBox XCoordinateTextBox;
        private System.Windows.Forms.TextBox YCoordinateTextBox;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox wgsDMSYTextBox;
        private System.Windows.Forms.TextBox wgsDMSXTextBox;
        private System.Windows.Forms.Label wgsProjectedLabel;
        private System.Windows.Forms.TextBox WgsXCoordinateTextBox;
        private System.Windows.Forms.TextBox WgsYCoordinateTextBox;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TextBox pulkovoDMSYTextBox;
        private System.Windows.Forms.TextBox pulkovoDMSXTextBox;
        private System.Windows.Forms.Label PulkovoGeoLabel;
        private System.Windows.Forms.Label PulkovoProjectedLabel;
        private System.Windows.Forms.TextBox PulkovoXCoordinateTextBox;
        private System.Windows.Forms.TextBox PulkovoYCoordinateTextBox;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.TextBox ukraineDMSYTextBox;
        private System.Windows.Forms.TextBox ukraineDMSXTextBox;
        private System.Windows.Forms.Label UkraineGeoLabel;
        private System.Windows.Forms.Label UkraineProjectedLabel;
        private System.Windows.Forms.TextBox UkraineXCoordinateTextBox;
        private System.Windows.Forms.TextBox UkraineYCoordinateTextBox;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Label MgrsNotationLabel;
        private System.Windows.Forms.TextBox MgrsNotationTextBox;
        private System.Windows.Forms.TextBox UTMNotationTextBox;
        private System.Windows.Forms.Label UTMNotationLabel;
        private System.Windows.Forms.ToolStrip GridToolStrip;
        private System.Windows.Forms.ToolStripButton ClearGridButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton CopyGridPointButton;
        private System.Windows.Forms.ToolStripButton SaveGridPointsButton;
        private System.Windows.Forms.Label WgsGeoLabel;
        private System.Windows.Forms.DataGridView PointsGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn NumberColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn XCoordColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn YCoordColumn;
        private System.Windows.Forms.DataGridViewImageColumn HighlightColumn;
        private System.Windows.Forms.DataGridViewImageColumn DeleteColumn;
    }
}
