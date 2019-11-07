using MilSpace.Core;
using MilSpace.DataAccess.DataTransfer;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MilSpace.Visualization3D
{
    public partial class VisibilitySessionsModalWindow : Form
    {
        private LocalizationContext _localizationContext;
        public List<VisibilitySession> SelectedVisibilitySessions { get; } = new List<VisibilitySession>();

        internal VisibilitySessionsModalWindow(LocalizationContext context)
        {
            InitializeComponent();
            _localizationContext = context;
            LoadSessions();
            SetListView();
        }

        private void SetListView()
        {
            lvVisibilitySessions.View = View.Details;
            lvVisibilitySessions.FullRowSelect = true;

            lvVisibilitySessions.Columns.Add("Name", -2);
            lvVisibilitySessions.Columns.Add("Date", -2);

            lvVisibilitySessions.HeaderStyle = ColumnHeaderStyle.None;
        }

        //private void LocalizeControls()
        //{
        //}

        private void LoadSessions()
        {
            lvVisibilitySessions.Items.Clear();

            var dataAccess = new DataAccess(_localizationContext);
            var sessions = dataAccess.GetUserVisibilitySessions();

            foreach(var session in sessions)
            {
                var newItem = new ListViewItem(session.Name);
                newItem.SubItems.Add(session.Created.Value.ToString(Helper.DateFormat));
                newItem.Tag = session;

                lvVisibilitySessions.Items.Add(newItem);
            }
        }

        private void GetAllSelectedSessions()
        {
            foreach(ListViewItem selectedItem in lvVisibilitySessions.SelectedItems)
            {
                SelectedVisibilitySessions.Add((VisibilitySession)selectedItem.Tag);
            }

            DialogResult = DialogResult.OK;
        }

        private void SessionsToolBar_ButtonClick(object sender, ToolBarButtonClickEventArgs e)
        {
            switch(e.Button.Name)
            {
                case "tlbbAdd":

                    GetAllSelectedSessions();

                    break;
            }
        }

        private void VisibilitySessionsModalWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter) GetAllSelectedSessions();
        }
    }
}
