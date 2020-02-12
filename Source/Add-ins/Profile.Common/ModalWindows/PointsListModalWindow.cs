using ESRI.ArcGIS.Geometry;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MilSpace.Profile.ModalWindows
{
    public partial class PointsListModalWindow : Form
    {
        public IPoint SelectedPoint;
        private Dictionary<int, IPoint> _points = new Dictionary<int, IPoint>();

        public PointsListModalWindow(Dictionary<int, IPoint> points, string layer)
        {
            InitializeComponent();
            InitializeListView(points);

            lblLayer.Text = layer;
            _points = points;
        }

        private void InitializeListView(Dictionary<int, IPoint> points)
        {
            lvPoints.View = View.Details;

            lvPoints.Columns.Add("Number", (int)(lvPoints.Width * 0.3));
            lvPoints.Columns.Add("X", (int)(lvPoints.Width * 0.3));
            lvPoints.Columns.Add("Y", (lvPoints.Width - (lvPoints.Columns[0].Width + lvPoints.Columns[1].Width)- 25));

            lvPoints.HeaderStyle = ColumnHeaderStyle.Nonclickable;

            lvPoints.Items.Clear();

            foreach(var point in points)
            {
                var item = new ListViewItem(point.Key.ToString());
                item.SubItems.Add(point.Value.X.ToString());
                item.SubItems.Add(point.Value.Y.ToString());

                lvPoints.Items.Add(item);
            }
        }

        private void BtnChoosePoint_Click(object sender, EventArgs e)
        {
            if(lvPoints.SelectedItems.Count > 0)
            {
                SelectedPoint = _points[Convert.ToInt32(lvPoints.SelectedItems[0].Text)];
            }
        }
    }
}
