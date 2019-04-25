using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geometry;
using MilSpace.Core;
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

        private ProfileSettingsPointButtonEnum activeButtton = ProfileSettingsPointButtonEnum.None;

        MilSpaceProfileCalsController controller;

        List<ProfileSession> _sectionProfiles = new List<ProfileSession>();
        List<ProfileSession> _fanProfiles = new List<ProfileSession>();
        List<ProfileSession> _graphicProfiles = new List<ProfileSession>();

        public DockableWindowMilSpaceProfileCalc(MilSpaceProfileCalsController controller)
        {
            this.Instance = this;
            SetController(controller);
            controller.SetView(this);

        }

        public DockableWindowMilSpaceProfileCalc(object hook, MilSpaceProfileCalsController controller)
        {
            InitializeComponent();
            SetController(controller);
            this.Hook = hook;
            this.Instance = this;
            SubscribeForEvents();
            controller.SetView(this);
        }

        private void OnProfileSettingsChanged(ProfileSettingsEventArgs e)
        {
            this.calcProfile.Enabled = e.ProfileSetting.IsReady;
        }

        public IActiveView ActiveView => ArcMap.Document.ActiveView;

        public MilSpaceProfileCalsController Controller => controller;

        public ProfileSettingsPointButtonEnum ActiveButton => activeButtton;

        public string DemLayerName => cmbRasterLayers.SelectedItem == null ? string.Empty : cmbRasterLayers.SelectedItem.ToString();

        public int ProfileId
        {
            set { txtProfileName.Text = value.ToString(); }
        }

        public bool AddSectionProfileNodes(ProfileSession profile)
        {
            return AddNodeToTreeView("sectionsNode", profile, 205, 205);
        }

        public bool AddFanProfileNode(ProfileSession profile)
        {
            return AddNodeToTreeView("fanNode", profile, 208, 208);
        }

        public void AddSectionProfileToList(ProfileSession profile)
        {
            _sectionProfiles.Add(profile);
        }

        public void AddFanProfileToList(ProfileSession profile)
        {
            _fanProfiles.Add(profile);
        }

        public void RemoveSectionProfileFromList(string profileName)
        {
            var profileToRemove = _sectionProfiles
                .FirstOrDefault(profile => profile.SessionName.Equals(profileName));
            if (profileToRemove != null)
            {
                _sectionProfiles.Remove(profileToRemove);
            }
        }

        public ProfileSession GetSectionProfile(string profileName)
        {
            return GetProfileFromList(_sectionProfiles, profileName);
        }

        public ProfileSession GetFanProfile(string profileName)
        {
            return GetProfileFromList(_fanProfiles, profileName);
        }

        private ProfileSession GetProfileFromList(IEnumerable<ProfileSession> listOfProfiles, string profileName)
        {
            var resultProfile = listOfProfiles.FirstOrDefault(profile => profile.SessionName.Equals(profileName));
            return resultProfile;
        }

        private bool AddNodeToTreeView(string parentNodeName, ProfileSession profile, int imageIndex, int selectedImageIndex)
        {
            var parentNode = profilesTreeView.Nodes.Find(parentNodeName, false).FirstOrDefault();
            if (parentNode != null)
            {
                var date = DateTime.Now;
                var newNode = new ProfileTreeNode(profile.SessionName, imageIndex, selectedImageIndex)
                {
                    Checked = parentNode.Checked,
                    Tag = profile.SessionId
                };

                newNode.SetProfileName(profile.SessionName);
                newNode.SetProfileId(profile.SessionId.ToString());
                newNode.SetProfileType(ConvertProfileTypeToString(profile.DefinitionType));
                newNode.SetProfileDistance(CalculateProfileDistance(profile.ProfileSurfaces).ToString("F4"));
                newNode.SetLineCount(profile.ProfileLines.Length.ToString());
                newNode.SetCreatorName(Environment.UserName);
                newNode.SetMapName(ArcMap.Application.Document.Title);
                newNode.SetDate($"{date.ToLongDateString()} {date.ToLongTimeString()}");

                newNode.SetLineDistance(string.Empty);
                newNode.SetBasePoint(string.Empty);
                newNode.SetToPoint(string.Empty);
                newNode.SetAzimuth1(string.Empty);
                newNode.SetAzimuth2(string.Empty);

                foreach (var line in profile.ProfileLines)
                {
                    var childNode = new ProfileTreeNode($"X={line.PointFrom.X:F4}; Y={line.PointTo.Y:F4}; Дистанция={line.Length:F4} {MapUnitsText}", 205, 205);
                    newNode.Nodes.Add(childNode);
                    childNode.Tag = line.Id;
                    childNode.Checked = newNode.Checked;
                    ProfileId = profile.SessionId;

                    childNode.SetProfileName(profile.SessionName);
                    childNode.SetProfileId(profile.SessionId.ToString());
                    childNode.SetProfileType(ConvertProfileTypeToString(profile.DefinitionType));
                    childNode.SetProfileDistance(CalculateProfileDistance(profile.ProfileSurfaces).ToString("F4"));
                    childNode.SetLineDistance(line.Length.ToString("F4"));
                    childNode.SetBasePoint($"X={line.Line.FromPoint.X:F4}; Y={line.Line.FromPoint.Y:F4}");
                    childNode.SetToPoint($"X={line.Line.ToPoint.X:F4}; Y={line.Line.ToPoint.Y:F4}");
                    childNode.SetAzimuth1(FunAzimuth1 == -1 ? string.Empty : FunAzimuth1.ToString("F0"));
                    childNode.SetAzimuth2(FunAzimuth2 == -1 ? string.Empty : FunAzimuth2.ToString("F0"));

                    childNode.SetCreatorName(Environment.UserName);
                    childNode.SetMapName(ArcMap.Application.Document.Title);
                    childNode.SetDate($"{date.ToLongDateString()} {date.ToLongTimeString()}");

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

                case ProfileSettingsTypeEnum.SelectedFeatures:
                    return "Графика";

                case ProfileSettingsTypeEnum.Load:
                    return "Загрузка";

                default:
                    throw new ArgumentOutOfRangeException(nameof(profileType), profileType, null);
            }
        }



        private string MapUnitsText
        {
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

        private void OnRasterComboDropped()
        {
            cmbRasterLayers.Items.Clear();
            PopulateComboBox(cmbRasterLayers, ProfileLayers.RasterLayers);
        }


        private void OnRoadComboDropped()
        {

            cmbRoadLayers.Items.Clear();
            PopulateComboBox(cmbRoadLayers, ProfileLayers.LineLayers);
        }

        private void OnHydrographyDropped()
        {
            cmbHydrographyLayer.Items.Clear();
            PopulateComboBox(cmbHydrographyLayer, ProfileLayers.LineLayers);
        }

        private void OnVegetationDropped()
        {
            cmbPolygonLayer.Items.Clear();
            PopulateComboBox(cmbPolygonLayer, ProfileLayers.PolygonLayers);
        }

        private void OnObservationPointDropped()
        {
            cmbPointLayers.Items.Clear();
            PopulateComboBox(cmbPointLayers, ProfileLayers.PointLayers);
        }

        private void SetZoomToState(object sender, TreeViewEventArgs treeViewEventArgs)
        {
            if (profilesTreeView.SelectedNode.Parent != null)
            {
                toolBtnShowOnMap.Enabled = true;
                toolBtnFlash.Enabled = true;
            }

            else
            {
                toolBtnShowOnMap.Enabled = false;
                toolBtnFlash.Enabled = false;
            }
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

            //((IActiveViewEvents_Event) (ArcMap.Document.FocusMap)).ItemAdded += OnRasterComboDropped;
            ArcMap.Events.OpenDocument += OnRasterComboDropped;
            ArcMap.Events.OpenDocument += OnHydrographyDropped;
            ArcMap.Events.OpenDocument += OnObservationPointDropped;
            ArcMap.Events.OpenDocument += OnRoadComboDropped;
            ArcMap.Events.OpenDocument += OnVegetationDropped;
            profilesTreeView.AfterSelect += SetZoomToState;
            profilesTreeView.AfterSelect += DisplaySelectedNodeAttributes;
            lvProfileAttributes.Resize += OnListViewResize;

            controller.OnProfileSettingsChanged += OnProfileSettingsChanged;

            azimuth1.LostFocus += AzimuthCheck;
            azimuth2.LostFocus += AzimuthCheck;
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
                case 1:

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
                case 0:
                    break;
                case 2:
                    controller.FlashPoint(ProfileSettingsPointButtonEnum.PointsSecond);
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
                controlX.Text = point.X.ToString("F4");
                controlY.Text = point.Y.ToString("F4");
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
                controller.CallGraphsHandle(session, SelectedProfileSettingsType);
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

            switch (treeNode.Name)
            {
                case "sectionsNode":
                    return ProfileSettingsTypeEnum.Points;
                case "fanNode":
                    return ProfileSettingsTypeEnum.Fun;
                default:
                    return ProfileSettingsTypeEnum.SelectedFeatures;
            }
        }

        public string GetProfileNameFromNode()
        {
            var treeNode = profilesTreeView.SelectedNode;

            while (treeNode.Level > 1)
                treeNode = treeNode.Parent;
            return treeNode.Text;
        }

        private void toolBtnShowOnMap_Click(object sender, EventArgs e)
        {
            var node = profilesTreeView.SelectedNode;
            var ids = GetProfileAndLineIds(node);

            Controller.ShowProfileOnMap(ids.Item1, ids.Item2);
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
                        controller.ShowWorkingProfile(id, lineId);
                    }
                    else
                    {
                        controller.HideWorkingProfile(id, lineId);
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

    }
}
