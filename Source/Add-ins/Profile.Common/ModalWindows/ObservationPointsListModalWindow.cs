using MilSpace.Core;
using MilSpace.Core.DataAccess;
using MilSpace.Profile.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace MilSpace.Profile.ModalWindows
{
    public partial class ObservationPointsListModalWindow : Form
    {
        private List<FromLayerPointModel> _points = new List<FromLayerPointModel>();
        private Logger _log = Logger.GetLoggerEx("MilSpace.Profile.ModalWindows.ObservationPointsListModalWindow");
        internal FromLayerPointModel SelectedPoint;

        public ObservationPointsListModalWindow(List<FromLayerPointModel> points)
        {
            InitializeComponent();
            LocalizeStrings();
            _points = points;
            FillPointsGrid();
        }

        private void LocalizeStrings()
        {
            this.Text = LocalizationContext.Instance.FindLocalizedElement("ModalObservPointsTitle", "Вибір точки з шару точок спостереження");
            btnChoosePoint.Text = LocalizationContext.Instance.FindLocalizedElement("BtnChooseText", "Обрати");
            dgvPoints.Columns["IdCol"].HeaderText = LocalizationContext.Instance.FindLocalizedElement("DgvObservPointsIdHeader", "Ідентифікатор");
            dgvPoints.Columns["TitleCol"].HeaderText = LocalizationContext.Instance.FindLocalizedElement("DgvObservPointsTitleHeader", "Назва");
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
