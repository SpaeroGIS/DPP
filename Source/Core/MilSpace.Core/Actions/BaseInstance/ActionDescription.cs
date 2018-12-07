namespace MilSpace.Core.Actions.Base
{
    /// <summary>
    /// Defines an action description
    /// </summary>
    public class ActionDescription
    {

        internal ActionDescription()
        { }

        public string ActionId;

        /// <summary>
        /// Defines a Spaero functional area
        /// </summary>
        public string Area;

        /// <summary>
        /// The action description
        /// </summary>
        public string Description;
        
        /// <summary>
        /// Gets true if the description is empty
        /// </summary>
        public bool IsEmpty
        {
            get { return string.IsNullOrEmpty(Area) || string.IsNullOrEmpty(Description); }
        }
    }
}
