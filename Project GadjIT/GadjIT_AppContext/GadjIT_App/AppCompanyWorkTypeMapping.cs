using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GadjIT.AppContext.GadjIT_App
{
    public partial class AppCompanyWorkTypeMapping
    {
        [Key]
        public int Id { get; set; }
        public int CaseTypeCode { get; set; }
        public int WorkTypeId { get; set; }
        [Required]
        [StringLength(10)]
        public string System { get; set; }
        public int CompanyId { get; set; }
    }
}
