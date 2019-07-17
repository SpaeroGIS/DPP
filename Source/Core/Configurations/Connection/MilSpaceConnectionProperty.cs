using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MilSpace.Configurations.Connection
{
    public class MilSpaceConnectionProperty
    {
        public string TemporaryGDBConnection
        {
            get;
            internal set;
        }
        public string WorkingDBConnection
        {
            get;
            internal set;
        }
        public string WorkingGDBConnection
        {
            get;
            internal set;
        }
    }
}
