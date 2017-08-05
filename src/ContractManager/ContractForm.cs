using ContractManager.ContractForms;
using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
        private UserControl myControl;
        private MemoryStream ContractInMemory;
        private string TemplatesFolder = @"C:\Users\milad\Desktop\Abbas\Templates\";
        private static string OutputsFolder = @"C:\Users\milad\Desktop\Abbas\Outputs\";
        private static string contractName = "قرارداد آزمایشگاه" + ".docx";
        private string contractNameAsPDF = OutputsFolder + "قرارداد آزمایشگاه" + ".pdf";
        private string contractNameAsWord = OutputsFolder + "قرارداد آزمایشگاه" + ".docx";
        private string contractTempFile = Path.Combine(Path.GetTempPath(), contractName);
        private Document wordDocument;

        public ContractForm()
        {
            InitializeComponent();
            ContractInMemory = new MemoryStream();

            if (File.Exists(contractTempFile))
            {
                File.Delete(contractTempFile);
            }
           

            using (FileStream fs = File.OpenRead(TemplatesFolder + contractName  ))
            {
                fs.CopyTo(ContractInMemory);
            }

            FileStream file = new FileStream(contractTempFile, FileMode.Create, FileAccess.Write);
            ContractInMemory.WriteTo(file);
            file.Close();

            myControl = new SampleForm(contractName);
            ((SampleForm)myControl).applycng += applyConfig;
            FillDataPanel.Controls.Add(myControl);



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
    }
}
