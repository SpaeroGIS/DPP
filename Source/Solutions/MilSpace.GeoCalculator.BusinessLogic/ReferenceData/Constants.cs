using ESRI.ArcGIS.Geometry;
using MilSpace.GeoCalculator.BusinessLogic.Models;
using System.Collections.Generic;

namespace MilSpace.GeoCalculator.BusinessLogic.ReferenceData
{
    public static class Constants
    {
        public static readonly int[] Ukraine2000ID = new int[] { 5561, 5562, 5563, 5564, 5565 };

        public static readonly int NumberOfDigitsForDoubleRounding = 5;

        //TODO: May be refactored in order to have more complex logic searching zone automatically
        //Projection Models
        public static readonly List<ProjectionsModel> ProjectionsModels = new List<ProjectionsModel>{
        new ProjectionsModel(
                             new CoordinateSystemModel((int)esriSRProjCSType.esriSRProjCS_WGS1984UTM_35N, 24.000, 0.000, "WGS 1984 UTM Zone 35N"),
                             new CoordinateSystemModel((int)esriSRProjCSType.esriSRProjCS_Pulkovo1942GK_5N, 24.000, 45.180, "Pulkovo 1942 GK Zone 5N"),
                             new CoordinateSystemModel(Constants.Ukraine2000ID[2], 24.000, 45.100, "Ukraine 2000 GK Zone 5")),
        new ProjectionsModel(
                             new CoordinateSystemModel((int)esriSRProjCSType.esriSRProjCS_WGS1984UTM_36N, 30.000, 0.000, "WGS 1984 UTM Zone 36N"),
                             new CoordinateSystemModel((int)esriSRProjCSType.esriSRProjCS_Pulkovo1942GK_6N, 30.000, 44.330, "Pulkovo 1942 GK Zone 6N"),
                             new CoordinateSystemModel(Constants.Ukraine2000ID[3], 30.000, 43.190, "Ukraine 2000 GK Zone 6")),
        new ProjectionsModel(
                             new CoordinateSystemModel((int)esriSRProjCSType.esriSRProjCS_WGS1984UTM_37N, 36.000, 0.000, "WGS 1984 UTM Zone 37N"),
                             new CoordinateSystemModel((int)esriSRProjCSType.esriSRProjCS_Pulkovo1942GK_7N, 36.000, 41.430, "Pulkovo 1942 GK Zone 7N"),
                             new CoordinateSystemModel(Constants.Ukraine2000ID[4], 36.000, 43.430, "Ukraine 2000 GK Zone 7"))};

        //Geographics Models
        public static readonly CoordinateSystemModel WgsGeoModel = new CoordinateSystemModel((int)esriSRGeoCSType.esriSRGeoCS_WGS1984, -180.000, -90.000, "WGS 1984", 1000000);
        public static readonly CoordinateSystemModel PulkovoGeoModel = new CoordinateSystemModel((int)esriSRGeoCSType.esriSRGeoCS_Pulkovo1942, 19.580, 35.150, "Pulkovo 1942", 1000000);
        public static readonly CoordinateSystemModel UkraineGeoModel = new CoordinateSystemModel(Constants.Ukraine2000ID[0], 22.150, 43.190, "Ukraine 2000", 1000000);

        //DataGridView constants
        public const string HighlightColumnName = "HighlightColumn";
        public const string DeleteColumnName = "DeleteColumn";
        public const string NumberColumnName = "NumberColumn";
    }
}
