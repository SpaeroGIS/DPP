using ESRI.ArcGIS.Carto;
using MilSpace.DataAccess.DataTransfer;
using MilSpace.Visibility.DTO;
using MilSpace.Visibility.ViewController;
using MilSpace.Core.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using MilSpace.Tools;
using System.Drawing;
using MilSpace.Core;

namespace MilSpace.Visibility
{
    public partial class WindowMilSpaceMVisibilityMaster : Form, IObservationPointsView
    {
        private string _previousPickedRasterLayer;
        private int _stepControl = 0;
        private const string _allValuesFilterText = "All";
        private ObservationPointsController controller = new ObservationPointsController(ArcMap.Document);
        private BindingList<CheckObservPointGui> _observPointGuis;

        BindingList<CheckObservPointGui> _AllObjects = new BindingList<CheckObservPointGui>();


        private List<CheckObservPointGui> CheckedList = new List<CheckObservPointGui>();
        private List<CheckObservPointGui> CheckedObjectList = new List<CheckObservPointGui>();

        internal MasterResult FinalResult = new MasterResult();

        private static IActiveView ActiveView => ArcMap.Document.ActiveView;

        private MapLayersManager manager = new MapLayersManager(ActiveView);

        public WindowMilSpaceMVisibilityMaster(string selectedObservPoints, string selectedObservObjects, string previousPickedRaster)
        {
            InitializeComponent();
            controller.SetView(this);
            _previousPickedRasterLayer = previousPickedRaster;
            ONload();

            //label слой ПН\ТН
            ObservPointLabel.Text = selectedObservPoints;
            //label слой ОН
            observObjectsLabel.Text = selectedObservObjects;
        }


        public void ONload()
        {

            FillComboBoxes();
            foreach (TabPage tab in StepsTabControl.TabPages)//disable all tabs
            {
                tab.Enabled = false;
            }
            (StepsTabControl.TabPages[0] as TabPage).Enabled = true;

            panel1.Enabled = false;
        }
        public void DisabelObjList()
        {
            dgvObjects.Enabled = panel15.Enabled = panel16.Enabled = panel17.Enabled = panel18.Enabled = panel19.Enabled = false;
        }
        public void EanableObjList()
        {
            dgvObjects.Enabled = true;
            dgvObjects.Refresh();
            panel15.Enabled = panel16.Enabled = panel17.Enabled = panel18.Enabled = panel19.Enabled = true;

        }
        public void FirstTypePicked()//triggers when user picks first type
        {
            _stepControl = 1;
            controller.UpdateObservationPointsList();
            PopulateComboBox();
            FillObservPointLabel();
            DisabelObjList();
            dgvObjects.DataSource = null;
        }
        public void SecondTypePicked()//triggers when user picks second type
        {
            _stepControl = 2;
            controller.UpdateObservationPointsList();
            EanableObjList();
            PopulateComboBox();
            FillObservPointLabel();
            FillObsObj();
        }

        public void FillObservPointLabel()
        {
            var temp = controller.GetObservationPointsLayers(ActiveView).ToArray();

        }
        public void FillComboBoxes()
        {
            var list = new List<string>();
            list.AddRange(controller.GetObservationPointTypes().ToArray());
            cmbAffiliation.Items.Clear();

            cmbAffiliation.Items.Add(controller.GetAllAffiliationType());
            cmbAffiliation.Items.AddRange(list.ToArray());
            cmbAffiliation.SelectedItem = controller.GetAllAffiliationType();

            list = new List<string>();
            list.AddRange(controller.GetObservationObjectTypes().ToArray());
            cmbObservObject.Items.Clear();

            cmbObservObject.Items.AddRange(list.ToArray());
            cmbObservObject.Items.Add(controller.GetAllAffiliationType_for_objects());

            cmbObservObject.SelectedItem = _allValuesFilterText;

            list = new List<string>();
            cmbType.Items.Clear();
            list.AddRange(controller.GetObservationPointMobilityTypes().ToArray());
            cmbType.Items.AddRange(list.ToArray());
            cmbType.Items.Add(controller.GetAllMobilityType());
            cmbType.SelectedItem = controller.GetAllMobilityType();

            cmbMapLayers.Items.Clear();
            cmbMapLayers.Items.AddRange(controller.GetAllLayers().ToArray());
            cmbMapLayers.SelectedItem = controller.GetLastLayer();

            cmbPositions.Items.Clear();
            cmbPositions.Items.AddRange(controller.GetLayerPositions().ToArray());
            cmbPositions.SelectedItem = controller.GetDefaultLayerPosition();
        }
        public void PopulateComboBox()
        {
            comboBox1.DataSource = null;
            comboBox1.DataSource = (manager.RasterLayers.Select(i => i.Name).ToArray());

            if (_previousPickedRasterLayer != null)
            {
                comboBox1.SelectedItem = _previousPickedRasterLayer;
            }
        }

        public void FillObservationPointList(IEnumerable<ObservationPoint> observationPoints, VeluableObservPointFieldsEnum filter)
        {
            if (observationPoints != null && observationPoints.Any())
            {
                var ItemsToShow = observationPoints.Select(t => new CheckObservPointGui
                {
                    Title = t.Title,
                    Type = t.Type,
                    Affiliation = t.Affiliation,
                    Date = t.Dto.Value.ToShortDateString(),
                    Id = t.Objectid

                }).ToList();

                dvgCheckList.Rows.Clear();
                dvgCheckList.CurrentCell = null;

                _observPointGuis = new BindingList<CheckObservPointGui>(ItemsToShow);
                dvgCheckList.DataSource = _observPointGuis;
                SetDataGridView();

                dvgCheckList.Update();
                dvgCheckList.Rows[0].Selected = true;

            }
            else
            {

            }
        }
        public void FillObservPointsOnCurrentView(IEnumerable<ObservationPoint> observationPoints)
        {
            if (observationPoints != null && observationPoints.Any())
            {
                var ItemsToShow = observationPoints.Select(t => new CheckObservPointGui
                {
                    Title = t.Title,
                    Type = t.Type,
                    Affiliation = t.Affiliation,
                    Date = t.Dto.Value.ToString(Helper.DateFormatSmall),
                    Id = t.Objectid

                });
                //Finding coincidence
                var commonT = (_observPointGuis.Select(a => a.Id).Intersect(ItemsToShow.Select(b => b.Id))).ToList();

                foreach (CheckObservPointGui e in _observPointGuis)
                {
                    if (commonT.Contains(e.Id))
                    {
                        e.Check = true;
                    }
                }

                BindingList<CheckObservPointGui> temp = new BindingList<CheckObservPointGui>(_observPointGuis);

                dvgCheckList.CurrentCell = null;

                dvgCheckList.DataSource = temp;
                SetDataGridView();

                dvgCheckList.Update();

            }
        }
        public void FillObsObj(bool useCurrentExtent = false)
        {
            try
            {
                var All = controller.GetAllObservObjects();
                var itemsToShow = All.Select(t => new CheckObservPointGui
                {
                    Title = t.Title,
                    Affiliation = t.ObjectType.ToString(),
                    Id = t.ObjectId,
                    Type = t.Group,
                    Date = t.DTO.ToString(Helper.DateFormatSmall)
                }).ToList();

                dgvObjects.DataSource = null; //Clearing listbox

                _AllObjects = new BindingList<CheckObservPointGui>(itemsToShow);

                if (_AllObjects != null)
                {
                    dgvObjects.DataSource = _AllObjects;
                    SetDataGridView_For_Objects();
                }
                if (useCurrentExtent)
                {
                    var onCurrent = controller
                                .GetObservObjectsOnCurrentMapExtent(ActiveView);

                    var commonO = (itemsToShow.Select(a => a.Id).Intersect(onCurrent.Select(b => b.ObjectId))).ToList();

                    foreach (CheckObservPointGui e in itemsToShow)
                    {
                        if (commonO.Contains(e.Id))
                        {
                            e.Check = true;
                        }
                    }
                    dgvObjects.Refresh();
                }
            }
            catch (ArgumentNullException)
            {
                dgvObjects.Text = "Objects are not found";
            }

        }
        private void SetDataGridView_For_Objects()
        {
            try
            {
                dgvObjects.Columns["Check"].HeaderText = "";
                dgvObjects.Columns["Type"].HeaderText = "Group";//stands for "Afillation"

                dgvObjects.Columns["Title"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dgvObjects.Columns["Affiliation"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;//stands for "Group"
                dgvObjects.Columns["Type"].AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;//stands for "Afillation"
                dgvObjects.Columns["Date"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;

                dgvObjects.Columns["Check"].MinimumWidth = 25;

                dgvObjects.Columns["Type"].ReadOnly = true;
                dgvObjects.Columns["Title"].ReadOnly = true;
                dgvObjects.Columns["Affiliation"].ReadOnly = true;

                dgvObjects.Columns["Type"].Visible = true;//basically its an "Afillation" column
                dgvObjects.Columns["Id"].Visible = false;
                dgvObjects.Columns["Date"].Visible = true;
            }
            catch (NullReferenceException)
            {

            }
        }

        private void SetDataGridView()
        {
            try
            {
                dvgCheckList.Columns["Check"].HeaderText = "";
                dvgCheckList.Columns["Id"].Visible = false;

                dvgCheckList.Columns["Date"].ReadOnly = true;
                dvgCheckList.Columns["Type"].ReadOnly = true;
                dvgCheckList.Columns["Affiliation"].ReadOnly = true;
                dvgCheckList.Columns["Title"].ReadOnly = true;

                dvgCheckList.Columns["Affiliation"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
                dvgCheckList.Columns["Title"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dvgCheckList.Columns["Type"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;

                dvgCheckList.Columns["Affiliation"].MinimumWidth = 50;
                dvgCheckList.Columns["Title"].MinimumWidth = 50;
                dvgCheckList.Columns["Type"].MinimumWidth = 50;
                dvgCheckList.Columns["Date"].MinimumWidth = 50;
                dvgCheckList.Columns["Chck"].MinimumWidth = 25;

            }
            catch (NullReferenceException)
            {

            }
        }

        private void DisplaySelectedColumns_Points(DataGridView D)
        {
            try
            {
                D.Columns["Affiliation"].Visible = checkAffiliation.Checked;
                D.Columns["Type"].Visible = checkType.Checked;
                D.Columns["Date"].Visible = checkDate.Checked;
            }
            catch (NullReferenceException)
            {

            }
        }
        private void DisplaySelectedColumns_Objects(DataGridView D)
        {
            try
            {
                D.Columns["Affiliation"].Visible = checkB_Affilation.Checked;
                D.Columns["Date"].Visible = checkDate_Object.Checked;
            }
            catch (NullReferenceException)
            {

            }

        }

        private void Filter_CheckedChanged(object sender, EventArgs e)
        {
            DisplaySelectedColumns_Points(dvgCheckList);
        }

        private void Filter_For_Object_CheckedChanged(object sender, EventArgs e)
        {
            DisplaySelectedColumns_Objects(dgvObjects);
        }
        private void Select_All(object sender, EventArgs e)
        {

            foreach (CheckObservPointGui o in _AllObjects)
            {
                o.Check = checkBox4.Checked;
                dgvObjects.DataSource = _AllObjects;
                dgvObjects.Refresh();
            }

        }
        private void Select_All_Points(object sender, EventArgs e)
        {

            foreach (CheckObservPointGui o in _observPointGuis)
            {

                o.Check = checkBox6.Checked;
                dvgCheckList.DataSource = _observPointGuis;
                dvgCheckList.Refresh();
            }

        }
        public VeluableObservPointFieldsEnum GetFilter
        {
            get
            {
                var result = VeluableObservPointFieldsEnum.All;

                if (checkAffiliation.Checked)
                {
                    result = result | VeluableObservPointFieldsEnum.Affiliation;
                }
                if (checkDate.Checked)
                {
                    result = result | VeluableObservPointFieldsEnum.Date;
                }

                if (checkType.Checked)
                {
                    result = result | VeluableObservPointFieldsEnum.Type;
                }

                return result;
            }

        }

        private void FilterData(DataGridView Grid, ComboBox combo)
        {
            if (Grid.Rows.Count == 0)
            {
                return;
            }

            Grid.CurrentCell = null;

            foreach (DataGridViewRow row in Grid.Rows)
            {
                CheckRowForFilter(row, combo);
            }

            if (Grid.FirstDisplayedScrollingRowIndex != -1)
            {
                Grid.Rows[Grid.FirstDisplayedScrollingRowIndex].Selected = true;
            }
        }

        private void CheckRowForFilter(DataGridViewRow row, ComboBox combo)
        {
            if (combo.SelectedItem != null && combo.SelectedItem.ToString() != _allValuesFilterText)
            {
                row.Visible = (row.Cells["Affiliation"].Value.ToString() == combo.SelectedItem.ToString());
                if (!row.Visible) return;
            }

            if (row.Cells["Type"].Value != null && cmbType.SelectedItem != null && cmbType.SelectedItem.ToString() != _allValuesFilterText)
            {
                row.Visible = (row.Cells["Type"].Value.ToString() == cmbType.SelectedItem.ToString());
                return;
            }

            row.Visible = true;
        }
        private void cmbType_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterData(dvgCheckList, cmbAffiliation);
        }

        private void cmbAffiliation_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterData(dvgCheckList, cmbAffiliation);
        }
        private void cmbObservObject_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterData(dgvObjects, cmbObservObject);
        }

        public void AssemblMasterResult()
        {

            CheckedObjectList = new List<CheckObservPointGui>();
            CheckedList = new List<CheckObservPointGui>();
            foreach (DataGridViewRow row in dvgCheckList.Rows)
            {
                var PickedObSerPointRow = row.DataBoundItem as CheckObservPointGui;
                if (PickedObSerPointRow.Check)
                {
                    CheckedList.Add(PickedObSerPointRow);
                }
            }
            foreach (DataGridViewRow row in dgvObjects.Rows)
            {
                var PickedObSerPointRow = row.DataBoundItem as CheckObservPointGui;
                if (PickedObSerPointRow.Check)
                {
                    CheckedObjectList.Add(PickedObSerPointRow);
                }
            }

            VisibilityCalcTypeEnum calcType = (VisibilityCalcTypeEnum)_stepControl;

            if (_stepControl == 1)
            {
                FinalResult = new MasterResult
                {
                    ObservPointIDs = CheckedList.Select(i => i.Id).ToList(),
                    Table = TableChkBox.Checked,
                    SumFieldOfView = SumChkBox.Checked,
                    RasterLayerName = comboBox1.SelectedItem.ToString(),
                    VisibilityCalculationResults = SumChkBox.Checked ? VisibilityCalculationresultsEnum.ObservationPoints | VisibilityCalculationresultsEnum.VisibilityAreaRaster :
                        VisibilityCalculationresultsEnum.None,
                    RelativeLayerName = cmbMapLayers.SelectedItem.ToString(),
                    ResultLayerPosition = controller.GetPositionByStringValue(cmbPositions.SelectedItem.ToString()),
                    ResultLayerTransparency = Convert.ToInt16(tbTransparency.Text),
                    CalculationType = calcType
                };
            }
            else if (_stepControl == 2)
            {
                FinalResult = new MasterResult
                {
                    ObservPointIDs = CheckedList.Select(i => i.Id).ToList(),
                    ObservObjectIDs = CheckedObjectList.Select(i => i.Id).ToList(),
                    Table = TableChkBox.Checked,
                    SumFieldOfView = SumChkBox.Checked,
                    RasterLayerName = comboBox1.SelectedItem.ToString(),
                    RelativeLayerName = cmbMapLayers.SelectedItem.ToString(),
                    ResultLayerPosition = controller.GetPositionByStringValue(cmbPositions.SelectedItem.ToString()),
                    ResultLayerTransparency = Convert.ToInt16(tbTransparency.Text),
                    VisibilityCalculationResults = (SumChkBox.Checked ? 
                                                    VisibilityCalculationresultsEnum.ObservationPoints | VisibilityCalculationresultsEnum.VisibilityAreaRaster :
                                                    VisibilityCalculationresultsEnum.None)  
                                                    | VisibilityCalculationresultsEnum.ObservationStations | VisibilityCalculationresultsEnum.VisibilityObservStationClip,
                    CalculationType = calcType
                };
            }

            if (checkBoxOP.Checked)
            {
                FinalResult.VisibilityCalculationResults = FinalResult.VisibilityCalculationResults | VisibilityCalculationresultsEnum.VisibilityAreaRasterSingle | VisibilityCalculationresultsEnum.ObservationPointSingle;
            }

        }
        public void ALLinfo()
        {
            if (checkBoxOP.Checked) { labelOP.Visible = true; } else { labelOP.Visible = false; }
            if (SumChkBox.Checked) { labelOB.Visible = true; } else { labelOB.Visible = false; }
            if (TableChkBox.Checked) { labelT.Visible = true; } else { labelT.Visible = false; }

            label27.Text = CheckedList.Count().ToString();
            label24.Text = comboBox1.SelectedItem.ToString();
            label28.Text = CheckedObjectList.Count().ToString();

        }

        public string ObservationStationFeatureClass => observObjectsLabel.Text;
        public string ObservationPointsFeatureClass => ObservPointLabel.Text;

        public void AddRecord(ObservationPoint observationPoint)
        {
            throw new NotImplementedException();
        }
        public void ChangeRecord(int id, ObservationPoint observationPoint) => throw new NotImplementedException();

        private void NextStepButton_Click(object sender, EventArgs e)
        {
            if (StepsTabControl.SelectedIndex == 2)
            {
                AssemblMasterResult();
                ALLinfo();
            }

            if (StepsTabControl.SelectedIndex == StepsTabControl.TabCount - 1)
            {
                if (!FinalResult.VisibilityCalculationResults.HasFlag(VisibilityCalculationresultsEnum.ObservationPoints) && !FinalResult.VisibilityCalculationResults.HasFlag(VisibilityCalculationresultsEnum.ObservationPointSingle))
                {
                    //TODO: Localise
                    MessageBox.Show("The is no results sources for calculating. Please, select source!", "SPPRD", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }

                if (string.IsNullOrEmpty(comboBox1.Text))
                {
                    //TODO: Localise
                    MessageBox.Show("The Raster layer must be selected!", "SPPRD", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                DialogResult = DialogResult.OK;
                this.Close();

            }
            //StepsTabControl.SelectedTab.Enabled = false;
            if (StepsTabControl.TabPages.Count - 1 == StepsTabControl.SelectedIndex)
            {
                return;
            }

            if (StepsTabControl.SelectedIndex < StepsTabControl.TabCount - 1 && StepsTabControl.SelectedIndex != 0)
            {
                StepsTabControl.SelectedTab.Enabled = false;

                var nextTab = StepsTabControl
                    .TabPages[StepsTabControl.SelectedIndex + 1] as TabPage;

                nextTab.Enabled = true;
                StepsTabControl.SelectedIndex++;
            }

        }

        private void PreviousStepButton_Click(object sender, EventArgs e)
        {
            if (StepsTabControl.SelectedIndex != 0)
            {
                StepsTabControl.SelectedTab.Enabled = false;
                var prevTab = StepsTabControl.TabPages[StepsTabControl.SelectedIndex - 1] as TabPage;
                prevTab.Enabled = true;


                StepsTabControl.SelectedIndex--;
            }
            if (StepsTabControl.SelectedIndex == 0)
            {
                panel1.Enabled = false;
            }
        }
        private void ultraButton1_Click(object sender, EventArgs e)
        {
            SecondTypePicked();
            StepsTabControl.SelectedTab.Enabled = false;
            var nextTab = StepsTabControl.TabPages[StepsTabControl.SelectedIndex + 1] as TabPage;

            nextTab.Enabled = true;
            panel1.Enabled = true;

            StepsTabControl.SelectedIndex++;
        }
        private void Button1_Click(object sender, EventArgs e)
        {
            FirstTypePicked();

            StepsTabControl.SelectedTab.Enabled = false;
            var nextTab = StepsTabControl.TabPages[StepsTabControl.SelectedIndex + 1] as TabPage;

            nextTab.Enabled = true;
            panel1.Enabled = true;

            StepsTabControl.SelectedIndex++;
        }
        private void tabControl_Selecting(object sender, TabControlCancelEventArgs e)
        {
            TabPage current = (sender as TabControl).SelectedTab;
            var nextTab = StepsTabControl.TabPages[StepsTabControl.SelectedIndex] as TabPage;
            if (!nextTab.Enabled)
            {
                e.Cancel = true;
            }
        }


        public void FillVisibilitySessionsList(IEnumerable<VisibilitySession> visibilitySessions, bool isNewSessionAdded)
        {
            throw new NotImplementedException();
        }


        public IEnumerable<string> GetTypes => throw new NotImplementedException();

        public IEnumerable<string> GetAffiliation => throw new NotImplementedException();

        public void FillVisibilitySessionsList(IEnumerable<VisibilitySession> visibilitySessions)
        {
            throw new NotImplementedException();
        }

        public void FillObservationObjectsList(IEnumerable<ObservationObject> observationObjects)
        {
            throw new NotImplementedException();
        }

        private void TbTransparency_Leave(object sender, EventArgs e)
        {
            if (!Int16.TryParse(tbTransparency.Text, out short res) || (res < 0 || res > 100))
            {
                MessageBox.Show($"Invalid data.\nInsert the value in the range from 0 to 100");
                tbTransparency.Text = "33";
            }
        }
        public void FillVisibilitySessionsTree(IEnumerable<VisibilitySession> visibilitySessions, bool isNewSessionAdded)
        {
            throw new NotImplementedException();
        }
    }
}
