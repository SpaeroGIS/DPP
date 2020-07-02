using ESRI.ArcGIS.Carto;
using MilSpace.Core.DataAccess;
using MilSpace.Core.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace MilSpace.Core.ModalWindows
{
    public partial class PointsFromLayerModalWindow : Form
    {
        private PointsFromLayerController _controller;
        private List<FromLayerPointModel> _points;
        private string[] _layers;
        private bool _setInputLayers;
        public FromLayerPointModel SelectedPoint;
        public string LayerName;
        public string TitleField => cmbFields.SelectedItem.ToString();

        public PointsFromLayerModalWindow(IActiveView activeView, string[] layers = null, bool setInputLayers = false)
        {
            _controller = new PointsFromLayerController(activeView);
            _setInputLayers = setInputLayers;
            _layers = layers;

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

            if (!_setInputLayers)
            {
                if (_layers == null || !_layers.Any())
                {
                    _layers = _controller.GetPointLayers(); 
                }
            }

            if (_layers == null || !_layers.Any())
            {
               
                return;
            }

            cmbLayers.Items.AddRange(_layers);
            cmbLayers.SelectedIndex = 0;
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
            cmbFields.SelectedIndex = 0;
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
            dgvPoints.Rows.Clear();
            PopulateFieldsComboBox(cmbLayers.SelectedItem.ToString());
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
            SelectPoint();
        }

        private void PointsFromLayerModalWindow_Shown(object sender, EventArgs e)
        {
            if (cmbLayers.Items.Count == 0)
            {
                MessageBox.Show(LocalizationContext.Instance.FindLocalizedElement("MsgThereAreNotAnyLayers", "У проекті відсутні відповідні шари"),
                               LocalizationContext.Instance.MessageBoxTitle);

                this.Close();
                this.DialogResult = DialogResult.Cancel;
            }
        }

        private void DgvPoints_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            SelectPoint();
            DialogResult = DialogResult.OK;
        }

        private void DgvPoints_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter && dgvPoints.SelectedRows.Count > 0)
            {
                SelectPoint();
                DialogResult = DialogResult.OK;
            }
        }

        private void SelectPoint()
        {
            if (dgvPoints.SelectedRows.Count > 0)
            {
                SelectedPoint = _points.First(point => point.ObjId == (int)dgvPoints.SelectedRows[0].Cells["IdCol"].Value);
                LayerName = lblLayer.Text;
            }
        }
    }
}
