using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Editor;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geometry;
using MilSpace.Core.Tools;
using MilSpace.DataAccess.DataTransfer;
using MilSpace.DataAccess.Facade;
using MilSpace.Tools;
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
        private ObservationPointsController _observPointsController;
        private VisibilitySessionsController _visibilitySessionsController;

        private BindingList<ObservPointGui> _observPointGuis = new BindingList<ObservPointGui>();
        private BindingList<VisibilitySessionGui> _visibilitySessionsGui = new BindingList<VisibilitySessionGui>();
        private BindingSource _observObjectsGui = new BindingSource();

        private bool _isDropDownItemChangedManualy = false;
        private bool _isFieldsChanged = false;

        public DockableWindowMilSpaceMVisibilitySt(object hook, ObservationPointsController controller)
        {
            InitializeComponent();
            this._observPointsController = controller;
            this._observPointsController.SetView(this);
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
            SubscribeForEvents();
            InitilizeData();
            _observPointsController.UpdateObservationPointsList();
        }

        private void SubscribeForEvents()
        {
            IEditEvents_Event editEvent = (IEditEvents_Event)ArcMap.Editor;
            editEvent.OnCreateFeature += _observPointsController.OnCreateFeature;

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
        private int _selectedPointId => Convert.ToInt32(dgvObservationPoints.SelectedRows[0].Cells["Id"].Value);
        private bool _isFieldsEnabled => xCoord.Enabled && yCoord.Enabled;

        public VeluableObservPointFieldsEnum GetFilter
        {
            get
            {
                var result = VeluableObservPointFieldsEnum.All;

                if (chckFilterAffiliation.Checked)
                {
                    result = result | VeluableObservPointFieldsEnum.Affiliation;
                }
                if (chckFilterDate.Checked)
                {
                    result = result | VeluableObservPointFieldsEnum.Date;
                }

                if (chckFilterType.Checked)
                {
                    result = result | VeluableObservPointFieldsEnum.Type;
                }

                return result;
            }
        }

        public string ObservationPointsFeatureClass => cmbObservPointsLayers.Text;

        public IEnumerable<string> GetTypes
        {
            get
            {
                return _observPointsController.GetObservationPointMobilityTypes();
            }
        }
        public IEnumerable<string> GetAffiliation
        {
            get
            {
                return _observPointsController.GetObservationPointTypes();
            }
        }

        public void FillObservationPointList(IEnumerable<ObservationPoint> observationPoints, VeluableObservPointFieldsEnum filter)
        {
            if (observationPoints.Any())
            {
                var ItemsToShow = observationPoints.Select(i => new ObservPointGui
                {
                    Title = i.Title,
                    Type = i.Type,
                    Affiliation = i.Affiliation,
                    Date = i.Dto.Value.ToShortDateString(),
                    Id = i.Objectid
                }).ToList();

                dgvObservationPoints.Rows.Clear();
                dgvObservationPoints.CurrentCell = null;
                _observPointGuis = new BindingList<ObservPointGui>(ItemsToShow);
                dgvObservationPoints.DataSource = _observPointGuis;

                SetDataGridView();
                DisplaySelectedColumns(filter);
                dgvObservationPoints.Update();
                dgvObservationPoints.Rows[0].Selected = true;
            }
        }

        public void FillObservationObjectsList(IEnumerable<ObservationObject> observationObjects)
        {
            if(observationObjects.Any())
            {
                dgvObservObjects.Rows.Clear();

                var itemsToShow = observationObjects.Select(i => new ObservObjectGui
                {
                    Title = i.Title,
                    Id = i.Id,
                    Affiliation = _observPointsController.GetObservObjectsTypeString(i.ObjectType),
                    Group = i.Group

                }).ToList();


                dgvObservObjects.CurrentCell = null;
                _observObjectsGui.DataSource = itemsToShow;
                dgvObservObjects.DataSource = _observObjectsGui;

                SetObservObjectsTableView();
                DisplayObservObjectsSelectedColumns();
                dgvObservObjects.Rows[0].Selected = true;
            }
        }

        public void ChangeRecord(int id, ObservationPoint observationPoint)
        {
            var rowIndex = dgvObservationPoints.SelectedRows[0].Index;
            var pointGui = _observPointGuis.FirstOrDefault(point => point.Id == id);

            pointGui.Title = observationPoint.Title;
            pointGui.Type = observationPoint.Type;
            pointGui.Affiliation = observationPoint.Affiliation;
            pointGui.Date = observationPoint.Dto.Value.ToShortDateString();

            dgvObservationPoints.Refresh();
            UpdateFilter(dgvObservationPoints.Rows[rowIndex]);
            _isFieldsChanged = false;
        }

        public void AddRecord(ObservationPoint observationPoint)
        {
            _observPointGuis.Add(new ObservPointGui
            {
                Title = observationPoint.Title,
                Type = observationPoint.Type,
                Affiliation = observationPoint.Affiliation,
                Date = observationPoint.Dto.Value.ToShortDateString(),
                Id = observationPoint.Objectid
            });

            dgvObservationPoints.Refresh();
            FilterData();

            if (dgvObservationPoints.Rows[dgvObservationPoints.Rows.Count - 1].Visible)
            {
                dgvObservationPoints.Rows[dgvObservationPoints.Rows.Count - 1].Selected = true;
            }
        }

        public void FillVisibilitySessionsList(IEnumerable<VisibilitySession> visibilitySessions)
        {
            if(visibilitySessions.Any())
            {
                dgvVisibilitySessions.Rows.Clear();

                foreach (var session in visibilitySessions)
                {
                    string state;

                    if(!session.Started.HasValue)
                    {
                        state = _visibilitySessionsController.GetStringForStateType(VisibilitySessionStateEnum.Pending);
                    }
                    else if(!session.Finished.HasValue)
                    {
                        state = _visibilitySessionsController.GetStringForStateType(VisibilitySessionStateEnum.Calculated);
                    }
                    else
                    {
                        state = _visibilitySessionsController.GetStringForStateType(VisibilitySessionStateEnum.Finished);
                    }

                    _visibilitySessionsGui.Add(new VisibilitySessionGui
                    {
                        Id = session.Id,
                        Name = session.Name,
                        State = state
                    });
                }

                dgvVisibilitySessions.CurrentCell = null;
                dgvVisibilitySessions.DataSource = _visibilitySessionsGui;
                SetVisibilitySessionsTableView();
                dgvObservationPoints.Rows[0].Selected = true;
            }
        }

        //private void PopulateStationsLayersComboBox()
        //{
        //    cmbObservStationLayers.Items.Clear();
        //    cmbObservStationLayers.Items.AddRange(_observPointsController.GetObservationStationsLayers().ToArray());
        //    if(cmbObservStationLayers.Items.Count > 0)
        //    {
        //        cmbObservStationLayers.SelectedItem = cmbObservStationLayers.Items[0];
        //    }
        //}

        private void OnSelectObserbPoint()
        {

        }

        private void OnItemAdded(object item)
        {
            EnableObservPointsControls();
            PopulatePointsLayersComboBox();
            SetObservObjectsControlsState(_observPointsController.IsObservObjectsExists(ActiveView));
        }

        private void OnContentsChanged()
        {
            EnableObservPointsControls();
            PopulatePointsLayersComboBox();
            SetCoordDefaultValues();
            SetObservObjectsControlsState(_observPointsController.IsObservObjectsExists(ActiveView));
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
                var controller = new ObservationPointsController(ArcMap.Document);

                m_windowUI = new DockableWindowMilSpaceMVisibilitySt(this.Hook, controller);
                return m_windowUI.Handle;
            }

            protected override void Dispose(bool disposing)
            {
                if (m_windowUI != null)
                    m_windowUI.Dispose(disposing);

                base.Dispose(disposing);
            }

        }


        internal void ArcMap_OnMouseDown(int x, int y)
        {
            if (!(this.Hook is IApplication arcMap) || !(arcMap.Document is IMxDocument currentDocument)) return;

            IPoint resultPoint = new Point();

            resultPoint = (currentDocument.FocusMap as IActiveView).ScreenDisplay.DisplayTransformation.ToMapPoint(x, y);
            resultPoint.ID = dgvObservationPoints.Rows.Count + 1;

            resultPoint.Project(EsriTools.Wgs84Spatialreference);

            xCoord.Text = resultPoint.X.ToString();
            yCoord.Text = resultPoint.Y.ToString();
            SavePoint();
        }

        internal void ArcMap_OnMouseMove(int x, int y)
        {
            //Place Mouce Move logic here if needed
        }


        #region ObservationPointsPrivateMethods

        private void SetDataGridView()
        {
            dgvObservationPoints.Columns["Title"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvObservationPoints.Columns["Id"].Visible = false;
        }

        private void DisplaySelectedColumns(VeluableObservPointFieldsEnum filter)
        {
            dgvObservationPoints.Columns["Affiliation"].Visible = chckFilterAffiliation.Checked;
            dgvObservationPoints.Columns["Type"].Visible = chckFilterType.Checked;
            dgvObservationPoints.Columns["Date"].Visible = chckFilterDate.Checked;
        }

        private void FilterData()
        {
            if(dgvObservationPoints.Rows.Count == 0)
            {
                return;
            }

            dgvObservationPoints.CurrentCell = null;

            foreach(DataGridViewRow row in dgvObservationPoints.Rows)
            {
                CheckRowForFilter(row);
            }

            if(dgvObservationPoints.FirstDisplayedScrollingRowIndex != -1)
            {
                dgvObservationPoints.Rows[dgvObservationPoints.FirstDisplayedScrollingRowIndex].Selected = true;
                if(!_isFieldsEnabled) EnableObservPointsControls();
            }
            else
            {
                EnableObservPointsControls(true);
            }

        }

        private void CheckRowForFilter(DataGridViewRow row)
        {
            if(cmbAffiliation.SelectedItem != null && cmbAffiliation.SelectedItem.ToString() != _observPointsController.GetAllAffiliationType())
            {
                row.Visible = (row.Cells["Affiliation"].Value.ToString() == cmbAffiliation.SelectedItem.ToString());
                if(!row.Visible) return;
            }

            if(cmbObservPointType.SelectedItem != null && cmbObservPointType.SelectedItem.ToString() != _observPointsController.GetAllMobilityType())
            {
                row.Visible = (row.Cells["Type"].Value.ToString() == cmbObservPointType.SelectedItem.ToString());
                return;
            }

            row.Visible = true;
        }

        private void InitilizeData()
        {
            cmbObservPointType.Items.Clear();
            cmbObservTypesEdit.Items.Clear();
            var filters = new List<string>();
            filters.AddRange(GetTypes.ToArray());

            cmbObservPointType.Items.AddRange(filters.ToArray());
            cmbObservPointType.Items.Add(_observPointsController.GetAllMobilityType());
            cmbObservTypesEdit.Items.AddRange(GetTypes.ToArray());

            filters = new List<string>();

            filters.AddRange(GetAffiliation.ToArray());
            cmbAffiliation.Items.Clear();
            cmbAffiliationEdit.Items.Clear();

            cmbAffiliation.Items.AddRange(filters.ToArray());
            cmbAffiliation.Items.Add(_observPointsController.GetAllAffiliationType());
            cmbAffiliationEdit.Items.AddRange(GetAffiliation.ToArray());

            SetDefaultValues();
        }

        private void PopulatePointsLayersComboBox()
        {
            if(cmbObservPointsLayers.Visible)
            {
                cmbObservPointsLayers.Items.Clear();
                cmbObservPointsLayers.Items.AddRange(_observPointsController.GetObservationPointsLayers(ActiveView).ToArray());
                cmbObservPointsLayers.SelectedItem = _observPointsController.GetObservPointFeatureName();
            }
        }

        private void SetDefaultValues()
        {
            _isDropDownItemChangedManualy = false;

            cmbObservTypesEdit.SelectedItem = ObservationPointMobilityTypesEnum.Stationary.ToString();
            cmbAffiliationEdit.SelectedItem = ObservationPointTypesEnum.Enemy.ToString();
            cmbObservPointType.SelectedItem = _observPointsController.GetAllMobilityType();
            cmbAffiliation.SelectedItem = _observPointsController.GetAllAffiliationType();

            _isDropDownItemChangedManualy = true;

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
            var centerPoint = _observPointsController.GetEnvelopeCenterPoint(ArcMap.Document.ActiveView.Extent);
            xCoord.Text = centerPoint.X.ToString();
            yCoord.Text = centerPoint.Y.ToString();
        }

        private void OnFieldChanged(object sender, EventArgs e)
        {
            if(!_isFieldsChanged || !_isFieldsEnabled)
            {
                return;
            }

            var selectedPoint = _observPointsController.GetObservPointById(_selectedPointId);

            if(FieldsValidation(sender, selectedPoint))
            {
                _observPointsController.UpdateObservPoint(GetObservationPoint(), cmbObservPointsLayers.SelectedItem.ToString(), ActiveView, selectedPoint.Objectid);
            }
        }

        private bool FieldsValidation(object sender, ObservationPoint point)
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
                            xCoord.Text = point.X.ToString();

                            return false;
                        }
                        else
                        {
                            var x = Convert.ToDouble(xCoord.Text);
                            var y = Convert.ToDouble(yCoord.Text);
                        }

                        return true;

                    case "yCoord":

                        if(!Regex.IsMatch(yCoord.Text, @"^([-]?[\d]{1,2}\,\d+)$"))
                        {
                            MessageBox.Show("Invalid data.\nInsert the coordinates in the WGS84 format.");
                            yCoord.Text = point.Y.ToString();

                            return false;
                        }
                        else
                        {
                            var x = Convert.ToDouble(xCoord.Text);
                            var y = Convert.ToDouble(yCoord.Text);
                        }

                        return true;

                    case "angleOFViewMin":

                        return ValidateRange(angleOFViewMin, point.AngelMinH.ToString(), -90, 0);

                    case "angleOFViewMax":

                        return ValidateRange(angleOFViewMax, point.AngelMaxH.ToString(), 0, 90);

                    case "azimuthB":

                        return ValidateAzimuth(textBox, point.AzimuthStart.ToString());

                    case "azimuthE":

                        return ValidateAzimuth(textBox, point.AzimuthEnd.ToString());

                    case "azimuthMainAxis":

                        return ValidateAzimuth(textBox, point.AzimuthMainAxis.ToString());

                    case "cameraRotationH":

                        return ValidateAzimuth(textBox, point.AngelCameraRotationH.ToString());

                    case "cameraRotationV":

                        return ValidateAzimuth(textBox, point.AngelCameraRotationV.ToString());

                    case "heightCurrent":

                        var currentHeight = ValidateHeight(textBox, point.RelativeHeight.ToString());

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

                            return true;
                        }

                        return false;

                    case "heightMin":

                        var minHeightChanged = ValidateHeight(textBox, point.AvailableHeightLover.ToString());

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

                            return true;
                        }

                        return false;

                    case "heightMax":

                        var maxHeightChanged = ValidateHeight(textBox, point.AvailableHeightUpper.ToString());

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

                            return true;
                        }

                        return false;

                    default:

                        return true;
                }
            }

            catch(Exception ex) { return false; }
        }

        private bool ValidateAzimuth(TextBox azimuthTextBox, string defaultValue)
        {
            return ValidateRange(azimuthTextBox, defaultValue, 0, 360);
        }

        private bool ValidateRange(TextBox textBox, string defaultValue, double lowValue, double upperValue)
        {
            double value;

            if(Double.TryParse(textBox.Text, out value))
            {
                if(value >= lowValue && value <= upperValue)
                {
                    return true;
                }
            }

            textBox.Text = defaultValue;
            MessageBox.Show($"Invalid data.\nInsert the value in the range from {lowValue} to {upperValue}");

            return false;
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

        private void EnableObservPointsControls(bool isAllDisabled = false)
        {
            lblLayer.Visible = cmbObservPointsLayers.Visible = cmbAffiliationEdit.Enabled = cmbObservTypesEdit.Enabled = azimuthB.Enabled
                = azimuthE.Enabled = xCoord.Enabled = yCoord.Enabled = angleOFViewMin.Enabled = angleOFViewMax.Enabled
                = heightCurrent.Enabled = heightMin.Enabled = azimuthMainAxis.Enabled = cameraRotationH.Enabled = cameraRotationV.Enabled
                = heightMax.Enabled = observPointName.Enabled = tlbCoordinates.Enabled = (_observPointsController.IsObservPointsExists(ActiveView) && !isAllDisabled);

            angleFrameH.Enabled = angleFrameV.Enabled = observPointDate.Enabled = observPointCreator.Enabled = false;
            tlbObservPoints.Buttons["tlbbRemovePoint"].Enabled = tlbObservPoints.Buttons["tlbbShowPoint"].Enabled = (dgvObservationPoints.SelectedRows.Count != 0 && !isAllDisabled);
        }


        private void RemovePoint()
        {
            var result = MessageBox.Show("Do you realy want to remove point?", "SPPRD", MessageBoxButtons.OKCancel);

            if(result == DialogResult.OK)
            {
                var rowIndex = dgvObservationPoints.SelectedRows[0].Index;

                _observPointsController.RemoveObservPoint(cmbObservPointsLayers.SelectedItem.ToString(), ActiveView, _selectedPointId);
                _observPointGuis.Remove(_observPointGuis.First(point => point.Id == _selectedPointId));

                if(rowIndex < dgvObservationPoints.Rows.Count)
                {
                    UpdateFilter(dgvObservationPoints.Rows[rowIndex]);
                }
            }
        }

        private void SavePoint()
        {
            var selectedPoint = _observPointsController.GetObservPointById(_selectedPointId);
            _observPointsController.UpdateObservPoint(GetObservationPoint(), cmbObservPointsLayers.SelectedItem.ToString(), ActiveView, selectedPoint.Objectid);
        }

        private void CreateNewPoint(ObservationPoint point)
        {
            _observPointsController.AddPoint(cmbObservPointsLayers.SelectedItem.ToString(), ActiveView);
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

        private void UpdateFilter(DataGridViewRow row)
        {
            dgvObservationPoints.CurrentCell = null;

            CheckRowForFilter(row);

            if(row.Visible)
            {
                row.Selected = true;
            }
            else
            {
                if(dgvObservationPoints.FirstDisplayedScrollingRowIndex != -1)
                {
                    dgvObservationPoints.Rows[dgvObservationPoints.FirstDisplayedScrollingRowIndex].Selected = true;
                    if(!_isFieldsEnabled) EnableObservPointsControls();
                }
                else
                {
                    EnableObservPointsControls(true);
                }
            }
        }

        private void FillFields(ObservationPoint selectedPoint)
        {
            _isDropDownItemChangedManualy = false;

            cmbObservTypesEdit.SelectedItem = selectedPoint.Type.ToString();
            cmbAffiliationEdit.SelectedItem = selectedPoint.Affiliation.ToString();

            _isDropDownItemChangedManualy = true;

            var centerPoint = _observPointsController.GetEnvelopeCenterPoint(ArcMap.Document.ActiveView.Extent);

            xCoord.Text = selectedPoint.X != null ? selectedPoint.X.ToString() : centerPoint.X.ToString();
            yCoord.Text = selectedPoint.Y != null ? selectedPoint.Y.ToString() : centerPoint.Y.ToString();
            azimuthB.Text = selectedPoint.AzimuthStart != null ? selectedPoint.AzimuthStart.ToString() : ObservPointDefaultValues.AzimuthBText;
            azimuthE.Text = selectedPoint.AzimuthEnd != null ? selectedPoint.AzimuthEnd.ToString() : ObservPointDefaultValues.AzimuthEText;
            heightCurrent.Text = selectedPoint.RelativeHeight != null ? selectedPoint.RelativeHeight.ToString() : ObservPointDefaultValues.RelativeHeightText;
            heightMin.Text = selectedPoint.AvailableHeightLover.ToString();
            heightMax.Text = selectedPoint.AvailableHeightUpper.ToString();
            observPointName.Text = selectedPoint.Title;
            angleOFViewMin.Text = selectedPoint.AngelMinH != null ? selectedPoint.AngelMinH.ToString() : ObservPointDefaultValues.AngleOFViewMinText;
            angleOFViewMax.Text = selectedPoint.AngelMaxH != null ? selectedPoint.AngelMaxH.ToString() : ObservPointDefaultValues.AngleOFViewMaxText;
            angleFrameH.Text = selectedPoint.AngelFrameH != null ? selectedPoint.AngelFrameH.ToString() : ObservPointDefaultValues.AngleFrameHText;
            angleFrameV.Text = selectedPoint.AngelFrameV != null ? selectedPoint.AngelFrameV.ToString() : ObservPointDefaultValues.AngleFrameVText;
            cameraRotationH.Text = selectedPoint.AngelCameraRotationH != null ? selectedPoint.AngelCameraRotationH.ToString() : ObservPointDefaultValues.CameraRotationHText;
            cameraRotationV.Text = selectedPoint.AngelCameraRotationV != null ? selectedPoint.AngelCameraRotationV.ToString() : ObservPointDefaultValues.CameraRotationVText;
            azimuthMainAxis.Text = selectedPoint.AzimuthMainAxis != null ? selectedPoint.AzimuthMainAxis.ToString() : ObservPointDefaultValues.AzimuthMainAxisText;

            observPointDate.Text = selectedPoint.Dto.Value.ToShortDateString();
            observPointCreator.Text = selectedPoint.Operator;
        }

        #endregion

        #region VisibilitySessionsPrivateMethods

        private void SetVisibilitySessionsTableView()
        {
            dgvVisibilitySessions.Columns["Id"].Visible = false;
            dgvVisibilitySessions.Columns["Name"].HeaderText = "Название";
            dgvVisibilitySessions.Columns["Name"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvVisibilitySessions.Columns["State"].HeaderText = "Состояние";
            dgvVisibilitySessions.Columns["State"].Width = 100;
        }

        private void SetVisibilitySessionsController()
        {
            var controller = new VisibilitySessionsController();
            controller.SetView(this);
            _visibilitySessionsController = controller;
        }

        private void FillVisibilitySessionFields(VisibilitySession session)
        {
            tbVisibilitySessionName.Text = session.Name;
            tbVisibilitySessionCreator.Text = session.UserName;
            tbVisibilitySessionCreated.Text = session.Created.Value.ToLongDateString();
            tbVisibilitySessionStarted.Text = session.Started.HasValue ? session.Started.Value.ToLongDateString() : string.Empty;
            tbVisibilitySessionFinished.Text = session.Finished.HasValue ? session.Finished.Value.ToLongDateString() : string.Empty;
        }

        private void PopulateVisibilityComboBoxes()
        {
            cmbStateFilter.Items.Clear();
            cmbStateFilter.Items.AddRange(_visibilitySessionsController.GetVisibilitySessionStateTypes().ToArray());
            cmbStateFilter.SelectedItem = _visibilitySessionsController.GetStringForStateType(VisibilitySessionStateEnum.All);
        }

        private void FilterVisibilityList()
        {
            if(dgvVisibilitySessions.Rows.Count == 0)
            {
                return;
            }

            dgvVisibilitySessions.CurrentCell = null;

            foreach(DataGridViewRow row in dgvVisibilitySessions.Rows)
            {
                row.Visible = row.Cells["State"].Value.ToString() == cmbStateFilter.SelectedItem.ToString()
                || cmbStateFilter.SelectedItem.ToString() == _visibilitySessionsController.GetStringForStateType(VisibilitySessionStateEnum.All);
            }

            if(dgvVisibilitySessions.FirstDisplayedScrollingRowIndex != -1)
            {
                dgvVisibilitySessions.Rows[dgvVisibilitySessions.FirstDisplayedScrollingRowIndex].Selected = true;
            }
            else
            {
                tlbVisibilitySessions.Buttons["removeTask"].Enabled = false;
            }
        }

        #endregion

        #region ObservationObjectsPrivateMethods

        private void SetObservObjectsTableView()
        {
            dgvObservObjects.Columns["Id"].Visible = false;
            dgvObservObjects.Columns["Title"].HeaderText = "Название";
            dgvObservObjects.Columns["Title"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvObservObjects.Columns["Affiliation"].HeaderText = "Принадлежность";
            dgvObservObjects.Columns["Affiliation"].Width = 100;
            dgvObservObjects.Columns["Group"].HeaderText = "Группа";
            dgvObservObjects.Columns["Group"].Width = 100;

            dgvObservObjects.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void DisplayObservObjectsSelectedColumns()
        {
            dgvObservObjects.Columns["Affiliation"].Visible = chckObservObjAffiliation.Checked;
            dgvObservObjects.Columns["Title"].Visible = chckObservObjTitle.Checked;
            dgvObservObjects.Columns["Group"].Visible = chckObservObjGroup.Checked;
        }

        private void PopulateObservObjectsComboBoxes()
        {
            cmbObservObjAffiliationFilter.Items.Clear();
            cmbObservObjAffiliationFilter.Items.AddRange(_observPointsController.GetObservationObjectTypes().ToArray());
            cmbObservObjAffiliationFilter.SelectedItem = _observPointsController.GetObservObjectsTypeString(ObservationObjectTypesEnum.All);
        }

        private void FilterObservObjects()
        {
            if(dgvObservObjects.Rows.Count == 0)
            {
                return;
            }

            dgvObservObjects.CurrentCell = null;

            foreach(DataGridViewRow row in dgvObservObjects.Rows)
            {
                row.Visible = row.Cells["Affiliation"].Value.ToString() == cmbObservObjAffiliationFilter.SelectedItem.ToString()
                 || cmbObservObjAffiliationFilter.SelectedItem.ToString() == _observPointsController.GetObservObjectsTypeString(ObservationObjectTypesEnum.All);
            }

            if(dgvObservObjects.FirstDisplayedScrollingRowIndex != -1)
            {
                dgvObservObjects.Rows[dgvObservObjects.FirstDisplayedScrollingRowIndex].Selected = true;
            }
            else
            {
                ClearObservObjectFields();
            }
        }

        private void FillObservObjectFields(ObservationObject observObject)
        {
            tbObservObjTitle.Text = observObject.Title;
            tbObservObjGroup.Text = observObject.Group;
            tbObservObjAffiliation.Text = _observPointsController.GetObservObjectsTypeString(observObject.ObjectType);
            tbObservObjDate.Text = observObject.DTO.ToLongDateString();
        }

        private void ClearObservObjectFields()
        {
            tbObservObjTitle.Text = string.Empty;
            tbObservObjGroup.Text = string.Empty;
            tbObservObjAffiliation.Text = string.Empty;
            tbObservObjDate.Text = string.Empty;
        }

        private void SetObservObjectsControlsState(bool isObservObjectsExist)
        {
            if (!isObservObjectsExist)
            {
               dgvObservObjects.Rows.Clear();
            }
            else
            {
                _observPointsController.UpdateObservObjectsList();
            }

            addNewObjectPanel.Enabled = isObservObjectsExist;
            cmbObservObjAffiliationFilter.Enabled = isObservObjectsExist;
            chckObservObjColumnsVisibilityPanel.Enabled = isObservObjectsExist;
            tbObservObjects.Buttons["tlbbAddObservObjLayer"].Enabled = !isObservObjectsExist;
        }

        #endregion

        private void MainTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(mainTabControl.SelectedTab.Name == "tbpSessions")
            {
                if(_visibilitySessionsController == null)
                {
                    SetVisibilitySessionsController();
                    PopulateVisibilityComboBoxes();
                    _visibilitySessionsController.UpdateVisibilitySessionsList();

                    if(dgvVisibilitySessions.Rows.Count == 0)
                    {
                        tlbVisibilitySessions.Buttons["removeTask"].Enabled = false;
                    }
                }
            }

            if(mainTabControl.SelectedTab.Name == "tbpObservObjects")
            {
                if(cmbObservObjAffiliationFilter.Items.Count == 0)
                {
                    if(_observPointsController.IsObservObjectsExists(ActiveView))
                    {
                        PopulateObservObjectsComboBoxes();
                        _observPointsController.UpdateObservObjectsList();
                        SetObservObjectsControlsState(true);
                    }
                    else
                    {
                        SetObservObjectsControlsState(false);
                    }
                }
            }
        }

        #region ObservationPointsTabEvents

      
        private void TlbObserPoints_ButtonClick(object sender, ToolBarButtonClickEventArgs e)
        {
            switch(e.Button.Name)
            {

                case "tlbbAddNewPoint":

                    CreateNewPoint(GetObservationPoint());

                    break;

                case "tlbbRemovePoint":

                    RemovePoint();

                    break;


                case "tlbbShowPoint":

                    _observPointsController.ShowObservPoint(ActiveView, _selectedPointId);

                    break;
            }

        }

        private void TlbCoordinates_ButtonClick(object sender, ToolBarButtonClickEventArgs e)
        {
            switch (e.Button.Name)
            {
                case "tlbbGetCoord":

                    UID mapToolID = new UIDClass
                    {
                        Value = ThisAddIn.IDs.MapInteropTool
                    };
                    var documentBars = ArcMap.Application.Document.CommandBars;
                    var mapTool = documentBars.Find(mapToolID, false, false);

                    if (ArcMap.Application.CurrentTool?.ID?.Value != null && ArcMap.Application.CurrentTool.ID.Value.Equals(mapTool.ID.Value))
                    {
                        ArcMap.Application.CurrentTool = null;
                    }
                    else
                    {
                        ArcMap.Application.CurrentTool = mapTool;
                    }

                    break;

                case "tlbbCopyCoord":

                    Clipboard.Clear();
                    Clipboard.SetText($"{xCoord.Text};{yCoord.Text}");

                    break;

                case "tlbbPasteCoord":

                    var clipboard = Clipboard.GetText();
                    if (string.IsNullOrWhiteSpace(clipboard)) return;

                    if (Regex.IsMatch(clipboard, @"^([-]?[\d]{1,2}[\,|\.]\d+);([-]?[\d]{1,2}[\,|\.]\d+)$"))
                    {
                        clipboard.Replace('.', ',');
                        var coords = clipboard.Split(';');
                        xCoord.Text = coords[0];
                        yCoord.Text = coords[1];

                        _observPointsController.UpdateObservPoint(GetObservationPoint(), cmbObservPointsLayers.SelectedItem.ToString(), ActiveView, _selectedPointId);
                    }
                    else
                    {
                        MessageBox.Show("Invalid format");
                    }

                    break;

                case "tlbbShowCoord":

                    _observPointsController.ShowObservPoint(ActiveView, _selectedPointId);

                    break;
            }
        }

        private void Filter_CheckedChanged(object sender, EventArgs e)
        {
            DisplaySelectedColumns(GetFilter);
        }

        private void DgvObservationPoints_SelectionChanged(object sender, EventArgs e)
        {
            _isFieldsChanged = false;

            if (dgvObservationPoints.SelectedRows.Count == 0)
            {
                EnableObservPointsControls(true);
                return;
            }

            if (!_isFieldsEnabled) EnableObservPointsControls();
            var selectedPoint = _observPointsController.GetObservPointById(_selectedPointId);

            if (selectedPoint == null)
            {
                return;
            }

            FillFields(selectedPoint);
        }


        private void FilterComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterData();
        }

        private void EditComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (dgvObservationPoints.SelectedRows.Count == 0 || !_isDropDownItemChangedManualy)
            {
                return;
            }

            SavePoint();
        }

        private void Fields_TextChanged(object sender, EventArgs e)
        {
            _isFieldsChanged = true;
        }

        #endregion

        #region VisibilitySessionsTabEvents

        private void TlbVisiilitySessions_ButtonClick(object sender, ToolBarButtonClickEventArgs e)
        {
            if(e.Button == removeTask)
            {
                var result = MessageBox.Show("Do you really want to delete this session?", "SPPRD", MessageBoxButtons.OKCancel);

                if(result == DialogResult.OK)
                {
                    var id = dgvVisibilitySessions.SelectedRows[0].Cells["Id"].Value.ToString();
                    var rowIndex = dgvVisibilitySessions.SelectedRows[0].Index;

                    if(!_visibilitySessionsController.RemoveSession(id))
                    {
                        MessageBox.Show("Unable to delete session");
                        return;
                    }
                    
                    _visibilitySessionsGui.Remove(_visibilitySessionsGui.First(session => session.Id == id));

                    if(cmbStateFilter.SelectedItem.ToString() != _visibilitySessionsController.GetStringForStateType(VisibilitySessionStateEnum.All))
                    {
                        FilterVisibilityList();
                    }
                }
            }
            else if(e.Button == wizardTask)
            {
                var wizard = (new WindowMilSpaceMVisibilityMaster(ObservationPointsFeatureClass, _observPointsController.GetObservationStationLayerName));
                wizard.ShowDialog();
                var dialogResult = wizard.DialogResult;

                if (dialogResult == DialogResult.OK)
                {
                    var calcParams = wizard.FinalResult;

                    var clculated = _observPointsController.CalculateVisibility(calcParams.RasterLayerName, VisibilityManager.GenerateResultId(), 
                            VisibilitySession.DefaultResultsSet, calcParams.ObservPointIDs, calcParams.ObservObjectIDs);

                    if (!clculated)
                    {
                        //Localize message
                        MessageBox.Show("The calculation finished with errors.\nFor more details go to the log file", "SPPRD", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    _visibilitySessionsController.UpdateVisibilitySessionsList();
                    FilterVisibilityList();
                }
            }
        }

        private void DgvVisibilitySessions_SelectionChanged(object sender, EventArgs e)
        {
            if(dgvVisibilitySessions.SelectedRows.Count == 0)
            {
                tlbVisibilitySessions.Buttons["removeTask"].Enabled = false;
                return;
            }

            var selectedSessionId = dgvVisibilitySessions.SelectedRows[0].Cells["Id"].Value.ToString();
            var selectedSession = _visibilitySessionsController.GetSession(selectedSessionId);

            if(selectedSession != null)
            {
                FillVisibilitySessionFields(selectedSession);
            }

            tlbVisibilitySessions.Buttons["removeTask"].Enabled = true;
        }

        private void CmbStateFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterVisibilityList();
        }

        #endregion

        #region ObservationObjectsTabEvents

        private void CmbObservObjAffiliationFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbObservObjAffiliationFilter.SelectedItem != null)
            {
                FilterObservObjects();
            }
        }

        private void DgvObservObjects_SelectionChanged(object sender, EventArgs e)
        {
            if(dgvObservObjects.SelectedRows.Count == 0 || dgvObservObjects.SelectedRows[0].Cells["Id"].Value == null)
            {
                ClearObservObjectFields();
                return;
            }

            var selectedObject = _observPointsController.GetObservObjectById(dgvObservObjects.SelectedRows[0].Cells["Id"].Value.ToString());

            if(selectedObject != null)
            {
                FillObservObjectFields(selectedObject);
            }
        }

        private void TbObservObjects_ButtonClick(object sender, ToolBarButtonClickEventArgs e)
        {
            switch(e.Button.Name)
            {
                case "tlbbAddObservObjLayer":

                    _observPointsController.AddObservObjectsLayer(ActiveView);

                    break;
            }
        }

        private void ChckObservObj_CheckedChanged(object sender, EventArgs e)
        {
            dgvObservObjects.Columns["Title"].Visible = chckObservObjTitle.Checked;
            dgvObservObjects.Columns["Affiliation"].Visible = chckObservObjAffiliation.Checked;
            dgvObservObjects.Columns["Group"].Visible = chckObservObjGroup.Checked;
        }

        private void toolBar6_ButtonClick(object sender, ToolBarButtonClickEventArgs e)
        {
        }
    }
}
#endregion