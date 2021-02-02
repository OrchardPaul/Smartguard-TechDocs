using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace GadjIT.ClientContext.P4W_Scaffold
{
    public partial class CaseTypeGroups
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [StringLength(50)]
        public string Name { get; set; }
        [Column("CMTemplates")]
        [StringLength(255)]
        public string Cmtemplates { get; set; }
        [StringLength(3)]
        public string Department { get; set; }
    }
}
