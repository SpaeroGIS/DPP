using MilSpace.Core;
using MilSpace.Core.DataAccess;
using MilSpace.Profile.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace MilSpace.Profile.ModalWindows
{
    public partial class PointsFromLayerModalWindow : Form
    {
        private PointsFromLayerController _controller = new PointsFromLayerController();
        private List<FromLayerPointModel> _points;
        internal FromLayerPointModel SelectedPoint;
        internal string LayerName;

        public PointsFromLayerModalWindow()
        {
            InitializeComponent();
            LocalizeStrings();
            PopulateLayerComboBox();
        }

        private void LocalizeStrings()
        {
            lblChooseLayer.Text = LocalizationContext.Instance.FindLocalizedElement("LblLayersText", "Шар");
            lblField.Text = LocalizationContext.Instance.FindLocalizedElement("LblFieldsText", "Поле");
            btnChoosePoint.Text = LocalizationContext.Instance.FindLocalizedElement("BtnChooseText", "Обрати");

            this.Text = LocalizationContext.Instance.FindLocalizedElement("ModalPointsFromLayerTitle", "Вибір точки з точкового шару");
        }

        private void PopulateLayerComboBox()
        {
            cmbLayers.Items.Clear();
            cmbLayers.Items.AddRange(_controller.GetPointLayers());
        }

        private void PopulateFieldsComboBox(string layerName)
        {
            var fields = _controller.GetLayerFields(layerName);

            if(fields == null || fields.Count() == 0)
            {
                cmbFields.Enabled = false;
                FillPointsGrid();
                return;
            }

            cmbFields.Items.Clear();
            cmbFields.Enabled = true;
            cmbFields.Items.AddRange(fields);
        }

        private void FillPointsGrid(string selectedField = null)
        {
            _points = _controller.GetPoints(cmbLayers.SelectedItem.ToString(), selectedField);
            if(_points == null)
            {
                return;
            }

            dgvPoints.Rows.Clear();
            dgvPoints.Columns["DisplayFieldCol"].Visible = selectedField != null;

            foreach(var point in _points)
            {
                dgvPoints.Rows.Add(point.ObjId, point.DisplayedField, point.Point.X.ToFormattedString(), point.Point.Y.ToFormattedString());
            }
        }

        private void CmbLayers_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmbFields.SelectedItem = null;
            PopulateFieldsComboBox(cmbLayers.SelectedItem.ToString());
            dgvPoints.Rows.Clear();
            lblLayer.Text = cmbLayers.SelectedItem.ToString();
        }

        private void CmbFields_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(cmbFields.SelectedItem == null)
            {
                return;
            }

            dgvPoints.Columns["DisplayFieldCol"].HeaderText = cmbFields.SelectedItem.ToString();
            FillPointsGrid(cmbFields.SelectedItem.ToString());
        }

        private void BtnChoosePoint_Click(object sender, EventArgs e)
        {
            if(dgvPoints.SelectedRows.Count > 0)
            {
                SelectedPoint = _points.First(point => point.ObjId == (int)dgvPoints.SelectedRows[0].Cells["IdCol"].Value);
                LayerName = lblLayer.Text;
            }
        }
    }
}
