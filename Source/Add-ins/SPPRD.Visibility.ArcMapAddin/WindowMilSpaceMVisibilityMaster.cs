using ESRI.ArcGIS.Carto;
using MilSpace.DataAccess.DataTransfer;
using MilSpace.Visibility.DTO;
using MilSpace.Visibility.ViewController;
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
        private ObservationPointsController controller;
        private BindingList<ObservPointGui> _observPointGuis;
       

        public IActiveView ActiveView => ArcMap.Document.ActiveView;



        public WindowMilSpaceMVisibilityMaster()
        {
            InitializeComponent();
           
        }
        public void SecondTypePicked()
        {
           controller = new ObservationPointsController();
           controller.SetView(this);
           controller.UpdateObservationPointsList();

            //FillPointsLayersComboBox();
          //  PopulateComboBox(comboBox1, ProfileLayers.RasterLayers);
            FillObservPointLabel();
        }
        public void FillObservPointLabel()
        {
           var temp = controller.GetObservationPointsLayers(ActiveView).ToArray();
            
            ObservPointLabel.Text = temp.FirstOrDefault();
        }
        public void PopulateComboBox(ComboBox comboBox, IEnumerable<ILayer> layers)
        {
            comboBox.Items.AddRange(layers.Select(l => l.Name).ToArray());
        }

        public void FillObservationPointList(IEnumerable<ObservationPoint> observationPoints, VeluableObservPointFieldsEnum filter)
        {
            if (observationPoints.Any())
            {
                var ItemsToShow = observationPoints.Select(i => new ObservPointGui
                {
                    Title = i.Title,
                    Type = i.Type,
                    Affiliation = i.Affiliation,
                    Date = i.Dto.Value.ToShortDateString(),
                    Id = i.Objectid
                }).ToList();

                dvgCheckList.Rows.Clear();
                dvgCheckList.CurrentCell = null;
                _observPointGuis = new BindingList<ObservPointGui>(ItemsToShow);
                dvgCheckList.DataSource = _observPointGuis;

                SetDataGridView();
                DisplaySelectedColumns(filter);
                dvgCheckList.Update();
                dvgCheckList.Rows[0].Selected = true;
            }
        }

        private void SetDataGridView()
        {
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
        


        public string ObservationPointsFeatureClass => throw new NotImplementedException();
        public IEnumerable<string> GetTypes => throw new NotImplementedException();

        public IEnumerable<string> GetAffiliation => throw new NotImplementedException();

        public string ObservationStationFeatureClass => throw new NotImplementedException();

        public void AddRecord(ObservationPoint observationPoint)
        {
            throw new NotImplementedException();
        }
        public void ChangeRecord(int id, ObservationPoint observationPoint) => throw new NotImplementedException();
        
        private void NextStepButton_Click(object sender, EventArgs e)
        {
            if(StepsTabControl.SelectedIndex < StepsTabControl.TabCount - 1) StepsTabControl.SelectedIndex++; 
        }

        private void PreviousStepButton_Click(object sender, EventArgs e)
        {
            if (StepsTabControl.SelectedIndex != 0) StepsTabControl.SelectedIndex--;
        }

        //todo
        //private static IEnumerable<IRasterLayer> GetRasterLayers(ILayer layer)
        //{
        //    var result = new List<IRasterLayer>();


        //    if (layer is IRasterLayer fLayer)
        //    {
        //        result.Add(fLayer);
        //    }

        //    if (layer is ICompositeLayer cLayer)
        //    {

        //        for (int j = 0; j < cLayer.Count; j++)

        //        {
        //            if ((layer is IRasterLayer cRastreLayer))
        //            {
        //                result.Add(cRastreLayer);
        //            }
        //        }

        //    }

        //    return result;
        //}

        

        private void ultraButton1_Click(object sender, EventArgs e)
        {
            SecondTypePicked();
            StepsTabControl.SelectedIndex++;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
