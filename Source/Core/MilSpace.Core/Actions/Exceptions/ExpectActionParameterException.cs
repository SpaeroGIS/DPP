using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MilSpace.Core.Actions.Exceptions
{
    /// <summary>
    /// Is thrown when an action is not receive required parameter
    /// </summary>
    public class ExpectActionParameterException : Exception
    {
        const string  errorMessageTemplate = "The action {0} expects paramenetr {1}.";
        string message;
        public ExpectActionParameterException(string actionName, string actionParamName)
        {
            message = string.Format(errorMessageTemplate, actionName, actionParamName);
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
