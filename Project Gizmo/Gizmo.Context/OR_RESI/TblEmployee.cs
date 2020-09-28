using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gizmo.Context.OR_RESI
{
    [Table("tblEmployee")]
    public partial class TblEmployee
    {
        public int EmployeeId { get; set; }
        [Required]
        [StringLength(20)]
        public string Name { get; set; }
        [Required]
        [StringLength(20)]
        public string City { get; set; }
        [Required]
        [StringLength(20)]
        public string Department { get; set; }
        [Required]
        [StringLength(6)]
        public string Gender { get; set; }
    }
}
