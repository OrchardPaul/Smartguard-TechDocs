using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Gizmo.Context.Gizmo_Authentification
{
    public partial class AppCompanyDetails
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(256)]
        public string CompanyName { get; set; }

        [StringLength(256)]
        public string DevUri { get; set; }

        [StringLength(256)]
        public string LiveUri { get; set; }
    }
}
