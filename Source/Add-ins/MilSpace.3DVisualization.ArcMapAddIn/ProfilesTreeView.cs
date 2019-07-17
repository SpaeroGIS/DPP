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
    public partial class ProfilesTreeView : Form
    {
        private IList<TreeViewNodeModel> treeViewNodeModels;
        
        public ProfilesTreeView()
        {
            InitializeComponent();
        }

        public IList<TreeViewNodeModel> TreeViewNodeModels => treeViewNodeModels;


        public IList<TreeViewNodeModel> LoadProfiles()
        {
            var dataAccess = new DataAccess(new LocalizationContext());
            treeViewNodeModels = dataAccess.ParseUserProfileSessions();
            foreach (var node in treeViewNodeModels)
            {
                var newNode = UserSessionsProfilesTreeView.Nodes.Add(node.Name);
                if (node.ChildNodes == null || !node.ChildNodes.Any()) continue;

                newNode.Nodes.AddRange(node.ChildNodes.Select(item => new TreeNode(item.Name)).ToArray());                
            }
            return treeViewNodeModels;
        }
    }
}
