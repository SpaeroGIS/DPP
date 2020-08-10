using MilSpace.AddDem.ReliefProcessing.Exceptions;
using MilSpace.Configurations;
using MilSpace.Core;
using MilSpace.DataAccess.Facade;
using MilSpace.Tools.CopyRaster;
using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace MilSpace.AddDem.ReliefProcessing
{
    public class PrepareDemControllerSrtm
    {
        Logger log = Logger.GetLoggerEx("PrepareDemControllerSrtm");

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
                }
            }
        }

        public void ReadConfiguration()
        {
            if (prepareSrtmView == null)
            {
                throw new MethodAccessException("prepareDemview cannot be null");
            }
            prepareSrtmView.SrtmSrtorage = MilSpaceConfiguration.DemStorages.SrtmStorage;
            prepareSrtmView.SrtmSrtorageExternal = MilSpaceConfiguration.DemStorages.SrtmStorageExternal;
        }

        public void ReadSrtmFilesFromFolder(string sourceFolder, bool replaceExisted = true)
        {
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
                mgr.CopySrtmRasterToStoreage(prepareSrtmView.SrtmFilesInfo?.Select(fi => fi.FullName), false);
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
    }
}
