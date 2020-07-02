using ESRI.ArcGIS.Geometry;
using MilSpace.Core.DataAccess;
using MilSpace.Core.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace MilSpace.Core.ModalWindows
{
    public partial class PointsListModalWindow : Form
    {
        public FromLayerPointModel SelectedPoint;
        private Dictionary<int, IPoint> _points = new Dictionary<int, IPoint>();

        public PointsListModalWindow(Dictionary<int, IPoint> points)
        {
            InitializeComponent();
            InitializeListView(points);

            lblLayer.Text = LocalizationContext.Instance.FindLocalizedElement("LblGetPointsFromGeoCalculator", "Список точок модулю Геокалькулятор");
            btnChoosePoint.Text = LocalizationContext.Instance.ChooseText;
            this.Text = LocalizationContext.Instance.FindLocalizedElement("ModalPointsListTitle", "Список точок Геокалькулятора");
            _points = points;
        }

        private void InitializeListView(Dictionary<int, IPoint> points)
        {
            lvPoints.View = View.Details;
           
            lvPoints.Items.Clear();

            foreach(var point in points)
            {
                var item = new ListViewItem(point.Key.ToString());
                item.SubItems.Add(point.Value.X.ToFormattedString());
                item.SubItems.Add(point.Value.Y.ToFormattedString());

                lvPoints.Items.Add(item);
            }

            lvPoints.Columns.Add(LocalizationContext.Instance.FindLocalizedElement("LvPointsFromGeoCalculatorNumberHeader", "Номер"), -2, HorizontalAlignment.Center);
            lvPoints.Columns.Add("X", -2, HorizontalAlignment.Center);
            lvPoints.Columns.Add("Y", -2, HorizontalAlignment.Center);

            lvPoints.HeaderStyle = ColumnHeaderStyle.Nonclickable;
        }

        private void BtnChoosePoint_Click(object sender, EventArgs e)
        {
            SelectPoint();
        }

        private void LvPoints_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            SelectPoint();
            DialogResult = DialogResult.OK;
        }
        
        private void LvPoints_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyData == Keys.Enter && lvPoints.SelectedItems.Count > 0)
            {
                SelectPoint();
                DialogResult = DialogResult.OK;
            }
        }

        private void SelectPoint()
        {
            if (lvPoints.SelectedItems.Count > 0)
            {
                var pair = _points.First(point => point.Key == Convert.ToInt32(lvPoints.SelectedItems[0].Text));
                SelectedPoint = new FromLayerPointModel { Point = pair.Value, ObjId = pair.Key, DisplayedField = pair.Key.ToString() };
            }
        }
    }
}
