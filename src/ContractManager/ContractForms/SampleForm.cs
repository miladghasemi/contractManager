using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ContractManager.ContractForms
{
    public delegate void applyChanges();
    public partial class SampleForm : UserControl  , IContract
    {
        public Dictionary<string, string> allAttributes { get; private set; }
        public string formName { get; private set; }

        public applyChanges applycng;

        public bool allDataAreSaved { get
            {
                foreach (string name in allAttributes.Keys)
                {
                    if (allAttributes[name] != userInputs[name].Text)
                    {
                        return false;
                    }                    
                }
                return true;
            }
        }

        private Dictionary<string, TextBox> userInputs;
        public SampleForm(Dictionary<string, string> initAttributes=null)
        {
            InitializeComponent();
            formName = "فرم نمونه";
            allAttributes = new Dictionary<string, string>();
            userInputs = new Dictionary<string, TextBox>();
            
            userInputs.Add("نام", nameTxtBx);        
            userInputs.Add("نام آزمایش", textBox1);
            if (initAttributes!= null)
            {
                allAttributes.Add("نام", initAttributes["نام"]);
                allAttributes.Add("نام آزمایش", initAttributes["نام آزمایش"]);

                foreach (var elm in allAttributes.Keys.ToList())
                {
                    userInputs[elm].Text = allAttributes[elm];
                }
            }
            else
            {
                allAttributes.Add("نام", "");
                allAttributes.Add("نام آزمایش", "");

            }

        }

        private void applyBtn_Click(object sender, EventArgs e)
        {
            foreach(var elm in allAttributes.Keys.ToList())
            {
                allAttributes[elm] = userInputs[elm].Text;
            }
            if (applycng != null)
            {
                applycng();
            }
            
        }
    }
}
