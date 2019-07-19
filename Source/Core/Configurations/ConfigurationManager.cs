using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spaero.Configurations
{
    public static class SpaeroConfigurationManager
    {
        public static ActionsToLoad[] ActionConfiguration
        {
            get { return Spaero.Configurations.ActionConfiguration.LoadActionAssemblies; }
        }

    }
}
