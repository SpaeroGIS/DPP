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

        private string sNamePlaceholder = "Name";
        private string sCreatorPlaceholder = "Creator";

        internal List<VisibilityCalcResults> SelectedResults;

        public AccessibleResultsModalWindow(IEnumerable<VisibilityCalcResults> userSession, ISpatialReference spatialReference)
        {
            InitializeComponent();
            LocalizeControls();

            _controller = new AccessibleResultsController(userSession.ToList(), spatialReference);

            SetComponentsView();
            SetTextDefaultValues();
            InitializeListView(_controller.GetAllAccessibleVisibilitesSets());
        }
        private void LocalizeControls()
        {
            try
            {
                this.Text = LocalizationContext.Instance.FindLocalizedElement("WinDB.Caption", "SavedProfilesModalWindow");
                this.gbFilters.Text = LocalizationContext.Instance.FindLocalizedElement("WinDB.gbFilters.Text", "Filters");
                this.btnReset.Text = LocalizationContext.Instance.FindLocalizedElement("WinDB.btnReset.Text", "Reset");
                this.btnFilter.Text = LocalizationContext.Instance.FindLocalizedElement("WinDB.btnFilter.Text", "Filter");
                this.cmbCalcType.Text = LocalizationContext.Instance.FindLocalizedElement("WinDB.cmbGraphType.Text", "Type");
                this.lblTo.Text = LocalizationContext.Instance.FindLocalizedElement("WinDB.lblTo.Text", "to");
                this.lblFrom.Text = LocalizationContext.Instance.FindLocalizedElement("WinDB.lblFrom.Text", "from");
                this.lblDateText.Text = LocalizationContext.Instance.FindLocalizedElement("WinDB.lblDateText.Text", "Creation date:");
                this.txtName.Text = LocalizationContext.Instance.FindLocalizedElement("WinDB.txtName.Text", "Name");
                this.btnCancel.Text = LocalizationContext.Instance.FindLocalizedElement("WinDB.btnCancel.Text", "Close");
                this.btnOk.Text = LocalizationContext.Instance.FindLocalizedElement("WinDB.btnOk.Text", "Add to session");
            }
            catch
            {
                string sMsgText = LocalizationContext.Instance.FindLocalizedElement(
                    "MsgTextNoLocalizationXML",
                    "No Localization xml-file found or there is an error during loading/nVisibility window is not fully localized");
                MessageBox.Show(
                    sMsgText,
                    LocalizationContext.Instance.MsgBoxInfoHeader,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
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
                    Text = LocalizationContext.Instance.FindLocalizedElement("WinDB.ColumnName", "Name")
                });
            lvProfilesSets.Columns.Add(new ColumnHeader
            {
                Name = "CreatorCol",
                Width = (int)(lvProfilesSets.Width * 0.25),
                Text = LocalizationContext.Instance.FindLocalizedElement("WinDB.ColumnCreator", "CreateBy")
            });
            lvProfilesSets.Columns.Add(new ColumnHeader
            {
                Name = "DateCol",
                Width = (int)(lvProfilesSets.Width * 0.15),
                Text = LocalizationContext.Instance.FindLocalizedElement("WinDB.ColumnDate", "Date")
            }
            );
            lvProfilesSets.Columns.Add(new ColumnHeader
            {
                Name = "TypeCol",
                Width = (int)(lvProfilesSets.Width * 0.12),
                Text = LocalizationContext.Instance.FindLocalizedElement("WinDB.ColumnType", "Type")
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
                Text = LocalizationContext.Instance.FindLocalizedElement("WinDB.ColumnShared", "Shared")
            });

            lvProfilesSets.View = View.Details;
        }

        private void FillComboBox()
        {
            cmbCalcType.Items.Clear();

            cmbCalcType.Items.Add(LocalizationContext.Instance.FindLocalizedElement("WinDB.CalcTypeAll", "Усі типи"));
            //cmbGraphType.Items.AddRange(LocalizationContext.Instance.CalcTypeLocalisationShort.Values.ToArray());
            cmbCalcType.Items.AddRange(LocalizationContext.Instance.CalcTypeLocalisationShort
                .Where (k => k.Key != VisibilityCalcTypeEnum.None)
                .Select(v => v.Value)
                .ToArray());
            //cmbGraphType.Items.Remove(LocalizationContext.Instance.CalcTypeLocalisationShort.First(type => type.Key == VisibilityCalcTypeEnum.None).Value);
        }

        private void SetTextDefaultValues()
        {
            txtName.Text = LocalizationContext.Instance.FindLocalizedElement("WinDB.NamePlaceholder", sNamePlaceholder);
            txtName.ForeColor = Color.DimGray;

            txtCreator.Text = LocalizationContext.Instance.FindLocalizedElement("WinDB.CreatorPlaceholder", sCreatorPlaceholder);
            txtCreator.ForeColor = Color.DimGray;

            FillComboBox();
            cmbCalcType.Text = cmbCalcType.Items[0].ToString();
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

        private void BtnFilter_Click(object sender, EventArgs e)
        {
            var allSetsInListView = _controller.GetAllAccessibleVisibilitesSets();
            var filteredFields = allSetsInListView;

            if(txtName.Text != LocalizationContext.Instance.FindLocalizedElement("WinDB.NamePlaceholder", sNamePlaceholder)
                && !string.IsNullOrWhiteSpace(txtName.Text))
            {
                filteredFields = _controller.FilterByName(txtName.Text, filteredFields);
            }

            if(txtCreator.Text != LocalizationContext.Instance.FindLocalizedElement("WinDB.CreatorPlaceholder", sCreatorPlaceholder)
                && !string.IsNullOrWhiteSpace(txtCreator.Text))
            {
                filteredFields = _controller.FilterByCreator(txtCreator.Text, filteredFields);
            }

            if(fromDate.Value != _controller.GetMinDateTime(allSetsInListView) 
                || toDate.Value != _controller.GetMaxDateTime(allSetsInListView))
            {
                filteredFields = _controller.FilterByDate(fromDate.Value, toDate.Value, filteredFields);
            }

            //if(cmbGraphType.SelectedItem.ToString() != LocalizationConstants.GraphTypeText)
            //{
                //var types = LocalizationContext.Instance.CalcTypeLocalisationShort;
                //filteredFields = _controller.FilterByType(types[cmbCalcType.SelectedItem.ToString()], filteredFields);
            //}

            if (filteredFields != allSetsInListView)
            {
                InitializeListView(filteredFields);
            }
        }

        private void BtnReset_Click(object sender, EventArgs e)
        {
            InitializeListView(_controller.GetAllAccessibleVisibilitesSets());
            SetTextDefaultValues();
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

        private void TxtName_Enter(object sender, EventArgs e)
        {
            ChangePlaceholderHandler(
                txtName, 
                LocalizationContext.Instance.FindLocalizedElement("WinDB.NamePlaceholder", sNamePlaceholder),
                true);
        }
        private void TxtName_Leave(object sender, EventArgs e)
        {
            ChangePlaceholderHandler(
                txtName, 
                LocalizationContext.Instance.FindLocalizedElement("WinDB.NamePlaceholder", sNamePlaceholder),
                true);
        }

        private void TxtCreator_Enter(object sender, EventArgs e)
        {
            ChangePlaceholderHandler(
                txtCreator, 
                LocalizationContext.Instance.FindLocalizedElement("WinDB.CreatorPlaceholder", sCreatorPlaceholder),
                true);
        }

        private void TxtCreator_Leave(object sender, EventArgs e)
        {
            ChangePlaceholderHandler(
                txtCreator, 
                LocalizationContext.Instance.FindLocalizedElement("WinDB.CreatorPlaceholder", sCreatorPlaceholder),
                true);
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
