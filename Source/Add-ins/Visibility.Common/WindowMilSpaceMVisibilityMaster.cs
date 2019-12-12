using ESRI.ArcGIS.Carto;
using MilSpace.Core;
using MilSpace.Core.Tools;
using MilSpace.DataAccess.DataTransfer;
using MilSpace.Tools;
using MilSpace.Visibility.Localization;
using MilSpace.Visibility.ViewController;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace MilSpace.Visibility
{
    public partial class WindowMilSpaceMVisibilityMaster : Form, IObservationPointsView
    {
        private string _previousPickedRasterLayer;
        private VisibilityCalcTypeEnum _stepControl = VisibilityCalcTypeEnum.None;
        private const string _allValuesFilterText = "All";
        private ObservationPointsController controller = new ObservationPointsController(ArcMap.Document, ArcMap.ThisApplication);
        private BindingList<CheckObservPointGui> _observPointGuis;
        private BindingList<CheckObservPointGui> _observationObjects;
        internal WizardResult FinalResult = new WizardResult();

        private static IActiveView ActiveView => ArcMap.Document.ActiveView;

        private MapLayersManager manager = new MapLayersManager(ActiveView);

        public WindowMilSpaceMVisibilityMaster(string selectedObservPoints, string selectedObservObjects, string previousPickedRaster)
        {
            InitializeComponent();
            LocalizeComponent();
            controller.SetView(this);
            _previousPickedRasterLayer = previousPickedRaster;

            //label слой ПН\ТН
            ObservPointLabel.Text = selectedObservPoints;
            //label слой ОН
            observObjectsLabel.Text = selectedObservObjects;

            cmbEmptyDataValue.SelectedIndex = 1;

        }

        private void LocalizeComponent()
        {
            try
            {
                this.Text = LocalizationContext.Instance.WindowCaptionMaster;
                this.PreviousStepButton.Text = 
                    LocalizationContext.Instance.FindLocalizedElement("this.PreviousStepButton.Text", "< Попередній");
                this.NextStepButton.Text = 
                    LocalizationContext.Instance.FindLocalizedElement("WinM.NextStepButton.Text", "Наступний >");
                this.stepOne.Text = LocalizationContext.Instance.FindLocalizedElement("WinM.stepOne.Text", "Крок 1");
                this.label50.Text = 
                    LocalizationContext.Instance.FindLocalizedElement("WinM.label50.Text", "Аналіз результатів спостереження \r\nАналіз результатів спостереження з відомими параметрами (наприклад - аерофотозйомки) з точки зору розташування кадрів на місцевості і забезпечення видимості");
                this.button4.Text = LocalizationContext.Instance.FindLocalizedElement("WinM.button4.Text", "Обрати >");
                this.label45.Text = LocalizationContext.Instance.FindLocalizedElement("WinM.label45.Text", "4 (VP)");
                this.label49.Text = 
                    LocalizationContext.Instance.FindLocalizedElement("WinM.label49.Text", "Визначення параметрів пунктів спостереження (ПC) \r\nПошук параметрів ПC для спостереження заданих об\'ектів спостереження (ОC) (параметри апаратури, висота над поверхнею, напрямки і кути огляду)");
                this.button2.Text = LocalizationContext.Instance.FindLocalizedElement("WinM.button2.Text", "Обрати >");
                this.label33.Text = LocalizationContext.Instance.FindLocalizedElement("WinM.label33.Text", "3 (VO)");
                this.label48.Text =
                    LocalizationContext.Instance.FindLocalizedElement("WinM.label48.Text", "Визначення видимості в заданих ОC. \r\nПроводиться для обраних пунктів спостереження (ПС) та об\'єктів нагляду, з урахуванням індивідуальних параметрів ПС");
                this.uButton2.Text = LocalizationContext.Instance.FindLocalizedElement("WinM.uButton2.Text", "Обрати >");
                this.label32.Text = LocalizationContext.Instance.FindLocalizedElement("WinM.label32.Text", "2 (VA)");
                this.label47.Text =
                    LocalizationContext.Instance.FindLocalizedElement("WinM.label47.Text", "Визначення областей видимості на обраної поверхні в цілому.\r\nРозрахунок проводиться для заданих характеристик обраних пунктів спостереження. \r\nОцінка результату проводиться для вказаной поверхні.");
                this.button1.Text = LocalizationContext.Instance.FindLocalizedElement("WinM.button1.Text", "Обрати >");
                this.label31.Text = LocalizationContext.Instance.FindLocalizedElement("WinM.label31.Text", "1 (VS)");
                this.label46.Text = LocalizationContext.Instance.FindLocalizedElement("WinM.label46.Text", "Крок 1. Обрати тип розрахунку");
                this.stepTwo.Text = LocalizationContext.Instance.FindLocalizedElement("WinM.stepTwo.Text", "Крок 2");
                this.checkDate.Text = LocalizationContext.Instance.FindLocalizedElement("WinM.checkDate.Text", "дата");
                this.checkAffiliation.Text = LocalizationContext.Instance.FindLocalizedElement("WinM.checkAffiliation.Text", "належність");
                this.checkType.Text = LocalizationContext.Instance.FindLocalizedElement("WinM.checkType.Text", "тип");
                this.label1.Text = LocalizationContext.Instance.FindLocalizedElement("WinM.label1.Text", "відображати");
                this.label3.Text = LocalizationContext.Instance.FindLocalizedElement("WinM.label3.Text", "належність");
                this.label2.Text = LocalizationContext.Instance.FindLocalizedElement("WinM.label2.Text", "тип");
                this.ObservPointLabel.Text = LocalizationContext.Instance.FindLocalizedElement("WinM.ObservPointLabel.Text", "ObStations_201810");
                this.label5.Text = LocalizationContext.Instance.FindLocalizedElement("WinM.label5.Text", "Пункти спостереження (ПС)");
                this.checkDate_Object.Text = LocalizationContext.Instance.FindLocalizedElement("WinM.checkDate_Object.Text", "дата");
                this.checkB_Affilation.Text = LocalizationContext.Instance.FindLocalizedElement("WinM.checkB_Affilation.Text", "належність");
                this.label4.Text = LocalizationContext.Instance.FindLocalizedElement("WinM.label4.Text", "відображати");
                this.label9.Text = LocalizationContext.Instance.FindLocalizedElement("WinM.label9.Text", "належність");
                this.label6.Text = LocalizationContext.Instance.FindLocalizedElement("WinM.label6.Text", "Об\'єкти нагляду (ОН)");
                this.label22.Text = LocalizationContext.Instance.FindLocalizedElement("WinM.label22.Text", "Поверхня для розпрахунку");
                this.label30.Text = LocalizationContext.Instance.FindLocalizedElement("WinM.label30.Text", "Крок 2. Основні параметри розрахунку");
                this.stepThree.Text = LocalizationContext.Instance.FindLocalizedElement("WinM.stepThree.Text", "Крок 3");
                this.checkBox7.Text = LocalizationContext.Instance.FindLocalizedElement("WinM.checkBox7.Text", "розрахувати \"пірамиду\"");
                this.label20.Text = LocalizationContext.Instance.FindLocalizedElement("WinM.label20.Text", "значення для відсутності видимості");
                this.label21.Text = LocalizationContext.Instance.FindLocalizedElement("WinM.label21.Text", "Вигляд растрів");
                this.label18.Text = LocalizationContext.Instance.FindLocalizedElement("WinM.label18.Text", "прозорість (%)");
                this.label16.Text = LocalizationContext.Instance.FindLocalizedElement("WinM.label16.Text", "колір ОВ");
                this.label17.Text = LocalizationContext.Instance.FindLocalizedElement("WinM.label17.Text", "Оформлення");
                this.chkTrimRaster.Text = LocalizationContext.Instance.FindLocalizedElement("WinM.chkTrimRaster.Text", "обрізати растри до дійсного екстенту");
                this.label13.Text = LocalizationContext.Instance.FindLocalizedElement("WinM.label13.Text", "Додаткова обробка");
                this.label14.Text = LocalizationContext.Instance.FindLocalizedElement("WinM.label14.Text", "відносно шару");
                this.label12.Text = LocalizationContext.Instance.FindLocalizedElement("WinM.label12.Text", "Розташувати у карті");
                this.TableChkBox.Text = LocalizationContext.Instance.FindLocalizedElement("WinM.TableChkBox.Text", "таблиця покриття");
                this.chkConvertToPolygon.Text = LocalizationContext.Instance.FindLocalizedElement("WinM.chkConvertToPolygon.Text", "конвертувати у полігони");
                this.SumChkBox.Text = LocalizationContext.Instance.FindLocalizedElement("WinM.SumChkBox.Text", "загальна видимость");
                this.checkBoxOP.Text = LocalizationContext.Instance.FindLocalizedElement("WinM.checkBoxOP.Text", "видимость для окремих ПС");
                this.label11.Text = LocalizationContext.Instance.FindLocalizedElement("WinM.label11.Text", "Склад результату");
                this.label10.Text = LocalizationContext.Instance.FindLocalizedElement("WinM.label10.Text", "Крок 3. Додаткові параметри формування результату");
                this.stepFour.Text = LocalizationContext.Instance.FindLocalizedElement("WinM.stepFour.Text", "Крок 4");
                this.label42.Text = LocalizationContext.Instance.FindLocalizedElement("WinM.label42.Text", "Обрізати розраховані поверхні");
                this.label40.Text = LocalizationContext.Instance.FindLocalizedElement("WinM.label40.Text", "Розташувати відносно шару");
                this.labelOB.Text = LocalizationContext.Instance.FindLocalizedElement("WinM.labelOB.Text", "Розрахувати загальну ОВ");
                this.labelOP.Text = LocalizationContext.Instance.FindLocalizedElement("WinM.labelOP.Text", "Розрахувати поверхні всіх ПН");
                this.label26.Text = LocalizationContext.Instance.FindLocalizedElement("WinM.label26.Text", "Обрані області нагляду (ОН)");
                this.label25.Text = LocalizationContext.Instance.FindLocalizedElement("WinM.label25.Text", "Обрані пункти спостереження (ПН)");
                this.label23.Text = LocalizationContext.Instance.FindLocalizedElement("WinM.label23.Text", "Поверхня розрахунку");
                this.label44.Text = LocalizationContext.Instance.FindLocalizedElement("WinM.label44.Text", "Назва результату розрахунку");
                this.label37.Text = LocalizationContext.Instance.FindLocalizedElement("WinM.label37.Text", "Тип  розрахунку");
                this.label29.Text = LocalizationContext.Instance.FindLocalizedElement("WinM.label29.Text", "Крок 4. Перевірка параметрів розрахунку");
            }
            catch
            {
                string sMsgText = LocalizationContext.Instance.FindLocalizedElement(
                    "MsgTextNoLocalizationXML",
                    "No Localization xml-file found or there is an error during loading/nVisibility window is not fully localized");
                MessageBox.Show(
                    sMsgText,
                    LocalizationContext.Instance.MsgBoxInfoHeader,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            FillComboBoxes();

            //disable all tabs
            foreach (TabPage tab in StepsTabControl.TabPages)
            {
                tab.Enabled = false;
            }
            (StepsTabControl.TabPages[0] as TabPage).Enabled = true;

            panel1.Enabled = false;
        }

        public void DisabelObjList()
        {
            dgvObjects.Enabled = false;
            splitContainer1.Panel2.Enabled = false;
        }
        public void EanableObjList()
        {
            dgvObjects.Enabled = true;
            dgvObjects.Refresh();
            splitContainer1.Panel2.Enabled = true;

        }
        public void FirstTypePicked()//triggers when user picks first type
        {
            _stepControl = VisibilityCalcTypeEnum.OpservationPoints;
            controller.UpdateObservationPointsList();
            PopulateComboBox();
            FillObservPointLabel();
            DisabelObjList();
            FillObservPointsOnCurrentView(controller.GetObservPointsOnCurrentMapExtent(ActiveView));
            dgvObjects.DataSource = null;
        }
        public void SecondTypePicked()//triggers when user picks second type
        {
            _stepControl = VisibilityCalcTypeEnum.ObservationObjects;
            controller.UpdateObservationPointsList();
            EanableObjList();
            PopulateComboBox();
            FillObservPointLabel();
            FillObsObj(true);
            FillObservPointsOnCurrentView(controller.GetObservPointsOnCurrentMapExtent(ActiveView));
        }

        public void FillObservPointLabel()
        {
            var temp = controller.GetObservationPointsLayers(ActiveView).ToArray();

        }
        public void FillComboBoxes()
        {
            cmbAffiliation.Items.Clear();
            cmbAffiliation.Items.Add(controller.GetAllAffiliationType());
            cmbAffiliation.Items.AddRange(controller.GetObservationPointTypes().ToArray());
            cmbAffiliation.SelectedItem = controller.GetAllAffiliationType();

            cmbObservObject.Items.Clear();

            cmbObservObject.Items.AddRange(controller.GetObservationObjectTypes().ToArray());
            cmbObservObject.Items.Add(controller.GetAllAffiliationType_for_objects());

            cmbObservObject.SelectedItem = _allValuesFilterText;

            cmbType.Items.Clear();
            cmbType.Items.AddRange(controller.GetObservationPointMobilityTypes().ToArray());
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
            imagesComboBox.DataSource = null;
            imagesComboBox.DataSource = (manager.RasterLayers.Select(i => i.Name).ToArray());

            if (_previousPickedRasterLayer != null)
            {
                imagesComboBox.SelectedItem = _previousPickedRasterLayer;
            }
        }

        public void FillObservationPointList(IEnumerable<ObservationPoint> observationPoints, ValuableObservPointFieldsEnum filter)
        {
            if (observationPoints != null && observationPoints.Any())
            {
                var ItemsToShow = observationPoints.Select(
                    t => new CheckObservPointGui
                    {
                        Title = t.Title,
                        Type = t.Type,
                        Affiliation = t.Affiliation,
                        Date = t.Dto.Value.ToShortDateString(),
                        Id = t.Objectid
                    }).OrderBy(l => l.Title).ToList();

                dvgCheckList.Rows.Clear();
                dvgCheckList.CurrentCell = null;

                _observPointGuis = new BindingList<CheckObservPointGui>(ItemsToShow);
                dvgCheckList.DataSource = _observPointGuis;

                SetDataGridView();

                dvgCheckList.Update();
                dvgCheckList.Rows[0].Selected = true;
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
                var itemsToShow = All.Select(
                    t => new CheckObservPointGui
                    {
                        Title = t.Title,
                        Affiliation = t.ObjectType.ToString(),
                        Id = t.ObjectId,
                        Type = t.Group,
                        Date = t.DTO.ToString(Helper.DateFormatSmall)
                    }).OrderBy(l => l.Title);

                dgvObjects.DataSource = null; //Clearing listbox

                _observationObjects = new BindingList<CheckObservPointGui>(itemsToShow.ToArray());

                if (_observationObjects != null)
                {
                    dgvObjects.DataSource = _observationObjects;
                    SetDataGridView_For_Objects();
                }
                if (useCurrentExtent)
                {
                    var onCurrent = controller.GetObservObjectsOnCurrentMapExtent(ActiveView);
                    var commonO = (_observationObjects.Select(a => a.Id).Intersect(onCurrent.Select(b => b.ObjectId)));
                    foreach (CheckObservPointGui e in _observationObjects)
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

            foreach (CheckObservPointGui o in _observationObjects)
            {
                o.Check = checkBox4.Checked;
            }

            dgvObjects.DataSource = _observationObjects;
            dgvObjects.Refresh();

        }
        private void Select_All_Points(object sender, EventArgs e)
        {

            foreach (CheckObservPointGui o in _observPointGuis)
            {
                o.Check = checkBox6.Checked;
            }

            dvgCheckList.DataSource = _observPointGuis;
            dvgCheckList.Refresh();
        }
        public ValuableObservPointFieldsEnum GetFilter
        {
            get
            {
                var result = ValuableObservPointFieldsEnum.All;

                if (checkAffiliation.Checked)
                {
                    result = result | ValuableObservPointFieldsEnum.Affiliation;
                }
                if (checkDate.Checked)
                {
                    result = result | ValuableObservPointFieldsEnum.Date;
                }

                if (checkType.Checked)
                {
                    result = result | ValuableObservPointFieldsEnum.Type;
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

        private void AssemblyWizardResult()
        {

            FinalResult = new WizardResult
            {
                ObservPointIDs = _observPointGuis.Where(p => p.Check).Select(i => i.Id).ToArray(),
                Table = TableChkBox.Checked,
                SumFieldOfView = SumChkBox.Checked,
                RasterLayerName = imagesComboBox.SelectedItem.ToString(),
                RelativeLayerName = cmbMapLayers.SelectedItem.ToString(),
                ResultLayerPosition = controller.GetPositionByStringValue(cmbPositions.SelectedItem.ToString()),
                ResultLayerTransparency = Convert.ToInt16(tbTransparency.Text),
                CalculationType = _stepControl,
                TaskName = VisibilityManager.GenerateResultId(LocalizationContext.Instance.CalcTypeLocalisationShort[_stepControl])
            };

            if (_stepControl == VisibilityCalcTypeEnum.OpservationPoints)
            {
                FinalResult.VisibilityCalculationResults = 
                    SumChkBox.Checked ?
                    VisibilityCalculationResultsEnum.ObservationPoints 
                    | VisibilityCalculationResultsEnum.VisibilityAreaRaster 
                    | VisibilityCalculationResultsEnum.VisibilityAreasPotential /*| VisibilityCalculationResultsEnum.CoverageTable*/ 
                    : VisibilityCalculationResultsEnum.None;
            }
            else if (_stepControl == VisibilityCalcTypeEnum.ObservationObjects)
            {
                FinalResult.ObservObjectIDs = _observationObjects.Where(o => o.Check).Select(i => i.Id).ToArray();
                FinalResult.VisibilityCalculationResults = 
                    (SumChkBox.Checked ?
                    VisibilityCalculationResultsEnum.ObservationPoints 
                    | VisibilityCalculationResultsEnum.VisibilityAreaRaster 
                    | VisibilityCalculationResultsEnum.VisibilityAreasPotential /*| VisibilityCalculationResultsEnum.CoverageTable*/ 
                    : VisibilityCalculationResultsEnum.None)
                    | VisibilityCalculationResultsEnum.ObservationObjects 
                    | VisibilityCalculationResultsEnum.VisibilityObservStationClip;
            }

            //Trim by real Area
            if (chkTrimRaster.Checked)
            {
                FinalResult.VisibilityCalculationResults |= VisibilityCalculationResultsEnum.VisibilityAreasTrimmedByPoly;
                if (checkBoxOP.Checked)
                {
                    FinalResult.VisibilityCalculationResults |= VisibilityCalculationResultsEnum.VisibilityAreaTrimmedByPolySingle;
                }
            }

            if (chkConvertToPolygon.Checked)
            {
                FinalResult.VisibilityCalculationResults |= VisibilityCalculationResultsEnum.VisibilityAreaPolygons;
                if (checkBoxOP.Checked)
                {
                    FinalResult.VisibilityCalculationResults |= VisibilityCalculationResultsEnum.VisibilityAreaPolygonSingle;
                }
            }

            if (checkBoxOP.Checked && (_observPointGuis != null && _observPointGuis.Count(p => p.Check) > 1))
            {
                FinalResult.VisibilityCalculationResults |= VisibilityCalculationResultsEnum.VisibilityAreaRasterSingle | VisibilityCalculationResultsEnum.ObservationPointSingle | VisibilityCalculationResultsEnum.VisibilityAreaPotentialSingle;
            }
        }

        public void SummaryInfo()
        {
            lblCalcType.Text = 
                LocalizationContext.Instance.CalcTypeLocalisation[FinalResult.CalculationType];
            lblTaskName.Text = 
                FinalResult.TaskName;
            lblDEMName.Text = 
                FinalResult.RasterLayerName;
            lblObservObjectsSingle.Text = 
                checkBoxOP.Checked ? LocalizationContext.Instance.YesWord : LocalizationContext.Instance.NoWord;

            lblObservObjectsAll.Text = 
                SumChkBox.Checked ? LocalizationContext.Instance.YesWord : LocalizationContext.Instance.NoWord;

            lblReferencedLayerName.Text = 
                cmbPositions.SelectedItem.ToString() + " " + cmbMapLayers.SelectedItem.ToString();

            var selectedObservPoints = 
                _observPointGuis != null ? _observPointGuis.Where(p => p.Check).Select(o => o.Title).ToArray() : null;
            var selectedObservObjects = 
                _observationObjects != null ? _observationObjects.Where(p => p.Check).Select(o => o.Title).ToArray() : null;

            lblObservPointsSummary.Text = 
                selectedObservPoints == null ? string.Empty : $"{selectedObservPoints.Length} - {string.Join("; ", selectedObservPoints)}";
            lblObservObjectsSummary.Text = 
                selectedObservObjects == null ? string.Empty : $"{selectedObservObjects.Length} - {string.Join("; ", selectedObservObjects)}";

            lblTrimCalcresults.Text = 
                !chkTrimRaster.Enabled ? string.Empty : (chkTrimRaster.Checked ? LocalizationContext.Instance.YesWord : LocalizationContext.Instance.NoWord);
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
                AssemblyWizardResult();
                SummaryInfo();
            }

            if (StepsTabControl.SelectedIndex == StepsTabControl.TabCount - 1)
            {
                if (!FinalResult.VisibilityCalculationResults.HasFlag(VisibilityCalculationResultsEnum.ObservationPoints) 
                    && !FinalResult.VisibilityCalculationResults.HasFlag(VisibilityCalculationResultsEnum.ObservationPointSingle)
                    )
                {
                    MessageBox.Show(
                        LocalizationContext.Instance.FindLocalizedElement(
                            "WinMWarningNoSourceForCalculation", 
                            "The is no results sources for calculating. Please, select source!"),
                        LocalizationContext.Instance.MsgBoxWarningHeader, 
                        MessageBoxButtons.OK, 
                        MessageBoxIcon.Warning);
                }

                if (string.IsNullOrEmpty(imagesComboBox.Text))
                {
                    MessageBox.Show(
                        LocalizationContext.Instance.FindLocalizedElement(
                            "WinMWarningNoRasterLayrSelected",
                            "The Raster layer must be selected!"),
                        LocalizationContext.Instance.MsgBoxWarningHeader, 
                        MessageBoxButtons.OK, 
                        MessageBoxIcon.Warning);
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
            StepsTabControl.SelectedTab.Enabled = chkTrimRaster.Enabled = chkTrimRaster.Checked = false;
            (StepsTabControl.TabPages[StepsTabControl.SelectedIndex + 1] as TabPage).Enabled = panel1.Enabled = true;
            StepsTabControl.SelectedIndex++;
        }
        private void Button1_Click(object sender, EventArgs e)
        {
            FirstTypePicked();

            StepsTabControl.SelectedTab.Enabled = false;
            (StepsTabControl.TabPages[StepsTabControl.SelectedIndex + 1] as TabPage).Enabled = 
                panel1.Enabled = 
                chkTrimRaster.Enabled = 
                chkTrimRaster.Checked = true;
            StepsTabControl.SelectedIndex++;
        }
        private void tabControl_Selecting(object sender, TabControlCancelEventArgs e)
        {
            var nextTab = StepsTabControl.TabPages[StepsTabControl.SelectedIndex] as TabPage;
            if (!nextTab.Enabled)
            {
                e.Cancel = true;
            }
        }


        public void FillVisibilitySessionsList(IEnumerable<VisibilityTask> visibilitySessions, bool isNewSessionAdded)
        {
            throw new NotImplementedException();
        }


        public IEnumerable<string> GetTypes => throw new NotImplementedException();

        public IEnumerable<string> GetAffiliation => throw new NotImplementedException();

        public void FillVisibilitySessionsList(IEnumerable<VisibilityTask> visibilitySessions)
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
                MessageBox.Show(
                        LocalizationContext.Instance.FindLocalizedElement(
                            "WinMErrorValueOutRange",  $"Invalid data.\nInsert the value in the range from 0 to 100"),
                        LocalizationContext.Instance.MsgBoxWarningHeader,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                tbTransparency.Text = "33";
            }
        }
        public void FillVisibilitySessionsTree(IEnumerable<VisibilityTask> visibilitySessions, bool isNewSessionAdded)
        {
            throw new NotImplementedException();
        }

        private void tbTransparency_TextChanged(object sender, EventArgs e)
        {

        }

        public void FillVisibilityResultsTree(IEnumerable<VisibilityCalcResults> visibilityResults)
        {
            throw new NotImplementedException();
        }

        public void RemoveSessionFromList(string id)
        {
            throw new NotImplementedException();
        }

        public void FillVisibilitySessionsList(IEnumerable<VisibilityTask> visibilitySessions, bool isNewSessionAdded, string newTaskName)
        {
            throw new NotImplementedException();
        }
    }
}
