namespace MilSpace.Core.Actions.Exceptions
{
    public class CannotOpenConnectionStringException : SpaeroException
    {
        public CannotOpenConnectionStringException(string connectionString)
            : base()
        {
            this.errorMessage = string.Format("Cannot open connection to the \"{0}\"", connectionString);
        }
    }
}
