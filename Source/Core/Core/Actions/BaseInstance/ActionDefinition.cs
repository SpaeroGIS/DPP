using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MilSpace.Core.Actions.Interfaces;

namespace MilSpace.Core.Actions.Base
{
    public class ActionDefinition
    {
        public string ActionId { get; internal set; }
        public Type ActionClassType { get; internal set; }
        public string Area { get; internal set; }
        public ActionDescription Description { get; internal set; }
        public IActionParam[] Paramenetrs { get; internal set; }
    }
}
