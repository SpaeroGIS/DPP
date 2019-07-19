using MilSpace.Configurations.Base;
using System.Configuration;

namespace MilSpace.Configurations
{
    public class WorkingGDBConnectionSection : ConfigurationSection
    {
        internal static string SectionName = ConfigElementNames.WorkingGDBConnectionSection;



        [ConfigurationProperty(ConfigElementNames.ConnectionString, IsKey = true, IsRequired = true)]
        public string ConnectionString
        {
            get { return (string)this[ConfigElementNames.ConnectionString]; }
            set { base[ConfigElementNames.ConnectionString] = value; }
        }
    }
}
    