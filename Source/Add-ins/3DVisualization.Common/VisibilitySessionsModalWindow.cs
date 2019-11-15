using MilSpace.Core;
using MilSpace.DataAccess.DataTransfer;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MilSpace.Visualization3D
{
    public partial class VisibilitySessionsModalWindow : Form
    {
        private LocalizationContext _localizationContext;
        //private LocalizationContext context;

        public List<VisibilityTask> SelectedVisibilitySessions { get; } = new List<VisibilityTask>();

        internal VisibilitySessionsModalWindow(LocalizationContext context)
        {
            InitializeComponent();
            _localizationContext = context;
            LocalizeComponent();
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

        private void LocalizeComponent()
        {
            try
            {
                _localizationContext = new LocalizationContext();
                this.Text = _localizationContext.WindowCaptionVisibilites;
                this.labelHeaderSessionProfiles.Text = _localizationContext.labelHeaderSessionVisibilites;
                this.buttonAddProfilesTo3D.Text = _localizationContext.buttonAddProfilesTo3D;
            }
            catch
            {
                MessageBox.Show("No Localization.xml found or there is an error during loading", "Error Localization form");
            }
        }

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
                SelectedVisibilitySessions.Add((VisibilityTask)selectedItem.Tag);
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

        private void button1_Click(object sender, System.EventArgs e)
        {
            GetAllSelectedSessions();
        }
    }
}
