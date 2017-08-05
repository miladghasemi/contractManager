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
    public partial class SaveContractToDB : Form
    {
        public string contractNameToSave { get; set; }
        public SaveContractToDB(string TName, string TVersion, string User)
        {
            InitializeComponent();


            textBox2.Text = TName;
            textBox3.Text = TVersion;
            textBox4.Text = User;
            textBox5.Text = DateTime.Now.Date.ToString();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            contractNameToSave = textBox1.Text;
        }
    }
}
