using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Gizmo.Context.Gizmo_Authentification
{
    public partial class AppDepartments
    {
        [Key]
        public int Id { get; set; }

        [StringLength(256)]
        public string DepartmentName { get; set; }
    }
}
