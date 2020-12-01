using System;
using System.Collections.Generic;
using System.Text;

namespace Gizmo.Context.Gizmo_Authentification.Custom
{
    public partial class WorkTypeGroupAssignment
    {
        public AppWorkTypes WorkType { get; set; }
        public bool IsAssigned { get; set; }
    }
}
