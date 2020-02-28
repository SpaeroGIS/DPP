using ESRI.ArcGIS.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilSpace.Core.DataAccess
{ 
    public class PointsInGeometry
    {
        public IGeometry Geometry;
        public List<IPoint> Points = new List<IPoint>();
    }
}
