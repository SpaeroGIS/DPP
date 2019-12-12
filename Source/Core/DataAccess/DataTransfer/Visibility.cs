using ESRI.ArcGIS.Geodatabase;
using MilSpace.DataAccess.Facade;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MilSpace.DataAccess.DataTransfer
{
    public class VisibilityCalcResults
    {
        public static Dictionary<VisibilityCalculationResultsEnum, string> VisibilityResulSuffixes = new Dictionary<VisibilityCalculationResultsEnum, string>
        {
            { VisibilityCalculationResultsEnum.None , ""},
            { VisibilityCalculationResultsEnum.ObservationPoints , "_op_p"},
            { VisibilityCalculationResultsEnum.VisibilityAreaRaster , "_img"},
            { VisibilityCalculationResultsEnum.VisibilityAreaRasterSingle , "_imgs"},
            { VisibilityCalculationResultsEnum.ObservationObjects , "_oo_r"},
            { VisibilityCalculationResultsEnum.VisibilityAreaPolygons , "_va_r"},
            { VisibilityCalculationResultsEnum.VisibilityAreaPolygonSingle , "_vas_r"},
            { VisibilityCalculationResultsEnum.CoverageTable , "_ct"},
            { VisibilityCalculationResultsEnum.ObservationPointSingle , "_ops"},
            { VisibilityCalculationResultsEnum.VisibilityObservStationClip , "_imgc"},
            { VisibilityCalculationResultsEnum.VisibilityObservStationClipSingle , "_imgcs"},
            { VisibilityCalculationResultsEnum.VisibilityAreasTrimmedByPoly , "_imgt"},
            { VisibilityCalculationResultsEnum.VisibilityAreaTrimmedByPolySingle , "_imgts"},
            { VisibilityCalculationResultsEnum.VisibilityAreasPotential , "_pva_r" },
            { VisibilityCalculationResultsEnum.VisibilityAreaPotentialSingle , "_pvas_r" }

        };

        private static Dictionary<VisibilityCalcTypeEnum, IEnumerable<VisibilityCalculationResultsEnum>> ResultsToShow = new Dictionary<VisibilityCalcTypeEnum, IEnumerable<VisibilityCalculationResultsEnum>>
        {
            { VisibilityCalcTypeEnum.OpservationPoints, new VisibilityCalculationResultsEnum[]
                        {
                            VisibilityCalculationResultsEnum.VisibilityAreaRaster,
                            VisibilityCalculationResultsEnum.VisibilityAreaRasterSingle,
                            VisibilityCalculationResultsEnum.ObservationPoints,
                            VisibilityCalculationResultsEnum.VisibilityAreaPolygons,
                            VisibilityCalculationResultsEnum.VisibilityAreasTrimmedByPoly,
                            VisibilityCalculationResultsEnum.VisibilityAreaTrimmedByPolySingle,
                            VisibilityCalculationResultsEnum.VisibilityAreaPotentialSingle,
                            VisibilityCalculationResultsEnum.VisibilityAreasPotential,
                            VisibilityCalculationResultsEnum.CoverageTable
                        }
            },
            { VisibilityCalcTypeEnum.ObservationObjects, new VisibilityCalculationResultsEnum[]
                        {
                            VisibilityCalculationResultsEnum.VisibilityObservStationClip,
                            VisibilityCalculationResultsEnum.VisibilityObservStationClipSingle,
                            VisibilityCalculationResultsEnum.ObservationObjects,
                            VisibilityCalculationResultsEnum.ObservationPoints,
                            VisibilityCalculationResultsEnum.VisibilityAreaPolygons,
                            VisibilityCalculationResultsEnum.VisibilityAreaPolygonSingle,
                            VisibilityCalculationResultsEnum.VisibilityAreaPotentialSingle,
                            VisibilityCalculationResultsEnum.VisibilityAreasPotential,
                            VisibilityCalculationResultsEnum.CoverageTable
                        }
            }
        };

        internal static VisibilityCalculationResultsEnum[] FeatureClassResults = {
            VisibilityCalculationResultsEnum.ObservationPoints,
            VisibilityCalculationResultsEnum.ObservationObjects,
            VisibilityCalculationResultsEnum.VisibilityAreaPolygons,
            VisibilityCalculationResultsEnum.ObservationPointSingle,
            VisibilityCalculationResultsEnum.VisibilityAreaPolygonSingle,
            VisibilityCalculationResultsEnum.VisibilityAreaPotentialSingle,
            VisibilityCalculationResultsEnum.VisibilityAreasPotential
        };

        internal static VisibilityCalculationResultsEnum[] RasterResults = {
            VisibilityCalculationResultsEnum.VisibilityAreaRaster,
            VisibilityCalculationResultsEnum.VisibilityObservStationClip,
            VisibilityCalculationResultsEnum.VisibilityAreasTrimmedByPoly,
            VisibilityCalculationResultsEnum.VisibilityAreaRasterSingle,
            VisibilityCalculationResultsEnum.VisibilityAreaTrimmedByPolySingle,
            VisibilityCalculationResultsEnum.VisibilityObservStationClipSingle
        };

        internal static VisibilityCalculationResultsEnum[] TableResults = {
            VisibilityCalculationResultsEnum.CoverageTable
        };

        internal static Dictionary<esriDatasetType, VisibilityCalculationResultsEnum[]> EsriDatatypeToResultMapping = new Dictionary<esriDatasetType, VisibilityCalculationResultsEnum[]>
        {
            { esriDatasetType.esriDTFeatureClass, FeatureClassResults},
            { esriDatasetType.esriDTRasterDataset, RasterResults},
            { esriDatasetType.esriDTTable, TableResults}

        };

        private static IEnumerable<VisibilityCalculationResultsEnum> ResultsRelatedToSingle = new VisibilityCalculationResultsEnum[] {
            VisibilityCalculationResultsEnum.ObservationPointSingle,
            VisibilityCalculationResultsEnum.VisibilityAreaRasterSingle,
            VisibilityCalculationResultsEnum.VisibilityAreaPolygonSingle,
            VisibilityCalculationResultsEnum.VisibilityObservStationClipSingle,
            VisibilityCalculationResultsEnum.VisibilityAreaTrimmedByPolySingle,
            VisibilityCalculationResultsEnum.VisibilityAreaPotentialSingle
        };

        public const VisibilityCalculationResultsEnum DefaultResultsSet = VisibilityCalculationResultsEnum.ObservationPoints | VisibilityCalculationResultsEnum.ObservationObjects | VisibilityCalculationResultsEnum.VisibilityAreaRaster;
        private VisibilityresultSummary summary;
        List<string> allResulrs = null;

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

        public VisibilityresultSummary Summary
        {
            get
            {
                if (summary == null)
                {
                    summary = new VisibilityresultSummary(this);
                }

                return summary;
            }
        }

        public IEnumerable<VisibilityResultInfo> ResultsInfo => ValueableResults().Select(r => new VisibilityResultInfo
        {
            ResultName = r,
            GdbPath = ReferencedGDB
        }).ToArray();


        public IEnumerable<VisibilityResultInfo> AllResultsInfo => AllResults().Select(r => new VisibilityResultInfo
        {
            ResultName = r,
            GdbPath = ReferencedGDB
        }).ToArray();

        public static string GetResultName(VisibilityCalculationResultsEnum resultType, string sessionName, int pointId = -1)
        {
            var pointIdstr = pointId > -1 ? $"_{pointId}" : string.Empty;
            return $"{sessionName}{pointIdstr}{VisibilityResulSuffixes[resultType]}";
        }

        public static VisibilityCalculationResultsEnum GetResultTypeByName(string name)
        {
            return VisibilityResulSuffixes.FirstOrDefault(suff => name.EndsWith(suff.Value) && suff.Key != VisibilityCalculationResultsEnum.None).Key;
        }

        public static IEnumerable<VisibilityCalculationResultsEnum> GetRasterResults()
        {
            return RasterResults;
        }

        public IEnumerable<string> ValueableResults()
        {
            return GetResults();
        }

        public IEnumerable<string> AllResults()
        {
            return GetResults();
        }
        internal static esriDatasetType GetEsriDataTypeByVisibilityresyltType(VisibilityCalculationResultsEnum resultType)
        {
            return EsriDatatypeToResultMapping.First(r => r.Value.Any(t => t == resultType)).Key;
        }
        private IEnumerable<string> GetResults(bool allresults = false)
        {
            //VisibilityCalculationresultsEnum resultsInGDB = GdbAccess.Instance.CheckVisibilityResult(Id);
            if (allResulrs == null)
            {
                var resultsToShow = ResultsToShow[CalculationType];

                allResulrs = new List<string>();
                VisibilityCalculationResultsEnum calculatedResults = (VisibilityCalculationResultsEnum)CalculatedResults;

                int pointsCount = Summary.ObservationPoints.Count();

                foreach (var result in VisibilityResulSuffixes)
                {
                    if (calculatedResults.HasFlag(result.Key) && resultsToShow.Any(r => r.Equals(result.Key)))
                    {
                        if (ResultsRelatedToSingle.Any(v => result.Key == v))
                        {
                            //int index = pointsCount - 1;
                            for (int index = 0; index < pointsCount; index++)
                            {
                                string resultBName = GetResultName(result.Key, Id, index);
                                if (VisibilityZonesFacade.CheckVisibilityResultEistance(resultBName, result.Key))
                                {
                                    allResulrs.Add(resultBName);
                                }
                            }
                        }
                        else
                        {
                            var resultBName = GetResultName(result.Key, Id);
                            if (VisibilityZonesFacade.CheckVisibilityResultEistance(resultBName, result.Key))
                            {
                                allResulrs.Add(resultBName);
                            }
                        }
                    }
                }
            }

            return allResulrs;
        }
    }
}
