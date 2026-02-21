using System;
using System.Net;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using WinSCP;

namespace CentralTestTPL
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }


        private void ShowError(string message) =>
        MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

        private void Login_Load(object sender, EventArgs e)
        {
            var appName = "CentralTestTPL";
            int app1Count = Process.GetProcessesByName(appName).Length;
            if (app1Count > 1)
            {
                MessageBox.Show("TPL is already running.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Application.Exit();
                return;
            }else{
                txtMachine.Focus();
            }
        }

        private void txtMachine_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)Keys.Enter) return;
            if (string.IsNullOrWhiteSpace(txtMachine.Text))
            {
                ShowError("Invalid Tester.\nPlease Scan again.");
                return;
            }
            else
            {
                var list = new DataAccess().selectMachine(txtMachine.Text);

                if (list.Count > 0) {
                    int app2Count = Process.GetProcessesByName(CentralTest.Application2).Length;
                    if (app2Count >= 1)
                    {
                        foreach (var proc in Process.GetProcessesByName(CentralTest.Application2))
                        {
                            try
                            {
                                proc.Kill();
                                proc.WaitForExit();
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"Failed to close process: {ex.Message}");
                            }
                        }
                    }
                    txtMachine.Enabled = false;
                    txtHandler.Enabled = true;
                    txtHandler.Focus();
                }
                else
                {
                    ShowError("Tester not found in Database.\nPlease Scan again.");
                    txtMachine.Clear();
                }
            }
        }

        private void txtUsername_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)Keys.Enter) return;
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                ShowError("Invalid Employee Number.\nPlease Scan again.");
                return;
            }
            else
            {
                var list = new DataAccess().selectUser(txtUsername.Text);
                if (list.Count > 0)
                {
                    this.Hide();
                    new Main().Show();
                }
                else
                {
                    ShowError("Employee Number not found in Database.\nPlease Scan again.");
                    txtUsername.Clear();
                }
            }

        }

        private void txtHandler_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)Keys.Enter) return;
            if (string.IsNullOrWhiteSpace(txtHandler.Text))
            {
                ShowError("Invalid Handler.\nPlease Scan again.");
                return;
            }
            else
            {
                if(txtHandler.Text != CentralTest.Handler)
                {
                    ShowError("Invalid Handler.\nPlease Scan again.");
                    txtHandler.Clear();
                }
                else
                {
                    txtHandler.Enabled = false;
                    txtUsername.Enabled = true;
                    txtUsername.Focus();
                }
            }
        }
    }
}
