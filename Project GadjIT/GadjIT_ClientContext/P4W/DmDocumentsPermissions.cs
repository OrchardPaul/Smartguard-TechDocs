using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace GadjIT.ClientContext.P4W
{
    [Table("DM_DocumentsPermissions")]
    public partial class DmDocumentsPermissions
    {
        [Key]
        [Column("doccode")]
        public int Doccode { get; set; }
        [Key]
        [Column("casetype")]
        public int Casetype { get; set; }
    }
}
