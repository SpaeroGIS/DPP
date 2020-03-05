using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geometry;
using MilSpace.Core;
using MilSpace.Core.ModulesInteraction;
using MilSpace.Core.Tools;
using MilSpace.DataAccess.DataTransfer;
using MilSpace.Profile.DTO;
using MilSpace.Profile.Helpers;
using MilSpace.Profile.Interaction;
using MilSpace.Profile.Localization;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
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

        private static Logger logger = Logger.GetLoggerEx("MilSpace.Profile.DockableWindowMilSpaceProfileCalc");

        private static readonly Dictionary<ProfileSettingsTypeEnum, int[]> nodeDefinition = new Dictionary<ProfileSettingsTypeEnum, int[]>
        {
            {ProfileSettingsTypeEnum.Points , new int[] { 205, 205 } },
            {ProfileSettingsTypeEnum.Fun , new int[] { 206, 208 } },
            {ProfileSettingsTypeEnum.Primitives , new int[] {207, 209 } },
        };

        private ProfileSettingsPointButtonEnum activeButtton = ProfileSettingsPointButtonEnum.None;

        private TreeViewSelectedProfileIds treeViewselectedIds; //= new TreeViewSelectedProfileIds();

        MilSpaceProfileCalcsController controller;

        List<ProfileSession> _fanProfiles = new List<ProfileSession>();
        List<ProfileSession> _graphicProfiles = new List<ProfileSession>();

        Dictionary<ProfileSettingsTypeEnum, List<ProfileSession>> profileLists =
            new Dictionary<ProfileSettingsTypeEnum, List<ProfileSession>>
        {
            {ProfileSettingsTypeEnum.Points, new List<ProfileSession>() }

        };

        public DockableWindowMilSpaceProfileCalc(MilSpaceProfileCalcsController controller)
        {
            logger.InfoEx(">>> DockableWindowMilSpaceProfileCalc(1) START <<<");
            this.Instance = this;
            SetController(controller);
            controller.SetView(this);
            LocalizeStrings();
            logger.InfoEx("> DockableWindowMilSpaceProfileCalc(1) END");
        }

        public DockableWindowMilSpaceProfileCalc(object hook, MilSpaceProfileCalcsController controller)
        {
            logger.InfoEx(">>> DockableWindowMilSpaceProfileCalc(2) START <<<");

            InitializeComponent();
            SetController(controller);
            this.Hook = hook;
            this.Instance = this;
            SubscribeForEvents();
            controller.SetView(this);
            LocalizeStrings();

            fanPickCoord = firstPointToolBar.Buttons[0];
            linePickCoordFirst = secondPointToolbar.Buttons[0];
            linePickCoordSecond = basePointToolbar.Buttons[0];
            logger.InfoEx("> DockableWindowMilSpaceProfileCalc(2) END");
        }

        private void OnProfileSettingsChanged(ProfileSettingsEventArgs e)
        {
            this.calcProfile.Enabled = e.ProfileSetting.IsReady;
        }

        public IActiveView ActiveView => ArcMap.Document.ActiveView;

        public ISpatialReference MapSpatialreverence => ArcMap.Document.FocusMap.SpatialReference;

        public MilSpaceProfileCalcsController Controller => controller;

        public ProfileSettingsPointButtonEnum ActiveButton => activeButtton;

        public IEnumerable<string> GetLayersForLineSelection => controller.GetLayersForLineSelection();

        public string DemLayerName => cmbRasterLayers.SelectedItem == null ? string.Empty : cmbRasterLayers.SelectedItem.ToString();
       
        public AssignmentMethodsEnum PrimitiveAssignmentMethod { get => controller.GetPrimitiveAssignmentMethodByString(layersToSelectLine.SelectedItem.ToString()); }

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
        private object Hook { get; set; }

        protected override void OnLoad(EventArgs e)
        {
            Helper.SetConfiguration();
        }

        private void OnDocumentOpenFillDropdowns()
        {
            logger.InfoEx("> OnDocumentOpenFillDropdowns START");

            cmbRasterLayers.Items.Clear();
            cmbRoadLayers.Items.Clear();
            cmbHydrographyLayer.Items.Clear();
            cmbBuildings.Items.Clear();
            cmbVegetationLayer.Items.Clear();
            //cmbPointLayers.Items.Clear();
            layersToSelectLine.Items.Clear();

            MapLayersManager mlmngr = new MapLayersManager(ActiveView);

            PopulateComboBox(cmbRasterLayers, mlmngr.RasterLayers);
            PopulateComboBox(cmbRoadLayers, mlmngr.PolygonLayers);
            PopulateComboBox(cmbHydrographyLayer, mlmngr.PolygonLayers);
            PopulateComboBox(cmbBuildings, mlmngr.PolygonLayers);
            PopulateComboBox(cmbVegetationLayer, mlmngr.PolygonLayers);
            //PopulateComboBox(cmbPointLayers, mlmngr.PointLayers);

            layersToSelectLine.Items.AddRange(controller.GetPrimitiveAssigmentMethodsString().ToArray());
            layersToSelectLine.SelectedItem = layersToSelectLine.Items[0];

            logger.InfoEx("> OnDocumentOpenFillDropdowns END");
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
                logger.ErrorEx("RemoveTreeViewItem Exception:{0}", ex.Message);
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

            renameProfile.Enabled = removeProfile.Enabled = addProfileToGraph.Enabled = toolPanOnMap.Enabled = toolBtnFlash.Enabled =
                treeViewselectedIds.ProfileSessionId > 0;

            var profileType = GetProfileTypeFromNode();
            setProfileSettingsToCalc.Enabled =
                (addProfileToGraph.Enabled &&
                (profileType == ProfileSettingsTypeEnum.Points || profileType == ProfileSettingsTypeEnum.Fun));

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
                if (string.IsNullOrWhiteSpace(row[AttributeKeys.ValueColumnName].ToString()))
                {
                    continue;
                }

                var lvItem = new ListViewItem(row[AttributeKeys.AttributeColumnName].ToString());
                lvItem.SubItems.Add(row[AttributeKeys.ValueColumnName].ToString());

                lvProfileAttributes.Items.Add(lvItem);
            }

            SetListView();
        }

        private void OnListViewResize(object sender, EventArgs eventArgs)
        {
            lvProfileAttributes.Columns[0].Width = -1;
            lvProfileAttributes.Columns[0].Width += 5;
        }

        private static void PopulateComboBox(ComboBox comboBox, IEnumerable<ILayer> layers)
        {
            comboBox.Items.AddRange(layers.Select(l => l.Name).ToArray());
        }

        private void SubscribeForEvents()
        {
            logger.InfoEx("> SubscribeForEvents (Module Profile) START");

            ArcMap.Events.OpenDocument += OnDocumentOpenFillDropdowns;
            ArcMap.Events.OpenDocument += controller.InitiateUserProfiles;

            profilesTreeView.AfterSelect += ChangeTreeViewToolbarState;
            profilesTreeView.AfterSelect += DisplaySelectedNodeAttributes;
            lvProfileAttributes.Resize += OnListViewResize;

            controller.OnProfileSettingsChanged += OnProfileSettingsChanged;
            controller.OnMapSelectionChanged += Controller_OnMapSelectionChanged;

            azimuth1.LostFocus += AzimuthCheck;
            azimuth2.LostFocus += AzimuthCheck;

            logger.InfoEx("> SubscribeForEvents (Module Profile) END");
        }

        private void Controller_OnMapSelectionChanged(SelectedGraphicsArgs selectedLines)
        {
            //lblSelectedPrimitivesValue.Text = selectedLines.LinesCount.ToString();
            //lblCommonLengthValue.Text = selectedLines.FullLength.ToString("F2");
        }

        public void SetController(MilSpaceProfileCalcsController controller)
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

            public AddinImpl()
            {
                Helper.SetConfiguration();
            }

            protected override IntPtr OnCreateChild()
            {
                MilSpaceProfileCalsController = new MilSpaceProfileCalcsController();
                ModuleInteraction.Instance.RegisterModuleInteraction<IProfileInteraction>(new ProfileInteraction(MilSpaceProfileCalsController));

                m_windowUI = new DockableWindowMilSpaceProfileCalc(this.Hook, MilSpaceProfileCalsController);
                AtivateDocableWindow();
                return m_windowUI.Handle;
            }

            internal static IDockableWindow AtivateDocableWindow()
            {
                UID dockWinID = new UIDClass();
                dockWinID.Value = ThisAddIn.IDs.DockableWindowMilSpaceProfileCalc;
                IDockableWindow dockWindow = ArcMap.DockableWindowManager.GetDockableWindow(dockWinID);
                dockWindow.Caption = LocalizationContext.Instance.FindLocalizedElement("LblProfileDocableWinCaption", "Спостереження. Розрахунок профілю");
                return dockWindow;
            }

            protected override void Dispose(bool disposing)
            {
                if (m_windowUI != null)
                    m_windowUI.Dispose(disposing);

                base.Dispose(disposing);
            }

            internal DockableWindowMilSpaceProfileCalc DockableWindowUI => m_windowUI;


            internal MilSpaceProfileCalcsController MilSpaceProfileCalsController { get; private set; }

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

            switch (ToolbarButtonClicked.Name)
            {
                case "toolBarButton8":
                    HandlePickCoordTool(e.Button);

                    if (ArcMap.Application.CurrentTool == null)
                    {
                        linePickCoordFirst.Pushed = false;
                        var message = LocalizationContext.Instance.FindLocalizedElement("MsgPickCoordinatesToolText", "Будь ласка, увімкніть інструмент для виділення точки");
                        MessageBox.Show(
                            message,
                            "Profile Calc",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Exclamation);
                        break;
                    }
                    activeButtton = ProfileSettingsPointButtonEnum.PointsFist;
                    break;

                case "toolBarButton55":
                    controller.FlashPoint(ProfileSettingsPointButtonEnum.PointsFist);
                    break;

                case "toolBarButton57":
                    //if (txtFirstPointX.Focused) CopyTextToBuffer(txtFirstPointX.Text);
                    Clipboard.Clear();
                    string sCoord = $"{txtFirstPointX.Text} {txtFirstPointY.Text}";

                    if(String.IsNullOrEmpty(sCoord) || String.IsNullOrEmpty(sCoord.Trim()))
                    {
                        break;
                    }

                    Clipboard.SetText(sCoord.Replace(",", "."));

                    //CopyTextToBuffer(txtFirstPointY.Focused ? txtFirstPointY.Text : txtFirstPointX.Text);
                    break;

                case "toolBarButton58":
                    var sclipboard = Clipboard.GetText();
                    if (string.IsNullOrWhiteSpace(sclipboard)) return;

                    if (Regex.IsMatch(sclipboard, @"^([-]?[\d]{1,2}[\,|\.]\d+)[\;| ]([-]?[\d]{1,2}[\,|\.]\d+)$"))
                    {
                        string sCoords = sclipboard.Replace('.', ',');
                        var coords = sCoords.Replace(' ', ';');
                        var point = GetPointFromRowValue(coords);
                        point.SpatialReference = EsriTools.Wgs84Spatialreference;
                        var pointOnMap = new PointClass { X = point.X, Y = point.Y, Z = point.Z, SpatialReference = point.SpatialReference };
                        pointOnMap.Project(ArcMap.Document.ActiveView.FocusMap.SpatialReference);
                        controller.SetFirsPointForLineProfile(point, pointOnMap);
                    }
                    else
                    {
                        //string sMsgText = LocalizationContext.Instance.FindLocalizedElement.GetLocalization(
                        //    "MsgInvalidCoordinatesDD",
                        //    "недійсні дані \nПотрібні коордінати представлені у СК WGS-84, десяткові градуси");
                        string sMsgText = "Недійсні дані. Потрібні коордінати у СК WGS-84, десяткові градуси.";

                        MessageBox.Show(
                            sMsgText + " (Отримано: " + sclipboard + ")",
                            "Profile Calc",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                    break;

                case "tlbbReturnPoint":

                    var startPoint = controller.GetStartPointValue(ProfileSettingsPointButtonEnum.PointsFist);
                    var pointToMapSpatial = startPoint.Clone();
                    pointToMapSpatial.Project(ActiveView.FocusMap.SpatialReference);

                    controller.SetFirsPointForLineProfile(startPoint, pointToMapSpatial);

                    break;

                case "?":
                    txtFirstPointX.Clear();
                    txtFirstPointY.Clear();
                    controller.SetFirsPointForLineProfile(null, null);
                    break;
            }
        }

        private void HandlePickCoordTool(ToolBarButton pressedButton)
        {
            fanPickCoord.Pushed = pressedButton == fanPickCoord;
            linePickCoordFirst.Pushed = pressedButton == linePickCoordFirst;
            linePickCoordSecond.Pushed = pressedButton == linePickCoordSecond;

            UID mapToolID = new UIDClass
            {
                Value = ThisAddIn.IDs.PickProfileCoordinates
            };

            var documentBars = ArcMap.Application.Document.CommandBars;
            var mapTool = documentBars.Find(mapToolID, false, false);
            ArcMap.Application.CurrentTool = mapTool;
        }

        private void secondPointToolbar_ButtonClick(object sender, ToolBarButtonClickEventArgs e)
        {
            ToolbarButtonClicked = e.Button;
            switch (ToolbarButtonClicked.Name)
            //switch (secondPointToolbar.Buttons.IndexOf(e.Button))
            {
                case "toolBarButton61":
                    HandlePickCoordTool(e.Button);
                    if (ArcMap.Application.CurrentTool == null)
                    {
                        linePickCoordFirst.Pushed = false;
                        var message = LocalizationContext.Instance.FindLocalizedElement("MsgPickCoordinatesToolText", "Будь ласка, увімкніть інструмент для виділення точки");
                        MessageBox.Show(
                            message,
                            "Profile Calc",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Exclamation);
                        break;
                    }
                    activeButtton = ProfileSettingsPointButtonEnum.PointsSecond;
                    break;

                case "toolBarButton2":
                    controller.FlashPoint(ProfileSettingsPointButtonEnum.PointsSecond);
                    break;

                case "toolBarButton3":
                    Clipboard.Clear();
                    string sCoord = $"{txtSecondPointX.Text} {txtSecondPointY.Text}";

                    if(String.IsNullOrEmpty(sCoord) || String.IsNullOrEmpty(sCoord.Trim()))
                    {
                        break;
                    }

                    Clipboard.SetText(sCoord.Replace(",", "."));
                    break;

                case "toolBarButton4":
                    var sclipboard = Clipboard.GetText();
                    if (string.IsNullOrWhiteSpace(sclipboard)) return;

                    if (Regex.IsMatch(sclipboard, @"^([-]?[\d]{1,2}[\,|\.]\d+)[\;| ]([-]?[\d]{1,2}[\,|\.]\d+)$"))
                    {
                        string sCoords = sclipboard.Replace('.', ',');
                        var coords = sCoords.Replace(' ', ';');
                        var point = GetPointFromRowValue(coords);
                        point.SpatialReference = EsriTools.Wgs84Spatialreference;
                        var pointOnMap = new PointClass { X = point.X, Y = point.Y, Z = point.Z, SpatialReference = point.SpatialReference };
                        pointOnMap.Project(ArcMap.Document.ActiveView.FocusMap.SpatialReference);
                        controller.SetSecondfPointForLineProfile(point, pointOnMap);
                    }
                    else
                    {
                        //string sMsgText = LocalizationContext.Instance.FindLocalizedElement.GetLocalization(
                        //    "MsgInvalidCoordinatesDD",
                        //    "недійсні дані \nПотрібні коордінати представлені у СК WGS-84, десяткові градуси");
                        string sMsgText = "Недійсні дані. Потрібні коордінати у СК WGS-84, десяткові градуси.";

                        MessageBox.Show(
                            sMsgText + " (Отримано: " + sclipboard + ")",
                            "Profile Calc",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                    break;


                case "tlbbReturnToPoint":

                    var startPoint = controller.GetStartPointValue(ProfileSettingsPointButtonEnum.PointsSecond);
                    var pointToMapSpatial = startPoint.Clone();
                    pointToMapSpatial.Project(ActiveView.FocusMap.SpatialReference);

                    controller.SetSecondfPointForLineProfile(startPoint, pointToMapSpatial);

                    break;

                case "7":
                    txtSecondPointX.Clear();
                    txtSecondPointY.Clear();
                    controller.SetSecondfPointForLineProfile(null, null);
                    break;
            }
        }

        private void toolBar3_ButtonClick(object sender, ToolBarButtonClickEventArgs e)
        {
            ToolbarButtonClicked = e.Button;
            switch (ToolbarButtonClicked.Name)
            //switch (basePointToolbar.Buttons.IndexOf(e.Button))
            {
                case "toolBarButton16":
                    HandlePickCoordTool(e.Button);
                    if (ArcMap.Application.CurrentTool == null)
                    {
                        var message = LocalizationContext.Instance.FindLocalizedElement("MsgPickCoordinatesToolText", "Будь ласка, увімкніть інструмент для виділення точки");// $"Please add Pick Coordinates tool to any toolbar first.";
                        MessageBox.Show(
                            message,
                            "Profile Calc",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Exclamation);
                        break;
                    }
                    activeButtton = ProfileSettingsPointButtonEnum.CenterFun;
                    break;

                case "toolBarButton17":
                    controller.FlashPoint(ProfileSettingsPointButtonEnum.CenterFun);
                    break;

                case "toolBarButton19":
                    Clipboard.Clear();
                    string sCoord = $"{txtBasePointX.Text} {txtBasePointY.Text}";

                    if(String.IsNullOrEmpty(sCoord) || String.IsNullOrEmpty(sCoord.Trim()))
                    {
                        break;
                    }

                    Clipboard.SetText(sCoord.Replace(",", "."));

                    //CopyTextToBuffer(txtBasePointY.Focused ? txtBasePointY.Text : txtBasePointX.Text);
                    break;

                case "toolBarButton20":
                    //if (txtBasePointX.Focused) PasteTextToEditField(txtBasePointX);
                    //PasteTextToEditField(txtBasePointY.Focused ? txtBasePointY : txtBasePointX);

                    var sclipboard = Clipboard.GetText();
                    if (string.IsNullOrWhiteSpace(sclipboard)) return;

                    if (Regex.IsMatch(sclipboard, @"^([-]?[\d]{1,2}[\,|\.]\d+)[\;| ]([-]?[\d]{1,2}[\,|\.]\d+)$"))
                    {
                        string sCoords = sclipboard.Replace('.', ',');
                        var coords = sCoords.Replace(' ', ';');
                        var point = GetPointFromRowValue(coords);
                        point.SpatialReference = EsriTools.Wgs84Spatialreference;
                        var pointOnMap = new PointClass { X = point.X, Y = point.Y, Z = point.Z, SpatialReference = point.SpatialReference };
                        pointOnMap.Project(ArcMap.Document.ActiveView.FocusMap.SpatialReference);
                        controller.SetCenterPointForFunProfile(point, pointOnMap);
                    }
                    else
                    {
                        //string sMsgText = LocalizationContext.Instance.FindLocalizedElement.GetLocalization(
                        //    "MsgInvalidCoordinatesDD",
                        //    "недійсні дані \nПотрібні коордінати представлені у СК WGS-84, десяткові градуси");
                        string sMsgText = "Недійсні дані. Потрібні коордінати у СК WGS-84, десяткові градуси.";

                        MessageBox.Show(
                            sMsgText + " (Отримано: " + sclipboard + ")",
                            "Profile Calc",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                    break;

                case "tlbbReturnCenterPoint":

                    var startPoint = controller.GetStartPointValue(ProfileSettingsPointButtonEnum.CenterFun);
                    var pointToMapSpatial = startPoint.Clone();
                    pointToMapSpatial.Project(ActiveView.FocusMap.SpatialReference);

                    controller.SetCenterPointForFunProfile(startPoint, pointToMapSpatial);


                    break;

                case "6":
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
                MessageBox.Show(
                    "Please make sure X and Y values are valid and try again!"
                    );
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

        public ProfileSettingsTypeEnum SelectedProfileSettingsType =>
            controller.ProfileSettingsType[profileSettingsTab.SelectedIndex];

        public IPoint LinePropertiesFirstPoint
        {
            set
            {
                SetPointValue(txtFirstPointX, txtFirstPointY, txtFirstPointZ, value);

                if(String.IsNullOrEmpty(txtSecondPointX.Text) && String.IsNullOrEmpty(txtSecondPointY.Text))
                {
                    var newPoint = new PointClass { X = value.X, Y = controller.GetDefaultSecondYCoord(value), Z = value.Z, SpatialReference = value.SpatialReference };
                    var pointCopy = new PointClass { X = newPoint.X, Y = newPoint.Y, Z = newPoint.Z, SpatialReference = newPoint.SpatialReference };

                    pointCopy.Project(ArcMap.Document.ActiveView.FocusMap.SpatialReference);

                    controller.SetSecondfPointForLineProfile(newPoint, pointCopy);
                }
            }
        }

        /// <summary>
        /// Second point for Line  Profile setting 
        /// </summary>
        public IPoint LinePropertiesSecondPoint
        {
            set
            {
                SetPointValue(txtSecondPointX, txtSecondPointY, txtSecondPointZ, value);

                if(String.IsNullOrEmpty(txtFirstPointX.Text) && String.IsNullOrEmpty(txtFirstPointY.Text))
                {
                    var newPoint = new PointClass { X = value.X, Y = controller.GetDefaultSecondYCoord(value) };
                    var pointCopy = new PointClass { X = newPoint.X, Y = newPoint.Y, Z = newPoint.Z, SpatialReference = newPoint.SpatialReference };

                    pointCopy.Project(ArcMap.Document.ActiveView.FocusMap.SpatialReference);

                    controller.SetFirsPointForLineProfile(newPoint, pointCopy);
                }
            }
        }


        private static void SetPointValue(TextBox controlX, TextBox controlY, TextBox controlZ, IPoint point)
        {
            logger.DebugEx("> SetPointValue. controlX.Text:{0} controlY.Text:{1}", controlX.Text, controlY.Text);
            try
            {
                if (point != null)
                {
                    controlX.Text = point.X.ToFormattedString();
                    controlY.Text = point.Y.ToFormattedString();
                    controlZ.Text = point.Z.ToFormattedString(1);
                }
                else
                {
                    controlX.Text = controlY.Text = controlZ.Text = string.Empty;
                }
            }
            catch (Exception ex)
            {
                logger.DebugEx("> SetPointValue Exception ex.Message:{0}", ex.Message);
                MessageBox.Show(
                    "Set coordinates value Exception"
                    );
            }

        }

        /// <summary>
        /// Center point for Fun Profile setting 
        /// </summary>
        public IPoint FunPropertiesCenterPoint
        {
            set
            {
                SetPointValue(txtBasePointX, txtBasePointY, txtCenterPointZ, value);
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
        public int FanHeight => TryParseHeight(txtCenterPointHeight);
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
            logger.DebugEx("> calcProfile_Click START");

            var session = controller.GenerateProfile();
            if (session != null)
            {
                logger.DebugEx("calcProfile_Click. session.SessionName:{0}", session.SessionName);
                controller.AddProfileToList(session);
                controller.CallGraphsHandle(session);
                controller.SaveProfileSet(session);
            }
            else
            {
                logger.DebugEx("calcProfile_Click controller.GenerateProfile ERROR. Session is NULL");
                MessageBox.Show(
                    "Calculation error. GenerateProfile return NULL",
                    "Модуль профілю. Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }

            logger.DebugEx("> calcProfile_Click END");
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
            if(sender == funLinesCount && Convert.ToInt32(funLinesCount.Text) < 2)
            {
                funLinesCount.Text = "2";

                MessageBox.Show(LocalizationContext.Instance.FindLocalizedElement("MsgLinesCountLessThanTwoText", "Кількість профілів не може бути меншою за 2"),
                                    LocalizationContext.Instance.MessageBoxTitle);
            }

            RecalculateFunWithParams();
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
            return (((charValue == BACKSPACE) || ((charValue >= ZERO) && (charValue <= NINE)))
                || (justInt || ((charValue == DECIMAL_POINT) && textValue.Text.IndexOf(".") == NOT_FOUND)));
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

        public void ChangeSessionHeightInNode(int sessionId, double height, ProfileSettingsTypeEnum type)
        {
            if (type == ProfileSettingsTypeEnum.Points)
            {
                ChangeSessionHeigth(profilesTreeView.Nodes["Points"].Nodes, sessionId, height);
                return;
            }

            if (type == ProfileSettingsTypeEnum.Fun)
            {
                ChangeSessionHeigth(profilesTreeView.Nodes["Fun"].Nodes, sessionId, height);
                return;
            }

            if (type == ProfileSettingsTypeEnum.Primitives)
            {
                ChangeSessionHeigth(profilesTreeView.Nodes["Primitives"].Nodes, sessionId, height);
                return;
            }
        }

        public void SetProifileLineInfo(double length, double azimuth)
        {
            lblAzimuthInfo.Text = $"{LocalizationContext.Instance.AzimuthInfoText} {Math.Round(azimuth).ToString()}";
            lblLengthInfo.Text = $"{LocalizationContext.Instance.LengthInfoText} {Math.Round(length).ToString()} {LocalizationContext.Instance.DimensionText}";
        }

        public void SetPointInfo(ProfileSettingsPointButtonEnum pointType, string text)
        {
            if(pointType == ProfileSettingsPointButtonEnum.PointsFist)
            {
                lblFirstPointInfo.Text = text;
            }
            else if(pointType == ProfileSettingsPointButtonEnum.PointsSecond)
            {
                lblSecondPointInfo.Text = text;
            }
            else
            {
                lblCenterPointInfo.Text = text;
            }
        }

        public void SetFunToPointsParams(double averageAzimuth, double averageAngle, double averageLength, int count)
        {
            lbFunInfo.Items.Clear();

            lbFunInfo.Items.Add(LocalizationContext.Instance.FindLocalizedElement("LbFunParamsTitleText", "Параметри набору профілів:"));
            lbFunInfo.Items.Add(string.Empty);
            lbFunInfo.Items.Add($"{LocalizationContext.Instance.FindLocalizedElement("LbFunParamsAvgAzimuthText", "Середній азимут:")} {Math.Round(averageAzimuth)}");
            lbFunInfo.Items.Add($"{LocalizationContext.Instance.FindLocalizedElement("LbFunParamsAvgAngleText", "Середній кут між лініями:")} {Math.Round(averageAngle)}");
            lbFunInfo.Items.Add($"{LocalizationContext.Instance.FindLocalizedElement("LbFunParamsAvgLengthText", "Середня довжина проекції лінії:")} {Math.Round(averageLength)}");
            lbFunInfo.Items.Add($"{LocalizationContext.Instance.FindLocalizedElement("LbFunParamsLinesCountText", "Кількість ліній")} {count}");
        }

        public void SetPrimitiveInfo(double length, double azimuth, double projectionLength, int segmentsCount)
        {
            lbGraphicsParam.Items.Clear();

            lbGraphicsParam.Items.Add(LocalizationContext.Instance.FindLocalizedElement("LbPrimitiveParamsTitleText", "Параметру примітиву:"));
            lbGraphicsParam.Items.Add(string.Empty);
            lbGraphicsParam.Items.Add($"{LocalizationContext.Instance.FindLocalizedElement("LbPrimitiveParamsLengthText", "Довжина проекцій ліній:")} {Math.Round(length)}");
            lbGraphicsParam.Items.Add($"{LocalizationContext.Instance.FindLocalizedElement("LbPrimitiveParamsAzimuthText", "Азимут між крайніми точками:")} {Math.Round(azimuth)}");
            lbGraphicsParam.Items.Add($"{LocalizationContext.Instance.FindLocalizedElement("LbPrimitiveParamsProjLengthText", "Відстань між крайніми точками:")} {Math.Round(projectionLength)}");
            lbGraphicsParam.Items.Add($"{LocalizationContext.Instance.FindLocalizedElement("LbPrimitiveParamsVerticesCountText", "Кількість сегментів:")} {segmentsCount}");
        }

        public void SetFunTxtValues(double length, double maxAzimuth, double minAzimuth, int linesCount)
        {
            profileLength.Text = Math.Round(length).ToString();
            azimuth1.Text = Math.Round(minAzimuth).ToString();
            azimuth2.Text = Math.Round(maxAzimuth).ToString();
            funLinesCount.Text = linesCount.ToString();
        }

        public void SetReturnButtonEnable(ProfileSettingsPointButtonEnum pointType, bool enabled)
        {
            if(pointType == ProfileSettingsPointButtonEnum.PointsFist)
            {
                tlbbReturnPoint.Enabled = enabled;
            }
            else if(pointType == ProfileSettingsPointButtonEnum.PointsSecond)
            {
                tlbbReturnToPoint.Enabled = enabled;
            }
            else
            {
                tlbbReturnCenterPoint.Enabled = enabled;
            }
        }

        public void RecalculateFun()
        {
            var creationMethod = controller.GetCreationMethodByString(cmbTargetObjCreation.SelectedItem.ToString());

            if(creationMethod == ToPointsCreationMethodsEnum.AzimuthsLines)
            {
                controller.CalcFunToPoints(controller.GetTargetAssignmentMethodByString(cmbTargetObjAssignmentMethod.SelectedItem.ToString()),
                                        ToPointsCreationMethodsEnum.Default, false);
            }
            else
            {
                controller.CalcFunToPoints(controller.GetTargetAssignmentMethodByString(cmbTargetObjAssignmentMethod.SelectedItem.ToString()),
                                            creationMethod, false);
            }
        }

        public void RecalculateFunWithParams()
        {
            var creationMethod = controller.GetCreationMethodByString(cmbTargetObjCreation.SelectedItem.ToString());

            if(creationMethod == ToPointsCreationMethodsEnum.AzimuthsLines)
            {
                controller.CalcFunToPoints(controller.GetTargetAssignmentMethodByString(cmbTargetObjAssignmentMethod.SelectedItem.ToString()),
                                        ToPointsCreationMethodsEnum.AzimuthsLines, false);
            }
            else
            {
                controller.CalcFunToPoints(controller.GetTargetAssignmentMethodByString(cmbTargetObjAssignmentMethod.SelectedItem.ToString()),
                                            creationMethod, false, Convert.ToDouble(profileLength.Text));
            }
        }

        private void ChangeSessionHeigth(TreeNodeCollection nodes, int id, double height)
        {
            foreach (TreeNode node in nodes)
            {
                if ((int)node.Tag == id)
                {
                    var profileNode = (ProfileTreeNode)node;
                    profileNode.SetBasePointHeight(height.ToString());
                    return;
                }
            }
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
            MessageBox.Show(
                "Значення азимута задається в десяткових градусах і має бути більше або дорівнює 0 або менше або дорівнює 360",
                "Спостереження. Профіль",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

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
            logger.InfoEx("> LocalizeStrings Profile START");

            //TODO: Set all localization srting here

            ToolTip toolTip = new ToolTip();
            toolTip.SetToolTip(this.btnRefreshLayers, LocalizationContext.Instance.FindLocalizedElement("BtnRefreshLayersToolTip", "Оновити шари даних"));
            toolTip.SetToolTip(reverseButton, LocalizationContext.Instance.FindLocalizedElement("BtnReverseToolTip", "Змінити напрямок профілю"));
            toolTip.SetToolTip(reverseSecondPointButton, LocalizationContext.Instance.FindLocalizedElement("BtnReverseToolTip", "Змінити напрямок профілю"));
            toolTip.SetToolTip(btnPanToFun, LocalizationContext.Instance.FindLocalizedElement("BtnPanToFunToolTip", "Наблизити до набору профілів"));
            toolTip.SetToolTip(btnPanToPrimitive, LocalizationContext.Instance.FindLocalizedElement("BtnPanToPrimitive", "Наблизити до примітиву"));

            firstPointToolBar.Buttons["toolBarButton8"].ToolTipText = LocalizationContext.Instance.FindLocalizedElement("BtnTakeCoordToolTip", "Взяти координати з карти");
            firstPointToolBar.Buttons["toolBarButton55"].ToolTipText = LocalizationContext.Instance.FindLocalizedElement("BtnShowCoordToolTip", "Показати координати на карті");
            firstPointToolBar.Buttons["toolBarButton57"].ToolTipText = LocalizationContext.Instance.FindLocalizedElement("BtnCopyCoordToolTip", "Копіювати координати");
            firstPointToolBar.Buttons["toolBarButton58"].ToolTipText = LocalizationContext.Instance.FindLocalizedElement("BtnPasteCoordToolTip", "Вставити координати");
            firstPointToolBar.Buttons["tlbbReturnPoint"].ToolTipText = LocalizationContext.Instance.ReturnPointValueText;

            secondPointToolbar.Buttons["toolBarButton61"].ToolTipText = LocalizationContext.Instance.FindLocalizedElement("BtnTakeCoordToolTip", "Взяти координати з карти");
            secondPointToolbar.Buttons["toolBarButton2"].ToolTipText = LocalizationContext.Instance.FindLocalizedElement("BtnShowCoordToolTip", "Показати координати на карті");
            secondPointToolbar.Buttons["toolBarButton3"].ToolTipText = LocalizationContext.Instance.FindLocalizedElement("BtnCopyCoordToolTip", "Копіювати координати");
            secondPointToolbar.Buttons["toolBarButton4"].ToolTipText = LocalizationContext.Instance.FindLocalizedElement("BtnPasteCoordToolTip", "Вставити координати");
            secondPointToolbar.Buttons["tlbbReturnToPoint"].ToolTipText = LocalizationContext.Instance.ReturnPointValueText;

            basePointToolbar.Buttons["toolBarButton16"].ToolTipText = LocalizationContext.Instance.FindLocalizedElement("BtnTakeCoordToolTip", "Взяти координати з карти");
            basePointToolbar.Buttons["toolBarButton17"].ToolTipText =  LocalizationContext.Instance.FindLocalizedElement("BtnShowCoordToolTip", "Показати координати на карті");
            basePointToolbar.Buttons["toolBarButton19"].ToolTipText =  LocalizationContext.Instance.FindLocalizedElement("BtnCopyCoordToolTip", "Копіювати координати");
            basePointToolbar.Buttons["toolBarButton20"].ToolTipText = LocalizationContext.Instance.FindLocalizedElement("BtnPasteCoordToolTip", "Вставити координати");
            basePointToolbar.Buttons["tlbbReturnCenterPoint"].ToolTipText = LocalizationContext.Instance.ReturnPointValueText;

            addAvailableProfilesSets.ToolTipText = LocalizationContext.Instance.FindLocalizedElement("BtnAddAvailableProfilesSetsToolTip", "Додати доступні набори профілів");
            //lblSelectedPrimitives.Text = LocalizationContext.Instance.FindLocalizedElement("LblSelectedPrimitivesText", "Вибрані об'єкти:");
            //lblCommonLength.Text = LocalizationContext.Instance.FindLocalizedElement("LblCommonLengthText", "Довжина вибраних об'єктів:");

            //Profile Tabs
            profileTabPage.Text = LocalizationContext.Instance.FindLocalizedElement("TabProfileTabPageText", "Параметри профілю");
            profileTreeTabPage.Text = LocalizationContext.Instance.FindLocalizedElement("TabPofileTreeTabPageText", "Профілі в роботі");

            sectionTab.Text = LocalizationContext.Instance.FindLocalizedElement("TabSectionTabText", "Відрізки");
            //loadTab.Text = LocalizationContext.Instance.FindLocalizedElement.LoadTabText;
            primitiveTab.Text = LocalizationContext.Instance.FindLocalizedElement("TabPrimitiveTabText", "Графіка");
            funTab.Text = LocalizationContext.Instance.FindLocalizedElement("TabFunTabText", "\"Віяло\"");


            //Labels
            lblDEM.Text = LocalizationContext.Instance.FindLocalizedElement("LblDEMText", "ЦМР/ЦММ");

            lblLayersForCalc.Text = LocalizationContext.Instance.FindLocalizedElement("LblLayersForCalcText", "Шари для розрахунків");
            lblVegetationLayer.Text = LocalizationContext.Instance.FindLocalizedElement("LblVegetationLayerText", "рослинність");
            lblBuildingsLayer.Text = LocalizationContext.Instance.FindLocalizedElement("LblBuildingsLayerText", "забудова");
            lblRoadsLayer.Text = LocalizationContext.Instance.FindLocalizedElement("LblRoadsLayerText", "транспорт");
            lblHydrographyLayer.Text = LocalizationContext.Instance.FindLocalizedElement("LblHydrographyLayerText", "гідрографія");
           // lblPointOfViewLayer.Text = LocalizationContext.Instance.FindLocalizedElement("LblPointOfViewLayerText", "Шар точок спостереження");
            lblSetPeofileProperties.Text = LocalizationContext.Instance.FindLocalizedElement("LblSetProfilePropertiesText", "Задати профіль");
            lblProfileName.Text = LocalizationContext.Instance.FindLocalizedElement("LblProfileNameText", "Ім'я профілю");

            calcProfile.Text = LocalizationContext.Instance.FindLocalizedElement("BtnСalcProfileText", "Розрахувати");
            lblLineFirstPoint.Text = LocalizationContext.Instance.FindLocalizedElement("LblLineFirstPointText", "Перша точка (довгота / широта)");
            lblLineSecondPoint.Text = LocalizationContext.Instance.FindLocalizedElement("LblLineSecondPointText", "Друга точка (довгота / широта)");

            lblHeightOfViewFirst.Text = LocalizationContext.Instance.FindLocalizedElement("LblHeightOfViewText", "Висота");
            lblHeightOfViewSecond.Text = LocalizationContext.Instance.FindLocalizedElement("LblHeightOfViewText", "Висота");
            lblDimensionSecond.Text = LocalizationContext.Instance.FindLocalizedElement("LblDimensionSecondText", "м");
            lblDimensionFirst.Text = LocalizationContext.Instance.FindLocalizedElement("LblDimensionFirstText", "м");
            lblDimentionCenter.Text = LocalizationContext.Instance.DimensionText;
            lblFunBasePoint.Text = LocalizationContext.Instance.FindLocalizedElement("LblFunBasePointText", "Базова точка");
            lblCenterPointHeight.Text = LocalizationContext.Instance.FindLocalizedElement("LblHeightOfViewText", "Висота");
            lblFunParameters.Text = LocalizationContext.Instance.FindLocalizedElement("LblFunParametersText", "Параметри");
            lblFunDistance.Text = LocalizationContext.Instance.FindLocalizedElement("LblFunDistanceText", "Відстань");
            lblFunCount.Text = LocalizationContext.Instance.FindLocalizedElement("LblFunCountText", "Кількість");

            lblFunAzimuth1.Text = LocalizationContext.Instance.FindLocalizedElement("LblFunAzimuth1Text", "Азимут 1");
            lblFunAzimuth2.Text = LocalizationContext.Instance.FindLocalizedElement("LblFunAzimuth2Text", "Азимут 2");

            lblHeightOfViewGraphics.Text = LocalizationContext.Instance.FindLocalizedElement("LblHeightOfViewText", "Висота точки спостереження");
            lblPrimitivesLayerToSelect.Text = LocalizationContext.Instance.FindLocalizedElement("LblPrimitivesLayerToSelectText", "Шари для вибору об'єктів");
            lblAboutSelected.Text = LocalizationContext.Instance.FindLocalizedElement("LblAboutSelectedText", "Інформація про обране");

            lblProfileList.Text = LocalizationContext.Instance.FindLocalizedElement("LblProfileListText", "Профілі в роботі");
            lblFirstlPointGettingWay.Text = LocalizationContext.Instance.AssignmentMethodText;
            lblSecondPointGettingWay.Text = LocalizationContext.Instance.AssignmentMethodText;
            lblCenterPointAssignmentMethod.Text = LocalizationContext.Instance.AssignmentMethodText;
            lblTargetObjAssignmentMethod.Text = LocalizationContext.Instance.AssignmentMethodText;
            lblProfileInfo.Text = LocalizationContext.Instance.FindLocalizedElement("LblProfileInfoText", "Параметри відрізку");
            lblLengthInfo.Text = LocalizationContext.Instance.LengthInfoText;
            lblAzimuthInfo.Text = LocalizationContext.Instance.AzimuthInfoText;
            lblTargetObj.Text = LocalizationContext.Instance.FindLocalizedElement("LblTargetObjText", "Цільовий об'єкт");
            lblProfileInfoTitle.Text = LocalizationContext.Instance.FindLocalizedElement("LblProfileInfoTitle", "Параметри набору/профілю");

            lblFirstPointInfo.Text = string.Empty;
            lblSecondPointInfo.Text = string.Empty;
            lblCenterPointInfo.Text = string.Empty;
            lblTargetObjInfo.Text = string.Empty;


            toolPanOnMap.ToolTipText = LocalizationContext.Instance.FindLocalizedElement("HintToolBtnPanOnMapText", "Переміститись  на карті");
            toolBtnFlash.ToolTipText = LocalizationContext.Instance.FindLocalizedElement("HintToolBtnShowOnMapText", "Показати на карті");
            setProfileSettingsToCalc.ToolTipText = LocalizationContext.Instance.FindLocalizedElement("HintToolSetProfileSettingsToCalcText", "Скопіювати параметри профілю");
            addProfileToExistingGraph.ToolTipText = LocalizationContext.Instance.FindLocalizedElement("HintAddProfileToExistingGraphText", "Додати профіль на графік");
            addProfileToGraph.ToolTipText = LocalizationContext.Instance.FindLocalizedElement("HintAddProfileToGraphText", "Відкрити графік профілю");
            openGraphWindow.ToolTipText = LocalizationContext.Instance.FindLocalizedElement("HintOpenGraphWindowText", "Відкрити вікно графіків");
            removeProfile.ToolTipText = LocalizationContext.Instance.FindLocalizedElement("HintRemoveProfileText", "Cкинути профіль з сесії");
            saveProfileAsShared.ToolTipText = LocalizationContext.Instance.FindLocalizedElement("HintSaveProfileAsSharedText", "Надати спільний доступ до профілю");
            eraseProfile.ToolTipText = LocalizationContext.Instance.FindLocalizedElement("HintEraseProfileText", "Видалити профіль");
            clearExtraGraphic.ToolTipText = LocalizationContext.Instance.FindLocalizedElement("HintClearExtraGraphicText", "Очистити графіку на карті");

            btnChooseFirstPointAssignmentMethod.Text = LocalizationContext.Instance.ChooseText;
            btnChooseSecondPointAssignmentMethod.Text = LocalizationContext.Instance.ChooseText;
            btnCenterPointAssignmantMethod.Text = LocalizationContext.Instance.ChooseText;
            btnTargetObjAssignmentMethod.Text = LocalizationContext.Instance.ChooseText;
            btnChooseCreationMethod.Text = LocalizationContext.Instance.ChooseText;
            btnPrimitiveAssignmentMethod.Text = LocalizationContext.Instance.ChooseText;

            profilesTreeView.Nodes["Points"].Text = LocalizationContext.Instance.FindLocalizedElement("TvProfilesPointsNodeText", "Відрізки");
            profilesTreeView.Nodes["Fun"].Text = LocalizationContext.Instance.FindLocalizedElement("TvProfilesFunNodeText", "\"Віяло\"");
            profilesTreeView.Nodes["Primitives"].Text = LocalizationContext.Instance.FindLocalizedElement("TvProfilesPritiveNodeText", "Графіка");

            var mapItem = LocalizationContext.Instance.AssignmentMethodFromMapItem;

            cmbFirstPointAssignmentMethod.Items.AddRange(controller.GetAssignmentMethodsStrings());
            cmbFirstPointAssignmentMethod.SelectedItem = mapItem;

            cmbSecondPointAssignmentMethod.Items.AddRange(controller.GetAssignmentMethodsStrings());
            cmbSecondPointAssignmentMethod.SelectedItem = mapItem;

            cmbCenterPointAssignmentMethod.Items.AddRange(controller.GetAssignmentMethodsStrings());
            cmbCenterPointAssignmentMethod.SelectedItem = mapItem;

            cmbTargetObjAssignmentMethod.Items.AddRange(controller.GetTargetAssignmentMethodsStrings());
            cmbTargetObjAssignmentMethod.SelectedItem = LocalizationContext.Instance.TargetAssignmentMethodInSector;

            cmbTargetObjCreation.Items.AddRange(controller.GetToPointsCreationMethodsString());
            cmbTargetObjCreation.SelectedItem = LocalizationContext.Instance.ToPointsCreationMethodAzimuthsLines;
            cmbTargetObjCreation.Enabled = false;

            logger.InfoEx("> LocalizeStrings Profile END");
        }

        private void addProfileToGraph_Click(object sender, EventArgs e)
        {
            var node = profilesTreeView.SelectedNode;
            var ids = GetProfileAndLineIds(node);
            logger.DebugEx("CallGraphsHandle");
            //TODO set observer height
            controller.CallGraphsHandle(ids.Item1);
        }

        private void openGraphWindow_Click(object sender, EventArgs e)
        {
            controller.ShowGraphsWindow();
        }

        private void removeProfile_Click(object sender, EventArgs e)
        {
            string loalizedtext =
               LocalizationContext.Instance.FindLocalizedElement("MsgRemoveProfaileText", $"Ви дійсно бажаєти cкинути профіль  \"{0}\" з сесії?")
               .InvariantFormat(profilesTreeView.SelectedNode.Text);
            if (MessageBox.Show(
                loalizedtext, 
                LocalizationContext.Instance.MessageBoxTitle, 
                MessageBoxButtons.YesNo, 
                MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (!controller.RemoveProfilesFromUserSession())
                {
                    MessageBox.Show(
                        "There was an error. Look at the log file for more detail", 
                        LocalizationContext.Instance.MessageBoxTitle, 
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
                if(profile.DefinitionType == ProfileSettingsTypeEnum.Points)
                {
                    var fromPoint = profile.ProfileSurfaces.First().ProfileSurfacePoints.First();
                    var toPoint = profile.ProfileSurfaces.First().ProfileSurfacePoints.Last();

                    var firstX = fromPoint.X.ToFormattedString();
                    var firstY = fromPoint.Y.ToFormattedString();
                    var secondX = toPoint.X.ToFormattedString();
                    var secondY = toPoint.Y.ToFormattedString();
                    var lineDistance = profile.ProfileLines.First().Line.Length.ToString("F0");

                    newNode.SetBasePointX(firstX);
                    newNode.SetBasePointY(firstY);
                    newNode.SetBasePointHeight(profile.ObserverHeight.ToString());
                    newNode.SetBasePointFromSurfaceHeight(fromPoint.Z.ToFormattedString(1));
                    newNode.SetToPointX(secondX);
                    newNode.SetToPointY(secondY);
                    newNode.SetToPointHeight(SectionHeightSecond.ToString());
                    newNode.SetToPointFromSurfaceHeight(toPoint.Z.ToFormattedString(1));
                    newNode.SetLineDistance(lineDistance);
                    newNode.SetAzimuth(profile.ProfileLines.First().Azimuth.ToString("F0"));
                    newNode.SetSurface(profile.SurfaceLayerName);
                }
                else if(profile.DefinitionType == ProfileSettingsTypeEnum.Fun)
                {
                    var fromPoint = profile.ProfileLines.First().Line.FromPoint.CloneWithProjecting();

                    var basePointX = fromPoint.X.ToFormattedString();
                    var basePointY = fromPoint.Y.ToFormattedString();
                    var lineDistance = profile.ProfileLines.First().Line.Length.ToString("F0");
                    var linesCount = profile.ProfileLines.Length.ToString();

                    newNode.SetBasePointX(basePointX);
                    newNode.SetBasePointY(basePointY);
                    newNode.SetBasePointHeight(profile.ObserverHeight.ToString());
                    newNode.SetBasePointFromSurfaceHeight(profile.ProfileSurfaces.First().ProfileSurfacePoints.First().Z.ToFormattedString(1));
                    newNode.SetLineDistance(lineDistance);
                    newNode.SetAzimuth1(profile.Azimuth1);
                    newNode.SetAzimuth2(profile.Azimuth2);
                    newNode.SetLineCount(linesCount);
                    newNode.SetSurface(profile.SurfaceLayerName);
                }
                else if(profile.DefinitionType == ProfileSettingsTypeEnum.Primitives)
                {
                    var fromPoint = profile.ProfileSurfaces.First().ProfileSurfacePoints.First();
                    var toPoint = profile.ProfileSurfaces.Last().ProfileSurfacePoints.Last();

                    var firstX = fromPoint.X.ToFormattedString();
                    var firstY = fromPoint.Y.ToFormattedString();
                    var secondX = toPoint.X.ToFormattedString();
                    var secondY = toPoint.Y.ToFormattedString();
                    var linesCount = profile.ProfileLines.First().Vertices.Count().ToString("F0");

                    newNode.SetBasePointX(firstX);
                    newNode.SetBasePointY(firstY);
                    newNode.SetBasePointHeight(profile.ObserverHeight.ToString());
                    newNode.SetBasePointFromSurfaceHeight(fromPoint.Z.ToFormattedString(1));
                    newNode.SetToPointX(secondX);
                    newNode.SetToPointY(secondY);
                    newNode.SetToPointHeight(SectionHeightSecond.ToString());
                    newNode.SetToPointFromSurfaceHeight(toPoint.Z.ToFormattedString(1));
                    newNode.SetLineCount(linesCount);
                    newNode.SetSurface(profile.SurfaceLayerName);
                }

                newNode.SetCreatorName(profile.CreatedBy);
                newNode.SetDate($"{date.ToLongDateString()} {date.ToLongTimeString()}");

                logger.InfoEx($"Profile  {profile.SessionName} added to the tree");

                string lineDefinition = LocalizationContext.Instance.FindLocalizedElement("TxtTreeViewProfileText", "Профіль:");

                foreach (var line in profile.ProfileLines)
                {
                    var profileSurface = profile.ProfileSurfaces.FirstOrDefault(surface => surface.LineId == line.Id);
                   
                    var fromPoint = (profileSurface == null) ? new ProfileSurfacePoint { X = line.PointFrom.X, Y = line.PointFrom.Y, Z = line.PointFrom.Z } : profileSurface.ProfileSurfacePoints.First();
                    var toPoint = (profileSurface == null) ? new ProfileSurfacePoint { X = line.PointTo.X, Y = line.PointTo.Y, Z = line.PointTo.Z } : profileSurface.ProfileSurfacePoints.Last();
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
                    childNode.SetBasePointX(fromPoint.X.ToFormattedString());
                    childNode.SetBasePointY(fromPoint.Y.ToFormattedString());
                    childNode.SetBasePointHeight(profile.ObserverHeight.ToString());
                    childNode.SetBasePointFromSurfaceHeight(fromPoint.Z.ToFormattedString(1));
                    childNode.SetToPointX(toPoint.X.ToFormattedString());
                    childNode.SetToPointY(toPoint.Y.ToFormattedString());
                    childNode.SetToPointHeight(SectionHeightSecond.ToString());
                    childNode.SetToPointFromSurfaceHeight(toPoint.Z.ToFormattedString(1));
                    childNode.SetLineDistance(line.Line.Length.ToString("F0"));
                    childNode.SetAzimuth($"{azimuth}{Degree}");
                    childNode.SetSurface(profile.SurfaceLayerName);

                    logger.InfoEx($"Line {nodeName} was added to the tree");
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
                    return LocalizationContext.Instance.FindLocalizedElement("TabSectionTabText", "Відрізки");

                case ProfileSettingsTypeEnum.Fun:
                    return LocalizationContext.Instance.FindLocalizedElement("TabFunTabText", "\"Віяло\"");

                case ProfileSettingsTypeEnum.Primitives:
                    return LocalizationContext.Instance.FindLocalizedElement("TabPrimitiveTabText", "Графіка");

                case ProfileSettingsTypeEnum.Load:
                    return LocalizationContext.Instance.FindLocalizedElement("TabLoadTabText", "Завантажити");

                default:
                    throw new ArgumentOutOfRangeException(nameof(profileType), profileType, null);
            }
        }

        private void saveProfileAsShared_Click_1(object sender, EventArgs e)
        {
            var node = profilesTreeView.SelectedNode;
            var ids = GetProfileAndLineIds(node);

            treeViewselectedIds.ProfileLineId = ids.Item2;
            treeViewselectedIds.ProfileSessionId = ids.Item1;

            var pr = controller.GetProfileById(ids.Item1);

            var res = controller.ShareProfileSession(pr);

            if (!res.HasValue)
            {
                MessageBox.Show(
                    LocalizationContext.Instance.FindLocalizedElement("MsgNotAllowedToShareText", "Ви не можете дозволити спільний доступ для цього профілю."), 
                    LocalizationContext.Instance.MessageBoxTitle, 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Error);
                return;
            }

            if (!res.Value)
            {
                MessageBox.Show(
                    LocalizationContext.Instance.ErrorHappendText, 
                    LocalizationContext.Instance.MessageBoxTitle, 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Error);
            }

            ChangeTreeViewToolbarState(null, null);

        }

        private void eraseProfile_Click(object sender, EventArgs e)
        {
            string loalizedtext = 
                LocalizationContext.Instance.FindLocalizedElement("MsgDeleteProfaileText", $"Ви дійсно бажаєти видалити профіль  \"{0}\"?")
                .InvariantFormat(profilesTreeView.SelectedNode.Text);
            if (MessageBox.Show(
                loalizedtext, 
                LocalizationContext.Instance.MessageBoxTitle, 
                MessageBoxButtons.YesNo, 
                MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (!controller.RemoveProfilesFromUserSession(true))
                {
                    MessageBox.Show(
                       LocalizationContext.Instance.ErrorHappendText,
                        LocalizationContext.Instance.MessageBoxTitle, 
                        MessageBoxButtons.OK, 
                        MessageBoxIcon.Exclamation);
                }
            }
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

        private void setProfileSettingsToCalc_Click(object sender, EventArgs e)
        {
            var node = profilesTreeView.SelectedNode;

            if (!(node is ProfileTreeNode)) return;

            ProfileTreeNode profileNode = (ProfileTreeNode)node;
            var profileType = GetProfileTypeFromNode();
            var rows = profileNode.Attributes.Rows;


            if (profileType == ProfileSettingsTypeEnum.Points)
            {
                profileSettingsTab.SelectTab(0);

                var baseValueX = rows.Find(AttributeKeys.BasePointX)[AttributeKeys.ValueColumnName].ToString();
                var baseValueY = rows.Find(AttributeKeys.BasePointX)[AttributeKeys.ValueColumnName].ToString();

                var basePoint = GetPointFromRowValue(baseValueX, baseValueY);
                controller.SetFirsPointForLineProfile(basePoint.CloneWithProjecting(), basePoint);

                var toValueX = rows.Find(AttributeKeys.ToPointX)[AttributeKeys.ValueColumnName].ToString();
                var toValueY = rows.Find(AttributeKeys.ToPointY)[AttributeKeys.ValueColumnName].ToString();

                var toPoint = GetPointFromRowValue(toValueX, toValueY);
                controller.SetSecondfPointForLineProfile(toPoint.CloneWithProjecting(), toPoint);

                txtFirstHeight.Text = rows.Find(AttributeKeys.SectionFirstPointHeight)[AttributeKeys.ValueColumnName].ToString();
                txtSecondHeight.Text = rows.Find(AttributeKeys.SectionSecondPointHeight)[AttributeKeys.ValueColumnName].ToString();
            }
            if (profileType == ProfileSettingsTypeEnum.Fun)
            {
                profileSettingsTab.SelectTab(1);

                var baseValueX = rows.Find(AttributeKeys.BasePointX)[AttributeKeys.ValueColumnName].ToString();
                var baseValueY = rows.Find(AttributeKeys.BasePointX)[AttributeKeys.ValueColumnName].ToString();

                var basePoint = GetPointFromRowValue(baseValueX, baseValueY);
                controller.SetFirsPointForLineProfile(basePoint.CloneWithProjecting(), basePoint);

                txtCenterPointHeight.Text = rows.Find(AttributeKeys.SectionFirstPointHeight)[AttributeKeys.ValueColumnName].ToString();

                var length = rows.Find(AttributeKeys.LineDistance)[AttributeKeys.ValueColumnName].ToString();
                profileLength.Text = length.Split(',')[0];

                funLinesCount.Text = rows.Find(AttributeKeys.LinesCount)[AttributeKeys.ValueColumnName].ToString();
                azimuth1.Text = rows.Find(AttributeKeys.Azimuth1)[AttributeKeys.ValueColumnName].ToString();
                azimuth2.Text = rows.Find(AttributeKeys.Azimuth2)[AttributeKeys.ValueColumnName].ToString();
            }

            controller.SetProfileSettings(profileType);
        }

        private IPoint GetPointFromRowValue(string xValue, string yValue)
        {
            //var points = rowValue.Split(';');

            var pointX = Convert.ToDouble(Regex.Match(xValue, @"\d+,?\d+").Value);
            var pointY = Convert.ToDouble(Regex.Match(yValue, @"\d+,?\d+").Value);

          //  var av = ArcMap.Document.ActivatedView;

            return new Point()
            {
                X = pointX,
                Y = pointY,
                SpatialReference = EsriTools.Wgs84Spatialreference
            };
        }

        private IPoint GetPointFromRowValue(string rowValue)
        {
            var points = rowValue.Split(';');

            var pointX = Convert.ToDouble(Regex.Match(points[0], @"\d+,?\d+").Value);
            var pointY = Convert.ToDouble(Regex.Match(points[1], @"\d+,?\d+").Value);

            var av = ArcMap.Document.ActivatedView;

            return new Point()
            {
                X = pointX,
                Y = pointY,
                SpatialReference = av.FocusMap.SpatialReference
            };
        }

        private void addAvailableProfilesSets_Click(object sender, EventArgs e)
        {
            controller.AddAvailableSets();
        }

        private void toolBarSelectedPrimitives_ButtonClick(object sender, ToolBarButtonClickEventArgs e)
        {

        }

        private void CmbFirstPointAssignmentMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(cmbFirstPointAssignmentMethod.SelectedItem.Equals(LocalizationContext.Instance.AssignmentMethodFromMapItem))
            {
                btnChooseFirstPointAssignmentMethod.Enabled = false;
               // SetPointInfo(ProfileSettingsPointButtonEnum.PointsFist, string.Empty);
                SetReturnButtonEnable(ProfileSettingsPointButtonEnum.PointsFist, false);
            }
            else
            {
                btnChooseFirstPointAssignmentMethod.Enabled = true;
            }
        }

        private void CmbSecondPointAssignmentMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(cmbSecondPointAssignmentMethod.SelectedItem.Equals(LocalizationContext.Instance.AssignmentMethodFromMapItem))
            {
                btnChooseSecondPointAssignmentMethod.Enabled = false;
                //SetPointInfo(ProfileSettingsPointButtonEnum.PointsSecond, string.Empty);
                SetReturnButtonEnable(ProfileSettingsPointButtonEnum.PointsSecond, false);
            }
            else
            {
                btnChooseSecondPointAssignmentMethod.Enabled = true;
            }
        }

        private void CmbCenterPointAssignmentMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(cmbCenterPointAssignmentMethod.SelectedItem.Equals(LocalizationContext.Instance.AssignmentMethodFromMapItem))
            {
                btnCenterPointAssignmantMethod.Enabled = false;
               // SetPointInfo(ProfileSettingsPointButtonEnum.CenterFun, string.Empty);
                SetReturnButtonEnable(ProfileSettingsPointButtonEnum.CenterFun, false);
            }
            else
            {
                btnCenterPointAssignmantMethod.Enabled = true;
            }
        }

        private void BtnChooseSecondPointAssignmentMethod_Click(object sender, EventArgs e)
        {
            controller.SetPointBySelectedMethod(controller.GetMethodByString(cmbSecondPointAssignmentMethod.Text), ProfileSettingsPointButtonEnum.PointsSecond);
        }

        private void BtnChooseFirstPointAssignmentMethod_Click(object sender, EventArgs e)
        {
            controller.SetPointBySelectedMethod(controller.GetMethodByString(cmbFirstPointAssignmentMethod.Text), ProfileSettingsPointButtonEnum.PointsFist);
        }

        private void BtnCenterPointAssignmantMethod_Click(object sender, EventArgs e)
        {
            controller.SetPointBySelectedMethod(controller.GetMethodByString(cmbCenterPointAssignmentMethod.Text), ProfileSettingsPointButtonEnum.CenterFun);
        }

        private void BtnFlipPoints_Click(object sender, EventArgs e)
        {
            controller.FlipPoints();
        }

        private void BtnTargetObjAssignmentMethod_Click(object sender, EventArgs e)
        {
            var targetAssignmentMethod = controller.GetTargetAssignmentMethodByString(cmbTargetObjAssignmentMethod.SelectedItem.ToString());
            var creationMethod = controller.GetCreationMethodByString(cmbTargetObjCreation.SelectedItem.ToString());

            if(targetAssignmentMethod == AssignmentMethodsEnum.Sector || creationMethod == ToPointsCreationMethodsEnum.AzimuthsLines)
            {
                controller.CalcFunToPoints(targetAssignmentMethod, ToPointsCreationMethodsEnum.Default, true);
            }
            else
            {
                controller.CalcFunToPoints(targetAssignmentMethod, creationMethod, true);
            }

            if(targetAssignmentMethod == AssignmentMethodsEnum.Sector)
            {
                cmbTargetObjCreation.SelectedItem = LocalizationContext.Instance.ToPointsCreationMethodAzimuthsLines;
                cmbTargetObjCreation.Enabled = false;
            }
            else
            {
                cmbTargetObjCreation.Enabled = true;
            }
        }
        
        private void BtnChooseCreationMethod_Click(object sender, EventArgs e)
        {
            if(controller.GetCreationMethodByString(cmbTargetObjCreation.SelectedItem.ToString()) == ToPointsCreationMethodsEnum.AzimuthsLines)
            {
                SetFunTxtEnabled(true);
            }
            else
            {
                SetFunTxtEnabled(false);
            }

            RecalculateFun();
        }

        private void SetFunTxtEnabled(bool isEnabled)
        {
            azimuth1.Enabled = isEnabled;
            azimuth2.Enabled = isEnabled;
            funLinesCount.Enabled = isEnabled;
        }

        private void BtnPanToFun_Click(object sender, EventArgs e)
        {
            controller.PanToProfile(ProfileSettingsTypeEnum.Fun);
        }
        
        private void BtnPrimitiveAssignmentMethod_Click(object sender, EventArgs e)
        {
            controller.SetProfileSettings(ProfileSettingsTypeEnum.Primitives);
        }

        private void BtnPanToPrimitive_Click(object sender, EventArgs e)
        {
            controller.PanToProfile(ProfileSettingsTypeEnum.Primitives);
        }

        private void SetListView()
        {
            lvProfileAttributes.Columns.Add("Attribute", -1);
            lvProfileAttributes.Columns[0].Width += 5;
            lvProfileAttributes.Columns.Add("Value", -1);

            lvProfileAttributes.HeaderStyle = ColumnHeaderStyle.None;
        }

        private void RenameProfile_Click(object sender, EventArgs e)
        {
            profilesTreeView.LabelEdit = true;

            var node = profilesTreeView.SelectedNode;
            var ids = GetProfileAndLineIds(node);

            var lineId = ids.Item2;
            treeViewselectedIds.ProfileSessionId = ids.Item1;
            TreeNode selectedNode;

            if(lineId == -1)
            {
                selectedNode = profilesTreeView.SelectedNode;
            }
            else
            {
                selectedNode = profilesTreeView.SelectedNode.Parent;
            }

            selectedNode.BeginEdit();
        }

        private void ProfilesTreeView_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            var res = MessageBox.Show(LocalizationContext.Instance.FindLocalizedElement("MsgRenameProfileText", "Ви дійсно хочете перейменувати профіль/набор профілів?"), LocalizationContext.Instance.MessageBoxTitle, MessageBoxButtons.OKCancel);

            if(res == DialogResult.OK)
            {
                e.Node.EndEdit(false);

                if(!String.IsNullOrEmpty(e.Label))
                {
                    var ids = GetProfileAndLineIds(e.Node);
                    controller.RenameProfile(ids.Item1, e.Label);
                    var node = e.Node as ProfileTreeNode;

                    node.SetProfileName(e.Label);

                    var lvItem = new ListViewItem(node.Attributes.Rows[0][AttributeKeys.AttributeColumnName].ToString());
                    lvItem.SubItems.Add(node.Attributes.Rows[0][AttributeKeys.ValueColumnName].ToString());

                    lvProfileAttributes.Items[0] = lvItem;
                }
            }
            else
            {
                e.CancelEdit = true;
            }

            profilesTreeView.LabelEdit = false;
        }
    }
}
