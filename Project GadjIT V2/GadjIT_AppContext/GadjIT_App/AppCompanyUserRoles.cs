using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GadjIT_AppContext.GadjIT_App
{
    public partial class AppCompanyUserRoles
    {
        [Key]
        public int Id { get; set; }
        public int CompanyId { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public string RoleId { get; set; }
    }
}
