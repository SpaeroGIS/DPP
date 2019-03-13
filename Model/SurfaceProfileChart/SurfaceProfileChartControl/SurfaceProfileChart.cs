using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using MilSpace.DataAccess.DataTransfer;

namespace SurfaceProfileChart.SurfaceProfileChartControl
{
    public partial class SurfaceProfileChart : UserControl
    {
        private ProfileSession _profileSession = new ProfileSession();

        [Category("Profile"), Description("")]
        public double PathLength { get; set; }

        [Category("Profile"), Description("")]
        public double MinAngel { get; set; }

        [Category("Profile"), Description("")]
        public double MaxAngel { get; set; }

        [Category("Profile"), Description("")]
        public double MinHeight { get; set; }

        [Category("Profile"), Description("")]
        public double MaxHeight { get; set; }

        public bool Current { get; set; }

        public SurfaceProfileChart()
        {
            Current = false;

            InitializeComponent();
        }

        private void InitializeProfile()
        {
            profile.Series.Clear();

            _profileSession = DataPreparator.Get();

            foreach (var line in _profileSession.ProfileLines)
            {
                profile.Series.Add(new Series
                {
                    ChartType = SeriesChartType.Line,
                    Color = Color.ForestGreen,
                });

                var profileSurface =
                    _profileSession.ProfileSurfaces.First(surface => surface.LineId == line.Id);

                foreach (var point in profileSurface.ProfileSurfacePoints)
                {
                    profile.Series.Last().Points.AddXY(point.Distance, point.Z);
                }
            }
        }

        private void SurfaceProfileChart_Load(object sender, EventArgs e)
        {
            profile.ChartAreas["Default"].CursorX.IsUserEnabled = true;
            profile.ChartAreas["Default"].CursorX.IsUserSelectionEnabled = true;
            profile.ChartAreas["Default"].AxisX.ScaleView.Zoomable = true;

            InitializeProfile();
        }

        private void Profile_MouseDown(object sender, MouseEventArgs e)
        {
            var selectedPoint = profile.HitTest(e.X, e.Y);

            if (selectedPoint.ChartElementType == ChartElementType.DataPoint)
            {
              //todo fire event 
            }

        }
    }
}
