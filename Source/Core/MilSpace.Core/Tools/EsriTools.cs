using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using MilSpace.Core.Exceptions;
using System;
using System.Collections.Generic;

namespace MilSpace.Core.Tools
{
    public static class EsriTools
    {

        private static ISpatialReference wgs84 = null;

        private static Dictionary<esriGeometryType, Func<ISymbol>> symbolsToFlash = new Dictionary<esriGeometryType, Func<ISymbol>>()
        {
            { esriGeometryType.esriGeometryPoint, () => {
              //Set point props to  the flash geometry
            RgbColor rgbColor = new  RgbColor();
            rgbColor.Red = 255;
            ISimpleMarkerSymbol simpleMarkerSymbol = new SimpleMarkerSymbol()
            {
                Style = esriSimpleMarkerStyle.esriSMSCross,
                Size = 10,
                Color = rgbColor
            };


            return (ISymbol)simpleMarkerSymbol; } },

            { esriGeometryType.esriGeometryPolyline, () => {throw new NotImplementedException();} }
        };

        public static void ProjectToWgs84(IGeometry geometry)
        {
            try
            {
                geometry.Project(Wgs84Spatialreference);
            }
            catch (Exception ex)
            {
                //ToDO: Loggig
            }

        }

        /// <summary>
        /// Clone the point with projectin to Wgs84
        /// </summary>
        /// <param name="point">Clonning point</param>
        /// <returns>Point projected to Wgs84 SC</returns>
        public static IPoint CloneWithProjecting(this IPoint point)
        {

            var clonedPoint =  new Point() { X = point.X, Y = point.Y, Z = point.Z, SpatialReference = point.SpatialReference };
            ProjectToWgs84(clonedPoint);

            return clonedPoint;
        }

        public static void ProjectToMapSpatialReference(IGeometry geometry, ISpatialReference mapSpatialReference)
        {

            try
            {
                geometry.Project(mapSpatialReference);
            }
            catch (Exception ex)
            {
                //ToDO: Loggig
            }
        }


        public static ISpatialReference Wgs84Spatialreference
        {
            get
            {
                if (wgs84 == null)
                {
                    SpatialReferenceEnvironmentClass factory = new SpatialReferenceEnvironmentClass();
                    wgs84 = factory.CreateGeographicCoordinateSystem((int)esriSRGeoCSType.esriSRGeoCS_WGS1984);
                }

                return wgs84;
            }
        }

        public static void FlashGeometry(IDisplay display, IGeometry geometry)
        {
            if (symbolsToFlash.ContainsKey(geometry.GeometryType))
            {
                display.StartDrawing(display.hDC, (short)ESRI.ArcGIS.Display.esriScreenCache.esriNoScreenCache);
                var symbol = symbolsToFlash[geometry.GeometryType].Invoke();
                display.SetSymbol(symbol);
                display.DrawPoint(geometry);
                System.Threading.Thread.Sleep(100);
                display.DrawPoint(geometry);
                display.FinishDrawing();
            }
            else
            { throw new KeyNotFoundException("{0} cannot be found in the Symbol dictionary".InvariantFormat(geometry.GeometryType)); }
        }


        public static IEnumerable<IPolyline> CreatePolylinesFromPointAndAzimuths(IPoint centerPoint, double length, int count, double azimuth1, double azimuth2)
        {

            if (centerPoint == null)
            {
                return null;
            }


            if (count < 2)
            {
                //TODO: Localize error message
                throw new MilSpaceProfileLackOfParameterException("Line numbers", count);
            }


            double minAzimuth = Math.Min(azimuth1, azimuth2);
            double maxAzimuth = Math.Max(azimuth1, azimuth2);

            double sector = maxAzimuth - minAzimuth;

            if (sector == 0)
            {
                //TODO: Localize error message
                throw new MilSpaceProfileLackOfParameterException("Azimuth", 0);
            }

            double step = sector / (count - 1);

            List<IPolyline> result = new List<IPolyline>();
            for (int i = 0; i < count; i++)
            {
                IConstructPoint outPoint = new PointClass();
                double radian = (minAzimuth + (i * step)) * (Math.PI / 180);
                outPoint.ConstructAngleDistance(centerPoint, radian, length);
                result.Add(CreatePolylineFromPoints(centerPoint, outPoint as IPoint));
            }

            return result;
        }

        public static IPolyline CreatePolylineFromPoints(IPoint pointFrom, IPoint pointTo)
        {
            if (pointFrom == null || pointTo == null)
            {
                return null;
            }


            WKSPoint[] segmentWksPoints = new WKSPoint[2];
            segmentWksPoints[0].X = pointFrom.X;
            segmentWksPoints[0].Y = pointFrom.Y;
            segmentWksPoints[1].X = pointTo.X;
            segmentWksPoints[1].Y = pointTo.Y;

            IPointCollection4 trackLine = new PolylineClass();

            IGeometryBridge2 m_geometryBridge = new GeometryEnvironmentClass();
            m_geometryBridge.AddWKSPoints(trackLine, ref segmentWksPoints);


            var result = trackLine as IPolyline;

            if (pointFrom.SpatialReference != null && pointTo.SpatialReference != null && pointFrom.SpatialReference == pointTo.SpatialReference)
            {
                result.SpatialReference = pointFrom.SpatialReference;
            }

            return result;
        }

    }
}
