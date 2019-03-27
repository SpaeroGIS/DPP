using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using System;
using System.Collections.Generic;

namespace MilSpace.Core.Tools
{
    public class EsriTools
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

            return trackLine as IPolyline;
        }

    }
}
