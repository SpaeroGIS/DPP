using MilSpace.Visualization3D.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MilSpace.Visualization3D
{
    internal partial class ProfilesTreeView : Form
    {
        private TreeViewModel treeViewModel;
        private LocalizationContext _context;
        
        internal ProfilesTreeView(LocalizationContext context)
        {
            InitializeComponent();
            _context = context;
        }

        internal TreeViewModel TreeViewModel => treeViewModel;


        internal TreeViewModel LoadProfiles()
        {
            var dataAccess = new DataAccess(_context);
            treeViewModel = dataAccess.ParseUserProfileSessions();

            if (treeViewModel.Lines != null && treeViewModel.Lines.Any())
            {
                var newNode = UserSessionsProfilesTreeView.Nodes.Add(_context.Section);
                AddNodesToTreeView(newNode, treeViewModel.Lines);                
            }

            if (treeViewModel.Funs != null && treeViewModel.Funs.Any())
            {
                var newNode = UserSessionsProfilesTreeView.Nodes.Add($@"""{ _context.Fun}""");
                AddNodesToTreeView(newNode, treeViewModel.Funs);
            }

            if (treeViewModel.Primitives != null && treeViewModel.Primitives.Any())
            {
                var newNode = UserSessionsProfilesTreeView.Nodes.Add(_context.Primitive);
                AddNodesToTreeView(newNode, treeViewModel.Primitives);
            }


            return treeViewModel;
        }

        private void AddNodesToTreeView(TreeNode parentNode, IList<TreeViewNodeModel> nodesCollection)
        {
            foreach (var node in nodesCollection)
            {
                var bufferNode = parentNode.Nodes.Add(node.Name);

                if (node.ChildNodes == null || !node.ChildNodes.Any()) continue;

                AddNodesToTreeView(bufferNode, node.ChildNodes);
            }
        }
    }
}
