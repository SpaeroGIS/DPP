using ESRI.ArcGIS.Display;
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

        public static void FLashGeometry(IDisplay display, IGeometry geometry)
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

    }
}
