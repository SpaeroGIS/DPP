using ESRI.ArcGIS.Geometry;
using MilSpace.GeoCalculator.BusinessLogic.Models;

namespace MilSpace.GeoCalculator.BusinessLogic.ReferenceData
{
    public static class Constants
    {
        public static readonly int[] Ukraine2000ID = new int[] { 5561, 5562, 5563, 5564, 5565, 5566, 5567, 5568, 5569, 5570, 5571, 5572, 5573, 5574, 5575, 5576, 5577, 5578, 5579, 5580, 5581, 5582, 5583, 6381, 6382, 6383, 6384, 6385, 6486, 6387 };

        //CS Names
        //TODO: Take them from ESRI enums
        public static readonly string[] Ukraine2000CSNames = new string[] {"Ukraine 2000", "Ukraine 2000 GK Zone 4", "Ukraine 2000 GK Zone 5", "Ukraine 2000 GK Zone 6", "Ukraine 2000 GK Zone 7"};
        public static string WgsProjected => "WGS 1984 UTM Zone 36N";
        public static string WgsGeo => "WGS 1984";
        public static string PulkovoProjected => "Pulkovo 1942 GK Zone 6N";
        public static string PulkovoGeo => "Pulkovo 1942";

        public static readonly int NumberOfDigitsForDoubleRounding = 5;        

        //Projection Models
        public static readonly CoordinateSystemModel WgsModel = new CoordinateSystemModel((int)esriSRProjCSType.esriSRProjCS_WGS1984UTM_36N, 30.000, 0.000);
        public static readonly CoordinateSystemModel PulkovoModel = new CoordinateSystemModel((int)esriSRProjCSType.esriSRProjCS_Pulkovo1942GK_6N, 30.000, 44.330);
        public static readonly CoordinateSystemModel UkraineModel = new CoordinateSystemModel(Constants.Ukraine2000ID[2], 30.000, 43.190);
         
        //Geographics Models
        public static readonly CoordinateSystemModel WgsGeoModel = new CoordinateSystemModel((int)esriSRGeoCSType.esriSRGeoCS_WGS1984, -180.000, - 90.000, 1000000);
        public static readonly CoordinateSystemModel PulkovoGeoModel = new CoordinateSystemModel((int)esriSRGeoCSType.esriSRGeoCS_Pulkovo1942, 19.580, 35.150, 1000000);
        public static readonly CoordinateSystemModel UkraineGeoModel = new CoordinateSystemModel(Constants.Ukraine2000ID[0], 22.150, 43.190, 1000000);

        //DataGridView constants
        public const string HighlightColumnName = "HighlightColumn";
        public const string DeleteColumnName = "DeleteColumn";
        public const string NumberColumnName = "NumberColumn";
    }
}
