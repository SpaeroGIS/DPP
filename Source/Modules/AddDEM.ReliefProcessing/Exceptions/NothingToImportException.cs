using MilSpace.Core;
using MilSpace.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilSpace.AddDem.ReliefProcessing.Exceptions
{
    public class NothingToImportException : MilSpaceException
    {
        private static string messageTempleate = "The folder \"{0}\" doesnot contain files to import";

        public NothingToImportException(string folder): base()
        {
            errorMessage = messageTempleate.InvariantFormat(folder);
        }
    }
}
