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
    public partial class ContractSelect : Form
    {
        public string SelectedContract { get; set; }
        public ContractSelect()
        {
            InitializeComponent();
            SQLiteConnection m_dbConnection;
            m_dbConnection =
                new SQLiteConnection("Data Source=contractManager.sqlite;Version=3;");
            m_dbConnection.Open();
            string sql = "select * from Templates order by Name desc";
            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                string contract = (string)reader["Name"];
                string version = (string)reader["Version"];
                dataGridView1.Rows.Add(contract,version);
            }
            m_dbConnection.Close();

            SelectedContract = "hello";
        }
    }
}
