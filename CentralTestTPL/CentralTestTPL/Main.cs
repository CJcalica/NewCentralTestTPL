using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CentralTestTPL
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private void ShowError(string message) =>
        MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

        private void Main_Load(object sender, EventArgs e)
        {
            txtLotnumber.Focus();
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

            }
        }








    }
}
