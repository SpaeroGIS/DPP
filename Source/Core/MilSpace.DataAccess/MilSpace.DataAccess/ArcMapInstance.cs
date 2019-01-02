using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Editor;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;
using MilSpace.Visibility.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilSpace.DataAccess
{

    public static class ArcMapInstance
    {
        private static IApplication s_app = null;
        private static IDocumentEvents_Event s_docEvent;

        public static IApplication Application
        {
            get
            {
                if (s_app == null)
                {
                    s_app = AddInStartupObject.GetHook<IMxApplication>() as IApplication;
                    if (s_app == null)
                    {
                        IEditor editorHost = AddInStartupObject.GetHook<IEditor>();
                        if (editorHost != null)
                            s_app = editorHost.Parent;
                    }
                }
                return s_app;
            }
            set
            {
                if (s_app == null)
                {
                    s_app = value;
                }
            }
        }

        public static IMxDocument Document
        {
            get
            {
                if (Application != null)
                    return Application.Document as IMxDocument;

                return null;
            }
        }
        public static IMxApplication ThisApplication
        {
            get { return Application as IMxApplication; }
        }
        public static IDockableWindowManager DockableWindowManager
        {
            get { return Application as IDockableWindowManager; }
        }
        public static IDocumentEvents_Event Events
        {
            get
            {
                s_docEvent = Document as IDocumentEvents_Event;
                return s_docEvent;
            }
        }
        public static IEditor Editor
        {
            get
            {
                UID editorUID = new UID();
                editorUID.Value = "esriEditor.Editor";
                return Application.FindExtensionByCLSID(editorUID) as IEditor;
            }
        }
    }
}

