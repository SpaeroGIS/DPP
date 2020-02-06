using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geometry;
using MilSpace.Core;
using MilSpace.Core.ModulesInteraction;
using MilSpace.DataAccess.DataTransfer;
using MilSpace.GeoCalculator.BusinessLogic;
using MilSpace.GeoCalculator.BusinessLogic.Extensions;
using MilSpace.GeoCalculator.BusinessLogic.Interfaces;
using MilSpace.GeoCalculator.BusinessLogic.Models;
using MilSpace.GeoCalculator.BusinessLogic.ReferenceData;
using MilSpace.GeoCalculator.Enums;
using MilSpace.GeoCalculator.Interaction;
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
        private Dictionary<string, PointModel> pointModels = new Dictionary<string, PointModel>();
        private Dictionary<CoordinateSystemsEnum, string> _coordinateSystems = new Dictionary<CoordinateSystemsEnum, string>();
        private LocalizationContext context;
        private readonly Dictionary<string, IPoint> ClickedPointsDictionary = new Dictionary<string, IPoint>();
        private GeoCalculatorController controller;

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

            LocalizeComponents();

            log.InfoEx("> DockableWindowGeoCalculator (Constructor) END");
        }

        public void SetController(GeoCalculatorController controller)
        {
            this.controller = controller;
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
                    context.NoSelectedPointError,
                    context.ErrorString,
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
                        context.WrongFormatMessage + "\n" + ss,
                        context.ErrorString,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    log.DebugEx("MoveToTheProjectedCoordButton_Click " + context.WrongFormatMessage + ss);
                }

            }
            catch
            {
                MessageBox.Show(
                    context.WrongFormatMessage + " -> " + XCoordinateTextBox.Text + ";" + YCoordinateTextBox.Text,
                    context.ErrorString,
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
                        context.WrongFormatMessage,
                        context.ErrorString,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                if (!stringParts.First().ToDoubleInvariantCulture(out double xCoordinate) ||
                    !stringParts.Last().ToDoubleInvariantCulture(out double yCoordinate))
                    MessageBox.Show(
                        context.WrongFormatMessage,
                        context.ErrorString,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                else
                {
                    SetCurrentValues(xCoordinate, yCoordinate);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    context.WrongFormatMessage,
                    context.ErrorString,
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
                    string sMsgText = context.FindLocalizedElement(
                        "MsgInvalidCoordinatesDD",
                        "Недійсні дані. Потрібні коордінати представлені у десяткових градусах");
                    MessageBox.Show(
                        sMsgText + " -> " + clipboard,
                        context.ErrorString,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                //var stringParts = clipboard.ToNormalizedString().Replace(';', ' ').Split(' ');
                var stringParts = clipboard.Replace(';', ' ').Split(' ');

                if (stringParts.Length < 1)
                {
                    MessageBox.Show(
                        context.FindLocalizedElement(
                            "WrongFormatMessageToFewParts", "Не вдалося отримати обидві складові координати")
                            + " -> " + stringParts,
                        context.ErrorString,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                if (!Helper.TryParceToDouble(stringParts[0], out double xCoordinate)
                    || !Helper.TryParceToDouble(stringParts[1], out double yCoordinate))

                    //if (!stringParts[0].ToDoubleInvariantCulture(out double xCoordinate) 
                    //|| !stringParts[1].ToDoubleInvariantCulture(out double yCoordinate))

                    MessageBox.Show(
                        context.WrongFormatMessage + " WGS DD -> " + stringParts[0] + ' ' + stringParts[1],
                        context.ErrorString,
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
                    context.WrongFormatMessage,
                    context.ErrorString,
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
                        context.WrongFormatMessage + " -> " + clipboard,
                        context.ErrorString,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                if (!stringParts.First().ToDoubleInvariantCulture(out double xCoordinate) ||
                    !stringParts.Last().ToDoubleInvariantCulture(out double yCoordinate))
                    MessageBox.Show(
                        context.WrongFormatMessage,
                        context.ErrorString,
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
                    context.WrongFormatMessage,
                    context.ErrorString,
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
                    string sMsgText = context.FindLocalizedElement(
                        "MsgInvalidCoordinatesDD",
                        "Недійсні дані. Потрібні коордінати представлені у десяткових градусах");
                    MessageBox.Show(
                        sMsgText + " -> " + clipboard,
                        context.ErrorString,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }
                var stringParts = clipboard.Replace(';', ' ').Split(' ');

                //var stringParts = clipboard.ToNormalizedString().Split(' ');

                if (stringParts.Length < 2)
                {
                    MessageBox.Show(
                        context.WrongFormatMessage,
                        context.ErrorString,
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
                        context.WrongFormatMessage + " -> " + clipboard,
                        context.ErrorString,
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
                    context.WrongFormatMessage,
                    context.ErrorString,
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
                        context.WrongFormatMessage + " -> " + clipboard,
                        context.ErrorString,
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
                        context.WrongFormatMessage + " -> " + clipboard,
                        context.ErrorString,
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
                MessageBox.Show(context.WrongFormatMessage, context.ErrorString, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    string sMsgText = context.FindLocalizedElement(
                        "MsgInvalidCoordinatesDD",
                        "Недійсні дані. Потрібні коордінати представлені у десяткових градусах");
                    MessageBox.Show(
                        sMsgText + " -> " + clipboard,
                        context.ErrorString,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }
                var stringParts = clipboard.Replace(';', ' ').Split(' ');
                //var stringParts = clipboard.ToNormalizedString().Split(' ');

                if (stringParts.Length < 2)
                {
                    MessageBox.Show(
                        context.WrongFormatMessage,
                        context.ErrorString,
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
                        context.WrongFormatMessage + " -> " + clipboard,
                        context.ErrorString,
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
                    context.WrongFormatMessage,
                    context.ErrorString,
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
                        context.WrongFormatMessage,
                        context.ErrorString,
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
                        context.WrongFormatMessage,
                        context.ErrorString,
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
                    context.WrongFormatMessage,
                    context.ErrorString,
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
                    context.WrongMgrsFormatMessage,
                    context.ErrorString,
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
                MessageBox.Show(context.WrongFormatMessage, context.ErrorString, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                MessageBox.Show(context.WrongFormatMessage, context.ErrorString, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                MessageBox.Show(context.WrongFormatMessage, context.ErrorString, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                MessageBox.Show(context.WrongFormatMessage, context.ErrorString, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                MessageBox.Show(context.WrongFormatMessage, context.ErrorString, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                MessageBox.Show(context.WrongFormatMessage, context.ErrorString, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                MessageBox.Show(context.WrongFormatMessage, context.ErrorString, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                if (!UTMNotationTextBox.Modified) return;

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
            var selectedPoint = ClickedPointsDictionary.First(point => point.Key == PointsGridView.Rows[e.RowIndex].Tag.ToString());

            if (column is DataGridViewImageColumn && column.Name == Constants.HighlightColumnName)
            {
                ArcMapHelper.FlashGeometry(selectedPoint.Value, 500);
                ProjectPointAsync(selectedPoint.Value, true);
            }
            else
            if (column is DataGridViewImageColumn && column.Name == Constants.DeleteColumnName)
            {
                var prevPointGuid = PointsGridView.Rows[e.RowIndex - 1].Tag.ToString();
                var nextPointGuid = PointsGridView.Rows[e.RowIndex + 1].Tag.ToString();

                grid.Rows.RemoveAt(e.RowIndex);

                if(chkShowLine.Checked)
                {
                    ArcMapHelper.RemovePoint(selectedPoint.Key);
                    var linesToRemove = new string[] { $"{_lineName}_{prevPointGuid}" };

                    ArcMapHelper.RemoveGraphicsFromMap(linesToRemove);
                    ArcMapHelper.AddLineSegmentToMap(ClickedPointsDictionary[prevPointGuid], ClickedPointsDictionary[nextPointGuid], _lineName, prevPointGuid);
                }

                ClickedPointsDictionary.Remove(selectedPoint.Key);

                pointModels.Remove(selectedPoint.Key);

                //SynchronizePointNumbers(e.RowIndex + 1);

                //Refresh Numbers column cells values
                for (int i = 0; i < grid.Rows.Count; i++)
                {
                    grid[Constants.NumberColumnName, i].Value = i + 1;
                }
                grid.Refresh();
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
            if (pointModels == null || !pointModels.Any()) MessageBox.Show(context.NoSelectedPointError, context.ErrorString, MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                var chosenRadio = ShowExportForm();
                var folderBrowserResult = saveFileDialog.ShowDialog();
                if (folderBrowserResult == DialogResult.OK)
                {
                    if (chosenRadio == RadioButtonsValues.XML)
                        await _businessLogic.SaveProjectionsToXmlFileAsync(
                            pointModels.ToSortedPointModelsList(), saveFileDialog.FileName).ConfigureAwait(false);
                    else if (chosenRadio == RadioButtonsValues.CSV)
                        await _businessLogic.SaveProjectionsToCsvFileAsync(
                            pointModels.ToSortedPointModelsList(), saveFileDialog.FileName).ConfigureAwait(false);
                }
            }
        }

        private async void OpenFileGridButton_Click(object sender, EventArgs e)
        {
            if (PointsGridView.Rows.Count > 0)
            {
                var warningResult = MessageBox.Show(
                    context.GridCleanWarningMessage,
                    context.WarningString,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);
                if (warningResult == DialogResult.No) return;
            }

            var openFileDialogResult = openFileDialog.ShowDialog();
            var fileName = openFileDialog.FileName;
            var pointsList = new List<PointModel>();
            if (openFileDialogResult == DialogResult.OK && !string.IsNullOrWhiteSpace(fileName))
            {
                try
                {
                    if (System.IO.Path.GetExtension(fileName).Equals(Constants.CSV))
                        pointsList = await _businessLogic.ImportProjectionsFromCsvAsync(fileName);
                    else if (System.IO.Path.GetExtension(fileName).Equals(Constants.XML))
                        pointsList = await _businessLogic.ImportProjectionsFromXmlAsync(fileName);

                    ClearGridButton_Click(sender, e);

                    if (pointsList != null && pointsList.Any())
                    {
                        foreach (var pointModel in pointsList)
                        {
                            ProcessPointAsClicked(pointModel, Constants.WgsGeoModel, true);
                        }
                    }
                }
                catch
                {
                    MessageBox.Show(context.WrongFormatMessage, context.ErrorString, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void CopyGridPointButton_Click(object sender, EventArgs e)
        {
            if (pointModels == null || !pointModels.Any()) MessageBox.Show(context.NoSelectedPointError, context.ErrorString, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                foreach (var point in ClickedPointsDictionary)
                {
                    ArcMapHelper.RemovePoint(point.Key);
                }

                ClickedPointsDictionary?.Clear();
                pointModels?.Clear();
            }
            grid.Refresh();
        }
        #endregion
        #endregion

        #region ArcMap events handlers
        internal void ArcMap_OnMouseDown(int x, int y)
        {
            var clickedPoint = _businessLogic.GetSelectedPoint(x, y);
            ProcessPointAsClicked(clickedPoint, true);
        }

        internal void ArcMap_OnMouseMove(int x, int y)
        {
            var currentPoint = _businessLogic.GetSelectedPoint(x, y);
            ProjectPointAsync(currentPoint, true);
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

                this.NumberColumn.HeaderText = context.FindLocalizedElement("NumColHeader", "N");
                this.XCoordColumn.HeaderText = context.FindLocalizedElement("XCoordColHeader", "Довгота");
                this.YCoordColumn.HeaderText = context.FindLocalizedElement("YCoordColHeader", "Широта");

                //TODO:Localize the next Labels and add config here
                this.wgsProjectedLabel.Text = CurrentProjectionsModel.WGS84Projection.Name;
                this.WgsGeoLabel.Text = context.WgsLabel;
                this.PulkovoProjectedLabel.Text = CurrentProjectionsModel.Pulkovo1942Projection.Name;
                this.PulkovoGeoLabel.Text = context.PulkovoLabel;
                this.UkraineProjectedLabel.Text = CurrentProjectionsModel.Ukraine2000Projection.Name;
                this.UkraineGeoLabel.Text = context.UkraineLabel;
                this.toolsLabel.Text = context.FindLocalizedElement("ToolBarTitle", "Список точок");
                this.coordLabel.Text = context.FindLocalizedElement("CoordsSystemsLabel", "СК:");
                this.graphicLabel.Text = context.FindLocalizedElement("GraphicToolAreaTitle", "Робота з графікою");
                this.chkShowLine.Text = context.FindLocalizedElement("ShowLineCheckText", "Показати лінію на карті");
                this.chkShowNumbers.Text = context.FindLocalizedElement("ShowNumbersCheckText", "Нумерувати точки на карті");
                this.mgrsToolTip.SetToolTip(this.MgrsNotationTextBox, context.AltRightToMove);
                this.utmToolTip.SetToolTip(this.UTMNotationTextBox, context.AltRightToMove);

                //Tool Tips
                this.MapPointToolButton.ToolTipText = context.ToolButton;
                this.MoveToCenterButton.ToolTipText = context.MoveToCenterButton;
                this.MoveToTheProjectedCoordButton.ToolTipText = context.MoveToProjectedCoordButton;
                this.CopyButton.ToolTipText = context.CopyButton;
                this.ClearGridButton.ToolTipText = context.ClearGridButton;
                this.SaveGridPointsButton.ToolTipText = context.SaveButton;
                this.CopyGridPointButton.ToolTipText = context.CopyButton;
                this.OpenFileGridButton.ToolTipText = context.OpenFileButton;
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
                this.CurrentCoordsPasteButton.ToolTipText = context.PasteCoordinateButton;
                this.WgsGeoPasteButton.ToolTipText = context.PasteCoordinateButton;
                this.WgsProjPasteButton.ToolTipText = context.PasteCoordinateButton;
                this.PulkovoGeoPasteButton.ToolTipText = context.PasteCoordinateButton;
                this.PulkovoProjPasteButton.ToolTipText = context.PasteCoordinateButton;
                this.UkraineGeoPasteButton.ToolTipText = context.PasteCoordinateButton;
                this.UkraineProjPasteButton.ToolTipText = context.PasteCoordinateButton;
                this.MgrsPasteButton.ToolTipText = context.PasteCoordinateButton;
                this.upPointMoveButton.ToolTipText = context.FindLocalizedElement("MoveUpButtonToolTip", "Перемістити виділені точки по списку вгору");
                this.downPointMoveButton.ToolTipText = context.FindLocalizedElement("MoveDownButtonTitle", "Помістити у початок");
                this.upToFirstStripItem.Text = context.FindLocalizedElement("MoveToFirstButtonToolTip", "Перемістити виділені точки по списку вниз");
                this.toDownStripItem.Text = context.FindLocalizedElement("MoveToLastButtonTitle", "Помістити у кінець");
                this.renumberButton.ToolTipText = context.FindLocalizedElement("RenumberButtonToolTip", "Обновити нумерацію точок");
                this.panToLineButton.ToolTipText = context.FindLocalizedElement("PanToLineButtonToolTip", "Показати лінію на карті");

                _coordinateSystems.Add(CoordinateSystemsEnum.MapSystem, context.CurrentMapLabel);
                _coordinateSystems.Add(CoordinateSystemsEnum.WGS84, context.WgsLabel);
                _coordinateSystems.Add(CoordinateSystemsEnum.WGS84UTM, context.UtmName);
                _coordinateSystems.Add(CoordinateSystemsEnum.SK42, context.PulkovoLabel);
                _coordinateSystems.Add(CoordinateSystemsEnum.Pulkovo42Projected, CurrentProjectionsModel.Pulkovo1942Projection.Name);
                _coordinateSystems.Add(CoordinateSystemsEnum.Ukraine2000, context.UkraineLabel);
                _coordinateSystems.Add(CoordinateSystemsEnum.UkraineProjected, CurrentProjectionsModel.Ukraine2000Projection.Name);
                _coordinateSystems.Add(CoordinateSystemsEnum.MGRS, context.MgrsName);

                cmbCoordSystem.Items.Clear();
                cmbCoordSystem.Items.AddRange(_coordinateSystems.Values.ToArray());
                cmbCoordSystem.SelectedItem = _coordinateSystems[CoordinateSystemsEnum.WGS84];
            }
            catch
            {
                MessageBox.Show(
                    "No Localization.xml found or there is an error during loading. Coordinates Converter window is not fully localized."
                    );
            }
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
                    guid,
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

            this.wgsProjectedLabel.Text = CurrentProjectionsModel.WGS84Projection.Name;
            this.PulkovoProjectedLabel.Text = CurrentProjectionsModel.Pulkovo1942Projection.Name;
            this.UkraineProjectedLabel.Text = CurrentProjectionsModel.Ukraine2000Projection.Name;
        }

        private void ProcessPointAsClicked(IPoint point, bool projectPoint)
        {
            var pointGuid = AddPointToList(point, chkShowLine.Checked);
            if (pointGuid != null)
            {
                var pointNumber = ClickedPointsDictionary.Count();

                AddPointToGrid(point, pointNumber, pointGuid);
                if (projectPoint)
                {
                    ProjectPointAsync(point, false, pointGuid, pointNumber);
                }
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

            point.Project(FocusMapSpatialReference);

            var pointGuid = AddPointToList(point, chkShowLine.Checked);
            if (pointGuid != null)
            {
                pointModels.Add(pointGuid, pointModel);
                var pointNumber = ClickedPointsDictionary.Count();

                AddPointToGrid(point, pointNumber, pointGuid);
            }
        }

        private string AddPointToList(IPoint point, bool drawLine)
        {
            log.DebugEx("> AddPointToList START. point.X:{0} point.Y:{1}", point.X, point.Y);
            if (point != null && !point.IsEmpty)
            {
                bool fExists = false;
                foreach (var p in ClickedPointsDictionary)
                {
                    if ((Math.Abs(p.Value.X - point.X) < 1) && (Math.Abs(p.Value.Y - point.Y) < 1)) fExists = true;
                }

                if (!fExists)
                {
                    var color = (IColor)new RgbColorClass() { Green = 255 };

                    log.DebugEx("AddPointToList. point.X:{0} point.Y:{1}", point.X, point.Y);

                    var placedPoint = ArcMapHelper.AddGraphicToMap(
                        point, 
                        color,
                        PointsGridView.RowCount + 1,
                        chkShowNumbers.Checked,
                        _textName,
                        true, 
                        esriSimpleMarkerStyle.esriSMSCross, 
                        16);

                    if(drawLine == true && PointsGridView.RowCount > 0)
                    {
                        var fromPointGuid = PointsGridView.Rows[PointsGridView.RowCount - 1].Tag.ToString();
                        ArcMapHelper.AddLineSegmentToMap(ClickedPointsDictionary[fromPointGuid], point, _lineName, fromPointGuid);
                    }

                    log.DebugEx("AddPointToList. placedPoint.Value.X:{0} placedPoint.Value.Y:{1} placedPoint.Key:{2}"
                        , placedPoint.Value.X, placedPoint.Value.Y, placedPoint.Key);

                    ClickedPointsDictionary.Add(placedPoint.Key, point);
                    //NIKOL ClickedPointsDictionary.Add(placedPoint.Key, placedPoint.Value);

                    return placedPoint.Key;
                }
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

        private void SynchronizePointNumbers(int rowIndex)
        {
            if (pointModels == null) return;

            foreach (var pointModel in pointModels.Values)
            {
                if (pointModel.Number > rowIndex)
                {
                    pointModel.Number--;
                }
            }
        }

        private void AddPointToGrid(IPoint point, int pointNumber, string key)
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

        private void AddPointStringToGrid(int pointNumber, string key, string x, string y = null)
        {
            var newRow = new DataGridViewRow();
            newRow.Tag = key;

            newRow.Cells.Add(new DataGridViewTextBoxCell() { Value = pointNumber });
            newRow.Cells.Add(new DataGridViewTextBoxCell() { Value = x });

            if (y != null)
            {
                newRow.Cells.Add(new DataGridViewTextBoxCell() { Value = y });
                PointsGridView.Columns["YCoordColumn"].Visible = true;
                PointsGridView.Columns["XCoordColumn"].HeaderText = context.FindLocalizedElement("XCoordColHeader", "Довгота");
            }
            else
            {
                newRow.Cells.Add(new DataGridViewTextBoxCell() { Value = string.Empty });
                PointsGridView.Columns["YCoordColumn"].Visible = false;
                PointsGridView.Columns["XCoordColumn"].HeaderText = context.FindLocalizedElement("CoordsColHeader", "Координати");
            }

            newRow.Cells.Add(new DataGridViewImageCell()
            {
                Value = new Bitmap(Image.FromFile(System.IO.Path.Combine(
                    System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                    @"Images\LocatePoint.png")), 16, 16),
                ToolTipText = context.ShowPointOnMapButton
            });
            newRow.Cells.Add(new DataGridViewImageCell()
            {
                Value = new Bitmap(Image.FromFile(System.IO.Path.Combine(
                    System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                    @"Images\DeletePoint.png")), 16, 16),
                ToolTipText = context.DeletePointButton
            });

            PointsGridView.Rows.Add(newRow);
            PointsGridView.ClearSelection();
            PointsGridView.Refresh();
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

                var pointGuid = row.Tag.ToString();
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

            RedrawText();
        }

        private void RedrawLine()
        {
            if(!chkShowLine.Checked)
            {
                return;
            }

            ArcMapHelper.RemoveAllGeometryFromMap(_lineName);
            DrawLine();
        }

        private void DrawLine()
        {
            var orderedPoints = new Dictionary<string, IPoint>();

            foreach(DataGridViewRow row in PointsGridView.Rows)
            {
                if(row.Tag == null)
                {
                    continue;
                }

                var pointGuid = row.Tag.ToString();
                var pointPair = ClickedPointsDictionary.First(point => point.Key == pointGuid);
                orderedPoints.Add(pointPair.Key, pointPair.Value);
            }

            ArcMapHelper.AddLineToMap(orderedPoints, _lineName);
        }

        private void RedrawText()
        {
            if(!chkShowNumbers.Checked)
            {
                return;
            }

            ArcMapHelper.RemoveAllGeometryFromMap(_textName);
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

                var pointGuid = row.Tag.ToString();
                var pointGeom = ClickedPointsDictionary.First(point => point.Key == pointGuid).Value;
                ArcMapHelper.DrawText(pointGeom, (int)row.Cells[0].Value, row.Tag.ToString(), _textName);
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

                var pointGuid = row.Tag.ToString();
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
                ArcMapHelper.RemoveAllGeometryFromMap(_lineName);
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
                ArcMapHelper.RemoveAllGeometryFromMap(_textName);
            }
        }

        private void PointsGridView_Sorted(object sender, EventArgs e)
        {
            RedrawLine();
        }
    }
}
