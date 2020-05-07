using ESRI.ArcGIS.Carto;
using MilSpace.Core.Tools;
using MilSpace.Visibility.Localization;
using System;
using System.Windows.Forms;

namespace MilSpace.Visibility
{
    public partial class ChooseVectorLayerFromMapModalWindow : Form
    {
        public string SelectedLayer => lbLayers.SelectedItem.ToString();
        public string SelectedFiled => cmbTiltleField.SelectedItem.ToString();
        private MapLayersManager _mapLayersManager;
        private string[] _layers;

        public ChooseVectorLayerFromMapModalWindow(IActiveView activeView, bool chooseTitleField = false, string[] layers = null)
        {
            _mapLayersManager = new MapLayersManager(activeView);
            _layers = layers;

            InitializeComponent();

            chooseTitleFieldPanel.Visible = chooseTitleField;
            chooseTitleFieldPanel.Enabled = chooseTitleField;

            InitializeLayersList();
            LocalizeString();
        }

        private void LocalizeString()
        {
            lblTitle.Text = LocalizationContext.Instance.FindLocalizedElement("LayersListLabelText", "Шари");
            lblTiltleField.Text = LocalizationContext.Instance.FindLocalizedElement("Modal.LblFieldTitl.Text", "Ідентифікуюче поле");
            btnOk.Text = LocalizationContext.Instance.FindLocalizedElement("ChooseButtonText", "Обрати");
            this.Text = LocalizationContext.Instance.FindLocalizedElement("ChooseLayerWindowCaption", "Обрати шар");
        }

        private void InitializeLayersList()
        {
            lbLayers.Items.Clear();

            var layers = _layers ?? _mapLayersManager.GetFeatureLayersNames();

            foreach (var layer in layers)
            {
                lbLayers.Items.Add(layer);
            }

            lbLayers.SetSelected(0, true);
            FillFieldsComboBox();
        }

        private void LbLayers_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillFieldsComboBox();
        }

        private void FillFieldsComboBox()
        {
            if (chooseTitleFieldPanel.Enabled)
            {
                cmbTiltleField.Items.Clear();
                var layer = _mapLayersManager.GetLayer(lbLayers.SelectedItem.ToString());
                cmbTiltleField.Items.AddRange(_mapLayersManager.GetFeatureLayerFields(layer as IFeatureLayer).ToArray());
                cmbTiltleField.SelectedIndex = 0;
            }
        }
    }
}
