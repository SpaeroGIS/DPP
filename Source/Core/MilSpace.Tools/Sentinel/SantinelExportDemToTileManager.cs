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
                   g.ToPoints.ToList().ForEach(wktPoint =>
                      pointCollection.AddPoint(new Point
                      {
                          X = wktPoint.Latitude,
                          Y = wktPoint.Longitude,
                          SpatialReference = EsriTools.Wgs84Spatialreference
                      }));

                   return EsriTools.GetPolygonByPointCollection(pointCollection);
               };


            var tilePolygon = geometryToPoly(tile.Geometry);
            var quaziTilesPolygons = EsriTools.GetTotalPolygon(quaziTiles.ToList().Select(qt => geometryToPoly(qt.Geometry)));

            var relationalOperator = quaziTilesPolygons as IRelationalOperator;

            return relationalOperator.Contains(tilePolygon);

        }
    }
}
