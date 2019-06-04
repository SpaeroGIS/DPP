using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilSpace.GeoCalculator.BusinessLogic.Models
{
    [Serializable]
    public class PointModel
    {
        public double XCoord { get; set; }
        public double YCoord { get; set; }
        public double WgsXCoord { get; set; }
        public double WgsYCoord { get; set; }
        public double PulkovoXCoord { get; set; }
        public double PulkovoYCoord { get; set; }
        public double UkraineXCoord { get; set; }
        public double UkraineYCoord { get; set; }
        public string MgrsRepresentation { get; set; }
        public string UtmRepresentation { get; set; }
    }
}
