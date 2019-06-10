using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geometry;
using MilSpace.Core;
using MilSpace.Core.MilSpaceResourceManager;
using MilSpace.Core.Tools;
using MilSpace.DataAccess.DataTransfer;
using MilSpace.Profile.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Point = ESRI.ArcGIS.Geometry.Point;


namespace MilSpace.Profile
{
    /// <summary>
    /// Designer class of the dockable window add-in. It contains user interfaces that
    /// make up the dockable window.
    /// </summary>
    public partial class DockableWindowMilSpaceProfileCalc : UserControl, IMilSpaceProfileView
    {
        const int BACKSPACE = 8;
        const int DECIMAL_POINT = 46;
        const int ZERO = 48;
        const int NINE = 57;
        const int NOT_FOUND = -1;
        private const string Degree = "°";

        private static readonly Dictionary<ProfileSettingsTypeEnum, int[]> nodeDefinition = new Dictionary<ProfileSettingsTypeEnum, int[]>
        {
            {ProfileSettingsTypeEnum.Points , new int[] { 205, 205 } },
            {ProfileSettingsTypeEnum.Fun , new int[] { 206, 208 } },
            {ProfileSettingsTypeEnum.Primitives , new int[] {207, 209 } },
        };

        private ProfileSettingsPointButtonEnum activeButtton = ProfileSettingsPointButtonEnum.None;

        private TreeViewSelectedProfileIds treeViewselectedIds; //= new TreeViewSelectedProfileIds();

        MilSpaceProfileCalsController controller;

        List<ProfileSession> _fanProfiles = new List<ProfileSession>();
        List<ProfileSession> _graphicProfiles = new List<ProfileSession>();

        Dictionary<ProfileSettingsTypeEnum, List<ProfileSession>> profileLists = new Dictionary<ProfileSettingsTypeEnum, List<ProfileSession>>
        {
            {ProfileSettingsTypeEnum.Points, new List<ProfileSession>() }

        };

        public DockableWindowMilSpaceProfileCalc(MilSpaceProfileCalsController controller)
        {
            this.Instance = this;
            SetController(controller);
            controller.SetView(this);
            LocalizeStrings();

        }

        public DockableWindowMilSpaceProfileCalc(object hook, MilSpaceProfileCalsController controller)
        {
            InitializeComponent();
            SetController(controller);
            this.Hook = hook;
            this.Instance = this;
            SubscribeForEvents();
            controller.SetView(this);
            LocalizeStrings();
        }

        private void OnProfileSettingsChanged(ProfileSettingsEventArgs e)
        {
            this.calcProfile.Enabled = e.ProfileSetting.IsReady;
        }

        public IActiveView ActiveView => ArcMap.Document.ActiveView;

        public ISpatialReference MapSpatialreverence => ArcMap.Document.FocusMap.SpatialReference;

        public MilSpaceProfileCalsController Controller => controller;

        public ProfileSettingsPointButtonEnum ActiveButton => activeButtton;

        public IEnumerable<string> GetLayersForLineSelection => controller.GetLayersForLineSelection();

        public string DemLayerName => cmbRasterLayers.SelectedItem == null ? string.Empty : cmbRasterLayers.SelectedItem.ToString();

        public int ProfileId
        {
            set { txtProfileName.Text = value.ToString(); }
        }

        public string ProfileName
        {
            get => txtProfileName.Text;
            set
            {
                txtProfileName.Clear();
                txtProfileName.Text = value;
                var text = txtProfileName.Text;
            }
        }

        public TreeViewSelectedProfileIds SelectedProfileSessionIds => treeViewselectedIds;


        public void AddProfileToList(ProfileSession profile)
        {
            profileLists[profile.DefinitionType].Add(profile);
        }

        public ProfileSession GetProfileFromList(string profileName, ProfileSettingsTypeEnum profileType)
        {
            return GetProfileFromList(profileLists[profileType], profileName);
        }


        public List<string> GetLayers()
        {
            return new List<string>
            {
                (cmbVegetationLayer.SelectedItem != null)  ?  cmbVegetationLayer.SelectedItem.ToString()  : String.Empty,
                (cmbBuildings.SelectedItem != null)        ?  cmbBuildings.SelectedItem.ToString()        : String.Empty,
                (cmbRoadLayers.SelectedItem != null)       ?  cmbRoadLayers.SelectedItem.ToString()       : String.Empty,
                (cmbHydrographyLayer.SelectedItem != null) ?  cmbHydrographyLayer.SelectedItem.ToString() : String.Empty
            };
        }


        public bool AllowToProfileCalc
        {
            get
            {
                return false;
            }
        }

        public DockableWindowMilSpaceProfileCalc Instance { get; }

        /// <summary>
        /// Host object of the dockable window
        /// </summary>
        private object Hook
        {
            get;
            set;
        }

        protected override void OnLoad(EventArgs e)
        {
            Helper.SetConfiguration();
        }

        private void OnDocumentOpenFillDropdowns()
        {
            cmbRasterLayers.Items.Clear();
            cmbRoadLayers.Items.Clear();
            cmbHydrographyLayer.Items.Clear();
            cmbBuildings.Items.Clear();
            cmbVegetationLayer.Items.Clear();
            cmbPointLayers.Items.Clear();
            layersToSelectLine.Items.Clear();

            PopulateComboBox(cmbRasterLayers, ProfileLayers.RasterLayers);
            PopulateComboBox(cmbRoadLayers, ProfileLayers.PolygonLayers);
            PopulateComboBox(cmbHydrographyLayer, ProfileLayers.PolygonLayers);
            PopulateComboBox(cmbBuildings, ProfileLayers.PolygonLayers);
            PopulateComboBox(cmbVegetationLayer, ProfileLayers.PolygonLayers);
            PopulateComboBox(cmbPointLayers, ProfileLayers.PointLayers);

            layersToSelectLine.Items.AddRange(GetLayersForLineSelection.ToArray());
            layersToSelectLine.SelectedItem = layersToSelectLine.Items[0];
        }


        public bool RemoveTreeViewItem()
        {
            try
            {
                var node = profilesTreeView.SelectedNode.Parent;
                profilesTreeView.SelectedNode.Remove();
            }
            catch (Exception ex)
            {
                //TODO: log exception
                return false;
            }

            return true;
        }


        private void ChangeTreeViewToolbarState(object sender, TreeViewEventArgs treeViewEventArgs)
        {
            var node = profilesTreeView.SelectedNode;
            var ids = GetProfileAndLineIds(node);

            treeViewselectedIds.ProfileLineId = ids.Item2;
            treeViewselectedIds.ProfileSessionId = ids.Item1;

            var pr = controller.GetProfileById(treeViewselectedIds.ProfileSessionId);

            saveProfileAsShared.Enabled = (pr != null && pr.CreatedBy == Environment.UserName && !pr.Shared);

            removeProfile.Enabled = addProfileToGraph.Enabled = toolBtnShowOnMap.Enabled = toolBtnFlash.Enabled = treeViewselectedIds.ProfileSessionId > 0;
            openGraphWindow.Enabled = !controller.MilSpaceProfileGraphsController.IsWindowVisible;

            eraseProfile.Enabled = controller.CanEraseProfileSession(ids.Item1);
        }

        private void DisplaySelectedNodeAttributes(object sender, TreeViewEventArgs treeViewEventArgs)
        {
            lvProfileAttributes.Items.Clear();
            var node = profilesTreeView.SelectedNode;
            if (node.Parent == null)
            {
                lvProfileAttributes.Visible = false;
                return;
            }

            lvProfileAttributes.Visible = true;
            if (!(node is ProfileTreeNode)) return;
            ProfileTreeNode profileNode = (ProfileTreeNode)node;
            foreach (DataRow row in profileNode.Attributes.Rows)
            {
                if (row[AttributeKeys.ValueColumnName].ToString() == string.Empty)
                {
                    continue;
                }

                var lvItem = new ListViewItem(row[AttributeKeys.AttributeColumnName].ToString());
                lvItem.SubItems.Add(row[AttributeKeys.ValueColumnName].ToString());

                lvProfileAttributes.Items.Add(lvItem);
            }
        }

        private void OnListViewResize(object sender, EventArgs eventArgs)
        {
            lvProfileAttributes.Columns[0].Width = lvProfileAttributes.Width / 2 - 10;
            lvProfileAttributes.Columns[1].Width = lvProfileAttributes.Width / 2 - 10;
        }


        private static void PopulateComboBox(ComboBox comboBox, IEnumerable<ILayer> layers)
        {
            comboBox.Items.AddRange(layers.Select(l => l.Name).ToArray());
        }

        private void SubscribeForEvents()
        {

            ArcMap.Events.OpenDocument += OnDocumentOpenFillDropdowns;
            ArcMap.Events.OpenDocument += controller.InitiateUserProfiles;

            profilesTreeView.AfterSelect += ChangeTreeViewToolbarState;
            profilesTreeView.AfterSelect += DisplaySelectedNodeAttributes;
            lvProfileAttributes.Resize += OnListViewResize;

            controller.OnProfileSettingsChanged += OnProfileSettingsChanged;
            controller.OnMapSelectionChanged += Controller_OnMapSelectionChanged;

            azimuth1.LostFocus += AzimuthCheck;
            azimuth2.LostFocus += AzimuthCheck;
        }

        private void Controller_OnMapSelectionChanged(SelectedGraphicsArgs selectedLines)
        {
            lblSelectedPrimitivesValue.Text = selectedLines.LinesCount.ToString();
            lblCommonLengthValue.Text = selectedLines.FullLength.ToString("F2");
        }

        public void SetController(MilSpaceProfileCalsController controller)
        {
            this.controller = controller;
        }

        /// <summary>
        /// Implementation class of the dockable window add-in. It is responsible for 
        /// creating and disposing the user interface class of the dockable window.
        /// </summary>
        public class AddinImpl : ESRI.ArcGIS.Desktop.AddIns.DockableWindow
        {
            private DockableWindowMilSpaceProfileCalc m_windowUI;
            MilSpaceProfileCalsController controller;


            public AddinImpl()
            {
                Helper.SetConfiguration();
            }

            protected override IntPtr OnCreateChild()
            {
                controller = new MilSpaceProfileCalsController();

                m_windowUI = new DockableWindowMilSpaceProfileCalc(this.Hook, controller);


                return m_windowUI.Handle;
            }

            protected override void Dispose(bool disposing)
            {
                if (m_windowUI != null)
                    m_windowUI.Dispose(disposing);

                base.Dispose(disposing);
            }

            internal DockableWindowMilSpaceProfileCalc DockableWindowUI => m_windowUI;


            internal MilSpaceProfileCalsController MilSpaceProfileCalsController => controller;

        }

        internal ToolBarButton ToolbarButtonClicked { get; private set; }

        private void button1_Click(object sender, EventArgs e)
        {
            ArcMap.Application.CurrentTool = null;
            UID dockWinID = new UIDClass();
            dockWinID.Value = ThisAddIn.IDs.DockableWindowMilSpaceProfileGraph;
            IDockableWindow dockWindow = ArcMap.DockableWindowManager.GetDockableWindow(dockWinID);
            dockWindow.Show(true);
        }

        private void toolBar1_ButtonClick(object sender, ToolBarButtonClickEventArgs e)
        {
            ToolbarButtonClicked = e.Button;
            switch (firstPointToolBar.Buttons.IndexOf(e.Button))
            {

                case 0:

                    var commandItem = ArcMap.Application.Document.CommandBars.Find(ThisAddIn.IDs.PickCoordinates);
                    if (commandItem == null)
                    {
                        var message = $"Please add Pick Coordinates tool to any toolbar first.";
                        MessageBox.Show(message, "Profile Calc", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        break;

                    }
                    ArcMap.Application.CurrentTool = commandItem;
                    activeButtton = ProfileSettingsPointButtonEnum.PointsFist;

                    break;
                case 1:

                    break;
                case 2:
                    controller.FlashPoint(ProfileSettingsPointButtonEnum.PointsFist);
                    break;

                case 4:

                    if (txtFirstPointX.Focused)
                    {
                        CopyTextToBuffer(txtFirstPointX.Text);
                    }

                    CopyTextToBuffer(txtFirstPointY.Focused ? txtFirstPointY.Text : txtFirstPointX.Text);

                    break;

                case 5:
                    if (txtFirstPointX.Focused)
                    {
                        PasteTextToEditField(txtFirstPointX);
                    }

                    PasteTextToEditField(txtFirstPointY.Focused ? txtFirstPointY : txtFirstPointX);

                    break;

                case 7:

                    txtFirstPointX.Clear();
                    txtFirstPointY.Clear();
                    controller.SetFirsPointForLineProfile(null, null);
                    break;

            }
        }

        private void secondPointToolbar_ButtonClick(object sender, ToolBarButtonClickEventArgs e)
        {
            ToolbarButtonClicked = e.Button;
            switch (secondPointToolbar.Buttons.IndexOf(e.Button))
            {
                case 0:

                    var commandItem = ArcMap.Application.Document.CommandBars.Find(ThisAddIn.IDs.PickCoordinates);
                    if (commandItem == null)
                    {
                        var message = $"Please add Pick Coordinates tool to any toolbar first.";
                        MessageBox.Show(message, "Profile Calc", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        break;

                    }
                    ArcMap.Application.CurrentTool = commandItem;

                    activeButtton = ProfileSettingsPointButtonEnum.PointsSecond;

                    break;
                case 1:
                    controller.FlashPoint(ProfileSettingsPointButtonEnum.PointsSecond);
                    break;
                case 2:
                    break;

                case 4:

                    if (txtSecondPointX.Focused)
                    {
                        CopyTextToBuffer(txtSecondPointX.Text);
                    }


                    CopyTextToBuffer(txtSecondPointY.Focused ? txtSecondPointY.Text : txtSecondPointX.Text);

                    break;

                case 5:
                    if (txtSecondPointX.Focused)
                    {
                        PasteTextToEditField(txtSecondPointX);
                    }


                    PasteTextToEditField(txtSecondPointY.Focused ? txtSecondPointY : txtSecondPointX);

                    break;

                case 7:

                    txtSecondPointX.Clear();
                    txtSecondPointY.Clear();
                    controller.SetSecondfPointForLineProfile(null, null);
                    break;

            }
        }

        private void toolBar3_ButtonClick(object sender, ToolBarButtonClickEventArgs e)
        {
            ToolbarButtonClicked = e.Button;
            switch (basePointToolbar.Buttons.IndexOf(e.Button))
            {

                case 1:

                    var commandItem = ArcMap.Application.Document.CommandBars.Find(ThisAddIn.IDs.PickCoordinates);
                    if (commandItem == null)
                    {
                        var message = $"Please add Pick Coordinates tool to any toolbar first.";
                        MessageBox.Show(message, "Profile Calc", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        break;

                    }
                    ArcMap.Application.CurrentTool = commandItem;

                    activeButtton = ProfileSettingsPointButtonEnum.CenterFun;

                    break;
                case 0:

                    break;
                case 2:

                    controller.FlashPoint(ProfileSettingsPointButtonEnum.CenterFun);
                    break;


                case 4:

                    if (txtBasePointX.Focused)
                    {
                        CopyTextToBuffer(txtBasePointX.Text);
                    }


                    CopyTextToBuffer(txtBasePointY.Focused ? txtBasePointY.Text : txtBasePointX.Text);

                    break;

                case 5:
                    if (txtBasePointX.Focused)
                    {
                        PasteTextToEditField(txtBasePointX);
                    }

                    PasteTextToEditField(txtBasePointY.Focused ? txtBasePointY : txtBasePointX);

                    break;

                case 7:

                    txtBasePointX.Clear();
                    txtBasePointY.Clear();
                    controller.SetCenterPointForFunProfile(null, null);
                    break;

            }
        }


        private IPoint ParseStringCoordsToPoint(string coordX, string coordY)
        {
            try
            {
                var x = double.Parse(coordX);
                var y = double.Parse(coordY);
                var point = new Point()
                {
                    X = x,
                    Y = y,
                    SpatialReference = EsriTools.Wgs84Spatialreference
                };

                EsriTools.ProjectToMapSpatialReference(point, ArcMap.Document.FocusMap.SpatialReference);

                return point;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Please make sure X and Y values are valid and try again!");
                throw;
            }

        }

        private void CopyTextToBuffer(string text)
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                System.Windows.Forms.Clipboard.SetText(text);
            }
        }

        private void PasteTextToEditField(TextBox textBox)
        {
            var text = Clipboard.GetText();
            textBox.Text = text;
        }

        public ProfileSettingsTypeEnum SelectedProfileSettingsType => controller.ProfileSettingsType[profileSettingsTab.SelectedIndex];

        public IPoint LinePropertiesFirstPoint
        {
            set
            {
                SetPointValue(txtFirstPointX, txtFirstPointY, value);
            }
        }

        /// <summary>
        /// Second point for Line  Profile setting 
        /// </summary>
        public IPoint LinePropertiesSecondPoint
        {
            set
            {
                SetPointValue(txtSecondPointX, txtSecondPointY, value);
            }
        }


        private static void SetPointValue(TextBox controlX, TextBox controlY, IPoint point)
        {

            if (point != null)
            {
                controlX.Text = point.X.ToString("F5");
                controlY.Text = point.Y.ToString("F5");
            }
            else
            {
                controlX.Text = controlY.Text = string.Empty;
            }

        }

        /// <summary>
        /// Center point for Fun Profile setting 
        /// </summary>
        public IPoint FunPropertiesCenterPoint
        {
            set
            {
                SetPointValue(txtBasePointX, txtBasePointY, value);
            }
        }

        public int FunLinesCount
        {
            get
            {
                int result;
                if (int.TryParse(funLinesCount.Text, out result))
                {
                    return result;
                }
                return 0;
            }
        }

        public double FunAzimuth1
        {
            get
            {
                double result;
                if (Helper.TryParceToDouble(azimuth1.Text, out result))
                {
                    return result;
                }
                return -1;
            }
        }
        public double FunAzimuth2
        {
            get
            {
                double result;
                if (Helper.TryParceToDouble(azimuth2.Text, out result))
                {
                    return result;
                }
                return -1;
            }
        }

        public int SectionHeightFirst => TryParseHeight(txtFirstHeight);
        public int SectionHeightSecond => TryParseHeight(txtSecondHeight);
        public int FanHeight => TryParseHeight(txtObserverHeight);
        public int SelectionHeight => TryParseHeight(observerHeightSelection);

        private int TryParseHeight(TextBox heightTextBox)
        {
            if (int.TryParse(heightTextBox.Text, out var result))
            {
                return result;
            }

            return -1;
        }

        double IMilSpaceProfileView.FunLength
        {
            get
            {
                double result;
                if (Helper.TryParceToDouble(profileLength.Text, out result))
                {
                    return result;
                }
                return -1;
            }
        }

        double IMilSpaceProfileView.ObserveHeight
        {
            get
            {
                if (SelectedProfileSettingsType == ProfileSettingsTypeEnum.Fun)
                {
                    return FanHeight;
                }
                else if (SelectedProfileSettingsType == ProfileSettingsTypeEnum.Points)
                {
                    return SectionHeightFirst;
                }
                else if (SelectedProfileSettingsType == ProfileSettingsTypeEnum.Primitives)
                {
                    return SelectionHeight;
                }

                throw new NotImplementedException();
            }
        }

        private void panel1_Enter(object sender, EventArgs e)
        {
            ProfileLayers.GetAllLayers();
        }


        private void calcProfile_Click(object sender, EventArgs e)
        {
            var session = controller.GenerateProfile();
            if (session != null)
            {
                controller.AddProfileToList(session);
                controller.CallGraphsHandle(session);
            }
            else
            {
                //TODO log error
                MessageBox.Show("Calculation error", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ChechDouble_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !CheckDouble(e.KeyChar, (TextBox)sender);
        }

        private void funLinesCount_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !CheckDouble(e.KeyChar, (TextBox)sender, true);
        }

        private void UpdateFunProperties(object sender, EventArgs e)
        {
            controller.SetProfileSettings(ProfileSettingsTypeEnum.Fun);
        }

        private void profileSettingsTab_SelectedIndexChanged(object sender, EventArgs e)
        {
            controller.SetProfileSettings(SelectedProfileSettingsType);

        }

        private void cmbRasterLayers_SelectedIndexChanged(object sender, EventArgs e)
        {
            controller.SetProfileSettings(SelectedProfileSettingsType);
        }

        private static bool CheckDouble(char charValue, TextBox textValue, bool justInt = false)
        {

            return (((charValue == BACKSPACE) || ((charValue >= ZERO) && (charValue <= NINE))) || (justInt ||

                  ((charValue == DECIMAL_POINT) && textValue.Text.IndexOf(".") == NOT_FOUND)));
        }

        public ProfileSettingsTypeEnum GetProfileTypeFromNode()
        {
            var treeNode = profilesTreeView.SelectedNode;
            while (treeNode.Parent != null)
            {
                treeNode = treeNode.Parent;
            }

            ProfileSettingsTypeEnum res;

            if (Enum.TryParse<ProfileSettingsTypeEnum>(treeNode.Name, out res))
            {
                return res;
            }

            throw new NotImplementedException(treeNode.Name);
        }

        public TreeView GetTreeView()
        {
            return profilesTreeView;
        }

        public int GetProfileNameFromNode()
        {
            var treeNode = profilesTreeView.SelectedNode;

            while (treeNode.Level > 1)
                treeNode = treeNode.Parent;
            return (int)treeNode.Tag;
        }

        private void toolBtnShowOnMap_Click(object sender, EventArgs e)
        {
            var node = profilesTreeView.SelectedNode;
            var ids = GetProfileAndLineIds(node);

            Controller.ShowProfileOnMap();//ShowProfileOnMap(ids.Item1, ids.Item2);
        }

        private void profilesTreeView_AfterCheck(object sender, TreeViewEventArgs e)
        {

            if (e.Node.Tag != null && e.Node.Tag.GetType() == typeof(int))
            {
                var ids = GetProfileAndLineIds(e.Node);
                int id = ids.Item1;
                int lineId = ids.Item2;

                if (lineId > 0)//The node is line 
                {
                    if (e.Node.Checked)
                    {
                        controller.ShowUserSessionProfile(id, lineId);
                    }
                    else
                    {
                        controller.HideUserSessionProfile(id, lineId);
                    }
                }
            }

            FixChildNodes(e.Node);

        }


        private void FixChildNodes(TreeNode parentNode)
        {
            for (int i = 0; i < parentNode.Nodes.Count; i++)
            {
                var node = parentNode.Nodes[i];

                if (node.Checked != parentNode.Checked)
                {
                    node.Checked = parentNode.Checked;
                }

                if (node.Nodes != null && node.Nodes.Count > 0)
                {
                    FixChildNodes(node);
                }
            }
        }

        private Tuple<int, int> GetProfileAndLineIds(TreeNode node)
        {

            int id = -1;
            int lineId = -1;

            if (node != null && node.Tag != null && node.Tag.GetType() == typeof(int))
            {
                id = (int)node.Tag;

                bool isProfileNode = node.Parent != null && node.Nodes != null && node.Nodes.Count > 0;
                if (!isProfileNode)//The node is line 
                {
                    lineId = id;
                    id = (int)node.Parent.Tag;
                }
            }

            return new Tuple<int, int>(id, lineId);
        }

        private void AzimuthCheck(object sender, EventArgs e)
        {
            var athimuthControl = sender as TextBox;

            double result;
            if (Helper.TryParceToDouble(azimuth2.Text, out result) && (result <= 360 && result >= 0))
            {
                UpdateFunProperties(sender, e);
                return;
            }

            //TODO - Localize message
            MessageBox.Show("Значення повинно бути быльше за 0 та меньше 360", "MilSpace", MessageBoxButtons.OK, MessageBoxIcon.Information);

            athimuthControl.Focus();
        }

        private void azimuth2_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnRefreshLayers_Click(object sender, EventArgs e)
        {
            OnDocumentOpenFillDropdowns();
        }

        private void toolBtnFlash_Click(object sender, EventArgs e)
        {
            var node = profilesTreeView.SelectedNode;
            var ids = GetProfileAndLineIds(node);

            Controller.HighlightProfileOnMap(ids.Item1, ids.Item2);
        }

        private void LocalizeStrings()
        {
            //TODO: Set all localization srting here


            //MilSpaceResourceManager mngr = new MilSpaceResourceManager("MilSpace.Profile.Calc");

            //var testLocal = mngr.GetTesxtLocalisation();

            ToolTip ToolTip1 = new System.Windows.Forms.ToolTip();
            ToolTip1.SetToolTip(this.btnRefreshLayers, "Refresh interesing layers");
            lblSelectedPrimitives.Text = "Вибрані об'єкти:";
            lblCommonLength.Text = "Довжина вибраних об'єктів:";
        }


        private void addProfileToGraph_Click(object sender, EventArgs e)
        {
            var node = profilesTreeView.SelectedNode;
            var ids = GetProfileAndLineIds(node);
            //TODO set observer height
            controller.CallGraphsHandle(ids.Item1);
        }

        private void openGraphWindow_Click(object sender, EventArgs e)
        {
            controller.ShowGraphsWindow();
        }

        private void removeProfile_Click(object sender, EventArgs e)
        {
            //TODO: Localize text

            string loalizedtext = $"Do you realy want to remove \'{profilesTreeView.SelectedNode.Text}\"?";
            if (MessageBox.Show(loalizedtext, "MilSpace", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (!controller.RemoveProfilesFromUserSession())
                {
                    MessageBox.Show("There was an error. Look at the log file for more detail", "MilSpace", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }


        private ProfileSession GetProfileFromList(IEnumerable<ProfileSession> listOfProfiles, string profileName)
        {
            var resultProfile = listOfProfiles.FirstOrDefault(profile => profile.SessionName.Equals(profileName));
            return resultProfile;
        }

        public bool AddNodeToTreeView(ProfileSession profile)
        {

            int imageIndex = nodeDefinition[profile.DefinitionType][0];
            int selectedImageIndex = nodeDefinition[profile.DefinitionType][1];

            var parentNode = profilesTreeView.Nodes.Find(profile.DefinitionType.ToString(), false).FirstOrDefault();
            if (parentNode != null)
            {
                var date = DateTime.Now;
                var newNode = new ProfileTreeNode(profile.SessionName, imageIndex, selectedImageIndex)
                {
                    Checked = parentNode.Checked,
                    Tag = profile.SessionId,
                    IsProfileNode = true
                };

                newNode.SetProfileName(profile.SessionName);
                newNode.SetProfileType(ConvertProfileTypeToString(profile.DefinitionType));
                if (profile.DefinitionType == ProfileSettingsTypeEnum.Points)
                {
                    var firstX = profile.ProfileLines.First().Line.FromPoint.X.ToString("F5");
                    var firstY = profile.ProfileLines.First().Line.FromPoint.Y.ToString("F5");
                    var secondX = profile.ProfileLines.First().Line.ToPoint.X.ToString("F5");
                    var secondY = profile.ProfileLines.First().Line.ToPoint.Y.ToString("F5");

                    newNode.SetBasePoint($"X= {firstX}; Y= {firstY};");
                    newNode.SetToPoint($"X= {secondX}; Y= {secondY};");
                    newNode.SetBasePointHeight(SectionHeightFirst.ToString());
                    newNode.SetToPointHeight(SectionHeightSecond.ToString());
                }

                if (profile.DefinitionType == ProfileSettingsTypeEnum.Fun)
                {
                    var basePointX = profile.ProfileLines.FirstOrDefault().Line.FromPoint.X.ToString("F5");
                    var basePointY = profile.ProfileLines.FirstOrDefault().Line.FromPoint.Y.ToString("F5");
                    var lineDistance = profile.ProfileLines.FirstOrDefault().Line.Length.ToString("F5");
                    var linesCount = profile.ProfileLines.ToList().Count.ToString();
                    var height = FanHeight.ToString();
                    var az1 = FunAzimuth1.ToString("F0");
                    var az2 = FunAzimuth2.ToString("F0");


                    newNode.SetBasePoint($"X= {basePointX}; Y= {basePointY};");
                    newNode.SetLineDistance(lineDistance);
                    newNode.SetLineCount(linesCount);
                    newNode.SetAzimuth1(az1);
                    newNode.SetAzimuth2(az2);
                    newNode.SetBasePointHeight(height);
                }

                newNode.SetCreatorName(Environment.UserName);
                newNode.SetDate($"{date.ToLongDateString()} {date.ToLongTimeString()}");

                //TODO: Localize 
                string lineDefinition = "Профіль";


                foreach (var line in profile.ProfileLines)
                {
                    var azimuth = line.Azimuth.ToString("F0");
                    var nodeName = profile.DefinitionType == ProfileSettingsTypeEnum.Points
                        ? $"{azimuth}{Degree}" :
                        (line.Azimuth == double.MinValue ? $"{lineDefinition} ({System.Array.IndexOf(profile.ProfileLines, line) + 1})" :
                        $"{azimuth}{Degree} ({System.Array.IndexOf(profile.ProfileLines, line) + 1})");
                    var childNode = new ProfileTreeNode(nodeName, 205, 205);
                    newNode.Nodes.Add(childNode);
                    childNode.Tag = line.Id;
                    childNode.Checked = newNode.Checked;
                    childNode.IsProfileNode = false;
                    childNode.SetLineDistance(line.Length.ToString("F5"));
                    childNode.SetBasePoint($"X={line.Line.FromPoint.X:F5}; Y={line.Line.FromPoint.Y:F5}");
                    childNode.SetToPoint($"X={line.Line.ToPoint.X:F5}; Y={line.Line.ToPoint.Y:F5}");

                    childNode.SetAzimuth1($"{azimuth}{Degree}");


                }
                parentNode.Nodes.Add(newNode);
            }

            return parentNode.Checked;
        }

        private static double CalculateProfileDistance(IEnumerable<ProfileSurface> profileSurfaces)
        {
            double result = 0;
            foreach (var surface in profileSurfaces)
            {
                for (var i = 0; i < surface.ProfileSurfacePoints.Length - 1; i++)
                {
                    result += CalculateSectionDistance(surface.ProfileSurfacePoints[i], surface.ProfileSurfacePoints[i + 1]);
                }
            }

            return result;

        }

        private static double CalculateSectionDistance(ProfileSurfacePoint pointFrom, ProfileSurfacePoint pointTo)
        {
            var x = pointTo.Distance - pointFrom.Distance;
            var y = pointTo.Z - pointFrom.Z;

            return Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));
        }

        private string ConvertProfileTypeToString(ProfileSettingsTypeEnum profileType)
        {
            switch (profileType)
            {
                case ProfileSettingsTypeEnum.Points:
                    return "Отрезок";

                case ProfileSettingsTypeEnum.Fun:
                    return "Веер";

                case ProfileSettingsTypeEnum.Primitives:
                    return "Графика";

                case ProfileSettingsTypeEnum.Load:
                    return "Загрузка";

                default:
                    throw new ArgumentOutOfRangeException(nameof(profileType), profileType, null);
            }
        }

        private string MapUnitsText
        {
            //TODO: Localiza
            get
            {
                switch (ActiveView.FocusMap.DistanceUnits)
                {
                    case esriUnits.esriMeters:
                        return "метров";
                    case esriUnits.esriKilometers:
                        return "километров";
                    case esriUnits.esriMiles:
                        return "миль";
                    case esriUnits.esriFeet:
                        return "футов";
                    default:
                        return "метров";
                }
            }
        }

        private void saveProfileAsShared_Click_1(object sender, EventArgs e)
        {
            var node = profilesTreeView.SelectedNode;
            var ids = GetProfileAndLineIds(node);

            treeViewselectedIds.ProfileLineId = ids.Item2;
            treeViewselectedIds.ProfileSessionId = ids.Item1;

            var pr = profileLists.Values.SelectMany(p => p).FirstOrDefault(p => p.SessionId == ids.Item1);

            var res = controller.ShareProfileSession(pr);

            if (!res.HasValue)
            {
                //TODO:Localise
                MessageBox.Show($"You are not allowed to share this profile", "MilSpace", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!res.Value)
            {
                //TODO:Localise
                MessageBox.Show($"There was an error on saving this profile./n For more info look into thr log file", "MilSpace", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void eraseProfile_Click(object sender, EventArgs e)
        {
            //TODO: Localize text

            string loalizedtext = $"Do you realy want to remove profile \'{profilesTreeView.SelectedNode.Text}\" from the Database?";
            if (MessageBox.Show(loalizedtext, "MilSpace", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (!controller.RemoveProfilesFromUserSession(true))
                {
                    MessageBox.Show("There was an error. Look at the log file for more detail", "MilSpace", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }

        }

        private void tableLayoutPanel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void addProfileToExistingGraph_Click(object sender, EventArgs e)
        {
            var node = profilesTreeView.SelectedNode;
            var ids = GetProfileAndLineIds(node);

            controller.AddProfileToTab(ids.Item1, ids.Item2);
        }

        private void clearExtraGraphic_Click(object sender, EventArgs e)
        {
            controller.ClearMapFromOldGraphs();
        }
    }
}
