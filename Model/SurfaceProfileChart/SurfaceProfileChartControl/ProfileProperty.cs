using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurfaceProfileChart.SurfaceProfileChartControl
{
    public class ProfileProperty
    {
        public int LineId { get; set; }
        public double PathLength { get; set; }
        public double MinAngle { get; set; }
        public double MaxAngle { get; set; }
        public double MinHeight { get; set; }
        public double MaxHeight { get; set; }
    }
}
