using MilSpace.AddDem.ReliefProcessing.Exceptions;
using MilSpace.AddDem.ReliefProcessing.GuiData;
using MilSpace.Core;
using MilSpace.Core.DataAccess;
using MilSpace.DataAccess.DataTransfer.Sentinel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace MilSpace.AddDem.ReliefProcessing
{
    public partial class PrepareDem : Form, IPrepareDemViewSrtm, IPrepareDemViewSentinel, IPrepareDemViewSentinelPeocess
    {
        Logger log = Logger.GetLoggerEx("PrepareDem");
        PrepareDemControllerSrtm controllerSrtm = new PrepareDemControllerSrtm();
        PrepareDemControllerSentinel controllerSentinel = new PrepareDemControllerSentinel();
        PrepareDemControllerSentinelProcess controllerSentinelProcess = new PrepareDemControllerSentinelProcess();
        bool staredtFormArcMap;
        private IEnumerable<SentinelProduct> sentinelProducts = null;
        public PrepareDem(bool startFormArcMap = true)
        {
            staredtFormArcMap = startFormArcMap;
            controllerSrtm.SetView(this);
            controllerSentinel.SetView(this);

            controllerSentinel.OnProductsDownloaded += OnProductsDownloaded;

            InitializeComponent();
            if (startFormArcMap)
            {
                tabControlTop.Controls.Remove(srtmTabTop);
                tabControlTop.Controls.Remove(tabLoadTop);
                tabControlTop.Controls.Remove(tabPreprocessTop);
            }
            else
            {
                tabControlTop.Controls.Remove(tabGenerateTileTop);
            }
            InitializeData();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (controllerSentinel.DownloadStarted && MessageBox.Show("Downloading process in progress./n Do tou realy want to close form?", "Milspace Message title", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                e.Cancel = true;
            }
        }

        private void OnProductsDownloaded(IEnumerable<SentinelProduct> products)
        {
            MessageBox.Show("Products were sucessfully downloaded.", "Milspace Message title", MessageBoxButtons.OK, MessageBoxIcon.Information);

            var demPrepare = new DataAccess.Facade.DemPreparationFacade();

            var productRecords = products.ToList().Select(p => demPrepare.AddSentinelProduct(p));

            if (productRecords.Any(p => p == null))
            {
                log.ErrorEx("There was an error on adding Santinel product");
            }

            ShowButtons();
        }

        private void InitializeData()
        {
            controllerSrtm.ReadConfiguration();
            controllerSentinel.ReadConfiguration();

            lstSrtmFiles.DataSourceChanged += LstSrtmFiles_DataSourceChanged;
            lstSrtmFiles.DataSource = SrtmFilesInfo;
            lstSrtmFiles.DisplayMember = "Name";

            lstSentilenProducts.DataSourceChanged += LstSentilenProducts_DataSourceChanged;
            lstSentilenProducts.DisplayMember = "Identifier";

            controllerSentinel.OnProductLoaded += OnSentinelProductLoaded;

            lstSentinelProductsToDownload.Items.Clear();

            ShowButtons();
        }

        private void LstSentilenProducts_DataSourceChanged(object sender, EventArgs e)
        {
            var curSentineltile = SelectedTile;

            if (SelectedTile.DownloadingScenes != null)
            {
                foreach (var sg in SelectedTile.DownloadingScenes)
                {
                    AddSceneToDownload(sg);
                }
            }
        }

        private void OnSentinelProductLoaded(IEnumerable<SentinelProduct> products)
        {
            var selectedIndx = lstSentilenProducts.SelectedIndex;
            lstSentinelProductProps.Items.Clear();
            lstSentinelProductsToDownload.Items.Clear();

            var ttt = products.ToArray();
            lstSentilenProducts.DataSource = ttt;
            lstSentilenProducts.DisplayMember = "Identifier";
            lstSentilenProducts.Update();
            lstSentilenProducts.Refresh();

            if (products != null && products.Any())
            {
                if (products.Count() < selectedIndx)
                {
                    selectedIndx = products.Count() - 1;
                }
                lstSentilenProducts.SelectedItem = products.First();
            }

            lstSentilenProducts_SelectedIndexChanged(lstSentilenProducts, null);

            ShowButtons();
        }

        private void FillTileSource()
        {
            lstTiles.Items.Clear();
            TilesToImport?.ToList().ForEach(t => lstTiles.Items.Add(t.ParentTile.Name));
        }

        private void LstSrtmFiles_DataSourceChanged(object sender, EventArgs e)
        {

        }
        #region IPrepareDemViewSentinel
        public string SentinelSrtorage { get => lblSentinelStorage.Text; set => lblSentinelStorage.Text = value; }

        public IEnumerable<SentinelTile> TilesToImport { get => controllerSentinel.TilesToImport; }

        public SentinelTile SelectedTile => controllerSentinel.GetTileByName(lstTiles.SelectedItem?.ToString());

        public string TileLatitude { get => txtLatitude.Text; }
        public string TileLongtitude { get => txtLongtitude.Text; }

        public IEnumerable<SentinelProduct> SentinelProductsToDownload { get => sentinelProducts; set => sentinelProducts = value; }

        public DateTime SentinelRequestDate { get => dtSentinelProductes.Value; }
        #endregion
        #region IPrepareDemViewSrtm

        public string SrtmSrtorage { get => lblSrtmStorage.Text; set => lblSrtmStorage.Text = value; }
        public IEnumerable<FileInfo> SrtmFilesInfo { get; set; } = new List<FileInfo>();
        public IEnumerable<Tile> DownloadedTiles => controllerSentinelProcess.GetTilesFromDownloaded();

        public SentinelPairCoherence SelectedPair
        {
            get
            {
                if (lstPairsTOProcess.SelectedItem != null)
                {
                    string productName = lstPairsTOProcess.SelectedItem.ToString();
                    return controllerSentinelProcess.GetPairPairBySceneName(productName, lstPairsTOProcess.SelectedIndex % 2 == 0);
                }
                return null;
            }
        }

        public IEnumerable<SentinelProduct> SentinelProductsFromDatabase => controllerSentinel.GetAllSentinelProduct();

        public IEnumerable<Tile> TilesToProcess => throw new NotImplementedException();

        #endregion

        private void btnImportSrtm_Click(object sender, EventArgs e)
        {
            if (controllerSrtm.CopySrtmFilesToStorage())
            {
                MessageBox.Show("The files were imported sucessfully.", "Milspace Message title", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Something went wrong. /n for more detailed infor go to the log file",
                                "Milspace Message title", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSelectSrtm_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog selectFolder = new FolderBrowserDialog())
            {
                if (selectFolder.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(selectFolder.SelectedPath))
                {
                    string message = "The {0} files were found.";
                    MessageBoxIcon icon = MessageBoxIcon.Information;
                    try
                    {
                        controllerSrtm.ReadSrtmFilesFromFolder(selectFolder.SelectedPath);
                        lstSrtmFiles.Visible = true;
                        lstSrtmFiles.DataSource = SrtmFilesInfo;
                        lstSrtmFiles.Refresh();
                        lstSrtmFiles.Update();
                        message = message.InvariantFormat(SrtmFilesInfo.Count());

                        return;
                    }
                    catch (DirectoryNotFoundException ex)
                    {
                        log.ErrorEx(ex.Message);
                        message = ex.Message;
                        icon = MessageBoxIcon.Error;
                    }
                    catch (NothingToImportException ex)
                    {
                        log.ErrorEx(ex.Message);
                        message = ex.Message;
                        icon = MessageBoxIcon.Exclamation;
                    }
                    catch (Exception ex)
                    {
                        message = "Unexpected error";
                        log.ErrorEx(ex.Message);
                        icon = MessageBoxIcon.Error;
                    }
                    MessageBox.Show(message, "Mislspace Msg Cation", MessageBoxButtons.OK, icon);
                }
            }
        }

        private bool CheckDouble(TextBox textBox, char keyChar)
        {
            return (char.IsNumber(keyChar) || keyChar == (char)Keys.Back || keyChar == (char)KeyCodesEnum.Minus
                            || (keyChar == (int)KeyCodesEnum.DecimalPoint && textBox.Text.IndexOf(".") == -1));
        }

        private void txtLongtitude_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !CheckDouble(sender as TextBox, e.KeyChar);
            btnAddTileToList.Enabled = !e.Handled && controllerSentinel.GetTilesByPoint() != null;

        }

        private void txtLatitude_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !CheckDouble(sender as TextBox, e.KeyChar);
            btnAddTileToList.Enabled = !e.Handled && controllerSentinel.GetTilesByPoint() != null;
        }

        private void btnAddTileToList_Click(object sender, EventArgs e)
        {
            controllerSentinel.AddTileForImport();
            FillTileSource();
        }

        private void btnGetScenes_Click(object sender, EventArgs e)
        {
            controllerSentinel.GetScenes();
        }

        private void lstSentilenProducts_SelectedIndexChanged(object sender, EventArgs e)
        {
            var props = controllerSentinel.GetSentinelProductProperties(lstSentilenProducts.SelectedItem as SentinelProduct);

            lstSentinelProductProps.Items.Clear();

            foreach (var prop in props)
            {
                var item = new ListViewItem(prop[0]);
                item.SubItems.Add(prop[1]);
                lstSentinelProductProps.Items.Add(item);
            }

            ShowButtons();
        }

        private void ShowButtons()
        {
            bool selectedProduct = controllerSentinel.CheckProductExistanceToDownload(lstSentilenProducts.SelectedItem as SentinelProduct);
            btnGetScenes.Enabled = SelectedTile != null;
            btnAddSentinelProdToDownload.Enabled = lstSentilenProducts.SelectedItem != null && !selectedProduct;
            btnSetSentinelProdAsBase.Enabled = false;
            btnDownloadSentinelProd.Enabled = SelectedTile != null && SelectedTile.DownloadingScenes.Count() >= 2 && !controllerSentinel.DownloadStarted;

            btnChkCoherence.Enabled = SelectedPair != null && SelectedPair.Mean < 0;
            btnProcess.Enabled = SelectedPair != null;
        }

        private void btnAddSentinelProdToDownload_Click(object sender, EventArgs e)
        {
            var pg = lstSentilenProducts.SelectedItem as SentinelProduct;
            if (pg != null)
            {
                var pairs = controllerSentinel.GetScenePairProduct(pg);

                lstSentinelProductsToDownload.Items.Clear();
                var pgl = controllerSentinel.AddProductsToDownload(pairs);

                if (pairs.Count() == 2)
                {
                    controllerSentinel.AddSentinelPairCoherence(pgl.First(), pgl.Last());
                }

                foreach (var p in pgl)
                {
                    AddSceneToDownload(p);
                }
                ShowButtons();
            }
        }

        private void AddSceneToDownload(SentinelProductGui sentinelProduct)
        {
            if (sentinelProduct != null)
            {
                ListViewItem item = new ListViewItem(sentinelProduct.Identifier);
                lstSentinelProductsToDownload.Items.Add(item);
            }
        }

        private void btnDownloadSentinelProd_Click(object sender, EventArgs e)
        {
            controllerSentinel.DownloadProducts();
            ShowButtons();
        }

        private void FillScenesList()
        {
            var selectedTile = SelectedTile;
            if (selectedTile != null)
            {
                OnSentinelProductLoaded(selectedTile.TileScenes);
            }
        }

        private void lstTiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedTile = SelectedTile;
            if (selectedTile != null)
            {
                OnSentinelProductLoaded(selectedTile.TileScenes);
            }
        }

        private void btnSetSentinelProdAsBase_Click(object sender, EventArgs e)
        {

        }

        private void button17_Click(object sender, EventArgs e)
        {
            controllerSentinel.ProcessPreliminary();
        }

        private void loadProducts_Click(object sender, EventArgs e)
        {

        }

        private void tabControl2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControlTop.SelectedIndex == 2)
            {
                lstPreprocessTiles.Items.Clear();
                DownloadedTiles?.ToList().ForEach(t => lstPreprocessTiles.Items.Add(t.Name));
            }
        }

        private void lstPreprocessTiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            lstPairsTOProcess.Items.Clear();
            var pairs = controllerSentinelProcess.GetPairsFromDownloaded(lstPreprocessTiles.SelectedItem.ToString());

            pairs.ToList().ForEach(p => lstPairsTOProcess.Items.AddRange(p.Pair.ToArray()));

        }

        private void btnChkCoherence_Click(object sender, EventArgs e)
        {
            controllerSentinelProcess.CheckCoherence(SelectedPair);
        }

        private void btnProcess_Click(object sender, EventArgs e)
        {
            controllerSentinelProcess.PairProcessing(SelectedPair);
        }

        private void lstPairsTOProcess_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowButtons();
        }
    }
}
