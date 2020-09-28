using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gizmo.Context.OR_RESI
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
