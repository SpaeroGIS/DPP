using ESRI.ArcGIS.Geometry;
using MilSpace.Core.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace MilSpace.Core.ModalWindows
{
    public partial class CalcPointsForFunToPointsModalWindow : Form
    {
        private Dictionary<int, IPoint> _points = new Dictionary<int, IPoint>();
        public List<IGeometry> SelectedPoints;

        public CalcPointsForFunToPointsModalWindow(Dictionary<int, IPoint> points)
        {
            InitializeComponent();
            LocalizeStrings();
            _points = points;
            FillPointsGrid();
        }

        private void LocalizeStrings()
        {
            lblLayer.Text = LocalizationContext.Instance.FindLocalizedElement("LblGetPointsFromGeoCalculator", "Список точок модулю Геокалькулятор");
            btnChoosePoint.Text = LocalizationContext.Instance.ChooseText;
            dgvPoints.Columns["NumCol"].HeaderText = LocalizationContext.Instance.FindLocalizedElement("LvPointsFromGeoCalculatorNumberHeader", "Номер");
            this.Text = LocalizationContext.Instance.FindLocalizedElement("ModalPointsListTitle", "Список точок Геокалькулятора");
        }

        private void FillPointsGrid()
        {
            dgvPoints.Rows.Clear();

            foreach(var point in _points)
            {
                dgvPoints.Rows.Add(false, point.Key, point.Value.X, point.Value.Y);
            }
        }

        private void ChckAllPoints_CheckedChanged(object sender, EventArgs e)
        {
            foreach(DataGridViewRow row in dgvPoints.Rows)
            {
                row.Cells[0].Value = chckAllPoints.Checked;
            }
        }

        private void BtnChoosePoint_Click(object sender, EventArgs e)
        {
            SelectedPoints = new List<IGeometry>();

            foreach(DataGridViewRow row in dgvPoints.Rows)
            {
                if((bool)row.Cells[0].Value)
                {
                    SelectedPoints.Add(_points.First(point => point.Key == (int)row.Cells["NumCol"].Value).Value);
                }
            }
        }
    }
}
