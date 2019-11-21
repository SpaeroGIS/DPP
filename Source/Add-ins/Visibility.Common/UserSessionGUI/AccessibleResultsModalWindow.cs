using ESRI.ArcGIS.Geometry;
using MilSpace.DataAccess.DataTransfer;
using MilSpace.Visibility.Localization;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using MilSpace.Core;

namespace MilSpace.Visibility
{
    public partial class AccessibleResultsModalWindow : Form
    {
        private AccessibleResultsController _controller;

        internal List<VisibilityCalcResults> SelectedResults;

        public AccessibleResultsModalWindow(IEnumerable<VisibilityCalcResults> userSession, ISpatialReference spatialReference)
        {
            InitializeComponent();
            LocalizeControls();

            _controller = new AccessibleResultsController(userSession.ToList(), spatialReference);

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
                    Text = "Name"
                });
            lvProfilesSets.Columns.Add(new ColumnHeader
            {
                Name = "CreatorCol",
                Width = (int)(lvProfilesSets.Width * 0.25),
                Text = "CreateBy"
            });
            lvProfilesSets.Columns.Add(new ColumnHeader
            {
                Name = "DateCol",
                Width = (int)(lvProfilesSets.Width * 0.15),
                Text = "Date"
            }
            );
            lvProfilesSets.Columns.Add(new ColumnHeader
            {
                Name = "TypeCol",
                Width = (int)(lvProfilesSets.Width * 0.12),
                Text = "Type"
            });

            var occupiedSpace = 0;

            foreach (ColumnHeader column in lvProfilesSets.Columns)
            {
                occupiedSpace += (column.Width + 1);
            }
            var colSize = lvProfilesSets.Width - occupiedSpace - SystemInformation.VerticalScrollBarWidth;
            lvProfilesSets.Columns.Add(new ColumnHeader
            {
                Name = "IsSharedCol",
                Width = colSize,
                Text = "Shared"
            });

            lvProfilesSets.View = View.Details;
        }

        private void FillComboBox()
        {
            cmbGraphType.Items.Clear();

            //TODO:Localize
            cmbGraphType.Items.Add("Усі типи");
            cmbGraphType.Items.AddRange(LocalizationContext.Instance.CalcTypeLocalisationShort.Values.ToArray());
            cmbGraphType.Items.Remove(LocalizationContext.Instance.CalcTypeLocalisationShort.First(type => type.Key == VisibilityCalcTypeEnum.None).Value);

        }

        private void SetTextDefaultValues()
        {
            //TODO: Localize
            //txtName.Text = LocalizationConstants.NamePlaceholder;
            //txtCreator.Text = LocalizationConstants.CreatorPlaceholder;

            txtCreator.ForeColor = Color.DimGray;
            txtName.ForeColor = Color.DimGray;

            FillComboBox();
            cmbGraphType.Text = cmbGraphType.Items[0].ToString();
        }

        private void SetDataDefaultValues(VisibilityCalcResults[] profilesSets)
        {
            fromDate.Value = _controller.GetMinDateTime(profilesSets);
            toDate.Value = _controller.GetMaxDateTime(profilesSets);
        }

        private void InitializeListView(VisibilityCalcResults[] profilesSets)
        {
            lvProfilesSets.Items.Clear();
            var types = LocalizationContext.Instance.CalcTypeLocalisationShort;

            foreach (var set in profilesSets)
            {
                var newItem = new ListViewItem(set.Name);

                newItem.Tag = set;

                newItem.SubItems.Add(set.UserName);
                newItem.SubItems.Add(set.Created.Value.ToString(Helper.DateFormatSmall));
                newItem.SubItems.Add(types[set.CalculationType]);
                newItem.SubItems.Add(set.Shared ? "+" : "-");

                lvProfilesSets.Items.Add(newItem);
            }

            if (lvProfilesSets.Items.Count > 0)
            {
                SetDataDefaultValues(profilesSets);
            }
        }

        private VisibilityCalcResults[] GetSetsFromListView()
        {
            var sets = new List<VisibilityCalcResults>();

            foreach (ListViewItem item in lvProfilesSets.Items)
            {
                sets.Add((VisibilityCalcResults)item.Tag);
            }

            return sets.ToArray();
        }

        private string GetDefinitionName()
        {
            return String.Empty;
        }

        private void LocalizeControls()
        {
            //this.Text = LocalizationConstants.ProfilesSetsTitle; 

            //gbFilters.Text = LocalizationConstants.FiltersTitle;

            //lblDateText.Text = LocalizationConstants.CreationDateText;
            //lblFrom.Text = LocalizationConstants.FromText;
            //lblTo.Text = LocalizationConstants.ToText;

            //btnFilter.Text = LocalizationConstants.FilterText;
            //btnOk.Text = LocalizationConstants.AddToSessionText;
            //btnCancel.Text = LocalizationConstants.CloseText;
            //btnReset.Text = LocalizationConstants.ResetText;
            //Text = LocalizationConstants.SavedProfilesModalWindowText;
        }

        private void BtnFilter_Click(object sender, EventArgs e)
        {
            //var allSetsInListView = _controller.GetAllAccessibleProfilesSets();
            //var filteredFields = allSetsInListView;

            //if(txtName.Text != LocalizationConstants.NamePlaceholder && !string.IsNullOrWhiteSpace(txtName.Text))
            //{
            //    filteredFields = _controller.FilterByName(txtName.Text, filteredFields);
            //}

            //if(txtCreator.Text != LocalizationConstants.CreatorPlaceholder && !string.IsNullOrWhiteSpace(txtCreator.Text))
            //{
            //    filteredFields = _controller.FilterByCreator(txtCreator.Text, filteredFields);
            //}

            //if(fromDate.Value != _controller.GetMinDateTime(allSetsInListView) || toDate.Value != _controller.GetMaxDateTime(allSetsInListView))
            //{
            //    filteredFields = _controller.FilterByDate(fromDate.Value, toDate.Value, filteredFields);
            //}

            //if(cmbGraphType.SelectedItem.ToString() != LocalizationConstants.GraphTypeText)
            //{
            //    var types = LocalizationContext.Instance.CalcTypeLocalisationShort; 

            //    filteredFields = _controller.FilterByType(types[cmbGraphType.SelectedItem.ToString()], filteredFields);
            //}

            //if (filteredFields != allSetsInListView)
            //{
            //    InitializeListView(filteredFields);
            //}
        }

        private void BtnReset_Click(object sender, EventArgs e)
        {
            InitializeListView(_controller.GetAllAccessibleProfilesSets());
            SetTextDefaultValues();
        }

        private void TxtName_Enter(object sender, EventArgs e)
        {
            //ChangePlaceholderHandler(txtName, LocalizationConstants.NamePlaceholder, true);
        }

        private void ChangePlaceholderHandler(TextBox textBox, string placeHolderText, bool isEnter)
        {
            if (isEnter)
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
            //TODO:Localize
            //ChangePlaceholderHandler(txtCreator, LocalizationConstants.CreatorPlaceholder, true);
        }

        private void TxtName_Leave(object sender, EventArgs e)
        {
            //TODO:Localize
            //ChangePlaceholderHandler(txtName, LocalizationConstants.NamePlaceholder, false);
        }

        private void TxtCreator_Leave(object sender, EventArgs e)
        {
            //TODO:Localize
            //ChangePlaceholderHandler(txtCreator, LocalizationConstants.CreatorPlaceholder, false);
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            SelectedResults = new List<VisibilityCalcResults>(lvProfilesSets.Items.Count);

            foreach (ListViewItem selectedItem in lvProfilesSets.SelectedItems)
            {
                SelectedResults.Add((VisibilityCalcResults)selectedItem.Tag);
            }
        }
    }
}
