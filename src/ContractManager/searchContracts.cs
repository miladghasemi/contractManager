using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ContractManager
{
    public partial class searchContracts : Form
    {
        public string contractPerson;
        public string tname;
        public string tver;
        public string date;
        public string user;
        public searchContracts(string user, string access)
        {
            InitializeComponent();
            SQLiteConnection m_dbConnection;
            m_dbConnection =
                new SQLiteConnection("Data Source=contractManager.sqlite;Version=3;");
            m_dbConnection.Open();
            string sql;
            if (access != "admin")
            {
                sql = "select * from Contracts where UserName='" + user + "' order by ID";
            }
            else
            {
                sql = "select * from Contracts order by ID";
            }
            
            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                string contractPerson = (string)reader["Name"];
                string tname = (string)reader["TemplateName"];
                string tver = (string)reader["TemplateVersion"];
                string date = (string)reader["Date"];
                string usr = (string)reader["UserName"];
                dataGridView1.Rows.Add(contractPerson, tname, tver, date, usr);
            }
            m_dbConnection.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 1)
            {
                contractPerson = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();
                tname = dataGridView1.SelectedRows[0].Cells[1].Value.ToString();
                tver = dataGridView1.SelectedRows[0].Cells[2].Value.ToString();
                date = dataGridView1.SelectedRows[0].Cells[3].Value.ToString();
                user = dataGridView1.SelectedRows[0].Cells[4].Value.ToString();

                this.DialogResult = DialogResult.Yes;
                this.Close();
            }
            else
            {
                MessageBox.Show("you should select one item.");
            }
        }
    }
}
