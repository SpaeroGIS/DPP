using MilSpace.Configurations.Base;
using System.Configuration;

namespace MilSpace.Configurations.FeatrueClasses
{

    public class ObservationPoints : ConfigurationSection
    {
        internal static string SectionName = ConfigElementNames.ObservationPoint;



        [ConfigurationProperty(ConfigElementNames.NameAttribute, IsKey = true, IsRequired = true)]
        public string FeatureName
        {
            get { return (string)this[ConfigElementNames.NameAttribute]; }
            set { base[ConfigElementNames.NameAttribute] = value; }
        }
    }
}


