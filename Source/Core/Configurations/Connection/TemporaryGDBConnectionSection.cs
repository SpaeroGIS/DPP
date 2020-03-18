using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using MilSpace.Configurations.Base;

namespace MilSpace.Configurations
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "GDB")]
    public class TemporaryGDBConnectionSection : ConfigurationSection
    {
        internal static string SectionName = ConfigElementNames.TemporaryGDBConnectionSection;

        [ConfigurationProperty(ConfigElementNames.ConnectionString, IsKey = true, IsRequired = true)]
        public string ConnectionString
        {
            get { return (string)this[ConfigElementNames.ConnectionString]; }
            set { base[ConfigElementNames.ConnectionString] = value; }
        }
    }
}
