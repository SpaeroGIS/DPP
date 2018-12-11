using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using MilSpace.Configurations.Base;

namespace MilSpace.Configurations
{
    public class ConnectionSection : ConfigurationSection
    {
        internal static string SectionName = ConfigElementNames.SpaeroConnection;

        [ConfigurationProperty(ConfigElementNames.ConnectionString, IsKey = true, IsRequired = true)]
        public string ConnectionString
        {
            get { return (string)this[ConfigElementNames.ConnectionString]; }
            set { base[ConfigElementNames.ConnectionString] = value; }
        }

        [ConfigurationProperty(ConfigElementNames.TaskTablePrefix, IsKey = true, IsRequired = true)]
        public string TaskTablePrefix
        {
            get { return (string)base[ConfigElementNames.TaskTablePrefix]; }
            set { base[ConfigElementNames.TaskTablePrefix] = value; }
        }
    }
}
