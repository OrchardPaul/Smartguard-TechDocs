using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace GadjIT.ClientContext.OR_RESI
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
        [Required(ErrorMessage = "<b> Item type must be set </b>")]
        public string Type { get; set; }
        [StringLength(250)]
        [Required]
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
