using System;
using System.Collections.Generic;
using System.Text;

namespace GadjIT.AppContext.GadjIT_App.Custom
{
    public class CompanyAccountObject
    {
        public AppCompanyDetails CompanyObject { get; set; }

        public AppCompanyAccountsSmartflow AccountObject { get; set; }

        public List<AppCompanyAccountsSmartflowDetails> SmartflowAccounts { get; set; }
    }
}
