﻿using ESRI.ArcGIS.Carto;
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
        




        private static IActiveView ActiveView => ArcMap.Document.ActiveView;


       MapLayersManager manager = new MapLayersManager(ActiveView);
        



        public WindowMilSpaceMVisibilityMaster()
        {
            //this.ActiveView = ActiveView;
            InitializeComponent();
           
            controller.SetView(this);
            ONload();
        }
        public void ONload()
        {
            foreach (TabPage tab in StepsTabControl.TabPages)
            {
                tab.Enabled = false;
            }
            (StepsTabControl.TabPages[0] as TabPage).Enabled = true;
        }
        public void SecondTypePicked()
        {
           
           controller.UpdateObservationPointsList();

            PopulateComboBox();


            FillObservPointLabel();
            FillObsObj();
        }
        public void FirstTypePicked()
        {
            dvgCheckList.Rows.Clear();
            FillObservPointsOnCurrentView(controller.GetObservPointsOnCurrentMapExtent(ActiveView));
            //controller.GetObservPointsOnCurrentMapExtent(ActiveView);

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
            comboBox1.Items.AddRange(manager.RasterLayers.ToArray());
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

                }).ToList();
                dvgCheckList.Rows.Clear();
                dvgCheckList.CurrentCell = null;

                _observPointGuis = new BindingList<CheckObservPointGui>(ItemsToShow);
                dvgCheckList.DataSource = _observPointGuis;
                SetDataGridView();

                dvgCheckList.Update();
                dvgCheckList.Rows[0].Selected = true;

            }
        }
        public void FillObsObj()
        {
            try { 
                var temp = controller
                    .GetObservObjectsOnCurrentMapExtent(ActiveView).ToArray()
                    .Select(i => i.Title)
                   ;
            

                if(temp != null)
                {
                    checkedListBox2.Items.Add(temp);
                }
            }
            catch(ArgumentNullException)
            {
                checkedListBox2.Text = "no obser object added!";
            }
   
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

        private void FillPointsLayersComboBox()
        {
                //todo
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
                //if (!_isFieldsEnabled) EnableObservPointsControls();
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
        

         public string ObservationStationFeatureClass => label19.Text;
        public string ObservationPointsFeatureClass => ObservPointLabel.Text;


        public IEnumerable<string> GetTypes => throw new NotImplementedException();

        public IEnumerable<string> GetAffiliation => throw new NotImplementedException();

       

        public void AddRecord(ObservationPoint observationPoint)
        {
            throw new NotImplementedException();
        }
        public void ChangeRecord(int id, ObservationPoint observationPoint) => throw new NotImplementedException();
        
        private void NextStepButton_Click(object sender, EventArgs e)
        {
            
            if (StepsTabControl.SelectedIndex == StepsTabControl.TabCount - 1)
            {
                MessageBox.Show("Start calculation");
            }
            StepsTabControl.SelectedTab.Enabled = false;
            var nextTab = StepsTabControl.TabPages[StepsTabControl.SelectedIndex + 1] as TabPage;
            nextTab.Enabled = true;
            if (StepsTabControl.SelectedIndex < StepsTabControl.TabCount - 1) StepsTabControl.SelectedIndex++;

        }

        private void PreviousStepButton_Click(object sender, EventArgs e)
        {
            if (StepsTabControl.SelectedIndex != 0) StepsTabControl.SelectedIndex--;
            dvgCheckList.Rows.Clear();
            dvgCheckList.Columns.Clear();
        }

       
        private void ultraButton1_Click(object sender, EventArgs e)
        {
            SecondTypePicked();
            StepsTabControl.SelectedIndex++;
        }
        private void Button1_Click(object sender, EventArgs e)
        {
            FirstTypePicked();
            StepsTabControl.SelectedIndex++;
        }

        private void WindowMilSpaceMVisibilityMaster_Load(object sender, EventArgs e)
        {

        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
