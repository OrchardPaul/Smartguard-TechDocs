using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GadjIT.GadjitContext.GadjIT_App
{
    public partial class AppWorkTypeGroups
    {
        [Key]
        public int Id { get; set; }
        [StringLength(256)]
        public string GroupName { get; set; }
        [Column("parentId")]
        public int ParentId { get; set; }
    }
}
