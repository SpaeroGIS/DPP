using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator;
using FluentMigrator.Builders.Alter.Table;
using FluentMigrator.Builders.Create.Table;
using FluentMigrator.Infrastructure;

namespace MilSpace.DataAccess.Database
{
    public static class MigrationsExtension
    {
        /// <summary>
        /// Use
        /// this.CreateTableIfNotExists("table", table => table.WithColumn("Id").AsInt32().NotNullable());
        /// </summary>
        /// <param name="self"></param>
        /// <param name="tableName"></param>
        /// <param name="constructTableFunction"></param>
        /// <param name="schemaName"></param>
        /// <returns></returns>
        public static IFluentSyntax CreateTableIfNotExists(this MigrationBase self, string tableName,
            Func<ICreateTableWithColumnOrSchemaOrDescriptionSyntax, IFluentSyntax> constructTableFunction,
            string schemaName = "dbo")
        {
            return !TableExists(self, tableName)
                ? constructTableFunction(self.Create.Table(tableName))
                : null;
        }
        /// <summary>
        /// Use
        /// this.TableExists("table");
        /// </summary>
        /// <param name="self"></param>
        /// <param name="tableName"></param>
        /// <param name="schemaName"></param>
        /// <returns></returns>
        public static bool TableExists(this MigrationBase self, string tableName,
            string schemaName = "dbo")
        {
            return self.Schema.Schema(schemaName).Table(tableName).Exists();
        }

        /// <summary>
        /// Use
        /// this.ColumnExists("Table", "Column");
        /// </summary>
        /// <param name="self"></param>
        /// <param name="tableName"></param>
        /// <param name="colName"></param>
        /// <param name="schemaName"></param>
        /// <returns></returns>
        public static bool ColumnExists(this MigrationBase self, string tableName, string colName, string schemaName = "dbo")
        {
            return TableExists(self, tableName) &&
                   self.Schema.Schema(schemaName).Table(tableName).Column(colName).Exists();
        }

        /// <summary>
        /// Use
        /// this.CreateColIfNotExists("Table", "Col", col => col.AsInt32().Nullable().Indexed());
        /// </summary>
        /// <param name="self"></param>
        /// <param name="tableName"></param>
        /// <param name="colName"></param>
        /// <param name="constructColFunction"></param>
        /// <param name="schemaName"></param>
        /// <returns></returns>
        public static IFluentSyntax CreateColIfNotExists(this MigrationBase self, string tableName, string colName,
            Func<IAlterTableColumnAsTypeSyntax, IFluentSyntax> constructColFunction, string schemaName = "dbo")
        {
            return !ColumnExists(self, tableName, colName)
                ? constructColFunction(self.Alter.Table(tableName).AddColumn(colName))
                : null;
        }


        public static bool KeyExists(this MigrationBase self, string tableName, string keyName, string schemaName = "dbo")
        {
            return TableExists(self, tableName) &&
                   self.Schema.Schema(schemaName).Table(tableName).Constraint(keyName).Exists();
        }
    }
}
