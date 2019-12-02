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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SurfaceProfileChart));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            this.profileChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.lineColorDialog = new System.Windows.Forms.ColorDialog();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.graphToolBar = new System.Windows.Forms.ToolBar();
            this.leftGraphPaddingToolBarSpr = new System.Windows.Forms.ToolBarButton();
            this.displayProfileSignatureGraphToolBarBtn = new System.Windows.Forms.ToolBarButton();
            this.toolBarButton1 = new System.Windows.Forms.ToolBarButton();
            this.deleteSelectedProfileGraphToolBarBtn = new System.Windows.Forms.ToolBarButton();
            this.addProfileGraphToolBarButton = new System.Windows.Forms.ToolBarButton();
            this.panToSelectedProfileGraphToolBarBtn = new System.Windows.Forms.ToolBarButton();
            this.graphToolBarSpr1 = new System.Windows.Forms.ToolBarButton();
            this.panGraphToolBarBtn = new System.Windows.Forms.ToolBarButton();
            this.graphToolBarSpr2 = new System.Windows.Forms.ToolBarButton();
            this.showAllProfilesGraphToolBarBtn = new System.Windows.Forms.ToolBarButton();
            this.observerHeightIgnoreGraphToolBarBtn = new System.Windows.Forms.ToolBarButton();
            this.zoomInGraphToolBarBtn = new System.Windows.Forms.ToolBarButton();
            this.zoomOutGraphToolBarBtn = new System.Windows.Forms.ToolBarButton();
            this.graphToolBarSpr3 = new System.Windows.Forms.ToolBarButton();
            this.addPageGraphToolBarBtn = new System.Windows.Forms.ToolBarButton();
            this.deletePageGraphToolBarBtn = new System.Windows.Forms.ToolBarButton();
            this.toolBarSeparator19 = new System.Windows.Forms.ToolBarButton();
            this.saveGraphToolBarBtn = new System.Windows.Forms.ToolBarButton();
            this.toolBarButton2 = new System.Windows.Forms.ToolBarButton();
            this.updateIntersectionsLinesGraphToolBarBtn = new System.Windows.Forms.ToolBarButton();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.graphPanel = new System.Windows.Forms.Panel();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.profileDetailsListView = new System.Windows.Forms.ListView();
            this.changeAllObserversHeightsButton = new System.Windows.Forms.Button();
            this.propertiesSplitContainer = new System.Windows.Forms.SplitContainer();
            this.profilePropertiesTable = new System.Windows.Forms.DataGridView();
            this.IsVisibleCol = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.ProfileNumberCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AzimuthCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ObserverHeightCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ProfileLengthCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MinHeightCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MaxHeightCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.HeightDifferenceCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DescendingAngleCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AscendingAngleCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.VisiblePercentCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.profileNamePanel = new System.Windows.Forms.Panel();
            this.profileNameLabel = new System.Windows.Forms.Label();
            this.propertiesSettingsPanel = new System.Windows.Forms.Panel();
            this.observerHeightTextBox = new System.Windows.Forms.TextBox();
            this.observerHeightLabel = new System.Windows.Forms.Label();
            this.visibleLineColorButton = new System.Windows.Forms.Button();
            this.invisibleLineColorButton = new System.Windows.Forms.Button();
            this.invisibleLineColorLabel = new System.Windows.Forms.Label();
            this.visibleLineColorLabel = new System.Windows.Forms.Label();
            this.propertiesPanel = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.profileChart)).BeginInit();
            this.contextMenuStrip.SuspendLayout();
            this.graphPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.propertiesSplitContainer)).BeginInit();
            this.propertiesSplitContainer.Panel1.SuspendLayout();
            this.propertiesSplitContainer.Panel2.SuspendLayout();
            this.propertiesSplitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.profilePropertiesTable)).BeginInit();
            this.profileNamePanel.SuspendLayout();
            this.propertiesSettingsPanel.SuspendLayout();
            this.propertiesPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // profileChart
            // 
            chartArea2.AxisX.IntervalOffsetType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Number;
            chartArea2.AxisX.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Number;
            chartArea2.AxisX.LabelAutoFitStyle = System.Windows.Forms.DataVisualization.Charting.LabelAutoFitStyles.None;
            chartArea2.AxisX.Minimum = 0D;
            chartArea2.AxisX.TitleFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            chartArea2.AxisY.LabelAutoFitStyle = ((System.Windows.Forms.DataVisualization.Charting.LabelAutoFitStyles)((System.Windows.Forms.DataVisualization.Charting.LabelAutoFitStyles.LabelsAngleStep30 | System.Windows.Forms.DataVisualization.Charting.LabelAutoFitStyles.WordWrap)));
            chartArea2.AxisY.TitleFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            chartArea2.Name = "Default";
            this.profileChart.ChartAreas.Add(chartArea2);
            this.profileChart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.profileChart.Location = new System.Drawing.Point(0, 0);
            this.profileChart.Name = "profileChart";
            this.profileChart.Size = new System.Drawing.Size(570, 302);
            this.profileChart.TabIndex = 0;
            this.profileChart.Text = "chart1";
            this.profileChart.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.ProfileChart_MouseDoubleClick);
            this.profileChart.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Profile_MouseDown);
            // 
            // lineColorDialog
            // 
            this.lineColorDialog.ShowHelp = true;
            this.lineColorDialog.SolidColorOnly = true;
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyStripMenuItem});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(147, 26);
            // 
            // copyStripMenuItem
            // 
            this.copyStripMenuItem.Name = "copyStripMenuItem";
            this.copyStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.copyStripMenuItem.Text = "Скопировать";
            this.copyStripMenuItem.Click += new System.EventHandler(this.CopyStripMenuItem_Click);
            // 
            // graphToolBar
            // 
            this.graphToolBar.Appearance = System.Windows.Forms.ToolBarAppearance.Flat;
            this.graphToolBar.AutoSize = false;
            this.graphToolBar.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
            this.leftGraphPaddingToolBarSpr,
            this.displayProfileSignatureGraphToolBarBtn,
            this.toolBarButton1,
            this.deleteSelectedProfileGraphToolBarBtn,
            this.addProfileGraphToolBarButton,
            this.panToSelectedProfileGraphToolBarBtn,
            this.graphToolBarSpr1,
            this.panGraphToolBarBtn,
            this.graphToolBarSpr2,
            this.showAllProfilesGraphToolBarBtn,
            this.observerHeightIgnoreGraphToolBarBtn,
            this.zoomInGraphToolBarBtn,
            this.zoomOutGraphToolBarBtn,
            this.graphToolBarSpr3,
            this.addPageGraphToolBarBtn,
            this.deletePageGraphToolBarBtn,
            this.toolBarSeparator19,
            this.saveGraphToolBarBtn,
            this.toolBarButton2,
            this.updateIntersectionsLinesGraphToolBarBtn});
            this.graphToolBar.ButtonSize = new System.Drawing.Size(18, 18);
            this.graphToolBar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.graphToolBar.DropDownArrows = true;
            this.graphToolBar.ImageList = this.imageList1;
            this.graphToolBar.Location = new System.Drawing.Point(0, 302);
            this.graphToolBar.Name = "graphToolBar";
            this.graphToolBar.ShowToolTips = true;
            this.graphToolBar.Size = new System.Drawing.Size(570, 25);
            this.graphToolBar.TabIndex = 50;
            this.graphToolBar.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.GraphToolBar_ButtonClick);
            // 
            // leftGraphPaddingToolBarSpr
            // 
            this.leftGraphPaddingToolBarSpr.Name = "leftGraphPaddingToolBarSpr";
            this.leftGraphPaddingToolBarSpr.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
            // 
            // displayProfileSignatureGraphToolBarBtn
            // 
            this.displayProfileSignatureGraphToolBarBtn.ImageKey = "Bubble 3.png";
            this.displayProfileSignatureGraphToolBarBtn.Name = "displayProfileSignatureGraphToolBarBtn";
            this.displayProfileSignatureGraphToolBarBtn.ToolTipText = "Отобразить подписи профилей на графике";
            // 
            // toolBarButton1
            // 
            this.toolBarButton1.Name = "toolBarButton1";
            this.toolBarButton1.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
            // 
            // deleteSelectedProfileGraphToolBarBtn
            // 
            this.deleteSelectedProfileGraphToolBarBtn.ImageKey = "Cancel.png";
            this.deleteSelectedProfileGraphToolBarBtn.Name = "deleteSelectedProfileGraphToolBarBtn";
            this.deleteSelectedProfileGraphToolBarBtn.ToolTipText = "Удалить выбраный профиль из отображения";
            // 
            // addProfileGraphToolBarButton
            // 
            this.addProfileGraphToolBarButton.ImageKey = "Plus.png";
            this.addProfileGraphToolBarButton.Name = "addProfileGraphToolBarButton";
            this.addProfileGraphToolBarButton.ToolTipText = "Добавить профиль  на граф";
            // 
            // panToSelectedProfileGraphToolBarBtn
            // 
            this.panToSelectedProfileGraphToolBarBtn.ImageKey = "Wizard.png";
            this.panToSelectedProfileGraphToolBarBtn.Name = "panToSelectedProfileGraphToolBarBtn";
            this.panToSelectedProfileGraphToolBarBtn.ToolTipText = "Позиционирование на выбраный профиль";
            // 
            // graphToolBarSpr1
            // 
            this.graphToolBarSpr1.Name = "graphToolBarSpr1";
            this.graphToolBarSpr1.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
            // 
            // panGraphToolBarBtn
            // 
            this.panGraphToolBarBtn.ImageKey = "Search.png";
            this.panGraphToolBarBtn.Name = "panGraphToolBarBtn";
            this.panGraphToolBarBtn.ToolTipText = "Позиционирование на все профили";
            // 
            // graphToolBarSpr2
            // 
            this.graphToolBarSpr2.Name = "graphToolBarSpr2";
            this.graphToolBarSpr2.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
            // 
            // showAllProfilesGraphToolBarBtn
            // 
            this.showAllProfilesGraphToolBarBtn.ImageKey = "Full Screen.png";
            this.showAllProfilesGraphToolBarBtn.Name = "showAllProfilesGraphToolBarBtn";
            this.showAllProfilesGraphToolBarBtn.ToolTipText = "Показать все профили на графике полностью";
            // 
            // observerHeightIgnoreGraphToolBarBtn
            // 
            this.observerHeightIgnoreGraphToolBarBtn.ImageKey = "Stats2.png";
            this.observerHeightIgnoreGraphToolBarBtn.Name = "observerHeightIgnoreGraphToolBarBtn";
            this.observerHeightIgnoreGraphToolBarBtn.ToolTipText = "Масштабировать график с учетом/без учета высоты пункта наблюдения";
            // 
            // zoomInGraphToolBarBtn
            // 
            this.zoomInGraphToolBarBtn.ImageKey = "Zoom In.png";
            this.zoomInGraphToolBarBtn.Name = "zoomInGraphToolBarBtn";
            this.zoomInGraphToolBarBtn.ToolTipText = "Увеличить масштаб по расстоянию";
            // 
            // zoomOutGraphToolBarBtn
            // 
            this.zoomOutGraphToolBarBtn.ImageKey = "Zoom Out.png";
            this.zoomOutGraphToolBarBtn.Name = "zoomOutGraphToolBarBtn";
            this.zoomOutGraphToolBarBtn.ToolTipText = "Уменьшить масштаб по расстоянию";
            // 
            // graphToolBarSpr3
            // 
            this.graphToolBarSpr3.Name = "graphToolBarSpr3";
            this.graphToolBarSpr3.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
            // 
            // addPageGraphToolBarBtn
            // 
            this.addPageGraphToolBarBtn.ImageKey = "Document2.png";
            this.addPageGraphToolBarBtn.Name = "addPageGraphToolBarBtn";
            this.addPageGraphToolBarBtn.ToolTipText = "Добавить страницу графиков профилей";
            // 
            // deletePageGraphToolBarBtn
            // 
            this.deletePageGraphToolBarBtn.ImageKey = "Trash.png";
            this.deletePageGraphToolBarBtn.Name = "deletePageGraphToolBarBtn";
            this.deletePageGraphToolBarBtn.ToolTipText = "Удалить вкладку";
            // 
            // toolBarSeparator19
            // 
            this.toolBarSeparator19.Name = "toolBarSeparator19";
            this.toolBarSeparator19.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
            // 
            // saveGraphToolBarBtn
            // 
            this.saveGraphToolBarBtn.ImageKey = "Save.png";
            this.saveGraphToolBarBtn.Name = "saveGraphToolBarBtn";
            this.saveGraphToolBarBtn.ToolTipText = "Сохранить изображения графа и данные";
            // 
            // toolBarButton2
            // 
            this.toolBarButton2.Name = "toolBarButton2";
            this.toolBarButton2.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
            // 
            // updateIntersectionsLinesGraphToolBarBtn
            // 
            this.updateIntersectionsLinesGraphToolBarBtn.ImageKey = "Wizard.png";
            this.updateIntersectionsLinesGraphToolBarBtn.Name = "updateIntersectionsLinesGraphToolBarBtn";
            this.updateIntersectionsLinesGraphToolBarBtn.ToolTipText = "Пересчитать пересечения со слоями";
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
            this.imageList1.Images.SetKeyName(119, "r");
            this.imageList1.Images.SetKeyName(120, "Nuke.png");
            this.imageList1.Images.SetKeyName(121, "r");
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
            this.imageList1.Images.SetKeyName(212, "Refresh20x20.png");
            // 
            // graphPanel
            // 
            this.graphPanel.Controls.Add(this.profileChart);
            this.graphPanel.Controls.Add(this.graphToolBar);
            this.graphPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.graphPanel.Location = new System.Drawing.Point(0, 0);
            this.graphPanel.Name = "graphPanel";
            this.graphPanel.Padding = new System.Windows.Forms.Padding(0, 0, 4, 0);
            this.graphPanel.Size = new System.Drawing.Size(574, 327);
            this.graphPanel.TabIndex = 51;
            // 
            // profileDetailsListView
            // 
            this.profileDetailsListView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.profileDetailsListView.ContextMenuStrip = this.contextMenuStrip;
            this.profileDetailsListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.profileDetailsListView.FullRowSelect = true;
            this.profileDetailsListView.HideSelection = false;
            this.profileDetailsListView.Location = new System.Drawing.Point(0, 46);
            this.profileDetailsListView.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.profileDetailsListView.Name = "profileDetailsListView";
            this.profileDetailsListView.Size = new System.Drawing.Size(370, 142);
            this.profileDetailsListView.TabIndex = 50;
            this.toolTip.SetToolTip(this.profileDetailsListView, "Чтобы скопировать выделенную строку нажмите правую кнопку мыши");
            this.profileDetailsListView.UseCompatibleStateImageBehavior = false;
            // 
            // changeAllObserversHeightsButton
            // 
            this.changeAllObserversHeightsButton.ImageKey = "Direction Vert.png";
            this.changeAllObserversHeightsButton.ImageList = this.imageList1;
            this.changeAllObserversHeightsButton.Location = new System.Drawing.Point(169, 2);
            this.changeAllObserversHeightsButton.Name = "changeAllObserversHeightsButton";
            this.changeAllObserversHeightsButton.Size = new System.Drawing.Size(20, 20);
            this.changeAllObserversHeightsButton.TabIndex = 49;
            this.toolTip.SetToolTip(this.changeAllObserversHeightsButton, "Изменить высоту всех пунктов наблюдения");
            this.changeAllObserversHeightsButton.UseVisualStyleBackColor = true;
            this.changeAllObserversHeightsButton.Click += new System.EventHandler(this.ChangeAllObserversHeightsButton_Click);
            // 
            // propertiesSplitContainer
            // 
            this.propertiesSplitContainer.BackColor = System.Drawing.SystemColors.ControlDark;
            this.propertiesSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertiesSplitContainer.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.propertiesSplitContainer.Location = new System.Drawing.Point(0, 0);
            this.propertiesSplitContainer.Name = "propertiesSplitContainer";
            this.propertiesSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // propertiesSplitContainer.Panel1
            // 
            this.propertiesSplitContainer.Panel1.Controls.Add(this.profilePropertiesTable);
            this.propertiesSplitContainer.Panel1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.propertiesSplitContainer.Panel1MinSize = 80;
            // 
            // propertiesSplitContainer.Panel2
            // 
            this.propertiesSplitContainer.Panel2.Controls.Add(this.profileDetailsListView);
            this.propertiesSplitContainer.Panel2.Controls.Add(this.propertiesSettingsPanel);
            this.propertiesSplitContainer.Panel2.Controls.Add(this.profileNamePanel);
            this.propertiesSplitContainer.Panel2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.propertiesSplitContainer.Panel2MinSize = 80;
            this.propertiesSplitContainer.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.propertiesSplitContainer.Size = new System.Drawing.Size(370, 327);
            this.propertiesSplitContainer.SplitterDistance = 135;
            this.propertiesSplitContainer.TabIndex = 53;
            this.propertiesSplitContainer.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.PropertiesSplitContainer_SplitterMoved);
            // 
            // profilePropertiesTable
            // 
            this.profilePropertiesTable.AllowUserToAddRows = false;
            this.profilePropertiesTable.AllowUserToDeleteRows = false;
            this.profilePropertiesTable.AllowUserToResizeRows = false;
            this.profilePropertiesTable.BackgroundColor = System.Drawing.SystemColors.Window;
            this.profilePropertiesTable.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.profilePropertiesTable.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.RaisedHorizontal;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.profilePropertiesTable.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle6;
            this.profilePropertiesTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.profilePropertiesTable.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.IsVisibleCol,
            this.ProfileNumberCol,
            this.AzimuthCol,
            this.ObserverHeightCol,
            this.ProfileLengthCol,
            this.MinHeightCol,
            this.MaxHeightCol,
            this.HeightDifferenceCol,
            this.DescendingAngleCol,
            this.AscendingAngleCol,
            this.VisiblePercentCol});
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle8.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle8.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle8.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.profilePropertiesTable.DefaultCellStyle = dataGridViewCellStyle8;
            this.profilePropertiesTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.profilePropertiesTable.Location = new System.Drawing.Point(0, 0);
            this.profilePropertiesTable.MultiSelect = false;
            this.profilePropertiesTable.Name = "profilePropertiesTable";
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle9.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle9.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle9.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle9.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle9.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.profilePropertiesTable.RowHeadersDefaultCellStyle = dataGridViewCellStyle9;
            this.profilePropertiesTable.RowHeadersVisible = false;
            dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle10.BackColor = System.Drawing.Color.White;
            this.profilePropertiesTable.RowsDefaultCellStyle = dataGridViewCellStyle10;
            this.profilePropertiesTable.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.profilePropertiesTable.Size = new System.Drawing.Size(370, 135);
            this.profilePropertiesTable.TabIndex = 1;
            this.profilePropertiesTable.TabStop = false;
            this.profilePropertiesTable.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.ProfilePropertiesTable_CellValueChanged);
            this.profilePropertiesTable.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.ProfilePropertiesTable_ColumnHeaderMouseClick);
            this.profilePropertiesTable.CurrentCellDirtyStateChanged += new System.EventHandler(this.ProfilePropertiesTable_CurrentCellDirtyStateChanged);
            this.profilePropertiesTable.SelectionChanged += new System.EventHandler(this.ProfilePropertiesTable_SelectionChanged);
            this.profilePropertiesTable.Resize += new System.EventHandler(this.ProfilePropertiesTable_Resize);
            // 
            // IsVisibleCol
            // 
            this.IsVisibleCol.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.IsVisibleCol.Frozen = true;
            this.IsVisibleCol.HeaderText = "IV";
            this.IsVisibleCol.Name = "IsVisibleCol";
            this.IsVisibleCol.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.IsVisibleCol.ToolTipText = "Показать/скрыть профиль";
            this.IsVisibleCol.Width = 22;
            // 
            // ProfileNumberCol
            // 
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.ProfileNumberCol.DefaultCellStyle = dataGridViewCellStyle7;
            this.ProfileNumberCol.HeaderText = "N";
            this.ProfileNumberCol.Name = "ProfileNumberCol";
            this.ProfileNumberCol.ReadOnly = true;
            this.ProfileNumberCol.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ProfileNumberCol.ToolTipText = "Номер профиля";
            this.ProfileNumberCol.Width = 20;
            // 
            // AzimuthCol
            // 
            this.AzimuthCol.HeaderText = "AZ";
            this.AzimuthCol.Name = "AzimuthCol";
            this.AzimuthCol.ReadOnly = true;
            this.AzimuthCol.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.AzimuthCol.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.AzimuthCol.ToolTipText = "Азимут направления от пункта наблюдения к последней точке профиля";
            this.AzimuthCol.Width = 28;
            // 
            // ObserverHeightCol
            // 
            this.ObserverHeightCol.HeaderText = "H";
            this.ObserverHeightCol.Name = "ObserverHeightCol";
            this.ObserverHeightCol.ReadOnly = true;
            this.ObserverHeightCol.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ObserverHeightCol.ToolTipText = "Высота пункта наблюдения (м)";
            this.ObserverHeightCol.Width = 35;
            // 
            // ProfileLengthCol
            // 
            this.ProfileLengthCol.HeaderText = "L";
            this.ProfileLengthCol.Name = "ProfileLengthCol";
            this.ProfileLengthCol.ReadOnly = true;
            this.ProfileLengthCol.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ProfileLengthCol.ToolTipText = "Длина профиля (м)";
            this.ProfileLengthCol.Width = 35;
            // 
            // MinHeightCol
            // 
            this.MinHeightCol.HeaderText = "MnH";
            this.MinHeightCol.Name = "MinHeightCol";
            this.MinHeightCol.ReadOnly = true;
            this.MinHeightCol.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.MinHeightCol.ToolTipText = "Минимальная высота (м)";
            this.MinHeightCol.Width = 28;
            // 
            // MaxHeightCol
            // 
            this.MaxHeightCol.HeaderText = "MxH";
            this.MaxHeightCol.Name = "MaxHeightCol";
            this.MaxHeightCol.ReadOnly = true;
            this.MaxHeightCol.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.MaxHeightCol.ToolTipText = "Максимальная высота (м)";
            this.MaxHeightCol.Width = 28;
            // 
            // HeightDifferenceCol
            // 
            this.HeightDifferenceCol.HeaderText = "DH";
            this.HeightDifferenceCol.Name = "HeightDifferenceCol";
            this.HeightDifferenceCol.ReadOnly = true;
            this.HeightDifferenceCol.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.HeightDifferenceCol.ToolTipText = "Разница высот (м)";
            this.HeightDifferenceCol.Width = 28;
            // 
            // DescendingAngleCol
            // 
            this.DescendingAngleCol.HeaderText = "DA";
            this.DescendingAngleCol.Name = "DescendingAngleCol";
            this.DescendingAngleCol.ReadOnly = true;
            this.DescendingAngleCol.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.DescendingAngleCol.ToolTipText = "Максимальный угол спуска (градусы)";
            this.DescendingAngleCol.Width = 22;
            // 
            // AscendingAngleCol
            // 
            this.AscendingAngleCol.HeaderText = "RA";
            this.AscendingAngleCol.Name = "AscendingAngleCol";
            this.AscendingAngleCol.ReadOnly = true;
            this.AscendingAngleCol.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.AscendingAngleCol.ToolTipText = "Максимальный угол подъема (градусы)";
            this.AscendingAngleCol.Width = 22;
            // 
            // VisiblePercentCol
            // 
            this.VisiblePercentCol.HeaderText = "VP";
            this.VisiblePercentCol.Name = "VisiblePercentCol";
            this.VisiblePercentCol.ReadOnly = true;
            this.VisiblePercentCol.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.VisiblePercentCol.ToolTipText = "Процент видимых участков";
            this.VisiblePercentCol.Width = 28;
            // 
            // profileNamePanel
            // 
            this.profileNamePanel.BackColor = System.Drawing.SystemColors.Control;
            this.profileNamePanel.Controls.Add(this.profileNameLabel);
            this.profileNamePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.profileNamePanel.Location = new System.Drawing.Point(0, 0);
            this.profileNamePanel.Name = "profileNamePanel";
            this.profileNamePanel.Padding = new System.Windows.Forms.Padding(4, 1, 0, 1);
            this.profileNamePanel.Size = new System.Drawing.Size(370, 20);
            this.profileNamePanel.TabIndex = 61;
            // 
            // profileNameLabel
            // 
            this.profileNameLabel.BackColor = System.Drawing.Color.Transparent;
            this.profileNameLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.profileNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.profileNameLabel.Location = new System.Drawing.Point(4, 1);
            this.profileNameLabel.Name = "profileNameLabel";
            this.profileNameLabel.Size = new System.Drawing.Size(366, 18);
            this.profileNameLabel.TabIndex = 60;
            this.profileNameLabel.Text = "profileNameLabel";
            // 
            // propertiesSettingsPanel
            // 
            this.propertiesSettingsPanel.BackColor = System.Drawing.SystemColors.Window;
            this.propertiesSettingsPanel.Controls.Add(this.changeAllObserversHeightsButton);
            this.propertiesSettingsPanel.Controls.Add(this.observerHeightTextBox);
            this.propertiesSettingsPanel.Controls.Add(this.observerHeightLabel);
            this.propertiesSettingsPanel.Controls.Add(this.visibleLineColorButton);
            this.propertiesSettingsPanel.Controls.Add(this.invisibleLineColorButton);
            this.propertiesSettingsPanel.Controls.Add(this.invisibleLineColorLabel);
            this.propertiesSettingsPanel.Controls.Add(this.visibleLineColorLabel);
            this.propertiesSettingsPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.propertiesSettingsPanel.Location = new System.Drawing.Point(0, 20);
            this.propertiesSettingsPanel.Name = "propertiesSettingsPanel";
            this.propertiesSettingsPanel.Padding = new System.Windows.Forms.Padding(2);
            this.propertiesSettingsPanel.Size = new System.Drawing.Size(370, 26);
            this.propertiesSettingsPanel.TabIndex = 52;
            // 
            // observerHeightTextBox
            // 
            this.observerHeightTextBox.Location = new System.Drawing.Point(117, 2);
            this.observerHeightTextBox.Name = "observerHeightTextBox";
            this.observerHeightTextBox.Size = new System.Drawing.Size(50, 20);
            this.observerHeightTextBox.TabIndex = 39;
            this.observerHeightTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ObserverHeightTextBox_KeyDown);
            this.observerHeightTextBox.Leave += new System.EventHandler(this.ObserverHeightTextBox_Leave);
            // 
            // observerHeightLabel
            // 
            this.observerHeightLabel.AutoSize = true;
            this.observerHeightLabel.Location = new System.Drawing.Point(0, 5);
            this.observerHeightLabel.Name = "observerHeightLabel";
            this.observerHeightLabel.Size = new System.Drawing.Size(119, 13);
            this.observerHeightLabel.TabIndex = 38;
            this.observerHeightLabel.Text = "Пункт наблюдения (м)";
            // 
            // visibleLineColorButton
            // 
            this.visibleLineColorButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.visibleLineColorButton.BackColor = System.Drawing.SystemColors.Control;
            this.visibleLineColorButton.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control;
            this.visibleLineColorButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.visibleLineColorButton.Location = new System.Drawing.Point(216, 9);
            this.visibleLineColorButton.Name = "visibleLineColorButton";
            this.visibleLineColorButton.Size = new System.Drawing.Size(25, 8);
            this.visibleLineColorButton.TabIndex = 47;
            this.visibleLineColorButton.UseVisualStyleBackColor = false;
            this.visibleLineColorButton.Click += new System.EventHandler(this.VisibleLineColorButton_Click);
            // 
            // invisibleLineColorButton
            // 
            this.invisibleLineColorButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.invisibleLineColorButton.BackColor = System.Drawing.SystemColors.Control;
            this.invisibleLineColorButton.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control;
            this.invisibleLineColorButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.invisibleLineColorButton.Location = new System.Drawing.Point(292, 9);
            this.invisibleLineColorButton.Name = "invisibleLineColorButton";
            this.invisibleLineColorButton.Size = new System.Drawing.Size(25, 8);
            this.invisibleLineColorButton.TabIndex = 48;
            this.invisibleLineColorButton.UseVisualStyleBackColor = false;
            this.invisibleLineColorButton.Click += new System.EventHandler(this.InvisibleLineColorButton_Click);
            // 
            // invisibleLineColorLabel
            // 
            this.invisibleLineColorLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.invisibleLineColorLabel.AutoSize = true;
            this.invisibleLineColorLabel.Location = new System.Drawing.Point(318, 6);
            this.invisibleLineColorLabel.Name = "invisibleLineColorLabel";
            this.invisibleLineColorLabel.Size = new System.Drawing.Size(52, 13);
            this.invisibleLineColorLabel.TabIndex = 44;
            this.invisibleLineColorLabel.Text = "не видно";
            // 
            // visibleLineColorLabel
            // 
            this.visibleLineColorLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.visibleLineColorLabel.AutoSize = true;
            this.visibleLineColorLabel.Location = new System.Drawing.Point(242, 6);
            this.visibleLineColorLabel.Name = "visibleLineColorLabel";
            this.visibleLineColorLabel.Size = new System.Drawing.Size(37, 13);
            this.visibleLineColorLabel.TabIndex = 45;
            this.visibleLineColorLabel.Text = "видно";
            // 
            // propertiesPanel
            // 
            this.propertiesPanel.BackColor = System.Drawing.SystemColors.Control;
            this.propertiesPanel.Controls.Add(this.propertiesSplitContainer);
            this.propertiesPanel.Dock = System.Windows.Forms.DockStyle.Right;
            this.propertiesPanel.Location = new System.Drawing.Point(574, 0);
            this.propertiesPanel.Name = "propertiesPanel";
            this.propertiesPanel.Size = new System.Drawing.Size(370, 327);
            this.propertiesPanel.TabIndex = 49;
            // 
            // SurfaceProfileChart
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.graphPanel);
            this.Controls.Add(this.propertiesPanel);
            this.Name = "SurfaceProfileChart";
            this.Size = new System.Drawing.Size(944, 327);
            this.Resize += new System.EventHandler(this.SurfaceProfileChart_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.profileChart)).EndInit();
            this.contextMenuStrip.ResumeLayout(false);
            this.graphPanel.ResumeLayout(false);
            this.propertiesSplitContainer.Panel1.ResumeLayout(false);
            this.propertiesSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.propertiesSplitContainer)).EndInit();
            this.propertiesSplitContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.profilePropertiesTable)).EndInit();
            this.profileNamePanel.ResumeLayout(false);
            this.propertiesSettingsPanel.ResumeLayout(false);
            this.propertiesSettingsPanel.PerformLayout();
            this.propertiesPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart profileChart;
        private System.Windows.Forms.ToolBarButton changeObserverHeightToolBarBtn;
        private System.Windows.Forms.ToolBarButton changeOnlySelectedObserverHeightToolBarBtn;
        private System.Windows.Forms.ColorDialog lineColorDialog;
        private System.Windows.Forms.ToolBar graphToolBar;
        private System.Windows.Forms.ToolBarButton leftPaddingGraphSeparator;
        private System.Windows.Forms.ToolBarButton selectProfileToolBarBtn;
        private System.Windows.Forms.ToolBarButton displayProfilesSignaturesToolBarBtn;
        private System.Windows.Forms.ToolBarButton removeSelectedProfileToolBarBtn;
        private System.Windows.Forms.ToolBarButton graphToolBarSeparator1;
        private System.Windows.Forms.ToolBarButton synchronizationToolBarBtn;
        private System.Windows.Forms.ToolBarButton graphToolBarSeparator3;
        private System.Windows.Forms.ToolBarButton updateGraphDisplayToolBarBtn;
        private System.Windows.Forms.ToolBarButton profilesFullDisplayToolBarBtn;
        private System.Windows.Forms.ToolBarButton zoomInToolBarBtn;
        private System.Windows.Forms.ToolBarButton zoomOutToolBarBtn;
        private System.Windows.Forms.ToolBarButton graphToolBarSeparator2;
        private System.Windows.Forms.ToolBarButton addGraphPageToolBarBtn;
        private System.Windows.Forms.ToolBarButton deletePageToolBarBtn;
        private System.Windows.Forms.ToolBarButton graphToolBarSeparator4;
        private System.Windows.Forms.ToolBarButton createReportToolBarBtn;
        private System.Windows.Forms.ToolBarButton saveGraphAsImageToolBarBtn;
        private System.Windows.Forms.ToolBarButton leftGraphPaddingToolBarSpr;
        private System.Windows.Forms.ToolBarButton displayProfileSignatureGraphToolBarBtn;
        private System.Windows.Forms.ToolBarButton deleteSelectedProfileGraphToolBarBtn;
        private System.Windows.Forms.ToolBarButton graphToolBarSpr1;
        private System.Windows.Forms.ToolBarButton panGraphToolBarBtn;
        private System.Windows.Forms.ToolBarButton graphToolBarSpr2;
        private System.Windows.Forms.ToolBarButton showAllProfilesGraphToolBarBtn;
        private System.Windows.Forms.ToolBarButton zoomInGraphToolBarBtn;
        private System.Windows.Forms.ToolBarButton zoomOutGraphToolBarBtn;
        private System.Windows.Forms.ToolBarButton graphToolBarSpr3;
        private System.Windows.Forms.ToolBarButton addPageGraphToolBarBtn;
        private System.Windows.Forms.ToolBarButton deletePageGraphToolBarBtn;
        private System.Windows.Forms.ToolBarButton graphToolBarSpr4;
        private System.Windows.Forms.ToolBarButton saveGraphToolBarBtn;
        private System.Windows.Forms.ToolBarButton toolBarSeparator19;
        private System.Windows.Forms.Panel graphPanel;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.ToolBarButton graphToolBarSpr5;
        private System.Windows.Forms.ToolBarButton panToSelectedProfileGraphToolBarBtn;
        private System.Windows.Forms.ToolBarButton toolBarButton1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem copyStripMenuItem;
        private System.Windows.Forms.SplitContainer propertiesSplitContainer;
        private System.Windows.Forms.DataGridView profilePropertiesTable;
        private System.Windows.Forms.Label profileNameLabel;
        private System.Windows.Forms.Panel propertiesSettingsPanel;
        private System.Windows.Forms.Button changeAllObserversHeightsButton;
        private System.Windows.Forms.TextBox observerHeightTextBox;
        private System.Windows.Forms.Label observerHeightLabel;
        private System.Windows.Forms.Button visibleLineColorButton;
        private System.Windows.Forms.Button invisibleLineColorButton;
        private System.Windows.Forms.Label invisibleLineColorLabel;
        private System.Windows.Forms.Label visibleLineColorLabel;
        private System.Windows.Forms.ListView profileDetailsListView;
        private System.Windows.Forms.Panel propertiesPanel;
        private System.Windows.Forms.ToolBarButton graphToolBarSpr6;
        private System.Windows.Forms.ToolBarButton updateIntersectionsLinesGraphToolBarBtn;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ToolBarButton toolBarButton2;
        private System.Windows.Forms.Panel profileNamePanel;
        private System.Windows.Forms.ToolBarButton addProfileGraphToolBarButton;
        private System.Windows.Forms.ToolBarButton observerHeightIgnoreGraphToolBarBtn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn IsVisibleCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn ProfileNumberCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn AzimuthCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn ObserverHeightCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn ProfileLengthCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn MinHeightCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn MaxHeightCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn HeightDifferenceCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn DescendingAngleCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn AscendingAngleCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn VisiblePercentCol;
    }
}
