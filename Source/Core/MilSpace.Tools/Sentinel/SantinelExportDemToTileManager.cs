using ESRI.ArcGIS.Geometry;
using MilSpace.Core.Geometry;
using MilSpace.Core.Tools;
using MilSpace.DataAccess.DataTransfer.Sentinel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilSpace.Tools.Sentinel
{
    public class SantinelExportDemToTileManager
    {
        public static bool CheckTileCompleteness(Tile tile, IEnumerable<SentinelTilesCoverage> quaziTiles)
        {

            Func<IWktGeometry, IPolygon> geometryToPoly = (g) =>
               {
                   var pointCollection = new MultipointClass();
                   pointCollection.SpatialReference = EsriTools.Wgs84Spatialreference;
                   g.ToPoints.ToList().ForEach(wktPoint =>
                   {
                       var pnt = new Point
                       {
                           X = wktPoint.Longitude,
                           Y = wktPoint.Latitude,
                       };
                       pnt.SpatialReference = EsriTools.Wgs84Spatialreference;
                       pointCollection.AddPoint(pnt);
                   });

                   return EsriTools.GetPolygonByPointCollection(pointCollection);
               };


            var tilePolygon = geometryToPoly(tile.Geometry);
            var quaziTilesPolygons = EsriTools.GetTotalPolygon(quaziTiles.ToList().Select(qt => geometryToPoly(qt.Geometry)));

            var relationalOperator = quaziTilesPolygons as IRelationalOperator;

            return true || relationalOperator.Contains(tilePolygon);
        }
    }
}
