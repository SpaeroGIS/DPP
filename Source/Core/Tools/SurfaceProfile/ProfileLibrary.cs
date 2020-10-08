using ESRI.ArcGIS.Analyst3DTools;
using ESRI.ArcGIS.ConversionTools;
using ESRI.ArcGIS.DataManagementTools;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geoprocessing;
using ESRI.ArcGIS.Geoprocessor;
using MilSpace.Configurations;
using MilSpace.Core;
using MilSpace.Core.Tools;
using MilSpace.DataAccess.DataTransfer.Sentinel;
using MilSpace.DataAccess.Facade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace MilSpace.Tools.SurfaceProfile
{
    public static class CalculationLibrary
    {
        private static readonly string environmentName = "workspace";
        private static readonly string temporaryWorkspace = MilSpaceConfiguration.ConnectionProperty.TemporaryGDBConnection;
        private static Logger log = Logger.GetLoggerEx("ProfileLibrary");
        private const string NonvisibleCellValue = "NODATA";
        private const string ClippingGeometry = "ClippingGeometry";


        #region RasterToOtherFormat
        private const string rasterOutFormat = "TIFF";
        #endregion
        private static Geoprocessor gp = null;

        //-------------------------------------------------------------------------
        static CalculationLibrary()
        {
        }

        //-------------------------------------------------------------------------
        internal static bool GenerateProfileData(
            string lineFeatureClass,
            string profileSource,
            string outTable,
            IEnumerable<string> messages,
            string outGraphName = null
            )
        {
            StackProfile stackProfile = new StackProfile
            {
                in_line_features = lineFeatureClass,
                profile_targets = profileSource,
                out_table = outTable
            };

            if (!string.IsNullOrEmpty(outGraphName)) stackProfile.out_graph = outGraphName;


            return RunTool(stackProfile, null, out messages);
        }
        //-------------------------------------------------------------------------

        public static bool GenerateVisibilityData(
                        string rasterSource,
                        string observerPointsFeatureClass,
                        VisibilityAnalysisTypesEnum analyzeType,
                        string outRasterName,
                        out IEnumerable<string> messages,
                        VisibilityCurvatureCorrectionEnum curvatureCorrection = VisibilityCurvatureCorrectionEnum.FLAT_EARTH,
                        string outAglRaster = null
                        )
        {

            Visibility visibility = new Visibility
            {
                analysis_type = analyzeType.ToString().ToUpper(),
                in_raster = rasterSource,
                in_observer_features = observerPointsFeatureClass,
                out_raster = outRasterName
            };

            if (!string.IsNullOrWhiteSpace(outAglRaster))
            {
                visibility.out_agl_raster = outAglRaster;
            }

            visibility.nonvisible_cell_value = NonvisibleCellValue;

            visibility.horizontal_start_angle = VisibilityFieldsEnum.AzimuthB.ToString();
            visibility.horizontal_end_angle = VisibilityFieldsEnum.AzimuthE.ToString();
            visibility.vertical_lower_angle = VisibilityFieldsEnum.AnglMinH.ToString();
            visibility.vertical_upper_angle = VisibilityFieldsEnum.AnglMaxH.ToString();
            visibility.surface_offset = "";
            visibility.observer_elevation = "";
            visibility.observer_offset = VisibilityFieldsEnum.HRel.ToString();
            visibility.inner_radius = VisibilityFieldsEnum.InnerRadius.ToString();
            visibility.outer_radius = VisibilityFieldsEnum.OuterRadius.ToString();

            visibility.curvature_correction = curvatureCorrection.ToString();


            return RunTool(visibility, null, out messages);
        }


        public static bool ClipRasterByArea(
            string inRaster,
            string outRaster,
            Tile tile,
            out IEnumerable<string> messages,
            string workspace = null,
            string clippingGeometry = ClippingGeometry
            )
        {
            var lr = GdbAccess.GetRasterLayerFromFile(inRaster);
            var rasterpr = lr.Raster as IRasterProps;
            

            IEnvelope templateDataset = tile.EsriGeometry;
            templateDataset.Project(rasterpr.SpatialReference);
            var area = $"{templateDataset.XMin.ToString().Replace(",", ".")} {templateDataset.YMin.ToString().Replace(",", ".")} " +
                $"{templateDataset.XMax.ToString().Replace(",", ".")} {templateDataset.YMax.ToString().Replace(",", ".")}";


            Clip clipper = new Clip()
            {
                rectangle = area,
                in_raster = inRaster,
                out_raster = outRaster,
                maintain_clipping_extent = "MAINTAIN_EXTENT"
                // clipping_geometry = clippingGeometry
            };

            clipper.nodata_value = NonvisibleCellValue;

            return RunTool(clipper, null, out messages, @"E:\Data\S1\Temp\");
        }

        public static bool ClipVisibilityZonesByAreas(
            string inRaster,
            string outRaster,
            string templateDataset,
            out IEnumerable<string> messages,
            string clippingGeometry = ClippingGeometry
            )
        {

            Clip clipper = new Clip()
            {
                in_template_dataset = templateDataset,
                in_raster = inRaster,
                out_raster = outRaster,
                clipping_geometry = clippingGeometry
            };

            clipper.nodata_value = NonvisibleCellValue;

            return RunTool(clipper, null, out messages);
        }

        public static bool ConvertRasterToPolygon(string inRaster, string outPolygon, out IEnumerable<string> messages)
        {
            RasterToPolygon rasterToPolygon = new RasterToPolygon()
            {
                in_raster = inRaster,
                out_polygon_features = outPolygon,
                simplify = "SIMPLIFY",
                raster_field = "Value"
            };

            return RunTool(rasterToPolygon, null, out messages);
        }

        public static bool RasterToOtherFormat(IEnumerable<string> inputRasters, string outputFolder, out IEnumerable<string> messages)
        {
            RasterToOtherFormat coppier = new RasterToOtherFormat
            {
                Input_Rasters = string.Join(";", inputRasters.ToArray()),
                Output_Workspace = outputFolder,
                Raster_Format = rasterOutFormat
            };

            return RunTool(coppier, null, out messages);
        }

        public static bool MosaicToRaster(IEnumerable<string> inputRasters, string outputPath, string outputFile, out IEnumerable<string> messages,
            string workspace)
        {
            MosaicToNewRaster runner = new MosaicToNewRaster(string.Join(";", inputRasters.ToArray()), outputPath, outputFile, 1);
            runner.coordinate_system_for_the_raster = EsriTools.Wgs84Spatialreference.FactoryCode;
            runner.pixel_type = "32_BIT_FLOAT";
            runner.mosaic_method = "MEAN";
            runner.mosaic_colormap_mode = "FIRST";

            log.InfoEx("Starting MosaicToRaster..");
            var result = RunTool(runner, null, out messages, workspace);

            if (messages.Any(m => m.StartsWith("ERROR")))
            { result = false; }

            return result;
        }

        private static bool RunTool(IGPProcess process, ITrackCancel TC, out IEnumerable<string> messages, string workspace = null)
        {
            if (gp == null)
            {
                gp = new Geoprocessor
                {
                    AddOutputsToMap = false
                };

                if (string.IsNullOrEmpty(workspace))
                {
                    workspace = temporaryWorkspace;
                }
                gp.SetEnvironmentValue(environmentName, temporaryWorkspace);
            }

            gp.OverwriteOutput = true; // Set the overwrite output option to true
            bool result = true;
            try
            {
                IGeoProcessorResult pResult = (IGeoProcessorResult)gp.Execute(process, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                result = false;
            }

            messages = ReturnMessages(gp).ToArray();

            Marshal.ReleaseComObject(gp.IGeoProcessor);

            gp = null;

            return result;
        }
        //-------------------------------------------------------------------------
        private static IEnumerable<string> ReturnMessages(Geoprocessor gp)
        {
            if (gp.MessageCount > 0)
            {
                var result = new string[gp.MessageCount];
                for (int count = 0; count < gp.MessageCount; count++)
                {
                    result[count] = gp.GetMessage(count);
                    log.WarnEx(result[count]);
                }
                return result;
            }
            return null;
        }
    }
}
