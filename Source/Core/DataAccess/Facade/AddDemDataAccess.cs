using MilSpace.Configurations;
using MilSpace.DataAccess.DataTransfer;
using MilSpace.DataAccess.Definition;
using MilSpace.DataAccess.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilSpace.DataAccess.Facade
{

    internal class AddDemDataAccess : DataAccessor<MilSpaceStorageContext>, IDisposable
    {
        public override string ConnectionString => MilSpaceConfiguration.ConnectionProperty.WorkingDBConnection;

        internal IEnumerable<SrtmGrid> GetSrtmGrids()
        {
            try
            {
                return context.MilSp_SrtmGrids.Select(g => g.Get());
            }
            catch (MilSpaceDataException ex)
            {
                log.WarnEx(ex.Message);

                if (ex.InnerException != null)
                {
                    log.WarnEx(ex.InnerException.Message);
                }
            }
            catch (Exception ex)
            {
                log.WarnEx($"Unexpected exception:{ex.Message}");

            }
            return null;
        }

        internal IEnumerable<S1Grid> GetS1Grids()
        {
            try
            {
                return context.MilSp_S1Grids.Select(g => g.Get());
            }
            catch (MilSpaceDataException ex)
            {
                log.WarnEx(ex.Message);

                if (ex.InnerException != null)
                {
                    log.WarnEx(ex.InnerException.Message);
                }
            }
            catch (Exception ex)
            {
                log.WarnEx($"Unexpected exception:{ex.Message}");

            }
            return null;
        }

        internal IEnumerable<SrtmGrid> GetNotLoadedSrtmGrids()
        {
            return GetLoadedSrtmGrids(false);
        }

        internal IEnumerable<SrtmGrid> GetLoadedSrtmGrids()
        {
            return GetLoadedSrtmGrids(true);
        }

        internal IEnumerable<S1Grid> GetNotLoadedS1Grids()
        {
            return GetLoadedS1Grids(false);
        }

        internal IEnumerable<S1Grid> GetLoadedS1Grids()
        {
            return GetLoadedS1Grids(true);
        }

        internal bool UpdateSiGrid(S1Grid s1Tile)
        {
            try
            {
                var tileToUpdate = context.MilSp_S1Grids.FirstOrDefault(g =>
                g.SRTM.ToUpper() == s1Tile.SRTM.ToUpper());

                if (tileToUpdate == null)
                {
                    log.WarnEx($"Cannot find Si tile {s1Tile.SRTM} ");
                    return false;
                }

                tileToUpdate.Loaded = (short)(s1Tile.Loaded ? 1 : 0);

                Submit();
                log.InfoEx($"Tile {tileToUpdate.SRTM} was successfully updated");
                return true;
            }
            catch (MilSpaceDataException ex)
            {
                log.WarnEx(ex.Message);

                if (ex.InnerException != null)
                {
                    log.WarnEx(ex.InnerException.Message);
                }
            }
            catch (Exception ex)
            {
                log.WarnEx($"> Update S1 Tile EXCEPTION:{ex.Message}");
            }

            return false;
        }
        private IEnumerable<SrtmGrid> GetLoadedSrtmGrids(bool isLoaded)
        {
            try
            {
                short loaded = (short)(isLoaded ? 1 : 0);

                return context.MilSp_SrtmGrids.Where(g => g.Loaded == 0).Select(g => g.Get());
            }
            catch (MilSpaceDataException ex)
            {
                log.WarnEx(ex.Message);

                if (ex.InnerException != null)
                {
                    log.WarnEx(ex.InnerException.Message);
                }
            }
            catch (Exception ex)
            {
                log.WarnEx($"Unexpected exception:{ex.Message}");

            }
            return null;
        }



        private IEnumerable<S1Grid> GetLoadedS1Grids(bool isLoaded)
        {
            try
            {
                short loaded = (short)(isLoaded ? 1 : 0);

                return context.MilSp_S1Grids.Where(g => g.Loaded == 0).Select(g => g.Get());
            }
            catch (MilSpaceDataException ex)
            {
                log.WarnEx(ex.Message);

                if (ex.InnerException != null)
                {
                    log.WarnEx(ex.InnerException.Message);
                }
            }
            catch (Exception ex)
            {
                log.WarnEx($"Unexpected exception:{ex.Message}");

            }
            return null;
        }
    }
}
