using System.Collections.Generic;

namespace MilSpace.DataAccess.DataTransfer
{
    public class WizardResult
    {
        public IEnumerable<int>  ObservPointIDs{ get; set; }
        public IEnumerable<int> ObservObjectIDs { get; set; }
        public string RasterLayerName { get; set; }
        public string RelativeLayerName { get; set; }
        public short ResultLayerTransparency { get; set; }
        public bool SumFieldOfView { get; set; }
        public bool Table { get; set; }
        public VisibilityCalculationresultsEnum VisibilityCalculationResults { get; set; }
        public LayerPositionsEnum ResultLayerPosition { get; set; }

        public VisibilityCalcTypeEnum CalculationType;

        public string TaskName;
    }
}
