using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GadjIT.GadjitContext.GadjIT_App
{
    public partial class AppCompanyAccountsSmartflowDetails
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int SmartflowAccountId { get; set; }

        [Required]
        public int SmartflowRecordId { get; set; }

        public string Status { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int Duration { get; set; }

        public int MonthsRemaining { get; set; }

        public decimal MonthlyCharge { get; set; }

        public decimal TotalBilled { get; set; }

        public decimal Outstanding { get; set; }
    }
}
