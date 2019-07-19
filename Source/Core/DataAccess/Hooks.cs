using ESRI.ArcGIS.Desktop.AddIns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilSpace.DataAccess
{
    [StartupObjectAttribute()]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public sealed partial class AddInStartupObject : AddInEntryPoint
    {
        private static AddInStartupObject _sAddInHostManager;
        private List<object> m_addinHooks = null;

        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Never)]
        public AddInStartupObject()
        {
        }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Never)]
        protected override bool Initialize(object hook)
        {
            bool createSingleton = _sAddInHostManager == null;
            if (createSingleton)
            {
                _sAddInHostManager = this;
                m_addinHooks = new List<object>();
                m_addinHooks.Add(hook);
            }
            else if (!_sAddInHostManager.m_addinHooks.Contains(hook))
                _sAddInHostManager.m_addinHooks.Add(hook);

            return createSingleton;
        }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Never)]
        protected override void Shutdown()
        {
            _sAddInHostManager = null;
            m_addinHooks = null;
        }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Never)]
        internal static T GetHook<T>() where T : class
        {
            if (_sAddInHostManager != null)
            {
                foreach (object o in _sAddInHostManager.m_addinHooks)
                {
                    if (o is T)
                        return o as T;
                }
            }

            return null;
        }

        // Expose this instance of Add-in class externally
        public static AddInStartupObject GetThis()
        {
            return _sAddInHostManager;
        }
    }
}
