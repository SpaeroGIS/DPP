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
using MilSpace.Core.Tools;
using System.Text;
using System.IO;

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
            ShowButtons();
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

        public IMap ActiveMap => ArcMap.Document.FocusMap;

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
            IEnvelope env = geometry.Envelope;

            IPoint point1 = env.UpperRight.CloneWithProjecting();
            IPoint point2 = env.LowerLeft.CloneWithProjecting();

            IArea are = env as IArea;
            ILine line = new Line();
            ILine line2 = new Line();
            line2.FromPoint = env.LowerLeft;
            line2.ToPoint = env.UpperLeft;
            line.FromPoint = env.LowerLeft;
            line.ToPoint = env.LowerRight;


            lblLengthWidth.Text = $"розмір (км.) {(line.Length / 1000).ToString("F2")} x {(line2.Length / 1000).ToString("F2")}";//    (км) 150 х  80
            lblSquare.Text = $"площа (кв.км.): {(are.Area / 1000000).ToString("F2")}";

            txtPoint1X.Text = point1.X.ToString("F5");
            txtPoint1Y.Text = point1.Y.ToString("F5");
            txtPoint2X.Text = point2.X.ToString("F5");
            txtPoint2Y.Text = point2.Y.ToString("F5");

            FillTilelist();
        }

        private void FillTilelist()
        {
            IEnumerable<string[]> tileList = null;
            if (CurrentSourceType == DemSourceTypeEnum.STRM)
            {
                tileList = SelectedSrtmGrid?.Where(t => (chckYes.Checked && t.Loaded) || (chckNo.Checked && !t.Loaded)
                    ).Select(g => new string[] { g.Loaded ? "Plus.png" : "Minus.png", g.SRTM });
            }
            else if (CurrentSourceType == DemSourceTypeEnum.Sentinel1)
            {
                tileList = SelectedS1Grid?.Where(t => (chckYes.Checked && t.Loaded) || (chckNo.Checked && !t.Loaded)
                ).Select(g => new string[] { g.Loaded ? "Plus.png" : "Minus.png", g.SRTM });
            }

            lstSelectedTiles.Items.Clear();

            tileList?.ToList().ForEach(l => lstSelectedTiles.Items.Add(new ListViewItem(l[1], l[0])));
            ShowButtons();
        }

        private void ShowButtons()
        {
            btnExport.Enabled = lstSelectedTiles.Items.Count > 0;
            btnAddToMap.Enabled = lstSelectedTiles.SelectedItems.Count > 0 && 
                lstSelectedTiles.SelectedItems[0].ImageKey.StartsWith("Plus");
            btnLoadFromCatalog.Enabled = lstSelectedTiles.Items.Count > 0 &&
                lstSelectedTiles.Items.Cast<ListViewItem>().Any(l => l.ImageKey.StartsWith("Plus"));


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
           // btnGenerateList.Text = LocalizationContext.Instance.FindLocalizedElement("BtnGenerateListText", "сформувати список");
            //btnLoadScheme.Text = LocalizationContext.Instance.FindLocalizedElement("BtnLoadSchemeText", "завантажити схему тайлів");
            btnExport.Text = LocalizationContext.Instance.FindLocalizedElement("BtnExportText", "експорт тайлів");
            btnLoadFromCatalog.Text = LocalizationContext.Instance.FindLocalizedElement("BtnLoadFromCatalogText", "сформувати список тайлів");
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
                mapPointToolButton.Checked = false;
            }
            else
            {
                ArcMap.Application.CurrentTool = mapTool;
                mapPointToolButton.Checked = true;
            }
        }

        private void chck_CheckedChanged(object sender, EventArgs e)
        {
            FillTilelist();
        }

        private void btnGenerateList_Click(object sender, EventArgs e)
        {
          
        }

        private void btnLoadScheme_Click(object sender, EventArgs e)
        {

        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            using (var dd = new SaveFileDialog())
            {
                var text = new StringBuilder();
                for (int i = 0; i < lstSelectedTiles.Items.Count; i++)
                {
                    text.AppendLine(lstSelectedTiles.Items[i].Text);

                }
                if (dd.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllText(dd.FileName, text.ToString());
                }
            }
        }

        private void lstSelectedTiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowButtons();
        }

        private void btnAddToMap_Click(object sender, EventArgs e)
        {
            var isAdded = controller.AddSelectedTileToMap(lstSelectedTiles.SelectedItems[0].Text);

            if (!isAdded)
            {
                MessageBox.Show($"The tile {lstSelectedTiles.SelectedItems[0].Text} was not added.{Environment.NewLine}" +
                    $"To get more onformation try to look at the log file", "AddDemAddIn", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void rbtnSentinel1Type_CheckedChanged(object sender, EventArgs e)
        {
            if (CurrentSourceType == DemSourceTypeEnum.STRM)
            {
                lstSelectedTiles.Items.Clear();
                ShowButtons();
            }

        }

        private void rbtnSrtmType_CheckedChanged(object sender, EventArgs e)
        {
            if (CurrentSourceType == DemSourceTypeEnum.Sentinel1 )
            {
                lstSelectedTiles.Items.Clear();
                ShowButtons();
            }
        }

        private void btnLoadFromCatalog_Click(object sender, EventArgs e)
        {
            var items = lstSelectedTiles.Items.Cast<ListViewItem>().Where(i => i.ImageKey.StartsWith("Plus")).Select(i => i.Text);

            if (items.Count() == 1)
            {
                var isAdded = controller.AddSelectedTileToMap(items.First());

                if (!isAdded)
                {
                    MessageBox.Show($"The tile {lstSelectedTiles.SelectedItems[0].Text} was not added.{Environment.NewLine}" +
                        $"To get more onformation try to look at the log file", "AddDemAddIn", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                return;
            }

            if (!controller.GenerateUnionTile(lstSelectedTiles.Items.Cast<ListViewItem>().Where(i => i.ImageKey.StartsWith("Plus")).Select(i => i.Text)))
            {
                MessageBox.Show($"The tiles were not combined.{Environment.NewLine}To get more info, please take e look at the log file",
                    "AddDemAddIn", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
