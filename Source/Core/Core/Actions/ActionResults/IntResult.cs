using MilSpace.Core.Actions.Base;

namespace MilSpace.Core.Actions.ActionResults
{
    public class IntResult : ActionResultBase<int>
    {
        public override int Result
        {
            get;
            set;
        }
    }
}
