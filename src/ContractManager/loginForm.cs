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
    public partial class loginForm : Form
    {
        public loginForm()
        {
            InitializeComponent();
        }

        private void LoginBtn_Click(object sender, EventArgs e)
        {
            SQLiteConnection m_dbConnection;
            m_dbConnection =
                new SQLiteConnection("Data Source=contractManager.sqlite;Version=3;");
            m_dbConnection.Open();
            string sql = "select * from Users where UserName='"+ userTxbx.Text + "'";
            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            SQLiteDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                string pass = (string)reader["Password"];
                string usr = (string)reader["UserName"];
                string acc = (string)reader["Access"];
                m_dbConnection.Close();

                if (pass == PassTxbx.Text)
                {
                    mainForm Check = new mainForm(usr, acc);
                    Check.Show();
                    Hide();
                }else
                {
                    MessageBox.Show("password is not correct!");
                }
                //MessageBox.Show("Name: " + reader["UserName"] + "\tpassword: " + reader["Password"]);

            }else
            {
                MessageBox.Show("username not found!");
                m_dbConnection.Close();
            }
        }
    }
}
