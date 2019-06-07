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
        private readonly Dictionary<string, IPoint> ClickedPointsDictionary = new Dictionary<string, IPoint>();

        public ISpatialReference FocusMapSpatialReference => ArcMap.Document.FocusMap.SpatialReference;

        public DockableWindowGeoCalculator(object hook, IBusinessLogic businessLogic, ProjectionsModel projectionsModel)
        {
            InitializeComponent();
            this.Hook = hook;
            _businessLogic = businessLogic ?? throw new ArgumentNullException(nameof(businessLogic));
            _projectionsModel = projectionsModel ?? throw new ArgumentNullException(nameof(projectionsModel));

            LocalizeComponents();
            SetCurrentMapUnits();
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
            if (_pointModels == null || !_pointModels.Any()) MessageBox.Show("Please select a point on the map.", context.ErrorString, MessageBoxButtons.OK, MessageBoxIcon.Error);

            var folderBrowserResult = saveFileDialog.ShowDialog();
            if (folderBrowserResult == DialogResult.OK)
                await _businessLogic.SaveProjectionsToXmlFileAsync(_pointModels, saveFileDialog.FileName).ConfigureAwait(false);
        }

        private void CopyButton_Click(object sender, EventArgs e)
        {
            if (_pointModels == null || !_pointModels.Any()) MessageBox.Show("Please select a point on the map.", context.ErrorString, MessageBoxButtons.OK, MessageBoxIcon.Error);
            else _businessLogic.CopyCoordinatesToClipboard(_pointModels);
        }

        private void MoveToCenterButton_Click(object sender, EventArgs e)
        {
            var centerPoint = _businessLogic.GetDisplayCenter();
            ProjectPointAsync(centerPoint);
        }      

        private void MgrsNotationTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter && !string.IsNullOrWhiteSpace(MgrsNotationTextBox.Text))
                {
                    var point = _businessLogic.ConvertFromMgrs(MgrsNotationTextBox.Text.Trim(), Constants.WgsGeoModel);
                    ProjectPointAsync(point, true);
                }
            }
            catch
            {
                MessageBox.Show(context.WrongMgrsFormatMessage, context.ErrorString, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            if (e.Alt && e.KeyCode == Keys.Right)
            {
                if (string.IsNullOrWhiteSpace(MgrsNotationTextBox.SelectedText) || MgrsNotationTextBox.SelectedText.Equals(MgrsNotationTextBox.Text))
                {
                    MgrsNotationTextBox.Select(0, MgrsNotationTextBox.Text.IndexOf(' '));
                }
                else
                {
                    var nextPartStartIndex = MgrsNotationTextBox.SelectionStart + MgrsNotationTextBox.SelectionLength + 1;
                    if (nextPartStartIndex - 1 == MgrsNotationTextBox.Text.Length)
                        MgrsNotationTextBox.Select(0, MgrsNotationTextBox.Text.IndexOf(' '));
                    else
                    {
                        if (nextPartStartIndex - 1 == MgrsNotationTextBox.Text.IndexOf(' '))
                        {
                            MgrsNotationTextBox.Select(nextPartStartIndex, MgrsNotationTextBox.Text.LastIndexOf(' ') - nextPartStartIndex);
                        }
                        else if (nextPartStartIndex - 1 == MgrsNotationTextBox.Text.LastIndexOf(' '))
                        {
                            MgrsNotationTextBox.Select(nextPartStartIndex, MgrsNotationTextBox.Text.Length - nextPartStartIndex);
                        }
                    }                        
                }
            }
        }

        private void UTMNotationTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter && !string.IsNullOrWhiteSpace(UTMNotationTextBox.Text))
                {
                    var point = _businessLogic.ConvertFromUtm(UTMNotationTextBox.Text.Trim(), Constants.WgsGeoModel);
                    ProjectPointAsync(point, true);
                }
            }
            catch
            {
                MessageBox.Show(context.WrongUtmFormatMessage, context.ErrorString, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            if (e.Alt && e.KeyCode == Keys.Right)
            {
                if (string.IsNullOrWhiteSpace(UTMNotationTextBox.SelectedText) || UTMNotationTextBox.SelectedText.Equals(UTMNotationTextBox.Text))
                {
                    UTMNotationTextBox.Select(0, UTMNotationTextBox.Text.IndexOf(' '));
                }
                else
                {                    
                    var nextPartStartIndex = UTMNotationTextBox.SelectionStart + UTMNotationTextBox.SelectionLength + 1;
                    if (nextPartStartIndex - 1 == UTMNotationTextBox.Text.Length)
                        UTMNotationTextBox.Select(0, UTMNotationTextBox.Text.IndexOf(' '));
                    else
                    {
                        if (nextPartStartIndex - 1 == UTMNotationTextBox.Text.IndexOf(' '))
                        {
                            UTMNotationTextBox.Select(nextPartStartIndex, UTMNotationTextBox.Text.LastIndexOf(' ') - nextPartStartIndex);
                        }
                        else if (nextPartStartIndex - 1 == UTMNotationTextBox.Text.LastIndexOf(' '))
                        {
                            UTMNotationTextBox.Select(nextPartStartIndex, UTMNotationTextBox.Text.Length - nextPartStartIndex);
                        }
                    }                    
                }
            }
        }        

        private void MgrsNotationTextBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(MgrsNotationTextBox.Text))
            {
                MgrsNotationTextBox.SelectAll();               
            }
        }
       
        private void UTMNotationTextBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(UTMNotationTextBox.Text))
            {
                UTMNotationTextBox.SelectAll();                
            }
        }

        private void XCoordinateTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter && !string.IsNullOrWhiteSpace(XCoordinateTextBox.Text))
                {
                    var point = new PointClass();
                    point.PutCoords(double.Parse(XCoordinateTextBox.Text), double.Parse(YCoordinateTextBox.Text));
                    point.SpatialReference = FocusMapSpatialReference;
                    ProjectPointAsync(point, true);
                }
            }
            catch
            {
                MessageBox.Show(context.WrongFormatMessage, context.ErrorString, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void YCoordinateTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter && !string.IsNullOrWhiteSpace(YCoordinateTextBox.Text))
                {
                    var point = new PointClass();
                    point.PutCoords(double.Parse(XCoordinateTextBox.Text), double.Parse(YCoordinateTextBox.Text));
                    point.SpatialReference = FocusMapSpatialReference;
                    ProjectPointAsync(point, true);
                }
            }
            catch
            {
                MessageBox.Show(context.WrongFormatMessage, context.ErrorString, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void WgsXCoordinateTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter && !string.IsNullOrWhiteSpace(WgsXCoordinateTextBox.Text))
                {
                    var point = _businessLogic.CreatePoint(double.Parse(WgsXCoordinateTextBox.Text), 
                                                           double.Parse(WgsYCoordinateTextBox.Text), 
                                                           Constants.WgsModel);                   
                    ProjectPointAsync(point, true);
                }
            }
            catch
            {
                MessageBox.Show(context.WrongFormatMessage, context.ErrorString, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void WgsYCoordinateTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter && !string.IsNullOrWhiteSpace(WgsYCoordinateTextBox.Text))
                {
                    var point = _businessLogic.CreatePoint(double.Parse(WgsXCoordinateTextBox.Text), 
                                                           double.Parse(WgsYCoordinateTextBox.Text), 
                                                           Constants.WgsModel);
                    ProjectPointAsync(point, true);
                }
            }
            catch
            {
                MessageBox.Show(context.WrongFormatMessage, context.ErrorString, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PulkovoXCoordinateTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter && !string.IsNullOrWhiteSpace(PulkovoXCoordinateTextBox.Text))
                {
                    var point = _businessLogic.CreatePoint(double.Parse(PulkovoXCoordinateTextBox.Text),
                                                           double.Parse(PulkovoYCoordinateTextBox.Text),
                                                           Constants.PulkovoModel);                    
                    ProjectPointAsync(point, true);
                }
            }
            catch
            {
                MessageBox.Show(context.WrongFormatMessage, context.ErrorString, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PulkovoYCoordinateTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter && !string.IsNullOrWhiteSpace(PulkovoYCoordinateTextBox.Text))
                {
                    var point = _businessLogic.CreatePoint(double.Parse(PulkovoXCoordinateTextBox.Text),
                                                           double.Parse(PulkovoYCoordinateTextBox.Text),
                                                           Constants.PulkovoModel);
                    ProjectPointAsync(point, true);
                }
            }
            catch
            {
                MessageBox.Show(context.WrongFormatMessage, context.ErrorString, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UkraineXCoordinateTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter && !string.IsNullOrWhiteSpace(UkraineXCoordinateTextBox.Text))
                {
                    var point = _businessLogic.CreatePoint(double.Parse(UkraineXCoordinateTextBox.Text), 
                                                           double.Parse(UkraineYCoordinateTextBox.Text), 
                                                           Constants.UkraineModel);
                    
                    ProjectPointAsync(point, true);
                }
            }
            catch
            {
                MessageBox.Show(context.WrongFormatMessage, context.ErrorString, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UkraineYCoordinateTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter && !string.IsNullOrWhiteSpace(UkraineYCoordinateTextBox.Text))
                {
                    var point = _businessLogic.CreatePoint(double.Parse(UkraineXCoordinateTextBox.Text),
                                                           double.Parse(UkraineYCoordinateTextBox.Text),
                                                           Constants.UkraineModel);
                    ProjectPointAsync(point, true);
                }
            }
            catch
            {
                MessageBox.Show(context.WrongFormatMessage, context.ErrorString, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PointsGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var grid = (DataGridView)sender;
            var column = grid.Columns[e.ColumnIndex];
            var selectedPoint = ClickedPointsDictionary.ElementAt(e.RowIndex);

            if (column is DataGridViewImageColumn && column.Name == Constants.HighlightColumnName)
            {                
                ArcMapHelper.FlashGeometry(selectedPoint.Value, 400);
                ProjectPointAsync(selectedPoint.Value);
            }
            else if (column is DataGridViewImageColumn && column.Name == Constants.DeleteColumnName)
            {
                grid.Rows.RemoveAt(e.RowIndex);
                ArcMapHelper.RemoveGraphicsFromMap(new string[] { selectedPoint.Key });
                ClickedPointsDictionary.Remove(selectedPoint.Key);

                //Refresh Numbers column cells values
                for (int i = 0; i < grid.Rows.Count; i++)
                {
                    grid[Constants.NumberColumnName, i].Value = i + 1;
                }
            }
            grid.Refresh();
        }
        #endregion

        #region ArcMap events handlers
        internal void ArcMap_OnMouseDown(int x, int y)
        {
            var clickedPoint = _businessLogic.GetSelectedPoint(x, y);
            AddPointToList(clickedPoint);

            var newRow = new DataGridViewRow();
            newRow.Cells.Add(new DataGridViewTextBoxCell() { Value = ClickedPointsDictionary.Count() });
            newRow.Cells.Add(new DataGridViewImageCell() { Value = Image.FromFile(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Images\LocatePoint.png")) });
            newRow.Cells.Add(new DataGridViewTextBoxCell() { Value = clickedPoint.X.ToRoundedString() });
            newRow.Cells.Add(new DataGridViewTextBoxCell() { Value = clickedPoint.Y.ToRoundedString() });
            newRow.Cells.Add(new DataGridViewImageCell() { Value = Image.FromFile(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Images\DeletePoint.png")) });
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

                //ToolTips
                this.mapCenterButtonToolTip.SetToolTip(this.MoveToCenterButton, context.MoveToCenterButton);
                this.toolButtonToolTip.SetToolTip(this.MapPointToolButton, context.ToolButton);
                this.saveButtonToolTip.SetToolTip(this.SaveButton, context.SaveButton);
                this.copyButtonToolTip.SetToolTip(this.CopyButton, context.CopyButton);                
            }
            catch { MessageBox.Show("No Localization.xml found or there is an error during loading. Coordinates Converter window is not fully localized."); }
        }

        private void SetCurrentMapUnits()
        {
            try
            {
                var pUnitConverter = new UnitConverter();
                this.CurrentMapUnitsLabel1.Text =
                    this.CurrentMapUnitsLabel2.Text =
                    $"({pUnitConverter.EsriUnitsAsString(ArcMap.Document.FocusMap.MapUnits, esriCaseAppearance.esriCaseAppearanceLower, true)})";
            }
            catch { }
        }

        private void ProjectPointAsync(IPoint inputPoint, bool fromUserInput = false)
        {
            var pointModel = new PointModel();

            if (inputPoint == null) throw new ArgumentNullException(nameof(inputPoint));
            if (inputPoint.SpatialReference == null) throw new NullReferenceException($"Point with ID = {inputPoint.ID} has no spatial reference.");

            if (fromUserInput)
                inputPoint.Project(FocusMapSpatialReference);

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

            var wgsDD = _businessLogic.ConvertToDecimalDegrees(inputPoint, Constants.WgsGeoModel);
            wgsDMSXTextBox.Text = wgsDD.X.ToRoundedString();
            wgsDMSYTextBox.Text = wgsDD.Y.ToRoundedString();
            pointModel.WgsXCoordDD = wgsDD.X.ToRoundedDouble();
            pointModel.WgsYCoordDD = wgsDD.Y.ToRoundedDouble();

            MgrsNotationTextBox.Text = (_businessLogic.ConvertToMgrs(wgsPoint))?.ToSeparatedMgrs();
            UTMNotationTextBox.Text = _businessLogic.ConvertToUtm(wgsPoint);

            pointModel.MgrsRepresentation = MgrsNotationTextBox.Text;
            pointModel.UtmRepresentation = UTMNotationTextBox.Text;

            var pulkovoPoint = _businessLogic.ProjectPoint(inputPoint, _projectionsModel.Pulkovo1942Projection);
            PulkovoXCoordinateTextBox.Text = pulkovoPoint.X.ToRoundedString();
            PulkovoYCoordinateTextBox.Text = pulkovoPoint.Y.ToRoundedString();
            pointModel.PulkovoXCoord = pulkovoPoint.X.ToRoundedDouble();
            pointModel.PulkovoYCoord = pulkovoPoint.Y.ToRoundedDouble();

            var pulkovoDD = _businessLogic.ConvertToDecimalDegrees(inputPoint, Constants.PulkovoGeoModel);
            pulkovoDMSXTextBox.Text = pulkovoDD.X.ToRoundedString();
            pulkovoDMSYTextBox.Text = pulkovoDD.Y.ToRoundedString();
            pointModel.PulkovoXCoordDD = pulkovoDD.X.ToRoundedDouble();
            pointModel.PulkovoYCoordDD = pulkovoDD.Y.ToRoundedDouble();

            var ukrainePoint = _businessLogic.ProjectPoint(inputPoint, _projectionsModel.Ukraine2000Projection);
            UkraineXCoordinateTextBox.Text = ukrainePoint.X.ToRoundedString();
            UkraineYCoordinateTextBox.Text = ukrainePoint.Y.ToRoundedString();
            pointModel.UkraineXCoord = ukrainePoint.X.ToRoundedDouble();
            pointModel.UkraineYCoord = ukrainePoint.Y.ToRoundedDouble();

            var ukraineDD = _businessLogic.ConvertToDecimalDegrees(inputPoint, Constants.UkraineGeoModel);
            ukraineDMSXTextBox.Text = ukraineDD.X.ToRoundedString();
            ukraineDMSYTextBox.Text = ukraineDD.Y.ToRoundedString();
            pointModel.UkraineXCoordDD = ukraineDD.X.ToRoundedDouble();
            pointModel.UkraineYCoordDD = ukraineDD.Y.ToRoundedDouble();

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

                ClickedPointsDictionary.Add(placedPoint.Key, placedPoint.Value);
            }
        }

        private static ProjectionsModel CreateProjecstionsModelFromSettings()
        {
            //Configuration settings should be here instead of Constants
            return new ProjectionsModel(Constants.WgsModel, Constants.PulkovoModel, Constants.UkraineModel);                                        
        }
        #endregion
    }
}
