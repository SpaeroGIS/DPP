using MilSpace.DataAccess.DataTransfer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilSpace.Visualization3D.Models
{
    public class TreeViewNodeModel
    {
        public string Name { get; set; }

        public Guid Guid { get; set; }

        public ProfileSession NodeProfileSession { get; set; }

        public IList<TreeViewNodeModel> ChildNodes { get; set; }
    }
}
