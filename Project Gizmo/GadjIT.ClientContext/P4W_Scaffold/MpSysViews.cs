using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace GadjIT.ClientContext.P4W_Scaffold
{
    [Table("Mp_Sys_Views")]
    public partial class MpSysViews
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Required]
        [StringLength(255)]
        public string Name { get; set; }
        public int? CaseGroupRef { get; set; }
        public int? ReadLock { get; set; }
        public int? WriteLock { get; set; }
        public int? NextItem { get; set; }
        public int? Flags { get; set; }
        public int? Type { get; set; }
        [StringLength(255)]
        public string Form { get; set; }
        [StringLength(255)]
        public string Icon { get; set; }
        public int? DescriptionField { get; set; }
        public byte? System { get; set; }
        [StringLength(50)]
        public string SystemName { get; set; }
        public byte? Designable { get; set; }
        [StringLength(255)]
        public string InternalName { get; set; }
        [Column("visibility")]
        public int? Visibility { get; set; }
        public byte WebPublish { get; set; }
        [Column("CRM_Linked")]
        public byte CrmLinked { get; set; }
        [StringLength(3000)]
        public string InternalNote { get; set; }
    }
}
