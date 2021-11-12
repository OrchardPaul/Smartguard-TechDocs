using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Gizmo.Context.Gizmo_Authentification
{
    public partial class AppCompanyWorkTypeMapping
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CaseTypeCode { get; set; }

        [Required]
        public int WorkTypeId { get; set; }

        [Required]
        public int CompanyId { get; set; }

        [Required]
        [StringLength(10)]
        public string System { get; set; }
    }
}
