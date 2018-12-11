using ESRI.ArcGIS.Analyst3DTools;
using ESRI.ArcGIS.DataManagementTools;
using ESRI.ArcGIS.Geoprocessing;
using ESRI.ArcGIS.Geoprocessor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilSpace.Core.Tools.SurfaceProfile
{
    public static class ProfileLibrary
    {
        internal static bool GenerateProfileData(string lineFeatureClass, string profileSource, string outTable, string outGraphName = null, string outGraphFileName = null)
        {
            Geoprocessor gp = new ESRI.ArcGIS.Geoprocessor.Geoprocessor();
            gp.OverwriteOutput = true;

            StackProfile stackProfile = new StackProfile
            {
                in_line_features = lineFeatureClass,
                profile_targets = profileSource,
                out_table = outTable
            };

            if (!string.IsNullOrEmpty(outGraphName))
            {
                stackProfile.out_graph = outGraphName;
            }

            GeoProcessorResult gpResult = new GeoProcessorResult();
            try
            { gp.Execute(stackProfile, null); }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                // Print geoprocessing execution error messages.
                for (int i = 0; i < gp.MessageCount; i++) Console.WriteLine(gp.GetMessage(i));

                return false;
            }

            if (!string.IsNullOrEmpty(outGraphName))
            {
                SaveGraph saveG = new SaveGraph();
                saveG.in_graph = outGraphName;
                saveG.out_graph_file = outGraphFileName;
                try
                {
                    gp.Execute(saveG, null);
                }
                catch (Exception ex)
                { // Print geoprocessing execution error messages.
                    for (int i = 0; i < gp.MessageCount; i++)
                        Console.WriteLine(gp.GetMessage(i));

                    return false;
                }
            }

            return true;
        }
    }
}
