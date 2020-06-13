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
    internal class DemPreparationDataAccess : DataAccessor<DemPreparationContext>, IDisposable
    {
        internal DemPreparationDataAccess()
        {
            log.InfoEx(
                $"Initialise GeoCalculatorDataAccess with connection: " +
                $"{MilSpaceConfiguration.ConnectionProperty.DemPreparationDBConnection}"
                );
        }

        public override string ConnectionString => MilSpaceConfiguration.ConnectionProperty.DemPreparationDBConnection;

        internal S1Sources GetSoureceByName(string sourceName)
        {
            try
            {
                return context.S1Sources.FirstOrDefault(s => s.idscene.ToUpper().Equals(sourceName.ToUpper()));
            }
            catch (Exception ex)
            {
                log.WarnEx($"Unexpected exception:{ex.Message}");
            }
            return null;
        }

        internal S1Sources AddSource(S1Sources source)
        {

            if (GetSoureceByName(source.idscene) == null)
            {
                context.S1Sources.InsertOnSubmit(source);
                try
                {
                    context.S1Sources.InsertOnSubmit(source);
                    Submit();

                    return GetSoureceByName(source.idscene);

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
            }

            return null;
        }
    }
}
