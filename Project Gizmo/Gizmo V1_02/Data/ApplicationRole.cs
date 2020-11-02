using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Gizmo_V1_02.Data
{
    public class ApplicationRole : IdentityRole
    {
        [StringLength(500)]
        public string RoleDescription { get; set; }
    }
}
