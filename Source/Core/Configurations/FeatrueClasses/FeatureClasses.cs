using MilSpace.Configurations.Base;
using System.Configuration;

namespace MilSpace.Configurations
{
    public class FeatureClasses : ConfigurationSectionGroup
    {
        internal static string SectionName = ConfigElementNames.FeatureClasses;


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "GDB")]
        public WorkingGDBConnectionSection WorkingGDBConnectionSection
        {
            get { return Sections[WorkingGDBConnectionSection.SectionName] as WorkingGDBConnectionSection; }

        }

        public WorkingDBConnectionSection WorkingDBConnectionSection
        {
            get { return Sections[WorkingDBConnectionSection.SectionName] as WorkingDBConnectionSection; }

        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "GDB")]
        public TemporaryGDBConnectionSection TemporaryGDBConnectionSection
        {
            get { return Sections[TemporaryGDBConnectionSection.SectionName] as TemporaryGDBConnectionSection; }

        }
    }
}
