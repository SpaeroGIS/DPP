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

        internal TreeViewNodeModel GetTreeViewNodeModel(object guid)
        {
            if (guid != null && Guid.TryParse(guid.ToString(), out Guid uid))
            {
                return Lines.Concat(Funs.Concat(Primitives)).FirstOrDefault(item => uid.Equals(item.Guid)); 
            }

            return null;
        }
    }
}
