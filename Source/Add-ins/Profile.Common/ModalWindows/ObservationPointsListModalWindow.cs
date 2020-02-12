using ESRI.ArcGIS.Geometry;
using MilSpace.Core.Tools;
using MilSpace.DataAccess.DataTransfer;
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
    public partial class ObservationPointsListModalWindow : Form
    {
        private List<ObservationPoint> _points = new List<ObservationPoint>();
        public IPoint SelectedPoint; 

        public ObservationPointsListModalWindow(IEnumerable<ObservationPoint> points)
        {
            InitializeComponent();

            _points = points.ToList();
            FillPointsGrid(points);
        }

        private void FillPointsGrid(IEnumerable<ObservationPoint> points)
        {
            dgvPoints.Rows.Clear();

            foreach(var point in points)
            {
                dgvPoints.Rows.Add(point.Objectid, point.Title, point.X, point.Y);
            }
        }

        private void BtnChoosePoint_Click(object sender, EventArgs e)
        {
            if(dgvPoints.SelectedRows.Count > 0)
            {
                var point = _points.First(p => p.Objectid == (int)dgvPoints.SelectedRows[0].Cells["IdCol"].Value);
                SelectedPoint = new ESRI.ArcGIS.Geometry.Point { X = point.X.Value, Y = point.Y.Value, SpatialReference = EsriTools.Wgs84Spatialreference };
            }
        }
    }
}
