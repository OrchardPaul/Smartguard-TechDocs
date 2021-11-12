using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gizmo.Context.OR_RESI
{
    [Table("Usr_OR_DEF_Chapter_Management")]
    public partial class UsrOrDefChapterManagement
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Column("ParentID")]
        public int? ParentId { get; set; }
        [StringLength(100)]
        public string CaseTypeGroup { get; set; }
        [StringLength(100)]
        public string CaseType { get; set; }
        [StringLength(100)]
        public string Type { get; set; }
        [StringLength(250)]
        public string Name { get; set; }
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
    }
}
