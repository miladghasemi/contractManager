using ContractManager.ContractForms;
using Microsoft.Office.Interop.Word;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TemplateEngine.Docx;

namespace ContractManager
{
    public partial class ContractForm : Form
    {
        private string user;
        private string TName;
        private string TVer;
        private UserControl myControl;
        private MemoryStream ContractInMemory;
        private string TemplatesFolder = @"C:\Users\milad\Desktop\Abbas\Templates\";
        private static string OutputsFolder = @"C:\Users\milad\Desktop\Abbas\Outputs\";
        private static string contractName = "قرارداد آزمایشگاه" + ".docx";
        private string contractNameAsPDF = OutputsFolder + "قرارداد آزمایشگاه" + ".pdf";
        private string contractNameAsWord = OutputsFolder + "قرارداد آزمایشگاه" + ".docx";
        private string contractTempFile = Path.Combine(Path.GetTempPath(), contractName);
        private Document wordDocument;

        public ContractForm(string name, string ver,string user,string person=null,string Date=null)
        {
            InitializeComponent();
            this.user = user;
            this.TName = name;
            this.TVer = ver;


            contractName = name + ".docx";
            contractTempFile = Path.Combine(Path.GetTempPath(), contractName);

            ContractInMemory = new MemoryStream();

            if (File.Exists(contractTempFile))
            {
                File.Delete(contractTempFile);
            }

            SQLiteConnection m_dbConnection;
            m_dbConnection =
                new SQLiteConnection("Data Source=contractManager.sqlite;Version=3;");
            m_dbConnection.Open();
            string sql = "select * from Templates where Name='" + name + "' and Version='" + ver + "'";
            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            SQLiteDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                byte[] fileStream = (byte[])reader["File"];
                ContractInMemory = new MemoryStream(fileStream);
                FileStream file = new FileStream(contractTempFile, FileMode.Create, FileAccess.Write);
                ContractInMemory.WriteTo(file);
                file.Close();
                Dictionary<string, string> attrs = null;
                if (!String.IsNullOrEmpty(person) && !String.IsNullOrEmpty(Date))
                {

                    string sql2 = "select * from Contracts where Name='"
                        + person + "' and TemplateName='" + TName + "' and TemplateVersion='"
                        + TVer + "' and Date='" + Date + "' and UserName='" + user + "'";
                    SQLiteCommand command2 = new SQLiteCommand(sql2, m_dbConnection);
                    SQLiteDataReader reader2 = command2.ExecuteReader();
                    if (reader2.Read())
                    {
                        attrs = JsonConvert.DeserializeObject<Dictionary<string, string>>((string)reader2["Items"]);

                    }
                    

                }

                myControl = new SampleForm(attrs);
                ((SampleForm)myControl).applycng += applyConfig;
                FillDataPanel.Controls.Add(myControl);

                m_dbConnection.Close();

            }else
            {
                m_dbConnection.Close();
                Close();
            }
            

            //using (FileStream fs = File.OpenRead(TemplatesFolder + contractName  ))
            //{
            //    fs.CopyTo(ContractInMemory);
            //}           

        }


        private void applyConfig()
        {
            IContract cntrc = myControl as IContract;
            if (cntrc.allDataAreSaved == true)
            {
                FieldContent[] allItems = new FieldContent[cntrc.allAttributes.Count];

                int i = 0;
                foreach (var item in cntrc.allAttributes.Keys.ToList())
                {
                    allItems[i] = new FieldContent(item, cntrc.allAttributes[item]);
                    i++;
                }
                var valuesToFill = new Content(allItems);

                using (var outputDocument = new TemplateProcessor(contractTempFile)
                    .SetRemoveContentControls(true))
                {
                    outputDocument.FillContent(valuesToFill);
                    outputDocument.SaveChanges();
                }
            }
            else
            {
                MessageBox.Show("please save all fields and try again.");
            }
        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void exportAsWordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IContract cntrc = myControl as IContract;
            if (cntrc.allDataAreSaved == true)
            {
                if (File.Exists(contractNameAsWord))
                {
                    File.Delete(contractNameAsWord);
                }

                File.Copy(contractTempFile, contractNameAsWord);
            }
            else
            {
                MessageBox.Show("please save all fields and try again.");
            }
        }

        private void printToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IContract cntrc = myControl as IContract;
            if (cntrc.allDataAreSaved == true)
            {
                Microsoft.Office.Interop.Word.Application wordApp = new Microsoft.Office.Interop.Word.Application();
                wordApp.Visible = false;

                PrintDialog pDialog = new PrintDialog();
                if (pDialog.ShowDialog() == DialogResult.OK)
                {
                    Microsoft.Office.Interop.Word.Document doc = wordApp.Documents.Add(contractTempFile);
                    wordApp.ActivePrinter = pDialog.PrinterSettings.PrinterName;
                    wordApp.ActiveDocument.PrintOut(); //this will also work: doc.PrintOut();
                    doc.Close(SaveChanges: false);
                    doc = null;
                }

                // </EDIT>

                // Original: wordApp.Quit(SaveChanges: false);
                wordApp = null;
            }
            else
            {
                MessageBox.Show("please save all fields and try again.");
            }
        }

        private void exportAsPdfToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IContract cntrc = myControl as IContract;
            if (cntrc.allDataAreSaved == true)
            {
                Microsoft.Office.Interop.Word.Application appWord = new Microsoft.Office.Interop.Word.Application();
                wordDocument = appWord.Documents.Open(contractTempFile);
                wordDocument.ExportAsFixedFormat(contractNameAsPDF, WdExportFormat.wdExportFormatPDF);
            }
            else
            {
                MessageBox.Show("please save all fields and try again.");
            }
        }

        private void saveToDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IContract cntrc = myControl as IContract;
            if (cntrc.allDataAreSaved == true)
            {
                SaveContractToDB frm = new SaveContractToDB(TName, TVer, user);
                frm.ShowDialog();
                if (frm.DialogResult == DialogResult.OK)
                {
                    SQLiteConnection m_dbConnection;
                    m_dbConnection =
                        new SQLiteConnection("Data Source=contractManager.sqlite;Version=3;");
                    m_dbConnection.Open();
                    string sql = "select * from Contracts where Name='" 
                        + frm.contractNameToSave + "' and TemplateName='" + TName + "' and TemplateVersion='"
                        + TVer + "' and Date='" + DateTime.Now.Date.ToString() + "' and UserName='" + user + "'";
                    SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
                    SQLiteDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        DialogResult dialogResult = MessageBox.Show("Contract Exists.Do you want to replace it?", "Some Title", MessageBoxButtons.YesNo);
                        if (dialogResult == DialogResult.Yes)
                        {
                            byte[] allBytes = File.ReadAllBytes(contractTempFile);
                            SQLiteCommand editSQL = new SQLiteCommand(
                                "update Contracts set File = @file, Items = @itms where Name='"
                                + frm.contractNameToSave + "' and TemplateName='" + TName + "' and TemplateVersion='"
                                + TVer + "' and Date='" + DateTime.Now.Date.ToString() + "' and UserName='" + user + "'", m_dbConnection);
                            editSQL.Parameters.Add("@file", DbType.Binary, allBytes.Length).Value = allBytes;
                            editSQL.Parameters.Add("@itms", DbType.String).Value = Newtonsoft.Json.JsonConvert.SerializeObject(cntrc.allAttributes);
                            try
                            {
                                editSQL.ExecuteNonQuery();
                            }
                            catch (Exception ex)
                            {
                                throw new Exception(ex.Message);
                            }
                        }
                        m_dbConnection.Close();

                    }
                    else
                    {
                        byte[] allBytes = File.ReadAllBytes(contractTempFile);
                        SQLiteCommand insertSQL = new SQLiteCommand(
                            "INSERT INTO Contracts (Name , TemplateName , TemplateVersion , Date , File ,Items,UserName )" +
                            " VALUES (@name , @tname , @tver , @date , @file ,@itms,@uname )", m_dbConnection);
                        insertSQL.Parameters.Add("@name", DbType.String).Value = frm.contractNameToSave;
                        insertSQL.Parameters.Add("@tname", DbType.String).Value = TName;
                        insertSQL.Parameters.Add("@tver", DbType.String).Value = TVer;
                        insertSQL.Parameters.Add("@date", DbType.String).Value = DateTime.Now.Date.ToString();
                        insertSQL.Parameters.Add("@file", DbType.Binary, allBytes.Length).Value = allBytes;
                        insertSQL.Parameters.Add("@itms", DbType.String).Value = Newtonsoft.Json.JsonConvert.SerializeObject(cntrc.allAttributes);
                        insertSQL.Parameters.Add("@uname", DbType.String).Value = user;
                        try
                        {
                            insertSQL.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(ex.Message);
                        }
                        m_dbConnection.Close();
                    }
                    
                }
            }
            else
            {
                MessageBox.Show("please save all fields and try again.");
            }
        }
    }
}
