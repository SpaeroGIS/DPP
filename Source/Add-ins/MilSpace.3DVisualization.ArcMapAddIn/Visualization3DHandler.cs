using ESRI.ArcGIS.Framework;
using MilSpace.Visualization3D.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilSpace.Visualization3D
{
    public static class Visualization3DHandler
    {
        private static IApplication m_application;

        //Application removed event
        private static IAppROTEvents_Event m_appROTEvent;
        private static int m_appHWnd = 0;

        static Visualization3DHandler()
        {

        }

        internal static void OpenProfilesSetIn3D(ArcSceneArguments layers)
        {
            OpenArcScene();
        }

        private static bool OpenArcScene()
        {
            IDocument doc = null;
            try
            {
                doc = new ESRI.ArcGIS.ArcScene.SxDocument();
            }
            catch
            {
                return false;
            }

            if(doc != null)
            {
                m_appROTEvent = new AppROTClass();
                m_appROTEvent.AppRemoved += new IAppROTEvents_AppRemovedEventHandler(m_appROTEvent_AppRemoved);

                m_application = doc.Parent;
                m_application.Visible = true;
                m_appHWnd = m_application.hWnd;
            }
            else
            {
                m_appROTEvent = null;
                m_application = null;

                return false;
            }

            return true;
        }

        #region "Handle the case when the application is shutdown by user manually"
        static void m_appROTEvent_AppRemoved(AppRef pApp)
        {
            //Application manually shuts down. Stop listening
            if(pApp.hWnd == m_appHWnd)
            {
                m_appROTEvent.AppRemoved -= new IAppROTEvents_AppRemovedEventHandler(m_appROTEvent_AppRemoved);
                m_appROTEvent = null;
                m_application = null;
                m_appHWnd = 0;
            }
        }
        #endregion
    }

}
