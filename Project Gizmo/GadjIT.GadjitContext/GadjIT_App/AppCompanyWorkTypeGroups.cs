using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GadjIT.GadjitContext.GadjIT_App
{
    public partial class AppCompanyWorkTypeGroups
    {
        public int WorkTypeGroupId { get; set; }
        [Key]
        public int Id { get; set; }
        [Column("CompanyID")]
        public int CompanyId { get; set; }
    }
}
