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
        public double ExpectedArea { get; set; }
        public double VisibilityArea { get; set; }
        public int VisibilityPercent
        {
            get
            {
                if  (ExpectedArea == 0)
                {
                    return 0;
                }

                return Convert.ToInt32(Math.Round((VisibilityArea * 100) / ExpectedArea));
            }
        }
        public int ToAllExpectedAreaPercent { get; set; }
        public int ToAllVisibilityAreaPercent { get; set; }
        public int ObservPointsSeeCount { get; set; }
    }
}
