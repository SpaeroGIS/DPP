using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geometry;
using MilSpace.Core;
using MilSpace.Core.ModulesInteraction;
using MilSpace.Core.Tools;
using MilSpace.DataAccess.DataTransfer;
using MilSpace.GeoCalculator.BusinessLogic;
using MilSpace.GeoCalculator.BusinessLogic.Extensions;
using MilSpace.GeoCalculator.BusinessLogic.Interfaces;
using MilSpace.GeoCalculator.BusinessLogic.Models;
using MilSpace.GeoCalculator.BusinessLogic.ReferenceData;
using MilSpace.GeoCalculator.Enums;
using MilSpace.GeoCalculator.Interaction;
using MilSpace.Tools.GraphicsLayer;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace MilSpace.GeoCalculator
{
    public partial class DockableWindowGeoCalculator : UserControl, IGeoCalculatorView
    {
        private readonly IBusinessLogic _businessLogic;
        private const string _lineName = "GeoCalcLineGeometry";
        private const string _textName = "text_";
        private ExtendedPointModel lastProjectedPoint;
        private Dictionary<Guid, PointModel> pointModels = new Dictionary<Guid, PointModel>();
        private Dictionary<CoordinateSystemsEnum, string> _coordinateSystems = new Dictionary<CoordinateSystemsEnum, string>();
        private LocalizationContext _context;
        private readonly Dictionary<Guid, IPoint> ClickedPointsDictionary = new Dictionary<Guid, IPoint>();
        //private List<GeoCalcPoint> _geoCalcPoints = new List<GeoCalcPoint>();
        private GeoCalculatorController controller;
        //private GraphicsLayerManager _graphicsLayerManager;
        private int maxNum = 0;

        private static Logger log = Logger.GetLoggerEx("MilSpace.GeoCalculator.DockableWindowGeoCalculator");

        //Current Projection Models
        private ProjectionsModel CurrentProjectionsModel = Constants.ProjectionsModels[0];

        public ISpatialReference FocusMapSpatialReference => ArcMap.Document.ActiveView.FocusMap.SpatialReference;

        public DockableWindowGeoCalculator(object hook, IBusinessLogic businessLogic, GeoCalculatorController controller)
        {
            log.InfoEx(">>> DockableWindowGeoCalculator (Constructor) START <<<");
            InitializeComponent();
            SetController(controller);
            this.Hook = hook;
            _businessLogic = businessLogic ?? throw new ArgumentNullException(nameof(businessLogic));
            //_graphicsLayerManager = new GraphicsLayerManager(ArcMap.Document.ActiveView);

            LocalizeComponents();

            log.InfoEx("> DockableWindowGeoCalculator (Constructor) END");
        }

        public void SetController(GeoCalculatorController controller)
        {
            this.controller = controller;
            controller.SetView(this);
        }

        public void AddPointsToGrid(IEnumerable<IPoint> points)
        {
            foreach(var point in points)
            {
                point.Project(FocusMapSpatialReference);
                ProcessPointAsClicked(point, true, true);
            }

            RedrawLine();
        }

        public void AddPointsToGrid(IEnumerable<GeoCalcPoint> points)
        {
            PointsGridView.Rows.Clear();
            foreach(var point in points)
            {
                GraphicsLayerManager graphicsLayerManager = GraphicsLayerManager.GetGraphicsLayerManager(ArcMap.Document.ActiveView);
                graphicsLayerManager.RemovePoint(point.Id.ToString());
                ProcessPointAsClicked(point, Constants.WgsGeoModel, true);
            }

            DrawLine();
        }

        public Dictionary<int, IPoint> GetPointsList()
        {
            var points = new Dictionary<int, IPoint>();

            foreach(DataGridViewRow row in PointsGridView.Rows)
            {
                if(row == null)
                {
                    continue;
                }

                var point = ClickedPointsDictionary[(Guid)row.Tag];
                var pointCopy = point.CloneWithProjecting();

                if (!points.Any(p => p.Key == (int)row.Cells[0].Value))
                {
                    points.Add((int)row.Cells[0].Value, pointCopy);
                }
            }

            return points;
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
                    GeoCalculatorController = new GeoCalculatorController();
                    ModuleInteraction.Instance.RegisterModuleInteraction<IGeocalculatorInteraction>(new GeoCalculatorInteraction(GeoCalculatorController));

                    m_windowUI = new DockableWindowGeoCalculator(
                        this.Hook,
                        new MilSpace.GeoCalculator.BusinessLogic.BusinessLogic(arcMap, new DataImportExport()), GeoCalculatorController);
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

            internal GeoCalculatorController GeoCalculatorController { get; private set; }

        }
        #endregion

        #region UserControl events handlers
        #region ButtonClickEvents
        private void MapPointToolButton_Click(object sender, EventArgs e)
        {
            UID mapToolID = new UIDClass
            {
                Value = ThisAddIn.IDs.MapInteropTool
            };
            var documentBars = ArcMap.Application.Document.CommandBars;
            var mapTool = documentBars.Find(mapToolID, false, false);

            if (ArcMap.Application.CurrentTool?.ID?.Value != null
                && ArcMap.Application.CurrentTool.ID.Value.Equals(mapTool.ID.Value))
            {
                ArcMap.Application.CurrentTool = null;
                MapPointToolButton.Checked = false;
            }
            else
            {
                ArcMap.Application.CurrentTool = mapTool;
                MapPointToolButton.Checked = true;
            }
        }

        private void CopyButton_Click(object sender, EventArgs e)
        {
            if (lastProjectedPoint == null)
                MessageBox.Show(
                    _context.NoSelectedPointError,
                    _context.ErrorString,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            else _businessLogic.CopyCoordinatesToClipboard(lastProjectedPoint);
        }

        private void MoveToCenterButton_Click(object sender, EventArgs e)
        {
            var centerPoint = _businessLogic.GetDisplayCenter();
            ProjectPointAsync(centerPoint, true);
        }

        private void MoveToTheProjectedCoordButton_Click(object sender, EventArgs e)
        {
            try
            {
                IPoint point = new PointClass();

                double xCoordinate = 0;
                double yCoordinate = 0;

                if (Helper.TryParceToDouble(XCoordinateTextBox.Text, out xCoordinate)
                    && Helper.TryParceToDouble(YCoordinateTextBox.Text, out yCoordinate))
                {
                    point.PutCoords(xCoordinate, yCoordinate);
                    //point.PutCoords(double.Parse(XCoordinateTextBox.Text), double.Parse(YCoordinateTextBox.Text));

                    point.SpatialReference = FocusMapSpatialReference;
                    ProcessPointAsClicked(point, false);

                    var document = ArcMap.Document as IMxDocument;
                    var activeView = document?.ActiveView;
                    IEnvelope envelope = activeView?.Extent;
                    if (envelope != null)
                    {
                        envelope.CenterAt(point);
                        activeView.Extent = envelope;
                        activeView.Refresh();
                    }

                }
                else
                {
                    string ss = string.Format("XCoordinateTextBox.Text:{0} YCoordinateTextBox.Text:{1}\nxCoordinate:{2} yCoordinate:{3}",
                        XCoordinateTextBox.Text, YCoordinateTextBox.Text, xCoordinate, yCoordinate);
                    MessageBox.Show(
                        _context.WrongFormatMessage + "\n" + ss,
                        _context.ErrorString,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    log.DebugEx("MoveToTheProjectedCoordButton_Click " + _context.WrongFormatMessage + ss);
                }

            }
            catch
            {
                MessageBox.Show(
                    _context.WrongFormatMessage + " -> " + XCoordinateTextBox.Text + ";" + YCoordinateTextBox.Text,
                    _context.ErrorString,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
        #endregion

        #region Copy Buttons Click
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
        #endregion

        #region Paste Buttons Click
        private void CurrentCoordsPasteButton_Click(object sender, EventArgs e)
        {
            double prevValueX = Double.Parse(XCoordinateTextBox.Text);
            double prevValueY = Double.Parse(YCoordinateTextBox.Text);

            var clipboard = Clipboard.GetText();
            if (string.IsNullOrWhiteSpace(clipboard)) return;

            try
            {
                var stringParts = clipboard.ToNormalizedString().Split(' ');

                if (stringParts.Length != 2)
                {
                    MessageBox.Show(
                        _context.WrongFormatMessage,
                        _context.ErrorString,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                if (!stringParts.First().ToDoubleInvariantCulture(out double xCoordinate) ||
                    !stringParts.Last().ToDoubleInvariantCulture(out double yCoordinate))
                    MessageBox.Show(
                        _context.WrongFormatMessage,
                        _context.ErrorString,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                else
                {
                    SetCurrentValues(xCoordinate, yCoordinate);
                }
            }
            catch (Exception ex)
            {
                log.ErrorEx(ex.Message);
                MessageBox.Show(
                    _context.WrongFormatMessage,
                    _context.ErrorString,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                SetCurrentValues(prevValueX, prevValueY);
            }
        }

        private void SetCurrentValues(double x, double y)
        {
            var point = new PointClass();
            point.PutCoords(x, y);
            point.SpatialReference = FocusMapSpatialReference;
            ProjectPointAsync(point, true);
        }

        private void WgsGeoPasteButton_Click(object sender, EventArgs e)
        {
            var clipboard = Clipboard.GetText().Trim();
            if (string.IsNullOrWhiteSpace(clipboard)) return;

            try
            {
                if (!Regex.IsMatch(clipboard, @"^([-]?[\d]{1,2}[\,|\.]\d+)[\;|\s]([-]?[\d]{1,2}[\,|\.]\d+)$"))
                {
                    string sMsgText = _context.FindLocalizedElement(
                        "MsgInvalidCoordinatesDD",
                        "Недійсні дані. Потрібні коордінати представлені у десяткових градусах");
                    MessageBox.Show(
                        sMsgText + " -> " + clipboard,
                        _context.ErrorString,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                //var stringParts = clipboard.ToNormalizedString().Replace(';', ' ').Split(' ');
                var stringParts = clipboard.Replace(';', ' ').Split(' ');

                if (stringParts.Length < 1)
                {
                    MessageBox.Show(
                        _context.FindLocalizedElement(
                            "WrongFormatMessageToFewParts", "Не вдалося отримати обидві складові координати")
                            + " -> " + stringParts,
                        _context.ErrorString,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                if (!Helper.TryParceToDouble(stringParts[0], out double xCoordinate)
                    || !Helper.TryParceToDouble(stringParts[1], out double yCoordinate))

                    //if (!stringParts[0].ToDoubleInvariantCulture(out double xCoordinate) 
                    //|| !stringParts[1].ToDoubleInvariantCulture(out double yCoordinate))

                    MessageBox.Show(
                        _context.WrongFormatMessage + " WGS DD -> " + stringParts[0] + ' ' + stringParts[1],
                        _context.ErrorString,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                else
                {
                    var point = _businessLogic.CreatePoint(xCoordinate, yCoordinate, Constants.WgsGeoModel, true);
                    ProjectPointAsync(point, true);
                }
            }
            catch
            {
                MessageBox.Show(
                    _context.WrongFormatMessage,
                    _context.ErrorString,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void WgsProjPasteButton_Click(object sender, EventArgs e)
        {
            var clipboard = Clipboard.GetText();
            if (string.IsNullOrWhiteSpace(clipboard)) return;

            try
            {
                var stringParts = clipboard.ToNormalizedString().Split(' ');

                if (stringParts.Length != 2)
                {
                    MessageBox.Show(
                        _context.WrongFormatMessage + " -> " + clipboard,
                        _context.ErrorString,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                if (!stringParts.First().ToDoubleInvariantCulture(out double xCoordinate) ||
                    !stringParts.Last().ToDoubleInvariantCulture(out double yCoordinate))
                    MessageBox.Show(
                        _context.WrongFormatMessage,
                        _context.ErrorString,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                else
                {
                    var point = _businessLogic.CreatePoint(xCoordinate, yCoordinate, CurrentProjectionsModel.WGS84Projection);
                    ProjectPointAsync(point, true);
                }
            }
            catch
            {
                MessageBox.Show(
                    _context.WrongFormatMessage,
                    _context.ErrorString,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void PulkovoGeoPasteButton_Click(object sender, EventArgs e)
        {
            var clipboard = Clipboard.GetText();
            if (string.IsNullOrWhiteSpace(clipboard)) return;

            try
            {
                if (!Regex.IsMatch(clipboard, @"^([-]?[\d]{1,2}[\,|\.]\d+)[\;| ]([-]?[\d]{1,2}[\,|\.]\d+)$"))
                {
                    string sMsgText = _context.FindLocalizedElement(
                        "MsgInvalidCoordinatesDD",
                        "Недійсні дані. Потрібні коордінати представлені у десяткових градусах");
                    MessageBox.Show(
                        sMsgText + " -> " + clipboard,
                        _context.ErrorString,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }
                var stringParts = clipboard.Replace(';', ' ').Split(' ');

                //var stringParts = clipboard.ToNormalizedString().Split(' ');

                if (stringParts.Length < 2)
                {
                    MessageBox.Show(
                        _context.WrongFormatMessage,
                        _context.ErrorString,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                if (pulkovoDMSXTextBox.Text == stringParts[0]
                    && pulkovoDMSYTextBox.Text == stringParts[1])
                    return;

                if (!Helper.TryParceToDouble(stringParts[0], out double xCoordinate)
                    || !Helper.TryParceToDouble(stringParts[1], out double yCoordinate))

                    //if (!stringParts.First().ToDoubleInvariantCulture(out double xCoordinate) ||
                    //!stringParts.Last().ToDoubleInvariantCulture(out double yCoordinate))

                    MessageBox.Show(
                        _context.WrongFormatMessage + " -> " + clipboard,
                        _context.ErrorString,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                else
                {
                    var point = _businessLogic.CreatePoint(xCoordinate, yCoordinate, Constants.PulkovoGeoModel, true);
                    ProjectPointAsync(point, true);
                }
            }
            catch
            {
                MessageBox.Show(
                    _context.WrongFormatMessage,
                    _context.ErrorString,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void PulkovoProjPasteButton_Click(object sender, EventArgs e)
        {
            var clipboard = Clipboard.GetText();
            if (string.IsNullOrWhiteSpace(clipboard)) return;

            try
            {
                var stringParts = clipboard.ToNormalizedString().Split(' ');

                if (stringParts.Length != 2)
                {
                    MessageBox.Show(
                        _context.WrongFormatMessage + " -> " + clipboard,
                        _context.ErrorString,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                if (PulkovoXCoordinateTextBox.Text == stringParts.First()
                    && PulkovoYCoordinateTextBox.Text == stringParts.Last())
                    return;

                if (!stringParts.First().ToDoubleInvariantCulture(out double xCoordinate) ||
                    !stringParts.Last().ToDoubleInvariantCulture(out double yCoordinate))
                    MessageBox.Show(
                        _context.WrongFormatMessage + " -> " + clipboard,
                        _context.ErrorString,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                else
                {
                    var point = _businessLogic.CreatePoint(
                        xCoordinate, yCoordinate, CurrentProjectionsModel.Pulkovo1942Projection);
                    ProjectPointAsync(point, true);
                }
            }
            catch
            {
                MessageBox.Show(_context.WrongFormatMessage, _context.ErrorString, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UkraineGeoPasteButton_Click(object sender, EventArgs e)
        {
            var clipboard = Clipboard.GetText();
            if (string.IsNullOrWhiteSpace(clipboard)) return;

            try
            {
                if (!Regex.IsMatch(clipboard, @"^([-]?[\d]{1,2}[\,|\.]\d+)[\;| ]([-]?[\d]{1,2}[\,|\.]\d+)$"))
                {
                    string sMsgText = _context.FindLocalizedElement(
                        "MsgInvalidCoordinatesDD",
                        "Недійсні дані. Потрібні коордінати представлені у десяткових градусах");
                    MessageBox.Show(
                        sMsgText + " -> " + clipboard,
                        _context.ErrorString,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }
                var stringParts = clipboard.Replace(';', ' ').Split(' ');
                //var stringParts = clipboard.ToNormalizedString().Split(' ');

                if (stringParts.Length < 2)
                {
                    MessageBox.Show(
                        _context.WrongFormatMessage,
                        _context.ErrorString,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                if (ukraineDMSXTextBox.Text == stringParts[0] && ukraineDMSYTextBox.Text == stringParts[1]) return;

                if (!Helper.TryParceToDouble(stringParts[0], out double xCoordinate)
                    || !Helper.TryParceToDouble(stringParts[1], out double yCoordinate))

                    //if (!stringParts.First().ToDoubleInvariantCulture(out double xCoordinate) ||
                    //!stringParts.Last().ToDoubleInvariantCulture(out double yCoordinate))

                    MessageBox.Show(
                        _context.WrongFormatMessage + " -> " + clipboard,
                        _context.ErrorString,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                else
                {
                    var point = _businessLogic.CreatePoint(xCoordinate, yCoordinate, Constants.UkraineGeoModel, true);
                    ProjectPointAsync(point, true);
                }
            }
            catch
            {
                MessageBox.Show(
                    _context.WrongFormatMessage,
                    _context.ErrorString,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void UkraineProjPasteButton_Click(object sender, EventArgs e)
        {
            var clipboard = Clipboard.GetText();
            if (string.IsNullOrWhiteSpace(clipboard)) return;

            try
            {
                var stringParts = clipboard.ToNormalizedString().Split(' ');

                if (stringParts.Length != 2)
                {
                    MessageBox.Show(
                        _context.WrongFormatMessage,
                        _context.ErrorString,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                if (UkraineXCoordinateTextBox.Text == stringParts.First()
                    && UkraineYCoordinateTextBox.Text == stringParts.Last())
                    return;

                if (!stringParts.First().ToDoubleInvariantCulture(out double xCoordinate) ||
                    !stringParts.Last().ToDoubleInvariantCulture(out double yCoordinate))
                    MessageBox.Show(
                        _context.WrongFormatMessage,
                        _context.ErrorString,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                else
                {
                    var point = _businessLogic.CreatePoint(
                        xCoordinate, yCoordinate, CurrentProjectionsModel.Ukraine2000Projection);
                    ProjectPointAsync(point, true);
                }
            }
            catch
            {
                MessageBox.Show(
                    _context.WrongFormatMessage,
                    _context.ErrorString,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void MgrsPasteButton_Click(object sender, EventArgs e)
        {
            var clipboard = Clipboard.GetText();
            if (string.IsNullOrWhiteSpace(clipboard)) return;

            try
            {
                var point = _businessLogic.ConvertFromMgrs(clipboard.ToNormalizedString(), Constants.WgsGeoModel);
                ProjectPointAsync(point, true);
            }
            catch
            {
                MessageBox.Show(
                    _context.WrongMgrsFormatMessage,
                    _context.ErrorString,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
        #endregion

        #region DoubleClick Events
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
        #endregion

        #region KeyDown Events
        private void XCoordinateTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter && !string.IsNullOrWhiteSpace(XCoordinateTextBox.Text) && !string.IsNullOrWhiteSpace(YCoordinateTextBox.Text))
                {
                    var point = new PointClass();
                    point.PutCoords(double.Parse(XCoordinateTextBox.Text, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture),
                                    double.Parse(YCoordinateTextBox.Text, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture));
                    point.SpatialReference = FocusMapSpatialReference;
                    ProjectPointAsync(point, true);
                }
            }
            catch
            {
                MessageBox.Show(_context.WrongFormatMessage, _context.ErrorString, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void WgsXCoordinateTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter && !string.IsNullOrWhiteSpace(WgsXCoordinateTextBox.Text) && !string.IsNullOrWhiteSpace(WgsYCoordinateTextBox.Text))
                {
                    var point = _businessLogic.CreatePoint(double.Parse(WgsXCoordinateTextBox.Text, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture),
                                                           double.Parse(WgsYCoordinateTextBox.Text, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture),
                                                           CurrentProjectionsModel.WGS84Projection);
                    ProjectPointAsync(point, true);
                }
            }
            catch
            {
                MessageBox.Show(_context.WrongFormatMessage, _context.ErrorString, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PulkovoXCoordinateTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (!PulkovoXCoordinateTextBox.Modified && !PulkovoYCoordinateTextBox.Modified) return;

                if (e.KeyCode == Keys.Enter && !string.IsNullOrWhiteSpace(PulkovoXCoordinateTextBox.Text) && !string.IsNullOrWhiteSpace(PulkovoYCoordinateTextBox.Text))
                {
                    var point = _businessLogic.CreatePoint(double.Parse(PulkovoXCoordinateTextBox.Text, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture),
                                                           double.Parse(PulkovoYCoordinateTextBox.Text, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture),
                                                           CurrentProjectionsModel.Pulkovo1942Projection);
                    ProjectPointAsync(point, true);
                }
            }
            catch
            {
                MessageBox.Show(_context.WrongFormatMessage, _context.ErrorString, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UkraineXCoordinateTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (!UkraineXCoordinateTextBox.Modified && !UkraineYCoordinateTextBox.Modified) return;

                if (e.KeyCode == Keys.Enter && !string.IsNullOrWhiteSpace(UkraineXCoordinateTextBox.Text) && !string.IsNullOrWhiteSpace(UkraineYCoordinateTextBox.Text))
                {
                    var point = _businessLogic.CreatePoint(double.Parse(UkraineXCoordinateTextBox.Text, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture),
                                                           double.Parse(UkraineYCoordinateTextBox.Text, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture),
                                                           CurrentProjectionsModel.Ukraine2000Projection);

                    ProjectPointAsync(point, true);
                }
            }
            catch
            {
                MessageBox.Show(_context.WrongFormatMessage, _context.ErrorString, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void WgsDMSXTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter && !string.IsNullOrWhiteSpace(wgsDMSXTextBox.Text) && !string.IsNullOrWhiteSpace(wgsDMSYTextBox.Text))
                {
                    var point = _businessLogic.CreatePoint(double.Parse(wgsDMSXTextBox.Text, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture),
                                                           double.Parse(wgsDMSYTextBox.Text, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture),
                                                           Constants.WgsGeoModel, true);
                    ProjectPointAsync(point, true);
                }
            }
            catch
            {
                MessageBox.Show(_context.WrongFormatMessage, _context.ErrorString, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PulkovoDMSXTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (!pulkovoDMSXTextBox.Modified && !pulkovoDMSYTextBox.Modified) return;

                if (e.KeyCode == Keys.Enter && !string.IsNullOrWhiteSpace(pulkovoDMSXTextBox.Text) && !string.IsNullOrWhiteSpace(pulkovoDMSYTextBox.Text))
                {
                    var point = _businessLogic.CreatePoint(double.Parse(pulkovoDMSXTextBox.Text, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture),
                                                           double.Parse(pulkovoDMSYTextBox.Text, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture),
                                                           Constants.PulkovoGeoModel, true);
                    ProjectPointAsync(point, true);
                }
            }
            catch
            {
                MessageBox.Show(_context.WrongFormatMessage, _context.ErrorString, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UkraineDMSXTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (!ukraineDMSXTextBox.Modified && !ukraineDMSYTextBox.Modified) return;

                if (e.KeyCode == Keys.Enter && !string.IsNullOrWhiteSpace(ukraineDMSXTextBox.Text) && !string.IsNullOrWhiteSpace(ukraineDMSYTextBox.Text))
                {
                    var point = _businessLogic.CreatePoint(double.Parse(ukraineDMSXTextBox.Text, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture),
                                                           double.Parse(ukraineDMSYTextBox.Text, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture),
                                                           Constants.UkraineGeoModel, true);
                    ProjectPointAsync(point, true);
                }
            }
            catch
            {
                MessageBox.Show(_context.WrongFormatMessage, _context.ErrorString, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MgrsNotationTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (!MgrsNotationTextBox.Modified) return;

                if (e.KeyCode == Keys.Enter && !string.IsNullOrWhiteSpace(MgrsNotationTextBox.Text))
                {
                    var point = _businessLogic.ConvertFromMgrs(MgrsNotationTextBox.Text.Trim(), Constants.WgsGeoModel);
                    ProjectPointAsync(point, true);
                }
            }
            catch
            {
                MessageBox.Show(_context.WrongMgrsFormatMessage, _context.ErrorString, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                if (!UTMNotationTextBox.Modified) return;

                if (e.KeyCode == Keys.Enter && !string.IsNullOrWhiteSpace(UTMNotationTextBox.Text))
                {
                    var point = _businessLogic.ConvertFromUtm(UTMNotationTextBox.Text.Trim(), Constants.WgsGeoModel);
                    ProjectPointAsync(point, true);
                }
            }
            catch
            {
                MessageBox.Show(_context.WrongUtmFormatMessage, _context.ErrorString, MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void CoordinatesTextBoxes_TextChanged(object sender, EventArgs e)
        {
            if (sender is TextBox currentTextBox)
            {
                currentTextBox.Text = currentTextBox.Text.Replace(" ", string.Empty);
            }
        }
        #endregion

        #region DataGridView Events
        private void PointsGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.RowIndex == -1)
            {
                return;
            }

            var grid = (DataGridView)sender;
            var column = grid.Columns[e.ColumnIndex];
            var num = (int)grid.Rows[e.RowIndex].Cells[0].Value;
            var selectedPoint = ClickedPointsDictionary.First(point => point.Key == (Guid)PointsGridView.Rows[e.RowIndex].Tag);

            if (column is DataGridViewImageColumn && column.Name == Constants.HighlightColumnName)
            {
                ArcMapHelper.FlashGeometry(selectedPoint.Value, 500);
                ProjectPointAsync(selectedPoint.Value, true);
            }
            else
            if (column is DataGridViewImageColumn && column.Name.Equals(Constants.DeleteColumnName, StringComparison.CurrentCultureIgnoreCase))
            {
                var prevPointGuid = (e.RowIndex == 0) ? (Guid)PointsGridView.Rows[e.RowIndex].Tag : (Guid)PointsGridView.Rows[e.RowIndex - 1].Tag;
                var nextPointGuid = (e.RowIndex == PointsGridView.RowCount - 1) ? (Guid)PointsGridView.Rows[e.RowIndex].Tag : (Guid)PointsGridView.Rows[e.RowIndex + 1].Tag;

                grid.Rows.RemoveAt(e.RowIndex);

                if(chkShowLine.Checked)
                {
                    GraphicsLayerManager graphicsLayerManager = GraphicsLayerManager.GetGraphicsLayerManager(ArcMap.Document.ActiveView);

                    graphicsLayerManager.RemovePoint(selectedPoint.Key.ToString());
                    var linesToRemove = new string[] { $"{_lineName}_{prevPointGuid}" };

                    graphicsLayerManager.RemoveGraphicsFromMap(linesToRemove);

                    if(e.RowIndex > 0 && e.RowIndex < PointsGridView.RowCount)
                    {
                        graphicsLayerManager.AddLineSegmentToMap(ClickedPointsDictionary[prevPointGuid], ClickedPointsDictionary[nextPointGuid], _lineName, prevPointGuid.ToString());
                    }
                }

                ClickedPointsDictionary.Remove(selectedPoint.Key);

                pointModels.Remove(selectedPoint.Key);
                controller.RemovePoint(selectedPoint.Key);

                if(num == maxNum)
                {
                    maxNum--;
                }
            }
            else
            {
                grid.Rows[e.RowIndex].Selected = true;
                ArcMapHelper.FlashGeometry(selectedPoint.Value, 500);
                ProjectPointAsync(selectedPoint.Value, true);
            }
        }

        private async void SaveGridPointsButton_Click(object sender, EventArgs e)
        {
            if (pointModels == null || !pointModels.Any()) MessageBox.Show(_context.NoSelectedPointError, _context.ErrorString, MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                var chosenRadio = ShowExportForm(true);

                if(chosenRadio == RadioButtonsValues.Layer)
                {
                    ExportToLayer();
                    return;
                }

                var folderBrowserResult = saveFileDialog.ShowDialog();
                if (folderBrowserResult == DialogResult.OK)
                {
                    if (chosenRadio == RadioButtonsValues.XML)
                        await _businessLogic.SaveProjectionsToXmlFileAsync(
                            pointModels.ToSortedPointModelsList(), saveFileDialog.FileName);
                    else if (chosenRadio == RadioButtonsValues.CSV)
                        await _businessLogic.SaveProjectionsToCsvFileAsync(
                            pointModels.ToSortedPointModelsList(), saveFileDialog.FileName);
                }
            }
        }

        private async void OpenFileGridButton_Click(object sender, EventArgs e)
        {
            var startCount = PointsGridView.RowCount;
            var chosenRadio = ShowExportForm(false);

            if(chosenRadio != RadioButtonsValues.Layer)
            {
                var filter = chosenRadio == RadioButtonsValues.CSV ? "CSV|*.csv" : "XML|*.xml";
                openFileDialog.Filter = filter;
                var openFileDialogResult = openFileDialog.ShowDialog();
                var fileName = openFileDialog.FileName;
                var pointsList = new List<PointModel>();
                if(openFileDialogResult == DialogResult.OK && !string.IsNullOrWhiteSpace(fileName))
                {
                    try
                    {
                        if(System.IO.Path.GetExtension(fileName).Equals(Constants.CSV))
                            pointsList = await _businessLogic.ImportProjectionsFromCsvAsync(fileName).ConfigureAwait(true);
                        else if(System.IO.Path.GetExtension(fileName).Equals(Constants.XML))
                            pointsList = await _businessLogic.ImportProjectionsFromXmlAsync(fileName).ConfigureAwait(true);

                        //ClearGridButton_Click(sender, e);

                        if(pointsList != null && pointsList.Any())
                        {
                            foreach(var pointModel in pointsList)
                            {
                                ProcessPointAsClicked(pointModel, Constants.WgsGeoModel, true);
                            }
                        }
                    }
                    catch
                    {
                        MessageBox.Show(_context.WrongFormatMessage, _context.ErrorString, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                var chooseLayerForm = new ChooseLayerForImportForm();
                var result = chooseLayerForm.ShowDialog();

                if(result == DialogResult.OK)
                {
                    controller.ImportFromLayer(chooseLayerForm.SelectedLayer);
                }
            }

            var points = new List<GeoCalcPoint>();
            int startIndex = (startCount == 0) ? startCount : startCount - 1;

            for(int i = startIndex; i < PointsGridView.RowCount; i++)
            {
                var row = PointsGridView.Rows[i];

                if(row == null)
                {
                    continue;
                }

                points.Add(GetGeoCalcPoint((Guid)row.Tag, (int)row.Cells[0].Value));
            }

            controller.UpdatePoints(points);
        }

        private void CopyGridPointButton_Click(object sender, EventArgs e)
        {
            if (pointModels == null || !pointModels.Any()) MessageBox.Show(_context.NoSelectedPointError, _context.ErrorString, MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                _businessLogic.CopyCoordinatesToClipboard(pointModels.ToSortedPointModelsList());
            }
        }

        private void ClearGridButton_Click(object sender, EventArgs e)
        {
            var grid = this.PointsGridView;
            if (grid.Rows.Count > 0)
            {
                grid.Rows.Clear();
                GraphicsLayerManager graphicsLayerManager = GraphicsLayerManager.GetGraphicsLayerManager(ArcMap.Document.ActiveView);

                foreach (var point in ClickedPointsDictionary)
                {
                    graphicsLayerManager.RemovePoint(point.Key.ToString());
                }

                ClickedPointsDictionary?.Clear();
                pointModels?.Clear();
            }
            grid.Refresh();

            controller.ClearSession();
            maxNum = 0;
        }
        #endregion
        #endregion

        #region ArcMap events handlers
        internal void ArcMap_OnMouseDown(int x, int y)
        {
            var clickedPoint = _businessLogic.GetSelectedPoint(x, y);
            ProcessPointAsClicked(clickedPoint, true);

            var lastRow = PointsGridView.Rows[PointsGridView.RowCount - 1];
            controller.UpdatePoints(new List<GeoCalcPoint> { GetGeoCalcPoint((Guid)lastRow.Tag, (int)lastRow.Cells[0].Value) });
        }

        internal void ArcMap_OnMouseMove(int x, int y)
        {
            var currentPoint = _businessLogic.GetSelectedPoint(x, y);
            ProjectPointAsync(currentPoint, true);
        }

        private void OnOpenDocument()
        {
            controller.FillPointsListFromDB();
        }

        #endregion        

        #region Private methods
        private void LocalizeComponents()
        {
            try
            {
                _context = new LocalizationContext();
                this.TitleLabel.Text = _context.CoordinatesConverterWindowCaption;
                this.CurrentMapLabel.Text = _context.CurrentMapLabel;
                this.MgrsNotationLabel.Text = _context.MgrsLabel;
                this.UTMNotationLabel.Text = _context.UtmLabel;

                this.NumberColumn.HeaderText = _context.FindLocalizedElement("NumColHeader", "N");
                this.XCoordColumn.HeaderText = _context.FindLocalizedElement("XCoordColHeader", "Довгота");
                this.YCoordColumn.HeaderText = _context.FindLocalizedElement("YCoordColHeader", "Широта");

                //TODO:Localize the next Labels and add config here
                this.wgsProjectedLabel.Text = CurrentProjectionsModel.WGS84Projection.Name;
                this.wgsGeoLabel.Text = _context.WgsLabel;
                this.PulkovoProjectedLabel.Text = CurrentProjectionsModel.Pulkovo1942Projection.Name;
                this.PulkovoGeoLabel.Text = _context.PulkovoLabel;
                this.UkraineProjectedLabel.Text = CurrentProjectionsModel.Ukraine2000Projection.Name;
                this.UkraineGeoLabel.Text = _context.UkraineLabel;
                this.toolsLabel.Text = _context.FindLocalizedElement("ToolBarTitle", "Список точок");
                this.coordLabel.Text = _context.FindLocalizedElement("CoordsSystemsLabel", "СК:");
                this.graphicLabel.Text = _context.FindLocalizedElement("GraphicToolAreaTitle", "Робота з графікою");
                this.chkShowLine.Text = _context.FindLocalizedElement("ShowLineCheckText", "Показати лінію на карті");
                this.chkShowNumbers.Text = _context.FindLocalizedElement("ShowNumbersCheckText", "Нумерувати точки на карті");
                this.mgrsToolTip.SetToolTip(this.MgrsNotationTextBox, _context.AltRightToMove);
                this.utmToolTip.SetToolTip(this.UTMNotationTextBox, _context.AltRightToMove);

                //Tool Tips
                this.MapPointToolButton.ToolTipText = _context.ToolButton;
                this.MoveToCenterButton.ToolTipText = _context.MoveToCenterButton;
                this.MoveToTheProjectedCoordButton.ToolTipText = _context.MoveToProjectedCoordButton;
                this.CopyButton.ToolTipText = _context.CopyButton;
                this.ClearGridButton.ToolTipText = _context.ClearGridButton;
                this.SaveGridPointsButton.ToolTipText = _context.SaveButton;
                this.CopyGridPointButton.ToolTipText = _context.CopyButton;
                this.OpenFileGridButton.ToolTipText = _context.OpenFileButton;
                this.PointsGridView.Columns[Constants.HighlightColumnName].ToolTipText = _context.ShowPointOnMapButton;
                this.PointsGridView.Columns[Constants.DeleteColumnName].ToolTipText = _context.DeletePointButton;
                this.CurrentCoordsCopyButton.ToolTipText = _context.CopyCoordinateButton;
                this.WgsGeoCopyButton.ToolTipText = _context.CopyCoordinateButton;
                this.WgsProjCopyButton.ToolTipText = _context.CopyCoordinateButton;
                this.PulkovoGeoCopyButton.ToolTipText = _context.CopyCoordinateButton;
                this.PulkovoProjCopyButton.ToolTipText = _context.CopyCoordinateButton;
                this.UkraineGeoCopyButton.ToolTipText = _context.CopyCoordinateButton;
                this.UkraineProjCopyButton.ToolTipText = _context.CopyCoordinateButton;
                this.MgrsCopyButton.ToolTipText = _context.CopyCoordinateButton;
                this.CurrentCoordsPasteButton.ToolTipText = _context.PasteCoordinateButton;
                this.WgsGeoPasteButton.ToolTipText = _context.PasteCoordinateButton;
                this.WgsProjPasteButton.ToolTipText = _context.PasteCoordinateButton;
                this.PulkovoGeoPasteButton.ToolTipText = _context.PasteCoordinateButton;
                this.PulkovoProjPasteButton.ToolTipText = _context.PasteCoordinateButton;
                this.UkraineGeoPasteButton.ToolTipText = _context.PasteCoordinateButton;
                this.UkraineProjPasteButton.ToolTipText = _context.PasteCoordinateButton;
                this.MgrsPasteButton.ToolTipText = _context.PasteCoordinateButton;
                this.upPointMoveButton.ToolTipText = _context.FindLocalizedElement("MoveUpButtonToolTip", "Перемістити виділені точки по списку вгору");
                this.downPointMoveButton.ToolTipText = _context.FindLocalizedElement("MoveDownButtonTitle", "Помістити у початок");
                this.upToFirstStripItem.Text = _context.FindLocalizedElement("MoveToFirstButtonToolTip", "Перемістити виділені точки по списку вниз");
                this.toDownStripItem.Text = _context.FindLocalizedElement("MoveToLastButtonTitle", "Помістити у кінець");
                this.renumberButton.ToolTipText = _context.FindLocalizedElement("RenumberButtonToolTip", "Обновити нумерацію точок");
                this.panToLineButton.ToolTipText = _context.FindLocalizedElement("PanToLineButtonToolTip", "Показати лінію на карті");
                this.refreshGraphicsButton.ToolTipText = _context.FindLocalizedElement("RefreshGraphicButtonToolTip", "Оновити графіку");
                this.toolTip.SetToolTip(btnRefreshGraphic, _context.FindLocalizedElement("RefreshGraphicButtonToolTip", "Оновити графіку"));
                
                SetSCComboBoxItems();
            }
            catch
            {
                MessageBox.Show(
                    "No Localization.xml found or there is an error during loading. Coordinates Converter window is not fully localized."
                    );
            }
        }

        private void SetCoordSystems()
        {
            _coordinateSystems.Clear();

            _coordinateSystems.Add(CoordinateSystemsEnum.MapSystem, _context.CurrentMapLabel);
            _coordinateSystems.Add(CoordinateSystemsEnum.WGS84, $"{ _context.WgsTitleText} {_context.WgsLabel}");
            _coordinateSystems.Add(CoordinateSystemsEnum.WGS84UTM, $"{ _context.WgsTitleText} {CurrentProjectionsModel.WGS84Projection.Name}");
            _coordinateSystems.Add(CoordinateSystemsEnum.SK42, $"{_context.PulkovoTitleText} {_context.PulkovoLabel}");
            _coordinateSystems.Add(CoordinateSystemsEnum.Pulkovo42Projected, $"{_context.PulkovoTitleText} {CurrentProjectionsModel.Pulkovo1942Projection.Name}");
            _coordinateSystems.Add(CoordinateSystemsEnum.Ukraine2000, $"{_context.UkraineTitleText} {_context.UkraineLabel}");
            _coordinateSystems.Add(CoordinateSystemsEnum.UkraineProjected, $"{_context.PulkovoTitleText} {CurrentProjectionsModel.Ukraine2000Projection.Name}");
            _coordinateSystems.Add(CoordinateSystemsEnum.MGRS, _context.MgrsName);
        }

        private void SetSCComboBoxItems()
        {
            SetCoordSystems();

            cmbCoordSystem.Items.Clear();
            cmbCoordSystem.Items.AddRange(_coordinateSystems.Values.ToArray());
            cmbCoordSystem.SelectedItem = _coordinateSystems[CoordinateSystemsEnum.WGS84];
        }

        private void ProjectPointAsync(
            IPoint inputPoint,
            bool fromUserInput = false,
            string pointGuid = null,
            int? pointNumber = null)
        {
            lastProjectedPoint = new ExtendedPointModel();

            if (inputPoint == null) throw new ArgumentNullException(nameof(inputPoint));
            if (inputPoint.SpatialReference == null)
                throw new NullReferenceException($"Point with ID = {inputPoint.ID} has no spatial reference.");

            ManageCurrentMapCoordinates(inputPoint, fromUserInput, lastProjectedPoint);

            var wgsDD = ManageWgsDecimalDegrees(inputPoint, lastProjectedPoint);

            var pulkovoDD = ManagePulkovoDecimalDegrees(inputPoint, wgsDD, lastProjectedPoint);

            var ukraineDD = ManageUkraineDecimalDegrees(inputPoint, wgsDD, lastProjectedPoint);

            ManageWgsCoordinates(wgsDD, lastProjectedPoint);

            ManagePulkovoCoordinates(inputPoint, pulkovoDD, lastProjectedPoint);

            ManageUkraineCoordinates(inputPoint, ukraineDD, lastProjectedPoint);

            var guid = pointGuid ?? Guid.NewGuid().ToString();

            if (!fromUserInput && !string.IsNullOrWhiteSpace(guid))
                pointModels.Add(
                    Guid.Parse(pointGuid),
                    new PointModel
                    {
                        Number = pointNumber,
                        Longitude = lastProjectedPoint.WgsXCoordDD,
                        Latitude = lastProjectedPoint.WgsYCoordDD
                    });
        }

        private void ManageUkraineCoordinates(IPoint inputPoint, IPoint ukraineDD, ExtendedPointModel lastProjectedPoint)
        {
            IPoint ukrainePoint;
            if (UkraineXCoordinateTextBox.Modified || UkraineYCoordinateTextBox.Modified)
            {
                ukrainePoint = inputPoint;
                UkraineXCoordinateTextBox.Modified = false;
                UkraineYCoordinateTextBox.Modified = false;
            }
            else
            {
                ukrainePoint = _businessLogic.ProjectPoint(ukraineDD, CurrentProjectionsModel.Ukraine2000Projection);
            }
            UkraineXCoordinateTextBox.Text = ukrainePoint.X.ToIntegerString();
            UkraineYCoordinateTextBox.Text = ukrainePoint.Y.ToIntegerString();
            lastProjectedPoint.UkraineXCoord = ukrainePoint.X.ToInteger();
            lastProjectedPoint.UkraineYCoord = ukrainePoint.Y.ToInteger();
        }

        private void ManagePulkovoCoordinates(IPoint inputPoint, IPoint pulkovoDD, ExtendedPointModel lastProjectedPoint)
        {
            IPoint pulkovoPoint;
            if (PulkovoXCoordinateTextBox.Modified || PulkovoYCoordinateTextBox.Modified)
            {
                pulkovoPoint = inputPoint;
                PulkovoXCoordinateTextBox.Modified = false;
                PulkovoYCoordinateTextBox.Modified = false;
            }
            else
            {
                pulkovoPoint = _businessLogic.ProjectPoint(pulkovoDD, CurrentProjectionsModel.Pulkovo1942Projection);
            }
            PulkovoXCoordinateTextBox.Text = pulkovoPoint.X.ToIntegerString();
            PulkovoYCoordinateTextBox.Text = pulkovoPoint.Y.ToIntegerString();
            lastProjectedPoint.PulkovoXCoord = pulkovoPoint.X.ToInteger();
            lastProjectedPoint.PulkovoYCoord = pulkovoPoint.Y.ToInteger();
        }

        private void ManageWgsCoordinates(IPoint wgsDD, ExtendedPointModel lastProjectedPoint)
        {
            var wgsPoint = _businessLogic.ProjectPoint(wgsDD, CurrentProjectionsModel.WGS84Projection);
            WgsXCoordinateTextBox.Text = wgsPoint.X.ToIntegerString();
            WgsYCoordinateTextBox.Text = wgsPoint.Y.ToIntegerString();
            lastProjectedPoint.WgsXCoord = wgsPoint.X.ToInteger();
            lastProjectedPoint.WgsYCoord = wgsPoint.Y.ToInteger();

            MgrsNotationTextBox.Text = (_businessLogic.ConvertToMgrs(wgsPoint))?.ToSeparatedMgrs();
            lastProjectedPoint.MgrsRepresentation = MgrsNotationTextBox.Text;
        }

        private IPoint ManageUkraineDecimalDegrees(IPoint inputPoint, IPoint wgsDD, ExtendedPointModel lastProjectedPoint)
        {
            IPoint ukraineDD;

            if (ukraineDMSXTextBox.Modified || ukraineDMSYTextBox.Modified)
            {
                ukraineDD = inputPoint;
                ukraineDMSXTextBox.Modified = false;
                ukraineDMSYTextBox.Modified = false;
            }
            else
            {
                ukraineDD = _businessLogic.ProjectWgsToUrkaine2000WithGeoTransformation(
                    wgsDD, Constants.UkraineGeoModel, esriTransformDirection.esriTransformForward);
            }

            ukraineDMSXTextBox.Text = ukraineDD.X.ToRoundedString();
            ukraineDMSYTextBox.Text = ukraineDD.Y.ToRoundedString();
            lastProjectedPoint.UkraineXCoordDD = ukraineDD.X.ToRoundedDouble();
            lastProjectedPoint.UkraineYCoordDD = ukraineDD.Y.ToRoundedDouble();

            return ukraineDD;
        }

        private IPoint ManagePulkovoDecimalDegrees(IPoint inputPoint, IPoint wgsDD, ExtendedPointModel lastProjectedPoint)
        {
            IPoint pulkovoDD;

            if (pulkovoDMSXTextBox.Modified || pulkovoDMSYTextBox.Modified)
            {
                pulkovoDD = inputPoint;
                pulkovoDMSXTextBox.Modified = false;
                pulkovoDMSYTextBox.Modified = false;
            }
            else
            {
                pulkovoDD = _businessLogic.ProjectWgsToPulkovoWithGeoTransformation(
                    wgsDD, Constants.PulkovoGeoModel, esriTransformDirection.esriTransformForward);
            }

            pulkovoDMSXTextBox.Text = pulkovoDD.X.ToRoundedString();
            pulkovoDMSYTextBox.Text = pulkovoDD.Y.ToRoundedString();
            lastProjectedPoint.PulkovoXCoordDD = pulkovoDD.X.ToRoundedDouble();
            lastProjectedPoint.PulkovoYCoordDD = pulkovoDD.Y.ToRoundedDouble();

            return pulkovoDD;
        }

        private IPoint ManageWgsDecimalDegrees(IPoint inputPoint, ExtendedPointModel lastProjectedPoint)
        {
            IPoint wgsDD;

            if (ukraineDMSXTextBox.Modified || ukraineDMSYTextBox.Modified)
            {
                wgsDD = _businessLogic.ProjectWgsToUrkaine2000WithGeoTransformation(
                    inputPoint, Constants.WgsGeoModel, esriTransformDirection.esriTransformReverse);
            }
            else if (pulkovoDMSXTextBox.Modified || pulkovoDMSYTextBox.Modified)
            {
                wgsDD = _businessLogic.ProjectWgsToPulkovoWithGeoTransformation(
                    inputPoint, Constants.WgsGeoModel, esriTransformDirection.esriTransformReverse);
            }
            else
            {
                wgsDD = _businessLogic.ConvertToDecimalDegrees(inputPoint, Constants.WgsGeoModel);
            }

            wgsDMSXTextBox.Text = wgsDD.X.ToRoundedString();
            wgsDMSYTextBox.Text = wgsDD.Y.ToRoundedString();
            lastProjectedPoint.WgsXCoordDD = wgsDD.X.ToRoundedDouble();
            lastProjectedPoint.WgsYCoordDD = wgsDD.Y.ToRoundedDouble();

            ManageProjectedCoordinateSystems(wgsDD.X);

            return wgsDD;
        }

        private void ManageCurrentMapCoordinates(IPoint inputPoint, bool fromUserInput, ExtendedPointModel lastProjectedPoint)
        {
            var currentMapPoint = new PointClass
            {
                X = inputPoint.X,
                Y = inputPoint.Y,
                SpatialReference = inputPoint.SpatialReference
            };

            if (fromUserInput) currentMapPoint.Project(FocusMapSpatialReference);

            XCoordinateTextBox.Text = currentMapPoint.X.ToIntegerString();
            YCoordinateTextBox.Text = currentMapPoint.Y.ToIntegerString();
            lastProjectedPoint.XCoord = currentMapPoint.X.ToInteger();
            lastProjectedPoint.YCoord = currentMapPoint.Y.ToInteger();
        }

        private void ManageProjectedCoordinateSystems(double longitudeValue)
        {
            var currentModelWgs = CurrentProjectionsModel.WGS84Projection.Name;

            var offset = longitudeValue - CurrentProjectionsModel.WGS84Projection.FalseOriginX;
            var currentIndex = Constants.ProjectionsModels.IndexOf(CurrentProjectionsModel);

            if (currentIndex < 0) return;
            if (offset == 0) return;

            var indexOffset = (int)Math.Abs(offset / 6);
            var newIndex = offset < 0 ? currentIndex - indexOffset - 1 : currentIndex + indexOffset;

            if (newIndex <= 0)
                CurrentProjectionsModel = Constants.ProjectionsModels.First();
            else
                if (newIndex >= Constants.ProjectionsModels.Count - 1)
                CurrentProjectionsModel = Constants.ProjectionsModels.Last();
            else
                CurrentProjectionsModel = Constants.ProjectionsModels[newIndex];

            if(!currentModelWgs.Equals(CurrentProjectionsModel.WGS84Projection.Name))
            {
                this.wgsProjectedLabel.Text = CurrentProjectionsModel.WGS84Projection.Name;
                this.PulkovoProjectedLabel.Text = CurrentProjectionsModel.Pulkovo1942Projection.Name;
                this.UkraineProjectedLabel.Text = CurrentProjectionsModel.Ukraine2000Projection.Name;

                SetSCComboBoxItems();
            }
        }

        private void ProcessPointAsClicked(IPoint point, bool projectPoint, bool forbidLineDrawing = false)
        {
            var drawLine = forbidLineDrawing ? true : chkShowLine.Checked;
            var pointGuid = AddPointToList(point, drawLine);

            if (pointGuid != Guid.Empty)
            {
                maxNum = (int)PointsGridView.Rows[0].Cells[0].Value;
                foreach (DataGridViewRow row in PointsGridView.Rows)
                {
                    var num = (int)row.Cells[0].Value;
                    if (num > maxNum)
                    {
                        maxNum = num;
                    }
                }

                maxNum++;
            }

            var pointNumber = maxNum;
            AddPointToGrid(point, pointNumber, pointGuid);
            if (projectPoint)
            {
                ProjectPointAsync(point, false, pointGuid.ToString(), pointNumber);
            }
        }

        private void ProcessPointAsClicked(
            PointModel pointModel, CoordinateSystemModel coordinateSystem, bool createGeoCoordinateSystem)
        {
            var point = _businessLogic.CreatePoint(
                pointModel.Longitude,
                pointModel.Latitude,
                coordinateSystem,
                createGeoCoordinateSystem);

            EsriTools.ProjectToMapSpatialReference(point, FocusMapSpatialReference);

            var pointGuid = AddPointToList(point, chkShowLine.Checked);
            if (pointGuid != Guid.Empty)
            {
                pointModels.Add(pointGuid, pointModel);
                if(maxNum < PointsGridView.RowCount)
                {
                    maxNum = PointsGridView.RowCount + 1;
                }
                else
                {
                    maxNum++;
                }

                var pointNumber = maxNum;

                AddPointToGrid(point, pointNumber, pointGuid);
            }
        }

        private void ProcessPointAsClicked(
            GeoCalcPoint geoPoint, CoordinateSystemModel coordinateSystem, bool createGeoCoordinateSystem)
        {
            var point = _businessLogic.CreatePoint(
                geoPoint.X,
                geoPoint.Y,
                coordinateSystem,
                createGeoCoordinateSystem);

            EsriTools.ProjectToMapSpatialReference(point, FocusMapSpatialReference);

            var pointGuid = AddPointToList(point, chkShowLine.Checked, geoPoint.Id.ToString(), geoPoint.PointNumber);
            if(pointGuid != Guid.Empty)
            {
                var pointModel = new PointModel { Latitude = geoPoint.Y, Longitude = geoPoint.X, Number = geoPoint.PointNumber };
                pointModels.Add(pointGuid, pointModel);

                AddPointToGrid(point, geoPoint.PointNumber, pointGuid);
            }
        }

        private Guid AddPointToList(IPoint point, bool drawLine, string guid = null, int pointNumber = -1)
        {
            log.DebugEx("> AddPointToList START. point.X:{0} point.Y:{1}", point.X, point.Y);
            if (point != null && !point.IsEmpty)
            {
                bool fExists = false;
                foreach (var p in ClickedPointsDictionary)
                {
                    var metre = ArcMapHelper.GetMetresInMapUnits(1);
                    if((Math.Abs(p.Value.X - point.X) < metre) && (Math.Abs(p.Value.Y - point.Y) < metre))
                    {
                        fExists = true;
                    }
                }

                if (!fExists)
                {
                    var color = (IColor)new RgbColorClass() { Green = 255 };

                    log.DebugEx("AddPointToList. point.X:{0} point.Y:{1}", point.X, point.Y);

                    var pointNum = (pointNumber == -1) ? PointsGridView.RowCount + 1 : pointNumber;
                    GraphicsLayerManager graphicsLayerManager = GraphicsLayerManager.GetGraphicsLayerManager(ArcMap.Document.ActiveView);

                    var placedPoint = graphicsLayerManager.AddGraphicToMap(
                        point, 
                        color,
                        pointNum,
                        chkShowNumbers.Checked,
                        _textName,
                        guid, 
                        esriSimpleMarkerStyle.esriSMSCross, 
                        16);

                    if(drawLine == true && PointsGridView.RowCount > 0)
                    {
                        var fromPointGuid = (Guid)PointsGridView.Rows[PointsGridView.RowCount - 1].Tag;
                        graphicsLayerManager.AddLineSegmentToMap(ClickedPointsDictionary[fromPointGuid], point, _lineName, fromPointGuid.ToString());
                    }

                    log.DebugEx("AddPointToList. placedPoint.Value.X:{0} placedPoint.Value.Y:{1} placedPoint.Key:{2}"
                        , placedPoint.Value.X, placedPoint.Value.Y, placedPoint.Key);

                    ClickedPointsDictionary.Add(placedPoint.Key, point);
                    //NIKOL ClickedPointsDictionary.Add(placedPoint.Key, placedPoint.Value);
                    return placedPoint.Key;
                }
            }
            return Guid.Empty;
        }

        private RadioButtonsValues ShowExportForm(bool isExport)
        {
            var exportForm = new ExportForm
            {
                Text = isExport? _context.SaveAs : _context.FindLocalizedElement("ImportTitle", "Імпорт з")
            };

            if(exportForm.ShowDialog(this) == DialogResult.OK)
            {
                if(exportForm.ChosenRadioButton == Enums.RadioButtonsValues.Layer)
                {
                    return exportForm.ChosenRadioButton;
                }

                if(exportForm.ChosenRadioButton == Enums.RadioButtonsValues.XML)
                {
                    saveFileDialog.Filter = "XML Files (*.xml)|*.xml";
                    saveFileDialog.DefaultExt = "xml";
                }

                else if(exportForm.ChosenRadioButton == Enums.RadioButtonsValues.CSV)
                {
                    saveFileDialog.Filter = "CSV Files (*.csv)|*.csv";
                    saveFileDialog.DefaultExt = "csv";
                }

                saveFileDialog.AddExtension = true;
            }
            return exportForm.ChosenRadioButton;
        }
        
        private void AddPointToGrid(IPoint point, int pointNumber, Guid key)
        {
            var pointCopy = new PointClass { SpatialReference = point.SpatialReference };
            pointCopy.PutCoords(point.X, point.Y);

            var wgsPoint = _businessLogic.ConvertToWgsMeters(pointCopy);

            if (cmbCoordSystem.SelectedItem.ToString() == _coordinateSystems[CoordinateSystemsEnum.MGRS])
            {
                var coords = GetMgrsPointCoords(wgsPoint);
                AddPointStringToGrid(pointNumber, key, coords);
            }
            else
            {
                var projectedPoint = ProjectWgsPointToSelectedCoordinateSystem(wgsPoint);
                var coords = GetCoordsFormattedStrings(projectedPoint);
                AddPointStringToGrid(pointNumber, key, coords[0], coords[1]);
            }
        }

        private void AddPointStringToGrid(int pointNumber, Guid key, string x, string y = null)
        {
            var newRow = new DataGridViewRow();
            newRow.Tag = key;

            newRow.Cells.Add(new DataGridViewTextBoxCell() { Value = pointNumber });
            newRow.Cells.Add(new DataGridViewTextBoxCell() { Value = x });

            if (y != null)
            {
                newRow.Cells.Add(new DataGridViewTextBoxCell() { Value = y });
                PointsGridView.Columns["YCoordColumn"].Visible = true;
                PointsGridView.Columns["XCoordColumn"].HeaderText = _context.FindLocalizedElement("XCoordColHeader", "Довгота");
            }
            else
            {
                newRow.Cells.Add(new DataGridViewTextBoxCell() { Value = string.Empty });
                PointsGridView.Columns["YCoordColumn"].Visible = false;
                PointsGridView.Columns["XCoordColumn"].HeaderText = _context.FindLocalizedElement("CoordsColHeader", "Координати");
            }

            newRow.Cells.Add(new DataGridViewImageCell()
            {
                Value = new Bitmap(Image.FromFile(System.IO.Path.Combine(
                    System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                    @"Images\LocatePoint.png")), 16, 16),
                ToolTipText = _context.ShowPointOnMapButton
            });
            newRow.Cells.Add(new DataGridViewImageCell()
            {
                Value = new Bitmap(Image.FromFile(System.IO.Path.Combine(
                    System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                    @"Images\DeletePoint.png")), 16, 16),
                ToolTipText = _context.DeletePointButton
            });

            PointsGridView.Rows.Add(newRow);
            PointsGridView.ClearSelection();
            PointsGridView.Refresh();
        }

        

        private GeoCalcPoint GetGeoCalcPoint(Guid pointGuid, int number)
        {
            try
            {
                var pointInMap = ClickedPointsDictionary[pointGuid];
                var point = pointInMap.CloneWithProjecting();

                return new GeoCalcPoint { Id = pointGuid, PointNumber = Convert.ToInt16(number), UserName = Environment.UserName, X = point.X, Y = point.Y };
            }
            catch(Exception ex)
            {
                log.WarnEx($"Unable to cast point to GeoCalcPoint. Exception: {ex.Message}");
                return null;
            }
        }

        #endregion

        private void CmbCoordSystem_SelectedIndexChanged(object sender, EventArgs e)
        {
            var points = ClickedPointsDictionary.Values.ToArray();
            DataGridViewRow[] rows = new DataGridViewRow[PointsGridView.RowCount];
            PointsGridView.Rows.CopyTo(rows, 0);

            PointsGridView.Rows.Clear();

            foreach(DataGridViewRow row in rows)
            {
                if(row.Tag == null)
                {
                    continue;
                }

                var pointGuid = (Guid)row.Tag;
                var point = ClickedPointsDictionary[pointGuid];
                AddPointToGrid(point, (int)row.Cells[0].Value, pointGuid);
            }
        }

        private IPoint ProjectWgsPointToSelectedCoordinateSystem(IPoint point)
        {
            var pointCopy = new PointClass { SpatialReference = point.SpatialReference };
            pointCopy.PutCoords(point.X, point.Y);

            if (cmbCoordSystem.SelectedItem.ToString() != _coordinateSystems[CoordinateSystemsEnum.WGS84])
            {
                if (cmbCoordSystem.SelectedItem.ToString() == _coordinateSystems[CoordinateSystemsEnum.MapSystem])
                {
                    pointCopy.Project(FocusMapSpatialReference);
                    return pointCopy;
                }

                if (cmbCoordSystem.SelectedItem.ToString() == _coordinateSystems[CoordinateSystemsEnum.SK42])
                {
                    return _businessLogic.ProjectWgsToPulkovoWithGeoTransformation(pointCopy, Constants.PulkovoGeoModel, esriTransformDirection.esriTransformForward);
                }

                if (cmbCoordSystem.SelectedItem.ToString() == _coordinateSystems[CoordinateSystemsEnum.Ukraine2000])
                {
                    return _businessLogic.ProjectWgsToUrkaine2000WithGeoTransformation(
                    pointCopy, Constants.UkraineGeoModel, esriTransformDirection.esriTransformForward);
                }

                if (cmbCoordSystem.SelectedItem.ToString() == _coordinateSystems[CoordinateSystemsEnum.WGS84UTM])
                {
                    return _businessLogic.ProjectPoint(pointCopy, CurrentProjectionsModel.WGS84Projection);
                }

                if (cmbCoordSystem.SelectedItem.ToString() == _coordinateSystems[CoordinateSystemsEnum.Pulkovo42Projected])
                {
                    var sk42Point = _businessLogic.ProjectWgsToPulkovoWithGeoTransformation(pointCopy, Constants.PulkovoGeoModel, esriTransformDirection.esriTransformForward);
                    return _businessLogic.ProjectPoint(sk42Point, CurrentProjectionsModel.Pulkovo1942Projection);
                }

                if (cmbCoordSystem.SelectedItem.ToString() == _coordinateSystems[CoordinateSystemsEnum.UkraineProjected])
                {
                    var ukrainePoint = _businessLogic.ProjectWgsToUrkaine2000WithGeoTransformation(
                    pointCopy, Constants.UkraineGeoModel, esriTransformDirection.esriTransformForward);
                    return _businessLogic.ProjectPoint(ukrainePoint, CurrentProjectionsModel.Ukraine2000Projection);
                }
            }

            return point;
        }

        private string[] GetCoordsFormattedStrings(IPoint point)
        {
            if (point.X >= 1000)
            {
                return new string[2] { point.X.ToIntegerString(), point.Y.ToIntegerString() };
            }

            return new string[2] { point.X.ToRoundedString(), point.Y.ToRoundedString() };
        }


        private string GetMgrsPointCoords(IPoint point)
        {
            return (_businessLogic.ConvertToMgrs(point))?.ToSeparatedMgrs();
        }

        private void UpPointMoveButton_Click(object sender, EventArgs e)
        {
            if(PointsGridView.SelectedRows == null || PointsGridView.SelectedRows.Count == 0)
            {
                return;
            }

            DataGridViewRow[] selectedRows = new DataGridViewRow[PointsGridView.SelectedRows.Count];
            PointsGridView.SelectedRows.CopyTo(selectedRows, 0);
            var orderedRows = selectedRows.OrderBy(row => row.Index).ToArray();

            if(orderedRows.First().Index == 0)
            {
                return;
            }

            var selectedRowIndex = orderedRows[0].Index;
            var insertIndex = selectedRowIndex - 1;

            if(insertIndex <= 0)
            {
                insertIndex = 0;
            }

            MoveSelectedRows(orderedRows, selectedRowIndex, insertIndex, true);
        }

        private void DownPointMoveButton_Click(object sender, EventArgs e)
        {
            if(PointsGridView.SelectedRows == null || PointsGridView.SelectedRows.Count == 0)
            {
                return;
            }

            DataGridViewRow[] selectedRows = new DataGridViewRow[PointsGridView.SelectedRows.Count];
            PointsGridView.SelectedRows.CopyTo(selectedRows, 0);
            var orderedRows = selectedRows.OrderBy(row => row.Index).ToArray();

            if(orderedRows.Last().Index == PointsGridView.RowCount - 1)
            {
                return;
            }

            var selectedRowIndex = orderedRows[0].Index;
            var insertIndex = selectedRowIndex + orderedRows.Count();

            if(insertIndex >= PointsGridView.RowCount)
            {
                insertIndex = PointsGridView.RowCount - 1;
            }

            MoveSelectedRows(orderedRows, selectedRowIndex, insertIndex, false);
        }

        private void PointsGridView_SelectionChanged(object sender, EventArgs e)
        {
            if(PointsGridView.SelectedRows.Count > 1)
            {
                DataGridViewRow[] selectedRows = new DataGridViewRow[PointsGridView.SelectedRows.Count];
                PointsGridView.SelectedRows.CopyTo(selectedRows, 0);
                var orderedRows = selectedRows.OrderBy(row => row.Index).ToArray();

                for(int i = orderedRows[0].Index + 1; i < orderedRows[orderedRows.Count() - 1].Index; i++)
                {
                    PointsGridView.Rows[i].Selected = true;
                }
            }
        }

        private void UpToFirstStripItem_Click(object sender, EventArgs e)
        {
            if(PointsGridView.SelectedRows == null || PointsGridView.SelectedRows.Count == 0)
            {
                return;
            }

            DataGridViewRow[] selectedRows = new DataGridViewRow[PointsGridView.SelectedRows.Count];
            PointsGridView.SelectedRows.CopyTo(selectedRows, 0);
            var orderedRows = selectedRows.OrderBy(row => row.Index).ToArray();

            if(orderedRows.First().Index == 0)
            {
                return;
            }
            
            MoveSelectedRows(orderedRows, orderedRows[0].Index, 0, true);
        }

      
        private void ToDownStripItem_Click(object sender, EventArgs e)
        {
            if(PointsGridView.SelectedRows == null || PointsGridView.SelectedRows.Count == 0)
            {
                return;
            }

            DataGridViewRow[] selectedRows = new DataGridViewRow[PointsGridView.SelectedRows.Count];
            PointsGridView.SelectedRows.CopyTo(selectedRows, 0);
            var orderedRows = selectedRows.OrderBy(row => row.Index).ToArray();

            if(orderedRows.Last().Index == PointsGridView.RowCount - 1)
            {
                return;
            }

            MoveSelectedRows(orderedRows, orderedRows[0].Index, PointsGridView.RowCount - 1, false);
        }

        private void MoveSelectedRows(DataGridViewRow[] orderedRows, int selectedRowIndex, int insertIndex, bool toUp)
        {
            foreach(DataGridViewRow row in orderedRows)
            {
                var values = new object[PointsGridView.ColumnCount];
                var i = 0;

                foreach(DataGridViewCell cell in row.Cells)
                {
                    values[i] = cell.Value;
                    i++;
                }

                PointsGridView.Rows.RemoveAt(selectedRowIndex);
                PointsGridView.Rows.Insert(insertIndex, values);
                PointsGridView.Rows[insertIndex].Tag = row.Tag;

                if(toUp)
                {
                    selectedRowIndex++;
                    insertIndex++;
                }
            }

            PointsGridView.ClearSelection();

            for(int i = 0; i < orderedRows.Count(); i++)
            {
                var index = toUp ? insertIndex - i - 1 : insertIndex - i;
                PointsGridView.Rows[index].Selected = true;
            }

            RedrawLine();
        }

        private void RenumberButton_Click(object sender, EventArgs e)
        {
            var i = 1;

            foreach(DataGridViewRow row in PointsGridView.Rows)
            {
                if(row.Index == -1)
                {
                    continue;
                }

                row.Cells[0].Value = i;
                i++;
            }

            var points = new List<GeoCalcPoint>();

            foreach(DataGridViewRow row in PointsGridView.Rows)
            {
                if(row == null)
                {
                    continue;
                }

                points.Add(GetGeoCalcPoint((Guid)row.Tag, (int)row.Cells[0].Value));
            }

            controller.UpdatePoints(points);
            RedrawText();
        }

        private void RedrawLine()
        {
            if(!chkShowLine.Checked)
            {
                return;
            }

            GraphicsLayerManager.GetGraphicsLayerManager(ArcMap.Document.ActiveView).RemoveAllGeometryFromMap(_lineName, MilSpaceGraphicsTypeEnum.GeoCalculator);
            DrawLine();
        }

        private void DrawLine()
        {
            if(!chkShowLine.Checked || PointsGridView.RowCount == 0)
            {
                return;
            }

            var orderedPoints = new Dictionary<Guid, IPoint>();

            foreach(DataGridViewRow row in PointsGridView.Rows)
            {
                if(row.Tag == null)
                {
                    continue;
                }

                var pointGuid = (Guid)row.Tag;
                var pointPair = ClickedPointsDictionary.First(point => point.Key == pointGuid);
                orderedPoints.Add(pointPair.Key, pointPair.Value);
            }

            GraphicsLayerManager.GetGraphicsLayerManager(ArcMap.Document.ActiveView).AddLineToMap(orderedPoints, _lineName);
        }

        private void RedrawText()
        {
            if(!chkShowNumbers.Checked)
            {
                return;
            }

            GraphicsLayerManager.GetGraphicsLayerManager(ArcMap.Document.ActiveView).RemoveAllGeometryFromMap(_textName, MilSpaceGraphicsTypeEnum.GeoCalculator);
            DrawText();
        }

        private void DrawText()
        {
            foreach(DataGridViewRow row in PointsGridView.Rows)
            {
                if(row.Tag == null)
                {
                    continue;
                }

                var pointGuid = (Guid)row.Tag;
                var pointGeom = ClickedPointsDictionary.First(point => point.Key == pointGuid).Value;
                GraphicsLayerManager.GetGraphicsLayerManager(ArcMap.Document.ActiveView).DrawText(pointGeom, row.Cells[0].Value.ToString(), $"{_textName}{row.Tag.ToString()}", MilSpaceGraphicsTypeEnum.Calculating);
            }
        }

        private void PanToLineButton_Click(object sender, EventArgs e)
        {
            var orderedPoints = new List<IPoint>();

            foreach(DataGridViewRow row in PointsGridView.Rows)
            {
                if(row.Tag == null)
                {
                    continue;
                }

                var pointGuid = (Guid)row.Tag;
                var pointGeom = ClickedPointsDictionary.First(point => point.Key == pointGuid).Value;
                orderedPoints.Add(pointGeom);
            }

            ArcMapHelper.FlashLine(orderedPoints);
        }

       

        private void ChkShowLine_CheckedChanged(object sender, EventArgs e)
        {
            if(PointsGridView.RowCount == 0)
            {
                return;
            }

            if(chkShowLine.Checked)
            {
                DrawLine();
            }
            else
            {
                GraphicsLayerManager.GetGraphicsLayerManager(ArcMap.Document.ActiveView).RemoveAllGeometryFromMap(_lineName, MilSpaceGraphicsTypeEnum.GeoCalculator);
            }
        }

        private void ChkShowNumbers_CheckedChanged(object sender, EventArgs e)
        {
            if(PointsGridView.RowCount == 0)
            {
                return;
            }

            if(chkShowNumbers.Checked)
            {
                DrawText();
            }
            else
            {
                GraphicsLayerManager.GetGraphicsLayerManager(ArcMap.Document.ActiveView).RemoveAllGeometryFromMap(_textName, MilSpaceGraphicsTypeEnum.GeoCalculator);
            }
        }

        private void PointsGridView_Sorted(object sender, EventArgs e)
        {
            RedrawLine();
        }

        private void ExportToLayer()
        {
            var points = new Dictionary<int, IPoint>();
            var orderedPoints = new List<IPoint>();

            foreach(DataGridViewRow row in PointsGridView.Rows)
            {
                if(row.Tag == null)
                {
                    continue;
                }

                var pointGuid = (Guid)row.Tag;
                var pointGeom = ClickedPointsDictionary.First(point => point.Key == pointGuid).Value;
                var pointCopy = pointGeom.CloneWithProjecting();

                points.Add((int)row.Cells[0].Value, pointCopy);
            }

            for(int i = 1; i <= points.Count; i++)
            {
                orderedPoints.Add(points[i]);
            }

            controller.ExportToLayer(orderedPoints);
        }

        private void BtnRefreshGraphic_Click(object sender, EventArgs e)
        {
            if(PointsGridView.RowCount == 0)
            {
                return;
            }

            var graphicsLayerManager = GraphicsLayerManager.GetGraphicsLayerManager(ArcMap.Document.ActiveView);

            foreach (var point in ClickedPointsDictionary)
            {
                graphicsLayerManager.RemovePoint(point.Key.ToString());
            }

            var orderedPoints = new List<IPoint>();

            foreach(DataGridViewRow row in PointsGridView.Rows)
            {
                if(row.Tag == null)
                {
                    continue;
                }

                var pointGuid = (Guid)row.Tag;
                var pointGeom = ClickedPointsDictionary.First(point => point.Key == pointGuid).Value;

                var color = (IColor)new RgbColorClass() { Green = 255 };

                var placedPoint = graphicsLayerManager.AddGraphicToMap(
                    pointGeom,
                    color,
                    (int)row.Cells[0].Value,
                    chkShowNumbers.Checked,
                    _textName,
                    pointGuid.ToString(),
                    esriSimpleMarkerStyle.esriSMSCross,
                    16);
            }

            DrawLine();
        }

        private void DockableWindowGeoCalculator_Load(object sender, EventArgs e)
        {
            ArcMap.Events.OpenDocument += OnOpenDocument;
        }
    }
}
