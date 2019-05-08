using MilSpace.DataAccess.DataTransfer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilSpace.DataAccess
{
    public class GroupedLines
    {
        public List<ProfileLine> Lines { get; set; }
        public int LineId { get; set; }
    }
}
