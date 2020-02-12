using ESRI.ArcGIS.Geometry;
using MilSpace.Profile.Localization;
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

        public PointsListModalWindow(Dictionary<int, IPoint> points)
        {
            InitializeComponent();
            InitializeListView(points);

            lblLayer.Text = LocalizationContext.Instance.FindLocalizedElement("LblGetPointsFromGeoCalculator", "Список точок модулю Геокалькулятор");
            _points = points;
        }

        private void InitializeListView(Dictionary<int, IPoint> points)
        {
            lvPoints.View = View.Details;

           
            lvPoints.Items.Clear();

            foreach(var point in points)
            {
                var item = new ListViewItem(point.Key.ToString());
                item.SubItems.Add(Math.Round(point.Value.X, 5).ToString());
                item.SubItems.Add(Math.Round(point.Value.Y, 5).ToString());

                lvPoints.Items.Add(item);
            }

            lvPoints.Columns.Add(LocalizationContext.Instance.FindLocalizedElement("LvPointsFromGeoCalculatorNumberHeader", "Номер"), -2, HorizontalAlignment.Center);
            lvPoints.Columns.Add("X", -2, HorizontalAlignment.Center);
            lvPoints.Columns.Add("Y", -2, HorizontalAlignment.Center);

            lvPoints.HeaderStyle = ColumnHeaderStyle.Nonclickable;
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
