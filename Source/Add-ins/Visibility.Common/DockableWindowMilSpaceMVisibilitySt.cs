using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Editor;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geometry;

using MilSpace.Core;
using MilSpace.Core.Tools;
using MilSpace.DataAccess.DataTransfer;
using MilSpace.Tools;
using MilSpace.Visibility.DTO;
using MilSpace.Visibility.Localization;
using MilSpace.Visibility.ViewController;

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace MilSpace.Visibility
{
    /// <summary>
    /// Designer class of the dockable window add-in. It contains user interfaces that
    /// make up the dockable window.
    /// </summary>
    public partial class DockableWindowMilSpaceMVisibilitySt : UserControl, IObservationPointsView
    {
        private ObservationPointsController _observPointsController;
        private VisibilitySessionsController _visibilitySessionsController;

        private bool observatioPointsSordDirection = true;
        private bool observatioObjectsSordDirection = true;
        private bool tasksSordDirection = false;
        private List<VisibilityTasknGui> _visibilitySessionsGui = new List<VisibilityTasknGui>();

        private bool _isDropDownItemChangedManualy = false;
        private bool _isFieldsChanged = false;
        private bool _isObservObjectsFieldsChanged = false;
        private ObservationPoint selectedPointMEM = new ObservationPoint();

        private int _selectedPointId => Convert.ToInt32(dgvObservationPoints.SelectedRows[0].Cells["Id"].Value);
        //private bool IsPointFieldsEnabled => _observPointsController.IsObservPointsExists();
        private bool IsPointFieldsEnabled = true;
        private ValuableObservPointSortFieldsEnum curObservPointsSorting = ValuableObservPointSortFieldsEnum.Name;
        private VeluableObservObjectSortFieldsEnum curObservObjectSorting = VeluableObservObjectSortFieldsEnum.Title;

        private static Logger log = Logger.GetLoggerEx("MilSpace.Visibility.DockableWindowMilSpaceMVisibilitySt");

        public DockableWindowMilSpaceMVisibilitySt(object hook, ObservationPointsController controller)
        {
            log.InfoEx(">>> DockableWindowMilSpaceMVisibilitySt (Constructor) START <<<");

            InitializeComponent();
            LocalizeComponent();
            this._observPointsController = controller;
            this._observPointsController.SetView(this);
            this.Hook = hook;
            SetVisibilitySessionsController();

            log.InfoEx("> DockableWindowMilSpaceMVisibilitySt (Constructor) END");
        }

        private void LocalizeComponent()
        {
            log.InfoEx("> LocalizeComponent (Visibility) START");
            try
            {
                this.Text = LocalizationContext.Instance.WindowCaption;

                this.tbpPoints.Text = LocalizationContext.Instance.FindLocalizedElement("MainW_tbpPoints_Text", "Пункти С");
                this.tabPage5.Text = LocalizationContext.Instance.FindLocalizedElement("MainW_tabPage5_Text", "Параметри ПС)");
                this.label19.Text = LocalizationContext.Instance.FindLocalizedElement("MainW_label19_Text", "Висота над поверхнею, м");
                this.lblMinDistance.Text = LocalizationContext.Instance.FindLocalizedElement("MainW_lblMinDistance_Text", "до");
                this.lblMaxDistance.Text = LocalizationContext.Instance.FindLocalizedElement("MainW_lblMaxDistance_Text", "Відст., м від");
                this.label52.Text = LocalizationContext.Instance.FindLocalizedElement("MainW_label52_Text", "до");
                this.label18.Text = LocalizationContext.Instance.FindLocalizedElement("MainW_label18_Text", "Верт вугол від");
                this.label13.Text = LocalizationContext.Instance.FindLocalizedElement("MainW_label13_Text", "до");
                this.label14.Text = LocalizationContext.Instance.FindLocalizedElement("MainW_label14_Text", "Азимут від");
                this.tlbbGetCoord.ToolTipText = LocalizationContext.Instance.FindLocalizedElement("MainW_tlbbGetCoord_ToolTipText", "Получить координаты с карты");
                this.tlbbCopyCoord.ToolTipText = LocalizationContext.Instance.FindLocalizedElement("MainW_tlbbCopyCoord_ToolTipText", "Скопировать");
                this.tlbbPasteCoord.ToolTipText = LocalizationContext.Instance.FindLocalizedElement("MainW_tlbbPasteCoord_ToolTipText", "Вставить");
                this.tlbbShowCoord.ToolTipText = LocalizationContext.Instance.FindLocalizedElement("MainW_tlbbShowCoord_ToolTipText", "Показать на карте");
                this.label8.Text = LocalizationContext.Instance.FindLocalizedElement("MainW_label8_Text", "Координати");
                this.label10.Text = LocalizationContext.Instance.FindLocalizedElement("MainW_label10_Text", "Належн");
                this.label11.Text = LocalizationContext.Instance.FindLocalizedElement("MainW_label11_Text", "Тип");
                this.label24.Text = LocalizationContext.Instance.FindLocalizedElement("MainW_label24_Text", "Оператор");
                this.label27.Text = LocalizationContext.Instance.FindLocalizedElement("MainW_label27_Text", "Дата");
                this.label20.Text = LocalizationContext.Instance.FindLocalizedElement("MainW_label20_Text", "Назва");
                //this.label15.Text = LocalizationContext.Instance.FindLocalizedElement("MainW_label15_Text", "верт.");
                //this.label16.Text = LocalizationContext.Instance.FindLocalizedElement("MainW_label16_Text", "кут бачення гориз");
                //this.label58.Text = LocalizationContext.Instance.FindLocalizedElement("MainW_label58_Text", "азимут осн. оси, дгр");
                //this.label55.Text = LocalizationContext.Instance.FindLocalizedElement("MainW_label55_Text", "верт");
                //this.label56.Text = LocalizationContext.Instance.FindLocalizedElement("MainW_label56_Text", "поворот камеры, дгр, гориз.");
                this.label7.Text = LocalizationContext.Instance.FindLocalizedElement("MainW_label7_Text", "Параметри ПС");
                this.buttonSaveOPoint.Tag = LocalizationContext.Instance.FindLocalizedElement("MainW_buttonSaveOPoint_Tag", "зберегти параметри ПС");
                this.tlbbShowPoint.ToolTipText = LocalizationContext.Instance.FindLocalizedElement("MainW_tlbbShowPoint_ToolTipText", "показати ПС на карті");
                this.tlbbAddNewPoint.ToolTipText = LocalizationContext.Instance.FindLocalizedElement("MainW_tlbbAddNewPoint_ToolTipText", "добавити новий ПС");
                this.tlbbAddObserPointLayer.ToolTipText = LocalizationContext.Instance.FindLocalizedElement("MainW_tlbbAddObserPointLayer_ToolTipText", "добавити шар пунктів спостереження");
                this.tlbbRemovePoint.ToolTipText = LocalizationContext.Instance.FindLocalizedElement("MainW_tlbbRemovePoint_ToolTipText", "видалити ПС");
                this.chckFilterDate.Text = LocalizationContext.Instance.FindLocalizedElement("MainW_chckFilterDate_Text", "дата");
                this.chckFilterAffiliation.Text = LocalizationContext.Instance.FindLocalizedElement("MainW_chckFilterAffiliation_Text", "належність");
                this.chckFilterType.Text = LocalizationContext.Instance.FindLocalizedElement("MainW_chckFilterType_Text", "тип");
                this.label3.Text = LocalizationContext.Instance.FindLocalizedElement("MainW_label3_Text", "Належн");
                this.label2.Text = LocalizationContext.Instance.FindLocalizedElement("MainW_label2_Text", "Тип");
                this.tbpObservObjects.Text = LocalizationContext.Instance.FindLocalizedElement("MainW_tbpObservObjects_Text", "Області Н");
                this.label39.Text = LocalizationContext.Instance.FindLocalizedElement("MainW_label39_Text", "дата створення");
                this.label5.Text = LocalizationContext.Instance.FindLocalizedElement("MainW_label5_Text", "оператор");
                this.label29.Text = LocalizationContext.Instance.FindLocalizedElement("MainW_label29_Text", "назва");
                this.label31.Text = LocalizationContext.Instance.FindLocalizedElement("MainW_label31_Text", "належність");
                this.label1.Text = LocalizationContext.Instance.FindLocalizedElement("MainW_label1_Text", "група");
                this.label4.Text = LocalizationContext.Instance.FindLocalizedElement("MainW_label4_Text", "Характеристики ОН");
                this.btnSaveParamPS.Tag = LocalizationContext.Instance.FindLocalizedElement("MainW_btnSaveParamPS_Tag", "Зберегти зміни");
                this.toolBarButton31.ToolTipText = LocalizationContext.Instance.FindLocalizedElement("MainW_toolBarButton31_ToolTipText", "Показати ОН на карті ");
                this.toolBarButton32.ToolTipText = LocalizationContext.Instance.FindLocalizedElement("MainW_toolBarButton32_ToolTipText", "Підсвітити ОН на карті");
                this.toolBarButton34.ToolTipText = LocalizationContext.Instance.FindLocalizedElement("MainW_toolBarButton34_ToolTipText", "видилити ОН");
                this.toolBarButton29.ToolTipText = LocalizationContext.Instance.FindLocalizedElement("MainW_toolBarButton29_ToolTipText", "поновити список ОН");
                this.chckObservObjAffiliation.Text = LocalizationContext.Instance.FindLocalizedElement("MainW_chckObservObjAffiliation_Text", "належність");
                this.chckObservObjGroup.Text = LocalizationContext.Instance.FindLocalizedElement("MainW_chckObservObjGroup_Text", "група");
                this.chckObservObjTitle.Text = LocalizationContext.Instance.FindLocalizedElement("MainW_chckObservObjTitle_Text", "назва");
                this.label28.Text = LocalizationContext.Instance.FindLocalizedElement("MainW_label28_Text", "належність");
                this.lblObservObjHeader.Text = LocalizationContext.Instance.FindLocalizedElement("MainW_label30_Text", "Області нагляду (ОН)");
                this.tlbbAddObservObjLayer.Tag = LocalizationContext.Instance.FindLocalizedElement("MainW_tlbbAddObservObjLayer_Tag", "Додати шар ОН до карти");
                this.toolBarButton18.ToolTipText = LocalizationContext.Instance.FindLocalizedElement("MainW_toolBarButton18_ToolTipText", "Вказать на карті");
                this.toolBarButton20.ToolTipText = LocalizationContext.Instance.FindLocalizedElement("MainW_toolBarButton20_ToolTipText", "Побновити");
                this.toolBarButton21.ToolTipText = LocalizationContext.Instance.FindLocalizedElement("MainW_toolBarButton21_ToolTipText", "Редагувати");
                this.toolBarButton22.ToolTipText = LocalizationContext.Instance.FindLocalizedElement("MainW_toolBarButton22_ToolTipText", "Показати на карті");
                this.toolBarButton23.ToolTipText = LocalizationContext.Instance.FindLocalizedElement("MainW_toolBarButton23_ToolTipText", "Показати параметри на карті");
                this.toolBarButton24.ToolTipText = LocalizationContext.Instance.FindLocalizedElement("MainW_toolBarButton24_ToolTipText", "Додати");
                this.toolBarButton25.ToolTipText = LocalizationContext.Instance.FindLocalizedElement("MainW_toolBarButton25_ToolTipText", "Видалити");
                this.tbpSessions.Text = LocalizationContext.Instance.FindLocalizedElement("MainW_tbpSessions_Text", "Завдання");
                this.tbpSessions.ToolTipText = LocalizationContext.Instance.FindLocalizedElement("MainW_tbpSessions_ToolTipText", "Завдання для розрахунків");
                this.label45.Text = LocalizationContext.Instance.FindLocalizedElement("MainW_label45_Text", "час закінчення");
                this.label44.Text = LocalizationContext.Instance.FindLocalizedElement("MainW_label44_Text", "час старту");
                this.label43.Text = LocalizationContext.Instance.FindLocalizedElement("MainW_label43_Text", "час створення");
                this.label42.Text = LocalizationContext.Instance.FindLocalizedElement("MainW_label42_Text", "оператор");
                this.label46.Text = LocalizationContext.Instance.FindLocalizedElement("MainW_label46_Text", "назва");
                this.label40.Text = LocalizationContext.Instance.FindLocalizedElement("MainW_label40_Text", "Інформація про завдання");
                this.wizardTask.ToolTipText = LocalizationContext.Instance.FindLocalizedElement("MainW_wizardTask_ToolTipText", "сформувати завдання на розрахунок");
                this.removeTask.ToolTipText = LocalizationContext.Instance.FindLocalizedElement("MainW_removeTask_ToolTipText", "видалити інформацію про завдання");
                this.label38.Text = LocalizationContext.Instance.FindLocalizedElement("MainW_label38_Text", "стан");
                this.lblCalcTasksHeader.Text = LocalizationContext.Instance.FindLocalizedElement("MainW_label36_Text", "Завдання для розрахунків");
                this.tbpVisibilityAreas.Text = LocalizationContext.Instance.FindLocalizedElement("MainW_tbpVisibilityAreas_Text", "ОВ");
                this.tbpVisibilityAreas.ToolTipText = LocalizationContext.Instance.FindLocalizedElement("MainW_tbpVisibilityAreas_ToolTipText", "Області видимості (результати розрахунків)");
                this.labelHeaderVisibilityInfo.Text = LocalizationContext.Instance.FindLocalizedElement("MainW_labelHeaderVisibilityInfo_Text", "Інформація про результат");
                this.tlbbZoomToResultRaster.ToolTipText = LocalizationContext.Instance.FindLocalizedElement("MainW_tlbbZoomToResultRaster_ToolTipText", "Показати");
                this.tlbbViewParamOnMap.ToolTipText = LocalizationContext.Instance.FindLocalizedElement("MainW_tlbbViewParamOnMap_ToolTipText", "Показати параметри на карті");
                this.tlbbAddFromDB.ToolTipText = LocalizationContext.Instance.FindLocalizedElement("MainW_tlbbAddFromDB_ToolTipText", "Додати");
                this.tlbbFullDelete.ToolTipText = LocalizationContext.Instance.FindLocalizedElement("MainW_tlbbFullDelete_ToolTipText", "видалити звідусіль");
                this.toolBarButtonRemoveFromSeanse.ToolTipText = LocalizationContext.Instance.FindLocalizedElement("MainW_toolBarButtonRemoveFromSeanse_ToolTipText", "видалити з сеансу роботи");
                this.tlbbUpdate.ToolTipText = LocalizationContext.Instance.FindLocalizedElement("MainW_tlbbUpdate_ToolTipText", "Поновити");
                this.lblHeaderVisibilityResult.Text = LocalizationContext.Instance.FindLocalizedElement("MainW_labelHeaderVisibilityResult_Text", "Список результатів");
                this.richTextBox1.Text = LocalizationContext.Instance.FindLocalizedElement(
                    "MainW_richTextBox1_Text",
                    "при выборе мобильного типа углы нелоступны, высоты недоступны, координаты недоступны, при добавлении дата и время автоматом");
                this.label12.Text = LocalizationContext.Instance.FindLocalizedElement("MainW_label12_Text", "макс");
                this.label9.Text = LocalizationContext.Instance.FindLocalizedElement("MainW_label9_Text", "висота над поверхнею (м) мин");
                this.btnAddLayerPS.Tag = LocalizationContext.Instance.FindLocalizedElement("MainW_btnAddLayerPS_Tag", "додати шар ПС до карти");
                this.lblLayer.Text = LocalizationContext.Instance.FindLocalizedElement("MainW_lblLayer_Text", "Пункти спостереження (ПС)");

                log.InfoEx("> LocalizeComponent (Visibility) END");
            }
            catch (Exception ex)
            {
                log.InfoEx("> LocalizeComponent (Visibility) Exception: {0}", ex.Message);

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
        public DockableWindowMilSpaceMVisibilitySt(object hook)
        {
            InitializeComponent();
            this.Hook = hook;
        }

        protected override void OnLoad(EventArgs e)
        {
            log.InfoEx("> OnLoad START");

            base.OnLoad(e);
            SubscribeForEvents();
            InitializeData();

            OnContentsChanged();

            log.InfoEx("> OnLoad END");
        }

        private void SubscribeForEvents()
        {
            log.InfoEx("> SubscribeForEvents START");


            ArcMap.Events.OpenDocument += OnContentsChanged;
            ArcMap.Events.NewDocument += OnContentsChanged;

            ArcMap.Events.OpenDocument += delegate ()
            {
                IActiveViewEvents_Event activeViewEvent = (IActiveViewEvents_Event)ActiveView;
                //activeViewEvent.SelectionChanged += OnContentsChanged;
                activeViewEvent.ItemAdded += OnItemAdded;
                activeViewEvent.ItemDeleted += OnItemDelete;
                IEditEvents_Event editEvent = (IEditEvents_Event)ArcMap.Editor;

                editEvent.OnStartEditing += _observPointsController.OnStartEditing;
                editEvent.OnStopEditing += _observPointsController.OnStopEditing;


                editEvent.OnCreateFeature += _observPointsController.OnCreateFeature;
                editEvent.OnDeleteFeature += _observPointsController.OnDeleteFeature;
            };

            //ArcMap.Events.NewDocument += delegate ()
            //{
            //    IActiveViewEvents_Event activeViewEvent = (IActiveViewEvents_Event)ActiveView;
            //    //activeViewEvent.SelectionChanged += OnContentsChanged;
            //    //activeViewEvent.ItemAdded += OnItemAdded;
            //    //OnContentsChanged();
            //};

            log.InfoEx("> SubscribeForEvents END");
        }

        /// <summary>
        /// Host object of the dockable window
        /// </summary>
        private object Hook
        {
            get;
            set;
        }


        #region

        public ValuableObservPointFieldsEnum GetFilter
        {
            get
            {
                var result = ValuableObservPointFieldsEnum.All;

                if (chckFilterAffiliation.Checked)
                {
                    result = result | ValuableObservPointFieldsEnum.Affiliation;
                }
                if (chckFilterDate.Checked)
                {
                    result = result | ValuableObservPointFieldsEnum.Date;
                }

                if (chckFilterType.Checked)
                {
                    result = result | ValuableObservPointFieldsEnum.Type;
                }

                return result;
            }
        }

        public string ObservationPointsFeatureClass => VisibilityManager.ObservPointFeature;

        public IEnumerable<string> GetTypes
        {
            get
            {
                return _observPointsController.GetObservationPointMobilityTypes();
            }
        }
        public IEnumerable<string> GetAffiliation
        {
            get
            {
                return _observPointsController.GetObservationPointTypes();
            }
        }

        public void FillObservationPointList(IEnumerable<ObservationPoint> observationPoints, ValuableObservPointFieldsEnum filter)
        {
            log.InfoEx("> FillObservationPointList START");

            var selected = dgvObservationPoints.SelectedRows.Count > 0 ? dgvObservationPoints.SelectedRows[0].Index : 0;
            dgvObservationPoints.CurrentCell = null;

            if (observationPoints != null && observationPoints.Any())
            {
                var ItemsToShow = observationPoints.Select(
                    op => new ObservPointGui
                    {
                        Title = op.Title,
                        Type = LocalizationContext.Instance.MobilityTypes[op.ObservationPointMobilityType],
                        Affiliation = LocalizationContext.Instance.AffiliationTypes[op.ObservationPointAffiliationType],
                        Date = op.Dto.Value.ToString(Helper.DateFormatSmall),
                        Id = op.Objectid
                    }).OrderBy(l => l.Title).ToArray();

                dgvObservationPoints.DataSource = ItemsToShow;// _observPointGuis01;

                SetDataGridView();
                DisplaySelectedColumns(filter);

                dgvObservationPoints.Update();
                if (selected > dgvObservationPoints.Rows.Count - 1)
                {
                    selected = dgvObservationPoints.Rows.Count - 1;
                }
                dgvObservationPoints.Rows[selected].Selected = true;
            }

            log.InfoEx("> FillObservationPointList END");
        }

       

        public void FillObservationObjectsList(IEnumerable<ObservationObject> observationObjects)
        {
            if (observationObjects.Any())
            {
                var itemsToShow = observationObjects.Select(i => new ObservObjectGui
                {
                    ObjectID = i.ObjectId,
                    Title = i.Title,
                    Created = i.DTO,
                    Id = i.Id,
                    Affiliation = _observPointsController.GetObservObjectsTypeString(i.ObjectType),
                    Group = i.Group
                }).OrderBy(l => l.Title).ToArray();

                dgvObservObjects.CurrentCell = null;
                dgvObservObjects.DataSource = itemsToShow;

                SetObservObjectsTableView();
                DisplayObservObjectsSelectedColumns();
                dgvObservObjects.Rows[0].Selected = true;
            }
        }

        public void ChangeRecord(int id, ObservationPoint observationPoint)
        {
            var rowIndex = dgvObservationPoints.SelectedRows[0].Index;
            var source = dgvObservationPoints.DataSource as ObservPointGui[];
            var pointGui = source.FirstOrDefault(point => point.Id == id);

            pointGui.Title = observationPoint.Title;
            pointGui.Type = LocalizationContext.Instance.MobilityTypes[observationPoint.ObservationPointMobilityType];
            pointGui.Affiliation = LocalizationContext.Instance.AffiliationTypes[observationPoint.ObservationPointAffiliationType];
            pointGui.Date = observationPoint.Dto.Value.ToString(Helper.DateFormatSmall);

            dgvObservationPoints.Refresh();
            UpdateFilter(dgvObservationPoints.Rows[rowIndex]);
            _isFieldsChanged = false;
        }

        public void AddRecord(ObservationPoint observationPoint)
        {
            var source = dgvObservationPoints.DataSource as ObservPointGui[];
            var sourceList = new List<ObservPointGui>(source);
            sourceList.Add(new ObservPointGui
            {
                Title = observationPoint.Title,
                Type = observationPoint.Type,
                Affiliation = observationPoint.Affiliation,
                Date = observationPoint.Dto.Value.ToShortDateString(),
                Id = observationPoint.Objectid
            });

            dgvObservationPoints.DataSource = sourceList.ToArray();
            dgvObservationPoints.Refresh();
            FilterData();

            if (dgvObservationPoints.Rows[dgvObservationPoints.RowCount - 1].Visible)
            {
                dgvObservationPoints.Rows[dgvObservationPoints.RowCount - 1].Selected = true;
            }
        }

        public void FillVisibilitySessionsList(IEnumerable<VisibilityTask> visibilitySessions, bool isNewSessionAdded, string newTaskId)
        {
            if (visibilitySessions.Any())
            {
       //         dgvVisibilitySessions.Rows.Clear();
                _visibilitySessionsGui = new List<VisibilityTasknGui>();

                foreach (var session in visibilitySessions)
                {
                    string state;

                    if (!session.Started.HasValue)
                    {
                        state = _visibilitySessionsController.GetStringForStateType(VisibilityTaskStateEnum.Pending);
                    }
                    else if (!session.Finished.HasValue)
                    {
                        state = _visibilitySessionsController.GetStringForStateType(VisibilityTaskStateEnum.Calculated);
                    }
                    else
                    {
                        state = _visibilitySessionsController.GetStringForStateType(VisibilityTaskStateEnum.Finished);
                    }

                    _visibilitySessionsGui.Add(new VisibilityTasknGui
                    {
                        Id = session.Id,
                        Name = session.Name,
                        Created = session.Created.Value,
                        State = state
                    });
                }

                dgvVisibilitySessions.CurrentCell = null;
                dgvVisibilitySessions.DataSource = _visibilitySessionsGui;
                SetVisibilitySessionsTableView();

                var lastRow = dgvVisibilitySessions.Rows[dgvVisibilitySessions.RowCount - 1];

                if (cmbStateFilter.SelectedItem.ToString() != _visibilitySessionsController.GetStringForStateType(VisibilityTaskStateEnum.All))
                {
                    FilterVisibilityList();
                }
                else
                {
                    dgvVisibilitySessions.Rows[0].Selected = true;
                }

                if (isNewSessionAdded && !String.IsNullOrEmpty(newTaskId))
                {
                    var newRow = dgvVisibilitySessions.Rows[0];

                    foreach (DataGridViewRow row in dgvVisibilitySessions.Rows)
                    {
                        if (row.Cells["Id"].Value.Equals(newTaskId))
                        {
                            newRow = row;
                        }
                    }

                    if (newRow.Visible)
                    {
                        newRow.Selected = true;
                        dgvVisibilitySessions.CurrentCell = newRow.Cells[1];
                    }
                }
            }
        }

        public void UpdateResultsTree()
        {
            _visibilitySessionsController.UpdateVisibilityResultsTree();
        }

        public void RemoveSessionFromList(string id)
        {

            _visibilitySessionsGui.Remove(_visibilitySessionsGui.First(session => session.Id == id));

            dgvVisibilitySessions.Refresh();

            if (cmbStateFilter.SelectedItem.ToString() != _visibilitySessionsController.GetStringForStateType(VisibilityTaskStateEnum.All))
            {
                FilterVisibilityList();
            }
            else
            {
                if (dgvVisibilitySessions.RowCount > 0)
                {
                    dgvVisibilitySessions.Rows[0].Selected = true;
                }
            }
        }


        private void OnSelectObserbPoint()
        {

        }

        private void OnItemAdded(object item)
        {
            log.DebugEx("> OnItemAdded START");

            IsPointFieldsEnabled = _observPointsController.IsObservPointsExists();
            EnableObservPointsControls();
            UpdateObservPointsList();
            SetObservObjectsControlsState(_observPointsController.IsObservObjectsExists());

            log.DebugEx("> OnItemAdded END");
        }

        private void OnContentsChanged()
        {
            log.DebugEx("> OnContentsChanged START");

            IsPointFieldsEnabled = _observPointsController.IsObservPointsExists();
            EnableObservPointsControls();
            //SetCoordDefaultValues();
            UpdateObservPointsList();
            SetObservObjectsControlsState(_observPointsController.IsObservObjectsExists());

            log.DebugEx("> OnContentsChanged END");
        }

        private void OnItemDelete(object item)
        {
            log.DebugEx("> OnItemDelete START");

            OnContentsChanged();

            log.DebugEx("> OnItemDelete END");
        }

        #endregion

        public IActiveView ActiveView => ArcMap.Document.ActiveView;

        /// <summary>
        /// Implementation class of the dockable window add-in. It is responsible for 
        /// creating and disposing the user interface class of the dockable window.
        /// </summary>
        public class AddinImpl : ESRI.ArcGIS.Desktop.AddIns.DockableWindow
        {
            private DockableWindowMilSpaceMVisibilitySt m_windowUI;

            public AddinImpl()
            {
            }

            internal DockableWindowMilSpaceMVisibilitySt UI
            {
                get { return m_windowUI; }
            }

            protected override IntPtr OnCreateChild()
            {
                var controller = new ObservationPointsController(ArcMap.Document, ArcMap.ThisApplication);

                m_windowUI = new DockableWindowMilSpaceMVisibilitySt(this.Hook, controller);
                return m_windowUI.Handle;
            }

            protected override void Dispose(bool disposing)
            {
                if (m_windowUI != null)
                    m_windowUI.Dispose(disposing);

                base.Dispose(disposing);
            }

        }

        internal void ArcMap_OnMouseDown(int x, int y)
        {
            if (!(this.Hook is IApplication arcMap) || !(arcMap.Document is IMxDocument currentDocument)) return;

            IPoint resultPoint = new Point();

            resultPoint = (currentDocument.FocusMap as IActiveView).ScreenDisplay.DisplayTransformation.ToMapPoint(x, y);
            resultPoint.ID = dgvObservationPoints.RowCount + 1;

            resultPoint.Project(EsriTools.Wgs84Spatialreference);

            xCoord.Text = resultPoint.X.ToString();
            yCoord.Text = resultPoint.Y.ToString();
            SavePoint();
        }

        internal void ArcMap_OnMouseMove(int x, int y)
        {
            //Place Mouce Move logic here if needed
        }

        private void InitializeData()
        {
            log.InfoEx("> InitializeData START");

            InitilizeObservPointsData();
            PopulateObservObjectsComboBoxes();
            PopulateVisibilityComboBoxes();
            SetVisibilityResultsButtonsState(false);

            log.InfoEx("> InitializeData END");
        }

        #region ObservationPointsPrivateMethods

        private void UpdateObservPointsList()
        {
            log.DebugEx("> UpdateObservPointsList START. IsPointFieldsEnabled:{0}", IsPointFieldsEnabled.ToString());
            if (IsPointFieldsEnabled)
            {
                _observPointsController.UpdateObservationPointsList();
            }
            else
            {
                ClearObservPointsData();
            }
            log.DebugEx("> UpdateObservPointsList END");
        }

        private void ClearObservPointsData()
        {
            dgvObservationPoints.DataSource = null;
            SetDefaultValues();
        }

        private void SetDataGridView()
        {
            dgvObservationPoints.Columns["Title"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvObservationPoints.Columns["Type"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dgvObservationPoints.Columns["Affiliation"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dgvObservationPoints.Columns["Date"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;

            dgvObservationPoints.Columns["Title"].HeaderText = LocalizationContext.Instance.NameHeaderText;
            dgvObservationPoints.Columns["Type"].HeaderText = LocalizationContext.Instance.TypeHeaderText;
            dgvObservationPoints.Columns["Affiliation"].HeaderText = LocalizationContext.Instance.AffiliationHeaderText;
            dgvObservationPoints.Columns["Date"].HeaderText = LocalizationContext.Instance.DateHeaderText;

            dgvObservationPoints.ColumnHeaderMouseClick += DgvObservationPoints_ColumnHeaderMouseClick;

            dgvObservationPoints.Columns["Id"].Visible = false;
        }

        private void DgvObservationPoints_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {

            ValuableObservPointSortFieldsEnum sortColumn = ValuableObservPointSortFieldsEnum.Name;
            if (e.ColumnIndex == 2)
            {
                sortColumn = ValuableObservPointSortFieldsEnum.Type;
            }
            else if (e.ColumnIndex == 3)
            {
                sortColumn = ValuableObservPointSortFieldsEnum.Affiliation;
            }
            else if (e.ColumnIndex == 4)
            {
                sortColumn = ValuableObservPointSortFieldsEnum.Date;
            }
            observatioPointsSordDirection = !observatioPointsSordDirection;

            var source = dgvObservationPoints.DataSource as ObservPointGui[];
            curObservPointsSorting = sortColumn;

            var sorted = _observPointsController.SortObservationPoints(source.ToArray(), sortColumn, observatioPointsSordDirection);
            dgvObservationPoints.DataSource = sorted.ToArray();
            FilterData();
            dgvObservationPoints.Refresh();
        }

        private void DisplaySelectedColumns(ValuableObservPointFieldsEnum filter)
        {
            dgvObservationPoints.Columns["Affiliation"].Visible = chckFilterAffiliation.Checked;
            dgvObservationPoints.Columns["Type"].Visible = chckFilterType.Checked;
            dgvObservationPoints.Columns["Date"].Visible = chckFilterDate.Checked;
        }

        private void FilterData()
        {
            if (dgvObservationPoints.RowCount == 0)
            {
                return;
            }

            dgvObservationPoints.CurrentCell = null;

            if (dgvObservationPoints.SelectedRows.Count > 0)
            {
                dgvObservationPoints.SelectedRows[0].Selected = false;
            }

            foreach (DataGridViewRow row in dgvObservationPoints.Rows)
            {
                CheckRowForFilter(row);
            }

            if (dgvObservationPoints.FirstDisplayedScrollingRowIndex != -1)
            {
                dgvObservationPoints.Rows[dgvObservationPoints.FirstDisplayedScrollingRowIndex].Selected = true;
                if (!IsPointFieldsEnabled) EnableObservPointsControls();
            }
            else
            {
                EnableObservPointsControls(true);
            }

        }
      
        private void CheckRowForFilter(DataGridViewRow row)
        {
            if (cmbAffiliation.SelectedItem != null
                && cmbAffiliation.SelectedItem.ToString() != _observPointsController.GetAllAffiliationType())
            {
                row.Visible = (row.Cells["Affiliation"].Value.ToString() == cmbAffiliation.SelectedItem.ToString());
                if (!row.Visible) return;
            }

            if (cmbObservPointType.SelectedItem != null
                && cmbObservPointType.SelectedItem.ToString() != _observPointsController.GetAllMobilityType())
            {
                row.Visible = (row.Cells["Type"].Value.ToString() == cmbObservPointType.SelectedItem.ToString());
                return;
            }

            row.Visible = true;
        }

        private void InitilizeObservPointsData()
        {
            log.InfoEx("> InitilizeObservPointsData START");

            cmbObservPointType.Items.Clear();
            cmbObservTypesEdit.Items.Clear();
            var filters = new List<string>();
            filters.AddRange(GetTypes.ToArray());

            cmbObservPointType.Items.AddRange(filters.ToArray());
            cmbObservPointType.Items.Add(_observPointsController.GetAllMobilityType());
            cmbObservTypesEdit.Items.AddRange(GetTypes.ToArray());

            filters = new List<string>();

            filters.AddRange(GetAffiliation.ToArray());
            cmbAffiliation.Items.Clear();
            cmbAffiliationEdit.Items.Clear();

            cmbAffiliation.Items.AddRange(filters.ToArray());

            cmbAffiliation.Items.Add(_observPointsController.GetAllAffiliationType());
            cmbAffiliationEdit.Items.AddRange(GetAffiliation.ToArray());

            SetDefaultValues();

            log.InfoEx("> InitilizeObservPointsData END");
        }

        private void SetDefaultValues()
        {
            _isDropDownItemChangedManualy = false;

            cmbObservTypesEdit.SelectedItem = ObservationPointMobilityTypesEnum.Stationary.ToString();
            cmbAffiliationEdit.SelectedItem = ObservationPointTypesEnum.Enemy.ToString();
            cmbObservPointType.SelectedItem = _observPointsController.GetAllMobilityType();
            cmbAffiliation.SelectedItem = _observPointsController.GetAllAffiliationType();

            _isDropDownItemChangedManualy = true;

            azimuthE.Text = ObservPointDefaultValues.AzimuthEText;
            azimuthB.Text = ObservPointDefaultValues.AzimuthBText;
            heightCurrent.Text = ObservPointDefaultValues.RelativeHeightText;
            heightMin.Text = ObservPointDefaultValues.HeightMinText;
            heightMax.Text = ObservPointDefaultValues.HeightMaxText;
            observPointName.Text = ObservPointDefaultValues.ObservPointNameText;
            angleOFViewMin.Text = ObservPointDefaultValues.AngleOFViewMinText;
            angleOFViewMax.Text = ObservPointDefaultValues.AngleOFViewMaxText;

            //angleFrameH.Text = ObservPointDefaultValues.AngleFrameHText;
            //angleFrameV.Text = ObservPointDefaultValues.AngleFrameVText;
            //cameraRotationH.Text = ObservPointDefaultValues.CameraRotationHText;
            //cameraRotationV.Text = ObservPointDefaultValues.CameraRotationVText;
            //azimuthMainAxis.Text = ObservPointDefaultValues.AzimuthMainAxisText;


            observPointDate.Text = DateTime.Now.ToString(Helper.DateFormatSmall);
            observPointCreator.Text = Environment.UserName;
        }

        private void SetCoordDefaultValues()
        {
            var centerPoint = _observPointsController.GetEnvelopeCenterPoint(ArcMap.Document.ActiveView.Extent);
            xCoord.Text = centerPoint.X.ToString();
            yCoord.Text = centerPoint.Y.ToString();
        }

        private void OnFieldChanged(object sender, EventArgs e)
        {
            if (!_isFieldsChanged || !IsPointFieldsEnabled)
            {
                return;
            }

            var selectedPoint = _observPointsController.GetObservPointById(_selectedPointId);

            if (!FieldsValidation(sender, selectedPoint))
            {
                ((TextBox)sender).Focus();
            }
            if (!FieldsEQ(selectedPoint))
            {
                _observPointsController.UpdateObservPoint(
                    GetObservationPoint(),
                    VisibilityManager.ObservPointFeature,
                    ActiveView,
                    selectedPoint.Objectid
                    );
                UpdateObservPointsList();
            }

        }

        private bool FieldsEQ(ObservationPoint point)
        {
            try
            {
                if (point == selectedPointMEM)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private bool FieldsValidation(object sender, ObservationPoint point)
        {
            try
            {
                var textBox = (TextBox)sender;

                switch (textBox.Name)
                {
                    case "txtMinDistance":
                        double minValue;
                        string sMsgTextMinValue = LocalizationContext.Instance.FindLocalizedElement(
                                "MsgValueLessThenZerro", "Значення повинно бути більше нуля.");

                        if (!Helper.TryParceToDouble(txtMinDistance.Text, out minValue))
                        {
                            MessageBox.Show(
                                    sMsgTextMinValue,
                                    LocalizationContext.Instance.MsgBoxErrorHeader,
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                            return false;
                        }
                        if (minValue <= 0)
                        {
                            MessageBox.Show(
                                sMsgTextMinValue,
                                LocalizationContext.Instance.MsgBoxErrorHeader,
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                            return false;
                        }

                        txtMinDistance.Text = minValue.ToString();
                        break;
                    case "txtMaxDistance":

                        double maxValue;
                        string sMsgTextMaxValue = LocalizationContext.Instance.FindLocalizedElement(
                                                                                  "MsgValueLessThenZerro",
                                                                                  "Значення бовинно бути більше нуля.");
                        if (!Helper.TryParceToDouble(txtMaxDistance.Text, out maxValue))
                        {
                            MessageBox.Show(
                               sMsgTextMaxValue,
                               LocalizationContext.Instance.MsgBoxErrorHeader,
                               MessageBoxButtons.OK,
                               MessageBoxIcon.Error);
                            return false;
                        }
                        if (maxValue <= 0)
                        {

                            MessageBox.Show(
                                sMsgTextMaxValue,
                                LocalizationContext.Instance.MsgBoxErrorHeader,
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                            return false;
                        }

                        txtMaxDistance.Text = maxValue.ToString();
                        break;
                    case "xCoord":

                        if (!Regex.IsMatch(xCoord.Text, @"^([-]?[\d]{1,2}\,\d+)$"))
                        {
                            string sMsgText = LocalizationContext.Instance.FindLocalizedElement(
                                "MsgInvalidCoordinatesDD",
                                "недійсні дані \nПотрібні коордінати представлені у СК WGS-84, десяткові градуси");
                            MessageBox.Show(
                                sMsgText,
                                LocalizationContext.Instance.MsgBoxErrorHeader,
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);

                            xCoord.Text = point.X.ToString();

                            return false;
                        }
                        else
                        {
                            //var x = Convert.ToDouble(xCoord.Text);
                            //var y = Convert.ToDouble(yCoord.Text);
                        }

                        return true;

                    case "yCoord":

                        if (!Regex.IsMatch(yCoord.Text, @"^([-]?[\d]{1,2}\,\d+)$"))
                        {
                            string sMsgText = LocalizationContext.Instance.FindLocalizedElement(
                                "MsgInvalidCoordinatesDD",
                                "Недійсні дані \nПотрібні коордінати представлені у СК WGS-84, десяткові градуси");
                            MessageBox.Show(
                                sMsgText,
                                LocalizationContext.Instance.MsgBoxErrorHeader,
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);

                            yCoord.Text = point.Y.ToString();

                            return false;
                        }
                        else
                        {
                            //var x = Convert.ToDouble(xCoord.Text);
                            //var y = Convert.ToDouble(yCoord.Text);
                        }

                        return true;

                    case "angleOFViewMin":
                        return ValidateRange(angleOFViewMin, point.AngelMinH.ToString(), -90, 0);

                    case "angleOFViewMax":
                        return ValidateRange(angleOFViewMax, point.AngelMaxH.ToString(), 0, 90);

                    case "azimuthB":
                        return ValidateAzimuth(textBox, point.AzimuthStart.ToString());

                    case "azimuthE":
                        return ValidateAzimuth(textBox, point.AzimuthEnd.ToString());

                    case "azimuthMainAxis":
                        return ValidateAzimuth(textBox, point.AzimuthMainAxis.ToString());

                    case "cameraRotationH":
                        return ValidateAzimuth(textBox, point.AngelCameraRotationH.ToString());

                    case "cameraRotationV":
                        return ValidateAzimuth(textBox, point.AngelCameraRotationV.ToString());

                    case "heightCurrent":
                        var currentHeight = ValidateHeight(textBox, point.RelativeHeight.ToString());

                        if (currentHeight != -1)
                        {
                            var minHeight = Convert.ToDouble(heightMin.Text);
                            var maxHeight = Convert.ToDouble(heightMax.Text);

                            if (currentHeight > maxHeight)
                            {
                                heightMax.Text = currentHeight.ToString();
                            }

                            if (currentHeight < minHeight)
                            {
                                heightMin.Text = currentHeight.ToString();
                            }

                            return true;
                        }

                        return false;

                    case "heightMin":
                        var minHeightChanged = ValidateHeight(textBox, point.AvailableHeightLover.ToString());

                        if (minHeightChanged != -1)
                        {
                            var curHeight = Convert.ToDouble(heightCurrent.Text);
                            var maxHeight = Convert.ToDouble(heightMax.Text);

                            if (minHeightChanged > curHeight)
                            {
                                heightCurrent.Text = minHeightChanged.ToString();
                            }

                            if (minHeightChanged > maxHeight)
                            {
                                heightMax.Text = minHeightChanged.ToString();
                            }

                            return true;
                        }

                        return false;

                    case "heightMax":
                        var maxHeightChanged = ValidateHeight(textBox, point.AvailableHeightUpper.ToString());

                        if (maxHeightChanged != -1)
                        {
                            var curHeight = Convert.ToDouble(heightCurrent.Text);
                            var minHeight = Convert.ToDouble(heightMin.Text);

                            if (maxHeightChanged < curHeight)
                            {
                                heightCurrent.Text = maxHeightChanged.ToString();
                            }

                            if (maxHeightChanged < minHeight)
                            {
                                heightMax.Text = maxHeightChanged.ToString();
                            }

                            return true;
                        }

                        return false;

                    default:

                        return true;
                }

                return true;
            }

            catch (Exception ex) { return false; }
        }

        private bool ValidateAzimuth(TextBox azimuthTextBox, string defaultValue)
        {
            return ValidateRange(azimuthTextBox, defaultValue, 0, 360);
        }

        private bool ValidateRange(TextBox textBox, string defaultValue, double lowValue, double upperValue)
        {
            double value;

            if (Helper.TryParceToDouble(textBox.Text, out value))
            {
                if (value >= lowValue && value <= upperValue)
                {
                    textBox.Text = value.ToString();
                    return true;
                }
            }

            textBox.Text = defaultValue;
            string sMsgText = LocalizationContext.Instance.FindLocalizedElement(
                "MsgErrorDataRange",
                $"Invalid data.\nЗначення має бути від {lowValue} до {upperValue}");
            MessageBox.Show(
                sMsgText,
                LocalizationContext.Instance.MsgBoxErrorHeader,
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);

            return false;
        }

        private double ValidateHeight(TextBox heightTextBox, string defaultValue)
        {
            double height;

            if (Double.TryParse(heightTextBox.Text, out height))
            {
                if (height >= 0)
                {
                    return height;
                }

                string sMsgText = LocalizationContext.Instance.FindLocalizedElement(
                    "MsgErrorValueLassNul",
                    "Недійсні дані \nЗначення не має бути меньш за 0");
                MessageBox.Show(
                    sMsgText,
                    LocalizationContext.Instance.MsgBoxErrorHeader,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            else
            {
                string sMsgText = LocalizationContext.Instance.FindLocalizedElement(
                    "MsgErrorValueNotNumeric",
                    "Недійсні дані \nПотрыбно вказати число число");
                MessageBox.Show(
                    sMsgText,
                    LocalizationContext.Instance.MsgBoxErrorHeader,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }

            heightTextBox.Text = defaultValue;

            return -1;
        }

        private void EnableObservPointsControls(bool isAllDisabled = false)
        {
            log.DebugEx("> EnableObservPointsControls START. isAllDisabled:{0} IsPointFieldsEnabled:{1}",
                isAllDisabled.ToString(), IsPointFieldsEnabled.ToString());

            bool layerExists = IsPointFieldsEnabled;

            cmbAffiliationEdit.Enabled =
                cmbObservTypesEdit.Enabled =
                azimuthE.Enabled =
                azimuthB.Enabled =
                xCoord.Enabled =
                yCoord.Enabled =
                angleOFViewMin.Enabled =
                angleOFViewMax.Enabled =
                heightCurrent.Enabled =
                heightMin.Enabled =
                heightMax.Enabled =
                observPointName.Enabled =
                tlbCoordinates.Enabled =
                txtMaxDistance.Enabled =
                txtMinDistance.Enabled =
                tlbbShowPoint.Enabled =
                tlbbRemovePoint.Enabled =
                tlbbAddNewPoint.Enabled =
                (layerExists && !isAllDisabled);

            //= azimuthMainAxis.Enabled = cameraRotationH.Enabled = cameraRotationV.Enabled

            //angleFrameH.Enabled = angleFrameV.Enabled = false;
            observPointDate.Enabled = observPointCreator.Enabled = true;
            observPointDate.ReadOnly = observPointCreator.ReadOnly = true;

            tlbbAddObserPointLayer.Enabled = !layerExists || isAllDisabled;
            //btnAddLayerPS.Enabled = !layerExists;

            log.DebugEx("> EnableObservPointsControls END");
        }

        private void RemovePoint()
        {
            string sMsgText = LocalizationContext.Instance.FindLocalizedElement(
                "MsgQueryDeletePointPS",
                "Ви дійсно бажаєте видалити точку (ПС)?");
            var result = MessageBox.Show(
                sMsgText,
                LocalizationContext.Instance.MsgBoxInfoHeader,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                var rowIndex = dgvObservationPoints.SelectedRows[0].Index;

                _observPointsController.RemoveObservPoint(VisibilityManager.ObservPointFeature, ActiveView, _selectedPointId);
                var source = dgvObservationPoints.DataSource as ObservPointGui[];
                var sourceList = new List<ObservPointGui>(source);
                sourceList.Remove(sourceList.First(point => point.Id == _selectedPointId));

                dgvObservationPoints.DataSource = sourceList.ToArray();

                if (rowIndex < dgvObservationPoints.RowCount)
                {
                    UpdateFilter(dgvObservationPoints.Rows[rowIndex]);
                }
            }
        }

        private void SavePoint()
        {
            var selectedPoint = _observPointsController.GetObservPointById(_selectedPointId);

            _observPointsController.UpdateObservPoint(
                GetObservationPoint(),
                VisibilityManager.ObservPointFeature,
                ActiveView,
                selectedPoint.Objectid
                );
        }

        //private void CreateNewPoint(ObservationPoint point)
        private void CreateNewPoint()
        {
            _observPointsController.AddPoint(VisibilityManager.ObservPointFeature, ActiveView);
        }

        private ObservationPoint GetObservationPoint()
        {
            var affiliationType =
                LocalizationContext.Instance.AffiliationTypes.First(v => v.Value.Equals(cmbAffiliationEdit.SelectedItem));
            var mobilityType =
                LocalizationContext.Instance.MobilityTypes.First(v => v.Value.Equals(cmbObservTypesEdit.SelectedItem));

            double xdd = 0;
            double ydd = 0;
            if (!Helper.TryParceToDouble(xCoord.Text, out xdd) || !Helper.TryParceToDouble(yCoord.Text, out ydd))
            {
                //string sMsgText = LocalizationContext.Instance.FindLocalizedElement(
                //    "MsgInvalidCoordinatesDD", "недійсні дані \nПотрібні коордінати представлені у СК WGS-84, десяткові градуси");
                //MessageBox.Show(
                //    sMsgText,LocalizationContext.Instance.MsgBoxErrorHeader,MessageBoxButtons.OK, MessageBoxIcon.Error);
                //return null;
                xdd = ydd = 25.2525252525;
            }

            ObservationPoint op = new ObservationPoint()
            {
                Title = observPointName.Text,
                Type = mobilityType.Key.ToString(),
                Affiliation = affiliationType.Key.ToString(),

                X = xdd,
                Y = ydd,

                AngelMaxH = Convert.ToDouble(angleOFViewMax.Text),
                AngelMinH = Convert.ToDouble(angleOFViewMin.Text),
                //AngelCameraRotationH = Convert.ToDouble(cameraRotationH.Text),
                //AngelCameraRotationV = Convert.ToDouble(cameraRotationV.Text),
                RelativeHeight = Convert.ToDouble(heightCurrent.Text),
                AvailableHeightLover = Convert.ToDouble(heightMin.Text),
                AvailableHeightUpper = Convert.ToDouble(heightMax.Text),
                AzimuthStart = Convert.ToDouble(azimuthB.Text),
                AzimuthEnd = Convert.ToDouble(azimuthE.Text),
                //AzimuthMainAxis = Convert.ToDouble(azimuthMainAxis.Text),
                InnerRadius = Convert.ToDouble(txtMinDistance.Text),
                OuterRadius = Convert.ToDouble(txtMaxDistance.Text),

                Dto = Convert.ToDateTime(observPointDate.Text),
                Operator = observPointCreator.Text,
            };

            return op;
        }

        private void UpdateFilter(DataGridViewRow row)
        {
            dgvObservationPoints.CurrentCell = null;

            if (dgvObservationPoints.SelectedRows.Count > 0)
            {
                dgvObservationPoints.SelectedRows[0].Selected = false;
            }

            CheckRowForFilter(row);

            if (row.Visible)
            {
                row.Selected = true;
            }
            else
            {
                if (dgvObservationPoints.FirstDisplayedScrollingRowIndex != -1)
                {
                    dgvObservationPoints.Rows[dgvObservationPoints.FirstDisplayedScrollingRowIndex].Selected = true;
                    if (!IsPointFieldsEnabled) EnableObservPointsControls();
                }
                else
                {
                    EnableObservPointsControls(true);
                }
            }
        }

        private void FillObservPointsFields(ObservationPoint selectedPoint)
        {
            _isDropDownItemChangedManualy = false;

            cmbObservTypesEdit.SelectedItem =
                _observPointsController.GetObservationPointMobilityTypeLocalized(selectedPoint.ObservationPointMobilityType);
            cmbAffiliationEdit.SelectedItem =
                _observPointsController.GetObservationPointTypeLocalized(selectedPoint.ObservationPointAffiliationType);

            _isDropDownItemChangedManualy = true;

            var FCPoint = _observPointsController.GetIPointObservPoint(selectedPoint.Objectid);
            var centerPoint = _observPointsController.GetEnvelopeCenterPoint(ArcMap.Document.ActiveView.Extent);

            xCoord.Text = 
                selectedPoint.X.HasValue ? 
                selectedPoint.X.Value.ToString("F5") :
                FCPoint != null ?
                FCPoint.X.ToString("F5") :
                centerPoint.X.ToString("F5");
            yCoord.Text = 
                selectedPoint.Y.HasValue ? 
                selectedPoint.Y.Value.ToString("F5") :
                FCPoint != null ?
                FCPoint.Y.ToString("F5") :
                centerPoint.Y.ToString("F5");

            azimuthB.Text =
                selectedPoint.AzimuthStart.HasValue ?
                selectedPoint.AzimuthStart.ToString() :
                ObservPointDefaultValues.AzimuthBText;
            azimuthE.Text =
                selectedPoint.AzimuthEnd.HasValue ?
                selectedPoint.AzimuthEnd.ToString() :
                ObservPointDefaultValues.AzimuthEText;
            heightCurrent.Text =
                selectedPoint.RelativeHeight.HasValue ?
                selectedPoint.RelativeHeight.ToString() :
                ObservPointDefaultValues.RelativeHeightText;
            heightMin.Text = selectedPoint.AvailableHeightLover.ToString();
            heightMax.Text = selectedPoint.AvailableHeightUpper.ToString();
            observPointName.Text = selectedPoint.Title;
            angleOFViewMin.Text =
                selectedPoint.AngelMinH.HasValue ?
                selectedPoint.AngelMinH.ToString() :
                ObservPointDefaultValues.AngleOFViewMinText;
            angleOFViewMax.Text = 
                selectedPoint.AngelMaxH.HasValue ? 
                selectedPoint.AngelMaxH.ToString() : 
                ObservPointDefaultValues.AngleOFViewMaxText;
            txtMinDistance.Text = 
                selectedPoint.InnerRadius.HasValue ? 
                selectedPoint.InnerRadius.ToString() : 
                ObservPointDefaultValues.DefaultRadiusText;
            txtMaxDistance.Text = 
                selectedPoint.OuterRadius.HasValue ? 
                selectedPoint.OuterRadius.ToString() : 
                ObservPointDefaultValues.DefaultRadiusText;

            observPointDate.Text = selectedPoint.Dto.Value.ToString(Helper.DateFormat);
            observPointCreator.Text = selectedPoint.Operator;
        }

        #endregion

        #region VisibilitySessionsPrivateMethods

        private void SetVisibilitySessionsTableView()
        {
            dgvVisibilitySessions.Columns["Id"].Visible = false;
            dgvVisibilitySessions.Columns["Name"].HeaderText =
                LocalizationContext.Instance.FindLocalizedElement("HeaderNameGridSessionResult", "Назва");
            dgvVisibilitySessions.Columns["Name"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvVisibilitySessions.Columns["Created"].HeaderText =
                LocalizationContext.Instance.FindLocalizedElement("HeaderCreatedGridSessionResult", "Строрено");
            dgvVisibilitySessions.Columns["Created"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dgvVisibilitySessions.Columns["Created"].ValueType = typeof(DateTime);
            dgvVisibilitySessions.Columns["Created"].DefaultCellStyle.Format = Helper.DateFormatSmall;
            dgvVisibilitySessions.Columns["State"].HeaderText =
                LocalizationContext.Instance.FindLocalizedElement("HeaderStateGridSessionResult", "Стан");
            dgvVisibilitySessions.Columns["State"].Width = 100;
        }

        private void dgvVisibilitySessions_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            VeluableTaskSortFieldsEnum sortColumn = VeluableTaskSortFieldsEnum.Title;
            //TBD: Create dictionary to get this values
            if (e.ColumnIndex == 2)
            {
                sortColumn = VeluableTaskSortFieldsEnum.Created;
            }
            if (e.ColumnIndex == 3)
            {
                sortColumn = VeluableTaskSortFieldsEnum.State;
            }

            tasksSordDirection = !tasksSordDirection;

            var source = dgvVisibilitySessions.DataSource as List<VisibilityTasknGui>;
            
            var sorted = _observPointsController.SortTasks(source, sortColumn, tasksSordDirection);
            dgvVisibilitySessions.DataSource = sorted.ToList();
            FilterVisibilityList();
            dgvVisibilitySessions.Refresh();
        }

        private void SetVisibilitySessionsController()
        {
            var controller = new VisibilitySessionsController(ArcMap.Document, ArcMap.ThisApplication);
            controller.SetView(this);
            _visibilitySessionsController = controller;
        }

        private void FillVisibilitySessionFields(VisibilityTask task)
        {
            tbVisibilitySessionName.Text = task.Name;
            tbVisibilitySessionCreator.Text = task.UserName;
            tbVisibilitySessionCreated.Text = task.Created.Value.ToString(Helper.DateFormat);
            tbVisibilitySessionStarted.Text =
                task.Started.HasValue ? task.Started.Value.ToString(Helper.DateFormat) : string.Empty;
            tbVisibilitySessionFinished.Text =
                task.Finished.HasValue ? task.Finished.Value.ToString(Helper.DateFormat) : string.Empty;
            txtTaskLog.Text = task.TaskLog;

            //wizardTask.Enabled = _observPointsController.IsObservObjectsExists() && _observPointsController.IsObservPointsExists();
            wizardTask.Enabled = _observPointsController.IsObservPointsExists();
        }

        private void PopulateVisibilityComboBoxes()
        {
            cmbStateFilter.Items.Clear();
            cmbStateFilter.Items.AddRange(_visibilitySessionsController.GetVisibilitySessionStateTypes().ToArray());
            cmbStateFilter.SelectedItem = _visibilitySessionsController.GetStringForStateType(VisibilityTaskStateEnum.All);
        }

        private void FilterVisibilityList()
        {
            if (dgvVisibilitySessions.RowCount == 0)
            {
                return;
            }

            dgvVisibilitySessions.CurrentCell = null;

            if (dgvVisibilitySessions.SelectedRows.Count > 0)
            {
                dgvVisibilitySessions.SelectedRows[0].Selected = false;
            }

            foreach (DataGridViewRow row in dgvVisibilitySessions.Rows)
            {
                row.Visible = row.Cells["State"].Value.ToString() == cmbStateFilter.SelectedItem.ToString()
                || cmbStateFilter.SelectedItem.ToString() == _visibilitySessionsController.GetStringForStateType(VisibilityTaskStateEnum.All);
            }

            if (dgvVisibilitySessions.FirstDisplayedScrollingRowIndex != -1)
            {
                dgvVisibilitySessions.Rows[dgvVisibilitySessions.FirstDisplayedScrollingRowIndex].Selected = true;
            }
            else
            {
                tlbVisibilitySessions.Buttons["removeTask"].Enabled = false;
            }
        }

        public bool RemoveSelectedSession()
        {
            var id = dgvVisibilitySessions.SelectedRows[0].Cells["Id"].Value.ToString();

            return _visibilitySessionsController.RemoveSession(id);
        }

        #endregion

        #region ObservationObjectsPrivateMethods

        private void SetObservObjectsTableView()
        {
            dgvObservObjects.Columns["ObjectId"].Visible = false;
            dgvObservObjects.Columns["Id"].Visible = false;

            dgvObservObjects.Columns["Title"].HeaderText =
                LocalizationContext.Instance.FindLocalizedElement("HeaderNameGridON", "Назва");
            dgvObservObjects.Columns["Title"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            dgvObservObjects.Columns["Created"].HeaderText =
                LocalizationContext.Instance.FindLocalizedElement("DateHeaderText", "Дата");
            dgvObservObjects.Columns["Created"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dgvObservObjects.Columns["Created"].ValueType = typeof(DateTime);
            dgvObservObjects.Columns["Created"].DefaultCellStyle.Format = Helper.DateFormatSmall;

            dgvObservObjects.Columns["Affiliation"].HeaderText =
                LocalizationContext.Instance.FindLocalizedElement("HeaderAfilGridON", "Належність");
            dgvObservObjects.Columns["Affiliation"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;

            dgvObservObjects.Columns["Group"].HeaderText =
                LocalizationContext.Instance.FindLocalizedElement("HeaderGroupGridON", "Група");
            dgvObservObjects.Columns["Group"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;

            dgvObservObjects.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dgvObservObjects.ColumnHeaderMouseClick += dgvObservationObjects_ColumnHeaderMouseClick;
        }

        private void dgvObservationObjects_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            VeluableObservObjectSortFieldsEnum sortColumn = VeluableObservObjectSortFieldsEnum.Id; 
            //TBD: cahnge it to Dictionary
            if (e.ColumnIndex == 2)
            {
                sortColumn = VeluableObservObjectSortFieldsEnum.Title;
            }else if (e.ColumnIndex == 3)
            {
                sortColumn = VeluableObservObjectSortFieldsEnum.Group;
            }
            else if (e.ColumnIndex == 4)
            {
                sortColumn = VeluableObservObjectSortFieldsEnum.Affiliation;
            }
            else if (e.ColumnIndex == 5)
            {
                sortColumn = VeluableObservObjectSortFieldsEnum.Date;
            }

            observatioObjectsSordDirection = !observatioObjectsSordDirection;

            var source = dgvObservObjects.DataSource as ObservObjectGui[];
            curObservObjectSorting = sortColumn;

            var sorted = _observPointsController.SortObservationObjects(source.ToArray(), sortColumn, observatioObjectsSordDirection);
            dgvObservObjects.DataSource = sorted.ToArray();
            CmbObservObjAffiliationFilter_SelectedIndexChanged(dgvObservObjects, null);
            dgvObservObjects.Refresh();
        }

        private void DisplayObservObjectsSelectedColumns()
        {
            dgvObservObjects.Columns["Affiliation"].Visible = chckObservObjAffiliation.Checked;
            dgvObservObjects.Columns["Title"].Visible = chckObservObjTitle.Checked;
            dgvObservObjects.Columns["Group"].Visible = chckObservObjGroup.Checked;
        }

        private void PopulateObservObjectsComboBoxes()
        {
            log.InfoEx("> PopulateObservObjectsComboBoxes START");

            cmbObservObjAffiliationFilter.Items.Clear();
            cmbObservObjAffiliation.Items.Clear();

            cmbObservObjAffiliationFilter.Items.AddRange(_observPointsController.GetObservationObjectTypes().ToArray());
            cmbObservObjAffiliationFilter.SelectedItem = _observPointsController.GetObservObjectsTypeString(ObservationObjectTypesEnum.All);

            cmbObservObjAffiliation.Items.AddRange(_observPointsController.GetObservationObjectTypes(false).ToArray());
            cmbObservObjAffiliation.SelectedItem = _observPointsController.GetObservObjectsTypeString(ObservationObjectTypesEnum.Enemy);

            log.InfoEx("> PopulateObservObjectsComboBoxes END");
        }

        private void FilterObservObjects()
        {
            if (dgvObservObjects.RowCount == 0)
            {
                return;
            }

            dgvObservObjects.CurrentCell = null;

            if (dgvObservObjects.SelectedRows.Count > 0)
            {
                dgvObservObjects.SelectedRows[0].Selected = false;
            }

            foreach (DataGridViewRow row in dgvObservObjects.Rows)
            {
                row.Visible = row.Cells["Affiliation"].Value.ToString() == cmbObservObjAffiliationFilter.SelectedItem.ToString()
                 || cmbObservObjAffiliationFilter.SelectedItem.ToString() == _observPointsController.GetObservObjectsTypeString(ObservationObjectTypesEnum.All);
            }

            if (dgvObservObjects.FirstDisplayedScrollingRowIndex != -1)
            {
                dgvObservObjects.Rows[dgvObservObjects.FirstDisplayedScrollingRowIndex].Selected = true;
            }
            else
            {
                ClearObservObjectFields();
            }
        }

        private void FillObservObjectFields(ObservationObject observObject)
        {
            tbObservObjTitle.Text = observObject.Title;
            tbObservObjGroup.Text = observObject.Group;
            tbObservObjCreator.Text = observObject.Creator;
            cmbObservObjAffiliation.SelectedItem = _observPointsController.GetObservObjectsTypeString(observObject.ObjectType);
            tbObservObjDate.Text = observObject.DTO.ToString(Helper.DateFormatSmall);
        }

        private void ClearObservObjectFields()
        {
            tbObservObjTitle.Text = string.Empty;
            tbObservObjGroup.Text = string.Empty;
            //cmbObservObjAffiliation.Text = string.Empty;
            tbObservObjDate.Text = string.Empty;
        }

        private void SetObservObjectsControlsState(bool isObservObjectsExist)
        {
            if (!isObservObjectsExist)
            {
                dgvObservObjects.Rows.Clear();
            }
            else
            {
                _observPointsController.UpdateObservObjectsList();

                if (cmbObservObjAffiliationFilter.Items.Count == 0)
                {
                    PopulateObservObjectsComboBoxes();
                }
            }

            //addNewObjectPanel.Enabled = isObservObjectsExist;
            cmbObservObjAffiliationFilter.Enabled = isObservObjectsExist;
            chckObservObjColumnsVisibilityPanel.Enabled = isObservObjectsExist;
            //tlbbAddObservObjLayer.Enabled = !isObservObjectsExist;

        }
        #endregion


        #region VisibilityResultsPrivateMethods

        private void SetVisibilityResultsButtonsState(bool enabled)
        {
            var isGroupedLayerExists = false;
            var isResultsShared = true;

            if (enabled)
            {
                isGroupedLayerExists =
                    _visibilitySessionsController.IsResultsLayerExist(tvResults.SelectedNode.Tag.ToString(), ActiveView);
                isResultsShared =
                    _visibilitySessionsController.IsResultsShared(tvResults.SelectedNode.Tag.ToString());
            }

            toolBarVisibleResults.Buttons["tlbbZoomToResultRaster"].Enabled = isGroupedLayerExists;
            // toolBarVisibleResults.Buttons["tlbbViewParamOnMap"].Enabled = enabled;
            toolBarVisibleResults.Buttons["toolBarButtonViewOnMap"].Enabled = enabled && !isGroupedLayerExists;
            toolBarVisibleResults.Buttons["tlbbFullDelete"].Enabled = enabled;
            toolBarVisibleResults.Buttons["toolBarButtonRemoveFromSeanse"].Enabled = enabled;
            toolBarVisibleResults.Buttons["tlbbShare"].Enabled = !isResultsShared;
        }

        #endregion

        #region VisibilitySessionsTree

        public void FillVisibilityResultsTree(IEnumerable<VisibilityCalcResults> visibilityResults)
        {
            tvResults.Nodes.Clear();

            var calcTypes = _visibilitySessionsController.GetCalcTypes();
            calcTypes.Remove(VisibilityCalcTypeEnum.None);

            foreach (var type in calcTypes)
            {
                tvResults.Nodes.Add(type.Key.ToString(), type.Value, (int)type.Key);
            }

            AddNewResultsToTree(visibilityResults);

        }

        private void AddNewResultsToTree(IEnumerable<VisibilityCalcResults> visibilityResults)
        {
            try
            {
                foreach (VisibilityCalcResults res in visibilityResults)
                {
                    var parentNode = tvResults.Nodes.Find(res.CalculationType.ToString(), false).FirstOrDefault();

                    if (parentNode == null)
                    {
                        throw new NullReferenceException();
                    }

                    TreeNode taskNode = new TreeNode(res.Name);
                    taskNode.ImageKey = "Stats.png";
                    taskNode.Tag = res.Id;
                    taskNode.Name = res.Id;

                    parentNode.Nodes.Add(taskNode);

                    foreach (var result in res.ValueableResults())
                    {
                        var img = _visibilitySessionsController.GetImgName(VisibilityCalcResults.GetResultTypeByName(result));

                        TreeNode resNode = new TreeNode(result);
                        resNode.ImageKey = img;
                        resNode.Tag = res.Id;

                        taskNode.Nodes.Add(resNode);
                    }
                }
            }
            catch (NullReferenceException e) { }
        }

        private void Node_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Action != TreeViewAction.Unknown)
            {
                SetVisibilityResultsButtonsState(e.Node.Parent != null);

                if (e.Node.Nodes.Count > 0)
                {
                    this.CheckAllChildNodes(e.Node, e.Node.Checked);
                }
            }
        }
        private void CheckAllChildNodes(TreeNode treeNode, bool nodeChecked)
        {
            foreach (TreeNode node in treeNode.Nodes)
            {
                node.Checked = nodeChecked;
                if (node.Nodes.Count > 0)
                {
                    // If the current node has child nodes call the CheckAllChildsNodes method recursively
                    this.CheckAllChildNodes(node, nodeChecked);
                }
            }
        }
        #endregion


        private void MainTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (mainTabControl.SelectedTab.Name == "tbpSessions")
            {
                if (dgvVisibilitySessions.DataSource == null)
                {
                    _visibilitySessionsController.UpdateVisibilitySessionsList();

                    if (dgvVisibilitySessions.RowCount == 0)
                    {
                        tlbVisibilitySessions.Buttons["removeTask"].Enabled = false;
                    }
                }
            }

            if (mainTabControl.SelectedTab.Name == "tbpObservObjects")
            {
                if (cmbObservObjAffiliationFilter.Items.Count == 0)
                {
                    SetObservObjectsControlsState(_observPointsController.IsObservObjectsExists());
                }
            }

            if (mainTabControl.SelectedTab.Name == "tbpVisibilityAreas")
            {
                if (tvResults.Nodes.Count == 0)
                {
                    _visibilitySessionsController.UpdateVisibilityResultsTree();
                }

            }
        }

        #region ObservationPointsTabEvents

        private void TlbObserPoints_ButtonClick(object sender, ToolBarButtonClickEventArgs e)
        {
            switch (e.Button.Name)
            {
                case "tlbbAddNewPoint":
                    CreateNewPoint();
                    break;

                case "tlbbRemovePoint":
                    RemovePoint();
                    break;

                case "tlbbShowPoint":
                    _observPointsController.ShowObservPoint(ActiveView, _selectedPointId);
                    break;

                case "tlbbAddObserPointLayer":
                    if (_observPointsController.AddObservPointsLayer())
                    {
                        tlbbAddObserPointLayer.Enabled = false;
                    }
                    break;
            }
        }

        private void TlbCoordinates_ButtonClick(object sender, ToolBarButtonClickEventArgs e)
        {
            switch (e.Button.Name)
            {
                case "tlbbGetCoord":

                    UID mapToolID = new UIDClass
                    {
                        Value = ThisAddIn.IDs.MapInteropTool
                    };
                    var documentBars = ArcMap.Application.Document.CommandBars;
                    var mapTool = documentBars.Find(mapToolID, false, false);

                    if (ArcMap.Application.CurrentTool?.ID?.Value != null
                        && ArcMap.Application.CurrentTool.ID.Value.Equals(mapTool.ID.Value))
                    {
                        ArcMap.Application.CurrentTool = null;
                    }
                    else
                    {
                        ArcMap.Application.CurrentTool = mapTool;
                    }
                    break;

                case "tlbbCopyCoord":
                    Clipboard.Clear();
                    string sCoord = $"{xCoord.Text} {yCoord.Text}";
                    Clipboard.SetText(sCoord.Trim().Replace(",", "."));
                    break;

                case "tlbbPasteCoord":
                    var clipboard = Clipboard.GetText();
                    if (string.IsNullOrWhiteSpace(clipboard)) return;

                    if (Regex.IsMatch(clipboard, @"^([-]?[\d]{1,2}[\,|\.]\d+)[\;| ]([-]?[\d]{1,2}[\,|\.]\d+)$"))
                    {
                        string sCoords = clipboard.Replace('.', ',');
                        var coords = sCoords.Replace(' ', ';').Split(';');
                        xCoord.Text = coords[0];
                        yCoord.Text = coords[1];

                        _observPointsController.UpdateObservPoint(
                            GetObservationPoint(),
                            VisibilityManager.ObservPointFeature,
                            ActiveView,
                            _selectedPointId);
                    }
                    else
                    {
                        string sMsgText = LocalizationContext.Instance.FindLocalizedElement(
                            "MsgInvalidCoordinatesDD",
                            "недійсні дані \nПотрібні коордінати представлені у СК WGS-84, десяткові градуси");
                        MessageBox.Show(
                            sMsgText,
                            LocalizationContext.Instance.MsgBoxErrorHeader,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }

                    break;

                case "tlbbShowCoord":

                    _observPointsController.ShowObservPoint(ActiveView, _selectedPointId);

                    break;
            }
        }

        private void Filter_CheckedChanged(object sender, EventArgs e)
        {
            DisplaySelectedColumns(GetFilter);
        }

        private void DgvObservationPoints_SelectionChanged(object sender, EventArgs e)
        {
            _isFieldsChanged = false;

            if (dgvObservationPoints.SelectedRows.Count == 0)
            {
                EnableObservPointsControls(true);
                return;
            }

            EnableObservPointsControls();
            var selectedPoint = _observPointsController.GetObservPointById(_selectedPointId);
            selectedPointMEM = selectedPoint;

            if (selectedPoint == null)
            {
                return;
            }

            FillObservPointsFields(selectedPoint);
        }


        private void FilterComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterData();
        }

        private void EditComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (dgvObservationPoints.SelectedRows.Count == 0 || !_isDropDownItemChangedManualy)
            {
                return;
            }

            //  SavePoint();
        }

        private void Fields_TextChanged(object sender, EventArgs e)
        {
            _isFieldsChanged = true;
        }

        #endregion

        #region VisibilitySessionsTabEvents

        private void TlbVisiilitySessions_ButtonClick(object sender, ToolBarButtonClickEventArgs e)
        {
            if (e.Button == removeTask)
            {
                string sMsgText = LocalizationContext.Instance.FindLocalizedElement(
                    "MsgQueryToDeleteResultFromSession",
                    "Ви дійсно бажаєте видалити результат розрахунку з поточного сеансу?");
                var result = MessageBox.Show(
                    sMsgText,
                    LocalizationContext.Instance.MsgBoxQueryHeader,
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Question);

                if (result == DialogResult.OK)
                {
                    if (!RemoveSelectedSession())
                    {
                        string sMsgText1 = LocalizationContext.Instance.FindLocalizedElement(
                            "MsgWarningToDeleteResultFromSession",
                            "Неможливо видалити результат розрахунку видимісті поточної сеансу");
                        MessageBox.Show(
                            sMsgText1,
                            LocalizationContext.Instance.MsgBoxWarningHeader,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                    }
                }
            }
            else if (e.Button == wizardTask)
            {
                MapLayersManager mlm = new MapLayersManager(ActiveView);

                string obserPointLayerName = mlm.GetLayerAliasByFeatureClass(_observPointsController.GetObservationPointsLayerName);
                string obserAreaLayerName = mlm.GetLayerAliasByFeatureClass(_observPointsController.GetObservationStationLayerName);

                var wizard = (new WindowMilSpaceMVisibilityMaster(obserPointLayerName, obserAreaLayerName, _observPointsController.GetPreviousPickedRasterLayer()));
                wizard.ShowDialog();
                var dialogResult = wizard.DialogResult;

                if (dialogResult == DialogResult.OK)
                {
                    var calcParams = wizard.FinalResult;

                    _observPointsController.UpdataPreviousPickedRasterLayer(calcParams.RasterLayerName);

                    var clculated = _observPointsController.CalculateVisibility(calcParams);

                    if (!clculated)
                    {
                        string sMsgText = LocalizationContext.Instance.FindLocalizedElement(
                            "MsgErrorCalculateVisibility",
                            "Розрахунок скінчився з помилкою\nДля перегляду повної інформації зверніться до журналу роботи");
                        MessageBox.Show(
                            sMsgText,
                            LocalizationContext.Instance.MsgBoxErrorHeader,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }

                    _visibilitySessionsController.UpdateVisibilitySessionsList(true, calcParams.TaskName);
                    _visibilitySessionsController.UpdateVisibilityResultsTree();
                }
            }
        }

        private void DgvVisibilitySessions_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvVisibilitySessions.SelectedRows.Count == 0)
            {
                tlbVisibilitySessions.Buttons["removeTask"].Enabled = false;
                return;
            }

            var selectedSessionId = dgvVisibilitySessions.SelectedRows[0].Cells["Id"].Value.ToString();
            var selectedTask = _visibilitySessionsController.GetCalcTask(selectedSessionId);

            if (selectedTask != null)
            {
                FillVisibilitySessionFields(selectedTask);
            }

            tlbVisibilitySessions.Buttons["removeTask"].Enabled = true;
        }

        private void CmbStateFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterVisibilityList();
        }

        #endregion

        #region ObservationObjectsTabEvents

        private void CmbObservObjAffiliationFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbObservObjAffiliationFilter.SelectedItem != null)
            {
                FilterObservObjects();
            }
        }

        private void DgvObservObjects_SelectionChanged(object sender, EventArgs e)
        {
            toolBarButton31.Enabled = toolBarButton34.Enabled = dgvObservObjects.SelectedRows.Count > 0;
            if (dgvObservObjects.SelectedRows.Count == 0)
            {
                ClearObservObjectFields();
                return;
            }

            if (dgvObservObjects.SelectedRows[0].Cells["Id"].Value == null
                || dgvObservObjects.SelectedRows[0].Cells["Id"].Value.ToString() == string.Empty)
            {
                var selectedObjectOID =
                    _observPointsController.GetObservObjectByOId(dgvObservObjects.SelectedRows[0].Cells["ObjectId"].Value.ToString());
                if (selectedObjectOID != null)
                {
                    string sID = "ON" + DateTime.Now.ToString("yyyyMMddTHHmmss");
                    dgvObservObjects.SelectedRows[0].Cells["Id"].Value = sID;
                    var seletctedItem = dgvObservObjects.SelectedRows[0];
                    if (seletctedItem.DataBoundItem is ObservObjectGui sourceItem)
                    {
                        sourceItem.Id = sID;
                        bool result = _observPointsController.SaveObservationObject(sourceItem);
                        if (!result)
                        {
                            string sMsgText = LocalizationContext.Instance.FindLocalizedElement(
                                "MsgTextCannotSaveObservationstahtionsID", "Ідентіфікатор облаcті нагляду не було збережено");
                            MessageBox.Show(
                                sMsgText,
                                LocalizationContext.Instance.MsgBoxInfoHeader,
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                            //return;
                        }
                        FillObservObjectFields(selectedObjectOID);
                    }
                }
            }
            else
            {
                var selectedObject = _observPointsController.GetObservObjectById(dgvObservObjects.SelectedRows[0].Cells["Id"].Value.ToString());
                if (selectedObject != null)
                {
                    FillObservObjectFields(selectedObject);
                }
            }
        }

        private void tbObservObjects_ButtonClick(object sender, EventArgs e)
        {
            log.DebugEx("> tbObservObjects START");

            if (!_observPointsController.IsObservObjectsExists())
            {
                if (_observPointsController.AddObservObjectsLayer())
                {
                    log.DebugEx("tbObservObjects. _observPointsController.AddObservObjectsLayer OK");
                    _observPointsController.UpdateObservObjectsList();
                    //tlbbAddObservObjLayer.Enabled = false;
                }
                else
                {
                    log.DebugEx("tbObservObjects. _observPointsController.AddObservObjectsLayer NOK");
                    //tlbbAddObservObjLayer.Enabled = true;
                }
            }
            else
            {
                log.DebugEx("tbObservObjects. _observPointsController.UpdateObservObjectsList");
                _observPointsController.UpdateObservObjectsList();
            }
            log.DebugEx("> tbObservObjects END");
        }

        private void ChckObservObj_CheckedChanged(object sender, EventArgs e)
        {
            dgvObservObjects.Columns["Title"].Visible = chckObservObjTitle.Checked;
            dgvObservObjects.Columns["Affiliation"].Visible = chckObservObjAffiliation.Checked;
            dgvObservObjects.Columns["Group"].Visible = chckObservObjGroup.Checked;
        }

        private void buttonSaveOPoint_Click(object sender, EventArgs e)
        {
            if (_observPointsController.IsArcMapEditingStarted())
            {

                string sMsgText = LocalizationContext.Instance.FindLocalizedElement(
                    "MsgCannotSaveObservationpoints",
                    $"Для збереження властивостей пункту спостереження {Environment.NewLine} треба вимкнути режим редагування ArcMap.");

                MessageBox.Show(
                    sMsgText,
                    LocalizationContext.Instance.MsgBoxQueryHeader,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
                return;
            }
            SavePoint();
        }

        #endregion


        #region VisibilityResultsTabEvents


        private void ToolBarVisibleResults_ButtonClick(object sender, ToolBarButtonClickEventArgs e)
        {
            if (e.Button.Name == tlbbFullDelete.Name)
            {
                string sMsgText = LocalizationContext.Instance.FindLocalizedElement(
                    "MsgQueryDeleteResultFull",
                    "Ви дійсно бажаєте повністтю видалити результат розрахунку?");
                var result = MessageBox.Show(
                    sMsgText,
                    LocalizationContext.Instance.MsgBoxQueryHeader,
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Question);

                if (result == DialogResult.OK)
                {
                    var resultId = tvResults.SelectedNode.Tag.ToString();

                    var isRemovingSuccessfull = _visibilitySessionsController.RemoveResult(resultId, ActiveView);

                    if (!isRemovingSuccessfull)
                    {
                        string sMsgText1 = LocalizationContext.Instance.FindLocalizedElement(
                            "MsgWarningDeleteResultFull",
                            "Неможливо повністтю видалити результат розрахунку");
                        MessageBox.Show(
                            sMsgText1,
                            LocalizationContext.Instance.MsgBoxWarningHeader,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                    }
                    else
                    {
                        var node = tvResults.Nodes.Find(resultId, true).First();
                        tvResults.Nodes.Remove(node);
                    }
                }

                return;
            }

            if (e.Button.Name == toolBarButtonRemoveFromSeanse.Name)
            {
                string sMsgText1 = LocalizationContext.Instance.FindLocalizedElement(
                    "MsgQueryDeleteResultFromSession",
                    "Ви дійсно бажаєте видалити результат розрахунку?");
                var result = MessageBox.Show(
                    "Ви дійсно бажаєте видалити результат розрахунку?",
                    LocalizationContext.Instance.MsgBoxQueryHeader,
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Question);

                if (result == DialogResult.OK)
                {
                    var resultId = tvResults.SelectedNode.Tag.ToString();

                    var removeLayers = _visibilitySessionsController.IsResultsLayerExist(resultId, ActiveView);

                    if (removeLayers)
                    {
                        var removeLayersDialogResult = MessageBox.Show(
                        LocalizationContext.Instance.VisibilityResultLayersRemoveMessage,
                        LocalizationContext.Instance.MessageBoxCaption,
                        MessageBoxButtons.OKCancel, MessageBoxIcon.Information);

                        removeLayers = removeLayersDialogResult == DialogResult.OK;
                    }

                    _visibilitySessionsController.RemoveResultsFromSession(resultId, removeLayers, ActiveView);
                    var node = tvResults.Nodes.Find(resultId, true).First();
                    tvResults.Nodes.Remove(node);
                }
            }

            if (e.Button.Name == tlbbShare.Name)
            {
                var isShared = _visibilitySessionsController.ShareResults(tvResults.SelectedNode.Tag.ToString());

                if (isShared)
                {
                    SetVisibilityResultsButtonsState(true);

                    string sMsgText1 = LocalizationContext.Instance.FindLocalizedElement(
                        "MsgInfoSetResultShare",
                        "Доступ до результату для усіх користувачів встановлено");
                    MessageBox.Show(
                        sMsgText1,
                        LocalizationContext.Instance.MsgBoxInfoHeader,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                else
                {
                    string sMsgText1 = LocalizationContext.Instance.FindLocalizedElement(
                        "MsgInfoSetResultShareAgain",
                        "Доступ до результату для усіх користувачів вже встановлено");
                    MessageBox.Show(
                        sMsgText1,
                        LocalizationContext.Instance.MsgBoxInfoHeader,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }

            if (e.Button.Name == toolBarButtonViewOnMap.Name)
            {
                _visibilitySessionsController.AddResultsGroupLayer(tvResults.SelectedNode.Tag.ToString(), ActiveView);
                SetVisibilityResultsButtonsState(true);
            }

            if (e.Button.Name == tlbbZoomToResultRaster.Name)
            {
                _visibilitySessionsController.ZoomToLayer(tvResults.SelectedNode.Tag.ToString(), ActiveView);
            }

            if (e.Button.Name == tlbbAddFromDB.Name)
            {
                var accessibleResultsWindow = new AccessibleResultsModalWindow(_visibilitySessionsController.GetAllResults(), ActiveView.FocusMap.SpatialReference);
                var dialogResult = accessibleResultsWindow.ShowDialog();

                if (dialogResult == DialogResult.OK)
                {
                    if (accessibleResultsWindow.SelectedResults != null)
                    {
                        AddNewResultsToTree(accessibleResultsWindow.SelectedResults);
                        var operationResult = _visibilitySessionsController.AddSharedResults(accessibleResultsWindow.SelectedResults);

                        if (!operationResult)
                        {
                            string sMsgText1 = LocalizationContext.Instance.FindLocalizedElement(
                                "MsgWarninsNoSetPartResultToSession",
                                "Частина результатів розрахунку не може бути добавлена до поточного сеансу");
                            MessageBox.Show(
                                sMsgText1,
                                LocalizationContext.Instance.MsgBoxWarningHeader,
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                        }
                    }
                }
            }

            if (e.Button.Name == tlbbUpdate.Name)
            {
                _visibilitySessionsController.UpdateVisibilityResultsTree();
            }
        }

        private void tvResults_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var selectedNode = e.Node;

            SetVisibilityResultsButtonsState(selectedNode.Parent != null);

            int devuders = selectedNode.FullPath.Split('%').Length;
            if (devuders == 1) //Root node
            {
                lvResultsSummary.Items.Clear();
                lvResultsSummary.Tag = null;
                return;
            }

            if (lvResultsSummary.Tag == null || !lvResultsSummary.Tag.Equals(selectedNode.Tag))
            {
                lvResultsSummary.Tag = selectedNode.Tag;

                var summary = _visibilitySessionsController.GetSummaryResultById(selectedNode.Tag.ToString());

                var summaryInfos = summary.SummaryToString();
                lvResultsSummary.Items.Clear();
                foreach (var item in summaryInfos)
                {
                    var lstViewitem = new ListViewItem(LocalizationContext.Instance.SummaryItems[item.Key]);

                    string contentValue = item.Value;
                    if (LocalizationContext.Instance.HasLocalizedElement(item.Value))
                    {
                        contentValue = LocalizationContext.Instance.FindLocalizedElement(item.Value, item.Value);
                    }
                    lstViewitem.SubItems.Add(contentValue);
                    lvResultsSummary.Items.Add(lstViewitem);
                }
            }
            //else if(devuders == 1)
            //{ }
            //else if(devuders == 2)
            //{ }


        }

        #endregion

        private void button2_Click(object sender, EventArgs e)
        {
            log.DebugEx("> (button2_Click) AddObservPointsLayer START");

            IsPointFieldsEnabled = _observPointsController.IsObservPointsExists();
            UpdateObservPointsList();

            if (!IsPointFieldsEnabled)
            {
                if (_observPointsController.AddObservPointsLayer())
                {
                    log.DebugEx("AddObservPointsLayer OK");
                    //_observPointsController.UpdateObservationPointsList();
                    //btnAddLayerPS.Enabled = false;
                }
                else
                {
                    log.DebugEx("AddObservPointsLayer NOK");
                    //btnAddLayerPS.Enabled = true;
                }
            }

            log.DebugEx("> (button2_Click) AddObservPointsLayer END");
        }

        private void DockableWindowMilSpaceMVisibilitySt_Load(object sender, EventArgs e)
        {
            ToolTip toolTip = new ToolTip();
            toolTip.AutoPopDelay = 5000;
            toolTip.InitialDelay = 1000;
            toolTip.ReshowDelay = 500;
            toolTip.ShowAlways = true;

            toolTip.SetToolTip(this.tlbbAddObservObjLayer, this.tlbbAddObservObjLayer.Tag.ToString());
            toolTip.SetToolTip(this.btnAddLayerPS, this.btnAddLayerPS.Tag.ToString());
            toolTip.SetToolTip(this.buttonSaveOPoint, this.buttonSaveOPoint.Tag.ToString());
            toolTip.SetToolTip(this.btnSaveParamPS, this.btnSaveParamPS.Tag.ToString());
        }

        private void tbObservObjects_CheckChanged(object sender, EventArgs e)
        {

            if (dgvObservObjects.SelectedRows.Count == 0) return;

            var seletctedItem = dgvObservObjects.SelectedRows[0];
            if (seletctedItem.DataBoundItem is ObservObjectGui sourceIten)
            {
                if (sender is TextBox control)
                {

                    if (control == tbObservObjGroup)
                    {
                        _isObservObjectsFieldsChanged = _isObservObjectsFieldsChanged || !sourceIten.Group.Equals(control.Text);
                    }
                    if (control == tbObservObjTitle)
                    {
                        _isObservObjectsFieldsChanged = _isObservObjectsFieldsChanged || !sourceIten.Title.Equals(control.Text);
                    }

                }
                if (sender is ComboBox comboControl)
                {
                    _isObservObjectsFieldsChanged = _isObservObjectsFieldsChanged ||
                        !sourceIten.Affiliation.Equals(cmbObservObjAffiliation.Text);
                }
            }
        }


        private void ObservationObjectChanged(object sender, EventArgs e)
        {
            if (_isObservObjectsFieldsChanged)
            {
                btnSaveParamPS.Enabled = true;
            }
        }

        private void btnSaveParamPS_Click(object sender, EventArgs e)
        {
            var seletctedItem = dgvObservObjects.SelectedRows[0];
            if (seletctedItem.DataBoundItem is ObservObjectGui sourceItem)
            {
                sourceItem.Title = tbObservObjTitle.Text;
                sourceItem.Group = tbObservObjGroup.Text;
                sourceItem.Affiliation = cmbObservObjAffiliation.Text;

                bool result = _observPointsController.SaveObservationObject(sourceItem);
                if (!result)
                {

                    string sMsgText = LocalizationContext.Instance.FindLocalizedElement(
                                        "MsgTextCannotSaveObservationstahtions",
                                        "Параметри облаcті нагляду не були збережені./nБільш детпльна інформація знахолится у лог файлі.");
                    MessageBox.Show(
                        sMsgText,
                        LocalizationContext.Instance.MsgBoxInfoHeader,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                    return;
                }

                dgvObservObjects.RefreshEdit();
                dgvObservObjects.Refresh();
                _isObservObjectsFieldsChanged = btnSaveParamPS.Enabled = false;
            }
        }

        private void tbObservObjects_ButtonClick(object sender, ToolBarButtonClickEventArgs e)
        {
            if (e.Button == toolBarButton31 && dgvObservObjects.RowCount > 0)
            {
                var obj = dgvObservObjects.SelectedRows[0].DataBoundItem as ObservObjectGui;
                _observPointsController.FlashObservationObject(obj.Id);
            }
            else if (e.Button == toolBarButton34 && dgvObservObjects.RowCount > 0)
            {
                var obj = dgvObservObjects.SelectedRows[0].DataBoundItem as ObservObjectGui;
                var sMsgText = LocalizationContext.Instance.FindLocalizedElement(
                                        "MsgTextDeleteObservStation",
                                        "Ви дійсно бажаэте видалити об'єкт нагляду?");
                var res = MessageBox.Show(
                    sMsgText,
                    LocalizationContext.Instance.MsgBoxInfoHeader,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);
                if (res == DialogResult.Yes)
                    _observPointsController.DeleteObservationObject(obj.Id);
            }
            else if (e.Button == toolBarButton29)
            {
                _observPointsController.UpdateObservObjectsList();
            }
        }
    }
}


