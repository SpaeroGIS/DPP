namespace MilSpace.Visualization3D
{
    partial class Visualization3DMainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Visualization3DMainForm));
            this.basePanel = new System.Windows.Forms.Panel();
            this.GenerateTab = new System.Windows.Forms.TabControl();
            this.ProfilesTabPage = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.profilePanel = new System.Windows.Forms.Panel();
            this.panel7 = new System.Windows.Forms.Panel();
            this.ProfilesListBox = new System.Windows.Forms.ListBox();
            this.panel6 = new System.Windows.Forms.Panel();
            this.profilesToolBar = new System.Windows.Forms.ToolBar();
            this.AddProfile = new System.Windows.Forms.ToolBarButton();
            this.RemoveProfile = new System.Windows.Forms.ToolBarButton();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.panel5 = new System.Windows.Forms.Panel();
            this.lblProfiles = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.panel8 = new System.Windows.Forms.Panel();
            this.SessionsListBox = new System.Windows.Forms.ListBox();
            this.panel9 = new System.Windows.Forms.Panel();
            this.surfaceToolBar = new System.Windows.Forms.ToolBar();
            this.AddSurface = new System.Windows.Forms.ToolBarButton();
            this.RemoveSurface = new System.Windows.Forms.ToolBarButton();
            this.panel10 = new System.Windows.Forms.Panel();
            this.surfaceLabels = new System.Windows.Forms.Label();
            this.panel12 = new System.Windows.Forms.Panel();
            this.GenerateButton = new System.Windows.Forms.Button();
            this.GenerateImageTab = new System.Windows.Forms.TabPage();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.tbZFactor = new System.Windows.Forms.TextBox();
            this.lblZFactor = new System.Windows.Forms.Label();
            this.panel16 = new System.Windows.Forms.Panel();
            this.HydroHightTextBox = new System.Windows.Forms.TextBox();
            this.HydroHightLabel = new System.Windows.Forms.Label();
            this.HydroLayerComboBox = new System.Windows.Forms.ComboBox();
            this.HydroLayerLabel = new System.Windows.Forms.Label();
            this.panel15 = new System.Windows.Forms.Panel();
            this.TransportHightTextBox = new System.Windows.Forms.TextBox();
            this.TransportHightLabel = new System.Windows.Forms.Label();
            this.TransportLayerComboBox = new System.Windows.Forms.ComboBox();
            this.TransportLayerLabel = new System.Windows.Forms.Label();
            this.panel14 = new System.Windows.Forms.Panel();
            this.BuildingsHightComboBox = new System.Windows.Forms.ComboBox();
            this.BuildingsHight = new System.Windows.Forms.Label();
            this.BuildingsLayerComboBox = new System.Windows.Forms.ComboBox();
            this.BuildingsLayerLabel = new System.Windows.Forms.Label();
            this.panel13 = new System.Windows.Forms.Panel();
            this.PlantsHightComboBox = new System.Windows.Forms.ComboBox();
            this.PlantsHightLablel = new System.Windows.Forms.Label();
            this.PlantsLayerComboBox = new System.Windows.Forms.ComboBox();
            this.PlantsLayerLabel = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.SurfaceComboBox = new System.Windows.Forms.ComboBox();
            this.SurfaceLabel = new System.Windows.Forms.Label();
            this.panel11 = new System.Windows.Forms.Panel();
            this.btnRefreshLayers = new System.Windows.Forms.Button();
            this.lbl3DProfiles = new System.Windows.Forms.Label();
            this.basePanel.SuspendLayout();
            this.GenerateTab.SuspendLayout();
            this.ProfilesTabPage.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.profilePanel.SuspendLayout();
            this.panel7.SuspendLayout();
            this.panel6.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel8.SuspendLayout();
            this.panel9.SuspendLayout();
            this.panel10.SuspendLayout();
            this.panel12.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel16.SuspendLayout();
            this.panel15.SuspendLayout();
            this.panel14.SuspendLayout();
            this.panel13.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel11.SuspendLayout();
            this.SuspendLayout();
            // 
            // basePanel
            // 
            this.basePanel.BackColor = System.Drawing.SystemColors.Control;
            this.basePanel.Controls.Add(this.GenerateTab);
            this.basePanel.Controls.Add(this.panel3);
            this.basePanel.Controls.Add(this.panel2);
            this.basePanel.Controls.Add(this.panel16);
            this.basePanel.Controls.Add(this.panel15);
            this.basePanel.Controls.Add(this.panel14);
            this.basePanel.Controls.Add(this.panel13);
            this.basePanel.Controls.Add(this.panel1);
            this.basePanel.Controls.Add(this.panel11);
            this.basePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.basePanel.Location = new System.Drawing.Point(0, 0);
            this.basePanel.Margin = new System.Windows.Forms.Padding(0);
            this.basePanel.Name = "basePanel";
            this.basePanel.Size = new System.Drawing.Size(322, 600);
            this.basePanel.TabIndex = 0;
            // 
            // GenerateTab
            // 
            this.GenerateTab.Controls.Add(this.ProfilesTabPage);
            this.GenerateTab.Controls.Add(this.GenerateImageTab);
            this.GenerateTab.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GenerateTab.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.GenerateTab.Location = new System.Drawing.Point(0, 206);
            this.GenerateTab.Name = "GenerateTab";
            this.GenerateTab.SelectedIndex = 0;
            this.GenerateTab.Size = new System.Drawing.Size(322, 394);
            this.GenerateTab.TabIndex = 1001;
            // 
            // ProfilesTabPage
            // 
            this.ProfilesTabPage.BackColor = System.Drawing.SystemColors.Control;
            this.ProfilesTabPage.Controls.Add(this.tableLayoutPanel2);
            this.ProfilesTabPage.Controls.Add(this.panel12);
            this.ProfilesTabPage.Location = new System.Drawing.Point(4, 24);
            this.ProfilesTabPage.Name = "ProfilesTabPage";
            this.ProfilesTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.ProfilesTabPage.Size = new System.Drawing.Size(314, 366);
            this.ProfilesTabPage.TabIndex = 0;
            this.ProfilesTabPage.Text = "Profiles";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.profilePanel, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.panel4, 0, 1);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(308, 330);
            this.tableLayoutPanel2.TabIndex = 62;
            // 
            // profilePanel
            // 
            this.profilePanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.profilePanel.Controls.Add(this.panel7);
            this.profilePanel.Controls.Add(this.panel6);
            this.profilePanel.Controls.Add(this.panel5);
            this.profilePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.profilePanel.Location = new System.Drawing.Point(3, 3);
            this.profilePanel.Name = "profilePanel";
            this.profilePanel.Size = new System.Drawing.Size(302, 159);
            this.profilePanel.TabIndex = 58;
            // 
            // panel7
            // 
            this.panel7.Controls.Add(this.ProfilesListBox);
            this.panel7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel7.Location = new System.Drawing.Point(0, 57);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(302, 102);
            this.panel7.TabIndex = 67;
            // 
            // ProfilesListBox
            // 
            this.ProfilesListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ProfilesListBox.FormattingEnabled = true;
            this.ProfilesListBox.ItemHeight = 15;
            this.ProfilesListBox.Location = new System.Drawing.Point(0, 0);
            this.ProfilesListBox.Name = "ProfilesListBox";
            this.ProfilesListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.ProfilesListBox.Size = new System.Drawing.Size(302, 102);
            this.ProfilesListBox.TabIndex = 65;
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.profilesToolBar);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel6.Location = new System.Drawing.Point(0, 32);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(302, 25);
            this.panel6.TabIndex = 67;
            // 
            // profilesToolBar
            // 
            this.profilesToolBar.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
            this.AddProfile,
            this.RemoveProfile});
            this.profilesToolBar.ButtonSize = new System.Drawing.Size(18, 18);
            this.profilesToolBar.DropDownArrows = true;
            this.profilesToolBar.ImageList = this.imageList1;
            this.profilesToolBar.Location = new System.Drawing.Point(0, 0);
            this.profilesToolBar.Margin = new System.Windows.Forms.Padding(0);
            this.profilesToolBar.Name = "profilesToolBar";
            this.profilesToolBar.ShowToolTips = true;
            this.profilesToolBar.Size = new System.Drawing.Size(302, 28);
            this.profilesToolBar.TabIndex = 66;
            this.profilesToolBar.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.ToolBars_ButtonClick);
            // 
            // AddProfile
            // 
            this.AddProfile.ImageKey = "Plus.png";
            this.AddProfile.Name = "AddProfile";
            // 
            // RemoveProfile
            // 
            this.RemoveProfile.ImageIndex = 43;
            this.RemoveProfile.Name = "RemoveProfile";
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
            // panel5
            // 
            this.panel5.Controls.Add(this.lblProfiles);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel5.Location = new System.Drawing.Point(0, 0);
            this.panel5.Name = "panel5";
            this.panel5.Padding = new System.Windows.Forms.Padding(4);
            this.panel5.Size = new System.Drawing.Size(302, 32);
            this.panel5.TabIndex = 66;
            // 
            // lblProfiles
            // 
            this.lblProfiles.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblProfiles.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblProfiles.Location = new System.Drawing.Point(4, 4);
            this.lblProfiles.Name = "lblProfiles";
            this.lblProfiles.Padding = new System.Windows.Forms.Padding(0, 2, 0, 4);
            this.lblProfiles.Size = new System.Drawing.Size(274, 24);
            this.lblProfiles.TabIndex = 66;
            this.lblProfiles.Text = "Profiles";
            // 
            // panel4
            // 
            this.panel4.AutoSize = true;
            this.panel4.Controls.Add(this.panel8);
            this.panel4.Controls.Add(this.panel9);
            this.panel4.Controls.Add(this.panel10);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(3, 168);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(302, 159);
            this.panel4.TabIndex = 60;
            // 
            // panel8
            // 
            this.panel8.AutoSize = true;
            this.panel8.Controls.Add(this.SessionsListBox);
            this.panel8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel8.Location = new System.Drawing.Point(0, 57);
            this.panel8.Name = "panel8";
            this.panel8.Size = new System.Drawing.Size(302, 102);
            this.panel8.TabIndex = 67;
            // 
            // SessionsListBox
            // 
            this.SessionsListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SessionsListBox.FormattingEnabled = true;
            this.SessionsListBox.ItemHeight = 15;
            this.SessionsListBox.Location = new System.Drawing.Point(0, 0);
            this.SessionsListBox.Name = "SessionsListBox";
            this.SessionsListBox.Size = new System.Drawing.Size(302, 102);
            this.SessionsListBox.TabIndex = 65;
            // 
            // panel9
            // 
            this.panel9.Controls.Add(this.surfaceToolBar);
            this.panel9.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel9.Location = new System.Drawing.Point(0, 32);
            this.panel9.Name = "panel9";
            this.panel9.Size = new System.Drawing.Size(302, 25);
            this.panel9.TabIndex = 67;
            // 
            // surfaceToolBar
            // 
            this.surfaceToolBar.AutoSize = false;
            this.surfaceToolBar.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
            this.AddSurface,
            this.RemoveSurface});
            this.surfaceToolBar.ButtonSize = new System.Drawing.Size(18, 18);
            this.surfaceToolBar.DropDownArrows = true;
            this.surfaceToolBar.ImageList = this.imageList1;
            this.surfaceToolBar.Location = new System.Drawing.Point(0, 0);
            this.surfaceToolBar.Margin = new System.Windows.Forms.Padding(0);
            this.surfaceToolBar.Name = "surfaceToolBar";
            this.surfaceToolBar.ShowToolTips = true;
            this.surfaceToolBar.Size = new System.Drawing.Size(302, 28);
            this.surfaceToolBar.TabIndex = 65;
            this.surfaceToolBar.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.SurfaceToolBar_ButtonClick);
            // 
            // AddSurface
            // 
            this.AddSurface.ImageKey = "Plus.png";
            this.AddSurface.Name = "AddSurface";
            // 
            // RemoveSurface
            // 
            this.RemoveSurface.ImageIndex = 43;
            this.RemoveSurface.Name = "RemoveSurface";
            // 
            // panel10
            // 
            this.panel10.Controls.Add(this.surfaceLabels);
            this.panel10.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel10.Location = new System.Drawing.Point(0, 0);
            this.panel10.Name = "panel10";
            this.panel10.Size = new System.Drawing.Size(302, 32);
            this.panel10.TabIndex = 66;
            // 
            // surfaceLabels
            // 
            this.surfaceLabels.Dock = System.Windows.Forms.DockStyle.Left;
            this.surfaceLabels.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.surfaceLabels.Location = new System.Drawing.Point(0, 0);
            this.surfaceLabels.Name = "surfaceLabels";
            this.surfaceLabels.Padding = new System.Windows.Forms.Padding(0, 4, 0, 4);
            this.surfaceLabels.Size = new System.Drawing.Size(278, 32);
            this.surfaceLabels.TabIndex = 66;
            this.surfaceLabels.Text = "Surfaces";
            // 
            // panel12
            // 
            this.panel12.Controls.Add(this.GenerateButton);
            this.panel12.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel12.Location = new System.Drawing.Point(3, 333);
            this.panel12.Name = "panel12";
            this.panel12.Size = new System.Drawing.Size(308, 30);
            this.panel12.TabIndex = 61;
            // 
            // GenerateButton
            // 
            this.GenerateButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.GenerateButton.AutoSize = true;
            this.GenerateButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.GenerateButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.GenerateButton.Location = new System.Drawing.Point(237, 2);
            this.GenerateButton.Name = "GenerateButton";
            this.GenerateButton.Size = new System.Drawing.Size(68, 25);
            this.GenerateButton.TabIndex = 59;
            this.GenerateButton.Text = "Generate";
            this.GenerateButton.UseVisualStyleBackColor = true;
            this.GenerateButton.Click += new System.EventHandler(this.GenerateButton_Click);
            // 
            // GenerateImageTab
            // 
            this.GenerateImageTab.BackColor = System.Drawing.SystemColors.Control;
            this.GenerateImageTab.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.GenerateImageTab.Location = new System.Drawing.Point(4, 24);
            this.GenerateImageTab.Name = "GenerateImageTab";
            this.GenerateImageTab.Padding = new System.Windows.Forms.Padding(3);
            this.GenerateImageTab.Size = new System.Drawing.Size(314, 366);
            this.GenerateImageTab.TabIndex = 1;
            this.GenerateImageTab.Text = "tabPage2";
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.SystemColors.ControlDark;
            this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel3.Location = new System.Drawing.Point(0, 202);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(322, 4);
            this.panel3.TabIndex = 78;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.tbZFactor);
            this.panel2.Controls.Add(this.lblZFactor);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 174);
            this.panel2.Name = "panel2";
            this.panel2.Padding = new System.Windows.Forms.Padding(4);
            this.panel2.Size = new System.Drawing.Size(322, 28);
            this.panel2.TabIndex = 77;
            // 
            // tbZFactor
            // 
            this.tbZFactor.Dock = System.Windows.Forms.DockStyle.Left;
            this.tbZFactor.Location = new System.Drawing.Point(134, 4);
            this.tbZFactor.Name = "tbZFactor";
            this.tbZFactor.Size = new System.Drawing.Size(74, 20);
            this.tbZFactor.TabIndex = 2;
            this.tbZFactor.TextChanged += new System.EventHandler(this.TbZFactor_TextChanged);
            // 
            // lblZFactor
            // 
            this.lblZFactor.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblZFactor.Location = new System.Drawing.Point(4, 4);
            this.lblZFactor.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.lblZFactor.Name = "lblZFactor";
            this.lblZFactor.Padding = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.lblZFactor.Size = new System.Drawing.Size(130, 20);
            this.lblZFactor.TabIndex = 1;
            this.lblZFactor.Text = "Height conversion factor";
            // 
            // panel16
            // 
            this.panel16.Controls.Add(this.HydroHightTextBox);
            this.panel16.Controls.Add(this.HydroHightLabel);
            this.panel16.Controls.Add(this.HydroLayerComboBox);
            this.panel16.Controls.Add(this.HydroLayerLabel);
            this.panel16.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel16.Location = new System.Drawing.Point(0, 146);
            this.panel16.Name = "panel16";
            this.panel16.Padding = new System.Windows.Forms.Padding(4);
            this.panel16.Size = new System.Drawing.Size(322, 28);
            this.panel16.TabIndex = 75;
            // 
            // HydroHightTextBox
            // 
            this.HydroHightTextBox.Dock = System.Windows.Forms.DockStyle.Left;
            this.HydroHightTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.HydroHightTextBox.Location = new System.Drawing.Point(252, 4);
            this.HydroHightTextBox.Name = "HydroHightTextBox";
            this.HydroHightTextBox.Size = new System.Drawing.Size(60, 20);
            this.HydroHightTextBox.TabIndex = 62;
            // 
            // HydroHightLabel
            // 
            this.HydroHightLabel.Dock = System.Windows.Forms.DockStyle.Left;
            this.HydroHightLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.HydroHightLabel.Location = new System.Drawing.Point(208, 4);
            this.HydroHightLabel.Name = "HydroHightLabel";
            this.HydroHightLabel.Padding = new System.Windows.Forms.Padding(2, 0, 0, 0);
            this.HydroHightLabel.Size = new System.Drawing.Size(44, 20);
            this.HydroHightLabel.TabIndex = 61;
            this.HydroHightLabel.Text = "Hight";
            this.HydroHightLabel.Click += new System.EventHandler(this.HydroHightLabel_Click);
            // 
            // HydroLayerComboBox
            // 
            this.HydroLayerComboBox.Dock = System.Windows.Forms.DockStyle.Left;
            this.HydroLayerComboBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.HydroLayerComboBox.FormattingEnabled = true;
            this.HydroLayerComboBox.Location = new System.Drawing.Point(74, 4);
            this.HydroLayerComboBox.Name = "HydroLayerComboBox";
            this.HydroLayerComboBox.Size = new System.Drawing.Size(134, 21);
            this.HydroLayerComboBox.TabIndex = 48;
            // 
            // HydroLayerLabel
            // 
            this.HydroLayerLabel.Dock = System.Windows.Forms.DockStyle.Left;
            this.HydroLayerLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.HydroLayerLabel.Location = new System.Drawing.Point(4, 4);
            this.HydroLayerLabel.Name = "HydroLayerLabel";
            this.HydroLayerLabel.Size = new System.Drawing.Size(70, 20);
            this.HydroLayerLabel.TabIndex = 47;
            this.HydroLayerLabel.Text = "Hydro";
            // 
            // panel15
            // 
            this.panel15.Controls.Add(this.TransportHightTextBox);
            this.panel15.Controls.Add(this.TransportHightLabel);
            this.panel15.Controls.Add(this.TransportLayerComboBox);
            this.panel15.Controls.Add(this.TransportLayerLabel);
            this.panel15.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel15.Location = new System.Drawing.Point(0, 118);
            this.panel15.Name = "panel15";
            this.panel15.Padding = new System.Windows.Forms.Padding(4);
            this.panel15.Size = new System.Drawing.Size(322, 28);
            this.panel15.TabIndex = 74;
            // 
            // TransportHightTextBox
            // 
            this.TransportHightTextBox.Dock = System.Windows.Forms.DockStyle.Left;
            this.TransportHightTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.TransportHightTextBox.Location = new System.Drawing.Point(252, 4);
            this.TransportHightTextBox.Name = "TransportHightTextBox";
            this.TransportHightTextBox.Size = new System.Drawing.Size(60, 20);
            this.TransportHightTextBox.TabIndex = 60;
            // 
            // TransportHightLabel
            // 
            this.TransportHightLabel.Dock = System.Windows.Forms.DockStyle.Left;
            this.TransportHightLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.TransportHightLabel.Location = new System.Drawing.Point(208, 4);
            this.TransportHightLabel.Name = "TransportHightLabel";
            this.TransportHightLabel.Padding = new System.Windows.Forms.Padding(2, 0, 0, 0);
            this.TransportHightLabel.Size = new System.Drawing.Size(44, 20);
            this.TransportHightLabel.TabIndex = 59;
            this.TransportHightLabel.Text = "Hight";
            // 
            // TransportLayerComboBox
            // 
            this.TransportLayerComboBox.Dock = System.Windows.Forms.DockStyle.Left;
            this.TransportLayerComboBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.TransportLayerComboBox.FormattingEnabled = true;
            this.TransportLayerComboBox.Location = new System.Drawing.Point(74, 4);
            this.TransportLayerComboBox.Name = "TransportLayerComboBox";
            this.TransportLayerComboBox.Size = new System.Drawing.Size(134, 21);
            this.TransportLayerComboBox.TabIndex = 46;
            // 
            // TransportLayerLabel
            // 
            this.TransportLayerLabel.Dock = System.Windows.Forms.DockStyle.Left;
            this.TransportLayerLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.TransportLayerLabel.Location = new System.Drawing.Point(4, 4);
            this.TransportLayerLabel.Name = "TransportLayerLabel";
            this.TransportLayerLabel.Size = new System.Drawing.Size(70, 20);
            this.TransportLayerLabel.TabIndex = 45;
            this.TransportLayerLabel.Text = "Transport";
            // 
            // panel14
            // 
            this.panel14.Controls.Add(this.BuildingsHightComboBox);
            this.panel14.Controls.Add(this.BuildingsHight);
            this.panel14.Controls.Add(this.BuildingsLayerComboBox);
            this.panel14.Controls.Add(this.BuildingsLayerLabel);
            this.panel14.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel14.Location = new System.Drawing.Point(0, 90);
            this.panel14.Name = "panel14";
            this.panel14.Padding = new System.Windows.Forms.Padding(4);
            this.panel14.Size = new System.Drawing.Size(322, 28);
            this.panel14.TabIndex = 73;
            // 
            // BuildingsHightComboBox
            // 
            this.BuildingsHightComboBox.Dock = System.Windows.Forms.DockStyle.Left;
            this.BuildingsHightComboBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.BuildingsHightComboBox.FormattingEnabled = true;
            this.BuildingsHightComboBox.Location = new System.Drawing.Point(254, 4);
            this.BuildingsHightComboBox.Name = "BuildingsHightComboBox";
            this.BuildingsHightComboBox.Size = new System.Drawing.Size(60, 21);
            this.BuildingsHightComboBox.TabIndex = 58;
            // 
            // BuildingsHight
            // 
            this.BuildingsHight.AutoSize = true;
            this.BuildingsHight.Dock = System.Windows.Forms.DockStyle.Left;
            this.BuildingsHight.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.BuildingsHight.Location = new System.Drawing.Point(208, 4);
            this.BuildingsHight.Name = "BuildingsHight";
            this.BuildingsHight.Padding = new System.Windows.Forms.Padding(2, 2, 0, 0);
            this.BuildingsHight.Size = new System.Drawing.Size(46, 15);
            this.BuildingsHight.TabIndex = 54;
            this.BuildingsHight.Text = "высота";
            // 
            // BuildingsLayerComboBox
            // 
            this.BuildingsLayerComboBox.Dock = System.Windows.Forms.DockStyle.Left;
            this.BuildingsLayerComboBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.BuildingsLayerComboBox.FormattingEnabled = true;
            this.BuildingsLayerComboBox.Location = new System.Drawing.Point(74, 4);
            this.BuildingsLayerComboBox.Name = "BuildingsLayerComboBox";
            this.BuildingsLayerComboBox.Size = new System.Drawing.Size(134, 21);
            this.BuildingsLayerComboBox.TabIndex = 44;
            // 
            // BuildingsLayerLabel
            // 
            this.BuildingsLayerLabel.Dock = System.Windows.Forms.DockStyle.Left;
            this.BuildingsLayerLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.BuildingsLayerLabel.Location = new System.Drawing.Point(4, 4);
            this.BuildingsLayerLabel.Name = "BuildingsLayerLabel";
            this.BuildingsLayerLabel.Size = new System.Drawing.Size(70, 20);
            this.BuildingsLayerLabel.TabIndex = 43;
            this.BuildingsLayerLabel.Text = "Buildings Layer";
            // 
            // panel13
            // 
            this.panel13.Controls.Add(this.PlantsHightComboBox);
            this.panel13.Controls.Add(this.PlantsHightLablel);
            this.panel13.Controls.Add(this.PlantsLayerComboBox);
            this.panel13.Controls.Add(this.PlantsLayerLabel);
            this.panel13.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel13.Location = new System.Drawing.Point(0, 62);
            this.panel13.Name = "panel13";
            this.panel13.Padding = new System.Windows.Forms.Padding(4);
            this.panel13.Size = new System.Drawing.Size(322, 28);
            this.panel13.TabIndex = 72;
            // 
            // PlantsHightComboBox
            // 
            this.PlantsHightComboBox.Dock = System.Windows.Forms.DockStyle.Left;
            this.PlantsHightComboBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.PlantsHightComboBox.FormattingEnabled = true;
            this.PlantsHightComboBox.Location = new System.Drawing.Point(254, 4);
            this.PlantsHightComboBox.Name = "PlantsHightComboBox";
            this.PlantsHightComboBox.Size = new System.Drawing.Size(60, 21);
            this.PlantsHightComboBox.TabIndex = 56;
            // 
            // PlantsHightLablel
            // 
            this.PlantsHightLablel.AutoSize = true;
            this.PlantsHightLablel.Dock = System.Windows.Forms.DockStyle.Left;
            this.PlantsHightLablel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.PlantsHightLablel.Location = new System.Drawing.Point(208, 4);
            this.PlantsHightLablel.Name = "PlantsHightLablel";
            this.PlantsHightLablel.Padding = new System.Windows.Forms.Padding(2, 2, 0, 0);
            this.PlantsHightLablel.Size = new System.Drawing.Size(46, 15);
            this.PlantsHightLablel.TabIndex = 53;
            this.PlantsHightLablel.Text = "высота";
            // 
            // PlantsLayerComboBox
            // 
            this.PlantsLayerComboBox.Dock = System.Windows.Forms.DockStyle.Left;
            this.PlantsLayerComboBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.PlantsLayerComboBox.FormattingEnabled = true;
            this.PlantsLayerComboBox.Location = new System.Drawing.Point(74, 4);
            this.PlantsLayerComboBox.Name = "PlantsLayerComboBox";
            this.PlantsLayerComboBox.Size = new System.Drawing.Size(134, 21);
            this.PlantsLayerComboBox.TabIndex = 42;
            // 
            // PlantsLayerLabel
            // 
            this.PlantsLayerLabel.Dock = System.Windows.Forms.DockStyle.Left;
            this.PlantsLayerLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.PlantsLayerLabel.Location = new System.Drawing.Point(4, 4);
            this.PlantsLayerLabel.Name = "PlantsLayerLabel";
            this.PlantsLayerLabel.Padding = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.PlantsLayerLabel.Size = new System.Drawing.Size(70, 20);
            this.PlantsLayerLabel.TabIndex = 41;
            this.PlantsLayerLabel.Text = "Plants Layer";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.SurfaceComboBox);
            this.panel1.Controls.Add(this.SurfaceLabel);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 32);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(4);
            this.panel1.Size = new System.Drawing.Size(322, 30);
            this.panel1.TabIndex = 68;
            // 
            // SurfaceComboBox
            // 
            this.SurfaceComboBox.Dock = System.Windows.Forms.DockStyle.Left;
            this.SurfaceComboBox.FormattingEnabled = true;
            this.SurfaceComboBox.Location = new System.Drawing.Point(74, 4);
            this.SurfaceComboBox.Name = "SurfaceComboBox";
            this.SurfaceComboBox.Size = new System.Drawing.Size(245, 21);
            this.SurfaceComboBox.TabIndex = 41;
            // 
            // SurfaceLabel
            // 
            this.SurfaceLabel.Dock = System.Windows.Forms.DockStyle.Left;
            this.SurfaceLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.SurfaceLabel.Location = new System.Drawing.Point(4, 4);
            this.SurfaceLabel.Name = "SurfaceLabel";
            this.SurfaceLabel.Size = new System.Drawing.Size(70, 22);
            this.SurfaceLabel.TabIndex = 40;
            this.SurfaceLabel.Text = "Surface";
            // 
            // panel11
            // 
            this.panel11.Controls.Add(this.btnRefreshLayers);
            this.panel11.Controls.Add(this.lbl3DProfiles);
            this.panel11.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel11.Location = new System.Drawing.Point(0, 0);
            this.panel11.Name = "panel11";
            this.panel11.Padding = new System.Windows.Forms.Padding(4);
            this.panel11.Size = new System.Drawing.Size(322, 32);
            this.panel11.TabIndex = 71;
            // 
            // btnRefreshLayers
            // 
            this.btnRefreshLayers.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefreshLayers.ImageKey = "Refresh20x20.png";
            this.btnRefreshLayers.ImageList = this.imageList1;
            this.btnRefreshLayers.Location = new System.Drawing.Point(290, 3);
            this.btnRefreshLayers.Name = "btnRefreshLayers";
            this.btnRefreshLayers.Size = new System.Drawing.Size(25, 25);
            this.btnRefreshLayers.TabIndex = 44;
            this.btnRefreshLayers.UseVisualStyleBackColor = true;
            this.btnRefreshLayers.Click += new System.EventHandler(this.btnRefreshLayers_Click);
            // 
            // lbl3DProfiles
            // 
            this.lbl3DProfiles.Dock = System.Windows.Forms.DockStyle.Left;
            this.lbl3DProfiles.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.lbl3DProfiles.Location = new System.Drawing.Point(4, 4);
            this.lbl3DProfiles.Name = "lbl3DProfiles";
            this.lbl3DProfiles.Padding = new System.Windows.Forms.Padding(0, 2, 0, 4);
            this.lbl3DProfiles.Size = new System.Drawing.Size(218, 24);
            this.lbl3DProfiles.TabIndex = 66;
            this.lbl3DProfiles.Text = "Layers";
            // 
            // Visualization3DMainForm
            // 
            this.AutoSize = true;
            this.Controls.Add(this.basePanel);
            this.Name = "Visualization3DMainForm";
            this.Size = new System.Drawing.Size(322, 600);
            this.basePanel.ResumeLayout(false);
            this.GenerateTab.ResumeLayout(false);
            this.ProfilesTabPage.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.profilePanel.ResumeLayout(false);
            this.panel7.ResumeLayout(false);
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.panel8.ResumeLayout(false);
            this.panel9.ResumeLayout(false);
            this.panel10.ResumeLayout(false);
            this.panel12.ResumeLayout(false);
            this.panel12.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel16.ResumeLayout(false);
            this.panel16.PerformLayout();
            this.panel15.ResumeLayout(false);
            this.panel15.PerformLayout();
            this.panel14.ResumeLayout(false);
            this.panel14.PerformLayout();
            this.panel13.ResumeLayout(false);
            this.panel13.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel11.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel basePanel;
        private System.Windows.Forms.ImageList imageList1;
        internal System.Windows.Forms.ToolBarButton toolBarButton1;
        private System.Windows.Forms.ToolBarButton toolBarButton2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ComboBox SurfaceComboBox;
        private System.Windows.Forms.Label SurfaceLabel;
        private System.Windows.Forms.Panel panel11;
        private System.Windows.Forms.Label lbl3DProfiles;
        private System.Windows.Forms.Button btnRefreshLayers;
        private System.Windows.Forms.Panel panel14;
        private System.Windows.Forms.ComboBox BuildingsHightComboBox;
        private System.Windows.Forms.Label BuildingsHight;
        private System.Windows.Forms.ComboBox BuildingsLayerComboBox;
        private System.Windows.Forms.Label BuildingsLayerLabel;
        private System.Windows.Forms.Panel panel13;
        private System.Windows.Forms.ComboBox PlantsHightComboBox;
        private System.Windows.Forms.Label PlantsHightLablel;
        private System.Windows.Forms.ComboBox PlantsLayerComboBox;
        private System.Windows.Forms.Label PlantsLayerLabel;
        private System.Windows.Forms.Panel panel16;
        private System.Windows.Forms.TextBox HydroHightTextBox;
        private System.Windows.Forms.Label HydroHightLabel;
        private System.Windows.Forms.ComboBox HydroLayerComboBox;
        private System.Windows.Forms.Label HydroLayerLabel;
        private System.Windows.Forms.Panel panel15;
        private System.Windows.Forms.TextBox TransportHightTextBox;
        private System.Windows.Forms.Label TransportHightLabel;
        private System.Windows.Forms.ComboBox TransportLayerComboBox;
        private System.Windows.Forms.Label TransportLayerLabel;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TextBox tbZFactor;
        private System.Windows.Forms.Label lblZFactor;
        private System.Windows.Forms.TabControl GenerateTab;
        private System.Windows.Forms.TabPage ProfilesTabPage;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Panel profilePanel;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.ListBox ProfilesListBox;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.ToolBar profilesToolBar;
        internal System.Windows.Forms.ToolBarButton AddProfile;
        private System.Windows.Forms.ToolBarButton RemoveProfile;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Label lblProfiles;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel8;
        private System.Windows.Forms.ListBox SessionsListBox;
        private System.Windows.Forms.Panel panel9;
        private System.Windows.Forms.ToolBar surfaceToolBar;
        internal System.Windows.Forms.ToolBarButton AddSurface;
        private System.Windows.Forms.ToolBarButton RemoveSurface;
        private System.Windows.Forms.Panel panel10;
        private System.Windows.Forms.Label surfaceLabels;
        private System.Windows.Forms.Panel panel12;
        private System.Windows.Forms.Button GenerateButton;
        private System.Windows.Forms.TabPage GenerateImageTab;
        private System.Windows.Forms.Panel panel3;
    }
}
