using MilSpace.AddDem.ReliefProcessing.Exceptions;
using MilSpace.Configurations;
using MilSpace.Core;
using MilSpace.Core.ModulesInteraction;
using MilSpace.DataAccess.DataTransfer.Sentinel;
using MilSpace.DataAccess.Facade;
using MilSpace.Tools.CopyRaster;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace MilSpace.AddDem.ReliefProcessing
{
    public class PrepareDemControllerSrtm
    {
        static Logger log = Logger.GetLoggerEx("PrepareDemControllerSrtm");
        private List<Tile> tiles = new List<Tile>();
        IPrepareDemViewSrtm prepareSrtmView;
        internal PrepareDemControllerSrtm()
        { }

        public void SetView(IPrepareDemViewSrtm view)
        {
            prepareSrtmView = view;
        }

        public void SelectSRTMStorageExternal()
        {
            using (var opedFolder = new FolderBrowserDialog())
            {

                if (opedFolder.ShowDialog() == DialogResult.OK)
                {

                    MilSpaceConfiguration.DemStorages.SrtmStorageExternal = opedFolder.SelectedPath;
                    MilSpaceConfiguration.Save();
                    ReadConfiguration();

                    ReadSrtmFilesFromFolder();

                }
            }
        }

        public void ReadConfiguration()
        {
            log.InfoEx("Reading Configuration....");
            if (prepareSrtmView == null)
            {
                throw new MethodAccessException("prepareDemview cannot be null");
            }
            try
            {
                prepareSrtmView.SrtmSrtorage = MilSpaceConfiguration.DemStorages.SrtmStorage;
                prepareSrtmView.SrtmSrtorageExternal = MilSpaceConfiguration.DemStorages.SrtmStorageExternal;
            }
            catch (Exception ex)
            {
                log.ErrorEx($"Error occured {ex.Message}");
            }

            log.InfoEx("Configuration was read");
        }

        public void ReadSrtmFilesFromFolder(bool replaceExisted = true)
        {
            var sourceFolder = MilSpaceConfiguration.DemStorages.SrtmStorageExternal;
            var sourceFolde = new DirectoryInfo(sourceFolder);

            if (sourceFolde.Exists)
            {

                var importfiles = sourceFolde.GetFiles("*.tif");

                var storageDbFiles = (replaceExisted ? AddDemFacade.GetSrtmGrids() : AddDemFacade.GetNotLoadedSrtmGrids()) ?? throw new InvalidDataException("Cannot read SRTM Grig data. For more detailed infor go to the Log file");
                var storageFiles = storageDbFiles.Select(gi => new FileInfo(Path.Combine(MilSpaceConfiguration.DemStorages.SrtmStorage, gi.FileName)));

                var filtered = importfiles.Where(impf => storageDbFiles.Any(sf => impf.Name.EndsWith(sf.FileName)));

                if (!filtered.Any())
                {
                    throw new NothingToImportException(sourceFolder);
                }

                prepareSrtmView.SrtmFilesInfo = filtered.ToArray();
            }
            else
            {
                throw new DirectoryNotFoundException(sourceFolder);
            }
        }

        public bool CopySrtmFilesToStorage()
        {
            var mgr = new CopyRasterManager();
            try
            {
                mgr.CopySrtmRasterToStoreage(prepareSrtmView.SrtmFilesInfo?.Select(fi => fi.FullName), prepareSrtmView.ReplaceSrtmFiles);
                return true;
            }
            catch (FileLoadException ex)
            {
                //List of files is empty
                log.ErrorEx(ex.Message);
            }
            catch (FormatException ex)
            {
                //Not all files wer coppied
                log.ErrorEx(ex.Message);
            }
            catch (Exception ex)
            {
                log.ErrorEx("Unexpected error: " + ex.Message);
            }

            return false;
        }

        internal void AddTilesToList(IEnumerable<Tile> newTiles)
        {
            tiles.Clear();
            newTiles.ToList().
                ForEach(tile =>
                {
                    if (!tiles.Any(t => t.Equals(tile)))
                    {
                        tiles.Add(tile);
                    }
                });
        }

        public Tile GetTilesByPoint()
        {
            var latString = prepareSrtmView.TileLatitudeSrtm;
            var lonString = prepareSrtmView.TileLongitudeSrtm;
            Tile testTile = null;

            if (latString.TryParceToDouble(out double latDouble) && lonString.TryParceToDouble(out double lonDouble))
            {
                int lat = Convert.ToInt32(latDouble);
                int lon = Convert.ToInt32(lonDouble);

                if (!tiles.Any(t => t.Lat == lat && t.Lon == lon))
                {
                    testTile = new Tile
                    {
                        Lat = lat,
                        Lon = lon
                    };

                }
            }

            return testTile;
        }

        public IEnumerable<Tile> Tiles => tiles;

        internal Tile AddTileToList()
        {
            var tile = GetTilesByPoint();
            if (tile != null)
            {

                tiles.Add(tile);
            }
            return tile;
        }

        public bool AddSrtmFileToMap()
        {

            var addDemModule = GetAddDemModule();
            if (addDemModule == null)
            {
                throw new EntryPointNotFoundException();
            }

            var srtmFileName = prepareSrtmView.SelectedSrtmFile;

            if (string.IsNullOrEmpty(srtmFileName))
                return false;

            var fsrtmFile = prepareSrtmView.SrtmFilesInfo.FirstOrDefault(fi => fi.Name == srtmFileName);

            if (fsrtmFile == null)
                return false;

            return addDemModule.AddDemToMap(fsrtmFile.FullName);
        }

        private static IAddDemInteraction GetAddDemModule()
        {
            var addDemModule = ModuleInteraction.Instance.GetModuleInteraction<IAddDemInteraction>(out bool changes);

            if (!changes && addDemModule == null)
            {
                log.ErrorEx($"> GetTargetObservPoints Exception: {LocalizationContext.Instance.FindLocalizedElement("MsgObservPointscModuleDoesnotExistText", "Модуль \"Видимість\" не було підключено. Будь ласка додайте модуль до проекту, щоб мати можливість взаємодіяти з ним")}");
                return null;
            }
            return addDemModule;
        }
    }
}
