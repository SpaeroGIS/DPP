using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Editor;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geometry;
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

            ArcMap.Events.OpenDocument += delegate () {
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

                if (chckFilterAffiliation.Checked)
                {
                    result = result | VeluableObservPointFieldsEnum.Affiliation;
                }
                if (chckFilterDate.Checked)
                {
                    result = result | VeluableObservPointFieldsEnum.Date;
                }

                if (chckFilterId.Checked)
                {
                    result = result | VeluableObservPointFieldsEnum.Id;
                }
                if (chckFilterType.Checked)
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
       public  IEnumerable<string> GetAffiliation
        {
            get
            {
                return controller.GetObservationPointTypes();
            }
        }

        public void FillObservationPointList(IEnumerable<ObservationPoint> observationPoints, VeluableObservPointFieldsEnum filter)
        {

            lstObservationPoinst.Items.Clear();

            if (observationPoints.Any())
            {
                var ItemsToShow = observationPoints.Select(i => new ObservPointGui
                {
                    Text = i.GetItemValue(filter),
                    Id = i.Id
                }).ToList();

                BindingList<ObservPointGui> observPointGuis = new BindingList<ObservPointGui>();
                lstObservationPoinst.DataSource = ItemsToShow;
                lstObservationPoinst.DisplayMember = "Text";
                lstObservationPoinst.Update();
            }
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
            cmbObservPointsLayers.Items.Clear();

            cmbAffiliation.Items.AddRange(filters.ToArray());
            cmbAffiliationEdit.Items.AddRange(GetAffiliation.ToArray());
            cmbObservPointsLayers.Items.AddRange(controller.GetObservationPointsLayers(ActiveView).ToArray());
        }


        private void SetDefaultValues()
        {
            cmbObservPointsLayers.SelectedItem = controller.GetObservFeatureName();
            cmbObservTypesEdit.SelectedItem = ObservationPointMobilityTypesEnum.Stationary.ToString();
            cmbAffiliationEdit.SelectedItem = ObservationPointTypesEnum.Enemy.ToString();

            var centerPoint = controller.GetEnvelopeCenterPoint(ArcMap.Document.ActiveView.Extent);
            xCoord.Text = centerPoint.X.ToString();
            yCoord.Text = centerPoint.Y.ToString();

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


        private void FieldsValidation()
        {

        }

        private void EnableObservPointsControls()
        {
            cmbAffiliationEdit.Enabled = cmbObservTypesEdit.Enabled = azimuthB.Enabled = azimuthE.Enabled 
                = xCoord.Enabled = yCoord.Enabled = heightCurrent.Enabled = heightMin.Enabled = azimuthMainAxis.Enabled 
                = heightMax.Enabled = observPointName.Enabled = controller.IsObservPointsExists(ActiveView);
            angleFrameH.Enabled = angleFrameV.Enabled = angleOFViewMin.Enabled = angleOFViewMax.Enabled 
                = observPointDate.Enabled = observPointCreator.Enabled = false;
        }

        private void OnSelectObserbPoint()
        {

        }

        private void OnItemAdded(object item)
        {
            EnableObservPointsControls();
        }

        private void OnContentsChanged()
        {
            EnableObservPointsControls();
            SetDefaultValues();
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
                if (m_windowUI != null)
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

                    break;

            }

        }

        internal void ArcMap_OnMouseDown(int x, int y)
        {
            if (!(this.Hook is IApplication arcMap) || !(arcMap.Document is IMxDocument currentDocument)) return;            

            IPoint resultPoint = new Point();

            resultPoint = (currentDocument.FocusMap as IActiveView).ScreenDisplay.DisplayTransformation.ToMapPoint(x, y);

            AddPointToMap(resultPoint);

            xCoord.Text = resultPoint.X.ToString();
            yCoord.Text = resultPoint.Y.ToString();
        }

        internal void ArcMap_OnMouseMove(int x, int y)
        {
            //Place Mouce Move logic here if needed
        }

        private void AddPointToMap(IPoint point)
        {
            if (point != null && !point.IsEmpty)
            {
                var color = (IColor)new RgbColorClass() { Green = 255 };
                var placedPoint = ArcMapHelper.AddGraphicToMap(point, color, true, esriSimpleMarkerStyle.esriSMSDiamond, 7);                
            }
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

        private void label18_Click(object sender, EventArgs e)
        {

        }

        private void label15_Click(object sender, EventArgs e)
        {

        }

        private void angleMax_TextChanged(object sender, EventArgs e)
        {

        }

        private void angleOFView_TextChanged(object sender, EventArgs e)
        {

        }

        private void panel16_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel15_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel17_Paint(object sender, PaintEventArgs e)
        {

        }

        private void azimuthB_TextChanged(object sender, EventArgs e)
        {

        }

        private void azimuthE_TextChanged(object sender, EventArgs e)
        {

        }

        private void angleFrameH_TextChanged(object sender, EventArgs e)
        {

        }

        private void cameraRotationV_TextChanged(object sender, EventArgs e)
        {

        }

        private void panel14_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel57_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label58_Click(object sender, EventArgs e)
        {

        }

        private void label13_Click(object sender, EventArgs e)
        {

        }

        private void label57_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void cmbObservTypesEdit_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void xCoord_TextChanged(object sender, EventArgs e)
        {

        }

        private ObservationPoint GetObservationPoint()
        {
            return new ObservationPoint()
                        {
                            X = Convert.ToDouble(xCoord.Text),
                            Y = Convert.ToDouble(yCoord.Text),
                            Affiliation = cmbAffiliationEdit.SelectedItem.ToString(),
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

        #region Validation

        private void XCoord_Leave(object sender, EventArgs e)
        {
            if(!Regex.IsMatch(xCoord.Text, @"^([-]?[\d]{1,2}\.\d+)$"))
            {
                MessageBox.Show("Invalid data.\nEnter the coordinates in the WGS84 format.");
                var centerPoint = controller.GetEnvelopeCenterPoint(ArcMap.Document.ActiveView.Extent);
                xCoord.Text = centerPoint.X.ToString();
            }
        }

        private void YCoord_Leave(object sender, EventArgs e)
        {
            if(!Regex.IsMatch(yCoord.Text, @"^([-]?[\d]{1,2}\.\d+)$"))
            {
                MessageBox.Show("Invalid data.\n Enter the coordinates in the WGS84 format.");
                var centerPoint = controller.GetEnvelopeCenterPoint(ArcMap.Document.ActiveView.Extent);
                yCoord.Text = centerPoint.Y.ToString();
            }
        }
        
        private void AzimuthB_Leave(object sender, EventArgs e)
        {
            var azimuth = Convert.ToInt32(azimuthB.Text);
            if(azimuth < 0 || azimuth > 360)
            {
                azimuthB.Text = ObservPointDefaultValues.AzimuthBText;
            }
        }

        #endregion
    }
}
