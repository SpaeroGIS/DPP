using System;
using System.Windows.Forms;
using ESRI.ArcGIS.Geometry;
using MilSpace.ProjectionsConverter.Interfaces;
using MilSpace.ProjectionsConverter.ReferenceData;

namespace MilSpace.ProjectionsConverter.UI
{
    public partial class CoordinatesConverter : Form
    {
        private readonly IBusinessLogic _businessLogic;
        private IPoint _clickedPoint;

        private CoordinatesConverter() { }

        public CoordinatesConverter(IBusinessLogic businessLogic)
        {
            InitializeComponent();
            _businessLogic = businessLogic ?? throw new ArgumentNullException(nameof(businessLogic));
        }

        private async void CoordinatesConverter_MouseClick(object sender, MouseEventArgs e)
        {
            _clickedPoint = await _businessLogic.GetSelectedPointAsync(e.X, e.Y).ConfigureAwait(false);
            XCoordinateTextBox.Text = _clickedPoint.X.ToString();
            YCoordinateTextBox.Text = _clickedPoint.Y.ToString();
            var wgsPoint = await _businessLogic.ProjectPointAsync(_clickedPoint, (int)esriSRGeoCSType.esriSRGeoCS_WGS1984).ConfigureAwait(false);
            WgsXCoordinateTextBox.Text = wgsPoint.X.ToString();
            WgsYCoordinateTextBox.Text = wgsPoint.Y.ToString();
            var pulkovoPoint = await _businessLogic.ProjectSelectedPointAsync((int)esriSRGeoCSType.esriSRGeoCS_Pulkovo1942, e.X, e.Y).ConfigureAwait(false); 
            PulkovoXCoordinateTextBox.Text = pulkovoPoint.X.ToString();
            PulkovoYCoordinateTextBox.Text = pulkovoPoint.Y.ToString();
            //TODO: clarify why there is no Ukraine_2000 in ESRI enums
            var ukrainePoint = await _businessLogic.ProjectSelectedPointAsync(Constants.Ukraine2000ID, e.X, e.Y).ConfigureAwait(false); 
            UkraineXCoordinateTextBox.Text = ukrainePoint.X.ToString();
            UkraineYCoordinateTextBox.Text = ukrainePoint.Y.ToString();
        }

        private async void SaveButton_Click(object sender, EventArgs e)
        {
            if (_clickedPoint == null) MessageBox.Show("Please select a point on the map.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                var folderBrowserResult = saveButtonFileDialog.ShowDialog();
                if (folderBrowserResult == DialogResult.OK)
                    await _businessLogic.SaveProjectionsToXmlFileAsync(_clickedPoint, saveButtonFileDialog.FileName).ConfigureAwait(false);
            }
        }

        private async void CopyButton_Click(object sender, EventArgs e)
        {
            if (_clickedPoint == null) MessageBox.Show("Please select a point on the map.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else await _businessLogic.CopyCoordinatesToClipboardAsync(_clickedPoint).ConfigureAwait(false);            
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
