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
    public partial class GeometryFromFeatureLayerModalWindow : Form
    {
        private GeometriesFromLayerController _controller;
        private List<FromLayerGeometry> _geometries = new List<FromLayerGeometry>();
        public IGeometry SelectedGeometry;
        public string SelectedLayerName;

        public GeometryFromFeatureLayerModalWindow(IActiveView activeView)
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

            this.Text = LocalizationContext.Instance.FindLocalizedElement("ModalGeometryFromLayerTitle", "Вибір геометрії з векторного шару");
        }

        private void PopulateLayerComboBox()
        {
            cmbLayers.Items.Clear();
            cmbLayers.Items.AddRange(_controller.GetNotPointFeatureLayers(true).ToArray());

            if (cmbLayers.Items.Count > 0)
            {
                cmbLayers.SelectedIndex = 0;
            }
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

            dgvGeometries.Rows.Clear();
            dgvGeometries.Columns["DisplayFieldCol"].Visible = selectedField != null;

            foreach(var geometry in _geometries)
            {
                dgvGeometries.Rows.Add(geometry.ObjId, geometry.Title);
            }
        }

        private void CmbLayers_SelectedIndexChanged(object sender, EventArgs e)
        {
            dgvGeometries.Rows.Clear();
            PopulateFieldsComboBox(cmbLayers.SelectedItem.ToString());
            SelectedLayerName = lblLayer.Text = cmbLayers.SelectedItem.ToString();
        }

        private void CmbFields_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(cmbFields.SelectedItem == null)
            {
                return;
            }

            dgvGeometries.Columns["DisplayFieldCol"].HeaderText = cmbFields.SelectedItem.ToString();
            FillPointsGrid(cmbFields.SelectedItem.ToString());
        }

        private void BtnChoosePoint_Click(object sender, EventArgs e)
        {
            SelectedGeometry = _geometries.First(geometry => geometry.ObjId == (int)dgvGeometries.SelectedRows[0].Cells["IdCol"].Value).Geometry;
        }
    }
}
