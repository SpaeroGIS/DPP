using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.SystemUI;
using MilSpace.ProjectionsConverter.Interfaces;
using MilSpace.ProjectionsConverter.Models;
using MilSpace.ProjectionsConverter.ReferenceData;

namespace MilSpace.ProjectionsConverter.UI
{
    public partial class DockableWindowCoordinatesConverter : Form, ITool, IDockableWindow
    {
        private readonly IBusinessLogic _businessLogic;
        private readonly IApplication _arcMap;
        private ProjectionsModel _projectionsModel;
        private PointModel _pointModel;

        public DockableWindowCoordinatesConverter(IApplication arcMap, IBusinessLogic businessLogic, ProjectionsModel projectionsModel)
        {
            InitializeComponent();

            _arcMap = arcMap ?? throw new ArgumentNullException(nameof(arcMap));
            _businessLogic = businessLogic ?? throw new ArgumentNullException(nameof(businessLogic));
            _projectionsModel = projectionsModel ?? throw new ArgumentNullException(nameof(projectionsModel));

            LocalizeComponents();
        }

        #region ITool Implementation
        public async void OnMouseDown(int button, int shift, int x, int y)
        {
            if (button == 1 && shift == 0)
            {
                var clickedPoint = await _businessLogic.GetSelectedPointAsync(x, y).ConfigureAwait(false);
                await ProjectPointAsync(clickedPoint).ConfigureAwait(false);
            }
        }

        public async void OnMouseMove(int button, int shift, int x, int y)
        {
            if (button == 0 && shift == 0)
            {
                var currentPoint = await _businessLogic.GetSelectedPointAsync(x, y).ConfigureAwait(false);
                XCoordinateTextBox.Text = currentPoint.X.ToString();
                YCoordinateTextBox.Text = currentPoint.Y.ToString();
            }
        }

        public void OnMouseUp(int button, int shift, int x, int y)
        {

        }

        public void OnDblClick()
        {

        }

        public void OnKeyDown(int keyCode, int shift)
        {

        }

        public void OnKeyUp(int keyCode, int shift)
        {

        }

        public bool OnContextMenu(int x, int y)
        {
            throw new NotImplementedException();
        }

        public void Refresh(int hdc)
        {

        }

        public new bool Deactivate()
        {
            return true;
        }

        public new int Cursor => Cursors.Arrow.Handle.ToInt32();
        #endregion

        #region Private methods
        private void LocalizeComponents()
        {
            try
            {
                var context = new LocalizationContext();
                this.Text = context.CoordinatesConverterWindowCaption;
                this.CurrentMapLabel.Text = context.CurrentMapLabel;
                this.WgsCoordinatesLabel.Text = context.WgsLabel;
                this.PulkovoCoordinatesLabel.Text = context.PulkovoLabel;
                this.UkraineCoordinatesLabel.Text = context.UkraineLabel;
                this.MgrsNotationLabel.Text = context.MgrsLabel;
                this.SaveButton.Text = context.SaveButton;
                this.CopyButton.Text = context.CopyButton;
                this.CurrentCoordsCopyButton.Text = context.CopyButton;
                this.WgsCopyButton.Text = context.CopyButton;
                this.PulkovoCopyButton.Text = context.CopyButton;
                this.UkraineCopyButton.Text = context.CopyButton;
                this.MgrsCopyButton.Text = context.CopyButton;
                this.MoveToCenterButton.Text = context.MoveToCenterButton;
            }
            catch { MessageBox.Show("No Localization.xml found or there is an error during loading. Coordinates Converter window is not fully localized."); }
        }

        private async Task ProjectPointAsync(IPoint inputPoint)
        {
            _pointModel = new PointModel();

            if (inputPoint == null) throw new ArgumentNullException(nameof(inputPoint));
            if (inputPoint.SpatialReference == null) throw new NullReferenceException($"Point with ID = {inputPoint.ID} has no spatial reference.");

            XCoordinateTextBox.Text = inputPoint.X.ToString();
            YCoordinateTextBox.Text = inputPoint.Y.ToString();

            _pointModel.XCoord = inputPoint.X;
            _pointModel.YCoord = inputPoint.Y;

            CurrentWKIDTextBox.Text = inputPoint.SpatialReference.FactoryCode.ToString();

            //MGRS string MUST be calculated using WGS84 projected point, thus the next lines order matters!
            var wgsPoint = await _businessLogic.ProjectPointAsync(inputPoint, _projectionsModel.WGS84Projection);
            WgsXCoordinateTextBox.Text = wgsPoint.X.ToString();
            WgsYCoordinateTextBox.Text = wgsPoint.Y.ToString();

            _pointModel.WgsXCoord = wgsPoint.X;
            _pointModel.WgsYCoord = wgsPoint.Y;

            MgrsNotationTextBox.Text = await _businessLogic.ConvertToMgrs(wgsPoint);

            _pointModel.MgrsRepresentation = MgrsNotationTextBox.Text;

            var pulkovoPoint = await _businessLogic.ProjectPointAsync(inputPoint, _projectionsModel.Pulkovo1942Projection);
            PulkovoXCoordinateTextBox.Text = pulkovoPoint.X.ToString();
            PulkovoYCoordinateTextBox.Text = pulkovoPoint.Y.ToString();

            _pointModel.PulkovoXCoord = pulkovoPoint.X;
            _pointModel.PulkovoYCoord = pulkovoPoint.Y;

            var ukrainePoint = await _businessLogic.ProjectPointAsync(inputPoint, _projectionsModel.Ukraine2000Projection);
            UkraineXCoordinateTextBox.Text = ukrainePoint.X.ToString();
            UkraineYCoordinateTextBox.Text = ukrainePoint.Y.ToString();

            _pointModel.UkraineXCoord = ukrainePoint.X;
            _pointModel.UkraineYCoord = ukrainePoint.Y;

        }        
        #endregion

        #region Windows.Forms events handlers
        private async void SaveButton_Click(object sender, EventArgs e)
        {
            if (_pointModel == null) MessageBox.Show("Please select a point on the map.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            var folderBrowserResult = saveButtonFileDialog.ShowDialog();
            if (folderBrowserResult == DialogResult.OK)
                await _businessLogic.SaveProjectionsToXmlFileAsync(_pointModel, saveButtonFileDialog.FileName).ConfigureAwait(false);
        }

        private void CopyButton_Click(object sender, EventArgs e)
        {
            if (_pointModel == null) MessageBox.Show("Please select a point on the map.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else _businessLogic.CopyCoordinatesToClipboard(_pointModel);
        }

        private void CoordinatesConverter_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Visible = false;
            _arcMap.CurrentTool = null;
        }

        private async void MoveToCenterButton_Click(object sender, EventArgs e)
        {
            var centerPoint = await _businessLogic.GetDisplayCenterAsync().ConfigureAwait(false);
            await ProjectPointAsync(centerPoint).ConfigureAwait(false);
        }
        #endregion

        #region AddinImpl
        public class AddinImpl : ESRI.ArcGIS.Desktop.AddIns.DockableWindow
        {
            private DockableWindowCoordinatesConverter m_windowUI;

            public AddinImpl()
            {
            }

            internal DockableWindowCoordinatesConverter UI
            {
                get { return m_windowUI; }
            }

            protected override IntPtr OnCreateChild()
            {
                if (this.Hook is IApplication arcMap)
                {
                    m_windowUI = new DockableWindowCoordinatesConverter(arcMap,
                                                                        new BusinessLogic(arcMap, new DataExport()),
                                                                        CreateProjecstionsModelFromSettings());
                    return m_windowUI.Handle;
                }
                else return IntPtr.Zero;
            }

            protected override void Dispose(bool disposing)
            {
                if (m_windowUI != null)
                    m_windowUI.Dispose();

                base.Dispose(disposing);
            }
            private static ProjectionsModel CreateProjecstionsModelFromSettings()
            {
                return new ProjectionsModel(new SingleProjectionModel((int)esriSRProjCSType.esriSRProjCS_WGS1984UTM_36N, 30.000, 0.000),
                                            new SingleProjectionModel((int)esriSRProjCSType.esriSRProjCS_Pulkovo1942GK_6N, 30.000, 44.330),
                                            new SingleProjectionModel(Constants.Ukraine2000ID[2], 30.000, 43.190));
            }
        }
        #endregion

        #region IDockableWindow Implementation
        public void Show(bool Show)
        {
            throw new NotImplementedException();
        }

        public bool IsVisible()
        {
            throw new NotImplementedException();
        }

        void IDockableWindow.Dock(esriDockFlags dockFlags)
        {
            throw new NotImplementedException();
        }

        public string Caption { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public UID ID => new UIDClass() { Value = "{140FA266-0614-4488-980E-199C3DEF3AD8}" };

        public dynamic UserData => throw new NotImplementedException();
        #endregion

    }
}
