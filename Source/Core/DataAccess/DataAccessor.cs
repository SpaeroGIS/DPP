using MilSpace.Core;
using MilSpace.Core.DataAccess;
using MilSpace.DataAccess.Exceptions;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;

namespace MilSpace.DataAccess
{
    public abstract class DataAccessor<T> where T : DataContext, IDisposable
    {

        bool disposed;
        protected T context;
        protected readonly string connectionString;
        protected Logger log = Logger.GetLoggerEx("MilSpace.DataAccess.DataAccessor");

        public DataAccessor() : this(null)
        {

        }
        public DataAccessor(string[] contextParams)
        {
            List<string> patamsoContext = new List<string>() { ConnectionString };
            if (contextParams != null && contextParams.Length > 0)
            {
                patamsoContext.AddRange(contextParams);
            }
            //log = Logger.GetLoggerEx(GetType());
            context = (T)Activator.CreateInstance(typeof(T), patamsoContext.ToArray());
        }

        protected bool Submit()
        {
            try
            {
                log.DebugEx("> Submit. {0} Submit changes", GetType());
                context.SubmitChanges(System.Data.Linq.ConflictMode.ContinueOnConflict);
            }
            catch (Exception ex)
            {
                log.ErrorEx("> Submit Exception:{0}", ex.Message);

                //TODO: Handle inserded or updated data
                //context.GetChangeSet().Inserts;
                throw new MilSpaceDataException(DataOperationsEnum.Submit, ex);
            }

            return true;
        }

        public abstract string ConnectionString { get; }

        protected string GetTableName(Type T)
        {
            var table = context.Mapping.GetTables().FirstOrDefault(t => t.RowType.Type.Equals(T));
            if (table == null)
            {
                throw new MilSpaceDataException(T.ToString(), DataOperationsEnum.Access, null);
            }
            return table.TableName;
        }

        public void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    if (this.context != null)
                    {
                        this.context.Dispose();
                        this.context = null;
                    }
                }
                disposed = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~DataAccessor()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
    }
}
