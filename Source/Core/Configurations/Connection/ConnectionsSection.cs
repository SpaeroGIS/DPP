using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using MilSpace.Configurations.Base;

namespace MilSpace.Configurations
{
    public class ConnectionsSection : ConfigurationSectionGroup
    {
        internal static string SectionName = ConfigElementNames.MilSpaceConnections;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "GDB")]
        public WorkingGDBConnectionSection WorkingGDBConnectionSection
        {
            get
            {
                return Sections[WorkingGDBConnectionSection.SectionName] as WorkingGDBConnectionSection;
            }
        }

        public WorkingDBConnectionSection WorkingDBConnectionSection
        {
            get
            {
                return Sections[WorkingDBConnectionSection.SectionName] as WorkingDBConnectionSection;
            }

        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "GDB")]
        public TemporaryGDBConnectionSection TemporaryGDBConnectionSection
        {
            get
            {
                return Sections[TemporaryGDBConnectionSection.SectionName] as TemporaryGDBConnectionSection;
            }

        }

        public DemPreparatonDBConnectionSection DemPreparatonDBConnectionSection
        {
            get
            {
                return Sections[DemPreparatonDBConnectionSection.SectionName] as DemPreparatonDBConnectionSection;
            }

        }
    }
}
