using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MilSpace.Core.Actions.Interfaces
{
    public interface IDataEntity
    {
        SimpleDataTypesEnum ItemType { get; set; }
        string ItemValue { get; set; }
        Type ItemCLRType { get; }
        object ItemTypedValue { get; }
        bool KeyField { get; set; }
    }
}
