using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GadjIT_AppContext.GadjIT_App
{
    public partial class AppWorkTypeGroupsTypeAssignments
    {
        [Key]
        public int Id { get; set; }
        public int WorkTypeGroupId { get; set; }
        public int WorkTypeId { get; set; }
    }
}
