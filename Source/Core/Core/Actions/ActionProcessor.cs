using MilSpace.Core.Actions.Base;
using MilSpace.Core.Actions.Exceptions;
using MilSpace.Core.Actions.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Serialization;
using System.Threading;
using System.Web;

namespace MilSpace.Core.Actions
{

    public class ActionProcessor : IActionProcessor
    {
        /// <summary>
        /// List of alive action processors
        /// </summary>
        private static List<IActionProcessor> processing = new List<IActionProcessor>();

        private readonly HttpContext context;
        private List<IActionParam> prosessingActionParams = new List<IActionParam>();
        private static string templateArgs = "{0}:";
        private IAction<IActionResult> act = null;
        private string[] usedParameters;
        public static string ArgTemplate = "{0}:{1}";
        private ActionFinishedDelegate onActionFinished;
        private delegate IActionResult ProcessAsyncCaller();
        private ActionStatesEnum state = ActionStatesEnum.Idle;
        private Thread currentTherad = null;
        ManualResetEvent manualResetEvent = null;


        private Guid processId = Guid.NewGuid();
        private DateTime started;


        /// <summary>
        /// Check if the Actop processor is started. Used for checking Async processing
        /// </summary>
        private bool performing = true;

        public event ActionProcessDelegate ProcessMonitor;

        public ActionProcessor(HttpContext context)
        {

            processing.Add(this);
            state = ActionStatesEnum.Initiating;
            IEnumerable<string> curParams;
            IEnumerable<IActionParam> prms = null;
            this.context = context;

            string val = context.Request.Params[ActionParamNamesCore.Action];
            if (this.ActionExists(val))
            {
                curParams = this.usedParameters.Where(p => context.Request.Params[p] != null);
                if (curParams.Count() > 0)
                {
                    prms = curParams.Where(x => !String.IsNullOrEmpty(context.Request.Params[x]))
                                    .Select(p => new ActionParam<string>()
                                    { ParamName = p, Value = context.Request.Params[p] })
                                    .Cast<IActionParam>();

                    this.prosessingActionParams.AddRange(prms);
                }
                this.InitiateAction(val);
            }

            state = ActionStatesEnum.Processing;
        }

        public ActionProcessor(string[] args)
        {
            processing.Add(this);
            state = ActionStatesEnum.Initiating;
            this.context = null;

            IEnumerable<string> curParams;
            IEnumerable<IActionParam> prms = null;

            string valFull = args.FirstOrDefault(a => a.StartsWith(string.Format(templateArgs, ActionParamNamesCore.Action)));

            string val = string.IsNullOrEmpty(valFull) ? valFull : GetParamValue(ActionParamNamesCore.Action, valFull);
            if (this.ActionExists(val))
            {
                curParams = this.usedParameters.Where(p => args.Any(a => a.StartsWith(string.Format(templateArgs, p))));
                if (curParams.Count() > 0)
                {
                    prms = curParams.Select(p => new ActionParam<string>()
                    {
                        ParamName = p,
                        Value = GetParamValue(p, args.First(a => a.StartsWith(string.Format(templateArgs, p))))
                    })
                    .Cast<IActionParam>();
                    this.prosessingActionParams.AddRange(prms);
                }

                this.InitiateAction(val);
            }

            state = ActionStatesEnum.Initiated;
        }

        public IAction<IActionResult> CurrentAction
        {
            get { return this.act; }
        }

        /// <summary>
        /// Gets a Action Processor's state
        /// </summary>
        public ActionStatesEnum State
        {
            get { return state; }
        }

        public ActionProcessor(IEnumerable<IActionParam> args)
        {
            processing.Add(this);
            state = ActionStatesEnum.Initiating;
            this.context = null;
            IEnumerable<string> curParams;
            IEnumerable<IActionParam> prms = null;

            if (args.Any(a => a.ParamName == ActionParamNamesCore.Action))
            {
                var s = args.First(a => a.ParamName == ActionParamNamesCore.Action);

                ActionParam<string> ap = (ActionParam<string>)Convert.ChangeType(args.First(a => a.ParamName == ActionParamNamesCore.Action)
                         , typeof(ActionParam<string>), null);

                string val = ap.Value;

                if (this.ActionExists(val))
                {
                    curParams = this.usedParameters.Where(p => args.Any(a => a.ParamName == p));
                    if (curParams.Count() > 0)
                    {
                        prms = args.Where(aa => this.usedParameters.Any(r => r.Equals(aa.ParamName, StringComparison.InvariantCultureIgnoreCase))).Cast<IActionParam>();
                        this.prosessingActionParams.AddRange(prms);
                    }

                    this.InitiateAction(val);
                }
            }
        }

        public ActionParam<string> GetParameter(string key)
        {
            return GetParameter<string>(key, string.Empty);
        }

        public ActionParam<T> GetParameter<T>(string key, T defaultVal)
        {
            ActionParam<T> paramVale = new ActionParam<T>
            {
                Value = defaultVal
            };

            if (this.ContainsParam(key))
            {
                IActionParam val = this.prosessingActionParams.Single(s => s.ParamName.Equals(key));

                ////TODO: Сделать проверку на возможность конвертирования значения в тип
                //// т.к. переданное значение часто не соответствует типу параметра, 
                //// включая случаи, когда параметр пустой (...&param=&...)
                //if (!String.IsNullOrEmpty(val.ToString()))
                //{
                ActionParam<T> param = (ActionParam<T>)Convert.ChangeType(val, typeof(ActionParam<T>), null);
                paramVale = param;
                //}
            }
            return paramVale;
        }

        public IEnumerable<ActionParam<T>> GetParameters<T>(string key, T defaultVal)
        {
            ActionParam<T> paramDefVale = new ActionParam<T>
            {
                Value = defaultVal
            };

            IEnumerable<ActionParam<T>> paramVales = new ActionParam<T>[] { paramDefVale };

            if (this.ContainsParam(key))
            {
                List<IActionParam> values = this.prosessingActionParams.Where(s => s.ParamName.Equals(key)).ToList();

                paramVales = values.Select(val => (ActionParam<T>)Convert.ChangeType(val, typeof(ActionParam<T>), null));
            }

            return paramVales;
        }

        public ActionParam<T> GetParameterWithValidition<T>(string key, T checkingVal)
        {
            ActionParam<T> paramValue = GetParameter<T>(key, checkingVal);

            if ((paramValue.IsDefault || paramValue.Value.Equals(checkingVal)) && checkingVal.GetType() != typeof(bool))
            {
                throw new ExpectActionParameterException(this.act.ActionId, paramValue.ParamName);
            }

            return paramValue;
        }

        public void Suspend()
        {
            var ss = CurrentThread;
            if (manualResetEvent != null)
            {
                manualResetEvent.WaitOne();
            }
        }

        /// <summary>
        /// Resume the action processing
        /// </summary>
        public void Resume()
        {
            if (manualResetEvent != null)
            { manualResetEvent.Set(); }
        }

        public bool ContainsParam(string key)
        {
            return this.prosessingActionParams.Any(pair => pair.ParamName.Equals(key));
        }

        public HttpContext Context
        {
            get { return this.context; }
        }

        public IActionResult Process()
        {
            this.CheckConsistency();
            IActionResult result = null;
            if (act != null)
            {
                MilSpace.Core.Logger.Info(string.Format("Action \"{0}\" processing", act.ActionId));
                this.started = DateTime.Now;
                state = ActionStatesEnum.Processing;

                if (ProcessMonitor != null)
                {
                    ProcessMonitorDelegate = ProcessMonitor;
                    ProcessMonitorDelegate(act.Description, state);
                }

                currentTherad = Thread.CurrentThread;
                manualResetEvent = new ManualResetEvent(false);
                act.Process();
                result = act.GetResult();

                MilSpace.Core.Logger.Info(string.Format("Action \"{0}\" processed", act.ActionId));
                result.PeocessId = this.ProcessId;
                state = ActionStatesEnum.Finished;
                ProcessMonitor?.Invoke(act.Description, state);
                this.performing = false;
            }

            processing.Remove(this);
            return result;
        }

        public static IActionResult Process(IEnumerable<IActionParam> args, ActionProcessDelegate monitor = null)
        {

            var procc = new ActionProcessor(args);
            if (monitor != null)
            {
                procc.ProcessMonitor += monitor;
            }
            var res = procc.Process<IActionResult>();

            if (monitor != null)
            {
                procc.ProcessMonitor -= monitor;
            }

            return res;

        }

        public static T Process<T>(IEnumerable<IActionParam> args, ActionProcessDelegate monitor = null) where T : IActionResult
        {
            var procc = new ActionProcessor(args);
            if (monitor != null)
            {
                procc.ProcessMonitor += monitor;
            }
            var res = procc.Process<T>();
            if (monitor != null)
            {
                procc.ProcessMonitor -= monitor;
            }

            return res;
        }

        public void ProcessAsync(ActionFinishedDelegate onActionFinished)
        {
            MilSpace.Core.Logger.Info(string.Format("Action \"{0}\" with id {1} processing async", act.ActionId, this.ProcessId));
            this.onActionFinished = onActionFinished;
            ProcessAsyncCaller caller = new ProcessAsyncCaller(this.Process);
            IAsyncResult result = caller.BeginInvoke(new AsyncCallback(Worker_DoWork), null);
        }

        private void Worker_DoWork(IAsyncResult ar)
        {

            AsyncResult result = (AsyncResult)ar;
            ProcessAsyncCaller caller = (ProcessAsyncCaller)result.AsyncDelegate;

            IActionResult res = caller.EndInvoke(result);

            if (this.onActionFinished != null)
            {
                MilSpace.Core.Logger.Info(string.Format("Action \"{0}\" with id {1} processed async", act.ActionId, this.ProcessId));
                MilSpace.Core.Logger.Info(string.Format("Action \"{0}\" calling callback function", act.ActionId));
                this.onActionFinished(res);
                MilSpace.Core.Logger.Info(string.Format("Action \"{0}\" callback function performed", act.ActionId));
            }

            //    caller.
        }

        void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            this.CheckConsistency();

            if (act != null)
            {
                act.Process();
                if (this.onActionFinished != null)
                {
                    this.onActionFinished.Invoke(act.GetResult());
                }
            }
        }

        public T Process<T>()
        {
            return (T)this.Process();
        }

        private void CheckConsistency()
        {
            if (this.act == null)
                throw new Exception("The action is undefined");
        }

        public IEnumerable<IActionParam> ProcessingActionParams
        {
            get { return prosessingActionParams; }
        }

        public DateTime Started
        {
            get { return this.started; }
        }

        public bool Performing
        {
            get { return performing; }
        }

        public Guid ProcessId
        {
            get { return processId; }
        }

        private bool ActionExists(string actionId)
        {
            if (LoadedActions.ActionExists(actionId)) // GetParamValue(ActionParamNamesCore.Action, )
            {
                this.usedParameters = LoadedActions.GetActionParamNames(actionId);
                return true;
            }

            return false;
        }

        private IAction<IActionResult> InitiateAction(string actionId)
        {
            if (string.IsNullOrEmpty(actionId))
            {
                return null;
            }

            if (LoadedActions.ActionExists(actionId)) // GetParamValue(ActionParamNamesCore.Action, )
            {
                act = LoadedActions.GetAction(actionId, this);
                return act;
            }
            return null;
        }

        private Thread CurrentThread
        {
            get
            {
                if (currentTherad == null)
                {
                    int waitForThreadSec = 3;
                    DateTime check = DateTime.Now;
                    while (currentTherad == null)
                    {
                        double diff = new TimeSpan((DateTime.Now.Ticks - check.Ticks)).TotalSeconds;
                        if (diff > waitForThreadSec)
                        {
                            Logger.Error(string.Format("Action \"{0}\". Cannot access the action thread during {1} second(s)", act.ActionId, waitForThreadSec));
                            throw new CannotAccessThreadException(this.CurrentAction.ActionId);
                        }
                    }
                }

                return currentTherad;
            }
        }

        public ActionProcessDelegate ProcessMonitorDelegate
        {
            get;
            private set;
        }

        private static string GetParamValue(string param, string fullValue)
        {
            return fullValue.Substring(string.Format(templateArgs, param).Length);
        }


    }

    [Serializable]
    internal class CannotAccessThreadException : Exception
    {
        public CannotAccessThreadException()
        {
        }

        public CannotAccessThreadException(string message) : base(message)
        {
        }

        public CannotAccessThreadException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CannotAccessThreadException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

}
