using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GadjIT.AppContext.GadjIT_App
{
    public partial class AppWorkTypes
    {
        [Key]
        public int Id { get; set; }
        [StringLength(256)]
        public string TypeName { get; set; }
        public int DepartmentId { get; set; }
    }
}
