using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Globalization;
using System.IO;
using MilSpace.Configurations;
using System.Reflection;
using ESRI.ArcGIS.Geometry;

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

    }
}