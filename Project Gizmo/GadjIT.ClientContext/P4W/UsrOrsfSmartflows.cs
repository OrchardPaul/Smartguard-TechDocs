using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GadjIT.ClientContext.P4W
{
    [Table("Usr_ORSF_Smartflows")]
    public partial class UsrOrsfSmartflows
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [StringLength(100)]
        public string CaseTypeGroup { get; set; }
        [StringLength(100)]
        public string CaseType { get; set; }
        [StringLength(250)]
        public string SmartflowName { get; set; }
        public int? SeqNo { get; set; }
        public string SmartflowData { get; set; }
        public int? VariantNo { get; set; }
        [StringLength(200)]
        public string VariantName { get; set; }
    }
}
