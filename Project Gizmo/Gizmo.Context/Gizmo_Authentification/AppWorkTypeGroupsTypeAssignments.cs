using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Gizmo.Context.Gizmo_Authentification
{
    public partial class AppWorkTypeGroupsTypeAssignments
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int WorkTypeGroupId { get; set; }

        [Required]
        public int WorkTypeId { get; set; }
    }
}
