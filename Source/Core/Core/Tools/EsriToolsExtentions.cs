using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using System;

namespace MilSpace.Core.Tools
{
    public static class EsriToolsExtentions
    {
        public static WKSPointZ WKSPointZ(this IPoint point)
        {
            return new WKSPointZ
            {
                X = point.X,
                Y = point.Y,
                Z = point.Z
            };
        }

        public static double GetZValue(this IRaster raster, IPoint point)
        {
            IPoint testPoint = new PointClass()
            {
                X = point.X,
                Y = point.Y,
                SpatialReference = point.SpatialReference
            };

            testPoint.Project(((IRasterProps)raster).SpatialReference);

            IRaster2 raster2 = (IRaster2)raster;

            try
            {
                //Get the column and row by giving x,y coordinates in a map space.
                int col = raster2.ToPixelColumn(testPoint.X);
                int row = raster2.ToPixelRow(testPoint.Y);

                //Get the value at a given band.
                var pixel = raster2.GetPixelValue(0, col, row);

                return pixel != null ? (double)Convert.ChangeType(pixel, typeof(double)) : double.NaN;
            }
            catch
            {
                return double.NaN;
            }

        }

        public static void AddZCoordinate(this IPoint point, IRaster raster)
        {
            point.Z = raster.GetZValue(point);
        }
    }
}
