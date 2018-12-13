using FluentMigrator;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Announcers;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.Processors;
using FluentMigrator.Runner.Processors.SqlServer;
using MilSpace.Core;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MilSpace.DataAccess.Database
{
    public class MigrationManager
    {
        public void DoMigration(string connectionString)
        {
            // Run Migrations v 2.*. Doc:
            // https://github.com/Athari/FluentMigrator/wiki/
            var processLog = new StringBuilder();

            processLog.AppendLine("Prepare migration.");
            Announcer announcer = new TextWriterAnnouncer(s => processLog.AppendLine(s))
            {
                ShowSql = true
            };

            Assembly assembly = Assembly.GetExecutingAssembly();
            IRunnerContext migrationContext = new RunnerContext(announcer);

            var options = new ProcessorOptions
            {
                PreviewOnly = false,  // set to true to see the SQL
                Timeout = new TimeSpan(0, 0, 60)
            };
            var factory = new SqlServerProcessorFactory();
            processLog.AppendLine($"Connecting to Db: {connectionString.AutoEllipses(20)}.");
            try
            {
                using (IMigrationProcessor processor = factory.Create(connectionString, announcer, options))
                {
                    var runner = new MigrationRunner(assembly, migrationContext, processor);

                    processLog.AppendLine("Start migration.");
                    processor.BeginTransaction();
                    try
                    {
                        runner.MigrateUp(false);
                        processor.CommitTransaction();
                    }
                    catch
                    {
                        processor.RollbackTransaction();
                        throw;
                    }

                    processLog.AppendLine("End migration.");

                   //TODO: Add notification
                }
            }
            catch (Exception ex)
            {
                //TODO: Add notification

            }
        }



      

        public long GetLastVersionNumber()
        {
            try
            {
                return Assembly.GetExecutingAssembly().GetTypes().Where(x => x.BaseType == typeof(Migration))
                    .SelectMany(c => c.GetCustomAttributes(typeof(MigrationAttribute)))
                    .Max(x => ((MigrationAttribute)x).Version);
            }
            catch (Exception ex)
            {
                //TODO: Add notification
                return -1;
            }
        }
    }
}
