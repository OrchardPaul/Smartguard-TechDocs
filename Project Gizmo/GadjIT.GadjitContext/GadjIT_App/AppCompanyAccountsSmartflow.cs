using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GadjIT.GadjitContext.GadjIT_App
{
    public partial class AppCompanyAccountsSmartflow
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CompanyId { get; set; }

        public decimal SmartflowCost { get; set; }

        public decimal Subscription { get; set; }

        public decimal TotalBilled { get; set; }
    }
}
