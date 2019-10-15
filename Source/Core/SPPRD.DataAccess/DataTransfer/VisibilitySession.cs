using System;

namespace MilSpace.DataAccess.DataTransfer
{
    public class VisibilitySession
    {
        public int IdRow;
        public string Id;
        public string Name;
        public string UserName;
        public DateTime Created;
        public DateTime? Started;
        public DateTime? Finished;
        public int CalculatedResults;
        public string ReferencedGDB;
    }
}
