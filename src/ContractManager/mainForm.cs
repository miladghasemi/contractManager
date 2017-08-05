using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ContractManager
{
    public partial class mainForm : Form
    {
        private string user;
        public mainForm(string user, string access)
        {
            InitializeComponent();
            this.user = user;
            if (access == "admin")
            {
                FooterTxbx.Text =  "user name: " + user +".logined as admin";
            }else if (access == "user")
            {
                FooterTxbx.Text = "user name: " + user + ".";
                button3.Enabled = false;
                button4.Enabled = false;


            }
        }

        private void mainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ContractSelect frm = new ContractSelect();
            frm.ShowDialog();
            if (frm.DialogResult == DialogResult.Yes)
            {
                ContractForm frm2 = new ContractForm(frm.SelectedContract, frm.Selectedversion, user);
                frm.Close();
                frm2.ShowDialog();
                frm2.Close();
            }
           
        }
    }
}
