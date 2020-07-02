using ESRI.ArcGIS.Geometry;
using MilSpace.Core.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace MilSpace.Core.ModalWindows
{
    public partial class ObservObjForFunModalWindow : Form
    {
        private List<FromLayerGeometry> _observObjects = new List<FromLayerGeometry>();
        private bool _isMultiSelect;
        public Dictionary<int, IGeometry> SelectedGeometries;

        public ObservObjForFunModalWindow(List<FromLayerGeometry> observObjects, bool isMultiSelect = true)
        {
            InitializeComponent();
            LocalizeStrings();
            _observObjects = observObjects;
            _isMultiSelect = isMultiSelect;
            FillPointsGrid();
        }

        private void LocalizeStrings()
        {
            this.Text = LocalizationContext.Instance.FindLocalizedElement("ModalTargetObservObjTitle", "Вибір об'єктів спостереження");
            btnChoosePoint.Text = LocalizationContext.Instance.ChooseText;
            dgvObjects.Columns["IdCol"].HeaderText = LocalizationContext.Instance.IdHeaderText;
            dgvObjects.Columns["TitleCol"].HeaderText = LocalizationContext.Instance.TitleHeaderText;
            lblLayer.Text = LocalizationContext.Instance.FindLocalizedElement("ObservObjectsTypeText", "Об'єкти спостереження");
        }

        private void FillPointsGrid()
        {
            dgvObjects.Rows.Clear();
            dgvObjects.Columns[0].Visible = _isMultiSelect;
            chckAllPoints.Visible = _isMultiSelect;

            foreach(var observObject in _observObjects)
            {
                dgvObjects.Rows.Add(false, observObject.ObjId, observObject.Title);
            }
        }

        private void ChckAllPoints_CheckedChanged(object sender, EventArgs e)
        {
            foreach(DataGridViewRow row in dgvObjects.Rows)
            {
                row.Cells[0].Value = chckAllPoints.Checked;
            }
        }

        private void BtnChoosePoint_Click(object sender, EventArgs e)
        {
            SelectPoints();
        }

        private void DgvObjects_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter && dgvObjects.SelectedRows.Count > 0)
            {
                SelectPoints();
                DialogResult = DialogResult.OK;
            }
        }

        private void DgvObjects_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (!_isMultiSelect)
            {
                SelectPoints();
                DialogResult = DialogResult.OK;
            }
        }

        private void SelectPoints()
        {
            SelectedGeometries = new Dictionary<int, IGeometry>();

            if (_isMultiSelect)
            {
                foreach (DataGridViewRow row in dgvObjects.Rows)
                {
                    if ((bool)row.Cells[0].Value)
                    {
                        var objId = (int)row.Cells["IdCol"].Value;
                        SelectedGeometries.Add(objId, _observObjects.First(observObject => observObject.ObjId == objId).Geometry);
                    }
                }
            }
            else
            {
                if (dgvObjects.SelectedRows.Count > 0)
                {
                    var objId = (int)dgvObjects.SelectedRows[0].Cells["IdCol"].Value;
                    SelectedGeometries.Add(objId, _observObjects.First(observObject => observObject.ObjId == objId).Geometry);
                }
            }
        }
    }
}
