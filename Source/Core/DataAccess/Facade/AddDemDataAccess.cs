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

        internal IEnumerable<SrtmGrid> GetLoadedSrtmGrids()
        {
            try
            {
                return context.MilSp_SrtmGrids.Where(g => g.Loaded == 1).Select(g => g.Get());
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
