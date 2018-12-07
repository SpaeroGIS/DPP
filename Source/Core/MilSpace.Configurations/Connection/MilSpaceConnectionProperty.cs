using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MilSpace.Configurations.Connection
{
    public class MilSpaceConnectionProperty
    {
        public string ConnectionString
        {
            get;
            internal set;
        }

        public string TaskTablePrefix
        {
            get;
            internal set;
        }
    }
}
