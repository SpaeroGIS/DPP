using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MilSpace.Core.Actions.Interfaces
{
    public interface IActionParam
    {
        string ParamName { get; set; }
        Type GetValueType { get; }
        string ToXml();
    }
}
