using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;

namespace CentralTestTPL
{
    public partial class Main : Form
    {
        private static string baseDirectory = AppDomain.CurrentDomain.BaseDirectory + "\\";
        private static string AutoFillDetails = Path.Combine(baseDirectory, "AutoFillDetails.txt");
        private static string CustomerCodeFilePath = Path.Combine(baseDirectory, "CustomerCode.txt");
        private static string LoadTestProg = Path.Combine(baseDirectory, "LaunchApp.exe");

        public Main()
        {
            InitializeComponent();
            if (!File.Exists(AutoFillDetails)) File.WriteAllText(AutoFillDetails, "");
            if (!File.Exists(CustomerCodeFilePath)) File.WriteAllText(CustomerCodeFilePath, "");
        }

        private void ShowError(string message) =>
        MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        
        private void Main_Load(object sender, EventArgs e)
        {
            txtLotnumber.Focus();
        }

        static string GetLocalIPAddress()
        {
            string hostName = Dns.GetHostName();
            IPAddress[] add = Dns.GetHostAddresses(hostName);
            foreach (var ip in add)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No IPv4 address found!");
        }

        private void SetControlState(Control disableCtrl, Control enableCtrl, bool setFocus = true)
        {
            disableCtrl.Enabled = false;
            enableCtrl.Enabled = true;

            if (setFocus)
                enableCtrl.Focus();
        }

        bool FileExistsOnFtp(string ftpPath, string lotNumber, string keyword, string[] allowedExtensions)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpPath);
            request.Method = WebRequestMethods.Ftp.ListDirectory;

            // if FTP requires login
            request.Credentials = new NetworkCredential(CentralTest.Username, CentralTest.Password);
            request.UsePassive = true;
            request.UseBinary = true;
            request.KeepAlive = false;

            using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
            using (Stream responseStream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(responseStream))
            {
                while (!reader.EndOfStream)
                {
                    string name = reader.ReadLine();
                    string ext = Path.GetExtension(name);

                    if (allowedExtensions.Contains(ext, StringComparer.OrdinalIgnoreCase) &&
                        name.StartsWith(lotNumber + "-" + keyword, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private void txtLotnumber_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)Keys.Enter) return;
            if (string.IsNullOrWhiteSpace(txtLotnumber.Text))
            {
                DataAccess da = new DataAccess();
                da.insertMasterLogs("Invalid Lot.", txtLotnumber.Text, "", "", CentralTest.MachineName, GetLocalIPAddress());
                ShowError("Invalid Lot.\nPlease Scan again.");
                return;
            }
            else
            {
                var list = new DataAccess().SelectLotInfo(txtLotnumber.Text);
                if (list.Count > 0) {
                    var allowedCodes = User.CustomerCodes.Split(',');
                    if (allowedCodes.Contains(LotInfo.CustomerCode.ToString())) {

                        string[] allowedExtensions = { ".dl4", ".lsr", ".spd", ".std", ".txt" };

                        bool CORRexists = FileExistsOnFtp(CentralTest.EngDatalogPath, LotInfo.LotNumber, "CORR", allowedExtensions);
                        if (!CORRexists)
                        {
                            DataAccess da = new DataAccess();
                            da.insertMasterLogs("Please Perform Test Correlation.", txtLotnumber.Text, LotInfo.CustomerCode.ToString(), CentralTest.EngDatalogPath, CentralTest.MachineName, GetLocalIPAddress());
                            ShowError("Please Perform Test Correlation.");
                            return;
                        }

                        bool BINCONexists = FileExistsOnFtp(CentralTest.EngDatalogPath, LotInfo.LotNumber, "BINCON", allowedExtensions);
                        if (!BINCONexists) {
                            DataAccess da = new DataAccess();
                            da.insertMasterLogs("Please Perform Binning Consistency Check.", txtLotnumber.Text, LotInfo.CustomerCode.ToString(), CentralTest.EngDatalogPath, CentralTest.MachineName, GetLocalIPAddress());
                            ShowError("Please Perform Binning Consistency Check.");
                            return;
                        }

                        txtDetailLotnumber.Text = LotInfo.LotNumber;
                        txtDetailQty.Text = LotInfo.SubLotQty.ToString();
                        txtDetailDevice.Text = LotInfo.Device;
                        txtDetailProduct.Text = LotInfo.ProductID;
                        txtDetailPkg.Text = LotInfo.PkgLD;
                        txtDetailLead.Text = LotInfo.LdType;
                        File.WriteAllText(CustomerCodeFilePath, LotInfo.CustomerCode.ToString());
                        txtCarrierID.Text = LotInfo.CarrierTape;
                        txtCoverID.Text = LotInfo.CoverTape;
                        txtReelID.Text = LotInfo.Reel;

                        // Clear existing items first
                        cmbTestProg.Items.Clear();
                        // Add FinalTestProgram if it exists
                        if (!string.IsNullOrWhiteSpace(LotInfo.FinalTestProgram))
                        {
                            cmbTestProg.Items.Add(LotInfo.FinalTestProgram);
                        }
                        // Add QAFinalTestProgram if it exists and is different
                        if (!string.IsNullOrWhiteSpace(LotInfo.QATestProgram) &&
                            LotInfo.QATestProgram != LotInfo.FinalTestProgram)
                        {
                            cmbTestProg.Items.Add(LotInfo.QATestProgram);
                        }
                        txtLotnumber.Enabled = false;
                        //SetControlState(txtLotnumber, txtTestProgram);
                        txtLBoard.Enabled = true;
                        txtLBoard.Focus();

                    }
                    else {
                        txtLotnumber.Clear();
                        DataAccess da = new DataAccess();
                        da.insertMasterLogs("Operator not qualified. " + User.Emp_No, txtLotnumber.Text, LotInfo.CustomerCode.ToString(), "", CentralTest.MachineName, GetLocalIPAddress());
                        ShowError("Operator not qualified.");
                        return;
                    }


                } else {
                    DataAccess da = new DataAccess();
                    da.insertMasterLogs("Lot not found.", txtLotnumber.Text, LotInfo.CustomerCode.ToString(), "", CentralTest.MachineName, GetLocalIPAddress());
                    ShowError("Lot not found.\nPlease Scan again.");
                    txtLotnumber.Clear();
                    return;
                }
            }
        }

        private bool IsInRange(string scanned, string rangeText)
        {
            var parts = rangeText.Split(',');

            if (parts.Length != 2)
                return false;

            string start = parts[0].Trim();
            string end = parts[1].Trim();

            string prefixStart = new string(start.TakeWhile(c => !char.IsDigit(c)).ToArray());
            string prefixEnd = new string(end.TakeWhile(c => !char.IsDigit(c)).ToArray());
            string prefixScan = new string(scanned.TakeWhile(c => !char.IsDigit(c)).ToArray());

            if (prefixStart != prefixEnd || prefixScan != prefixStart)
                return false;

            int numStart = int.Parse(new string(start.Where(char.IsDigit).ToArray()));
            int numEnd = int.Parse(new string(end.Where(char.IsDigit).ToArray()));
            int numScan = int.Parse(new string(scanned.Where(char.IsDigit).ToArray()));

            return numScan >= numStart && numScan <= numEnd;
        }

        private void HandleScan(KeyPressEventArgs e, TextBox currentTextBox, string expectedValue, TextBox nextTextBox, string itemName, bool matgroup)
        {
            if (string.IsNullOrWhiteSpace(currentTextBox.Text))
            {
                DataAccess da = new DataAccess();
                da.insertMasterLogs($"Invalid {itemName}.", txtLotnumber.Text, LotInfo.CustomerCode.ToString(), "", CentralTest.MachineName, GetLocalIPAddress());
                ShowError($"Invalid {itemName}.\nPlease Scan again.");
                return;
            }

            if (!IsInRange(currentTextBox.Text.Trim(), expectedValue))
            {
                DataAccess da = new DataAccess();
                da.insertMasterLogs($"{itemName} not Match in Database.", txtLotnumber.Text, LotInfo.CustomerCode.ToString(), "", CentralTest.MachineName, GetLocalIPAddress());
                ShowError($"{itemName} not Match in Database.\nPlease Scan again.");
                currentTextBox.Clear();
                return;
            }

            currentTextBox.Enabled = false;
            groupBox3.Enabled = matgroup;

            if (nextTextBox != null)
            {
                nextTextBox.Enabled = true;
                nextTextBox.Focus();
            }
        }

        private void txtLBoard_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)Keys.Enter) return;
            HandleScan(e, txtLBoard, LotInfo.LBoard, txtHIBs, "Load Board", false);
        }

        private void txtHIBs_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)Keys.Enter) return;
            HandleScan(e, txtHIBs, LotInfo.Hibs, txtCable, "HIBs", false);
        }

        private void txtHIBs2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)Keys.Enter) return;
            HandleScan(e, txtHIBs, LotInfo.Hibs, txtCable2, "HIBs", false);
        }

        private void txtHIBs3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)Keys.Enter) return;
            HandleScan(e, txtHIBs, LotInfo.Hibs, txtCable3, "HIBs", false);
        }

        private void txtHIBs4_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)Keys.Enter) return;
            HandleScan(e, txtHIBs, LotInfo.Hibs, txtCable4, "HIBs", false);
        }

        private void txtCable_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)Keys.Enter) return;
            HandleScan(e, txtCable, LotInfo.TPLCable, txtHIBs2, "Cable", true);
        }

        private void txtCable2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)Keys.Enter) return;
            HandleScan(e, txtCable, LotInfo.TPLCable, txtHIBs3, "Cable", true);
        }

        private void txtCable3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)Keys.Enter) return;
            HandleScan(e, txtCable, LotInfo.TPLCable, txtHIBs4, "Cable", true);
        }

        private void txtCable4_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)Keys.Enter) return;
            HandleScan(e, txtCable, LotInfo.TPLCable, txtCarrierLot, "Cable", true);
        }

        private void txtCarrierLot_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)Keys.Enter) return;
            if (string.IsNullOrWhiteSpace(txtCarrierLot.Text))
            {
                DataAccess da = new DataAccess();
                da.insertMasterLogs($"Invalid Carrier Lot. " + txtCarrierLot.Text, txtLotnumber.Text, LotInfo.CustomerCode.ToString(), "", CentralTest.MachineName, GetLocalIPAddress());
                ShowError("Invalid Carrier Lot.\nPlease Scan again.");
                return;
            }
            else {
                groupBox2.Enabled = false;
            }
        }

        private void txtCoverLot_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)Keys.Enter) return;
            if (string.IsNullOrWhiteSpace(txtCoverLot.Text))
            {
                DataAccess da = new DataAccess();
                da.insertMasterLogs($"Invalid Cover Lot. " + txtCoverLot.Text, txtLotnumber.Text, LotInfo.CustomerCode.ToString(), "", CentralTest.MachineName, GetLocalIPAddress());
                ShowError("Invalid Cover Lot.\nPlease Scan again.");
                return;
            }
            else { }
        }

        private void txtReelLot_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)Keys.Enter) return;
            if (string.IsNullOrWhiteSpace(txtReelLot.Text))
            {
                DataAccess da = new DataAccess();
                da.insertMasterLogs($"Invalid Reel Lot. " + txtReelLot.Text, txtLotnumber.Text, LotInfo.CustomerCode.ToString(), "", CentralTest.MachineName, GetLocalIPAddress());
                ShowError("Invalid Reel Lot.\nPlease Scan again.");
                return;
            }
            else {
                cmbTestProg.Enabled = true;
                cmbTestProg.DroppedDown = true;
            }
        }

        private void cmbTestProg_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (txtTestProgram.Enabled == false)
            {
                txtTestProgram.Enabled = true;
                txtTestProgram.Focus();
            }
            else
            {
                txtTestProgram.Clear();
                txtTestProgram.Focus();
                btnLaunch.Enabled = false;
                btnLaunch.BackColor = Color.Gray;
            }
        }


        private void CleanLocalFolders(string basePath)
        {
            if (!Directory.Exists(basePath)) return;
            foreach (var folder in Directory.GetDirectories(basePath))
            {
                try
                {
                    Directory.Delete(folder, true);
                }
                catch (Exception ex)
                {
                    DataAccess da = new DataAccess();
                    da.insertMasterLogs($"Failed to delete folder: {folder} - {ex.Message}", txtLotnumber.Text, LotInfo.CustomerCode.ToString(), "", CentralTest.MachineName, GetLocalIPAddress());
                }
            }
        }

        static void DownloadAllFilesAndFolders(string ftpUrl, string localFolder, string username, string password)
        {
            try
            {
                // Step 1: List directory contents using ListDirectoryDetails (to check if entry is file/folder)
                FtpWebRequest listRequest = (FtpWebRequest)WebRequest.Create(ftpUrl);
                listRequest.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                listRequest.Credentials = new NetworkCredential(username, password);
                listRequest.UsePassive = true;
                listRequest.UseBinary = true;
                listRequest.KeepAlive = false;

                List<string> files = new List<string>();
                List<string> folders = new List<string>();

                using (FtpWebResponse listResponse = (FtpWebResponse)listRequest.GetResponse())
                using (Stream listStream = listResponse.GetResponseStream())
                using (StreamReader reader = new StreamReader(listStream))
                {
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        string[] parts = line.Split(new[] { ' ' }, 9, StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length < 9)
                            continue;

                        string name = parts[8];
                        string permissions = parts[0];

                        if (permissions.StartsWith("d")) // directory
                        {
                            if (name != "." && name != "..")
                                folders.Add(name);
                        }
                        else // file
                        {
                            files.Add(name);
                        }
                    }
                }
                // Step 2: Create local folder if needed
                if (!Directory.Exists(localFolder))
                    Directory.CreateDirectory(localFolder);
                // Step 3: Download files
                foreach (string file in files)
                {
                    string sourceUrl = ftpUrl + "/" + file;
                    string destinationPath = Path.Combine(localFolder, file);

                    Console.WriteLine("⬇ Downloading: " + file);

                    FtpWebRequest downloadRequest = (FtpWebRequest)WebRequest.Create(sourceUrl);
                    downloadRequest.Method = WebRequestMethods.Ftp.DownloadFile;
                    downloadRequest.Credentials = new NetworkCredential(username, password);
                    downloadRequest.UsePassive = true;
                    downloadRequest.UseBinary = true;
                    downloadRequest.KeepAlive = false;

                    using (FtpWebResponse response = (FtpWebResponse)downloadRequest.GetResponse())
                    using (Stream ftpStream = response.GetResponseStream())
                    using (FileStream fileStream = new FileStream(destinationPath, FileMode.Create))
                    {
                        byte[] buffer = new byte[10240];
                        int bytesRead;
                        while ((bytesRead = ftpStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            fileStream.Write(buffer, 0, bytesRead);
                        }
                    }
                }
                // Step 4: Recursively download folders
                foreach (string folder in folders)
                {
                    string remoteSubfolder = ftpUrl.TrimEnd('/') + "/" + folder;
                    string localSubfolder = Path.Combine(localFolder, folder);
                    DownloadAllFilesAndFolders(remoteSubfolder, localSubfolder, username, password);
                }
            }
            catch (Exception ex)
            {
                Global.hasTestProg = false;
                DataAccess da = new DataAccess();
                da.insertMasterLogs(ex.Message, "", "", "", CentralTest.MachineName, GetLocalIPAddress());
            }
        }

        private void txtTestProgram_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)Keys.Enter) return;

            if (cmbTestProg.SelectedItem?.ToString() != txtTestProgram.Text) {
                DataAccess da = new DataAccess();
                da.insertMasterLogs("Invalid Testprogram not match. " + txtTestProgram.Text, txtLotnumber.Text, LotInfo.CustomerCode.ToString(), "", CentralTest.MachineName, GetLocalIPAddress());
                ShowError("Invalid Testprogram not match.\nPlease Scan again.");
                txtTestProgram.Clear();
                return;
            } else {
                var loadForm = new Loading(); loadForm.Show();
                string FTPPath = LotInfo.FTPpath + CentralTest.Source2;
                CleanLocalFolders(CentralTest.Destination);
                if (!Directory.Exists(CentralTest.Destination)) Directory.CreateDirectory(CentralTest.Destination);

                Global.hasTestProg = true;
                DownloadAllFilesAndFolders(FTPPath + LotInfo.TestProgramFolder, CentralTest.Destination + "\\" + LotInfo.TestProgramFolder, CentralTest.Username, CentralTest.Password);

                if (!Global.hasTestProg) {
                    DataAccess da = new DataAccess();
                    da.insertMasterLogs("Test Program not available in the server. " + LotInfo.TestProgramFolder, txtLotnumber.Text, LotInfo.CustomerCode.ToString(), FTPPath, CentralTest.MachineName, GetLocalIPAddress());
                    ShowError("Test Program not available in the server.\nPlease contact engineer.");
                    txtTestProgram.Clear();
                }
                else
                {
                    File.WriteAllText(AutoFillDetails, string.Join(Environment.NewLine,
                    Path.GetFileNameWithoutExtension(txtTestProgram.Text),
                    LotInfo.TestProgramFolder,
                    txtTestProgram.Text,
                    LotInfo.LotNumber));
                    txtTestProgram.Enabled = false;
                    btnLaunch.Enabled = true;
                    btnLaunch.BackColor = Color.Green;
                }

                loadForm.Hide();
            }
        }

        private void btnLaunch_Click(object sender, EventArgs e)
        {
            DataAccess da = new DataAccess();
            bool success = da.StartTLPLogs(
                txtLotnumber.Text,
                txtTestProgram.Text,
                txtLBoard.Text,
                txtHIBs.Text,
                txtHIBs2.Text,
                txtHIBs3.Text,
                txtHIBs4.Text,
                txtCable.Text,
                txtCable2.Text,
                txtCable3.Text,
                txtCable4.Text,
                txtCarrierLot.Text,
                txtCoverLot.Text,
                txtReelLot.Text,
                CentralTest.MachineName,
                GetLocalIPAddress());
            if (success)
            {
                // Launch external process
                Process.Start(Path.Combine(baseDirectory, LoadTestProg));
                // Exit application
                Application.Exit();
            }
            else
            {
                da.insertMasterLogs("Failed to insert logs. Application will not launch. " + LotInfo.TestProgramFolder, txtLotnumber.Text, LotInfo.CustomerCode.ToString(), "", CentralTest.MachineName, GetLocalIPAddress());
                ShowError("Test Program not available in the server.\nPlease contact engineer.");
            }
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
    }
}
