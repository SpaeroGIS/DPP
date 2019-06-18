using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geometry;
using MilSpace.GeoCalculator.BusinessLogic;
using MilSpace.GeoCalculator.BusinessLogic.Extensions;
using MilSpace.GeoCalculator.BusinessLogic.Interfaces;
using MilSpace.GeoCalculator.BusinessLogic.Models;
using MilSpace.GeoCalculator.BusinessLogic.ReferenceData;
using MilSpace.GeoCalculator.Enums;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MilSpace.GeoCalculator
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
        private PointModel lastProjectedPoint;
        private List<PointModel> pointModels = new List<PointModel>();
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
                    m_windowUI = new DockableWindowGeoCalculator(this.Hook, new MilSpace.GeoCalculator.BusinessLogic.BusinessLogic(arcMap, new DataExport()),
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
            if (lastProjectedPoint == null) MessageBox.Show(context.NoSelectedPointError, context.ErrorString, MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                var chosenRadio = ShowExportForm();
                var folderBrowserResult = saveFileDialog.ShowDialog();
                if (folderBrowserResult == DialogResult.OK)
                {
                    if (chosenRadio == RadioButtonsValues.XML)
                        await _businessLogic.SaveLastProjectionToXmlFileAsync(lastProjectedPoint, saveFileDialog.FileName).ConfigureAwait(false);
                    else if (chosenRadio == RadioButtonsValues.CSV)
                        await _businessLogic.SaveLastProjectionToCsvFileAsync(lastProjectedPoint, saveFileDialog.FileName).ConfigureAwait(false);
                }
            }
        }

        private void CopyButton_Click(object sender, EventArgs e)
        {
            if (lastProjectedPoint == null) MessageBox.Show(context.NoSelectedPointError, context.ErrorString, MessageBoxButtons.OK, MessageBoxIcon.Error);
            else _businessLogic.CopyCoordinatesToClipboard(new List<PointModel> { lastProjectedPoint });
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
                if (e.KeyCode == Keys.Enter && !string.IsNullOrWhiteSpace(XCoordinateTextBox.Text) && !string.IsNullOrWhiteSpace(YCoordinateTextBox.Text))
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
                if (e.KeyCode == Keys.Enter && !string.IsNullOrWhiteSpace(WgsXCoordinateTextBox.Text) && !string.IsNullOrWhiteSpace(WgsYCoordinateTextBox.Text))
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
                if (e.KeyCode == Keys.Enter && !string.IsNullOrWhiteSpace(PulkovoXCoordinateTextBox.Text) && !string.IsNullOrWhiteSpace(PulkovoYCoordinateTextBox.Text))
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
                if (e.KeyCode == Keys.Enter && !string.IsNullOrWhiteSpace(UkraineXCoordinateTextBox.Text) && !string.IsNullOrWhiteSpace(UkraineYCoordinateTextBox.Text))
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

        private void WgsDMSXTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter && !string.IsNullOrWhiteSpace(wgsDMSXTextBox.Text) && !string.IsNullOrWhiteSpace(wgsDMSYTextBox.Text))
                {
                    var point = _businessLogic.CreatePoint(double.Parse(wgsDMSXTextBox.Text),
                                                           double.Parse(wgsDMSYTextBox.Text),
                                                           Constants.WgsGeoModel, true);
                    ProjectPointAsync(point, true);
                }                
            }
            catch
            {
                MessageBox.Show(context.WrongFormatMessage, context.ErrorString, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PulkovoDMSXTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter && !string.IsNullOrWhiteSpace(pulkovoDMSXTextBox.Text) && !string.IsNullOrWhiteSpace(pulkovoDMSYTextBox.Text))
                {
                    var point = _businessLogic.CreatePoint(double.Parse(pulkovoDMSXTextBox.Text),
                                                           double.Parse(pulkovoDMSYTextBox.Text),
                                                           Constants.PulkovoGeoModel, true);
                    ProjectPointAsync(point, true);
                }                
            }
            catch
            {
                MessageBox.Show(context.WrongFormatMessage, context.ErrorString, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UkraineDMSXTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter && !string.IsNullOrWhiteSpace(ukraineDMSXTextBox.Text) && !string.IsNullOrWhiteSpace(ukraineDMSYTextBox.Text))
                {
                    var point = _businessLogic.CreatePoint(double.Parse(ukraineDMSXTextBox.Text),
                                                           double.Parse(ukraineDMSYTextBox.Text),
                                                           Constants.UkraineGeoModel, true);
                    ProjectPointAsync(point, true);
                }                
            }
            catch
            {
                MessageBox.Show(context.WrongFormatMessage, context.ErrorString, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CurrentCoordsCopyButton_Click(object sender, EventArgs e)
        {
            Clipboard.Clear();
            Clipboard.SetText($"{XCoordinateTextBox.Text} {YCoordinateTextBox.Text}");
        }

        private void WgsGeoCopyButton_Click(object sender, EventArgs e)
        {
            Clipboard.Clear();
            Clipboard.SetText($"{wgsDMSXTextBox.Text} {wgsDMSYTextBox.Text}");
        }    

        private void WgsProjCopyButton_Click(object sender, EventArgs e)
        {
            Clipboard.Clear();
            Clipboard.SetText($"{WgsXCoordinateTextBox.Text} {WgsYCoordinateTextBox.Text}");
        }    

        private void PulkovoGeoCopyButton_Click(object sender, EventArgs e)
        {
            Clipboard.Clear();
            Clipboard.SetText($"{pulkovoDMSXTextBox.Text} {pulkovoDMSYTextBox.Text}");
        }

        private void PulkovoProjCopyButton_Click(object sender, EventArgs e)
        {
            Clipboard.Clear();
            Clipboard.SetText($"{PulkovoXCoordinateTextBox.Text} {PulkovoYCoordinateTextBox.Text}");
        }

        private void UkraineGeoCopyButton_Click(object sender, EventArgs e)
        {
            Clipboard.Clear();
            Clipboard.SetText($"{ukraineDMSXTextBox.Text} {ukraineDMSYTextBox.Text}");
        }

        private void UkraineProjCopyButton_Click(object sender, EventArgs e)
        {
            Clipboard.Clear();
            Clipboard.SetText($"{UkraineXCoordinateTextBox.Text} {UkraineYCoordinateTextBox.Text}");
        }

        private void MgrsCopyButton_Click(object sender, EventArgs e)
        {
            Clipboard.Clear();
            Clipboard.SetText(MgrsNotationTextBox.Text);
        }

        private void UtmCopyButton_Click(object sender, EventArgs e)
        {
            Clipboard.Clear();
            Clipboard.SetText(UTMNotationTextBox.Text);
        }

        private void PointsGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var grid = (DataGridView)sender;
            var column = grid.Columns[e.ColumnIndex];
            var selectedPoint = ClickedPointsDictionary.ElementAt(e.RowIndex);

            if (column is DataGridViewImageColumn && column.Name == Constants.HighlightColumnName)
            {
                ArcMapHelper.FlashGeometry(selectedPoint.Value, 350);
                ProjectPointAsync(selectedPoint.Value);
            }
            else if (column is DataGridViewImageColumn && column.Name == Constants.DeleteColumnName)
            {
                grid.Rows.RemoveAt(e.RowIndex);
                ArcMapHelper.RemoveGraphicsFromMap(new string[] { selectedPoint.Key });
                ClickedPointsDictionary.Remove(selectedPoint.Key);

                pointModels.Remove(pointModels.FirstOrDefault(point => selectedPoint.Key.Equals(point.Guid)));

                //Refresh Numbers column cells values
                for (int i = 0; i < grid.Rows.Count; i++)
                {
                    grid[Constants.NumberColumnName, i].Value = i + 1;
                }
            }
            grid.Refresh();
        }

        private async void SaveGridPointsButton_Click(object sender, EventArgs e)
        {
            if (pointModels == null || !pointModels.Any()) MessageBox.Show(context.NoSelectedPointError, context.ErrorString, MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                var chosenRadio = ShowExportForm();
                var folderBrowserResult = saveFileDialog.ShowDialog();
                if (folderBrowserResult == DialogResult.OK)
                {
                    if (chosenRadio == RadioButtonsValues.XML)
                        await _businessLogic.SaveProjectionsToXmlFileAsync(pointModels, saveFileDialog.FileName).ConfigureAwait(false);
                    else if (chosenRadio == RadioButtonsValues.CSV)
                        await _businessLogic.SaveProjectionsToCsvFileAsync(pointModels, saveFileDialog.FileName).ConfigureAwait(false);
                }
            }
        }

        private void CopyGridPointButton_Click(object sender, EventArgs e)
        {
            if (pointModels == null || !pointModels.Any()) MessageBox.Show(context.NoSelectedPointError, context.ErrorString, MessageBoxButtons.OK, MessageBoxIcon.Error);
            else _businessLogic.CopyCoordinatesToClipboard(pointModels);
        }

        private void ClearGridButton_Click(object sender, EventArgs e)
        {
            var grid = this.PointsGridView;
            if (grid.Rows.Count > 0)
            {
                grid.Rows.Clear();
                foreach (var point in ClickedPointsDictionary)
                {
                    ArcMapHelper.RemoveGraphicsFromMap(new string[] { point.Key });
                }

                ClickedPointsDictionary?.Clear();
                pointModels?.Clear();
            }
            grid.Refresh();
        }
        #endregion

        #region ArcMap events handlers
        internal void ArcMap_OnMouseDown(int x, int y)
        {
            var clickedPoint = _businessLogic.GetSelectedPoint(x, y);
            var pointGuid = AddPointToList(clickedPoint);

            var newRow = new DataGridViewRow();
            newRow.Cells.Add(new DataGridViewTextBoxCell() { Value = ClickedPointsDictionary.Count() });
            newRow.Cells.Add(new DataGridViewTextBoxCell() { Value = clickedPoint.X.ToIntegerString() });
            newRow.Cells.Add(new DataGridViewTextBoxCell() { Value = clickedPoint.Y.ToIntegerString() });
            newRow.Cells.Add(new DataGridViewImageCell() { Value = new Bitmap(
                                                                              Image.FromFile(
                                                                                  System.IO.Path.Combine(
                                                                                      System.IO.Path.GetDirectoryName(
                                                                                          Assembly.GetExecutingAssembly().Location), @"Images\LocatePoint.png")), 16, 16),
                                                           ToolTipText = context.ShowPointOnMapButton });            
            newRow.Cells.Add(new DataGridViewImageCell() { Value = new Bitmap(
                                                                              Image.FromFile(
                                                                                  System.IO.Path.Combine(
                                                                                      System.IO.Path.GetDirectoryName(
                                                                                          Assembly.GetExecutingAssembly().Location), @"Images\DeletePoint.png")), 16, 16),
                                                           ToolTipText = context.DeletePointButton });
            PointsGridView.Rows.Add(newRow);

            ProjectPointAsync(clickedPoint, false, pointGuid);
        }

        internal void ArcMap_OnMouseMove(int x, int y)
        {
            var currentPoint = _businessLogic.GetSelectedPoint(x, y);
            XCoordinateTextBox.Text = currentPoint.X.ToIntegerString();
            YCoordinateTextBox.Text = currentPoint.Y.ToIntegerString();
        }
        #endregion        

        #region Private methods
        private void LocalizeComponents()
        {
            try
            {
                context = new LocalizationContext();
                this.TitleLabel.Text = context.CoordinatesConverterWindowCaption;
                this.CurrentMapLabel.Text = context.CurrentMapLabel;
                this.MgrsNotationLabel.Text = context.MgrsLabel;
                this.UTMNotationLabel.Text = context.UtmLabel;

                //TODO:Localize the next Labels and add config here
                this.wgsProjectedLabel.Text = Constants.WgsProjected;
                this.WgsGeoLabel.Text = Constants.WgsGeo;
                this.PulkovoProjectedLabel.Text = Constants.PulkovoProjected;
                this.PulkovoGeoLabel.Text = Constants.PulkovoGeo;
                this.UkraineProjectedLabel.Text = Constants.Ukraine2000CSNames[2];
                this.UkraineGeoLabel.Text = Constants.Ukraine2000CSNames[0];
                this.mgrsToolTip.SetToolTip(this.MgrsNotationTextBox, context.AltRightToMove);
                this.utmToolTip.SetToolTip(this.UTMNotationTextBox, context.AltRightToMove);

                //Tool Tips
                this.MapPointToolButton.ToolTipText = context.ToolButton;
                this.MoveToCenterButton.ToolTipText = context.MoveToCenterButton;
                this.SaveButton.ToolTipText = context.SaveButton;
                this.CopyButton.ToolTipText = context.CopyButton;
                this.ClearGridButton.ToolTipText = context.ClearGridButton;
                this.SaveGridPointsButton.ToolTipText = context.SaveButton;
                this.CopyGridPointButton.ToolTipText = context.CopyButton;
                this.PointsGridView.Columns[Constants.HighlightColumnName].ToolTipText = context.ShowPointOnMapButton;
                this.PointsGridView.Columns[Constants.DeleteColumnName].ToolTipText = context.DeletePointButton;
                this.CurrentCoordsCopyButton.ToolTipText = context.CopyCoordinateButton;
                this.WgsGeoCopyButton.ToolTipText = context.CopyCoordinateButton;
                this.WgsProjCopyButton.ToolTipText = context.CopyCoordinateButton;
                this.PulkovoGeoCopyButton.ToolTipText = context.CopyCoordinateButton;
                this.PulkovoProjCopyButton.ToolTipText = context.CopyCoordinateButton;
                this.UkraineGeoCopyButton.ToolTipText = context.CopyCoordinateButton;
                this.UkraineProjCopyButton.ToolTipText = context.CopyCoordinateButton;
                this.MgrsCopyButton.ToolTipText = context.CopyCoordinateButton;
                this.UtmCopyButton.ToolTipText = context.CopyCoordinateButton;
            }
            catch { MessageBox.Show("No Localization.xml found or there is an error during loading. Coordinates Converter window is not fully localized."); }
        }

        private void ProjectPointAsync(IPoint inputPoint, bool fromUserInput = false, string pointGuid = null)
        {
            lastProjectedPoint = new PointModel
            {
                Guid = pointGuid ?? Guid.NewGuid().ToString()
            };

            if (inputPoint == null) throw new ArgumentNullException(nameof(inputPoint));
            if (inputPoint.SpatialReference == null) throw new NullReferenceException($"Point with ID = {inputPoint.ID} has no spatial reference.");

            if (fromUserInput)
                inputPoint.Project(FocusMapSpatialReference);

            XCoordinateTextBox.Text = inputPoint.X.ToIntegerString();
            YCoordinateTextBox.Text = inputPoint.Y.ToIntegerString();
            lastProjectedPoint.XCoord = inputPoint.X.ToInteger();
            lastProjectedPoint.YCoord = inputPoint.Y.ToInteger();

            //MGRS string MUST be calculated using WGS84 projected point, thus the next lines order matters!
            var wgsPoint = _businessLogic.ProjectPoint(inputPoint, _projectionsModel.WGS84Projection);
            WgsXCoordinateTextBox.Text = wgsPoint.X.ToIntegerString();
            WgsYCoordinateTextBox.Text = wgsPoint.Y.ToIntegerString();
            lastProjectedPoint.WgsXCoord = wgsPoint.X.ToInteger();
            lastProjectedPoint.WgsYCoord = wgsPoint.Y.ToInteger();

            var wgsDD = _businessLogic.ConvertToDecimalDegrees(inputPoint, Constants.WgsGeoModel);
            wgsDMSXTextBox.Text = wgsDD.X.ToRoundedString();
            wgsDMSYTextBox.Text = wgsDD.Y.ToRoundedString();
            lastProjectedPoint.WgsXCoordDD = wgsDD.X.ToRoundedDouble();
            lastProjectedPoint.WgsYCoordDD = wgsDD.Y.ToRoundedDouble();

            MgrsNotationTextBox.Text = (_businessLogic.ConvertToMgrs(wgsPoint))?.ToSeparatedMgrs();
            UTMNotationTextBox.Text = _businessLogic.ConvertToUtm(wgsPoint);

            lastProjectedPoint.MgrsRepresentation = MgrsNotationTextBox.Text;
            lastProjectedPoint.UtmRepresentation = UTMNotationTextBox.Text;

            var pulkovoPoint = _businessLogic.ProjectPoint(inputPoint, _projectionsModel.Pulkovo1942Projection);
            PulkovoXCoordinateTextBox.Text = pulkovoPoint.X.ToIntegerString();
            PulkovoYCoordinateTextBox.Text = pulkovoPoint.Y.ToIntegerString();
            lastProjectedPoint.PulkovoXCoord = pulkovoPoint.X.ToInteger();
            lastProjectedPoint.PulkovoYCoord = pulkovoPoint.Y.ToInteger();

            var pulkovoDD = _businessLogic.ConvertToDecimalDegrees(inputPoint, Constants.PulkovoGeoModel);
            pulkovoDMSXTextBox.Text = pulkovoDD.X.ToRoundedString();
            pulkovoDMSYTextBox.Text = pulkovoDD.Y.ToRoundedString();
            lastProjectedPoint.PulkovoXCoordDD = pulkovoDD.X.ToRoundedDouble();
            lastProjectedPoint.PulkovoYCoordDD = pulkovoDD.Y.ToRoundedDouble();

            var ukrainePoint = _businessLogic.ProjectPoint(inputPoint, _projectionsModel.Ukraine2000Projection);
            UkraineXCoordinateTextBox.Text = ukrainePoint.X.ToIntegerString();
            UkraineYCoordinateTextBox.Text = ukrainePoint.Y.ToIntegerString();
            lastProjectedPoint.UkraineXCoord = ukrainePoint.X.ToInteger();
            lastProjectedPoint.UkraineYCoord = ukrainePoint.Y.ToInteger();

            var ukraineDD = _businessLogic.ConvertToDecimalDegrees(inputPoint, Constants.UkraineGeoModel);
            ukraineDMSXTextBox.Text = ukraineDD.X.ToRoundedString();
            ukraineDMSYTextBox.Text = ukraineDD.Y.ToRoundedString();
            lastProjectedPoint.UkraineXCoordDD = ukraineDD.X.ToRoundedDouble();
            lastProjectedPoint.UkraineYCoordDD = ukraineDD.Y.ToRoundedDouble();

            //Remove distorsions
            inputPoint.Project(FocusMapSpatialReference);

            if (!fromUserInput)
                pointModels.Add(lastProjectedPoint);
        }

        private string AddPointToList(IPoint point)
        {
            if (point != null && !point.IsEmpty)
            {
                var color = (IColor)new RgbColorClass() { Green = 255 };
                var placedPoint = ArcMapHelper.AddGraphicToMap(point, color, true, esriSimpleMarkerStyle.esriSMSCircle, 7);

                ClickedPointsDictionary.Add(placedPoint.Key, placedPoint.Value);
                return placedPoint.Key;
            }

            return null;
        }

        private RadioButtonsValues ShowExportForm()
        {
            var exportForm = new ExportForm
            {
                Text = context.SaveAs
            };

            if (exportForm.ShowDialog(this) == DialogResult.OK)
            {
                if (exportForm.ChosenRadioButton == Enums.RadioButtonsValues.XML)
                {
                    saveFileDialog.Filter = "XML Files (*.xml)|*.xml";
                    saveFileDialog.DefaultExt = "xml";                    
                }
                else if (exportForm.ChosenRadioButton == Enums.RadioButtonsValues.CSV)
                {
                    saveFileDialog.Filter = "CSV Files (*.csv)|*.csv";
                    saveFileDialog.DefaultExt = "csv";
                }
                saveFileDialog.AddExtension = true;
            }
            return exportForm.ChosenRadioButton;
        }

        private static ProjectionsModel CreateProjecstionsModelFromSettings()
        {
            //Configuration settings should be here instead of Constants
            return new ProjectionsModel(Constants.WgsModel, Constants.PulkovoModel, Constants.UkraineModel);
        }

        #endregion
    }
}
