using MilSpace.Configurations.Base;
using System.Configuration;

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
