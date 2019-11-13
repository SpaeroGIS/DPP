using MilSpace.Core.Actions.Base;

namespace MilSpace.Tools.SurfaceProfile.Actions
{
    public class VisibilityCalculationResult : ActionResultBase<VisibilityCalculation>
    {
        public VisibilityCalculationResult()
        {
            Result = new VisibilityCalculation();
        }
        public override VisibilityCalculation Result { get; set; }
    }
}
