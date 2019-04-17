namespace MilSpace.Profile.SurfaceProfileChartControl
{
    partial class SurfaceProfileChart
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SurfaceProfileChart));
            this.profileChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.observerHeightLabel = new System.Windows.Forms.Label();
            this.observerHeightTextBox = new System.Windows.Forms.TextBox();
            this.profilePropertiesTable = new System.Windows.Forms.DataGridView();
            this.ProfileNumberCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ObserverPointHeightCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BearingAzimuthCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ProfileLengthCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MinHeightCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MaxHeightCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MinAngleCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MaxAngleCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.VisiblePercentCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.profileDetailsListBox = new System.Windows.Forms.ListBox();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.propertiesToolBar = new System.Windows.Forms.ToolBar();
            this.changeOnlySelectedObserverHeightToolBarBtn = new System.Windows.Forms.ToolBarButton();
            this.updateTableToolBarBtn = new System.Windows.Forms.ToolBarButton();
            this.panToProfileToolBarBtn = new System.Windows.Forms.ToolBarButton();
            this.addNewProfileToolBarBtn = new System.Windows.Forms.ToolBarButton();
            this.dataExportToolBarBtn = new System.Windows.Forms.ToolBarButton();
            this.changeProfileDisplayToolBarBtn = new System.Windows.Forms.ToolBarButton();
            ((System.ComponentModel.ISupportInitialize)(this.profileChart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.profilePropertiesTable)).BeginInit();
            this.SuspendLayout();
            // 
            // profileChart
            // 
            this.profileChart.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            chartArea1.AxisX.IntervalOffsetType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Number;
            chartArea1.AxisX.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Number;
            chartArea1.AxisX.Minimum = 0D;
            chartArea1.Name = "Default";
            this.profileChart.ChartAreas.Add(chartArea1);
            this.profileChart.Location = new System.Drawing.Point(0, 0);
            this.profileChart.Name = "profileChart";
            series1.ChartArea = "Default";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series1.Name = "Series1";
            series1.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Int64;
            series1.YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Int64;
            this.profileChart.Series.Add(series1);
            this.profileChart.Size = new System.Drawing.Size(430, 156);
            this.profileChart.TabIndex = 0;
            this.profileChart.Text = "chart1";
            this.profileChart.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Profile_MouseDown);
            // 
            // observerHeightLabel
            // 
            this.observerHeightLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.observerHeightLabel.AutoSize = true;
            this.observerHeightLabel.Location = new System.Drawing.Point(555, 139);
            this.observerHeightLabel.Name = "observerHeightLabel";
            this.observerHeightLabel.Size = new System.Drawing.Size(158, 13);
            this.observerHeightLabel.TabIndex = 38;
            this.observerHeightLabel.Text = "Высота точки наблюдения (м)";
            // 
            // observerHeightTextBox
            // 
            this.observerHeightTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.observerHeightTextBox.Location = new System.Drawing.Point(714, 136);
            this.observerHeightTextBox.Name = "observerHeightTextBox";
            this.observerHeightTextBox.Size = new System.Drawing.Size(62, 20);
            this.observerHeightTextBox.TabIndex = 39;
            this.observerHeightTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ObserverHeightTextBox_KeyDown);
            // 
            // profilePropertiesTable
            // 
            this.profilePropertiesTable.AllowUserToAddRows = false;
            this.profilePropertiesTable.AllowUserToDeleteRows = false;
            this.profilePropertiesTable.AllowUserToResizeRows = false;
            this.profilePropertiesTable.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.profilePropertiesTable.BackgroundColor = System.Drawing.SystemColors.Control;
            this.profilePropertiesTable.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.profilePropertiesTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.profilePropertiesTable.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ProfileNumberCol,
            this.ObserverPointHeightCol,
            this.BearingAzimuthCol,
            this.ProfileLengthCol,
            this.MinHeightCol,
            this.MaxHeightCol,
            this.MinAngleCol,
            this.MaxAngleCol,
            this.VisiblePercentCol});
            this.profilePropertiesTable.Location = new System.Drawing.Point(553, 3);
            this.profilePropertiesTable.MultiSelect = false;
            this.profilePropertiesTable.Name = "profilePropertiesTable";
            this.profilePropertiesTable.ReadOnly = true;
            this.profilePropertiesTable.RowHeadersVisible = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.White;
            this.profilePropertiesTable.RowsDefaultCellStyle = dataGridViewCellStyle1;
            this.profilePropertiesTable.Size = new System.Drawing.Size(368, 128);
            this.profilePropertiesTable.TabIndex = 40;
            this.profilePropertiesTable.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.ProfilePropertiesTable_CellClick);
            // 
            // ProfileNumberCol
            // 
            this.ProfileNumberCol.HeaderText = "N";
            this.ProfileNumberCol.Name = "ProfileNumberCol";
            this.ProfileNumberCol.ReadOnly = true;
            this.ProfileNumberCol.ToolTipText = "Номер профиля";
            this.ProfileNumberCol.Width = 30;
            // 
            // ObserverPointHeightCol
            // 
            this.ObserverPointHeightCol.HeaderText = "OH";
            this.ObserverPointHeightCol.Name = "ObserverPointHeightCol";
            this.ObserverPointHeightCol.ReadOnly = true;
            this.ObserverPointHeightCol.ToolTipText = "Высота пункта наблюдения (м)";
            this.ObserverPointHeightCol.Width = 40;
            // 
            // BearingAzimuthCol
            // 
            this.BearingAzimuthCol.HeaderText = "AZ";
            this.BearingAzimuthCol.Name = "BearingAzimuthCol";
            this.BearingAzimuthCol.ReadOnly = true;
            this.BearingAzimuthCol.ToolTipText = "Азимут направления от пункта наблюдения к последней точке профиля";
            this.BearingAzimuthCol.Width = 40;
            // 
            // ProfileLengthCol
            // 
            this.ProfileLengthCol.HeaderText = "L";
            this.ProfileLengthCol.Name = "ProfileLengthCol";
            this.ProfileLengthCol.ReadOnly = true;
            this.ProfileLengthCol.ToolTipText = "Длина профиля (м)";
            this.ProfileLengthCol.Width = 40;
            // 
            // MinHeightCol
            // 
            this.MinHeightCol.HeaderText = "MnH";
            this.MinHeightCol.Name = "MinHeightCol";
            this.MinHeightCol.ReadOnly = true;
            this.MinHeightCol.ToolTipText = "Минимальная высота (м)";
            this.MinHeightCol.Width = 40;
            // 
            // MaxHeightCol
            // 
            this.MaxHeightCol.HeaderText = "MxH";
            this.MaxHeightCol.Name = "MaxHeightCol";
            this.MaxHeightCol.ReadOnly = true;
            this.MaxHeightCol.ToolTipText = "Максимальная высота (м)";
            this.MaxHeightCol.Width = 40;
            // 
            // MinAngleCol
            // 
            this.MinAngleCol.HeaderText = "MnA";
            this.MinAngleCol.Name = "MinAngleCol";
            this.MinAngleCol.ReadOnly = true;
            this.MinAngleCol.ToolTipText = "Максимальный угол спуска (градусы)";
            this.MinAngleCol.Width = 40;
            // 
            // MaxAngleCol
            // 
            this.MaxAngleCol.HeaderText = "MxA";
            this.MaxAngleCol.Name = "MaxAngleCol";
            this.MaxAngleCol.ReadOnly = true;
            this.MaxAngleCol.ToolTipText = "Максимальный угол подъема (графусы)";
            this.MaxAngleCol.Width = 40;
            // 
            // VisiblePercentCol
            // 
            this.VisiblePercentCol.HeaderText = "VP";
            this.VisiblePercentCol.Name = "VisiblePercentCol";
            this.VisiblePercentCol.ReadOnly = true;
            this.VisiblePercentCol.ToolTipText = "Процент видимых участков";
            this.VisiblePercentCol.Width = 40;
            // 
            // profileDetailsListBox
            // 
            this.profileDetailsListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.profileDetailsListBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.profileDetailsListBox.ColumnWidth = 175;
            this.profileDetailsListBox.FormattingEnabled = true;
            this.profileDetailsListBox.Location = new System.Drawing.Point(553, 160);
            this.profileDetailsListBox.Name = "profileDetailsListBox";
            this.profileDetailsListBox.Size = new System.Drawing.Size(326, 106);
            this.profileDetailsListBox.TabIndex = 42;
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "0.png");
            this.imageList1.Images.SetKeyName(1, "1.png");
            this.imageList1.Images.SetKeyName(2, "2.png");
            this.imageList1.Images.SetKeyName(3, "3.png");
            this.imageList1.Images.SetKeyName(4, "4.png");
            this.imageList1.Images.SetKeyName(5, "5.png");
            this.imageList1.Images.SetKeyName(6, "6.png");
            this.imageList1.Images.SetKeyName(7, "7.png");
            this.imageList1.Images.SetKeyName(8, "8.png");
            this.imageList1.Images.SetKeyName(9, "9.png");
            this.imageList1.Images.SetKeyName(10, "Alarme.png");
            this.imageList1.Images.SetKeyName(11, "Ampersand.png");
            this.imageList1.Images.SetKeyName(12, "Application.png");
            this.imageList1.Images.SetKeyName(13, "Applications.png");
            this.imageList1.Images.SetKeyName(14, "Arrow1 Down.png");
            this.imageList1.Images.SetKeyName(15, "Arrow1 DownLeft.png");
            this.imageList1.Images.SetKeyName(16, "Arrow1 DownRight.png");
            this.imageList1.Images.SetKeyName(17, "Arrow1 Left.png");
            this.imageList1.Images.SetKeyName(18, "Arrow1 Right.png");
            this.imageList1.Images.SetKeyName(19, "Arrow1 Up.png");
            this.imageList1.Images.SetKeyName(20, "Arrow1 UpLeft.png");
            this.imageList1.Images.SetKeyName(21, "Arrow1 UpRight.png");
            this.imageList1.Images.SetKeyName(22, "Arrow2 Down.png");
            this.imageList1.Images.SetKeyName(23, "Arrow2 DownLeft.png");
            this.imageList1.Images.SetKeyName(24, "Arrow2 DownRight.png");
            this.imageList1.Images.SetKeyName(25, "Arrow2 Left.png");
            this.imageList1.Images.SetKeyName(26, "Arrow2 Right.png");
            this.imageList1.Images.SetKeyName(27, "Arrow2 Up.png");
            this.imageList1.Images.SetKeyName(28, "Arrow2 UpLeft.png");
            this.imageList1.Images.SetKeyName(29, "Arrow2 UpRight.png");
            this.imageList1.Images.SetKeyName(30, "Arrow3 Down.png");
            this.imageList1.Images.SetKeyName(31, "Arrow3 Left.png");
            this.imageList1.Images.SetKeyName(32, "Arrow3 Right.png");
            this.imageList1.Images.SetKeyName(33, "Arrow3 Up.png");
            this.imageList1.Images.SetKeyName(34, "Attach.png");
            this.imageList1.Images.SetKeyName(35, "Audio Message.png");
            this.imageList1.Images.SetKeyName(36, "Back Top.png");
            this.imageList1.Images.SetKeyName(37, "Back.png");
            this.imageList1.Images.SetKeyName(38, "Bubble 1.png");
            this.imageList1.Images.SetKeyName(39, "Bubble 3.png");
            this.imageList1.Images.SetKeyName(40, "Burn.png");
            this.imageList1.Images.SetKeyName(41, "Calc.png");
            this.imageList1.Images.SetKeyName(42, "Calendar.png");
            this.imageList1.Images.SetKeyName(43, "Cancel.png");
            this.imageList1.Images.SetKeyName(44, "Car.png");
            this.imageList1.Images.SetKeyName(45, "Card1.png");
            this.imageList1.Images.SetKeyName(46, "Card2.png");
            this.imageList1.Images.SetKeyName(47, "Card3.png");
            this.imageList1.Images.SetKeyName(48, "Card4.png");
            this.imageList1.Images.SetKeyName(49, "Cart.png");
            this.imageList1.Images.SetKeyName(50, "Cart2.png");
            this.imageList1.Images.SetKeyName(51, "Cd.png");
            this.imageList1.Images.SetKeyName(52, "Clipboard Copy.png");
            this.imageList1.Images.SetKeyName(53, "Clipboard Cut.png");
            this.imageList1.Images.SetKeyName(54, "Clipboard Paste.png");
            this.imageList1.Images.SetKeyName(55, "Clock.png");
            this.imageList1.Images.SetKeyName(56, "Computer.png");
            this.imageList1.Images.SetKeyName(57, "Contact.png");
            this.imageList1.Images.SetKeyName(58, "Copyright.png");
            this.imageList1.Images.SetKeyName(59, "Cube.png");
            this.imageList1.Images.SetKeyName(60, "Currency Dollar.png");
            this.imageList1.Images.SetKeyName(61, "Currency Euro.png");
            this.imageList1.Images.SetKeyName(62, "Currency Pound.png");
            this.imageList1.Images.SetKeyName(63, "Database.png");
            this.imageList1.Images.SetKeyName(64, "Direction Diag1.png");
            this.imageList1.Images.SetKeyName(65, "Direction Diag2.png");
            this.imageList1.Images.SetKeyName(66, "Direction Horz.png");
            this.imageList1.Images.SetKeyName(67, "Direction Vert.png");
            this.imageList1.Images.SetKeyName(68, "Directions.png");
            this.imageList1.Images.SetKeyName(69, "Discuss.png");
            this.imageList1.Images.SetKeyName(70, "Document New.png");
            this.imageList1.Images.SetKeyName(71, "Document.png");
            this.imageList1.Images.SetKeyName(72, "Document2.png");
            this.imageList1.Images.SetKeyName(73, "Dots Down.png");
            this.imageList1.Images.SetKeyName(74, "Dots Up.png");
            this.imageList1.Images.SetKeyName(75, "Dots.png");
            this.imageList1.Images.SetKeyName(76, "Download.png");
            this.imageList1.Images.SetKeyName(77, "Email.png");
            this.imageList1.Images.SetKeyName(78, "Exclamation.png");
            this.imageList1.Images.SetKeyName(79, "Fbook.png");
            this.imageList1.Images.SetKeyName(80, "Flag.png");
            this.imageList1.Images.SetKeyName(81, "Folder.png");
            this.imageList1.Images.SetKeyName(82, "Folder2.png");
            this.imageList1.Images.SetKeyName(83, "Folder3.png");
            this.imageList1.Images.SetKeyName(84, "Footprint.png");
            this.imageList1.Images.SetKeyName(85, "Forbidden.png");
            this.imageList1.Images.SetKeyName(86, "Full Screen.png");
            this.imageList1.Images.SetKeyName(87, "Full Size.png");
            this.imageList1.Images.SetKeyName(88, "Game.png");
            this.imageList1.Images.SetKeyName(89, "Gear.png");
            this.imageList1.Images.SetKeyName(90, "Globe.png");
            this.imageList1.Images.SetKeyName(91, "Go In.png");
            this.imageList1.Images.SetKeyName(92, "Go Out.png");
            this.imageList1.Images.SetKeyName(93, "Graph.png");
            this.imageList1.Images.SetKeyName(94, "Hand.png");
            this.imageList1.Images.SetKeyName(95, "Hdd Network.png");
            this.imageList1.Images.SetKeyName(96, "Hdd.png");
            this.imageList1.Images.SetKeyName(97, "Health.png");
            this.imageList1.Images.SetKeyName(98, "Heart.png");
            this.imageList1.Images.SetKeyName(99, "Home.png");
            this.imageList1.Images.SetKeyName(100, "Home2.png");
            this.imageList1.Images.SetKeyName(101, "Info.png");
            this.imageList1.Images.SetKeyName(102, "Info2.png");
            this.imageList1.Images.SetKeyName(103, "Ipod.png");
            this.imageList1.Images.SetKeyName(104, "Key.png");
            this.imageList1.Images.SetKeyName(105, "Light.png");
            this.imageList1.Images.SetKeyName(106, "Link.png");
            this.imageList1.Images.SetKeyName(107, "Lock Open.png");
            this.imageList1.Images.SetKeyName(108, "Lock.png");
            this.imageList1.Images.SetKeyName(109, "Loop.png");
            this.imageList1.Images.SetKeyName(110, "Luggage.png");
            this.imageList1.Images.SetKeyName(111, "Mail.png");
            this.imageList1.Images.SetKeyName(112, "Man.png");
            this.imageList1.Images.SetKeyName(113, "Microphone.png");
            this.imageList1.Images.SetKeyName(114, "Minus.png");
            this.imageList1.Images.SetKeyName(115, "Mobile.png");
            this.imageList1.Images.SetKeyName(116, "Mouse.png");
            this.imageList1.Images.SetKeyName(117, "Movie.png");
            this.imageList1.Images.SetKeyName(118, "Music.png");
            this.imageList1.Images.SetKeyName(119, "Music2.png");
            this.imageList1.Images.SetKeyName(120, "Nuke.png");
            this.imageList1.Images.SetKeyName(121, "Ok.png");
            this.imageList1.Images.SetKeyName(122, "Paragraph.png");
            this.imageList1.Images.SetKeyName(123, "Percent.png");
            this.imageList1.Images.SetKeyName(124, "Phone.png");
            this.imageList1.Images.SetKeyName(125, "Photo.png");
            this.imageList1.Images.SetKeyName(126, "Picture.png");
            this.imageList1.Images.SetKeyName(127, "Player Eject.png");
            this.imageList1.Images.SetKeyName(128, "Player FastFwd.png");
            this.imageList1.Images.SetKeyName(129, "Player FastRev.png");
            this.imageList1.Images.SetKeyName(130, "Player Next.png");
            this.imageList1.Images.SetKeyName(131, "Player Pause.png");
            this.imageList1.Images.SetKeyName(132, "Player Play.png");
            this.imageList1.Images.SetKeyName(133, "Player Previous.png");
            this.imageList1.Images.SetKeyName(134, "Player Record.png");
            this.imageList1.Images.SetKeyName(135, "Player Stop.png");
            this.imageList1.Images.SetKeyName(136, "Plus.png");
            this.imageList1.Images.SetKeyName(137, "Podcast.png");
            this.imageList1.Images.SetKeyName(138, "Pointer.png");
            this.imageList1.Images.SetKeyName(139, "Poll.png");
            this.imageList1.Images.SetKeyName(140, "Printer.png");
            this.imageList1.Images.SetKeyName(141, "Puzzle.png");
            this.imageList1.Images.SetKeyName(142, "Question.png");
            this.imageList1.Images.SetKeyName(143, "Reduced Size.png");
            this.imageList1.Images.SetKeyName(144, "Refresh.png");
            this.imageList1.Images.SetKeyName(145, "Rss 1.png");
            this.imageList1.Images.SetKeyName(146, "Rss 2.png");
            this.imageList1.Images.SetKeyName(147, "Save.png");
            this.imageList1.Images.SetKeyName(148, "Screen.png");
            this.imageList1.Images.SetKeyName(149, "Search.png");
            this.imageList1.Images.SetKeyName(150, "Security.png");
            this.imageList1.Images.SetKeyName(151, "Sitemap.png");
            this.imageList1.Images.SetKeyName(152, "Size Diag1.png");
            this.imageList1.Images.SetKeyName(153, "Size Diag2.png");
            this.imageList1.Images.SetKeyName(154, "Size Horz.png");
            this.imageList1.Images.SetKeyName(155, "Size Vert.png");
            this.imageList1.Images.SetKeyName(156, "Sleep.png");
            this.imageList1.Images.SetKeyName(157, "Smiley1.png");
            this.imageList1.Images.SetKeyName(158, "Smiley2.png");
            this.imageList1.Images.SetKeyName(159, "Smiley3.png");
            this.imageList1.Images.SetKeyName(160, "Sound Minus.png");
            this.imageList1.Images.SetKeyName(161, "Sound Off.png");
            this.imageList1.Images.SetKeyName(162, "Sound On.png");
            this.imageList1.Images.SetKeyName(163, "Sound Plus.png");
            this.imageList1.Images.SetKeyName(164, "Standby.png");
            this.imageList1.Images.SetKeyName(165, "Star.png");
            this.imageList1.Images.SetKeyName(166, "Start.png");
            this.imageList1.Images.SetKeyName(167, "Stats 3.png");
            this.imageList1.Images.SetKeyName(168, "Stats.png");
            this.imageList1.Images.SetKeyName(169, "Stats2.png");
            this.imageList1.Images.SetKeyName(170, "Table.png");
            this.imageList1.Images.SetKeyName(171, "Tag.png");
            this.imageList1.Images.SetKeyName(172, "Tape.png");
            this.imageList1.Images.SetKeyName(173, "Target.png");
            this.imageList1.Images.SetKeyName(174, "Text Large.png");
            this.imageList1.Images.SetKeyName(175, "Text Meduim.png");
            this.imageList1.Images.SetKeyName(176, "Text Minus.png");
            this.imageList1.Images.SetKeyName(177, "Text Plus.png");
            this.imageList1.Images.SetKeyName(178, "Text Small.png");
            this.imageList1.Images.SetKeyName(179, "Thumb Down.png");
            this.imageList1.Images.SetKeyName(180, "Thumb Up.png");
            this.imageList1.Images.SetKeyName(181, "Tool.png");
            this.imageList1.Images.SetKeyName(182, "Tool2.png");
            this.imageList1.Images.SetKeyName(183, "Trackback.png");
            this.imageList1.Images.SetKeyName(184, "Trash.png");
            this.imageList1.Images.SetKeyName(185, "Travel.png");
            this.imageList1.Images.SetKeyName(186, "Tree.png");
            this.imageList1.Images.SetKeyName(187, "Tv.png");
            this.imageList1.Images.SetKeyName(188, "User.png");
            this.imageList1.Images.SetKeyName(189, "Video.png");
            this.imageList1.Images.SetKeyName(190, "Wait.png");
            this.imageList1.Images.SetKeyName(191, "Warning.png");
            this.imageList1.Images.SetKeyName(192, "Weather Cloud.png");
            this.imageList1.Images.SetKeyName(193, "Weather Could Sun.png");
            this.imageList1.Images.SetKeyName(194, "Weather Rain.png");
            this.imageList1.Images.SetKeyName(195, "Weather Snow.png");
            this.imageList1.Images.SetKeyName(196, "Weather Sun.png");
            this.imageList1.Images.SetKeyName(197, "Wizard.png");
            this.imageList1.Images.SetKeyName(198, "Woman.png");
            this.imageList1.Images.SetKeyName(199, "Wordpress.png");
            this.imageList1.Images.SetKeyName(200, "Write.png");
            this.imageList1.Images.SetKeyName(201, "Write2.png");
            this.imageList1.Images.SetKeyName(202, "Write3.png");
            this.imageList1.Images.SetKeyName(203, "Zoom In.png");
            this.imageList1.Images.SetKeyName(204, "Zoom Out.png");
            this.imageList1.Images.SetKeyName(205, "vector-path-line.png");
            this.imageList1.Images.SetKeyName(206, "Editing-Line-icon2.png");
            this.imageList1.Images.SetKeyName(207, "Icons8-Windows-8-Editing-Rectangle-Stroked.ico");
            this.imageList1.Images.SetKeyName(208, "Editing-Line-icon3.png");
            this.imageList1.Images.SetKeyName(209, "vector-polygon.png");
            this.imageList1.Images.SetKeyName(210, "load.png");
            this.imageList1.Images.SetKeyName(211, "Download.png");
            // 
            // propertiesToolBar
            // 
            this.propertiesToolBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.propertiesToolBar.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
            this.changeOnlySelectedObserverHeightToolBarBtn,
            this.updateTableToolBarBtn,
            this.panToProfileToolBarBtn,
            this.addNewProfileToolBarBtn,
            this.dataExportToolBarBtn,
            this.changeProfileDisplayToolBarBtn});
            this.propertiesToolBar.ButtonSize = new System.Drawing.Size(18, 18);
            this.propertiesToolBar.Dock = System.Windows.Forms.DockStyle.None;
            this.propertiesToolBar.DropDownArrows = true;
            this.propertiesToolBar.ImageList = this.imageList1;
            this.propertiesToolBar.Location = new System.Drawing.Point(885, 136);
            this.propertiesToolBar.Name = "propertiesToolBar";
            this.propertiesToolBar.ShowToolTips = true;
            this.propertiesToolBar.Size = new System.Drawing.Size(23, 138);
            this.propertiesToolBar.TabIndex = 43;
            this.propertiesToolBar.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.PropertiesToolBar_ButtonClick);
            // 
            // changeOnlySelectedObserverHeightToolBarBtn
            // 
            this.changeOnlySelectedObserverHeightToolBarBtn.Enabled = false;
            this.changeOnlySelectedObserverHeightToolBarBtn.ImageKey = "Direction Vert.png";
            this.changeOnlySelectedObserverHeightToolBarBtn.Name = "changeOserverHeightToolBarButton";
            this.changeOnlySelectedObserverHeightToolBarBtn.ToolTipText = "Изменить высоту точки наблюдения";
            // 
            // updateTableToolBarBtn
            // 
            this.updateTableToolBarBtn.ImageKey = "Refresh.png";
            this.updateTableToolBarBtn.Name = "updateTableToolBarBtn";
            this.updateTableToolBarBtn.ToolTipText = "Обновить таблицу";
            // 
            // panToProfileToolBarBtn
            // 
            this.panToProfileToolBarBtn.Enabled = false;
            this.panToProfileToolBarBtn.ImageKey = "Wizard.png";
            this.panToProfileToolBarBtn.Name = "panToProfileToolBarBtn";
            this.panToProfileToolBarBtn.ToolTipText = "Позиционирование на карте на выбраный профиль";
            // 
            // addNewProfileToolBarBtn
            // 
            this.addNewProfileToolBarBtn.ImageKey = "Plus.png";
            this.addNewProfileToolBarBtn.Name = "addNewProfileToolBarBtn";
            this.addNewProfileToolBarBtn.ToolTipText = "Добавить профиль на график";
            // 
            // dataExportToolBarBtn
            // 
            this.dataExportToolBarBtn.Enabled = false;
            this.dataExportToolBarBtn.ImageKey = "Go Out.png";
            this.dataExportToolBarBtn.Name = "dataExportToolBarBtn";
            this.dataExportToolBarBtn.ToolTipText = "Экспорт данных выбранного профиля";
            // 
            // changeProfileDisplayToolBarBtn
            // 
            this.changeProfileDisplayToolBarBtn.Enabled = false;
            this.changeProfileDisplayToolBarBtn.ImageIndex = 169;
            this.changeProfileDisplayToolBarBtn.Name = "changeProfileDisplayToolBarBtn";
            this.changeProfileDisplayToolBarBtn.ToolTipText = "Изменения способа отображения текущего профиля";
            // 
            // SurfaceProfileChart
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.propertiesToolBar);
            this.Controls.Add(this.profileDetailsListBox);
            this.Controls.Add(this.profilePropertiesTable);
            this.Controls.Add(this.observerHeightTextBox);
            this.Controls.Add(this.observerHeightLabel);
            this.Controls.Add(this.profileChart);
            this.Name = "SurfaceProfileChart";
            this.Size = new System.Drawing.Size(924, 275);
            this.Load += new System.EventHandler(this.SurfaceProfileChart_Load);
            this.Resize += new System.EventHandler(this.SurfaceProfileChart_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.profileChart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.profilePropertiesTable)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart profileChart;
        private System.Windows.Forms.Label observerHeightLabel;
        private System.Windows.Forms.TextBox observerHeightTextBox;
        private System.Windows.Forms.DataGridView profilePropertiesTable;
        private System.Windows.Forms.DataGridViewTextBoxColumn ProfileNumberCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn ObserverPointHeightCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn BearingAzimuthCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn ProfileLengthCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn MinHeightCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn MaxHeightCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn MinAngleCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn MaxAngleCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn VisiblePercentCol;
        private System.Windows.Forms.ListBox profileDetailsListBox;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ToolBar propertiesToolBar;
        private System.Windows.Forms.ToolBarButton changeObserverHeightToolBarBtn;
        private System.Windows.Forms.ToolBarButton updateTableToolBarBtn;
        private System.Windows.Forms.ToolBarButton panToProfileToolBarBtn;
        private System.Windows.Forms.ToolBarButton addNewProfileToolBarBtn;
        private System.Windows.Forms.ToolBarButton dataExportToolBarBtn;
        private System.Windows.Forms.ToolBarButton changeProfileDisplayToolBarBtn;
        private System.Windows.Forms.ToolBarButton changeOnlySelectedObserverHeightToolBarBtn;
    }
}
