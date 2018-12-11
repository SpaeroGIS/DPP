using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using MilSpace.Configurations.Base;

namespace MilSpace.Configurations
{
    public class LoadActionAssembliesSection : ConfigurationSection
    {
        public const string SectionName = ConfigElementNames.LoadActionAssembliesSectionName;

        [ConfigurationProperty(ActionsAssemblyCollection.SectionName, Options = ConfigurationPropertyOptions.IsRequired)]
        public ActionsAssemblyCollection AssemblyCollection
        {
            get
            {
                return (ActionsAssemblyCollection)this[ActionsAssemblyCollection.SectionName];
            }
        }
    }
}
