using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractManager.ContractForms
{
    interface IContract
    {
        Dictionary<string, string> allAttributes { get;}
        string formName { get; }

        bool allDataAreSaved { get; }
    }
}
