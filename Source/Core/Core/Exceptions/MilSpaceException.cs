using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilSpace.Core.Exceptions
{
    public abstract class MilSpaceException : Exception
    {
        protected string errorMessage = "General Exception";

        public MilSpaceException()
        {
        }

        public MilSpaceException(string message) : base(message)
        {
            errorMessage = message;
        }
        public MilSpaceException(Exception innerException)
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
