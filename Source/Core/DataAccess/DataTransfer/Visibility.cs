using ESRI.ArcGIS.Geodatabase;
using MilSpace.DataAccess.Facade;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MilSpace.DataAccess.DataTransfer
{
    public class VisibilityCalcResults
    {
        public static Dictionary<VisibilityCalculationresultsEnum, string> VisibilityResulSuffixes = new Dictionary<VisibilityCalculationresultsEnum, string>
        {
            { VisibilityCalculationresultsEnum.None , ""},
            { VisibilityCalculationresultsEnum.ObservationPoints , "_op_p"},
            { VisibilityCalculationresultsEnum.VisibilityAreaRaster , "_img"},
            { VisibilityCalculationresultsEnum.VisibilityAreaRasterSingle , "_imgs"},
            { VisibilityCalculationresultsEnum.ObservationStations , "_oo_r"},
            { VisibilityCalculationresultsEnum.VisibilityAreaPolygons , "_va_r"},
            { VisibilityCalculationresultsEnum.VisibilityAreaPolygonSingle , "_vas_r"},
            { VisibilityCalculationresultsEnum.CoverageTable , "_ct"},
            { VisibilityCalculationresultsEnum.ObservationPointSingle , "_ops"},
            { VisibilityCalculationresultsEnum.VisibilityObservStationClip , "_imgc"},
            { VisibilityCalculationresultsEnum.VisibilityAreasTrimmedByPoly , "_imgt"},
            { VisibilityCalculationresultsEnum.VisibilityAreaTrimmedByPolySingle , "_imgts"}

        };

        private static IEnumerable<VisibilityCalculationresultsEnum> ResultsToShow = new VisibilityCalculationresultsEnum[]
        {
            VisibilityCalculationresultsEnum.ObservationPoints,
            VisibilityCalculationresultsEnum.VisibilityAreaRaster,
            VisibilityCalculationresultsEnum.VisibilityAreaRasterSingle,
            VisibilityCalculationresultsEnum.ObservationStations,
            VisibilityCalculationresultsEnum.VisibilityAreaPolygons,
            VisibilityCalculationresultsEnum.VisibilityObservStationClip,
            VisibilityCalculationresultsEnum.VisibilityAreasTrimmedByPoly,
            VisibilityCalculationresultsEnum.VisibilityAreaTrimmedByPolySingle
        };

        internal static VisibilityCalculationresultsEnum[] FeatureClassResults = {
            VisibilityCalculationresultsEnum.ObservationPoints,
            VisibilityCalculationresultsEnum.ObservationStations,
            VisibilityCalculationresultsEnum.VisibilityAreaPolygons,
            VisibilityCalculationresultsEnum.ObservationPointSingle,
        };

        internal static VisibilityCalculationresultsEnum[] RasterResults = {
            VisibilityCalculationresultsEnum.VisibilityAreaRaster,
            VisibilityCalculationresultsEnum.VisibilityAreaRasterSingle,
            VisibilityCalculationresultsEnum.VisibilityObservStationClip,
            VisibilityCalculationresultsEnum.VisibilityAreasTrimmedByPoly,
            VisibilityCalculationresultsEnum.VisibilityAreaTrimmedByPolySingle
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

        private static IEnumerable<VisibilityCalculationresultsEnum> ResultsRelatedToSingle = new VisibilityCalculationresultsEnum[] {
            VisibilityCalculationresultsEnum.ObservationPointSingle,
            VisibilityCalculationresultsEnum.VisibilityAreaRasterSingle,
             VisibilityCalculationresultsEnum.VisibilityAreaPolygonSingle
        };

        public const VisibilityCalculationresultsEnum DefaultResultsSet = VisibilityCalculationresultsEnum.ObservationPoints | VisibilityCalculationresultsEnum.ObservationStations | VisibilityCalculationresultsEnum.VisibilityAreaRaster;

        public int IdRow;
        public string Id;
        public string Name;
        public string UserName;
        public DateTime? Created { get; internal set; }
        public int CalculatedResults;
        public string ReferencedGDB;
        public string Surface;
        public bool Shared;
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

        public static VisibilityCalculationresultsEnum GetResultTypeByName(string name)
        {
            return VisibilityResulSuffixes.FirstOrDefault(suff => name.EndsWith(suff.Value) && suff.Key != VisibilityCalculationresultsEnum.None).Key;
        }

        public IEnumerable<string> Results()
        {
            //VisibilityCalculationresultsEnum resultsInGDB = GdbAccess.Instance.CheckVisibilityResult(Id);

            VisibilityCalculationresultsEnum calculatedResults = (VisibilityCalculationresultsEnum)CalculatedResults;
            List<string> resulrs = new List<string>();

            foreach (var result in VisibilityResulSuffixes)
            {
                if (calculatedResults.HasFlag(result.Key) && ResultsToShow.Any(r => r.Equals(result.Key)))
                {

                    if (ResultsRelatedToSingle.Any(v => result.Key == v))
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
