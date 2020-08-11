using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geometry;
using MilSpace.Core.ModulesInteraction;
using MilSpace.DataAccess.DataTransfer;
using Sposterezhennya.AddDEM.ArcMapAddin.AddInComponents;
using Sposterezhennya.AddDEM.ArcMapAddin.Interaction;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Sposterezhennya.AddDEM.ArcMapAddin
{
    /// <summary>
    /// Designer class of the dockable window add-in. It contains user interfaces that
    /// make up the dockable window.
    /// </summary>
    public partial class DockableDEMWindow : UserControl, IAddDemView
    {
        AddDemController controller;

        public DockableDEMWindow(object hook, AddDemController controller)
        {
            InitializeComponent();
            this.Hook = hook;
            this.controller = controller;
            this.controller.RegisterView(this);

            LocalizeElements();
        }

        /// <summary>
        /// Host object of the dockable window
        /// </summary>
        private object Hook
        {
            get;
            set;
        }

        public IActiveView ActiveView => ArcMap.Document.ActiveView;

        public IMap ActiveMAp => ArcMap.Document.FocusMap;

        public DemSourceTypeEnum CurrentSourceType => rbtnSentinel1Type.Checked ?
            DemSourceTypeEnum.Sentinel1 : DemSourceTypeEnum.STRM;

        public IEnumerable<S1Grid> SelectedS1Grid { get; set; }
        public IEnumerable<SrtmGrid> SelectedSrtmGrid { get; set; }

        internal void ArcMap_OnMouseDown(int x, int y)
        {
        }

        internal void ArcMap_OnMouseMove(int x, int y)
        {
        }

        internal void SelectTilesByArea(IGeometry geometry)
        {
            controller.SearchSelectedTiles(geometry);
            FillTilelist();
        }

        private void FillTilelist()
        {
            IEnumerable<string> tileList = null;
            if (CurrentSourceType == DemSourceTypeEnum.STRM)
            {
                tileList = SelectedSrtmGrid?.Select(g => g.SRTM);
            }
            else if (CurrentSourceType == DemSourceTypeEnum.Sentinel1)
            {
                tileList = SelectedS1Grid?.Where(t => (chckYes.Checked && t.Loaded) || (chckNo.Checked && !t.Loaded)
                ).Select(g => g.SRTM);
            }

            lstSelectedTiles.Items.Clear();

            tileList?.ToList().ForEach(l => lstSelectedTiles.Items.Add(l));

        }


        /// <summary>
        /// Implementation class of the dockable window add-in. It is responsible for 
        /// creating and disposing the user interface class of the dockable window.
        /// </summary>
        public class AddinImpl : ESRI.ArcGIS.Desktop.AddIns.DockableWindow
        {
            private DockableDEMWindow m_windowUI;

            public AddinImpl()
            {
            }

            internal DockableDEMWindow UI
            {
                get { return m_windowUI; }
            }
            protected override IntPtr OnCreateChild()
            {
                AddDemController controller = new AddDemController();
                ModuleInteraction.Instance.RegisterModuleInteraction<IAddDemInteraction>(new AddDemInteraction(controller));

                m_windowUI = new DockableDEMWindow(this.Hook, controller);
                return m_windowUI.Handle;
            }

            protected override void Dispose(bool disposing)
            {
                if (m_windowUI != null)
                    m_windowUI.Dispose(disposing);

                base.Dispose(disposing);
            }

        }

        private void LocalizeElements()
        {
            lblWorkZone.Text = LocalizationContext.Instance.FindLocalizedElement("LblWorkZoneText", "зона роботи");
            lblChosenObjInfo.Text = LocalizationContext.Instance.FindLocalizedElement("LblChosenObjInfoText", "інформація про обраний об'єкт");
            lblCreationWay.Text = LocalizationContext.Instance.FindLocalizedElement("LblCreationWayText", "спосіб завдання");
            lblLeftConer.Text = LocalizationContext.Instance.FindLocalizedElement("LblLeftConerText", "ЛВ кут");
            lblRightCorner.Text = LocalizationContext.Instance.FindLocalizedElement("LblRightCornerText", "ПН кут");
            lblSizeInfo.Text = LocalizationContext.Instance.FindLocalizedElement("LblSizeInfoText", "інформація про площу та розміри");
            lblSquare.Text = String.Format(LocalizationContext.Instance.FindLocalizedElement("LblSquareText", "площа {0} км кв"), 1200);
            lblLengthWidth.Text = String.Format(LocalizationContext.Instance.FindLocalizedElement("LblLengthWidthText", "довжина {0} км ширина {1} км"), 150, 80);
            lblReliefCover.Text = LocalizationContext.Instance.FindLocalizedElement("LblReliefCoverText", "покриття рельєфу (каталог)");
            lblTilesList.Text = LocalizationContext.Instance.FindLocalizedElement("LblTilesListText", "список тайлів");

            chckYes.Text = LocalizationContext.Instance.FindLocalizedElement("ChckYesText", "є");
            chckNo.Text = LocalizationContext.Instance.FindLocalizedElement("ChckNoText", "немає");
            //            chckShowOnMap.Text = LocalizationContext.Instance.FindLocalizedElement("ChckShowOnMapText", "показати на карті");

            btnChoose.Text = LocalizationContext.Instance.FindLocalizedElement("BtnChooseText", "обрати");
            btnGenerateList.Text = LocalizationContext.Instance.FindLocalizedElement("BtnGenerateListText", "сформувати список");
            btnLoadScheme.Text = LocalizationContext.Instance.FindLocalizedElement("BtnLoadSchemeText", "завантажити схему тайлів");
            btnExport.Text = LocalizationContext.Instance.FindLocalizedElement("BtnExportText", "експорт тайлів");
            btnLoadFromCatalog.Text = LocalizationContext.Instance.FindLocalizedElement("BtnLoadFromCatalogText", "завантажити з каталога");
            btnAddToMap.Text = LocalizationContext.Instance.FindLocalizedElement("BtnAddToMapText", "приєднати до карти");
            btnCalculation.Text = LocalizationContext.Instance.FindLocalizedElement("BtnCalculationText", "Розрахунок");
        }

        private void button8_Click(object sender, EventArgs e)
        {
            controller.OpenDemCalcForm();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            UID mapToolID = new UIDClass
            {
                Value = ThisAddIn.IDs.AddDemMapInteropTool
            };
            var documentBars = ArcMap.Application.Document.CommandBars;
            var mapTool = documentBars.Find(mapToolID, false, false);

            if (ArcMap.Application.CurrentTool?.ID?.Value != null
                           && ArcMap.Application.CurrentTool.ID.Value.Equals(mapTool.ID.Value))
            {
                ArcMap.Application.CurrentTool = null;
                //MapPointToolButton.Checked = false;
            }
            else
            {
                ArcMap.Application.CurrentTool = mapTool;
                //MapPointToolButton.Checked = true;
            }
        }
    }
}
