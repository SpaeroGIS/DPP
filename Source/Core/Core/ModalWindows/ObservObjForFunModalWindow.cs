using ESRI.ArcGIS.Geometry;
using MilSpace.Core.DataAccess;
using MilSpace.Core.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace MilSpace.Core.ModalWindows
{
    public partial class ObservObjForFunModalWindow : Form
    {
        private List<FromLayerGeometry> _observObjects = new List<FromLayerGeometry>();
        public List<IGeometry> SelectedPoints;

        public ObservObjForFunModalWindow(List<FromLayerGeometry> observObjects)
        {
            InitializeComponent();
            LocalizeStrings();
            _observObjects = observObjects;
            FillPointsGrid();
        }

        private void LocalizeStrings()
        {
            this.Text = LocalizationContext.Instance.FindLocalizedElement("ModalTargetObservObjTitle", "Вибір об'єктів спостереження");
            btnChoosePoint.Text = LocalizationContext.Instance.ChooseText;
            dgvPoints.Columns["IdCol"].HeaderText = LocalizationContext.Instance.IdHeaderText;
            dgvPoints.Columns["TitleCol"].HeaderText = LocalizationContext.Instance.TitleHeaderText;
            lblLayer.Text = LocalizationContext.Instance.FindLocalizedElement("ObservObjectsTypeText", "Об'єкти спостереження");
        }

        private void FillPointsGrid()
        {
            dgvPoints.Rows.Clear();

            foreach(var observObject in _observObjects)
            {
                dgvPoints.Rows.Add(false, observObject.ObjId, observObject.Title);
            }
        }

        private void ChckAllPoints_CheckedChanged(object sender, EventArgs e)
        {
            foreach(DataGridViewRow row in dgvPoints.Rows)
            {
                row.Cells[0].Value = chckAllPoints.Checked;
            }
        }

        private void BtnChoosePoint_Click(object sender, EventArgs e)
        {
            SelectedPoints = new List<IGeometry>();

            foreach(DataGridViewRow row in dgvPoints.Rows)
            {
                if((bool)row.Cells[0].Value)
                {
                    SelectedPoints.Add(_observObjects.First(observObject => observObject.ObjId == (int)row.Cells["IdCol"].Value).Geometry);
                }
            }
        }
    }
}
