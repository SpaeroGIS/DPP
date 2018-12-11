namespace MilSpace.Core.Actions.Exceptions
{
    public class ActionNotFoundException : SpaeroException
    {
        public ActionNotFoundException(string actions) : base("Cannot find action(s) {0}".InvariantFormat(actions))
        {
        }

    }
}
