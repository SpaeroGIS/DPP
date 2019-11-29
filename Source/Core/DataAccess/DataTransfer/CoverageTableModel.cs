using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilSpace.DataAccess.DataTransfer
{
    public class CoverageTableRowModel
    {
        public string ObservPointName { get; set; }
        public int ObservPointId { get; set; }
        public string ObservObjName { get; set; }
        public int ObservObjId { get; set; }
        public double ObservObjArea { get; set; }
        public double ExpectedArea { get; set; }
        public double VisibilityArea { get; set; }
        public double VisibilityPercent { get; set; }
        public double ToAllExpectedAreaPercent { get; set; }
        public double ToAllVisibilityAreaPercent { get; set; }
        public int ObservPointsSeeCount { get; set; }
    }
}
