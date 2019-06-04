using ESRI.ArcGIS.SystemUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MilSpace.ProjectionsConverter;
using System.Windows.Forms;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Carto;
using MilSpace.ProjectionsConverter.Models;
using MilSpace.ProjectionsConverter.ReferenceData;
using ESRI.ArcGIS.esriSystem;
using System.Diagnostics;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Desktop.AddIns;

namespace MilSpace.ProjectionsConverter.UI
{
    [Guid("3C0C2853-F255-4360-8414-73C2E7EC3638")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("MilSpace.CoordinatesConverterCommand")]
    public class CoordinatesConverterCommand : ICommand
    {
        private IDockableWindow m_dockableWindow;
        private bool m_checked = false;
        private bool m_enabled = false;
        private const string DockableWindowGuid = "{140FA266-0614-4488-980E-199C3DEF3AD8}";

        public void OnCreate(object Hook)
        {
            if (m_dockableWindow == null)
            {
                Debugger.Launch();
                Debugger.Break();
                IDockableWindowManager dockWindowManager = Hook as IDockableWindowManager;
                if (dockWindowManager != null)
                {
                    UID windowID = new UIDClass();
                    windowID.Value = DockableWindowGuid;
                    m_dockableWindow = dockWindowManager.GetDockableWindow(windowID);
                }
                m_enabled = m_dockableWindow != null;
            }                      
        }

        public void OnClick()
        {
            if (m_dockableWindow == null)
                return;

            if (m_dockableWindow.IsVisible())
                m_dockableWindow.Show(false);
            else
                m_dockableWindow.Show(true);

            m_checked = m_dockableWindow.IsVisible();
        }

        public bool Enabled => m_enabled;

        public bool Checked => m_checked;

        public string Name => "Coordinates Converter";

        public string Caption => "Coordinates Converter";

        public string Tooltip => "Converts point coordinates to a different coordinate systems";

        public string Message => "";

        public string HelpFile => "";

        public int HelpContextID => 0;

        public int Bitmap => 0;

        public string Category => "MilSpace";        
    }
}
