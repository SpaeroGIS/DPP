using MilSpace.Core;
using MilSpace.DataAccess.DataTransfer;
using MilSpace.DataAccess.DataTransfer.Sentinel;
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
        public static IEnumerable<SrtmGrid> GetSrtmGridsByIds(IEnumerable<int> ids)
        {
            using (var accessor = new AddDemDataAccess())
            {
                var res = from grid in accessor.GetSrtmGrids()
                where ids.Contains(grid.OBJECTID)
                select grid;

                return res.ToArray();
            }
        }

        public static IEnumerable<S1Grid> GetS1GridsByIds(IEnumerable<int> ids)
        {
            using (var accessor = new AddDemDataAccess())
            {
                var res = from grid in accessor.GetS1Grids()
                          where ids.Contains(grid.OBJECTID)
                          select grid;

                return res.ToArray();
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

        public static IEnumerable<S1Grid> GetS1Grids()
        {
            using (var accessor = new AddDemDataAccess())
            {
                return accessor.GetS1Grids().ToArray();
            }
        }

        public static S1Grid GetS1GridByTile(Tile tile)
        {
            using (var accessor = new AddDemDataAccess())
            {
                return accessor.GetS1Grids().FirstOrDefault(g => g.POINT_X == tile.Lon && g.POINT_Y == tile.Lat);
            }
        }

        public static SrtmGrid GetSrtmGridByTile(Tile tile)
        {
            using (var accessor = new AddDemDataAccess())
            {
                return accessor.GetSrtmGrids().FirstOrDefault(g => g.POINT_X == tile.Lon && g.POINT_Y == tile.Lat);
            }
        }

        public static bool UpdateS1Grid(S1Grid tile)
        {
            using (var accessor = new AddDemDataAccess())
            {
                return accessor.UpdateSiGrid(tile);
            }
        }

        public static IEnumerable<S1Grid> GetLoadedS1Grids()
        {
            using (var accessor = new AddDemDataAccess())
            {
                return accessor.GetLoadedS1Grids().ToArray();
            }
        }

        public static IEnumerable<S1Grid> GetNotLoadedS1Grids()
        {
            using (var accessor = new AddDemDataAccess())
            {
                return accessor.GetLoadedS1Grids().ToArray();
            }
        }
    }
}
