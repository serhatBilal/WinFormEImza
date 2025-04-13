using WinFormEImza.Objects;
using Newtonsoft.Json;
using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using WinFormEImza.Operations;

namespace WinFormEImza
{
    public partial class WinFormEImza : Form
    {
        private bool isFirstLoad;
        public WinFormEImza()
        {
            InitializeComponent();
        }

        private void WinFormEImza_Load(object sender, EventArgs e)
        {
            try
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                                   | SecurityProtocolType.Tls11
                                   | SecurityProtocolType.Tls12
                                   | SecurityProtocolType.Ssl3;
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

                GeneralOperations.LogWrite(" Application started...");
                GeneralOperations.LoadSettingsFile();
                isFirstLoad = true;
                chkGunBoyuTekrarSorma.Checked = GeneralOperations.IsDontAskPinAllDay;
                chkDebugMod.Checked = GeneralOperations.IsDebugMode;
                isFirstLoad = false;
                GeneralOperations.LogWrite(" Settings loaded...");
                GeneralOperations.CheckCertificatePublicSM(false);
                ResetGrid();
            }
            catch (Exception ex)
            {
                GeneralOperations.LogWrite(" ERROR [WinFormEImza_Load] (" + ex.Message + ")");
                MessageBox.Show("WinFormEImza_Load", " Exception [WinFormEImza_Load] (" + ex.Message + ")", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void ResetGrid()
        {
            try
            {
                GridOperations objGridOperation = new GridOperations();
                objGridOperation.FillGrid(dgvBelgeler);
                dgvBelgeler.Refresh();
                objGridOperation.ConfigureGrid(dgvBelgeler);
            }
            catch (Exception ex)
            {
                GeneralOperations.LogWrite(" ERROR [ResetGrid] (" + ex.Message + ")");
            }
        }

        private void WinFormEImza_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                string filesToDelete = @"*_LogDetay.txt";
                string[] fileList = Directory.GetFiles(GeneralOperations.ROOT_DIR, filesToDelete);
                foreach (string file in fileList)
                {
                    if (File.GetCreationTime(file) < DateTime.Now.AddDays(-7))
                    {
                        File.Delete(file);
                    }
                }
                string[] arrStr = lstBoxLog.Items.Cast<string>().ToArray();
                char[] charDizi = string.Join("", arrStr).ToCharArray();
                var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(charDizi);

                File.AppendAllLines(GeneralOperations.ROOT_DIR + "\\" + DateTime.Now.ToString("yyyyMMdd") + "_LogDetail.txt", arrStr);
            }
            catch (Exception ex)
            {
                GeneralOperations.LogWrite(" ERROR [WinFormEImza_FormClosing] (" + ex.Message + ")");
            }
        }


        private static string GetInput()
        {
            if (GeneralOperations.IsDontAskPinAllDay && !string.IsNullOrEmpty(GeneralOperations.GetPin()))
            {
                return GeneralOperations.GetPin();
            }
            else
            {
                return GeneralOperations.ShowPasswordInputBox("Please enter your e-signature password:");
            }
        }

        private void btnDosyaSec_Click(object sender, EventArgs e)
        {
            string _filePath = "";
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.InitialDirectory = "C:/";
            fileDialog.Filter = "PDF Files|*.pdf";
            fileDialog.Title = "Select a file";
            fileDialog.RestoreDirectory = true;
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                _filePath = fileDialog.FileName;
            }
            new GridOperations().AddItemToGrid(dgvBelgeler, new SignatureDocument()
            {
                IsSelected = true,
                FilePath = _filePath,
                TargetUploadQueryString = "",
                TargetUploadUrl = ""
            });
        }

        private void btnSeciliBelgeleriImzala_Click(object sender, EventArgs e)
        {
            string eSignaturePassword = GetInput();
            if (!string.IsNullOrEmpty(eSignaturePassword))
            {
                if (GeneralOperations.IsDontAskPinAllDay && string.IsNullOrEmpty(GeneralOperations.GetPin()))
                {
                    GeneralOperations.SavePinToXml(eSignaturePassword);
                }
                GeneralOperations.SetPin(eSignaturePassword);
                foreach (DataGridViewRow row in dgvBelgeler.Rows)
                {
                    if ((bool)((DataGridViewCheckBoxCell)row.Cells[0]).Value)
                    {
                        string fileToSign = row.Cells[1].Value.ToString();
                        string signedFile = Regex.Replace(fileToSign, ".pdf", "-WinFormESignature.pdf", RegexOptions.IgnoreCase);
                        string targetUploadUrl = row.Cells[2].Value.ToString();
                        string targetUploadQueryString = row.Cells[3].Value.ToString();
                        PdfRequestDTO requestDTO = new PdfRequestDTO()
                        {
                            DonglePassword = GeneralOperations.GetPin(),
                            SourcePdfPath = fileToSign,
                            TargetPdfPath = signedFile
                        };
                        try
                        {
                            string signResult = SignFile(requestDTO);
                            GeneralOperations.LogWrite(" File signing result: (" + signResult + ")");
                        }
                        catch (Exception ex)
                        {
                            GeneralOperations.LogWrite(" File signing error: (" + ex.Message + ")");
                        }                       
                    }
                }
                ResetGrid();
            }
        }

        private string SignFile(PdfRequestDTO requestDTO)
        {
            GeneralOperations.LogWrite(" " + requestDTO.SourcePdfPath + " file is being signed, please wait.");
            SignatureManager signManager = new SignatureManager();
            return signManager.SignPdf(requestDTO);
        }

        private void btnSertifikaDeposuYenie_Click(object sender, EventArgs e)
        {
            GeneralOperations.CheckCertificatePublicSM(true);
        }
        private void chkGunBoyuTekrarSorma_CheckedChanged(object sender, EventArgs e)
        {
            if (!isFirstLoad)
                GeneralOperations.DontAskPinAllDay(chkGunBoyuTekrarSorma.Checked);
        }

        private void chkDebugMod_CheckedChanged(object sender, EventArgs e)
        {
            if (!isFirstLoad)
                GeneralOperations.ToggleDebugMode(chkDebugMod.Checked);
        }

    }
}
