using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilSpace.DataAccess.Migrations
{
    [Migration(1)]
    public class Migration0001: Migration
    {
        public override void Up()
        {
            Execute.Sql(@"
if NOT EXISTs(select * from sys.objects where name = 'MDR_Projects')
begin
CREATE TABLE [dbo].[MDR_Projects](
	[idRow] [int] IDENTITY(1,1) NOT NULL,
	[idProject] [nvarchar](50) NOT NULL,
	[Filename] [nvarchar](255) NOT NULL,
	[Title] [nvarchar](256) NULL,
	[DtCreate] [datetime] NULL,
	[sOperEdit] [nvarchar](50) NULL,
	[DTEdit] [datetime] NULL,
	[sOperCreate] [nvarchar](50) NULL,
	[isDel] [int] NULL
) ON [PRIMARY]

end
GO

if NOT EXISTs(select * from sys.objects where name = 'MDR_Reports')
begin
CREATE TABLE [dbo].[MDR_Reports](
	[idRow] [int] IDENTITY(1,1) NOT NULL,
	[idReport] [nvarchar](50) NOT NULL,
	[idProject] [nvarchar](50) NULL,
	[Filename] [nvarchar](255) NULL,
	[sFormat] [nvarchar](8) NULL,
	[countDObj] [int] NULL,
	[DPI] [int] NULL,
	[hMatrix] [int] NULL,
	[wMatrix] [int] NULL,
	[DefContent] [int] NULL,
	[isDel] [int] NULL,
	[sOperCreate] [nvarchar](50) NULL,
	[DTCreate] [datetime] NULL
) ON [PRIMARY]
end
GO
");
            
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }

}
