using MilSpace.Configurations.Base;
using System.Configuration;

namespace MilSpace.Configurations
{
    public class ConnectionsSection : ConfigurationSectionGroup
    {
        internal static string SectionName = ConfigElementNames.MilSpaceConnections;

        
        public WorkingGDBConnectionSection WorkingGDBConnectionSection
        {
            get { return Sections[WorkingGDBConnectionSection.SectionName] as WorkingGDBConnectionSection;  }
            
        }

        public WorkingDBConnectionSection WorkingDBConnectionSection
        {
            get { return Sections[WorkingDBConnectionSection.SectionName] as WorkingDBConnectionSection; }

        }
        public TemporaryGDBConnectionSection TemporaryGDBConnectionSection
        {
            get { return Sections[TemporaryGDBConnectionSection.SectionName] as TemporaryGDBConnectionSection; }

        }
    }
}
