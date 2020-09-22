using System;
using System.Collections.Generic;

namespace Gizmo.Context.OR_RESI
{
    public partial class TblToDo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public DateTime? DueDate { get; set; }
    }
}
