using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GadjIT.GadjitContext.GadjIT_App
{
    public class SmartflowRecords
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public int RowId { get; set; }

        [Column("ParentID")]
        public int? ParentId { get; set; }
        [StringLength(100)]
        public string CaseTypeGroup { get; set; }
        [StringLength(100)]
        public string CaseType { get; set; }
        [StringLength(100)]
        [Required(ErrorMessage = "Item type must be set")]
        public string Type { get; set; }
        [StringLength(250)]
        [Required]
        public string Name { get; set; }
        [NotMapped]
        public string ChapterName { get; set; }
        [Required]
        public int? SeqNo { get; set; }
        [Column("Suppress_Step")]
        [StringLength(1)]
        public string SuppressStep { get; set; }
        [Column("Entity_Type")]
        [StringLength(100)]
        public string EntityType { get; set; }
        [StringLength(250)]
        public string AsName { get; set; }
        [StringLength(250)]
        public string CompleteName { get; set; }
        public int? RescheduleDays { get; set; }
        [Column("Alt_Display_Name")]
        [StringLength(300)]
        public string AltDisplayName { get; set; }

        [Column("Next_Status")]
        [StringLength(100)]
        public string NextStatus { get; set; }
        public string ChapterData { get; set; }
    }

}
