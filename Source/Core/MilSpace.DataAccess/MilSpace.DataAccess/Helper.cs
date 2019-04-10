using ESRI.ArcGIS.Geometry;
using MilSpace.DataAccess.DataTransfer;
using System;

namespace MilSpace.DataAccess
{

    public static class Helper
    {

        /// <summary>
        /// Returns the Spatial reference for points taken form GUI (WGS1984)
        /// </summary>
        /// <returns></returns>
        public static ISpatialReference GetBasePointSpatialReference()
        {
            return (new SpatialReferenceEnvironment()).CreateGeographicCoordinateSystem((int)esriSRGeoCSType.esriSRGeoCS_WGS1984);
        }

        public static string GetTemporaryNameSuffix()
        {
            return DateTime.Now.ToString("yyyyMMddHHmmss");
        }

        public static IPoint GetEsriPoint(this ProfilePoint point, ISpatialReference spatial = null)
        {
            if (point == null)
            {
                return null;
            }

            return new Point() { X = point.X, Y = point.Y, SpatialReference = spatial };
        }

    }
}