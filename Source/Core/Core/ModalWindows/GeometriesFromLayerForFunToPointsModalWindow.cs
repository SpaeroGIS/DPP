using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using MilSpace.Core.DataAccess;
using MilSpace.Core.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace MilSpace.Core.ModalWindows
{
    public partial class GeometriesFromLayerForFunToPointsModalWindow : Form
    {
        private GeometriesFromLayerController _controller;
        private List<FromLayerGeometry> _geometries = new List<FromLayerGeometry>();
        public List<IGeometry> SelectedGeometries;
        public string SelectedLayerName;

        public GeometriesFromLayerForFunToPointsModalWindow(IActiveView activeView)
        {
            _controller = new GeometriesFromLayerController(activeView);

            InitializeComponent();
            LocalizeStrings();
            PopulateLayerComboBox();
        }

        private void LocalizeStrings()
        {
            lblChooseLayer.Text = LocalizationContext.Instance.FindLocalizedElement("LblLayersText", "Шар");
            lblField.Text = LocalizationContext.Instance.FindLocalizedElement("LblFieldsText", "Поле");
            btnChoosePoint.Text = LocalizationContext.Instance.ChooseText;

            this.Text = LocalizationContext.Instance.FindLocalizedElement("ModalPointsFromLayerTitle", "Вибір точки з точкового шару");
        }

        private void PopulateLayerComboBox()
        {
            cmbLayers.Items.Clear();
            cmbLayers.Items.AddRange(_controller.GetNotPointFeatureLayers().ToArray());
            cmbLayers.SelectedIndex = 0;
        }

        private void PopulateFieldsComboBox(string layerName)
        {
            var fields = _controller.GetLayerFields(layerName);

            if(fields == null || fields.Length == 0)
            {
                cmbFields.Enabled = false;
                FillPointsGrid();
                return;
            }

            cmbFields.Items.Clear();
            cmbFields.Enabled = true;
            cmbFields.Items.AddRange(fields);
            cmbFields.SelectedIndex = 0;
        }

        private void FillPointsGrid(string selectedField = null)
        {
            _geometries = _controller.GetGeometries(cmbLayers.SelectedItem.ToString(), selectedField);
            if(_geometries == null)
            {
                return;
            }

            dgvPoints.Rows.Clear();
            dgvPoints.Columns["DisplayFieldCol"].Visible = selectedField != null;

            foreach(var geometry in _geometries)
            {
                dgvPoints.Rows.Add(false, geometry.ObjId, geometry.Title);
            }
        }

        private void CmbLayers_SelectedIndexChanged(object sender, EventArgs e)
        {
            dgvPoints.Rows.Clear();
            PopulateFieldsComboBox(cmbLayers.SelectedItem.ToString());
            SelectedLayerName = lblLayer.Text = cmbLayers.SelectedItem.ToString();
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
            SelectedGeometries = new List<IGeometry>();

            foreach(DataGridViewRow row in dgvPoints.Rows)
            {
                if((bool)row.Cells[0].Value)
                {
                    SelectedGeometries.Add(_geometries.FirstOrDefault(geom => geom.ObjId == (int)row.Cells["IdCol"].Value).Geometry);
                }
            }
        }

        private void ChckAllPoints_CheckedChanged(object sender, EventArgs e)
        {
            foreach(DataGridViewRow row in dgvPoints.Rows)
            {
                row.Cells[0].Value = chckAllPoints.Checked;
            }
        }
    }
}
