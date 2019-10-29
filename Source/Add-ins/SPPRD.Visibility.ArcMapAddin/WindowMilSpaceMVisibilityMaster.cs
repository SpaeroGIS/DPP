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

namespace MilSpace.Visibility
{
    public partial class WindowMilSpaceMVisibilityMaster : Form , IObservationPointsView
    {
        private const string _allValuesFilterText = "All";
        private ObservationPointsController controller = new ObservationPointsController(ArcMap.Document);
        private BindingList<CheckObservPointGui> _observPointGuis;

        private List<CheckObservPointGui> CheckedList = new List<CheckObservPointGui>();
        private List<CheckObservPointGui> CheckedObjectList = new List<CheckObservPointGui>();

        private MasterResult _finalResult = new MasterResult();



        private static IActiveView ActiveView => ArcMap.Document.ActiveView;


       MapLayersManager manager = new MapLayersManager(ActiveView);
        



        public WindowMilSpaceMVisibilityMaster()
        {
            InitializeComponent();
            controller.SetView(this);

            ONload();
        }

        public void ONload()//disable all tabs
        {
            foreach (TabPage tab in StepsTabControl.TabPages)
            {
                tab.Enabled = false;
            }
            (StepsTabControl.TabPages[0] as TabPage).Enabled = true;
        }

        public void SecondTypePicked()//triggers when user picks second type
        {
           
           controller.UpdateObservationPointsList();

           PopulateComboBox();


            FillObservPointLabel();
            FillObsObj();
        }
        public void FirstTypePicked()//triggers when user picks first type
        {
            controller.UpdateObservationPointsList();
            PopulateComboBox();
            FillObservPointLabel();
            FillObservPointsOnCurrentView(controller.GetObservPointsOnCurrentMapExtent(ActiveView));
            FillObsObj();

        }


        public void FillObservPointLabel()
        {
           var temp = controller.GetObservationPointsLayers(ActiveView).ToArray();
            //label слой ПН\ТН
            ObservPointLabel.Text = temp.FirstOrDefault();
            //label слой ОН
            label19.Text = controller.GetObservationStationsLayers().FirstOrDefault();
        }
        public void PopulateComboBox()
        {
            comboBox1.DataSource = null;
            comboBox1.DataSource = (manager.RasterLayers.Select(i=>i.Name).ToArray());
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
                    Date = t.Dto.Value.ToShortDateString(),
                    Id = t.Objectid

                });
                //Finding coincidence
                var commonT = (_observPointGuis.Select(a => a.Id).Intersect(ItemsToShow.Select(b => b.Id))).ToList();

                foreach(CheckObservPointGui e in _observPointGuis)
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
        public void FillObsObj()
        {
            try {

                var temp = controller
                    .GetObservObjectsOnCurrentMapExtent(ActiveView);

                var itemsToShow = temp.Select(t => new CheckObservPointGui
                {
                    Title = t.Title,
                    Affiliation = t.Group,
                    Id = t.ObjectId
                }).ToList();

                dgvObjects.DataSource = null; //Clearing listbox

                BindingList<CheckObservPointGui> _AllObjects = new BindingList<CheckObservPointGui>(itemsToShow);

                if (_AllObjects != null)
                {
                    dgvObjects.DataSource = _AllObjects;
                    SetDataGridView_For_Objects();
                }
            }
            catch(ArgumentNullException)
            {
                dgvObjects.Text = "no obser object added!";
            }
   
        }
        private void SetDataGridView_For_Objects()
        {
            dgvObjects.Columns["Title"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvObjects.Columns["Affiliation"].ReadOnly = true;
            dgvObjects.Columns["Affiliation"].Width = 150;
           dgvObjects.Columns["Id"].Visible = false;
            dgvObjects.Columns["Type"].Visible = false;
            dgvObjects.Columns["Date"].Visible = false;
        }


        private void SetDataGridView()
        {
            dvgCheckList.Columns["Date"].ReadOnly = true;
            dvgCheckList.Columns["Type"].ReadOnly = true;
            dvgCheckList.Columns["Affiliation"].ReadOnly = true;
            dvgCheckList.Columns["Title"].ReadOnly = true;
            dvgCheckList.Columns["Title"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dvgCheckList.Columns["Id"].Visible = false;
        }

        private void DisplaySelectedColumns(VeluableObservPointFieldsEnum filter)
        {
            dvgCheckList.Columns["Affiliation"].Visible = checkAffiliation.Checked;
            dvgCheckList.Columns["Type"].Visible = checkType.Checked;
            dvgCheckList.Columns["Date"].Visible = checkDate.Checked;
        }

        private void Filter_CheckedChanged(object sender, EventArgs e)
        {
            DisplaySelectedColumns(GetFilter);
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

        private void FilterData()
        {
            if (dvgCheckList.Rows.Count == 0)
            {
                return;
            }

            dvgCheckList.CurrentCell = null;

            foreach (DataGridViewRow row in dvgCheckList.Rows)
            {
                CheckRowForFilter(row);
            }

            if (dvgCheckList.FirstDisplayedScrollingRowIndex != -1)
            {
                dvgCheckList.Rows[dvgCheckList.FirstDisplayedScrollingRowIndex].Selected = true;
               
            }
    
        }

        private void CheckRowForFilter(DataGridViewRow row)
        {
            if (cmbAffiliation.SelectedItem != null && cmbAffiliation.SelectedItem.ToString() != _allValuesFilterText)
            {
                row.Visible = (row.Cells["Affiliation"].Value.ToString() == cmbAffiliation.SelectedItem.ToString());
                if (!row.Visible) return;
            }

            if (cmbType.SelectedItem != null && cmbType.SelectedItem.ToString() != _allValuesFilterText)
            {
                row.Visible = (row.Cells["Type"].Value.ToString() == cmbType.SelectedItem.ToString());
                return;
            }

            row.Visible = true;
        }
        private void cmbType_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterData();
        }

        private void cmbAffiliation_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterData();
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
          


            _finalResult.ObservPointID = CheckedList.Select(i => i.Id).ToList();
            _finalResult.ObservObjectID = CheckedObjectList.Select(i => i.Id).ToList();
            _finalResult.Table = TableChkBox.Checked;
            _finalResult.sumFieldOfView = SumChkBox.Checked;
            _finalResult.RasterLayerNAME = comboBox1.SelectedItem.ToString();
            _finalResult.OP = checkBoxOP.Checked;
            
        }
        public void ALLinfo()
        {
            if (checkBoxOP.Checked) {
                checkBox1.Visible = true;
            } else { checkBox1.Visible = false; }
            if (SumChkBox.Checked) { checkBox2.Visible = true; } else { checkBox2.Visible = false; }
            if (TableChkBox.Checked) { checkBox3.Visible = true; } else { checkBox3.Visible = false; }
            label27.Text = CheckedList.Count().ToString();
            label24.Text = comboBox1.SelectedItem.ToString();
            label28.Text = CheckedObjectList.Count().ToString();
            
        }
        
         public string ObservationStationFeatureClass => label19.Text;
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
                MessageBox.Show("Start calculation");
            }
           
           if (StepsTabControl.SelectedIndex < StepsTabControl.TabCount - 1)
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
        }
        private void ultraButton1_Click(object sender, EventArgs e)
        {
            SecondTypePicked();
            StepsTabControl.SelectedTab.Enabled = false;
            var nextTab = StepsTabControl.TabPages[StepsTabControl.SelectedIndex + 1] as TabPage;
            nextTab.Enabled = true;
            StepsTabControl.SelectedIndex++;
        }
        private void Button1_Click(object sender, EventArgs e)
        {
            FirstTypePicked();

            StepsTabControl.SelectedTab.Enabled = false;
            var nextTab = StepsTabControl.TabPages[StepsTabControl.SelectedIndex + 1] as TabPage;
            nextTab.Enabled = true;
            StepsTabControl.SelectedIndex++;
        }



        public IEnumerable<string> GetTypes => throw new NotImplementedException();

        public IEnumerable<string> GetAffiliation => throw new NotImplementedException();
    }
}
