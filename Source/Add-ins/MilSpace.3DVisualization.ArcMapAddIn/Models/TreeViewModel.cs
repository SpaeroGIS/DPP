using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilSpace.Visualization3D.Models
{
    public class TreeViewModel
    {
        public List<TreeViewNodeModel> Lines { get; set; } = new List<TreeViewNodeModel>();
        public List<TreeViewNodeModel> Funs { get; set; } = new List<TreeViewNodeModel>();
        public List<TreeViewNodeModel> Primitives { get; set; } = new List<TreeViewNodeModel>();
    }
}
