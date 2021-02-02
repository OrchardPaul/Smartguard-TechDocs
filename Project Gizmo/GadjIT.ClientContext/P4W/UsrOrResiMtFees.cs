using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace GadjIT.ClientContext.P4W
{
    [Table("Usr_OR_RESI_MT_Fees")]
    public partial class UsrOrResiMtFees
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Required]
        [StringLength(15)]
        public string EntityRef { get; set; }
        public int MatterNo { get; set; }
        [Column(TypeName = "money")]
        public decimal? Amount { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? BilledOn { get; set; }
        [StringLength(255)]
        public string FeeDescription { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? PaidDate { get; set; }
        [StringLength(25)]
        public string PaymentMethod { get; set; }
        [Column(TypeName = "money")]
        public decimal? Total { get; set; }
        [Column("VAT", TypeName = "money")]
        public decimal? Vat { get; set; }
        public int? ActionFlag { get; set; }
        [StringLength(2000)]
        public string Payee { get; set; }
        [StringLength(20)]
        public string Status { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? DateInserted { get; set; }
        public int? GroupSort { get; set; }
        [StringLength(50)]
        public string Suffix { get; set; }
        [StringLength(30)]
        public string TransactionType { get; set; }
        [Column("ReconcileID")]
        public int? ReconcileId { get; set; }
        [Column("MOA_ID")]
        public int? MoaId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? Requested { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? AllocatedFunds { get; set; }
        [StringLength(1)]
        public string Funded { get; set; }
        [StringLength(1)]
        public string Anticipated { get; set; }
        [Column("Posting_Type")]
        [StringLength(30)]
        public string PostingType { get; set; }
        [StringLength(30)]
        public string Account { get; set; }
        [Column("Print_Slip_YN")]
        [StringLength(1)]
        public string PrintSlipYn { get; set; }
        [StringLength(255)]
        public string FeeType { get; set; }
        [StringLength(255)]
        public string FeeTypeGroup { get; set; }
        [StringLength(255)]
        public string PayeePayer { get; set; }
        [Column("Slip_Print_Status")]
        [StringLength(50)]
        public string SlipPrintStatus { get; set; }
        [StringLength(40)]
        public string ReconcileTable { get; set; }
        [StringLength(50)]
        public string Category { get; set; }
    }
}
