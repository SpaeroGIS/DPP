using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geometry;
using MilSpace.ProjectionsConverter.Interfaces;
using MilSpace.ProjectionsConverter.Models;
using MilSpace.ProjectionsConverter.ReferenceData;

namespace MilSpace.ProjectionsConverter.UI
{
    public partial class DockableWindowCoordinatesConverter : Form
    {
        private readonly IBusinessLogic _businessLogic;
        private readonly IApplication _arcMap;
        private ProjectionsModel _projectionsModel;
        private PointModel _pointModel;

        private DockableWindowCoordinatesConverter() { }

        public DockableWindowCoordinatesConverter(IApplication arcMap, IBusinessLogic businessLogic, ProjectionsModel projectionsModel)
        {
            InitializeComponent();            

            _arcMap = arcMap ?? throw new ArgumentNullException(nameof(arcMap));
            _businessLogic = businessLogic ?? throw new ArgumentNullException(nameof(businessLogic));
            _projectionsModel = projectionsModel ?? throw new ArgumentNullException(nameof(projectionsModel));

            LocalizeComponents();
        }

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

        internal async void ArcMap_MouseDown(int x, int y)
        {
            var clickedPoint = await _businessLogic.GetSelectedPointAsync(x, y).ConfigureAwait(false);
            await ProjectPointAsync(clickedPoint).ConfigureAwait(false);            
        }

        private async void SaveButton_Click(object sender, EventArgs e)
        {
            if(_pointModel == null) MessageBox.Show("Please select a point on the map.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

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

        private async Task ProjectPointAsync(IPoint inputPoint)
        {
            _pointModel = new PointModel();

            if (inputPoint == null) throw new ArgumentNullException(nameof(inputPoint));

            XCoordinateTextBox.Text = inputPoint.X.ToString();
            YCoordinateTextBox.Text = inputPoint.Y.ToString();

            _pointModel.XCoord = inputPoint.X;
            _pointModel.YCoord = inputPoint.Y;

            CurrentWKIDTextBox.Text = (_arcMap.Document as IActiveView)?.FocusMap?.SpatialReference?.FactoryCode.ToString();

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
    }
}
