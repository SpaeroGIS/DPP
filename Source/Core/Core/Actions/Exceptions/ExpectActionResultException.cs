using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MilSpace.Core.Actions.Exceptions
{
    /// <summary>
    /// Is thrown when an action is not receive required parameter
    /// </summary>
    public class ExpectActionResultException : Exception
    {
        const string  errorMessageTemplate = "The action {0} expects result of type {1}.";
        string message;
        public ExpectActionResultException(string actionName, Type actionResultType)
        {
            message = string.Format(errorMessageTemplate, actionName, actionResultType);
        }

        public override string Message
        {
            get
            {
                return this.message;
            }
        }
    }
}
