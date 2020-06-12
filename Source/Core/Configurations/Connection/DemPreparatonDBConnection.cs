using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using MilSpace.Configurations.Base;

namespace MilSpace.Configurations
{
    public class DemPreparatonDBConnectionSection : ConfigurationSection
    {
        internal static string SectionName = ConfigElementNames.DemPreparatonDBConnectionSection;

        [ConfigurationProperty(ConfigElementNames.ConnectionString, IsKey = false, IsRequired = false)]
        public string ConnectionString
        {
            get { return (string)this[ConfigElementNames.ConnectionString]; }
            set { base[ConfigElementNames.ConnectionString] = value; }
        }
    }
}
