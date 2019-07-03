using System;
using System.Windows.Forms;

namespace MilSpace.Profile.ModalWindows
{
    public partial class ProfilesTreeModalWindow : Form
    {
        public int SelectedSessionId;
        public int SelectedLineId;

        public ProfilesTreeModalWindow(TreeView calsProfilesTreeView)
        {
            InitializeComponent();
            profilesTreeView.Width = Width;
            InitializeTreeView(calsProfilesTreeView);
        }

        private void InitializeTreeView(TreeView calsTreeView)
        {
            foreach (TreeNode node in profilesTreeView.Nodes)
            {
                var calsNode = calsTreeView.Nodes[node.Name];

                CopyChildren(node, calsNode);
            }
        }

        public void CopyChildren(TreeNode parent, TreeNode original)
        {
            if (original.Nodes.Count == 0)
            {
                return;
            }

            TreeNode newNode;
            foreach (TreeNode node in original.Nodes)
            {
                newNode = new TreeNode(node.Text);
                newNode.Tag = node.Tag;
                parent.Nodes.Add(newNode);
                CopyChildren(newNode, node);
            }
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            var node = profilesTreeView.SelectedNode;
            var ids = GetProfileAndLineIds(node);

            SelectedSessionId = ids.Item1;
            SelectedLineId = ids.Item2;
        }

        private Tuple<int, int> GetProfileAndLineIds(TreeNode node)
        {
            int id = -1;
            int lineId = -1;

            if (node != null && node.Tag != null && node.Tag.GetType() == typeof(int))
            {
                id = (int)node.Tag;

                bool isProfileNode = node.Parent != null && node.Nodes != null && node.Nodes.Count > 0;
                if (!isProfileNode)
                {
                    lineId = id;
                    id = (int)node.Parent.Tag;
                }
            }

            return new Tuple<int, int>(id, lineId);
        }
    }
}
