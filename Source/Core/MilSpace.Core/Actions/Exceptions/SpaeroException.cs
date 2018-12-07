using System;

namespace MilSpace.Core.Actions.Exceptions
{
    public abstract class SpaeroException : Exception
    {
        protected string errorMessage = "General Exception";

        public SpaeroException()
        {
        }

        public SpaeroException(string message) : base(message)
        {
            errorMessage = message;
        }
        public SpaeroException(Exception innerException)
            : base("General Exception", innerException)
        { }

        public override string Message
        {
            get
            {
                return errorMessage;
            }
        }
    }
}
