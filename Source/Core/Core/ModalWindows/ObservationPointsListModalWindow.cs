using MilSpace.Core.DataAccess;
using MilSpace.Core.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace MilSpace.Core.ModalWindows
{
    public partial class ObservationPointsListModalWindow : Form
    {
        private List<FromLayerPointModel> _points = new List<FromLayerPointModel>();
        private Logger _log = Logger.GetLoggerEx("MilSpace.Core.ModalWindows.ObservationPointsListModalWindow");
        public FromLayerPointModel SelectedPoint;

        public ObservationPointsListModalWindow(List<FromLayerPointModel> points)
        {
            InitializeComponent();
            LocalizeStrings();
            _points = points;
            FillPointsGrid();
        }

        private void LocalizeStrings()
        {
            this.Text = LocalizationContext.Instance.FindLocalizedElement("ModalTargetObservPointsTitle", "Вибір точок з шару точок спостереження");
            btnChoosePoint.Text = LocalizationContext.Instance.ChooseText;
            dgvPoints.Columns["IdCol"].HeaderText = LocalizationContext.Instance.IdHeaderText;
            dgvPoints.Columns["TitleCol"].HeaderText = LocalizationContext.Instance.TitleHeaderText;
            lblLayer.Text = LocalizationContext.Instance.FindLocalizedElement("ObservPointsTypeText", "Пункти спостереження");
        }

        private void FillPointsGrid()
        {
            dgvPoints.Rows.Clear();

            foreach(var point in _points)
            {
                dgvPoints.Rows.Add(point.ObjId, point.DisplayedField, point.Point.X.ToFormattedString(), point.Point.Y.ToFormattedString());
            }
        }

        private void BtnChoosePoint_Click(object sender, EventArgs e)
        {
            if(dgvPoints.SelectedRows.Count > 0)
            {
                SelectedPoint = _points.First(p => p.ObjId == (int)dgvPoints.SelectedRows[0].Cells["IdCol"].Value);
            }
        }
    }
}
