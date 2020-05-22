using MilSpace.AddDem.ReliefProcessing.Exceptions;
using MilSpace.Core;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using MilSpace.Core.DataAccess;
using MilSpace.DataAccess.DataTransfer.Sentinel;
using MilSpace.Tools.Sentinel;

namespace MilSpace.AddDem.ReliefProcessing
{
    public partial class PrepareDem : Form, IPrepareDemViewSrtm, IPrepareDemViewSentinel
    {
        Logger log = Logger.GetLoggerEx("PrepareDem");
        PrepareDemControllerSrtm controllerSrtm = new PrepareDemControllerSrtm();
        PrepareDemControllerSentinel controllerSentinel = new PrepareDemControllerSentinel();
        private IEnumerable<SentinelProduct> sentinelProducts = null;
        public PrepareDem()
        {
            controllerSrtm.SetView(this);
            controllerSentinel.SetView(this);
            InitializeComponent();
            InitializeData();

        }

        private void InitializeData()
        {
            controllerSrtm.ReadConfiguration();
            controllerSentinel.ReadConfiguration();

            lstSrtmFiles.DataSourceChanged += LstSrtmFiles_DataSourceChanged;
            lstSrtmFiles.DataSource = SrtmFilesInfo;
            lstSrtmFiles.DisplayMember = "Name";

            lstTiles.Items.Clear();
            lstTiles.DataSource = TilesToImport;
            lstTiles.DisplayMember = "Name";

            lstSentilenProducts.DataSource = sentinelProducts;
            lstSentilenProducts.DisplayMember = "Identifier";

            controllerSentinel.OnProductLoaded += OnSentinelProductLoaded;
        }

        private void OnSentinelProductLoaded(IEnumerable<SentinelProduct> products)
        {
            lstSentilenProducts.DataSource = products;
            lstSentilenProducts.DisplayMember = "Identifier";
            lstSentilenProducts.Update();
            lstSentilenProducts.Refresh();
        }

        private void LstSrtmFiles_DataSourceChanged(object sender, EventArgs e)
        {

        }
        #region IPrepareDemViewSentinel
        public string SentinelSrtorage { get => lblSentinelStorage.Text; set => lblSentinelStorage.Text = value; }

        public IEnumerable<Tile> TilesToImport { get => controllerSentinel.TilesToImport; }

        public string TileLatitude { get => txtLatitude.Text; }
        public string TileLongtitude { get => txtLongtitude.Text; }

        public IEnumerable<SentinelProduct> SentinelProducts { get => sentinelProducts; set => sentinelProducts = value; }

        public DateTime SentinelRequestDate { get => dtSentinelProductes.Value; }
        #endregion
        #region IPrepareDemViewSrtm

        public string SrtmSrtorage { get => lblSrtmStorage.Text; set => lblSrtmStorage.Text = value; }
        public IEnumerable<FileInfo> SrtmFilesInfo { get; set; } = new List<FileInfo>();
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
            //lstTiles.Items.Add(controllerSentinel.GetTilesByPoint());

            controllerSentinel.AddTileForImport();
            lstTiles.DataSource = TilesToImport;
            lstTiles.DisplayMember = "Name";
            lstTiles.Refresh();
            lstTiles.Update();
        }

        private void btnGetScenes_Click(object sender, EventArgs e)
        {
            controllerSentinel.GetScenes();
        }
    }
}
