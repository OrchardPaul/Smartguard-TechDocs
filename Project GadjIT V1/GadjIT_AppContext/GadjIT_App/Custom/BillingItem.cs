using System;
using System.Collections.Generic;
using System.Text;

namespace GadjIT_AppContext.GadjIT_App.Custom
{
    public class BillingItem
    {
        public string AccountName { get; set; }

        public string SmartflowCaseTypeGroup { get; set; }

        public string SmartflowCaseType { get; set; }

        public string SmartflowName { get; set; }

        public decimal Outstanding { get; set; }

        public decimal Billed { get; set; }

        public DateTime BillDate { get; set; }

        public decimal MonthlyCharge { get; set; }
        
        public int MonthsRemaing { get; set; }


    }
}
