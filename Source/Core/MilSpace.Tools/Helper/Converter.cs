using ESRI.ArcGIS.Geometry;
using MilSpace.Core.Tools;
using MilSpace.DataAccess.DataTransfer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilSpace.Core.Tools.Helper
{
    public static class Converter
    {
        public static IEnumerable<IPolyline> ConvertLinesToEsriPolypile(List<ProfileLine> profileLines, ISpatialReference spatialReference)
        {
            return profileLines.Select(l =>
            {
                var pointFrom = new Point { X = l.PointFrom.X, Y = l.PointFrom.Y, SpatialReference = EsriTools.Wgs84Spatialreference };
                var pointTo = new Point { X = l.PointTo.X, Y = l.PointTo.Y, SpatialReference = EsriTools.Wgs84Spatialreference };

                pointFrom.Project(spatialReference);
                pointTo.Project(spatialReference);

                return EsriTools.CreatePolylineFromPoints(pointFrom, pointTo);
            }
             ).ToArray();
        }
    }
}
