using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Framework;
using MilSpace.Core;
using MilSpace.Core.SolutionSettings;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MilSpace.Tools.ArcMap.Commands
{
    [Guid("8FB94798-D857-4FC3-905F-6E035EFC1F56")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("MilSpace.OpenSettingCommand")]
    [ComVisible(true)]
    public class MisSpaceSettingCommand : BaseCommand
    {
        private IApplication m_application;
        public MisSpaceSettingCommand()
        {

            //caption = "Map point Tool"


            base.m_category = "Спостереження інструменти"; //localizable text
            base.m_caption = "Open Settings";  //localizable text
            base.m_message = "Open Settings form";  //localizable text 
            base.m_toolTip = "Open Settings form";  //localizable text 
            base.m_name = "MilSpace.OpenSettingCommand";   //unique id, non-localizable (e.g. "MyCategory_ArcMapCommand")
            try
            {

                m_bitmap = new Bitmap(GetType(), "Tool.png");
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
            }

        }

        #region COM Registration Function(s)
        [ComRegisterFunction()]
        [ComVisible(false)]
        static void RegisterFunction(Type registerType)
        {
            // Required for ArcGIS Component Category Registrar support
            ArcGISCategoryRegistration(registerType);

            //
            // TODO: Add any COM registration code here
            //
        }

        [ComUnregisterFunction()]
        [ComVisible(false)]
        static void UnregisterFunction(Type registerType)
        {
            // Required for ArcGIS Component Category Registrar support
            ArcGISCategoryUnregistration(registerType);

            //
            // TODO: Add any COM unregistration code here
            //
        }

        #region ArcGIS Component Category Registrar generated code
        /// <summary>
        /// Required method for ArcGIS Component Category registration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryRegistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            MxCommands.Register(regKey);

        }
        /// <summary>
        /// Required method for ArcGIS Component Category unregistration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryUnregistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            MxCommands.Unregister(regKey);

        }

        #endregion
        #endregion
        public override void OnCreate(object hook)
        {
            if (hook != null)
                m_application = hook as IApplication;

            //if (m_application != null)
            //{
            //    base.m_enabled = m_dockableWindow != null;
            //}
            //else

            //Check if a Milspace Add-ins are installed
            base.m_enabled = true;
        }

        public override void OnClick()
        {
            SolutionSettingsForm form = new SolutionSettingsForm();
            var result = form.ShowDialog();
        }

        public override bool Checked
        {
            get
            {
                return true;
            }
        }

        #region "ArcGIS Component Category Registrar generated code"

        // ReSharper disable once InconsistentNaming
        private static void ArcGisCategoryRegistration(Type registerType)
        {
            string regKey = string.Format(CultureInfo.CurrentCulture, "HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            MxCommands.Register(regKey);
        }

        // ReSharper disable once InconsistentNaming
        private static void ArcGisCategoryUnregistration(Type registerType)
        {
            string regKey = string.Format(CultureInfo.CurrentCulture, "HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            MxCommands.Unregister(regKey);
        }

        #endregion

    }
}
