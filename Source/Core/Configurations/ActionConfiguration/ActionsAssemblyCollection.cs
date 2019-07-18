using System.Configuration;

namespace MilSpace.Configurations
{
    [ConfigurationCollection(typeof(ActionAssemblyElement), CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap)]
    public class ActionsAssemblyCollection : ConfigurationElementCollection
    {
        public const string SectionName = "actions";

        public ActionsAssemblyCollection() { }

        protected override ConfigurationElement CreateNewElement()
        {
            return new ActionAssemblyElement();
        }
        
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ActionAssemblyElement)element).ActionsGroupName;
        }

        public ActionAssemblyElement this[int index]
        {
            get
            {
                return (ActionAssemblyElement)BaseGet(index);
            }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }


        new public ActionAssemblyElement this[string Name]
        {
            get
            {
                return (ActionAssemblyElement)BaseGet(Name);
            }
        }
    }
}
