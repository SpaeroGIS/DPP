using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Editor;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geometry;
using MilSpace.Core.Tools;
using MilSpace.DataAccess.DataTransfer;
using MilSpace.Visibility.DTO;
using MilSpace.Visibility.ViewController;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace MilSpace.Visibility
{
    /// <summary>
    /// Designer class of the dockable window add-in. It contains user interfaces that
    /// make up the dockable window.
    /// </summary>
    public partial class DockableWindowMilSpaceMVisibilitySt : UserControl, IObservationPointsView
    {
        private ObservationPointsController controller;
        private string _unsavedPointId = string.Empty;

        public DockableWindowMilSpaceMVisibilitySt(object hook, ObservationPointsController controller)
        {
            InitializeComponent();
            this.controller = controller;
            this.controller.SetView(this);
            this.Hook = hook;
        }

        public DockableWindowMilSpaceMVisibilitySt(object hook)
        {
            InitializeComponent();
            this.Hook = hook;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            controller.UpdateObservationPointsList();
            SubscribeForEvents();
            InitilizeData();
        }

        private void SubscribeForEvents()
        {
            IEditEvents_Event editEvent = (IEditEvents_Event)ArcMap.Editor;
            editEvent.OnCreateFeature += controller.OnCreateFeature;

            ArcMap.Events.OpenDocument += delegate ()
            {
                IActiveViewEvents_Event activeViewEvent = (IActiveViewEvents_Event)ActiveView;

                activeViewEvent.SelectionChanged += OnContentsChanged;
                activeViewEvent.ItemAdded += OnItemAdded;

                OnContentsChanged();
            };
        }

        /// <summary>
        /// Host object of the dockable window
        /// </summary>
        private object Hook
        {
            get;
            set;
        }

        #region
        public VeluableObservPointFieldsEnum GetFilter
        {
            get
            {
                var result = VeluableObservPointFieldsEnum.All;

                if(chckFilterAffiliation.Checked)
                {
                    result = result | VeluableObservPointFieldsEnum.Affiliation;
                }
                if(chckFilterDate.Checked)
                {
                    result = result | VeluableObservPointFieldsEnum.Date;
                }

                if(chckFilterType.Checked)
                {
                    result = result | VeluableObservPointFieldsEnum.Type;
                }

                return result;
            }
        }

        public IEnumerable<string> GetTypes
        {
            get
            {
                return controller.GetObservationPointMobilityTypes();
            }
        }
        public IEnumerable<string> GetAffiliation
        {
            get
            {
                return controller.GetObservationPointTypes();
            }
        }

        public void FillObservationPointList(IEnumerable<ObservationPoint> observationPoints, VeluableObservPointFieldsEnum filter)
        {
            if(observationPoints.Any())
            {
                var ItemsToShow = observationPoints.Select(i => new ObservPointGui
                {
                    Title = i.Title,
                    Type = i.Type,
                    Affiliation = i.Affiliation,
                    Date = i.Dto,
                    Id = i.Id
                }).ToList();

                dgvObservationPoints.Rows.Clear();

                BindingList<ObservPointGui> observPointGuis = new BindingList<ObservPointGui>(ItemsToShow);
                dgvObservationPoints.DataSource = observPointGuis;

                SetDataGridView();
                FilterColumns(filter);
                dgvObservationPoints.Update();
            }
        }

        private void SetDataGridView()
        {
            dgvObservationPoints.Columns["Title"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvObservationPoints.Columns["Id"].Visible = false;
        }

        private void FilterColumns(VeluableObservPointFieldsEnum filter)
        {
            dgvObservationPoints.Columns["Affiliation"].Visible = chckFilterAffiliation.Checked;
            dgvObservationPoints.Columns["Type"].Visible = chckFilterType.Checked;
            dgvObservationPoints.Columns["Date"].Visible = chckFilterDate.Checked;
        }

        private void InitilizeData()
        {
            cmbObservPointType.Items.Clear();
            cmbObservTypesEdit.Items.Clear();
            var filters = new List<string>();
            filters.Add(string.Empty);
            filters.AddRange(GetTypes.ToArray());

            cmbObservPointType.Items.AddRange(filters.ToArray());
            cmbObservTypesEdit.Items.AddRange(GetTypes.ToArray());

            filters = new List<string>();
            filters.Add(string.Empty);

            filters.AddRange(GetAffiliation.ToArray());
            cmbAffiliation.Items.Clear();
            cmbAffiliationEdit.Items.Clear();

            cmbAffiliation.Items.AddRange(filters.ToArray());
            cmbAffiliationEdit.Items.AddRange(GetAffiliation.ToArray());

            SetDefaultValues();
        }

        private void PopulatePointsLayersComboBox()
        {
            if(cmbObservPointsLayers.Visible)
            {
                cmbObservPointsLayers.Items.Clear();
                cmbObservPointsLayers.Items.AddRange(controller.GetObservationPointsLayers(ActiveView).ToArray());
                cmbObservPointsLayers.SelectedItem = controller.GetObservFeatureName();
            }
        }

        private void SetDefaultValues()
        {
            cmbObservTypesEdit.SelectedItem = ObservationPointMobilityTypesEnum.Stationary.ToString();
            cmbAffiliationEdit.SelectedItem = ObservationPointTypesEnum.Enemy.ToString();

            azimuthB.Text = ObservPointDefaultValues.AzimuthBText;
            azimuthE.Text = ObservPointDefaultValues.AzimuthEText;
            heightCurrent.Text = ObservPointDefaultValues.RelativeHeightText;
            heightMin.Text = ObservPointDefaultValues.HeightMinText;
            heightMax.Text = ObservPointDefaultValues.HeightMaxText;
            observPointName.Text = ObservPointDefaultValues.ObservPointNameText;
            angleOFViewMin.Text = ObservPointDefaultValues.AngleOFViewMinText;
            angleOFViewMax.Text = ObservPointDefaultValues.AngleOFViewMaxText;
            angleFrameH.Text = ObservPointDefaultValues.AngleFrameHText;
            angleFrameV.Text = ObservPointDefaultValues.AngleFrameVText;
            cameraRotationH.Text = ObservPointDefaultValues.CameraRotationHText;
            cameraRotationV.Text = ObservPointDefaultValues.CameraRotationVText;
            azimuthMainAxis.Text = ObservPointDefaultValues.AzimuthMainAxisText;

            observPointDate.Text = DateTime.Now.ToShortDateString();
            observPointCreator.Text = Environment.UserName;
        }

        private void SetCoordDefaultValues()
        {
            var centerPoint = controller.GetEnvelopeCenterPoint(ArcMap.Document.ActiveView.Extent);
            xCoord.Text = centerPoint.X.ToString();
            yCoord.Text = centerPoint.Y.ToString();
        }

        private void FieldsValidation(object sender, EventArgs e)
        {
            try
            {
                var textBox = (TextBox)sender;

                switch(textBox.Name)
                {
                    case "xCoord":

                        if(!Regex.IsMatch(xCoord.Text, @"^([-]?[\d]{1,2}\,\d+)$"))
                        {
                            MessageBox.Show("Invalid data.\nInsert the coordinates in the WGS84 format.");
                            var centerPoint = controller.GetEnvelopeCenterPoint(ArcMap.Document.ActiveView.Extent);
                            xCoord.Text = centerPoint.X.ToString();
                        }
                        else
                        {
                            var x = Convert.ToDouble(xCoord.Text);
                            var y = Convert.ToDouble(yCoord.Text);

                            ShowPoint(x, y);
                        }

                        break;

                    case "yCoord":

                        if(!Regex.IsMatch(yCoord.Text, @"^([-]?[\d]{1,2}\,\d+)$"))
                        {
                            MessageBox.Show("Invalid data.\nInsert the coordinates in the WGS84 format.");
                            var centerPoint = controller.GetEnvelopeCenterPoint(ArcMap.Document.ActiveView.Extent);
                            yCoord.Text = centerPoint.Y.ToString();
                        }
                        else
                        {
                            var x = Convert.ToDouble(xCoord.Text);
                            var y = Convert.ToDouble(yCoord.Text);

                            ShowPoint(x, y);
                        }

                        break;

                    case "angleOFViewMin":

                        ValidateRange(angleOFViewMin, ObservPointDefaultValues.AngleOFViewMinText, -90, 0);

                        break;


                    case "angleOFViewMax":

                        ValidateRange(angleOFViewMax, ObservPointDefaultValues.AngleOFViewMaxText, 0, 90);

                        break;

                    case "azimuthB":

                        ValidateAzimuth(textBox, ObservPointDefaultValues.AzimuthBText);

                        break;

                    case "azimuthE":

                        ValidateAzimuth(textBox, ObservPointDefaultValues.AzimuthEText);

                        break;

                    case "azimuthMainAxis":

                        ValidateAzimuth(textBox, ObservPointDefaultValues.AzimuthMainAxisText);

                        break;

                    case "cameraRotationH":

                        ValidateAzimuth(textBox, ObservPointDefaultValues.CameraRotationHText);

                        break;

                    case "cameraRotationV":

                        ValidateAzimuth(textBox, ObservPointDefaultValues.CameraRotationVText);

                        break;

                    case "heightCurrent":

                        var currentHeight = ValidateHeight(textBox, ObservPointDefaultValues.RelativeHeightText);

                        if(currentHeight != -1)
                        {
                            var minHeight = Convert.ToDouble(heightMin.Text);
                            var maxHeight = Convert.ToDouble(heightMax.Text);

                            if(currentHeight > maxHeight)
                            {
                                heightMax.Text = currentHeight.ToString();
                            }

                            if(currentHeight < minHeight)
                            {
                                heightMin.Text = currentHeight.ToString();
                            }
                        }


                        break;

                    case "heightMin":

                        var minHeightChanged = ValidateHeight(textBox, ObservPointDefaultValues.RelativeHeightText);

                        if(minHeightChanged != -1)
                        {
                            var curHeight = Convert.ToDouble(heightCurrent.Text);
                            var maxHeight = Convert.ToDouble(heightMax.Text);

                            if(minHeightChanged > curHeight)
                            {
                                heightCurrent.Text = minHeightChanged.ToString();
                            }

                            if(minHeightChanged > maxHeight)
                            {
                                heightMax.Text = minHeightChanged.ToString();
                            }
                        }

                        break;

                    case "heightMax":

                        var maxHeightChanged = ValidateHeight(textBox, ObservPointDefaultValues.RelativeHeightText);

                        if(maxHeightChanged != -1)
                        {
                            var curHeight = Convert.ToDouble(heightCurrent.Text);
                            var minHeight = Convert.ToDouble(heightMin.Text);

                            if(maxHeightChanged < curHeight)
                            {
                                heightCurrent.Text = maxHeightChanged.ToString();
                            }

                            if(maxHeightChanged < minHeight)
                            {
                                heightMax.Text = maxHeightChanged.ToString();
                            }
                        }

                        break;
                }
            }

            catch(Exception ex) { return; }
        }

        private void ValidateAzimuth(TextBox azimuthTextBox, string defaultValue)
        {
            ValidateRange(azimuthTextBox, defaultValue, 0, 360);
        }

        private void ValidateRange(TextBox textBox, string defaultValue, double lowValue, double upperValue)
        {
            double value;

            if(Double.TryParse(textBox.Text, out value))
            {
                if(value >= lowValue && value <= upperValue)
                {
                    return;
                }
            }

            textBox.Text = defaultValue;
            MessageBox.Show($"Invalid data.\nInsert the value in the range from {lowValue} to {upperValue}");
        }

        private double ValidateHeight(TextBox heightTextBox, string defaultValue)
        {
            double height;

            if(Double.TryParse(heightTextBox.Text, out height))
            {
                if(height >= 0)
                {
                    return height;
                }

                MessageBox.Show("Invalid data.\nValue cannot be less than 0");
            }
            else
            {
                MessageBox.Show("Invalid data.\nInsert the number");
            }

            heightTextBox.Text = defaultValue;


            return -1;
        }


        private void EnableObservPointsControls()
        {
            lblLayer.Visible = cmbObservPointsLayers.Visible = cmbAffiliationEdit.Enabled = cmbObservTypesEdit.Enabled = azimuthB.Enabled
                = azimuthE.Enabled = xCoord.Enabled = yCoord.Enabled =  angleOFViewMin.Enabled = angleOFViewMax.Enabled 
                = heightCurrent.Enabled = heightMin.Enabled = azimuthMainAxis.Enabled
                = heightMax.Enabled = observPointName.Enabled = controller.IsObservPointsExists(ActiveView);

            angleFrameH.Enabled = angleFrameV.Enabled = observPointDate.Enabled = observPointCreator.Enabled = false;
        }

        private void OnSelectObserbPoint()
        {

        }

        private void OnItemAdded(object item)
        {
            EnableObservPointsControls();
            PopulatePointsLayersComboBox();
        }

        private void OnContentsChanged()
        {
            EnableObservPointsControls();
            PopulatePointsLayersComboBox();
            SetCoordDefaultValues();
        }

        #endregion

        public IActiveView ActiveView => ArcMap.Document.ActiveView;

        /// <summary>
        /// Implementation class of the dockable window add-in. It is responsible for 
        /// creating and disposing the user interface class of the dockable window.
        /// </summary>
        public class AddinImpl : ESRI.ArcGIS.Desktop.AddIns.DockableWindow
        {
            private DockableWindowMilSpaceMVisibilitySt m_windowUI;

            public AddinImpl()
            {
            }

            internal DockableWindowMilSpaceMVisibilitySt UI
            {
                get { return m_windowUI; }
            }

            protected override IntPtr OnCreateChild()
            {
                var controller = new ObservationPointsController();

                m_windowUI = new DockableWindowMilSpaceMVisibilitySt(this.Hook, controller);
                return m_windowUI.Handle;
            }

            protected override void Dispose(bool disposing)
            {
                if(m_windowUI != null)
                    m_windowUI.Dispose(disposing);

                base.Dispose(disposing);
            }

        }

        private void toolBar9_ButtonClick(object sender, ToolBarButtonClickEventArgs e)
        {
            (new WindowMilSpaceMVisibilityMaster()).ShowDialog();
        }

        private void TlbObserPoints_ButtonClick(object sender, ToolBarButtonClickEventArgs e)
        {
            //UID mapToolID = new UIDClass
            //{
            //    Value = ThisAddIn.IDs.MapInteropTool
            //};
            //var documentBars = ArcMap.Application.Document.CommandBars;
            //var mapTool = documentBars.Find(mapToolID, false, false);

            //if (ArcMap.Application.CurrentTool?.ID?.Value != null && ArcMap.Application.CurrentTool.ID.Value.Equals(mapTool.ID.Value))
            //{
            //    ArcMap.Application.CurrentTool = null;
            //    toolBarButton51.Pushed = false;
            //}
            //else
            //{
            //    ArcMap.Application.CurrentTool = mapTool;
            //    toolBarButton51.Pushed = true;
            //}

            switch(tlbObservPoints.Buttons.IndexOf(e.Button))
            {

                case 3:

                    CreateNewPoint(GetObservationPoint());
                    _unsavedPointId = string.Empty;

                    break;
            }

        }

        private void ShowPoint(double x, double y)
        {
            IPoint resultPoint = new Point { X = x, Y = y, SpatialReference = EsriTools.Wgs84Spatialreference };
            resultPoint.ID = dgvObservationPoints.Rows.Count + 1;

            if(!string.IsNullOrEmpty(_unsavedPointId))
            {
                RemovePointFromMap(_unsavedPointId);
            }

            _unsavedPointId = AddPointToMap(resultPoint);
            EsriTools.PanToGeometry(ActiveView, resultPoint);
        }

        internal void ArcMap_OnMouseDown(int x, int y)
        {
            if(!(this.Hook is IApplication arcMap) || !(arcMap.Document is IMxDocument currentDocument)) return;

            IPoint resultPoint = new Point();

            resultPoint = (currentDocument.FocusMap as IActiveView).ScreenDisplay.DisplayTransformation.ToMapPoint(x, y);
            resultPoint.ID = dgvObservationPoints.Rows.Count + 1;

            if(!string.IsNullOrEmpty(_unsavedPointId))
            {
                RemovePointFromMap(_unsavedPointId);
            }

            _unsavedPointId = AddPointToMap(resultPoint);

            resultPoint.Project(EsriTools.Wgs84Spatialreference);

            xCoord.Text = resultPoint.X.ToString();
            yCoord.Text = resultPoint.Y.ToString();
        }

        internal void ArcMap_OnMouseMove(int x, int y)
        {
            //Place Mouce Move logic here if needed
        }

        private string AddPointToMap(IPoint point)
        {
            if(point != null && !point.IsEmpty)
            {
                var color = (IColor)new RgbColorClass() { Green = 255 };
                var placedPoint = ArcMapHelper.AddGraphicToMap(point, color, true, esriSimpleMarkerStyle.esriSMSDiamond, 7);
                return placedPoint.Key;
            }

            return string.Empty;
        }

        private void RemovePointFromMap(string pointId)
        {
            ArcMapHelper.RemoveGraphicsFromMap(new string[1] { pointId });
        }

        private void CreateNewPoint(ObservationPoint point)
        {
            controller.AddPoint(point, cmbObservPointsLayers.SelectedItem.ToString(), ActiveView);
        }

        private void TlbCoordinates_ButtonClick(object sender, ToolBarButtonClickEventArgs e)
        {
            switch(tlbCoordinates.Buttons.IndexOf(e.Button))
            {
                case 0:

                    UID mapToolID = new UIDClass
                    {
                        Value = ThisAddIn.IDs.MapInteropTool
                    };
                    var documentBars = ArcMap.Application.Document.CommandBars;
                    var mapTool = documentBars.Find(mapToolID, false, false);

                    if(ArcMap.Application.CurrentTool?.ID?.Value != null && ArcMap.Application.CurrentTool.ID.Value.Equals(mapTool.ID.Value))
                    {
                        ArcMap.Application.CurrentTool = null;
                    }
                    else
                    {
                        ArcMap.Application.CurrentTool = mapTool;
                    }

                    break;

                case 1:

                    break;
            }
        }
        
        private ObservationPoint GetObservationPoint()
        {
            return new ObservationPoint()
            {
                X = Convert.ToDouble(xCoord.Text),
                Y = Convert.ToDouble(yCoord.Text),
                Affiliation = cmbAffiliationEdit.SelectedItem.ToString(),
                AngelMaxH = Convert.ToDouble(angleOFViewMax.Text),
                AngelMinH = Convert.ToDouble(angleOFViewMin.Text),
                AngelCameraRotationH = Convert.ToDouble(cameraRotationH.Text),
                AngelCameraRotationV = Convert.ToDouble(cameraRotationV.Text),
                RelativeHeight = Convert.ToDouble(heightCurrent.Text),
                AvailableHeightLover = Convert.ToDouble(heightMin.Text),
                AvailableHeightUpper = Convert.ToDouble(heightMax.Text),
                AzimuthStart = Convert.ToDouble(azimuthB.Text),
                AzimuthEnd = Convert.ToDouble(azimuthE.Text),
                AzimuthMainAxis = Convert.ToDouble(azimuthMainAxis.Text),
                Dto = Convert.ToDateTime(observPointDate.Text),
                Operator = observPointCreator.Text,
                Title = observPointName.Text,
                Type = cmbObservTypesEdit.Text
            };
        }

        private void Filter_CheckedChanged(object sender, EventArgs e)
        {
            FilterColumns(GetFilter);
        }
    }
}
