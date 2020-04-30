using ESRI.ArcGIS.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilSpace.DataAccess.DataTransfer.Sentinel
{
    public class SentinelFootprint
    {
        public string Uuid;
        public int Id;
        public string Identifier;
        public IPolygon Footprint;
        public int RelativeOrbit;
        public string PassDirection;
    }
}
