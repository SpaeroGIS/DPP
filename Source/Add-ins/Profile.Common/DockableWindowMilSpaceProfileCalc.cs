﻿using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geometry;
using MilSpace.Core;
using MilSpace.Core.ModulesInteraction;
using MilSpace.Core.Tools;
using MilSpace.DataAccess.DataTransfer;
using MilSpace.Profile.DTO;
using MilSpace.Profile.Interaction;
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
            cmbPointLayers.Items.Clear();
            layersToSelectLine.Items.Clear();

            MapLayersManager mlmngr = new MapLayersManager(ActiveView);

            PopulateComboBox(cmbRasterLayers, mlmngr.RasterLayers);
            PopulateComboBox(cmbRoadLayers, mlmngr.PolygonLayers);
            PopulateComboBox(cmbHydrographyLayer, mlmngr.PolygonLayers);
            PopulateComboBox(cmbBuildings, mlmngr.PolygonLayers);
            PopulateComboBox(cmbVegetationLayer, mlmngr.PolygonLayers);
            PopulateComboBox(cmbPointLayers, mlmngr.PointLayers);

            layersToSelectLine.Items.AddRange(GetLayersForLineSelection.ToArray());
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

            removeProfile.Enabled = addProfileToGraph.Enabled = toolPanOnMap.Enabled = toolBtnFlash.Enabled =
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
            lblSelectedPrimitivesValue.Text = selectedLines.LinesCount.ToString();
            lblCommonLengthValue.Text = selectedLines.FullLength.ToString("F2");
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
            logger.DebugEx("> SetPointValue. controlX.Text:{0} controlY.Text:{1}", controlX.Text, controlY.Text);
            try
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

            firstPointToolBar.Buttons["toolBarButton8"].ToolTipText = LocalizationContext.Instance.FindLocalizedElement("BtnTakeCoordToolTip", "Взяти координати з карти");
            firstPointToolBar.Buttons["toolBarButton55"].ToolTipText = LocalizationContext.Instance.FindLocalizedElement("BtnShowCoordToolTip", "Показати координати на карті");
            firstPointToolBar.Buttons["toolBarButton57"].ToolTipText = LocalizationContext.Instance.FindLocalizedElement("BtnCopyCoordToolTip", "Копіювати координати");
            firstPointToolBar.Buttons["toolBarButton58"].ToolTipText = LocalizationContext.Instance.FindLocalizedElement("BtnPasteCoordToolTip", "Вставити координати");

            secondPointToolbar.Buttons["toolBarButton61"].ToolTipText = LocalizationContext.Instance.FindLocalizedElement("BtnTakeCoordToolTip", "Взяти координати з карти");
            secondPointToolbar.Buttons["toolBarButton2"].ToolTipText = LocalizationContext.Instance.FindLocalizedElement("BtnShowCoordToolTip", "Показати координати на карті");
            secondPointToolbar.Buttons["toolBarButton3"].ToolTipText = LocalizationContext.Instance.FindLocalizedElement("BtnCopyCoordToolTip", "Копіювати координати");
            secondPointToolbar.Buttons["toolBarButton4"].ToolTipText = LocalizationContext.Instance.FindLocalizedElement("BtnPasteCoordToolTip", "Вставити координати");

            basePointToolbar.Buttons["toolBarButton16"].ToolTipText = LocalizationContext.Instance.FindLocalizedElement("BtnTakeCoordToolTip", "Взяти координати з карти");
            basePointToolbar.Buttons["toolBarButton17"].ToolTipText =  LocalizationContext.Instance.FindLocalizedElement("BtnShowCoordToolTip", "Показати координати на карті");
            basePointToolbar.Buttons["toolBarButton19"].ToolTipText =  LocalizationContext.Instance.FindLocalizedElement("BtnCopyCoordToolTip", "Копіювати координати");
            basePointToolbar.Buttons["toolBarButton20"].ToolTipText = LocalizationContext.Instance.FindLocalizedElement("BtnPasteCoordToolTip", "Вставити координати");

            addAvailableProfilesSets.ToolTipText = LocalizationContext.Instance.FindLocalizedElement("BtnAddAvailableProfilesSetsToolTip", "Додати доступні набори профілів");
            lblSelectedPrimitives.Text = LocalizationContext.Instance.FindLocalizedElement("LblSelectedPrimitivesText", "Вибрані об'єкти:");
            lblCommonLength.Text = LocalizationContext.Instance.FindLocalizedElement("LblCommonLengthText", "Довжина вибраних об'єктів:");

            //Profile Tabs
            profileTabPage.Text = LocalizationContext.Instance.FindLocalizedElement("TabProfileTabPageText", "Параметри профілю");
            profileTreeTabPage.Text = LocalizationContext.Instance.FindLocalizedElement("TabPofileTreeTabPageText", "Профілі в роботі");

            sectionTab.Text = LocalizationContext.Instance.FindLocalizedElement("TabSectionTabText", "Відрізки");
            //loadTab.Text = LocalizationContext.Instance.FindLocalizedElement.LoadTabText;
            primitiveTab.Text = LocalizationContext.Instance.FindLocalizedElement("TabPrimitiveTabText", "Графіка");
            funTab.Text = LocalizationContext.Instance.FindLocalizedElement("TabFunTabText", "\"Віяло\"");


            //Labels
            lblDEM.Text = LocalizationContext.Instance.FindLocalizedElement("LblDEMTextKey", "Шар ЦМР/ЦММ");

            lblLayersForCalc.Text = LocalizationContext.Instance.FindLocalizedElement("LblLayersForCalcText", "Шари для розрахунків");
            lblVegetationLayer.Text = LocalizationContext.Instance.FindLocalizedElement("LblVegetationLayerText", "рослинність");
            lblBuildingsLayer.Text = LocalizationContext.Instance.FindLocalizedElement("LblBuildingsLayerText", "забудова");
            lblRoadsLayer.Text = LocalizationContext.Instance.FindLocalizedElement("LblRoadsLayerText", "транспорт");
            lblHydrographyLayer.Text = LocalizationContext.Instance.FindLocalizedElement("LblHydrographyLayerText", "гідрографія");
            lblPointOfViewLayer.Text = LocalizationContext.Instance.FindLocalizedElement("LblPointOfViewLayerText", "Шар точок спостереження");
            lblSetPeofileProperties.Text = LocalizationContext.Instance.FindLocalizedElement("LblSetProfilePropertiesText", "Задати профіль");
            lblProfileName.Text = LocalizationContext.Instance.FindLocalizedElement("LblProfileNameText", "Ім'я профілю");

            calcProfile.Text = LocalizationContext.Instance.FindLocalizedElement("BtnСalcProfileText", "Розрахувати");
            lblLineFirstPoint.Text = LocalizationContext.Instance.FindLocalizedElement("LblLineFirstPointText", "Перша точка (довгота / широта)");
            lblLineSecondPoint.Text = LocalizationContext.Instance.FindLocalizedElement("LblLineSecondPointText", "Друга точка (довгота / широта)");

            lblHeightOfViewFirst.Text = LocalizationContext.Instance.FindLocalizedElement("LblHeightOfViewText", "Висота точки спостереження");
            lblHeightOfViewSecond.Text = LocalizationContext.Instance.FindLocalizedElement("LblHeightOfViewText", "Висота точки спостереження");
            lblDimensionSecond.Text = LocalizationContext.Instance.FindLocalizedElement("LblDimensionSecondText", "м");
            lblDimensionFirst.Text = LocalizationContext.Instance.FindLocalizedElement("LblDimensionFirstText", "м");
            lblFunBasePoint.Text = LocalizationContext.Instance.FindLocalizedElement("LblFunBasePointText", " точка (довгота / широта)");
            lblHeightOfViewFunBaseText.Text = LocalizationContext.Instance.FindLocalizedElement("LblHeightOfViewText", "Висота точки спостереження");
            lblFunParameters.Text = LocalizationContext.Instance.FindLocalizedElement("LblFunParametersText", "Параметри");
            lblFunDistance.Text = LocalizationContext.Instance.FindLocalizedElement("LblFunDistanceText", "Відстань");
            lblFunCount.Text = LocalizationContext.Instance.FindLocalizedElement("LblFunCountText", "Кількість");

            lblFunAzimuth1.Text = LocalizationContext.Instance.FindLocalizedElement("LblFunAzimuth1Text", "Азимут 1");
            lblFunAzimuth2.Text = LocalizationContext.Instance.FindLocalizedElement("LblFunAzimuth2Text", "Азимут 2");

            lblHeightOfViewGraphics.Text = LocalizationContext.Instance.FindLocalizedElement("LblHeightOfViewText", "Висота точки спостереження");
            lblPrimitivesLayerToSelect.Text = LocalizationContext.Instance.FindLocalizedElement("LblPrimitivesLayerToSelectText", "Шари для вибору об'єктів");
            lblAboutSelected.Text = LocalizationContext.Instance.FindLocalizedElement("LblAboutSelectedText", "Інформація про обране");

            lblProfileList.Text = LocalizationContext.Instance.FindLocalizedElement("LblProfileListText", "Профілі в роботі");

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

            profilesTreeView.Nodes["Points"].Text = LocalizationContext.Instance.FindLocalizedElement("TvProfilesPointsNodeText", "Відрізки");
            profilesTreeView.Nodes["Fun"].Text = LocalizationContext.Instance.FindLocalizedElement("TvProfilesFunNodeText", "\"Віяло\"");
            profilesTreeView.Nodes["Primitives"].Text = LocalizationContext.Instance.FindLocalizedElement("TvProfilesPritiveNodeText", "Графіка");

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
                if (profile.DefinitionType == ProfileSettingsTypeEnum.Points)
                {
                    var firstX = profile.ProfileLines.First().Line.FromPoint.X.ToString("F5");
                    var firstY = profile.ProfileLines.First().Line.FromPoint.Y.ToString("F5");
                    var secondX = profile.ProfileLines.First().Line.ToPoint.X.ToString("F5");
                    var secondY = profile.ProfileLines.First().Line.ToPoint.Y.ToString("F5");
                    var lineDistance = profile.ProfileLines.First().Line.Length.ToString("F5");

                    newNode.SetBasePoint($"X= {firstX}; Y= {firstY};");
                    newNode.SetToPoint($"X= {secondX}; Y= {secondY};");
                    newNode.SetBasePointHeight(profile.ObserverHeight.ToString());
                    newNode.SetToPointHeight(SectionHeightSecond.ToString());
                    newNode.SetLineDistance(lineDistance);

                }
                else if (profile.DefinitionType == ProfileSettingsTypeEnum.Fun)
                {
                    var basePointX = profile.ProfileLines.First().Line.FromPoint.X.ToString("F5");
                    var basePointY = profile.ProfileLines.First().Line.FromPoint.Y.ToString("F5");
                    var lineDistance = profile.ProfileLines.First().Line.Length.ToString("F5");
                    var linesCount = profile.ProfileLines.Length.ToString();

                    newNode.SetBasePoint($"X= {basePointX}; Y= {basePointY};");
                    newNode.SetLineCount(linesCount);
                    newNode.SetAzimuth1(profile.Azimuth1);
                    newNode.SetAzimuth2(profile.Azimuth2);
                    newNode.SetBasePointHeight(profile.ObserverHeight.ToString());
                    newNode.SetLineDistance(lineDistance);
                }
                else if (profile.DefinitionType == ProfileSettingsTypeEnum.Primitives)
                {
                    var linesCount = profile.ProfileLines.Length.ToString();
                    newNode.SetLineCount(linesCount);
                }

                newNode.SetCreatorName(profile.CreatedBy);
                newNode.SetDate($"{date.ToLongDateString()} {date.ToLongTimeString()}");

                logger.InfoEx($"Profile  {profile.SessionName} added to the tree");

                string lineDefinition = LocalizationContext.Instance.FindLocalizedElement("TxtTreeViewProfileText", "Профіль:");

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
                    childNode.SetStartPoint($"X={line.Line.FromPoint.X:F5}; Y={line.Line.FromPoint.Y:F5}");
                    childNode.SetToPoint($"X={line.Line.ToPoint.X:F5}; Y={line.Line.ToPoint.Y:F5}");

                    childNode.SetAzimuth($"{azimuth}{Degree}");

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

                var baseValue = rows.Find(AttributeKeys.BasePoint)[AttributeKeys.ValueColumnName].ToString();

                var basePoint = GetPointFromRowValue(baseValue);
                controller.SetFirsPointForLineProfile(basePoint.CloneWithProjecting(), basePoint);

                var toValue = rows.Find(AttributeKeys.ToPoint)[AttributeKeys.ValueColumnName].ToString();

                var toPoint = GetPointFromRowValue(toValue);
                controller.SetSecondfPointForLineProfile(toPoint.CloneWithProjecting(), toPoint);

                txtFirstHeight.Text = rows.Find(AttributeKeys.SectionFirstPointHeight)[AttributeKeys.ValueColumnName].ToString();
                txtSecondHeight.Text = rows.Find(AttributeKeys.SectionSecondPointHeight)[AttributeKeys.ValueColumnName].ToString();
            }
            if (profileType == ProfileSettingsTypeEnum.Fun)
            {
                profileSettingsTab.SelectTab(1);

                var baseValue = rows.Find(AttributeKeys.BasePoint)[AttributeKeys.ValueColumnName].ToString();

                var basePoint = GetPointFromRowValue(baseValue);
                controller.SetCenterPointForFunProfile(basePoint.CloneWithProjecting(), basePoint);

                txtObserverHeight.Text = rows.Find(AttributeKeys.SectionFirstPointHeight)[AttributeKeys.ValueColumnName].ToString();

                var length = rows.Find(AttributeKeys.LineDistance)[AttributeKeys.ValueColumnName].ToString();
                profileLength.Text = length.Split(',')[0];

                funLinesCount.Text = rows.Find(AttributeKeys.LinesCount)[AttributeKeys.ValueColumnName].ToString();
                azimuth1.Text = rows.Find(AttributeKeys.Azimuth1)[AttributeKeys.ValueColumnName].ToString();
                azimuth2.Text = rows.Find(AttributeKeys.Azimuth2)[AttributeKeys.ValueColumnName].ToString();
            }

            controller.SetProfileSettings(profileType);
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
    }
}
