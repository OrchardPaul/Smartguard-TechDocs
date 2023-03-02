using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GadjIT_AppContext.GadjIT_App
{
    [Table("SmartflowRecords")]
    public class App_SmartflowRecord
    {

        [Key]
        [Column("ID")]
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public int RowId { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime LastModifiedDate { get; set; }

        public string CreatedByUserId { get; set; }

        public string LastModifiedByUserId { get; set; }

        public string System { get; set; }

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
