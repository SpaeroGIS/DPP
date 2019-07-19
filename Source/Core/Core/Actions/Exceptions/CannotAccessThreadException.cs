namespace MilSpace.Core.Actions.Exceptions
{
    public class CannotAccessThreadException : SpaeroException
    {
        public CannotAccessThreadException(string actionId)
            : base()
        {
            this.errorMessage = "Cannot access the action's thread. Action \"{0}\"".InvariantFormat(actionId);
        }
    }
}
