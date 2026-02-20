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


namespace CentralTestTPL
{
    public partial class Main : Form
    {
        private static string baseDirectory = AppDomain.CurrentDomain.BaseDirectory + "\\";
        private static string testProgFile = Path.Combine(baseDirectory, "TestProg.txt");
        private static string CustomerCodeFilePath = Path.Combine(baseDirectory, "CustomerCode.txt");
        private static string LoadTestProg = Path.Combine(baseDirectory, "LaunchApp.exe");

        public Main()
        {
            InitializeComponent();
            if (!File.Exists(testProgFile)) File.WriteAllText(testProgFile, "");
            if (!File.Exists(CustomerCodeFilePath)) File.WriteAllText(CustomerCodeFilePath, "");
        }

        private void ShowError(string message) =>
        MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

        private void Main_Load(object sender, EventArgs e)
        {
            txtLotnumber.Focus();
        }

        private void SetControlState(Control disableCtrl, Control enableCtrl, bool setFocus = true)
        {
            disableCtrl.Enabled = false;
            enableCtrl.Enabled = true;

            if (setFocus)
                enableCtrl.Focus();
        }

        private void txtLotnumber_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)Keys.Enter) return;
            if (string.IsNullOrWhiteSpace(txtLotnumber.Text))
            {
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

                        bool CORRexists = Directory.EnumerateFiles(CentralTest.EngDatalogPath)
                        .Any(file =>
                        {
                            string name = Path.GetFileName(file);
                            return allowedExtensions.Contains(Path.GetExtension(name), StringComparer.OrdinalIgnoreCase)
                                && name.StartsWith(LotInfo.LotNumber + "-CORR", StringComparison.OrdinalIgnoreCase);
                        });

                        bool BINCONexists = Directory.EnumerateFiles(CentralTest.EngDatalogPath)
                        .Any(file =>
                        {
                            string name = Path.GetFileName(file);
                            return allowedExtensions.Contains(Path.GetExtension(name), StringComparer.OrdinalIgnoreCase)
                                && name.StartsWith(LotInfo.LotNumber + "-BINCON", StringComparison.OrdinalIgnoreCase);
                        });

                        if (!CORRexists)
                        {
                            ShowError("Please Perform Test Correlation.");
                            return;
                        } else if (!BINCONexists) {
                            ShowError("Please Perform Binning Consistency Check.");
                            return;
                        }
                            else
                        {
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
                            //SetControlState(txtLotnumber, txtTestProgram);
                            txtLBoard.Enabled = true;
                            txtLBoard.Focus();
                        }
                    }
                    else {
                        txtLotnumber.Clear();
                        ShowError("Operator not qualified.");
                        return;
                    }


                } else {
                    ShowError("Lot not found.\nPlease Scan again.");
                    txtLotnumber.Clear();
                    return;
                }
            }
        }

        private void HandleScan(KeyPressEventArgs e, TextBox currentTextBox, string expectedValue, TextBox nextTextBox, string itemName)
        {
            if (string.IsNullOrWhiteSpace(currentTextBox.Text))
            {
                ShowError($"Invalid {itemName}.\nPlease Scan again.");
                return;
            }

            if (!expectedValue.StartsWith(currentTextBox.Text, StringComparison.OrdinalIgnoreCase))
            {
                ShowError($"{itemName} not Match in Database.\nPlease Scan again.");
                currentTextBox.Clear();
                return;
            }

            currentTextBox.Enabled = false;
            if (nextTextBox != null)
            {
                nextTextBox.Enabled = true;
                nextTextBox.Focus();
            }
        }

        private void txtLBoard_KeyPress(object sender, KeyPressEventArgs e)
 {
            if (e.KeyChar != (char)Keys.Enter) return;
            HandleScan(e, txtLBoard, LotInfo.LBoard, txtHIBs, "Load Board");
        }

        private void txtHIBs_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)Keys.Enter) return;
            HandleScan(e, txtHIBs, LotInfo.Hibs, txtCable, "HIBs");
        }

        private void txtCable_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)Keys.Enter) return;
            HandleScan(e, txtCable, LotInfo.TPLCable, txtCarrierLot, "Cable");
        }

        private void txtCarrierLot_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)Keys.Enter) return;
            if (string.IsNullOrWhiteSpace(txtCarrierLot.Text))
            {
                ShowError("Invalid Carrier Lot.\nPlease Scan again.");
                return;
            }
            else { }
        }

        private void txtCoverLot_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)Keys.Enter) return;
            if (string.IsNullOrWhiteSpace(txtCoverLot.Text))
            {
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

        private void txtTestProgram_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)Keys.Enter) return;

            if (cmbTestProg.SelectedItem?.ToString() != txtTestProgram.Text) {
                ShowError("Invalid Testprogram not match.\nPlease Scan again.");
                txtTestProgram.Clear();
                return;
            } else {
                txtTestProgram.Enabled = false;

                btnLaunch.Enabled = true;
                btnLaunch.BackColor = Color.Green;
            }
        }

        private void btnLaunch_Click(object sender, EventArgs e)
        {

        }

    }
}
