using ESRI.ArcGIS.Geodatabase;
using MilSpace.DataAccess.Facade;
using System;
using System.Linq;
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
        CoverageTable = 16,
        ObservationPointSingle = 32,
        VisibilityAreaRasterSingle = 64,
        VisibilityObservStationClip = 128,
    }

    public enum LayerPositionsEnum
    {
        Above,
        Below
    }

    public class VisibilityTask
    {
        public static Dictionary<VisibilityCalculationresultsEnum, string> VisibilityResulSuffixes = new Dictionary<VisibilityCalculationresultsEnum, string>
        {
            { VisibilityCalculationresultsEnum.None , ""},
            { VisibilityCalculationresultsEnum.ObservationPoints , "_op_p"},
            { VisibilityCalculationresultsEnum.VisibilityAreaRaster , "_img"},
            { VisibilityCalculationresultsEnum.VisibilityAreaRasterSingle , "_imgs"},
            { VisibilityCalculationresultsEnum.ObservationStations , "_oo_r"},
            { VisibilityCalculationresultsEnum.VisibilityAreaPolygons , "_va"},
            { VisibilityCalculationresultsEnum.CoverageTable , "_ct"},
            { VisibilityCalculationresultsEnum.ObservationPointSingle , "_ops"},
            { VisibilityCalculationresultsEnum.VisibilityObservStationClip , "__imgc"}
        };

        internal static VisibilityCalculationresultsEnum[] FeatureClassResults = {
            VisibilityCalculationresultsEnum.ObservationPoints,
            VisibilityCalculationresultsEnum.ObservationStations,
            VisibilityCalculationresultsEnum.VisibilityAreaPolygons,
            VisibilityCalculationresultsEnum.ObservationPointSingle
        };

        internal static VisibilityCalculationresultsEnum[] RasterResults = {
            VisibilityCalculationresultsEnum.VisibilityAreaRaster,
            VisibilityCalculationresultsEnum.VisibilityAreaRasterSingle,
            VisibilityCalculationresultsEnum.VisibilityObservStationClip
        };

        internal static VisibilityCalculationresultsEnum[] TableResults = {
            VisibilityCalculationresultsEnum.CoverageTable
        };

        internal static Dictionary<esriDatasetType, VisibilityCalculationresultsEnum[]> EsriDatatypeToResultMapping = new Dictionary<esriDatasetType, VisibilityCalculationresultsEnum[]>
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
        public string Surface;
        public VisibilityCalcTypeEnum CalculationType;

        public IEnumerable<VisibilityResultInfo> ResultsInfo
        {
            get
            {
                return
                    Results().Select(r => new VisibilityResultInfo
                    {
                        ResultName = r,
                        GdbPath = ReferencedGDB
                    }).ToArray();

            }
        }

        public static string GetResultName(VisibilityCalculationresultsEnum resultType, string sessionName, int pointId = -1)
        {
            var pointIdstr = pointId > -1 ? $"_{pointId}" : string.Empty;
            return $"{sessionName}{pointIdstr}{VisibilityResulSuffixes[resultType]}";
        }

        public IEnumerable<string> Results()
        {
            //VisibilityCalculationresultsEnum resultsInGDB = GdbAccess.Instance.CheckVisibilityResult(Id);

            VisibilityCalculationresultsEnum calculatedResults = (VisibilityCalculationresultsEnum)CalculatedResults;
            List<string> resulrs = new List<string>();

            foreach (var result in VisibilityResulSuffixes)
            {
                if (result.Key != VisibilityCalculationresultsEnum.None && calculatedResults.HasFlag(result.Key))
                {

                    if (result.Key == VisibilityCalculationresultsEnum.ObservationPointSingle ||
                        result.Key == VisibilityCalculationresultsEnum.VisibilityAreaRasterSingle)

                    {
                        int index = 0;
                        string resultBName = GetResultName(result.Key, Id, index);
                        while (VisibilityZonesFacade.CheckVisibilityResultEistance(resultBName, result.Key))
                        {
                            resulrs.Add(resultBName);
                            resultBName = GetResultName(result.Key, Id, ++index);
                        }
                    }
                    else
                    {
                        resulrs.Add(GetResultName(result.Key, Id));
                    }
                }
            }

            return resulrs;
        }

        internal static esriDatasetType GetEsriDataTypeByVisibilityresyltType(VisibilityCalculationresultsEnum resultType)
        {
            return EsriDatatypeToResultMapping.First(r => r.Value.Any(t => t == resultType)).Key;
        }
    }
}
