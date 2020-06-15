using MilSpace.Core;
using MilSpace.DataAccess.DataTransfer;
using System.Collections.Generic;
using System.Linq;

namespace MilSpace.DataAccess.Facade
{
    public static class AddDemFacade
    {
        private static readonly Logger log = Logger.GetLoggerEx("AddDemFacade");

        public static IEnumerable<SrtmGrid> GetSrtmGrids()
        {
            using (var accessor = new AddDemDataAccess())
            {
                return accessor.GetSrtmGrids().ToArray();
            }
        }

        public static IEnumerable<SrtmGrid> GetLoadedSrtmGrids()
        {
            using (var accessor = new AddDemDataAccess())
            {
                return accessor.GetLoadedSrtmGrids().ToArray();
            }
        }

        public static IEnumerable<SrtmGrid> GetNotLoadedSrtmGrids()
        {
            using (var accessor = new AddDemDataAccess())
            {
                return accessor.GetLoadedSrtmGrids().ToArray();
            }
        }
    }
}
