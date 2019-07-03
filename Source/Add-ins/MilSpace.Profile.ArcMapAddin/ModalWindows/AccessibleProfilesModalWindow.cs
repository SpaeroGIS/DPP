using ESRI.ArcGIS.Geometry;
using MilSpace.DataAccess.DataTransfer;
using MilSpace.Profile.Localization;
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
        internal ProfileSession[] SelectedProfilesSets;

        public AccessibleProfilesModalWindow(List<ProfileSession> userSession, ISpatialReference spatialReference)
        {
            InitializeComponent();
            LocalizeControls();

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
            lvProfilesSets.Columns.Add(new ColumnHeader { Name = "NameCol", Width = (int)(lvProfilesSets.Width * 0.3), Text = LocalizationConstants.ProfilesSetsNameColHeader });
            lvProfilesSets.Columns.Add(new ColumnHeader { Name = "CreatorCol", Width = (int)(lvProfilesSets.Width * 0.3), Text = LocalizationConstants.ProfilesSetsCreatorColHeader});
            lvProfilesSets.Columns.Add(new ColumnHeader { Name = "DateCol",  Width = (int)(lvProfilesSets.Width * 0.2), Text = LocalizationConstants.ProfilesSetsDateColHeader});
            lvProfilesSets.Columns.Add(new ColumnHeader { Name = "TypeCol", Width = (int)(lvProfilesSets.Width * 0.1), Text = LocalizationConstants.ProfilesSetsTypeColHeader});
            lvProfilesSets.Columns.Add(new ColumnHeader { Name = "IsSharedCol", Width = (int)(lvProfilesSets.Width * 0.1), Text = LocalizationConstants.ProfilesSetsSharedColHeader});

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

            foreach(var set in profilesSets)
            {
                var newItem = new ListViewItem(set.SessionName);

                newItem.SubItems.Add(set.CreatedBy);
                newItem.SubItems.Add(set.CreatedOn.ToLongDateString());
                //todo localize shared and type
                newItem.SubItems.Add(set.DefinitionType.ToString());
                newItem.SubItems.Add(set.Shared.ToString());

                lvProfilesSets.Items.Add(newItem);
            }
        }

        private string GetDefinitionName()
        {
            return String.Empty;
        }

        private void LocalizeControls()
        {
            gbFilters.Text = LocalizationConstants.FiltersTitle;

            txtName.Text = LocalizationConstants.NamePlaceholder;
            txtCreator.Text = LocalizationConstants.CreatorPlaceholder;

            cmbGraphType.Items.Add(LocalizationConstants.GraphTypeText);
            cmbGraphType.Items.Add(LocalizationConstants.PointsTypeText);
            cmbGraphType.Items.Add(LocalizationConstants.FunTypeText);
            cmbGraphType.Items.Add(LocalizationConstants.PrimitiveTypeText);
            //to default values
            cmbGraphType.Text = cmbGraphType.Items[0].ToString();

            lblDateText.Text = LocalizationConstants.CreationDateText;
            lblFrom.Text = LocalizationConstants.FromText;
            lblTo.Text = LocalizationConstants.ToText;

            btnFilter.Text = LocalizationConstants.FilterText;
            btnOk.Text = LocalizationConstants.OkText;
            btnCancel.Text = LocalizationConstants.CancelText;
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
