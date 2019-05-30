using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geometry;
using MilSpace.ProjectionsConverter.Interfaces;
using MilSpace.ProjectionsConverter.Models;
using MilSpace.ProjectionsConverter.ReferenceData;

namespace MilSpace.ProjectionsConverter.UI
{
    public partial class CoordinatesConverter : Form
    {
        private readonly IBusinessLogic _businessLogic;
        private readonly IApplication _arcMap;
        private ProjectionsModel _projectionsModel;
        private PointModel _pointModel;

        private CoordinatesConverter() { }

        public CoordinatesConverter(IApplication arcMap, IBusinessLogic businessLogic, ProjectionsModel projectionsModel)
        {
            InitializeComponent();
            
            _arcMap = arcMap ?? throw new ArgumentNullException(nameof(arcMap));
            _businessLogic = businessLogic ?? throw new ArgumentNullException(nameof(businessLogic));
            _projectionsModel = projectionsModel ?? throw new ArgumentNullException(nameof(projectionsModel));
        }

        internal async void ArcMap_MouseDown(int x, int y)
        {
            _pointModel = new PointModel();

            var clickedPoint = await _businessLogic.GetSelectedPointAsync(x, y).ConfigureAwait(false);
            XCoordinateTextBox.Text = clickedPoint.X.ToString();            
            YCoordinateTextBox.Text = clickedPoint.Y.ToString();

            _pointModel.XCoord = clickedPoint.X;
            _pointModel.YCoord = clickedPoint.Y;

            CurrentWKIDTextBox.Text = clickedPoint.SpatialReference.FactoryCode.ToString();

            //MGRS string MUST be calculated using WGS84 projected point, thus the next lines order matters!
            var wgsPoint = await _businessLogic.ProjectPointAsync(clickedPoint, _projectionsModel.WGS84Projection).ConfigureAwait(false);
            WgsXCoordinateTextBox.Text = wgsPoint.X.ToString();
            WgsYCoordinateTextBox.Text = wgsPoint.Y.ToString();

            _pointModel.WgsXCoord = wgsPoint.X;
            _pointModel.WgsYCoord = wgsPoint.Y;

            MgrsNotationTextBox.Text = await _businessLogic.ConvertToMgrs(wgsPoint);

            _pointModel.MgrsRepresentation = MgrsNotationTextBox.Text;

            var pulkovoPoint = await _businessLogic.ProjectPointAsync(clickedPoint, _projectionsModel.Pulkovo1942Projection).ConfigureAwait(false);
            PulkovoXCoordinateTextBox.Text = pulkovoPoint.X.ToString();
            PulkovoYCoordinateTextBox.Text = pulkovoPoint.Y.ToString();

            _pointModel.PulkovoXCoord = pulkovoPoint.X;
            _pointModel.PulkovoYCoord = pulkovoPoint.Y;

            var ukrainePoint = await _businessLogic.ProjectPointAsync(clickedPoint, _projectionsModel.Ukraine2000Projection).ConfigureAwait(false);
            UkraineXCoordinateTextBox.Text = ukrainePoint.X.ToString();
            UkraineYCoordinateTextBox.Text = ukrainePoint.Y.ToString();

            _pointModel.UkraineXCoord = ukrainePoint.X;
            _pointModel.UkraineYCoord = ukrainePoint.Y;
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
            else _businessLogic.CopyCoordinatesToClipboardAsync(_pointModel);            
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {            
            Close();
        }

        private void CoordinatesConverter_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Visible = false;
            _arcMap.CurrentTool = null;
        }
    }
}
