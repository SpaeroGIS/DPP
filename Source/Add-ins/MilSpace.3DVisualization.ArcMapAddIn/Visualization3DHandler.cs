using ESRI.ArcGIS.Framework;
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

        public static void OpenProfilesSetIn3D(/*string dem*/)
        {
            OpenArcScene();
        }

        private static bool OpenArcScene()
        {
            IDocument doc = null;
            try
            {
                //doc = new ESRI.ArcGIS.ArcScene.SxDocumentClass();
                doc = new ESRI.ArcGIS.ArcScene.SxDocument();
            }
            catch
            {
                return false;
            } //Fail if you haven't installed the target application

            if(doc != null)
            {
                //Advanced (AppROT event): Handle manual shutdown, comment out if not needed
                m_appROTEvent = new AppROTClass();
                m_appROTEvent.AppRemoved += new IAppROTEvents_AppRemovedEventHandler(m_appROTEvent_AppRemoved);

                //Get a reference of the application and make it visible
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
            if(pApp.hWnd == m_appHWnd) //compare by hwnd
            {
                m_appROTEvent.AppRemoved -= new IAppROTEvents_AppRemovedEventHandler(m_appROTEvent_AppRemoved);
                m_appROTEvent = null;
                m_application = null;
                m_appHWnd = 0;

                //Reset UI has to be in the form UI thread of this application, 
                //not the AppROT thread;
               /* if(this.InvokeRequired) //i.e. not on the right thread
                {
                    this.BeginInvoke(new IAppROTEvents_AppRemovedEventHandler(AppRemovedResetUI), pApp);
                }
                else
                {
                    AppRemovedResetUI(pApp); //call directly
                }*/
            }
        }
      /*  private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            //Clean up
            if(m_appROTEvent != null)
            {
                m_appROTEvent.AppRemoved -= new IAppROTEvents_AppRemovedEventHandler(m_appROTEvent_AppRemoved);
                m_appROTEvent = null;
            }
        }*/
        #endregion
    }

}
