using MilSpace.Core.ModulesInteraction;
using Sposterezhennya.AddDEM.ArcMapAddin.AddInComponents;
using Sposterezhennya.AddDEM.ArcMapAddin.Interaction;
using System;
using System.Windows.Forms;

namespace Sposterezhennya.AddDEM.ArcMapAddin
{
    /// <summary>
    /// Designer class of the dockable window add-in. It contains user interfaces that
    /// make up the dockable window.
    /// </summary>
    public partial class DockableDEMWindow : UserControl, IAddDemView
    {
        AddDemController controller;

        public DockableDEMWindow(object hook, AddDemController controller)
        {
            InitializeComponent();
            this.Hook = hook;
            this.controller = controller;
            this.controller.RegisterView(this);
        }

        /// <summary>
        /// Host object of the dockable window
        /// </summary>
        private object Hook
        {
            get;
            set;
        }

        /// <summary>
        /// Implementation class of the dockable window add-in. It is responsible for 
        /// creating and disposing the user interface class of the dockable window.
        /// </summary>
        public class AddinImpl : ESRI.ArcGIS.Desktop.AddIns.DockableWindow
        {
            private DockableDEMWindow m_windowUI;

            public AddinImpl()
            {
            }

            protected override IntPtr OnCreateChild()
            {
                AddDemController controller = new AddDemController();
                ModuleInteraction.Instance.RegisterModuleInteraction<IAddDemInteraction>(new AddDemInteraction(controller));

                m_windowUI = new DockableDEMWindow(this.Hook, controller);
                return m_windowUI.Handle;
            }

            protected override void Dispose(bool disposing)
            {
                if (m_windowUI != null)
                    m_windowUI.Dispose(disposing);

                base.Dispose(disposing);
            }

        }

        private void button8_Click(object sender, EventArgs e)
        {
            controller.OpenDemCalcForm();
        }
    }
}
