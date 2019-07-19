using MilSpace.Core;
using MilSpace.Core.Exceptions;

namespace MilSpace.DataAccess.Exceptions
{
    public class MilSpaceCanotDeletePrifileCalcTable : MilSpaceException
    {
        private static string messageTempleate = "The temporary table {0} cannot be deleterd fron {1} ";

        public MilSpaceCanotDeletePrifileCalcTable(string tableName, string sourceGdb) : base()
        {
            errorMessage = messageTempleate.InvariantFormat(tableName, sourceGdb);
        }
    }
}
