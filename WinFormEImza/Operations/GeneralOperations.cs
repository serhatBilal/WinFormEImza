extern alias merged;
using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using WinFormEImza.Objects;

namespace WinFormEImza.Operations
{
    public class GeneralOperations : CadesSampleBase
    {
        public const string SifrePublicKey = "ZzZ";
        public static byte[] SifreSalt = Encoding.ASCII.GetBytes("ZzzzzzzZ");
        public static string ROOT_DIR = @"C:\Users\Serha\OneDrive\Masaüstü\WinFormEImza\WinFormEImza\WinFormEImza";
        public static string SettingsFile = ROOT_DIR + @"\config\Settings.xml";
        // gets only qualified certificates in smart card
        public static readonly bool IS_QUALIFIED = true;
        // the pin of the smart card
        public static string PIN_SMARTCARD = "";
        public static bool IsDontAskPinAllDay = false;
        public static bool IsDebugMode = true;


        protected static string GetRootDir()
        {
            return ROOT_DIR;
        }

        public static string GetPin()
        {
            return PIN_SMARTCARD;
        }

        public static void SetPin(string s)
        {
            PIN_SMARTCARD = s;
        }

        public static bool IsQualified()
        {
            return IS_QUALIFIED;
        }

        public static void LogWrite(string p)
        {
            if (Application.OpenForms.Count > 0)
            {
                WinFormEImza mainForm = (WinFormEImza)Application.OpenForms[0];
                ListBox lstLog = (ListBox)mainForm.Controls.Find("lstBoxLog", false)[0];
                lstLog.Invoke((MethodInvoker)delegate
                {
                    lstLog.Items.Add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " | " + p);
                });

                //// lstLog.Items.Add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " | " + p);
            }
        }

        public static void CheckCertificatePublicSM(bool isRedownload)
        {
            try
            {
                string path = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\.sertifikadeposu";
                Directory.CreateDirectory(path);
                if (isRedownload)
                {
                    if (File.Exists(path + @"\SertifikaDeposu.svt"))
                        File.Move(path + @"\SertifikaDeposu.svt", path + @"\SertifikaDeposu-" + string.Format("{0:yyyyMMddHHmmssfffffff}", DateTime.Now) + ".svt");

                    if (File.Exists(path + @"\SertifikaDeposu.xml"))
                        File.Move(path + @"\SertifikaDeposu.xml", path + @"\SertifikaDeposu-" + string.Format("{0:yyyyMMddHHmmssfffffff}", DateTime.Now) + ".xml");
                }
                if (!File.Exists(path + @"\SertifikaDeposu.svt"))
                {
                    using (WebClient client = new WebClient())
                    {
                        client.DownloadFile("http://depo.kamusm.gov.tr/depo/SertifikaDeposu.svt", path + @"\SertifikaDeposu.svt");
                    }
                }
                if (!File.Exists(path + @"\SertifikaDeposu.xml"))
                {
                    using (WebClient client = new WebClient())
                    {
                        client.DownloadFile("http://depo.kamusm.gov.tr/depo/SertifikaDeposu.xml", path + @"\SertifikaDeposu.xml");
                    }
                }
                if (isRedownload)
                {
                    GeneralOperations.LogWrite(" Certificate repositories downloaded...");
                }
                else
                {
                    GeneralOperations.LogWrite(" Certificate repositories checked...");
                }
            }
            catch (Exception ex)
            {
                GeneralOperations.LogWrite(" ERROR: [CheckCertificatePublicSM] (" + ex.Message + ")");
            }
        }

        public static string Encrypt(string s)
        {
            Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(SifrePublicKey, SifreSalt);
            MemoryStream ms = new MemoryStream();
            StreamWriter sw = new StreamWriter(new CryptoStream(ms, new RijndaelManaged().CreateEncryptor(key.GetBytes(32), key.GetBytes(16)), CryptoStreamMode.Write));
            sw.Write(s);
            sw.Close();
            ms.Close();
            return Convert.ToBase64String(ms.ToArray());
        }

        public static string Decrypt(string s)
        {
            Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(SifrePublicKey, SifreSalt);
            ICryptoTransform d = new RijndaelManaged().CreateDecryptor(key.GetBytes(32), key.GetBytes(16));
            byte[] bytes = Convert.FromBase64String(s);
            return new StreamReader(new CryptoStream(new MemoryStream(bytes), d, CryptoStreamMode.Read)).ReadToEnd();
        }

        internal static void ToggleDebugMode(bool v)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(SettingsFile);
            xmlDoc.SelectSingleNode("/ROOT/DebugModeEnabled").InnerText = v ? "1" : "0";
            xmlDoc.Save(SettingsFile);
            IsDontAskPinAllDay = v;
        }
      
        public static string ShowPasswordInputBox(string Prompt)
        {
            Form frmInput = new Form()
            {
                Size = new Size(230, 120),
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                Text = Prompt
            };
            Button btn = new Button()
            {
                Text = "OK",
                Location = new System.Drawing.Point(100, 40),
                Width = 80                
            };
            btn.Click += inputclose;
            TextBox txtbox = new TextBox()
            {
                Width = 170,
                Location = new System.Drawing.Point(10, 10),
                PasswordChar = '*'
            };
            frmInput.Controls.Add(btn);
            frmInput.Controls.Add(txtbox);
            frmInput.ShowDialog();
            return txtbox.Text;
        }

        public static void inputclose(object s, EventArgs e)
        {
            ((Form)(((Control)s).Parent)).Close();
        }

        internal static void LoadSettingsFile()
        {
            XmlDocument xmlDoc = new XmlDocument();
            if (!File.Exists(SettingsFile))
            {
                xmlDoc.LoadXml(@"<?xml version=""1.0"" encoding=""utf-8"" ?><ROOT><DontAskPinAllDay></DontAskPinAllDay><DebugModeEnabled></DebugModeEnabled><Pin></Pin><Tarih></Date></ROOT>");
                xmlDoc.Save(SettingsFile);
            }
            xmlDoc.Load(SettingsFile);
            string strTarih = xmlDoc.SelectSingleNode("/ROOT/Date").InnerText;
            if (!string.IsNullOrEmpty(xmlDoc.SelectSingleNode("/ROOT/DebugModeEnabled").InnerText))
            {
                IsDebugMode = Convert.ToBoolean(int.Parse(xmlDoc.SelectSingleNode("/ROOT/DebugModeEnabled").InnerText));
            }
            if (DateTime.Now.ToString("yyyyMMdd") == strTarih)
            {
                IsDontAskPinAllDay = Convert.ToBoolean(int.Parse(xmlDoc.SelectSingleNode("/ROOT/DontAskPinAllDay").InnerText));
                if (IsDontAskPinAllDay && !string.IsNullOrEmpty(xmlDoc.SelectSingleNode("/ROOT/Pin").InnerText))
                {
                    SetPin(Decrypt(xmlDoc.SelectSingleNode("/ROOT/Pin").InnerText));
                }
            }
        }
        internal static void DontAskPinAllDay(bool v)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(SettingsFile);
            xmlDoc.SelectSingleNode("/ROOT/DontAskPinAllDay").InnerText = v ? "1" : "0";
            xmlDoc.SelectSingleNode("/ROOT/Date").InnerText = DateTime.Now.ToString("yyyyMMdd");
            xmlDoc.SelectSingleNode("/ROOT/Pin").InnerText = "";
            xmlDoc.Save(SettingsFile);
            IsDontAskPinAllDay = v;
        }
        internal static void SavePinToXml(string pin)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(SettingsFile);
            xmlDoc.SelectSingleNode("/ROOT/Date").InnerText = DateTime.Now.ToString("yyyyMMdd");
            xmlDoc.SelectSingleNode("/ROOT/Pin").InnerText = Encrypt(pin);
            xmlDoc.Save(SettingsFile);
        }

    }
}
