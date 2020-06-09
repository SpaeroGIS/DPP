using System;
using System.Collections.Generic;
using System.Web;
using MilSpace.Core.Actions.Base;

namespace MilSpace.Core.Actions.Interfaces
{
    public interface IActionProcessor
    {

        event ActionProcessDelegate ProcessMonitor;

        ActionProcessDelegate ProcessMonitorDelegate { get; }

        T Process<T>();

        IActionResult Process();
        
        void ProcessAsync(ActionFinishedDelegate onActionFinished);
        
        bool ContainsParam(string key);

        HttpContext Context { get;  }

        Guid ProcessId { get; }

        IEnumerable<IActionParam> ProcessingActionParams { get; }

        DateTime Started { get; }

        bool Performing { get; }

        /// <summary>
        /// Suspend the action processing
        /// </summary>
        void Suspend();

        /// <summary>
        /// Resume the action processing
        /// </summary>
        void Resume();

        ActionParam<string> GetParameter(string key);

        ActionParam<R> GetParameter<R>(string key, R defaultVal);

        /// <summary>
        /// Retuens parameters with the same name
        /// </summary>
        /// <typeparam name="R"></typeparam>
        /// <param name="key"></param>
        /// <param name="defaultVal"></param>
        /// <returns></returns>
        IEnumerable<ActionParam<R>> GetParameters<R>(string key, R defaultVal);

        ActionParam<R> GetParameterWithValidition<R>(string key, R defaultVal);

        IAction<IActionResult> CurrentAction { get; }
    }
}
