using System;
using FluentMigrator.Runner.VersionTableInfo;

namespace MilSpace.DataAccess.Database
{
    [VersionTableMetaData]
    public class MigrationVersionTable : DefaultVersionTableMetaData
    {
        public override string AppliedOnColumnName => "BuildDate";

        public override string DescriptionColumnName => "DatabaseVersion";

        public override string TableName => "Versions";
    }
}