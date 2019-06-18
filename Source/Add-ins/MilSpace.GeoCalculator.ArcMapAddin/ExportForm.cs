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
        public RadioButtonsValues ChosenRadioButton;

        public ExportForm()
        {
            InitializeComponent();
            SetField();
        }

        private void SetField()
        {
            ChosenRadioButton = XmlFileRadio.Checked ? RadioButtonsValues.XML : RadioButtonsValues.CSV;
        }

        private void XmlFileRadion_CheckedChanged(object sender, EventArgs e)
        {
            if (XmlFileRadio.Checked) ChosenRadioButton = RadioButtonsValues.XML;
        }

        private void CsvFileRadio_CheckedChanged(object sender, EventArgs e)
        {
            if (CsvFileRadio.Checked) ChosenRadioButton = RadioButtonsValues.CSV;
        }        
    }
}
