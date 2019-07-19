using System;
using System.Linq;
using System.Reflection;
using MilSpace.Core.Actions.Exceptions;
using MilSpace.Core.Actions.Interfaces;

namespace MilSpace.Core.Actions.Base
{
    [System.Runtime.InteropServices.Guid("70ABDCB3-2CAF-4D34-A4A3-B3024A1862F8")]
    public abstract class Action<R> : IAction<R> where R : IActionResult
    {
        protected R returnResult;
        protected string[] usedActionParams = null;
        protected IActionProcessor Parameters;

        protected Logger logger;
        protected readonly ActionDescription description = new ActionDescription();


        protected Action()
        {
            this.FillUserActionParams();
            description.Area = Area;
            description.ActionId = this.ActionId;
        }

        public Action(IActionProcessor parameters)
            : this()
        {

            logger = Logger.GetLoggerEx(this.ActionId + BitConverter.ToInt64(parameters.ProcessId.ToByteArray(), 8));

            Logger.Info(string.Format("Action \"{0}\" activating", this.ActionId));

            if (this.usedActionParams == null)
            {
                throw new ArgumentNullException("The list of used action parameters is not defined.");
            }

            this.Parameters = parameters;

            //Create instance of result
            ConstructorInfo ctor = typeof(R).GetConstructors().First(c => c.GetParameters().Count() == 0);
            LoadedActions.ObjectActivator<R> createdActivator = LoadedActions.GetActivator<R>(ctor);
            this.returnResult = createdActivator();

            Logger.Info(string.Format("Action \"{0}\" activated", this.ActionId));

        }

        public abstract string ActionId { get; }

        public abstract void Process();

        public string[] UsedParameters { get { return this.usedActionParams; } }

        public virtual string Area { get { return this.GetType().Namespace; } }

        public T ValidateParameter<T>(string parameterName, T comparingValue)
        {
            T paramValue = this.Parameters.GetParameter<T>(parameterName, comparingValue).Value;

            if (paramValue == null || paramValue.Equals(comparingValue))
            {
                throw new ExpectActionParameterException(this.ActionId, parameterName);
            }

            return paramValue;
        }

        public abstract R GetResult();

        public void FillUserActionParams()
        {
            if (ParametersTemplate != null)
            {
                this.usedActionParams = ParametersTemplate.Select(ap => ap.ParamName).ToArray();
            }
        }

        public abstract IActionParam[] ParametersTemplate
        {
            get;
        }

        public virtual ActionDescription Description
        {
            get { return description; }
        }
        
    }
}
