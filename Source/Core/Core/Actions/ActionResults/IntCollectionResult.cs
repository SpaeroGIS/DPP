using MilSpace.Core.Actions.Base;
using System.Collections.Generic;

namespace MilSpace.Core.Actions.ActionResults
{
    public class IntCollectionResult : ActionResultBase<IEnumerable<int>>
    {
        public override IEnumerable<int> Result
        { get; set; }
    }
}
