using System.Diagnostics;
using MilSpace.Core.Actions.Interfaces;

namespace MilSpace.Core.Actions.Exceptions
{
    internal class ActionDescriptionNotDefinedException : SpaeroException
    {
        internal ActionDescriptionNotDefinedException(IAction<IActionResult> action)
            : base()
        {
            errorMessage = "The action {0} doesn't contain description.".InvariantFormat(action.ActionId);
        }
    }
}
