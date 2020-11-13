using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Gizmo.Context.Gizmo_Authentification
{
    public partial class AppCompanySelection
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string UserId { get; set; }
        public int SelectedCompanyId { get; set; }
    }
}
