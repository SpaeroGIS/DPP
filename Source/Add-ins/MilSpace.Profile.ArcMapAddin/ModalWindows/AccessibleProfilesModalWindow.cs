using ESRI.ArcGIS.Geometry;
using MilSpace.DataAccess.DataTransfer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MilSpace.Profile.ModalWindows
{
    public partial class AccessibleProfilesModalWindow : Form
    {
        private AccessibleProfilesController _controller;

        public AccessibleProfilesModalWindow(List<ProfileSession> userSession, ISpatialReference spatialReference)
        {
            InitializeComponent();

            _controller = new AccessibleProfilesController(userSession, spatialReference);

            SetComponentsView();
            SetProfilesSets(userSession);
            InitializeListView(_controller.GetAllAccessibleProfilesSets());
        }

        private void SetComponentsView()
        {
            SetListView();
        }

        private void SetListView()
        {
            lvProfilesSets.Columns.Add(new ColumnHeader { Name = "NameCol", Width = (int)(lvProfilesSets.Width * 0.3)});
            lvProfilesSets.Columns.Add(new ColumnHeader { Name = "CreatorCol", Width = (int)(lvProfilesSets.Width * 0.3)});
            lvProfilesSets.Columns.Add(new ColumnHeader { Name = "DateCol",  Width = (int)(lvProfilesSets.Width * 0.2)});
            lvProfilesSets.Columns.Add(new ColumnHeader { Name = "TypeCol", Width = (int)(lvProfilesSets.Width * 0.1)});
            lvProfilesSets.Columns.Add(new ColumnHeader { Name = "IsSharedCol", Width = (int)(lvProfilesSets.Width * 0.1)});

            lvProfilesSets.View = View.Details;

            //lvProfilesSets.Columns.Add("NameCol", (int)(lvProfilesSets.Width * 0.32));
            //lvProfilesSets.Columns.Add("CreatorCol", (lvProfilesSets.Width - lvProfilesSets.Columns[0].Width - 25));
            //lvProfilesSets.Columns.Add("DateCol", (lvProfilesSets.Width - lvProfilesSets.Columns[0].Width - 25));
            //lvProfilesSets.Columns.Add("TypeCol", (lvProfilesSets.Width - lvProfilesSets.Columns[0].Width - 25));
            //lvProfilesSets.Columns.Add("IsSharedCol", (lvProfilesSets.Width - lvProfilesSets.Columns[0].Width - 25));


            //lvProfilesSets.HeaderStyle = ColumnHeaderStyle.None;
        }

        private void SetProfilesSets(List<ProfileSession> userSession)
        {

        }

        private void InitializeListView(ProfileSession[] profilesSets)
        {
            lvProfilesSets.Items.Clear();
        }

        private ListViewItem CreateNewItem(string mainText, string subText)
        {
            var newItem = new ListViewItem(mainText);
            newItem.SubItems.Add(subText);

            return newItem;
        }

        private void SavedProfilesModalWindow_Load(object sender, EventArgs e)
        {

        }

        private void txtCreator_TextChanged(object sender, EventArgs e)
        {

        }

        private void filtersGroupBox_Enter(object sender, EventArgs e)
        {

        }
    }
}
