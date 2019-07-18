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
        private IList<TreeViewNodeModel> selectedTreeViewNodes = new List<TreeViewNodeModel>();
        private TreeViewModel treeViewModel;
        private LocalizationContext _context;
        
        internal ProfilesTreeView(LocalizationContext context)
        {
            InitializeComponent();
            _context = context;
        }

        internal IList<TreeViewNodeModel> SelectedTreeViewNodes => selectedTreeViewNodes;


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
                bufferNode.Tag = node.Guid;

                if (node.ChildNodes == null || !node.ChildNodes.Any()) continue;

                AddNodesToTreeView(bufferNode, node.ChildNodes);
            }
        }

        private void UserSessionsProfilesTreeView_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && ModifierKeys == Keys.Control)
            {
                UserSessionsProfilesTreeView.HideSelection = false;                                
            }
            else
            {
                UserSessionsProfilesTreeView.HideSelection = true;
                selectedTreeViewNodes.Clear();                
            }

            var selectedNode = UserSessionsProfilesTreeView.SelectedNode;

            if (selectedNode != null) selectedTreeViewNodes.Add(treeViewModel.GetTreeViewNodeModel(selectedNode.Tag));
        }

        private void UserSessionsProfilesTreeView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) this.DialogResult = DialogResult.OK;
        }        
    }
}
