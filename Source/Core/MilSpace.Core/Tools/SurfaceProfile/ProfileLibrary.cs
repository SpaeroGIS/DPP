using ESRI.ArcGIS.Analyst3DTools;
using ESRI.ArcGIS.DataManagementTools;
using ESRI.ArcGIS.Geoprocessing;
using ESRI.ArcGIS.Geoprocessor;
using ESRI.ArcGIS.esriSystem;
using MilSpace.ArcGIS.License;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MilSpace.Configurations;

namespace MilSpace.Core.Tools.SurfaceProfile
{
    public static class ProfileLibrary
    {
        private static readonly string environmentName = "workspace";
        private static readonly string temporaryWorkspace = MilSpaceConfiguration.ConnectionProperty.TemporaryGDBConnection;

        //-------------------------------------------------------------------------
        static ProfileLibrary()
        {
            if (!MilSpaceToolsLicenseInitializer.Instance.Initialized)
            {
                MilSpaceToolsLicenseInitializer.Instance.InitializeApplication();
            }
        }
        //-------------------------------------------------------------------------
        internal static bool GenerateProfileData(
            string lineFeatureClass, 
            string profileSource, 
            string outTable, 
            string outGraphName = null 
            //string outGraphFileName = null
            )
        {
            Geoprocessor gp = new Geoprocessor();

            StackProfile stackProfile = new StackProfile();
            stackProfile.in_line_features = lineFeatureClass;
            stackProfile.profile_targets = profileSource;
            stackProfile.out_table = outTable;
            if (!string.IsNullOrEmpty(outGraphName)) stackProfile.out_graph = outGraphName;

            GeoProcessorResult gpResult = new GeoProcessorResult();

            gp.SetEnvironmentValue(environmentName, temporaryWorkspace);
            return RunTool(gp, stackProfile, null);
        }
        //-------------------------------------------------------------------------
        private static bool RunTool(Geoprocessor gp, IGPProcess process, ITrackCancel TC)
        {
            gp.OverwriteOutput = true; // Set the overwrite output option to true
            try
            {
                IGeoProcessorResult pResult = (IGeoProcessorResult) gp.Execute(process, null);
                ReturnMessages(gp);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ReturnMessages(gp);
                return false;
            }
            return true;
        }
        //-------------------------------------------------------------------------
        private static void ReturnMessages(Geoprocessor gp)
        {
            if (gp.MessageCount > 0)
            {
                for (int Count = 0; Count < gp.MessageCount; Count++) Console.WriteLine(gp.GetMessage(Count));
            }
        }
    }
}
