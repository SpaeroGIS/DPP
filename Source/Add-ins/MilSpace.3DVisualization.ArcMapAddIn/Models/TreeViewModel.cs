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

        internal TreeViewNodeModel GetTreeViewNodeModel(Guid guid)
        {
            if (guid != null && guid != Guid.Empty)
            {
                return Lines.FirstOrDefault(item => guid.Equals(item.Guid)) ?? 
                       Funs.FirstOrDefault(item => guid.Equals(item.Guid)) ?? 
                       Primitives.FirstOrDefault(item => guid.Equals(item.Guid));                
            }
            return null;
        }
    }
}
