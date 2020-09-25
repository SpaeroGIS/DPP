using MilSpace.DataAccess.DataTransfer.Sentinel;
using MilSpace.Tools.Sentinel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MilSpace.AddDem.ReliefProcessing
{
    public partial class QuaziTileSelection : Form
    {
        SentinelPairCoherence pair;
        private List<CheckBox> controls;

        public QuaziTileSelection(SentinelPairCoherence pair)
        {
            this.pair = pair;
            InitializeComponent();
            controls = new List<CheckBox>() { checkBox1, checkBox2, checkBox3, checkBox4, checkBox5, checkBox6 };
        }


        protected override void OnLoad(EventArgs e)
        {
            var quaziTiles = ProperiesManager.ComposeQuaziTileNames(pair);

            int index = 0;
            foreach (var qt in quaziTiles)
            {
                var fi = new FileInfo(qt.Key);
                var checkbox = controls[index++];
                checkbox.Text = fi.Name;
                var fontStyle = qt.Value ? FontStyle.Bold : FontStyle.Regular;
                var font = new Font(checkbox.Font.FontFamily, checkbox.Font.Size, fontStyle, GraphicsUnit.Point);
                checkbox.Font = font;
                checkbox.FlatStyle = FlatStyle.Flat;
                checkbox.Checked = !qt.Value;
            }
            ShowButtons();
        }

        private void ShowButtons()
        {
            btnOk.Enabled = controls.Any(c => c.Checked);
        }

        internal Dictionary<string, bool> QuazitilesToProcess
        {
            get
            {
                var quaziTiles = ProperiesManager.ComposeQuaziTileNames(pair);
                int index = 0;
                foreach (var qt in quaziTiles.Keys.ToArray())
                {
                    quaziTiles[qt] = controls[index++].Checked;
                }

                return quaziTiles;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            ShowButtons();
        }
    }
}
