using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilSpace.DataAccess.DataTransfer
{
    public class VisibilityresultSummary
    {
        private VisibilityCalcResults basedResult;
        private string sourceSurfaceame;
        VisibilityCalculationresultsEnum calculatedResults;

        internal VisibilityresultSummary(VisibilityCalcResults basedResult)
        {
            this.basedResult = basedResult;
            calculatedResults = (VisibilityCalculationresultsEnum)basedResult.CalculatedResults;
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
        public bool SeparatedVisibilityResult => calculatedResults.HasFlag(VisibilityCalculationresultsEnum.VisibilityAreaRasterSingle);

        /// <summary>
        /// Calculate common visibility for observation points
        /// </summary>
        //CommonVisibilityResult? LocalizationContext.Instance.YesWord : LocalizationContext.Instance.NoWord;
        public bool CommonVisibilityResult => calculatedResults.HasFlag(VisibilityCalculationresultsEnum.VisibilityAreaRaster) || calculatedResults.HasFlag(VisibilityCalculationresultsEnum.VisibilityObservStationClip);

        /// <summary>
        /// Trim visibility result by valueable area
        /// </summary>
        public bool TrimVisibilityResult => calculatedResults.HasFlag(VisibilityCalculationresultsEnum.VisibilityAreasTrimmedByPoly);

        public IEnumerable<string> OpeservationPoints { get; private set; }

        public IEnumerable<string> OpeservationObjects { get; private set; }




    }
}
