using ESRI.ArcGIS.Carto;
using MilSpace.Core.Tools;
using MilSpace.Visibility.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MilSpace.Visibility
{
    public partial class ChooseVectorLayerFromMapModalWindow : Form
    {
        public string SelectedLayer => lbLayers.SelectedItem.ToString();
        private MapLayersManager _mapLayersManager;

        public ChooseVectorLayerFromMapModalWindow(IActiveView activeView)
        {
            _mapLayersManager = new MapLayersManager(activeView);

            InitializeComponent();
            InitializeLayersList();
            LocalizeString();
        }

        private void LocalizeString()
        {
            lblTitle.Text = LocalizationContext.Instance.FindLocalizedElement("LayersListLabelText", "Шари");
            btnOk.Text = LocalizationContext.Instance.FindLocalizedElement("ChooseButtonText", "Обрати");
            this.Text = LocalizationContext.Instance.FindLocalizedElement("ChooseLayerWindowCaption", "Обрати шар");
        }

        private void InitializeLayersList()
        {
            lbLayers.Items.Clear();

            var layers = _mapLayersManager.GetFeatureLayersNames();

            foreach (var layer in layers)
            {
                lbLayers.Items.Add(layer);
            }

            lbLayers.SetSelected(0, true);
        }
    }
}
