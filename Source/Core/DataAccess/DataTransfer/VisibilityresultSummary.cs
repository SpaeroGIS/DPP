using MilSpace.DataAccess.Facade;
using System.Collections.Generic;

namespace MilSpace.DataAccess.DataTransfer
{
    public enum VisibilityresultSummaryItemsEnum
    {
        Type,
        Name,
        Surface,
        ObservationPoints,
        ObservationObjects,
        CalculateSeparatedPoints,
        CalculateCommonVisibilityResult,
        TrimCalculatedSurface,
        ConvertToPolygon
    }

    public class VisibilityresultSummary
    {
        private VisibilityCalcResults basedResult;
        private string sourceSurfaceame;
        private IEnumerable<string> pointsName;
        private IEnumerable<string> objectsName;
        VisibilityCalculationResultsEnum calculatedResults;


        internal VisibilityresultSummary(VisibilityCalcResults basedResult)
        {
            this.basedResult = basedResult;
            calculatedResults = (VisibilityCalculationResultsEnum)basedResult.CalculatedResults;
        }

        public VisibilityCalcTypeEnum CalculationType => basedResult.CalculationType;


        public string ResultName => basedResult.Name;

        public string Id => basedResult.Id;
        /// <summary>
        /// Base RasterLayerName
        /// </summary>
        public string SourceSurfaceame
        {
            get
            {
                if (string.IsNullOrEmpty(sourceSurfaceame))
                {
                    var lastDelim = basedResult.Surface.LastIndexOf(@"\");
                    sourceSurfaceame = (lastDelim > 0) ? basedResult.Surface.Substring(lastDelim) : basedResult.Surface;
                }
                return sourceSurfaceame;
            }
        }

        /// <summary>
        /// Calculate visibility for sepatare observation points
        /// </summary>
        //    SeparatedVisibilityResult ? LocalizationContext.Instance.YesWord : LocalizationContext.Instance.NoWord;
        public bool SeparatedVisibilityResult => calculatedResults.HasFlag(VisibilityCalculationResultsEnum.VisibilityAreaRasterSingle);

        /// <summary>
        /// Calculate common visibility for observation points
        /// </summary>
        //CommonVisibilityResult? LocalizationContext.Instance.YesWord : LocalizationContext.Instance.NoWord;
        public bool CommonVisibilityResult => calculatedResults.HasFlag(VisibilityCalculationResultsEnum.VisibilityAreaRaster) ||
                                    calculatedResults.HasFlag(VisibilityCalculationResultsEnum.VisibilityAreasTrimmedByPoly) ;

        /// <summary>
        /// Trim visibility result by valueable area
        /// </summary>
        public bool TrimVisibilityResult => calculatedResults.HasFlag(VisibilityCalculationResultsEnum.VisibilityAreasTrimmedByPoly);

        public bool ConvrtToPolygonResult => calculatedResults.HasFlag(VisibilityCalculationResultsEnum.VisibilityAreaPolygons);
        

        public IEnumerable<string> ObservationPoints
        {
            get
            {
                if (pointsName == null)
                {
                    pointsName = VisibilityZonesFacade.GetCalculatedObserPointsName(Id);
                }

                return pointsName;
            }
        }

        public IEnumerable<string> ObservationObjects {
            get
            {
                if (objectsName == null)
                {
                    objectsName = VisibilityZonesFacade.GetCalculatedObserObjectsName(Id);
                }

                return objectsName;
            }
        }

        public Dictionary<VisibilityresultSummaryItemsEnum, string> SummaryToString()
        {
            return new Dictionary<VisibilityresultSummaryItemsEnum, string>
            {
                { VisibilityresultSummaryItemsEnum.Type,  CalculationType.ToString() },
                { VisibilityresultSummaryItemsEnum.Name, ResultName },
                { VisibilityresultSummaryItemsEnum.Surface,  SourceSurfaceame },
                { VisibilityresultSummaryItemsEnum.ObservationPoints, ObservationPoints == null ? string.Empty : string.Join(", ",ObservationPoints) },
                { VisibilityresultSummaryItemsEnum.ObservationObjects, ObservationObjects == null ? string.Empty : string.Join(", ",ObservationObjects) },
                { VisibilityresultSummaryItemsEnum.CalculateCommonVisibilityResult, CommonVisibilityResult.ToString() },
                { VisibilityresultSummaryItemsEnum.CalculateSeparatedPoints, SeparatedVisibilityResult.ToString() },
                { VisibilityresultSummaryItemsEnum.TrimCalculatedSurface,  TrimVisibilityResult.ToString() },
                { VisibilityresultSummaryItemsEnum.ConvertToPolygon,  ConvrtToPolygonResult.ToString() },
            };
        }



    }
}
