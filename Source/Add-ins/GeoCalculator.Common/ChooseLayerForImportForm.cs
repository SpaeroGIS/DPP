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
        private LocalizationContext _context = new LocalizationContext();
        public string SelectedLayer => lbLayers.SelectedItem.ToString();

        public ChooseLayerForImportForm()
        {
            InitializeComponent();
            InitializeLayersList();
            LocalizeString();
        }

        private void LocalizeString()
        {
            lblTitle.Text = _context.FindLocalizedElement("LayersListLabel", "Шари");
            btnOk.Text = _context.FindLocalizedElement("ChooseButton", "Обрати");
            this.Text = _context.FindLocalizedElement("ChooseLayerWindowCaption", "Обрати шар");
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
    }
}
