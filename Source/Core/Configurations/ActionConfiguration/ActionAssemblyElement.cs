using System.Configuration;

namespace MilSpace.Configurations
{
    public class ActionAssemblyElement : ConfigurationElement
    {

        /// <summary>
        /// Defines assembly contained Spaero actions
        /// </summary>
        internal const string Assembly = "assembly";

        /// <summary>
        /// Defines actons group name name contained Spaero actions
        /// </summary>
        internal const string Name = "name";

        [ConfigurationProperty(Name, IsKey = true, IsRequired = true)]
        public string ActionsGroupName
        {
            get { return (string)this[Name]; }
            set { base[Name] = value; }
        }

        [ConfigurationProperty(Assembly, IsKey = true, IsRequired = true)]
        public string AssemblyName
        {
            get { return (string)base[Assembly]; }
            set { base[Assembly] = value; }
        }
    }
}