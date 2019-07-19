using System;
using MilSpace.Core.Actions.Interfaces;

namespace MilSpace.Core.Actions.Base
{
    /// <summary>
    /// Base class for action result
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ActionResultBase<T> : IActionResult
    {
        private string erroeMessage;

        public abstract T Result { get; set; }
        public Guid PeocessId { get; set; }
        public string ErrorMessage
        {
            get
            {
                if (this.Exception != null && string.IsNullOrEmpty(erroeMessage))
                {
                    erroeMessage = this.Exception.Message;
                }

                return erroeMessage;
            }

            set
            { erroeMessage = value; }
        }
        public Type ReturnType
        {
            get { return typeof(T); }
        }
        public Exception Exception { get; set; }

    }
}
