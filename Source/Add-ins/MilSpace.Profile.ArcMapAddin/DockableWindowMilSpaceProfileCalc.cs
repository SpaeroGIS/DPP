using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geometry;
using MilSpace.Core;
using MilSpace.Core.Tools;
using MilSpace.Tools;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Media;
using System.Linq;
using Point = ESRI.ArcGIS.Geometry.Point;
using System.Diagnostics;
using System.Globalization;

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

        private ProfileSettingsPointButton activeButtton = ProfileSettingsPointButton.None;

        MilSpaceProfileCalsController controller;

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

        public ProfileSettingsPointButton ActiveButton => activeButtton;

        public string DemLayerName => cmbRasterLayers.SelectedItem.ToString();

        public int ProfileId
        {
            set { txtProfileName.Text = value.ToString(); }
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

            controller.OnProfileSettingsChanged += OnProfileSettingsChanged;

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
                    activeButtton = ProfileSettingsPointButton.PointsFist;

                    break;
                case 1:

                    break;
                case 2:
                    controller.FlashPoint(ProfileSettingsPointButton.PointsFist);
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

                    activeButtton = ProfileSettingsPointButton.PointsSecond;

                    break;
                case 0:
                    break;
                case 2:
                    controller.FlashPoint(ProfileSettingsPointButton.PointsSecond);
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

                    activeButtton = ProfileSettingsPointButton.CenterFun;

                    break;
                case 0:

                    break;
                case 2:

                    controller.FlashPoint(ProfileSettingsPointButton.CenterFun);
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

        private IPolyline GetPolylineFromPoints()
        {
            var firstPoint = ParseStringCoordsToPoint(txtFirstPointX.Text, txtFirstPointY.Text);
            var secondPoint = ParseStringCoordsToPoint(txtSecondPointX.Text, txtSecondPointY.Text);
            IPolyline polyline = new PolylineClass();
            polyline.FromPoint = firstPoint;
            polyline.ToPoint = secondPoint;
            return polyline;
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
        double IMilSpaceProfileView.FunAzimuth2
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
                MessageBox.Show("Calculated");
            }
            else
            {
                //TODO log error
                MessageBox.Show("Calcu;lation error", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            controller.SetPeofileSettigs(ProfileSettingsTypeEnum.Fun);
        }


        private void profileSettingsTab_SelectedIndexChanged(object sender, EventArgs e)
        {
            controller.SetPeofileSettigs(SelectedProfileSettingsType);

        }

        private void cmbRasterLayers_SelectedIndexChanged(object sender, EventArgs e)
        {
            controller.SetPeofileSettigs(SelectedProfileSettingsType);
        }

        private static bool CheckDouble(char charValue, TextBox textValue, bool justInt = false)
        {

            return (((charValue == BACKSPACE) || ((charValue >= ZERO) && (charValue <= NINE))) || (justInt ||

                  ((charValue == DECIMAL_POINT) && textValue.Text.IndexOf(".") == NOT_FOUND)));
        }

    }
}
