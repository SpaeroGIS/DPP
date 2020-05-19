using MilSpace.AddDem.ReliefProcessing.Exceptions;
using MilSpace.Core;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace MilSpace.AddDem.ReliefProcessing
{
    public partial class PrepareDem : Form, IPrepareDemViewSrtm
    {
        Logger log = Logger.GetLoggerEx("PrepareDem");
        PrepareDemControllerSrtm controller = new PrepareDemControllerSrtm();
        public PrepareDem()
        {
            controller.SetView(this);
            InitializeComponent();
            InitializeData();

        }

        private void InitializeData()
        {
            controller.ReadConfiguration();
            lstSrtmFiles.DataSourceChanged += LstSrtmFiles_DataSourceChanged;
            lstSrtmFiles.DataSource = SrtmFilesInfo;
            lstSrtmFiles.DisplayMember = "Name";
        }

        private void LstSrtmFiles_DataSourceChanged(object sender, EventArgs e)
        {
            btnImportSrtm.Enabled = SrtmFilesInfo.Count() > 0;
        }
        #region IPrepareDemViewSrtm
        public string SentinelSrtorage { get => lblSentinelStorage.Text; set => lblSentinelStorage.Text = value; }
        public string SrtmSrtorage { get => lblSrtmStorage.Text; set => lblSrtmStorage.Text = value; }
        public IEnumerable<FileInfo> SrtmFilesInfo { get; set; } = new List<FileInfo>();
        #endregion

        private void btnImportSrtm_Click(object sender, EventArgs e)
        {
            if (controller.CopySrtmFilesToStorage())
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
                        controller.ReadSrtmFilesFromFolder(selectFolder.SelectedPath);
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
    }
}
