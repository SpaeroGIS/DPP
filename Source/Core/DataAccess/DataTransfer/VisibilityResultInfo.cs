using System.Linq;

namespace MilSpace.DataAccess.DataTransfer
{
    public class VisibilityResultInfo
    {
        private VisibilityCalculationResultsEnum resultType = VisibilityCalculationResultsEnum.None;
        public string ResultName { get; internal set; }
        public string GdbPath { get; internal set; }

        public VisibilityCalculationResultsEnum RessutType
        {
            get
            {

                if (resultType == VisibilityCalculationResultsEnum.None)
                {
                    if (VisibilityTask.VisibilityResulSuffixes.Any(v => ResultName.EndsWith(v.Value)))
                    {
                        resultType = VisibilityTask.VisibilityResulSuffixes.FirstOrDefault(v => v.Key != VisibilityCalculationResultsEnum.None &&
                        ResultName.EndsWith(v.Value)
                        
                        ).Key;
                    }
                }

                return resultType;
            }
        }
    }
}
