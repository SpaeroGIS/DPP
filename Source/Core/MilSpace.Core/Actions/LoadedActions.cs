using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using log4net;
using MilSpace.Configurations;
using MilSpace.Core.Actions.Base;
using MilSpace.Core.Actions.Exceptions;
using MilSpace.Core.Actions.Interfaces;

namespace MilSpace.Core.Actions
{
    public class LoadedActions
    {

        private static Guid baseClaassGuid = new Guid("70ABDCB3-2CAF-4D34-A4A3-B3024A1862F8");
        private static Dictionary<string, ActionDefinition> actions = null;
        private static ReadOnlyCollection<ActionDefinition> actionsDefinition = null;

        public delegate T ObjectActivator<T>(params object[] args);

        private const string areaDescriptionClassName = "AreaDescription";
        private const string areaNotDefined = "Area is not defined";

        internal static IEnumerable<ActionDefinition> GetLoadedActions()
        {
            FillActions();
            if (actionsDefinition == null)
            {
                actionsDefinition = actions.Values.ToList().AsReadOnly();
            }

            return actionsDefinition;
        }

        public static bool ActionExists(string actionId)
        {
            FillActions();

            var existed = string.IsNullOrEmpty(actionId) ? false : actions.ContainsKey(actionId);
            return existed;
        }

        internal static IAction<IActionResult> GetAction(string actionId, ActionProcessor prmtrs)
        {
            FillActions();

            if (actions.Keys.Any(k => k.Equals(actionId, StringComparison.InvariantCultureIgnoreCase)))
            {

                Type t = actions[actionId].ActionClassType;
                //Type t = actions.First(k => k.Key.Equals(actionId, StringComparison.InvariantCultureIgnoreCase)).Value.ActionClassType;

                var action = GetActionInstacebyType(t, prmtrs);

                if (action.Description.IsEmpty)
                {
                    Logger.Instance.Warn(new ActionDescriptionNotDefinedException(action).Message);
                    //   throw new ActionDescriptionNotDefinedException(action);
                }

                return action;
            }

            return null;
        }

        internal static string[] GetActionParamNames(string actionId)
        {
            FillActions();

            if (actions.Keys.Any(k => k.Equals(actionId, StringComparison.InvariantCultureIgnoreCase)))
            {
                string[] p = actions.First(k => k.Key.Equals(actionId, StringComparison.InvariantCultureIgnoreCase)).Value.Paramenetrs.Select(pr => pr.ParamName).ToArray();
                return p;
            }

            return null;
        }

        internal static IActionParam[] GetActionParams(string actionId)
        {
            FillActions();

            if (actions.Keys.Any(k => k.Equals(actionId, StringComparison.InvariantCultureIgnoreCase)))
            {
                IActionParam[] p = actions.First(k => k.Key.Equals(actionId, StringComparison.InvariantCultureIgnoreCase)).Value.Paramenetrs;
                return p;
            }

            return null;
        }

        internal static T GetDefaultInstance<T>()
        {
            ConstructorInfo ctor = typeof(T).GetConstructors().First();
            ObjectActivator<T> createdActivator = GetActivator<T>(ctor);
            return createdActivator(null);
        }

        internal static ObjectActivator<T> GetActivator<T>(ConstructorInfo ctor)
        {
            Type type = ctor.DeclaringType;
            ParameterInfo[] paramsInfo = ctor.GetParameters();

            //create a single param of type object[]
            ParameterExpression param = Expression.Parameter(typeof(object[]), "args");

            Expression[] argsExp = new Expression[paramsInfo.Length];

            //pick each arg from the params array 
            //and create a typed expression of them
            for (int i = 0; i < paramsInfo.Length; i++)
            {
                Expression index = Expression.Constant(i);
                Type paramType = paramsInfo[i].ParameterType;
                Expression paramAccessorExp = Expression.ArrayIndex(param, index);
                Expression paramCastExp = Expression.Convert(paramAccessorExp, paramType);
                argsExp[i] = paramCastExp;
            }

            //make a NewExpression that calls the
            //ctor with the args we just created

            NewExpression newExp = Expression.New(ctor, argsExp);

            //Create obk=ject without params
            //NewExpression newExp = Expression.New(ctor);

            //create a lambda with the New
            //Expression as body and our param object[] as arg 
            //LambdaExpression lambda = Expression.Lambda(typeof(ObjectActivator<T>), newExp, param);

            LambdaExpression lambda = Expression.Lambda(typeof(ObjectActivator<T>), newExp, param);

            //compile it
            ObjectActivator<T> compiled = (ObjectActivator<T>)lambda.Compile();
            return compiled;
        }

        private static IAction<IActionResult> GetActionInstacebyType(Type tp, ActionProcessor prmtrs)
        {

            if (!tp.GetConstructors().Any(c => c.GetParameters().Count() == 0))
            {
                throw new ArgumentNullException(string.Format("The type {0} must hane a default constructor", tp.Name));
            }

            int paramCnt = prmtrs == null ? 0 : 1;

            ConstructorInfo ctor = tp.GetConstructors().First(c => c.GetParameters().Count() == paramCnt);
            ObjectActivator<IAction<IActionResult>> createdActivator = GetActivator<IAction<IActionResult>>(ctor);
            return createdActivator(prmtrs);

        }

        private static void FillActions()
        {
            if (actions == null)
            {
                actions = new Dictionary<string, ActionDefinition>();
                var s = ActionConfiguration.LoadActionAssemblies;

                if (s.Length == 0)
                {
                    throw new Exception("There must be defined actions!");
                }

                Assembly[] ass = AppDomain.CurrentDomain.GetAssemblies();
                foreach (ActionsToLoad atl in s)
                {
                    if (!ass.Any(a => a.FullName.Equals(atl.AssemblyName)))
                    {
                        string ew = string.Empty;
                        AssemblyName an = new AssemblyName(atl.AssemblyName);
                        try
                        {
                            var assemb = AppDomain.CurrentDomain.Load(an);
                        }
                        catch (ArgumentNullException nullEx)
                        {
                            ew = nullEx.Message;
                        }
                        catch (FileNotFoundException fnn)
                        {
                            ew = fnn.Message;
                        }
                        catch (FileLoadException fle)
                        {
                            ew = fle.Message;
                        }
                        catch (Exception fnqn)
                        {
                            ew = fnqn.Message;
                        }
                        finally
                        {
                            if (!string.IsNullOrEmpty(ew))
                            {
                                Logger.Instance.Error(ew);
                            }
                        }
                    }
                }

                ILog log = LogManager.GetLogger(typeof(LoadedActions));
                List<Type> typesl = new List<Type>();
                Dictionary<Assembly, string> areaDesc = new Dictionary<Assembly, string>();
                foreach (Assembly asmb in AppDomain.CurrentDomain.GetAssemblies())
                {
                    try
                    {
                        var availableTypes = asmb.GetTypes();
                        if (availableTypes.Any(t => t.BaseType != null && t.BaseType.GUID.Equals(baseClaassGuid)))
                        {
                            log.InfoFormat("Reading actions from the assembly {0}", asmb.FullName);
                            var tps = availableTypes.Where(t => t.BaseType != null && t.BaseType.GUID.Equals(baseClaassGuid));
                            areaDesc.Add(asmb, GetAreaDescription(availableTypes));
                            typesl.AddRange(tps.ToArray());
                        }
                    }
                    catch (Exception ex)
                    {
                        log.FatalFormat("Cannot get types form the registred assembly {0}. \n{1}", asmb.FullName, ex.Message);
                    }
                }

                //IEnumerable<Type> types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes().Where(t => t.BaseType != null && t.BaseType.GUID.Equals(baseClaassGuid))).ToArray();
                IEnumerable<Type> types = typesl.ToArray();

                foreach (Type t in types)
                {
                    IAction<IActionResult> instance = GetActionInstacebyType(t, null);
                    if (actions.ContainsKey(instance.ActionId))
                    {
                        ActionDefinition existedAction = actions[instance.ActionId];
                        log.FatalFormat("The action {0} has been already defined. Action type {1}.", instance.ActionId, existedAction.ActionClassType.AssemblyQualifiedName);
                    }
                    else
                    {
                        actions.Add(instance.ActionId, new ActionDefinition()
                        {
                            ActionClassType = t,
                            ActionId = instance.ActionId,
                            Paramenetrs = instance.ParametersTemplate,
                            Description = instance.Description,
                            Area = areaDesc[t.Assembly]
                        });
                        log.InfoFormat("Load action {0}. Action type {1}.", instance.ActionId, instance.GetType().AssemblyQualifiedName);
                    }
                }
            }
        }

        private static string GetAreaDescription(IEnumerable<Type> types)
        {
            var areaDesc = types.FirstOrDefault(desc => desc.Name.Equals(areaDescriptionClassName));
            if (areaDesc == null)
            {
                string assemblyName = types.First().Module.Name;
                Logger.Warn("The assembly \"" + assemblyName + "\" doesn't contain AreaDefinition.");
                return assemblyName;
            }

            PropertyInfo field = areaDesc.GetProperty("Area", BindingFlags.Public | BindingFlags.Static);
            if (field == null)
            {
                string assemblyName = types.First().Module.Name;
                Logger.Warn("The assembly \"" + assemblyName + "\" contains wrong AreaDefinition. Cannot find a field description.");
                return assemblyName;
            }

            string areaDescription = field.GetValue(null, null).ToString();
            return areaDescription;
        }

    }
}
