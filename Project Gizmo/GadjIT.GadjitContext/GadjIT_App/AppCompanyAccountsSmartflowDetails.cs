using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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

        [StringLength(50)]
        public string System { get; set; }

        public bool Billable { get; set; }

        [StringLength(200)]
        public string BillingDescription { get; set; }

        [StringLength(50)]
        public string Status { get; set; }

        [StringLength(100)]
        public string CreatedBy { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public DateTime DeletedDate { get; set; }

        public int MonthsDuration { get; set; }

        public int MonthsRemaining { get; set; }

        [Column(TypeName = "money")]
        public decimal MonthlyCharge { get; set; }

        [Column(TypeName = "money")]
        public decimal TotalBilled { get; set; }

        [Column(TypeName = "money")]
        public decimal Outstanding { get; set; }
    }
}
