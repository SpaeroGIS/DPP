using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Web;
using System.Web.Configuration;
using System.Diagnostics;
using System.Reflection;

namespace MilSpace.Configurations
{
    public class ActionConfiguration : MilSpaceRootConfiguration
    {
        private static ActionsToLoad[] loadActionAssemblies = null;
        public static ActionsToLoad[] LoadActionAssemblies
        {
            get
            {
                if (loadActionAssemblies == null)
                {
                    LoadActionAssembliesSection section = GetRootSectionGroup.Sections[LoadActionAssembliesSection.SectionName] as LoadActionAssembliesSection;
                    loadActionAssemblies = section.AssemblyCollection.Cast<ActionAssemblyElement>().Select(r => new ActionsToLoad() { AssemblyName = r.AssemblyName, AssemblyGroupName = r.ActionsGroupName }).ToArray();
                }

                return loadActionAssemblies;
            }
        }
    }

    public class ActionsToLoad
    {
        public string AssemblyName { get; internal set; }

        public bool IsFullName => string.IsNullOrEmpty(AssemblyName) ? false: 
                (AssemblyName.Split(',').Count() > 1);
        public string AssemblyGroupName { get; internal set; }

    }
}
