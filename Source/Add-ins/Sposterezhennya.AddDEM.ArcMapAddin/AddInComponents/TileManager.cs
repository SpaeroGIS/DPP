using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using MilSpace.Configurations;
using MilSpace.Core.Tools;
using MilSpace.DataAccess.DataTransfer;
using MilSpace.DataAccess.DataTransfer.Sentinel;
using MilSpace.DataAccess.Facade;
using System;
using System.Collections.Generic;
using IO = System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using MilSpace.Core;
using MilSpace.Tools.SurfaceProfile;

namespace Sposterezhennya.AddDEM.ArcMapAddin.AddInComponents
{
    internal class TileManager
    {
        private static string srtmGrigsFeatureClassName = "MILSP_SRTM_GRID";
        private static string s1GrigsFeatureClassName = "MILSP_S1_GRID";
        static Logger log = Logger.GetLoggerEx("TileManager");
        static IFeatureClass srtmGridFeatureClass;
        static IFeatureClass s1GridFeatureClass;

        public static IFeatureClass SrtmGridFeatureClass
        {
            get
            {
                if (srtmGridFeatureClass == null)
                {
                    srtmGridFeatureClass = GdbAccess.Instance.GetFeatureFromWorkingWorkspace(srtmGrigsFeatureClassName);
                }

                return srtmGridFeatureClass;
            }
        }

        public static IFeatureClass S1GridFeatureClass
        {
            get
            {
                if (s1GridFeatureClass == null)
                {
                    s1GridFeatureClass = GdbAccess.Instance.GetFeatureFromWorkingWorkspace(s1GrigsFeatureClassName);
                }

                return s1GridFeatureClass;
            }
        }

        public static IEnumerable<S1Grid> GetS1GridByArea(IGeometry area)
        {
            return AddDemFacade.GetS1GridsByIds(Intersect(area, S1GridFeatureClass));
        }
        public static IEnumerable<SrtmGrid> GetSrtmGridByArea(IGeometry area)
        {
            return AddDemFacade.GetSrtmGridsByIds(Intersect(area, SrtmGridFeatureClass));
        }

        public static ILayer GetRasterLayer(string rasterFileName)
        {
            ILayer res = null;
            if (IO.File.Exists(rasterFileName))
            {
                res = GdbAccess.GetRasterLayerFromFile(rasterFileName);
                if (res == null)
                {
                    //Write log
                }

            }
            return res;
        }
        public static ILayer GetRasterLayer(string tileName, DemSourceTypeEnum sourceType)
        {
            Tile tile = new Tile(tileName);
            if (tile.IsEmpty)
            {
                return null;
            }
            var pathToSoursec = string.Empty;
            var rasterFileName = string.Empty;
            if (sourceType == DemSourceTypeEnum.Sentinel1)
            {
                var s1Tile = AddDemFacade.GetS1GridByTile(tile);
                pathToSoursec = MilSpaceConfiguration.DemStorages.SentinelStorage;
                rasterFileName = System.IO.Path.Combine(MilSpaceConfiguration.DemStorages.SentinelStorage, s1Tile.FileName);
            }
            else if (sourceType == DemSourceTypeEnum.STRM)
            {
                var srtmTile = AddDemFacade.GetSrtmGridByTile(tile);
                pathToSoursec = MilSpaceConfiguration.DemStorages.SrtmStorage;
                rasterFileName = System.IO.Path.Combine(MilSpaceConfiguration.DemStorages.SrtmStorage, srtmTile.FileName);
            }

            return GetRasterLayer(rasterFileName);
        }

        private static IEnumerable<int> Intersect(IGeometry area, IFeatureClass featureClass)
        {
            if (featureClass == null)
            {
                throw new Exception("Cannot find Grid Feature class");
            }
            ISpatialFilter spatialFilter = new SpatialFilterClass
            {
                Geometry = area,
                GeometryField = featureClass.ShapeFieldName,
                SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects
            };

            IFeatureCursor cursor = featureClass.Search(spatialFilter, false);
            IFeature grid = null;
            var results = new List<int>();
            while ((grid = cursor.NextFeature()) != null)
            {
                results.Add(Convert.ToInt32(grid.get_Value(0)));
            }

            // Discard the cursors as they are no longer needed.
            Marshal.ReleaseComObject(cursor);

            return results;
        }

        public static bool GenerateTiles(IEnumerable<string> filesToCombine, string resultFileName)
        {
            bool processing = true;

            //var pathToTempFile = IO.Path.GetTempPath();
            var pathToTempFile = IO.Path.Combine(MilSpaceConfiguration.DemStorages.SentinelStorage, "Temp");
            if (!IO.Directory.Exists(pathToTempFile))
            {
                IO.Directory.CreateDirectory(pathToTempFile);
            }

            IEnumerable<string> messages = new List<string>();

            log.InfoEx("Starting MosaicToRaster...");

            var list = filesToCombine.ToList();

            var tempFilesToDelete = new List<string>();
            var commonMessages = new List<string>();

            var tempFilePath = string.Empty;
            var tileount = 3;

            while (list.Count > 0)
            {
                var tempFileName = $"{MilSpace.DataAccess.Helper.GetTemporaryNameSuffix()}.tif";
                tempFilePath = IO.Path.Combine(pathToTempFile, tempFileName);


                var temp = list.Take(list.Count < tileount ? list.Count : tileount);
                list = list.Except(temp).ToList();
                if (list.Count > 0)
                {
                    tempFilesToDelete.Add(tempFilePath);
                    list.Add(tempFilePath);
                }
                else
                {
                    var fileInfo = new IO.FileInfo(resultFileName);
                    tempFileName = fileInfo.Name;
                    pathToTempFile = fileInfo.DirectoryName;
                    tempFilePath = resultFileName;
                }

                
                processing = CalculationLibrary.MosaicToRaster(temp, pathToTempFile, tempFileName, out messages, IO.Path.Combine(MilSpaceConfiguration.DemStorages.SentinelStorage, "Temp"));
                commonMessages.AddRange(messages);
                if (!processing)
                {
                    break;
                }
            }

            commonMessages.ForEach(m => { if (processing) log.InfoEx(m); else log.ErrorEx(m); });

            if (!processing)
            { return false; }

            if (!IO.File.Exists(tempFilePath))
            {
                processing = false;
                (messages.ToList()).Add($"ERROR:  Об'єднаний файл  {tempFilePath} не було знайдено!");
                return false;
            }

            //IO.File.Copy(tempFilePath, resultFileName, true);

            IEnumerable<string> messagesToClip;
            processing = false;
            messages = commonMessages;

            tempFilesToDelete.ForEach(f =>
            {
                var fl = new IO.FileInfo(f);
                string templateToDel = $"{fl.Name.Substring(0, fl.Name.Length - 3)}*";

                fl.Directory.GetFiles(templateToDel).ToList().ForEach(fd =>
                {
                    try
                    {
                        IO.File.Delete(fd.FullName);
                    }
                    catch (Exception ex)
                    {
                        log.ErrorEx($"Cannot delete {fd.FullName}");
                        log.ErrorEx(ex.Message);
                    }
                });
            });

            return true;
        }

    }
}
