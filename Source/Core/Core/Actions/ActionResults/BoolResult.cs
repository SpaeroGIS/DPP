using MilSpace.Core.Actions.Base;

namespace MilSpace.Core.Actions.ActionResults
{
    public class BoolResult : ActionResultBase<bool>
    {
        public override bool Result { get; set; }
        
    }
}
