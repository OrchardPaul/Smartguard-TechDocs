using System;
using System.Collections.Generic;

namespace Gizmo_V1_00.Models
{
    public partial class UsrOrResiMtFees
    {
        public int Id { get; set; }
        public string EntityRef { get; set; }
        public int MatterNo { get; set; }
        public decimal? Amount { get; set; }
        public DateTime? BilledOn { get; set; }
        public string FeeDescription { get; set; }
        public DateTime? PaidDate { get; set; }
        public string PaymentMethod { get; set; }
        public decimal? Total { get; set; }
        public decimal? Vat { get; set; }
        public int? ActionFlag { get; set; }
        public string Payee { get; set; }
        public string Status { get; set; }
        public DateTime? DateInserted { get; set; }
        public int? GroupSort { get; set; }
        public string Suffix { get; set; }
        public string TransactionType { get; set; }
        public int? ReconcileId { get; set; }
        public int? MoaId { get; set; }
        public DateTime? Requested { get; set; }
        public DateTime? AllocatedFunds { get; set; }
        public string Funded { get; set; }
        public string Anticipated { get; set; }
        public string PostingType { get; set; }
        public string Account { get; set; }
        public string PrintSlipYn { get; set; }
        public string FeeType { get; set; }
        public string FeeTypeGroup { get; set; }
        public string PayeePayer { get; set; }
        public string SlipPrintStatus { get; set; }
        public string ReconcileTable { get; set; }
        public string Category { get; set; }
    }
}
