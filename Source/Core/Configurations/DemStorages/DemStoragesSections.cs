using MilSpace.Configurations.Base;
using System.Configuration;

namespace MilSpace.Configurations.DemStorages
{
    public class DemStoragesSections : ConfigurationSectionGroup
    {
        internal static string SectionName = ConfigElementNames.DemStorages;

        public SrtmStorageSection WorkingGDBConnectionSection
        {
            get { return Sections[SrtmStorageSection.SectionName] as SrtmStorageSection; }
        }
    }
}
