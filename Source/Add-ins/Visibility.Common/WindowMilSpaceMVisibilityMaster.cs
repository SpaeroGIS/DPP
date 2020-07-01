using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using MilSpace.Core;
using MilSpace.Core.DataAccess;
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
        private string _allValuesFilterText = "All";
        private string _allValuesMobility = "All";
        private ObservationPointsController controller = new ObservationPointsController(ArcMap.Document, ArcMap.ThisApplication);
        private BindingList<CheckObservPointGui> _observPointGuis;
        private BindingList<CheckObservPointGui> _observationObjects;
        private ObservationPoint _selectedObservationPoint;
        private IGeometry _selectedGeometry;
        private ObservationSetsEnum _observPointsSource;
        private ObservationSetsEnum _observObjectsSource;
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
            SetVOControlsVisibility(false);
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
                this.btnVO.Text = LocalizationContext.Instance.FindLocalizedElement("WinM.button2.Text", "Обрати >");
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
                this.lblOPType.Text = LocalizationContext.Instance.FindLocalizedElement("WinM.lblOPType.Text", "Джерело ПС");
                this.lblOOSource.Text = LocalizationContext.Instance.FindLocalizedElement("WinM.lblOOSourse.Text", "Джерело ОН");
                this.lblMinAzimuth.Text = LocalizationContext.Instance.FindLocalizedElement("MinM.lblMin.Text", "мін.");
                this.lblMinAngle.Text = LocalizationContext.Instance.FindLocalizedElement("MinM.lblMin.Text", "мін.");
                this.lblMaxAzimuth.Text = LocalizationContext.Instance.FindLocalizedElement("MinM.lblMax.Text", "макс.");
                this.lblMaxAngle.Text = LocalizationContext.Instance.FindLocalizedElement("MinM.lblMax.Text", "макс.");
                this.lblAzimuths.Text = LocalizationContext.Instance.FindLocalizedElement("MinM.lblAzimuths.Text", "Азимути:");
                this.lblAngles.Text = LocalizationContext.Instance.FindLocalizedElement("MinM.lblAngles.Text", "Кути нахилу:");
                this.lblHeights.Text = LocalizationContext.Instance.FindLocalizedElement("MinM.lblHeights.Text", "Вибірка висот:");
                this.lblFrom.Text = LocalizationContext.Instance.FindLocalizedElement("MinM.lblFrom.Text", "від");
                this.lblTo.Text = LocalizationContext.Instance.FindLocalizedElement("MinM.lblTo.Text", "до");
                this.lblStep.Text = LocalizationContext.Instance.FindLocalizedElement("MinM.lblStep.Text", "крок");
                this.lblBufferDistance.Text = LocalizationContext.Instance.FindLocalizedElement("MinM.lblBufferDistance.Text", "Відстань для буфера");
                this.lblCoveragePercent.Text = LocalizationContext.Instance.FindLocalizedElement("MinM.lblCoveragePercent.Text", "Відсоток покриття");
                this.chckSaveOPParams.Text = LocalizationContext.Instance.FindLocalizedElement("MinM.chckSaveOPParams.Text", "Враховувати параметри ПН");

                ToolTip toolTip = new ToolTip();
                toolTip.SetToolTip(btnShowPoint, LocalizationContext.Instance.FindLocalizedElement("MinM.btnShowPoint.ToolTip", "Показати пункт спостеження на мапі"));
                toolTip.SetToolTip(btnShowOO, LocalizationContext.Instance.FindLocalizedElement("MinM.btnShowOO.ToolTip", "Показати об'єкт спостереження на мапі"));

                SetDefaultValues();
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

        private void SetDefaultValues()
        {
            txtStep.Text = "1";
            txtBufferDistance.Text = "10";
            txtBufferDistanceFroAllObjects.Text = "10";
            txtCoveragePercent.Text = "100";
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
        public void EnableObjList()
        {
            dgvObjects.Enabled = true;
            dgvObjects.Refresh();
            splitContainer1.Panel2.Enabled = true;

        }
        public void FirstTypePicked()//triggers when user picks first type
        {
            _stepControl = VisibilityCalcTypeEnum.OpservationPoints;

            SetCalculaionLabels();
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

            SetCalculaionLabels();
            controller.UpdateObservationPointsList();
            EnableObjList();
            PopulateComboBox();
            FillObservPointLabel();
            FillObsObj(true);
            FillObservPointsOnCurrentView(controller.GetObservPointsOnCurrentMapExtent(ActiveView));
        }

        public void ThirdTypePicked()//triggers when user picks third type
        {
           
            _stepControl = VisibilityCalcTypeEnum.BestObservationParameters;

            SetCalculaionLabels();
            SetVOControlsVisibility(true);
            EnableObjList();
            PopulateComboBox();
        }

        public void FillObservPointLabel()
        {
            var temp = controller.GetObservationPointsLayers(ActiveView).ToArray();

        }
        public void FillComboBoxes()
        {
            _allValuesFilterText = controller.GetAllAffiliationType();
            _allValuesMobility = controller.GetAllMobilityType();

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

            PopulateObservationPointsTypesComboBox();
            PopulateObservationObjectsTypesComboBox();
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

        public void PopulateObservationPointsTypesComboBox()
        {
            cmbOPSource.Items.Clear();
            cmbOPSource.Items.AddRange(LocalizationContext.Instance.ObservPointSets.Select(set => set.Value).ToArray());
            cmbOPSource.SelectedItem = LocalizationContext.Instance.ObservPointsSet;
            _observPointsSource = controller.GetObservPointsSet(cmbOPSource.SelectedItem.ToString());
        }

        public void PopulateObservationObjectsTypesComboBox()
        {
            cmbOOSource.Items.Clear();
            cmbOOSource.Items.AddRange(LocalizationContext.Instance.ObservObjectsSets.Select(set => set.Value).ToArray());
            cmbOOSource.SelectedItem = LocalizationContext.Instance.ObservObjectsSet;
            _observObjectsSource = controller.GetObservStationSet(cmbOOSource.SelectedItem.ToString());
        }

        public void FillObservationPointList(IEnumerable<IObserverPoint> observationPoints,
                                                ValuableObservPointFieldsEnum filter, bool newSelection = false,
                                                bool clearUpdatedPointsList = true)
        {
            if (observationPoints != null && observationPoints.Any())
            {
                var ItemsToShow = observationPoints.Select(
                    item =>
                    {
                        var t = controller.GetObservationPointFromInterface(item);

                        return new CheckObservPointGui
                        {
                            Title = t.Title,
                            Type = LocalizationContext.Instance.MobilityTypes[t.ObservationPointMobilityType],
                            Affiliation = LocalizationContext.Instance.AffiliationTypes[t.ObservationPointAffiliationType],
                            Date = t.Dto.Value.ToString(Helper.DateFormatSmall),
                            Id = t.Objectid
                        };

                    }).OrderBy(l => l.Title).ToList();


                dgvCheckList.Rows.Clear();
                dgvCheckList.CurrentCell = null;

                _observPointGuis = new BindingList<CheckObservPointGui>(ItemsToShow);
                dgvCheckList.DataSource = _observPointGuis;

                SetDataGridView();

                dgvCheckList.Update();
                dgvCheckList.Rows[0].Selected = true;
            }

        }
        public void FillObservPointsOnCurrentView(IEnumerable<ObservationPoint> observationPoints)
        {
            if (observationPoints != null && observationPoints.Any())
            {
                var ItemsToShow = observationPoints.Select(t => new CheckObservPointGui
                {
                    Title = t.Title,
                    Type = LocalizationContext.Instance.MobilityTypes[t.ObservationPointMobilityType],
                    Affiliation = LocalizationContext.Instance.AffiliationTypes[t.ObservationPointAffiliationType],
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

                dgvCheckList.CurrentCell = null;

                dgvCheckList.DataSource = temp;
                SetDataGridView();

                dgvCheckList.Update();

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
                        Affiliation = controller.GetObservObjectsTypeString(t.ObjectType),
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

        public void FillObservationObjectsList(Dictionary<int, IGeometry> observationObjects)
        {
            if(observationObjects == null)
            {
                dgvObjects.Text = "Objects are not found";
                return;
            }

            try
            {
                var itemsToShow = observationObjects.Select(
                    t => new CheckObservPointGui
                    {
                        Title = t.Key.ToString(),
                        Affiliation = controller.GetObservObjectsTypeString(ObservationObjectTypesEnum.All),
                        Id = t.Key,
                        Type = string.Empty,
                        Date = DateTime.Now.ToString(Helper.DateFormatSmall)
                    }).OrderBy(l => l.Title);


                dgvObjects.DataSource = null; //Clearing listbox

                _observationObjects = new BindingList<CheckObservPointGui>(itemsToShow.ToArray());

                if (_observationObjects != null)
                {
                    dgvObjects.DataSource = _observationObjects;
                    SetDataGridView_For_Objects();
                }
            }
            catch (ArgumentNullException)
            {
                dgvObjects.Text = "Objects are not found";
            }
        }

        public void FillSelectedOPFields(ObservationPoint point)
        {
            _selectedObservationPoint = point;

            lblSelectedOP.Text = point.Title;
            txtMinAzimuth.Text = point.AzimuthStart.Value.ToFormattedString(1);
            txtMaxAzimuth.Text = point.AzimuthEnd.Value.ToFormattedString(1);
            txtMinAngle.Text = point.AngelMinH.Value.ToFormattedString(1);
            txtMaxAngle.Text = point.AngelMaxH.Value.ToFormattedString(1);
            txtMinHeight.Text = point.RelativeHeight.Value == 0 ? "1" : point.RelativeHeight.Value.ToFormattedString(1);
            txtMaxHeight.Text = (point.RelativeHeight.Value + 100).ToFormattedString(1);
            txtStep.Text = "10";

            SetSelectedOPControlsEnabled(true);
        }

        public void AddSelectedOO(IGeometry geometry, string title)
        {
            _selectedGeometry = geometry;

            lblSelectedOO.Text = title;
            txtBufferDistance.Enabled = (geometry.GeometryType != esriGeometryType.esriGeometryPolygon);
            txtCoveragePercent.Enabled = true;
            btnShowOO.Enabled = true;
        }

        private void SetSelectedOPControlsEnabled(bool enabled)
        {
            txtMinAzimuth.Enabled = txtMaxAzimuth.Enabled = txtMinAngle.Enabled = txtMaxAngle.Enabled
             = txtMinHeight.Enabled = txtMaxHeight.Enabled = txtStep.Enabled = enabled;
            btnShowPoint.Enabled = enabled;
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
                dgvCheckList.Columns["Check"].HeaderText = "";
                dgvCheckList.Columns["Id"].Visible = false;

                dgvCheckList.Columns["Date"].ReadOnly = true;
                dgvCheckList.Columns["Type"].ReadOnly = true;
                dgvCheckList.Columns["Affiliation"].ReadOnly = true;
                dgvCheckList.Columns["Title"].ReadOnly = true;

                dgvCheckList.Columns["Affiliation"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
                dgvCheckList.Columns["Title"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dgvCheckList.Columns["Type"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;

                dgvCheckList.Columns["Affiliation"].MinimumWidth = 50;
                dgvCheckList.Columns["Title"].MinimumWidth = 50;
                dgvCheckList.Columns["Type"].MinimumWidth = 50;
                dgvCheckList.Columns["Date"].MinimumWidth = 50;

                if (dgvCheckList.Columns.Contains("Chck"))
                {
                    dgvCheckList.Columns["Chck"].MinimumWidth = 25;
                }
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
            DisplaySelectedColumns_Points(dgvCheckList);
        }

        private void Filter_For_Object_CheckedChanged(object sender, EventArgs e)
        {
            DisplaySelectedColumns_Objects(dgvObjects);
        }
        private void Select_All(object sender, EventArgs e)
        {

            foreach (CheckObservPointGui o in _observationObjects)
            {
                o.Check = chckOO.Checked;
            }

            dgvObjects.DataSource = _observationObjects;
            dgvObjects.Refresh();

        }
        private void Select_All_Points(object sender, EventArgs e)
        {

            foreach (CheckObservPointGui o in _observPointGuis)
            {
                o.Check = chckOP.Checked;
            }

            dgvCheckList.DataSource = _observPointGuis;
            dgvCheckList.Refresh();
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

        private  void FilterPointsData()
        {

            if (dgvCheckList.Rows.Count == 0)
            {
                return;
            }

            dgvCheckList.CurrentCell = null;

            var mobilityValue = cmbType.SelectedItem.ToString();
            var affiliationValue = cmbAffiliation.SelectedItem.ToString();

            foreach (DataGridViewRow row in dgvCheckList.Rows)
            {
                bool allowVisibleType = (mobilityValue == _allValuesMobility) || row.Cells["Type"].Value.Equals(mobilityValue);
                bool allowVisibleAffiliation = (affiliationValue == _allValuesFilterText) || row.Cells["Affiliation"].Value.Equals(affiliationValue);
                row.Visible = allowVisibleType && allowVisibleAffiliation;
            }

            if (dgvCheckList.FirstDisplayedScrollingRowIndex != -1)
            {
                dgvCheckList.Rows[dgvCheckList.FirstDisplayedScrollingRowIndex].Selected = true;
            }
        }

        private void FilterData(DataGridView Grid, ComboBox combo, string column, string allValues)
        {
            if (Grid.Rows.Count == 0)
            {
                return;
            }

            Grid.CurrentCell = null;

            foreach (DataGridViewRow row in Grid.Rows)
            {
                CheckRowForFilter(row, combo, column, allValues);
            }

            if (Grid.FirstDisplayedScrollingRowIndex != -1)
            {
                Grid.Rows[Grid.FirstDisplayedScrollingRowIndex].Selected = true;
            }
        }

        private void CheckRowForFilter(DataGridViewRow row, ComboBox combo, string column, string allValues)
        {
            if (combo.SelectedItem != null && combo.SelectedItem.ToString() != allValues)
            {
                row.Visible = (row.Cells[column].Value.ToString() == combo.SelectedItem.ToString());
                if (!row.Visible) return;
            }

            row.Visible = true;
        }
        private void cmbType_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterPointsData();
        }

        private void cmbAffiliation_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterPointsData();
        }
        private void cmbObservObject_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterData(dgvObjects, cmbObservObject, "Affiliation", _allValuesFilterText);
        }

        private void AssemblyWizardResult()
        {
            if (_stepControl == VisibilityCalcTypeEnum.BestObservationParameters)
            {
                SaveObservationPointParams();

                double fromHeight;
                if(!txtMinHeight.Text.TryParceToDouble(out fromHeight))
                {
                    fromHeight = 0;
                }

                double toHeight;
                if(!txtMaxHeight.Text.TryParceToDouble(out toHeight))
                {
                    toHeight = 0;
                }

                FinalResult = new WizardResult
                {
                    ObservationPoint = _selectedObservationPoint,
                    ObservationStation = controller.CalculateGeometryWithBuffer(_selectedGeometry, Convert.ToInt32(txtBufferDistance.Text)),
                    RasterLayerName = imagesComboBox.SelectedItem.ToString(),
                    CalculationType = _stepControl,
                    TaskName = VisibilityManager.GenerateResultId(LocalizationContext.Instance.CalcTypeLocalisationShort[_stepControl]),
                    FromHeight = fromHeight,
                    ToHeight = toHeight,
                    Step = Convert.ToInt32(txtStep.Text),
                    VisibilityPercent = Convert.ToInt16(txtCoveragePercent.Text),
                    VisibilityCalculationResults = VisibilityCalculationResultsEnum.BestParametersTable | VisibilityCalculationResultsEnum.ObservationPoints
                    | VisibilityCalculationResultsEnum.VisibilityAreaRaster
                };

                return;
            }

            var rasterName = cmbMapLayers.SelectedItem.ToString();

            FinalResult = new WizardResult
            {
                ObservPointIDs = _observPointGuis.Where(p => p.Check).Select(i => i.Id).ToArray(),
                Table = TableChkBox.Checked,
                SumFieldOfView = SumChkBox.Checked,
                RasterLayerName = imagesComboBox.SelectedItem.ToString(),
                RelativeLayerName = rasterName,
                ResultLayerPosition = controller.GetPositionByStringValue(cmbPositions.SelectedItem.ToString()),
                ResultLayerTransparency = Convert.ToInt16(tbTransparency.Text),
                CalculationType = _stepControl,
                TaskName = VisibilityManager.GenerateResultId(LocalizationContext.Instance.CalcTypeLocalisationShort[_stepControl]),
                VisibilityPercent = 100,
                ObserverPointsLayerName = controller.ObserverPointsLayerName,
                ObservationObjectLayerName = controller.ObservationObjectsLayerName,
                ObserverPointsSourceType = _observPointsSource,
                ObserverObjectsSourceType = _observObjectsSource,
                Buffer = Convert.ToInt32(txtBufferDistanceFroAllObjects.Text)
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

            FinalResult.VisibilityCalculationResults |= VisibilityCalculationResultsEnum.CoverageTable;
        }

        public void SummaryInfo()
        {
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
                    && !FinalResult.VisibilityCalculationResults.HasFlag(VisibilityCalculationResultsEnum.BestParametersTable)
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
                ResetControlsSettings();
            }
        }

        private void ResetControlsSettings()
        {
            panel1.Enabled = false;
            ObservPointLabel.Text = controller.GetObservPointsFromGdbFeatureClassName();
            SetVOControlsVisibility(false);
            cmbOPSource.SelectedIndex = 0;
            cmbOOSource.SelectedIndex = 0;
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
                            "WinMErrorValueOutRange", $"Invalid data.\nInsert the value in the range from 0 to 100"),
                        LocalizationContext.Instance.MsgBoxWarningHeader,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                tbTransparency.Text = "33";
            }
        }

        private void BtnVO_Click(object sender, EventArgs e)
        {
            ThirdTypePicked();
            StepsTabControl.SelectedTab.Enabled = false;
            (StepsTabControl.TabPages[StepsTabControl.SelectedIndex + 1] as TabPage).Enabled = panel1.Enabled = true;
            StepsTabControl.SelectedIndex++;
        }

        private void SetVOControlsVisibility(bool isVisible)
        {
            panelBufferDistanceForAllObjects.Visible = !isVisible;
            observPointsFiltersPanel.Visible = !isVisible;
            observObjectsFiltersPanel.Visible = !isVisible;
            columnsOOVisibilityPanel.Visible = !isVisible;
            columnsOPVisibilityPanel.Visible = !isVisible;
            selectedOPPanel.Visible = isVisible;
            selectedOOPanel.Visible = isVisible;
            chckOP.Visible = !isVisible;
            chckOO.Visible = !isVisible;
        }

        public void FillVisibilitySessionsTree(IEnumerable<VisibilityTask> visibilitySessions, bool isNewSessionAdded)
        {
            throw new NotImplementedException();
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

        private void BtnChooseOP_Click(object sender, EventArgs e)
        {
            _observPointsSource = controller.GetObservPointsSet(cmbOPSource.SelectedItem.ToString());

            if (_observPointsSource == ObservationSetsEnum.Gdb && _stepControl != VisibilityCalcTypeEnum.BestObservationParameters)
            {
                FillObservPointsOnCurrentView(controller.GetObservPointsOnCurrentMapExtent(ActiveView));
            }
            else
            {
                controller.FillObserverPointsInMasterFromSelectedSource(_observPointsSource, _stepControl);
            }
        }

        private void BtnChooseOO_Click(object sender, EventArgs e)
        {
            _observObjectsSource = controller.GetObservStationSet(cmbOOSource.SelectedItem.ToString());

            if (_observObjectsSource == ObservationSetsEnum.Gdb && _stepControl != VisibilityCalcTypeEnum.BestObservationParameters)
            {
                FillObsObj(true);
            }
            else
            {
                controller.FillObservationObjectsInMasterFromSelectedSource(_observObjectsSource, _stepControl);
            }

            txtBufferDistanceFroAllObjects.Enabled = _observObjectsSource != ObservationSetsEnum.Gdb;
        }


        private void BtnShowPoint_Click(object sender, EventArgs e)
        {
            if (_selectedObservationPoint != null)
            {
                controller.ShowPoint(_selectedObservationPoint);
            }

        }

        private void BtnShowOO_Click(object sender, EventArgs e)
        {
            if (_selectedGeometry != null)
            {
                controller.ShowGeometry(_selectedGeometry, Convert.ToInt32(txtBufferDistance.Text));
            }
        }

        private void SaveObservationPointParams()
        {
            if (txtMinAzimuth.Text.TryParceToDouble(out double minAzimuth))
            {
                _selectedObservationPoint.AzimuthStart = minAzimuth;
            }

            if (txtMaxAzimuth.Text.TryParceToDouble(out double maxAzimuth))
            {
                _selectedObservationPoint.AzimuthEnd = maxAzimuth;
            }

            if (txtMinAngle.Text.TryParceToDouble(out double minAngle))
            {
                _selectedObservationPoint.AngelMinH = minAngle;
            }

            if (txtMaxAngle.Text.TryParceToDouble(out double maxAngle))
            {
                _selectedObservationPoint.AngelMaxH = maxAngle;
            }
        }

        private void SetCalculaionLabels()
        {
            lblCalculationsType.Text = lblCalculationsTypeStep3.Text = lblCalculationsTypeStep4.Text =
                     LocalizationContext.Instance
                                        .CalcTypeLocalisationShort[_stepControl];
        }

        #region Validation

        private void TxSignedDouble_KeyPress(object sender, KeyPressEventArgs e)
        {
            var textBox = (TextBox)sender;
            e.Handled = !(CheckDouble(textBox, e.KeyChar) || (e.KeyChar == (char)KeyCodesEnum.Minus));
        }

        private void FieldsWithInteger_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !(char.IsNumber(e.KeyChar) || e.KeyChar == (char)Keys.Back);
        }

        private void FieldsWithDouble_KeyPress(object sender, KeyPressEventArgs e)
        {
            var textBox = (TextBox)sender;
            e.Handled = !CheckDouble(textBox, e.KeyChar);
        }

        private bool CheckDouble(TextBox textBox, char keyChar)
        {
            return (char.IsNumber(keyChar) || keyChar == (char)Keys.Back
                            || (keyChar == (int)KeyCodesEnum.DecimalPoint && textBox.Text.IndexOf(".") == -1));
        }

        private void TxtCoveragePercent_Leave(object sender, EventArgs e)
        {
            var textBox = (TextBox)sender;
            ValidateRangedValues(textBox, 0, 100, "100");
        }

        private void TxtAzimuth_Leave(object sender, EventArgs e)
        {
            var textBox = sender as TextBox;
            var replaceValue = (textBox == txtMaxAzimuth) ? _selectedObservationPoint.AzimuthEnd.Value : _selectedObservationPoint.AzimuthStart.Value;

            ValidateRangedValues(textBox, 0, 360, replaceValue.ToFormattedString(1), txtMinAzimuth, txtMaxAzimuth);
        }

        private void TxtAngle_Leave(object sender, EventArgs e)
        {
            var textBox = sender as TextBox;
            var replaceValue = (textBox == txtMaxAngle) ? _selectedObservationPoint.AngelMaxH.Value : _selectedObservationPoint.AngelMinH.Value;

            ValidateRangedValues(textBox, -90, 90, replaceValue.ToFormattedString(1), txtMinAngle, txtMaxAngle);
        }

        private void TxtHeight_Leave(object sender, EventArgs e)
        {
            var textBox = sender as TextBox;
            ValidateRangedValues(textBox, 1, Double.MaxValue, _selectedObservationPoint.RelativeHeight.Value.ToFormattedString(1), txtMinHeight, txtMaxHeight);
        }

        private void TxtStep_Leave(object sender, EventArgs e)
        {
            var textBox = sender as TextBox;
            var isInteger = Int32.TryParse(textBox.Text, out int value);

            if (!isInteger)
            {
                MessageBox.Show(LocalizationContext.Instance.InvalidFormatMessage, LocalizationContext.Instance.MessageBoxWarningCaption);
                textBox.Text = "1";
                return;
            }

            if (value < 0)
            {
                MessageBox.Show(LocalizationContext.Instance.ValueLessThenZeroMessage, LocalizationContext.Instance.MessageBoxWarningCaption);
                textBox.Text = "1";
                return;
            }

            var isMaxDouble = txtMaxHeight.Text.TryParceToDouble(out double max);
            var isMinDouble = txtMinHeight.Text.TryParceToDouble(out double min);

            if (isMaxDouble && isMinDouble)
            {
                var heightsDiff = max - min;

                if (value > heightsDiff)
                {
                    MessageBox.Show(String.Format(LocalizationContext.Instance.ValueMoreThanMaxMessage, heightsDiff.ToString("F0")), LocalizationContext.Instance.MessageBoxWarningCaption);
                    textBox.Text = "1";
                    return;
                }
            }
        }


        private void ValidateRangedValues(TextBox sender, double min, double max, string replaceValue, TextBox minTxt = null, TextBox maxTxt = null)
        {
            var isDouble = sender.Text.TryParceToDouble(out double value);

            if (!isDouble)
            {
                MessageBox.Show(LocalizationContext.Instance.InvalidFormatMessage, LocalizationContext.Instance.MessageBoxWarningCaption);
                sender.Text = replaceValue;
                return;
            }

            if (value < min || value > max)
            {
                MessageBox.Show(String.Format(LocalizationContext.Instance.IncorrectRangeMessage, min, max), LocalizationContext.Instance.MessageBoxWarningCaption);
                sender.Text = replaceValue;
                return;
            }

            if (maxTxt != null && minTxt != null)
            {
                var isMax = (sender == maxTxt);

                if (isMax)
                {
                    var isMinDouble = minTxt.Text.TryParceToDouble(out double minValue);
                    if (isMinDouble && value < minValue)
                    {
                        MessageBox.Show(String.Format(LocalizationContext.Instance.ValueLessThanMinMessage, minTxt.Text), LocalizationContext.Instance.MessageBoxWarningCaption);
                        sender.Text = replaceValue;
                        return;
                    }
                }
                else
                {
                    var isMaxDouble = maxTxt.Text.TryParceToDouble(out double maxValue);

                    if (isMaxDouble && value > maxValue)
                    {
                        MessageBox.Show(String.Format(LocalizationContext.Instance.ValueMoreThanMaxMessage, maxTxt.Text), LocalizationContext.Instance.MessageBoxWarningCaption);
                        sender.Text = replaceValue;
                        return;
                    }
                }
            }
        }

        public void ClearObserverPointsList(bool isOPFromGdb)
        {
            cmbOPSource.SelectedItem = LocalizationContext.Instance.ObservPointsSet;

            if (isOPFromGdb)
            {
                dgvCheckList.DataSource = null;
            }
            else
            {
                _observPointsSource = controller.GetObservPointsSet(cmbOPSource.SelectedItem.ToString());
                controller.FillObserverPointsInMasterFromSelectedSource(_observPointsSource, _stepControl);
            }
        }

        public void SetFieldsEditingAbility(bool areFiedlsReadOnly)
        {
            throw new NotImplementedException();
        }

        public void RemoveObserverPoint(int id)
        {
            var source = dgvCheckList.DataSource as BindingList<CheckObservPointGui>;
            var sourceList = new BindingList<CheckObservPointGui>(source);
            sourceList.Remove(sourceList.First(point => point.Id == id));

            dgvCheckList.DataSource = sourceList;
        }
        #endregion
    }
}
