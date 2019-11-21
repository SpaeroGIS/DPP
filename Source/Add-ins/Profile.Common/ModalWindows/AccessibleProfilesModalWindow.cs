using ESRI.ArcGIS.Geometry;
using MilSpace.DataAccess.DataTransfer;
using MilSpace.Profile.Localization;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace MilSpace.Profile.ModalWindows
{
    public partial class AccessibleProfilesModalWindow : Form
    {
        private AccessibleProfilesController _controller;

        internal List<ProfileSession> SelectedProfilesSets;

        public AccessibleProfilesModalWindow(List<ProfileSession> userSession, ISpatialReference spatialReference)
        {
            InitializeComponent();
            LocalizeControls();

            _controller = new AccessibleProfilesController(userSession, spatialReference);

            SetComponentsView();
            SetTextDefaultValues();
            InitializeListView(_controller.GetAllAccessibleProfilesSets());
        }

        private void SetComponentsView()
        {
            SetListView();
        }

        private void SetListView()
        {
            lvProfilesSets.Columns.Add(
                new ColumnHeader
                {
                    Name = "NameCol",
                    Width = (int)(lvProfilesSets.Width * 0.32),
                    Text = LocalizationConstants.ProfilesSetsNameColHeader
                });
            lvProfilesSets.Columns.Add(
                new ColumnHeader
                {
                    Name = "CreatorCol",
                    Width = (int)(lvProfilesSets.Width * 0.25),
                    Text = LocalizationConstants.ProfilesSetsCreatorColHeader
                });
            lvProfilesSets.Columns.Add(
                new ColumnHeader
                {
                    Name = "DateCol",
                    Width = (int)(lvProfilesSets.Width * 0.15),
                    Text = LocalizationConstants.ProfilesSetsDateColHeader
                });
            lvProfilesSets.Columns.Add(
                new ColumnHeader
                {
                    Name = "TypeCol",
                    Width = (int)(lvProfilesSets.Width * 0.12),
                    Text = LocalizationConstants.ProfilesSetsTypeColHeader
                });

            var occupiedSpace = 0;

            foreach(ColumnHeader column in lvProfilesSets.Columns)
            {
                occupiedSpace += (column.Width + 1);
            }

            var colSize = lvProfilesSets.Width - occupiedSpace - SystemInformation.VerticalScrollBarWidth;

            lvProfilesSets.Columns.Add(
                new ColumnHeader
                {
                    Name = "IsSharedCol",
                    Width = colSize,
                    Text = LocalizationConstants.ProfilesSetsSharedColHeader
                });

            lvProfilesSets.View = View.Details;
        }

        private void FillComboBox()
        {
            cmbGraphType.Items.Clear();

            var types = _controller.GetGraphDisplayTypes();

            cmbGraphType.Items.Add(LocalizationConstants.GraphTypeText);

            foreach(var type in types)
            {
                cmbGraphType.Items.Add(type.Value);
            }
        }

        private void SetTextDefaultValues()
        {
            txtName.Text = LocalizationConstants.NamePlaceholder;
            txtCreator.Text = LocalizationConstants.CreatorPlaceholder;

            txtCreator.ForeColor = Color.DimGray;
            txtName.ForeColor = Color.DimGray;

            FillComboBox();
            cmbGraphType.Text = cmbGraphType.Items[0].ToString();
        }

        private void SetDataDefaultValues(ProfileSession[] profilesSets)
        {
            fromDate.Value = _controller.GetMinDateTime(profilesSets);
            toDate.Value = _controller.GetMaxDateTime(profilesSets);
        }

        private void InitializeListView(ProfileSession[] profilesSets)
        {
            lvProfilesSets.Items.Clear();
            var types = _controller.GetGraphDisplayTypes();

            foreach(var set in profilesSets)
            {
                var newItem = new ListViewItem(set.SessionName);

                newItem.Tag = set;

                newItem.SubItems.Add(set.CreatedBy);
                newItem.SubItems.Add(set.CreatedOn.ToLongDateString());
                newItem.SubItems.Add(types[set.DefinitionType]);
                newItem.SubItems.Add(set.Shared ? "+" : "-");

                lvProfilesSets.Items.Add(newItem);
            }

            if (lvProfilesSets.Items.Count > 0)
            {
                SetDataDefaultValues(profilesSets);
            }
        }

        private ProfileSession[] GetSetsFromListView()
        {
            var sets = new List<ProfileSession>();

            foreach(ListViewItem item in lvProfilesSets.Items)
            {
                sets.Add((ProfileSession)item.Tag);
            }

            return sets.ToArray();
        }

        private string GetDefinitionName()
        {
            return String.Empty;
        }

        private void LocalizeControls()
        {
            this.Text = LocalizationConstants.ProfilesSetsTitle; 

            gbFilters.Text = LocalizationConstants.FiltersTitle;

            lblDateText.Text = LocalizationConstants.CreationDateText;
            lblFrom.Text = LocalizationConstants.FromText;
            lblTo.Text = LocalizationConstants.ToText;

            btnFilter.Text = LocalizationConstants.FilterText;
            btnOk.Text = LocalizationConstants.AddToSessionText;
            btnCancel.Text = LocalizationConstants.CloseText;
            btnReset.Text = LocalizationConstants.ResetText;
            Text = LocalizationConstants.SavedProfilesModalWindowText;
        }

        private void BtnFilter_Click(object sender, EventArgs e)
        {
            var allSetsInListView = _controller.GetAllAccessibleProfilesSets();
            var filteredFields = allSetsInListView;

            if(txtName.Text != LocalizationConstants.NamePlaceholder && !string.IsNullOrWhiteSpace(txtName.Text))
            {
                filteredFields = _controller.FilterByName(txtName.Text, filteredFields);
            }

            if(txtCreator.Text != LocalizationConstants.CreatorPlaceholder && !string.IsNullOrWhiteSpace(txtCreator.Text))
            {
                filteredFields = _controller.FilterByCreator(txtCreator.Text, filteredFields);
            }

            if(fromDate.Value != _controller.GetMinDateTime(allSetsInListView) || toDate.Value != _controller.GetMaxDateTime(allSetsInListView))
            {
                filteredFields = _controller.FilterByDate(fromDate.Value, toDate.Value, filteredFields);
            }

            if(cmbGraphType.SelectedItem.ToString() != LocalizationConstants.GraphTypeText)
            {
                var types = _controller.GetGraphTypes();

                filteredFields = _controller.FilterByType(types[cmbGraphType.SelectedItem.ToString()], filteredFields);
            }

            if (filteredFields != allSetsInListView)
            {
                InitializeListView(filteredFields);
            }
        }

        private void BtnReset_Click(object sender, EventArgs e)
        {
            InitializeListView(_controller.GetAllAccessibleProfilesSets());
            SetTextDefaultValues();
        }

        private void TxtName_Enter(object sender, EventArgs e)
        {
            ChangePlaceholderHandler(txtName, LocalizationConstants.NamePlaceholder, true);
        }

        private void ChangePlaceholderHandler(TextBox textBox, string placeHolderText, bool isEnter)
        {
            if(isEnter)
            {
                if (textBox.Text == placeHolderText)
                {
                    textBox.Text = string.Empty;
                    textBox.ForeColor = Color.Black;
                }
            }
            else
            {
                if (String.IsNullOrWhiteSpace(textBox.Text))
                {
                    textBox.Text = placeHolderText;
                    textBox.ForeColor = Color.DimGray;
                }
            }
        }

        private void TxtCreator_Enter(object sender, EventArgs e)
        {
            ChangePlaceholderHandler(txtCreator, LocalizationConstants.CreatorPlaceholder, true);
        }

        private void TxtName_Leave(object sender, EventArgs e)
        {
            ChangePlaceholderHandler(txtName, LocalizationConstants.NamePlaceholder, false);
        }

        private void TxtCreator_Leave(object sender, EventArgs e)
        {
            ChangePlaceholderHandler(txtCreator, LocalizationConstants.CreatorPlaceholder, false);
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            SelectedProfilesSets = new List<ProfileSession>(lvProfilesSets.Items.Count);

            foreach(ListViewItem selectedItem in lvProfilesSets.SelectedItems)
            {
                SelectedProfilesSets.Add((ProfileSession)selectedItem.Tag);
            }
        }
    }
}
