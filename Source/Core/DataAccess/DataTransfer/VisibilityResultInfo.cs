using System.Linq;

namespace MilSpace.DataAccess.DataTransfer
{
    public class VisibilityResultInfo
    {
        private VisibilityCalculationresultsEnum resultType = VisibilityCalculationresultsEnum.None;
        public string ResultName { get; internal set; }
        public string GdbPath { get; internal set; }

        public VisibilityCalculationresultsEnum RessutType
        {
            get
            {

                if (resultType == VisibilityCalculationresultsEnum.None)
                {
                    if (VisibilityTask.VisibilityResulSuffixes.Any(v => ResultName.EndsWith(v.Value)))
                    {
                        resultType = VisibilityTask.VisibilityResulSuffixes.FirstOrDefault(v => v.Key != VisibilityCalculationresultsEnum.None &&
                        ResultName.EndsWith(v.Value)
                        
                        ).Key;
                    }
                }

                return resultType;
            }
        }
    }
}
