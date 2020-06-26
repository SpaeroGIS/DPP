using System;
using System.IO.Compression;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MilSpace.Core.Zip
{
    public class ZipManager : IDisposable
    {
        private bool disposed = false;
        private readonly string zipFileName;
        private static Logger log = Logger.GetLoggerEx("ZipManager");
        ZipArchive archive;

        public ZipManager(string zipFileName)
        {
            if (OpenAndValidateZipfile(zipFileName))
            {
                this.zipFileName = zipFileName;
            }
        }


        public IEnumerable<string> GetFileNamesFormFolder(string folderName)
        {
            return archive.Entries.Where(e => e.Length > 0 && e.FullName.StartsWith(folderName)).Select(e => e.FullName);

        }


        public void GoThrogh()
        {
            foreach (ZipArchiveEntry entry in archive.Entries)
            {
                log.InfoEx($"File: {entry.FullName}");
            }
        }


        public IEnumerable<string> GetFoldersStructure()
        {
            return archive.Entries.Where(e => e.CompressedLength == 0).Select(e => e.FullName);
        }


        public Stream GetEntry(string entryName)
        {

            var entry = archive.GetEntry(entryName);
            if (entry != null)
            {
                return entry.Open();
            }

            return null;
        }

        private bool OpenAndValidateZipfile(string fileName)
        {
            if (!File.Exists(fileName))
            {
                log.ErrorEx($"The file {fileName} doesn't exist.");
                return false;
            }

            try
            {
                //zipToOpen = new FileStream(fileName, FileMode.Open);
                archive = ZipFile.OpenRead(fileName);/* new ZipArchive(zipToOpen, ZipArchiveMode.Read);*/
                return true;
            }
            catch (Exception commonException)
            {
                log.ErrorEx(commonException.Message);
            }
            return false;
        }

        public void Dispose() => Dispose(true);

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                archive.Dispose();
            }

            disposed = true;
        }

    }
}
