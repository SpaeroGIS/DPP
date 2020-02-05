using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MilSpace.GeoCalculator
{
    public partial class ChooseLayerForImportForm : Form
    {
        public string SelectedLayer => lbLayers.SelectedItem.ToString();

        public ChooseLayerForImportForm()
        {
            InitializeComponent();
            InitializeLayersList();
        }

        private void InitializeLayersList()
        {
            lbLayers.Items.Clear();

            var layers = ArcMapHelper.GetFeatureLayers();

            foreach(var layer in layers)
            {
                lbLayers.Items.Add(layer);
            }

            lbLayers.SetSelected(0, true);
        }

        private void LbLayers_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
