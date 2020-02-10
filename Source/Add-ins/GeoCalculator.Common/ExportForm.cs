using MilSpace.GeoCalculator.Enums;
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
    public partial class ExportForm : Form
    {
        private LocalizationContext _context = new LocalizationContext();
        public RadioButtonsValues ChosenRadioButton;

        public ExportForm()
        {
            InitializeComponent();
            SetField();
            LocalizeString();
        }

        private void LocalizeString()
        {
            LayerRadio.Text = _context.FindLocalizedElement("FromLayerRadioText", "Шар");
        }

        private void SetField()
        {
           XmlFileRadio.Checked = true;
           ChosenRadioButton =RadioButtonsValues.XML;
        }

        private void XmlFileRadion_CheckedChanged(object sender, EventArgs e)
        {
            if (XmlFileRadio.Checked) ChosenRadioButton = RadioButtonsValues.XML;
        }

        private void CsvFileRadio_CheckedChanged(object sender, EventArgs e)
        {
            if (CsvFileRadio.Checked) ChosenRadioButton = RadioButtonsValues.CSV;
        }

        private void LayerRadio_CheckedChanged(object sender, EventArgs e)
        {
            if (LayerRadio.Checked) ChosenRadioButton = RadioButtonsValues.Layer;
        }
    }
}
