using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace MilSpace.AddDem.ReliefProcessing
{
    public partial class ChecekListBoxQT : CheckedListBox
    {
        public ChecekListBoxQT()
        {
            InitializeComponent();
        }

        private Dictionary<int, bool> quaziTileItems = new Dictionary<int, bool>();

        private List<int> drow = new List<int>();


        internal Dictionary<string, bool> QuaziTiles
        {
            set
            {
                quaziTiles = value;
                int index = 0;
                foreach (var qt in quaziTiles)
                {
                    var fi = new FileInfo(qt.Key);
                    quaziTileItems.Add(index++, !qt.Value);
                }
            }
        }

        private Dictionary<string, bool> quaziTiles;
        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            //if (drow.Contains(e.Index))
            //{
            //    base.OnDrawItem(e);
            //    return;
            //}

            //drow.Add(e.Index);
            bool filEexists = !quaziTileItems[e.Index];

            var ttt = filEexists && (e.Index == 2) ? FontStyle.Italic : FontStyle.Regular;
            var font = new Font(e.Font.FontFamily, e.Font.Size, ttt, GraphicsUnit.Point);

            this.Font = font;

            DrawItemEventArgs e2 = new DrawItemEventArgs(e.Graphics, font, e.Bounds, e.Index, e.State);

            base.OnDrawItem(e2);
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
        }
    }
}
