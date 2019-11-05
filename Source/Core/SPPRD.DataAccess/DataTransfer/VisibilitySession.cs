using ESRI.ArcGIS.Geodatabase;
using MilSpace.DataAccess.Facade;
using System;
using System.Collections.Generic;

namespace MilSpace.DataAccess.DataTransfer
{
    [Flags]
    public enum VisibilityCalculationresultsEnum : byte
    {
        None = 0,
        ObservationPoints = 1,
        VisibilityAreaRaster = 2,
        ObservationStations = 4,
        VisibilityAreaPolygons = 8,
        CoverageTable = 16
    }



    public class VisibilitySession
    {
        internal static Dictionary<VisibilityCalculationresultsEnum, string> VisibilityResulSuffixes = new Dictionary<VisibilityCalculationresultsEnum, string>
        {
            { VisibilityCalculationresultsEnum.None , ""},
            { VisibilityCalculationresultsEnum.ObservationPoints , "_op"},
            { VisibilityCalculationresultsEnum.VisibilityAreaRaster , "_img"},
            { VisibilityCalculationresultsEnum.ObservationStations , "_oo"},
            { VisibilityCalculationresultsEnum.VisibilityAreaPolygons , "_va"},
            { VisibilityCalculationresultsEnum.CoverageTable , "_ct"},
        };

        internal static VisibilityCalculationresultsEnum[] FeatureClassResults = {
            VisibilityCalculationresultsEnum.ObservationPoints,
            VisibilityCalculationresultsEnum.ObservationStations,
            VisibilityCalculationresultsEnum.VisibilityAreaPolygons,
        };

        internal static VisibilityCalculationresultsEnum[] RasterResults = {
            VisibilityCalculationresultsEnum.VisibilityAreaRaster
        };

        internal static VisibilityCalculationresultsEnum[] TableResults = {
            VisibilityCalculationresultsEnum.VisibilityAreaRaster
        };

        internal static Dictionary<esriDatasetType, VisibilityCalculationresultsEnum[]> EsriDatatypeToresultMapping = new Dictionary<esriDatasetType, VisibilityCalculationresultsEnum[]>
        {
            { esriDatasetType.esriDTFeatureClass, FeatureClassResults},
            { esriDatasetType.esriDTRasterDataset, RasterResults},
            { esriDatasetType.esriDTTable, TableResults}

        };

        public const VisibilityCalculationresultsEnum DefaultResultsSet = VisibilityCalculationresultsEnum.ObservationPoints | VisibilityCalculationresultsEnum.ObservationStations | VisibilityCalculationresultsEnum.VisibilityAreaRaster;

        public int IdRow;
        public string Id;
        public string Name;
        public string UserName;
        public DateTime? Created { get; internal set; }
        public DateTime? Started { get; internal set; }
        public DateTime? Finished { get; internal set; }
        public int CalculatedResults;
        public string ReferencedGDB;
        public static string GetResultName(VisibilityCalculationresultsEnum resultType, string sessionName)
        {
            return $"{sessionName}{VisibilityResulSuffixes[resultType]}";
        }

        public IEnumerable<string> Results()
        {
            VisibilityCalculationresultsEnum resultsInGDB = GdbAccess.Instance.CheckVisibilityResult(Id);
            List<string> resulrs = new List<string>();

            foreach (var result in VisibilityResulSuffixes)
            {
                if (result.Key != VisibilityCalculationresultsEnum.None && resultsInGDB.HasFlag(result.Key))
                {
                    resulrs.Add(GetResultName(result.Key, Id));
                }
            }

            return resulrs;
        }
    }
}
