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

        internal IList<TreeViewNodeModel> SelectedTreeViewNodes { get; } = new List<TreeViewNodeModel>();


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

        private void UserSessionsProfilesTreeView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) this.DialogResult = DialogResult.OK;
        }

        private void UserSessionsProfilesTreeView_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            var selectedNode = e.Node;

            if (selectedNode != null)
            {
                if (Guid.TryParse(selectedNode.Tag?.ToString(), out Guid guid) && guid != Guid.Empty)
                {
                    var profileNode = treeViewModel.GetTreeViewNodeModel(guid);

                    if (ModifierKeys == Keys.Control)
                    {
                        if (SelectedTreeViewNodes.Contains(profileNode))
                        {
                            SelectedTreeViewNodes.Remove(profileNode);
                            selectedNode.ForeColor = UserSessionsProfilesTreeView.ForeColor;
                            selectedNode.BackColor = UserSessionsProfilesTreeView.BackColor;
                            e.Cancel = true;
                        }
                        else
                        {
                            SelectedTreeViewNodes.Add(profileNode);
                            selectedNode.BackColor = SystemColors.Highlight;
                            selectedNode.ForeColor = SystemColors.HighlightText;
                        }
                    }
                    else
                    {
                        ClearSelection(UserSessionsProfilesTreeView.Nodes);

                        SelectedTreeViewNodes.Clear();
                        SelectedTreeViewNodes.Add(profileNode);
                        selectedNode.BackColor = SystemColors.Highlight;
                        selectedNode.ForeColor = SystemColors.HighlightText;
                    }
                }
            }
        }

        private void ClearSelection(TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes)
            {
                node.ForeColor = UserSessionsProfilesTreeView.ForeColor;
                node.BackColor = UserSessionsProfilesTreeView.BackColor;

                if (node.Nodes != null && node.Nodes.Count > 0)
                    ClearSelection(node.Nodes);
            }
        }
    }
}
