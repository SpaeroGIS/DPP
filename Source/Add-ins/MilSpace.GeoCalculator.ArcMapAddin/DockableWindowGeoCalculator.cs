using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.SystemUI;
using MilSpace.GeoCalculator.BusinessLogic;
using MilSpace.GeoCalculator.BusinessLogic.Interfaces;
using MilSpace.GeoCalculator.BusinessLogic.Models;
using MilSpace.GeoCalculator.BusinessLogic.ReferenceData;
using MilSpace.GeoCalculator.BusinessLogic.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Desktop.AddIns;
using ESRI.ArcGIS.esriSystem;
using System.Linq;
using System.Reflection;

namespace ArcMapAddin
{
    /// <summary>
    /// Designer class of the dockable window add-in. It contains user interfaces that
    /// make up the dockable window.
    /// </summary>
    [Guid("CBD4D23F-9477-493E-B9D3-ADFC0753E38E")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("ArcMapAddin_DockableWindowGeoCalculator")]
    public partial class DockableWindowGeoCalculator : UserControl
    {
        private readonly IBusinessLogic _businessLogic;
        private readonly ProjectionsModel _projectionsModel;
        private List<PointModel> _pointModels = new List<PointModel>();
        private LocalizationContext context;
        private readonly List<IPoint> ClickedPointsList = new List<IPoint>();

        public ISpatialReference FocusMapSpatialReference => ArcMap.Document.FocusMap.SpatialReference;

        public DockableWindowGeoCalculator(object hook, IBusinessLogic businessLogic, ProjectionsModel projectionsModel)
        {
            InitializeComponent();
            this.Hook = hook;
            _businessLogic = businessLogic ?? throw new ArgumentNullException(nameof(businessLogic));
            _projectionsModel = projectionsModel ?? throw new ArgumentNullException(nameof(projectionsModel));

            LocalizeComponents();
        }

        /// <summary>
        /// Host object of the dockable window
        /// </summary>
        private object Hook
        {
            get;
            set;
        }

        #region AddInImpl
        /// <summary>
        /// Implementation class of the dockable window add-in. It is responsible for 
        /// creating and disposing the user interface class of the dockable window.
        /// </summary>
        public class AddinImpl : ESRI.ArcGIS.Desktop.AddIns.DockableWindow
        {
            private DockableWindowGeoCalculator m_windowUI;

            public AddinImpl()
            {
            }
            internal DockableWindowGeoCalculator UI
            {
                get { return m_windowUI; }
            }

            protected override IntPtr OnCreateChild()
            {
                if (this.Hook is IApplication arcMap)
                {
                    m_windowUI = new DockableWindowGeoCalculator(this.Hook, new BusinessLogic(arcMap, new DataExport()),
                                                                        CreateProjecstionsModelFromSettings());
                    return m_windowUI.Handle;
                }
                else return IntPtr.Zero;
            }

            protected override void Dispose(bool disposing)
            {
                if (m_windowUI != null)
                    m_windowUI.Dispose(disposing);

                base.Dispose(disposing);
            }

        }
        #endregion

        #region UserControl events handlers
        private void MapPointToolButton_Click(object sender, EventArgs e)
        {
            UID mapToolID = new UIDClass
            {
                Value = ThisAddIn.IDs.MapInteropTool
            };
            var documentBars = ArcMap.Application.Document.CommandBars;
            var mapTool = documentBars.Find(mapToolID, false, false);

            if (ArcMap.Application.CurrentTool?.ID?.Value != null && ArcMap.Application.CurrentTool.ID.Value.Equals(mapTool.ID.Value))
                ArcMap.Application.CurrentTool = null;
            else
                ArcMap.Application.CurrentTool = mapTool;
        }

        private async void SaveButton_Click(object sender, EventArgs e)
        {
            if (_pointModels == null || !_pointModels.Any()) MessageBox.Show("Please select a point on the map.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            var folderBrowserResult = saveFileDialog.ShowDialog();
            if (folderBrowserResult == DialogResult.OK)
                await _businessLogic.SaveProjectionsToXmlFileAsync(_pointModels, saveFileDialog.FileName).ConfigureAwait(false);
        }

        private void CopyButton_Click(object sender, EventArgs e)
        {
            if (_pointModels == null || !_pointModels.Any()) MessageBox.Show("Please select a point on the map.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else _businessLogic.CopyCoordinatesToClipboard(_pointModels);
        }

        private void MoveToCenterButton_Click(object sender, EventArgs e)
        {
            var centerPoint = _businessLogic.GetDisplayCenter();
            ProjectPointAsync(centerPoint);
        }        

        private async void MgrsNotationTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                //Enter key pressed
                if (e.KeyChar == (char)13 && !string.IsNullOrWhiteSpace(MgrsNotationTextBox.Text))
                {
                    var point = await _businessLogic.ConvertFromMgrs(MgrsNotationTextBox.Text.Trim()).ConfigureAwait(false);
                    ProjectPointAsync(point);
                }
            }
            catch
            {
                MessageBox.Show(context.WrongMgrsFormatMessage, context.ErrorString, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void UTMNotationTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                //Enter key pressed
                if (e.KeyChar == (char)13 && !string.IsNullOrWhiteSpace(UTMNotationTextBox.Text))
                {
                    var point = await _businessLogic.ConvertFromUtm(UTMNotationTextBox.Text.Trim()).ConfigureAwait(false);
                    ProjectPointAsync(point);
                }
            }
            catch
            {
                MessageBox.Show(context.WrongUtmFormatMessage, context.ErrorString, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void XCoordinateTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void YCoordinateTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void MgrsNotationTextBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(MgrsNotationTextBox.Text)) MgrsNotationTextBox.SelectAll();
        }

        private void UTMNotationTextBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(UTMNotationTextBox.Text)) UTMNotationTextBox.SelectAll();
        }

        private void PointsGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var grid = (DataGridView)sender;

            if (grid.Columns[e.ColumnIndex] is DataGridViewImageColumn)
            {
                var selectedPoint = ClickedPointsList[e.RowIndex];
                ArcMapHelper.FlashGeometry(selectedPoint, 400);
                ProjectPointAsync(selectedPoint);
            }
        }
        #endregion

        #region ArcMap events handlers
        internal void ArcMap_OnMouseDown(int x, int y)
        {
            var clickedPoint = _businessLogic.GetSelectedPoint(x, y);
            AddPointToList(clickedPoint);

            var newRow = new DataGridViewRow();
            newRow.Cells.Add(new DataGridViewTextBoxCell() { Value = ClickedPointsList.Count() });
            newRow.Cells.Add(new DataGridViewImageCell() { Value = Image.FromFile(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Images\LocatePoint.png")) });
            newRow.Cells.Add(new DataGridViewTextBoxCell() { Value = clickedPoint.X.ToRoundedString() });
            newRow.Cells.Add(new DataGridViewTextBoxCell() { Value = clickedPoint.Y.ToRoundedString() });
            PointsGridView.Rows.Add(newRow);
            
            ProjectPointAsync(clickedPoint);
        }

        internal void ArcMap_OnMouseMove(int x, int y)
        {
            var currentPoint = _businessLogic.GetSelectedPoint(x, y);
            XCoordinateTextBox.Text = currentPoint.X.ToRoundedString();
            YCoordinateTextBox.Text = currentPoint.Y.ToRoundedString();
        }
        #endregion        

        #region Private methods
        private void LocalizeComponents()
        {
            try
            {
                context = new LocalizationContext();
                this.Text = context.CoordinatesConverterWindowCaption;
                this.ProjectionsGroup.Text = context.ProjectionsGroup;
                this.CurrentMapLabel.Text = context.CurrentMapLabel;
                this.WgsCoordinatesLabel.Text = context.WgsLabel;
                this.PulkovoCoordinatesLabel.Text = context.PulkovoLabel;
                this.UkraineCoordinatesLabel.Text = context.UkraineLabel;
                this.MgrsNotationLabel.Text = context.MgrsLabel;
                this.UTMNotationLabel.Text = context.UtmLabel;
                //this.SaveButton.Text = context.SaveButton;
                //this.CopyButton.Text = context.CopyButton;
                //this.MoveToCenterButton.Text = context.MoveToCenterButton;
            }
            catch { MessageBox.Show("No Localization.xml found or there is an error during loading. Coordinates Converter window is not fully localized."); }
        }

        private void ProjectPointAsync(IPoint inputPoint)
        {
            var pointModel = new PointModel();

            if (inputPoint == null) throw new ArgumentNullException(nameof(inputPoint));
            if (inputPoint.SpatialReference == null) throw new NullReferenceException($"Point with ID = {inputPoint.ID} has no spatial reference.");

            XCoordinateTextBox.Text = inputPoint.X.ToRoundedString();
            YCoordinateTextBox.Text = inputPoint.Y.ToRoundedString();

            pointModel.XCoord = inputPoint.X.ToRoundedDouble();
            pointModel.YCoord = inputPoint.Y.ToRoundedDouble();

            //MGRS string MUST be calculated using WGS84 projected point, thus the next lines order matters!
            var wgsPoint = _businessLogic.ProjectPoint(inputPoint, _projectionsModel.WGS84Projection);
            WgsXCoordinateTextBox.Text = wgsPoint.X.ToRoundedString();
            WgsYCoordinateTextBox.Text = wgsPoint.Y.ToRoundedString();

            pointModel.WgsXCoord = wgsPoint.X.ToRoundedDouble();
            pointModel.WgsYCoord = wgsPoint.Y.ToRoundedDouble();

            MgrsNotationTextBox.Text = (_businessLogic.ConvertToMgrs(wgsPoint))?.ToSeparatedMgrs();

            UTMNotationTextBox.Text = _businessLogic.ConvertToUtm(wgsPoint);

            pointModel.MgrsRepresentation = MgrsNotationTextBox.Text;

            pointModel.UtmRepresentation = UTMNotationTextBox.Text;

            var pulkovoPoint = _businessLogic.ProjectPoint(inputPoint, _projectionsModel.Pulkovo1942Projection);
            PulkovoXCoordinateTextBox.Text = pulkovoPoint.X.ToRoundedString();
            PulkovoYCoordinateTextBox.Text = pulkovoPoint.Y.ToRoundedString();

            pointModel.PulkovoXCoord = pulkovoPoint.X.ToRoundedDouble();
            pointModel.PulkovoYCoord = pulkovoPoint.Y.ToRoundedDouble();

            var ukrainePoint = _businessLogic.ProjectPoint(inputPoint, _projectionsModel.Ukraine2000Projection);
            UkraineXCoordinateTextBox.Text = ukrainePoint.X.ToRoundedString();
            UkraineYCoordinateTextBox.Text = ukrainePoint.Y.ToRoundedString();

            pointModel.UkraineXCoord = ukrainePoint.X.ToRoundedDouble();
            pointModel.UkraineYCoord = ukrainePoint.Y.ToRoundedDouble();

            //Remove distorsions
            inputPoint.Project(FocusMapSpatialReference);

            _pointModels.Add(pointModel);
        }
        private void AddPointToList(IPoint point)
        {
            if (point != null && !point.IsEmpty)
            {
                var color = (IColor)new RgbColorClass() { Green = 255 };
                var placedPoint = ArcMapHelper.AddGraphicToMap(point, color, true, esriSimpleMarkerStyle.esriSMSCircle, 7);

                ClickedPointsList.Add(placedPoint);
            }
        }

        private static ProjectionsModel CreateProjecstionsModelFromSettings()
        {
            return new ProjectionsModel(new SingleProjectionModel((int)esriSRProjCSType.esriSRProjCS_WGS1984UTM_36N, 30.000, 0.000),
                                        new SingleProjectionModel((int)esriSRProjCSType.esriSRProjCS_Pulkovo1942GK_6N, 30.000, 44.330),
                                        new SingleProjectionModel(Constants.Ukraine2000ID[2], 30.000, 43.190));
        }
        #endregion        
    }
}
