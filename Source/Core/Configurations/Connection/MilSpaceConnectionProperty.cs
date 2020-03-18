using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MilSpace.Configurations.Connection
{
    public class MilSpaceConnectionProperty
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "GDB")]
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "GDB")]
        public string WorkingGDBConnection
        {
            get;
            internal set;
        }
    }
}
