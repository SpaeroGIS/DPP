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

        private static Dictionary<esriGeometryType, Func<IRgbColor, ISymbol>> symbolsToFlash = new Dictionary<esriGeometryType, Func<IRgbColor, ISymbol>>()
        {
            { esriGeometryType.esriGeometryPoint, (rgbColor) => {
                  //Set point props to  the flash geometry
                ISimpleMarkerSymbol simpleMarkerSymbol = new SimpleMarkerSymbol()
                {
                    Style = esriSimpleMarkerStyle.esriSMSCross,
                    Size = 10,
                    Color = rgbColor
                };



                return (ISymbol)simpleMarkerSymbol; }
            },

            { esriGeometryType.esriGeometryPolyline, (rgbColor) => {
                //Define an arrow marker  
                IArrowMarkerSymbol arrowMarkerSymbol = new ArrowMarkerSymbol();

                arrowMarkerSymbol.Color = rgbColor;
                arrowMarkerSymbol.Size = 8;
                arrowMarkerSymbol.Length = 8;
                arrowMarkerSymbol.Width = 6;
                //Add an offset to make sure the square end of the line is hidden  
                arrowMarkerSymbol.XOffset = 0.8;

                //Create cartographic line symbol  
                ICartographicLineSymbol cartographicLineSymbol = new CartographicLineSymbol();
                cartographicLineSymbol.Color = rgbColor;
                cartographicLineSymbol.Width = 1;

                //Define simple line decoration  
                ISimpleLineDecorationElement simpleLineDecorationElement = new SimpleLineDecorationElement();
                //Place the arrow at the end of the line (the "To" point in the geometry below)  
                simpleLineDecorationElement.AddPosition(1);
                simpleLineDecorationElement.MarkerSymbol = arrowMarkerSymbol;

                //Define line decoration  
                ILineDecoration lineDecoration = new LineDecoration();
                lineDecoration.AddElement(simpleLineDecorationElement);

                //Set line properties  
                ILineProperties lineProperties = (ILineProperties)cartographicLineSymbol;
                lineProperties.LineDecoration = lineDecoration;

                return (ISymbol)cartographicLineSymbol;

            } }
        };

        private static Dictionary<esriGeometryType, Func<IDisplay, IGeometry, bool>> actionToFlash = new Dictionary<esriGeometryType, Func<IDisplay, IGeometry, bool>>()
        {
            { esriGeometryType.esriGeometryPoint, (display, geometry) => {

                    for(int i =0; i < 4; i++ )
                    {
                        display.DrawPoint(geometry);
                        System.Threading.Thread.Sleep(300);
                    }
                    return true;
                }
            },
            { esriGeometryType.esriGeometryPolyline, (display, geometry) => {

                for(int i=0; i < 4; i++ )
                    {
                        display.DrawPolyline(geometry);
                        System.Threading.Thread.Sleep(100);
                    }
                    return true;
                }
            }
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

            var clonedPoint = new Point() { X = point.X, Y = point.Y, Z = point.Z, SpatialReference = point.SpatialReference };
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
                IRgbColor color = new RgbColor();
                color.Red = 255;
                var symbol = symbolsToFlash[geometry.GeometryType].Invoke(color);

                display.StartDrawing(display.hDC, (short)ESRI.ArcGIS.Display.esriScreenCache.esriNoScreenCache);
                display.SetSymbol(symbol);
                actionToFlash[geometry.GeometryType].Invoke(display, geometry);
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



            //double minAzimuth = Math.Min(azimuth1, azimuth2);
            //double maxAzimuth = Math.Max(azimuth1, azimuth2);

                double sector;
            if (azimuth1 > azimuth2) //clockwise
            {
                sector = (360 - azimuth1) + azimuth2;
            }
            else
            {
                sector = azimuth2 - azimuth1;
            }

            if (sector == 0)
            {
                sector = 360;
            }

            double step = sector / (count - 1);

            //remove one line if the first and last is the same
            if ((azimuth1 == 0 && azimuth2 == 360) || (azimuth2 == 0 && azimuth1 == 360) || (azimuth2 == azimuth1))
            {
                count -= 1;
            }


            List<IPolyline> result = new List<IPolyline>();
            for (int i = 0; i < count; i++)
            {
                double radian = (90 - (azimuth1 + (i * step))) * (Math.PI / 180);
                IPoint outPoint = GetPointFromAngelAndDistance(centerPoint, radian, length);
                result.Add(CreatePolylineFromPoints(centerPoint, outPoint as IPoint));
            }

            return result;
        }


        /// <summary>
        /// Get new point by distance and angel
        /// </summary>
        /// <param name="basePoint">The base point</param>
        /// <param name="angel">dirrection in radians</param>
        /// <param name="length">distance to the new point</param>
        /// <returns>Esri point</returns>
        public static IPoint GetPointFromAngelAndDistance(IPoint basePoint, double angel, double length)
        {
            IConstructPoint outPoint = new PointClass();
            outPoint.ConstructAngleDistance(basePoint, angel, length);
            IPoint resilt = outPoint as IPoint;
            resilt.SpatialReference = basePoint.SpatialReference;
            return resilt;
        }


        /// <summary>
        /// Create a Polyline from two Points
        /// </summary>
        /// <param name="pointFrom">Start point</param>
        /// <param name="pointTo">End point</param>
        /// <returns>Esri poliline</returns>
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
