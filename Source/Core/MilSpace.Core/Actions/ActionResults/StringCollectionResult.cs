using MilSpace.Core.Actions.Base;
using System.Collections.Generic;

namespace MilSpace.Core.Actions.ActionResults
{
    public class StringCollectionResult : ActionResultBase<IEnumerable<string>>
    {
        public override IEnumerable<string> Result
        {
            get;
            set;
        }
    }
}
