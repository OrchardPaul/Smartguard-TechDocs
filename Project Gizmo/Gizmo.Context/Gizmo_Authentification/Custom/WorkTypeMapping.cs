using Gizmo.Context.OR_RESI;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gizmo.Context.Gizmo_Authentification.Custom
{
    public partial class WorkTypeMapping
    {
        public AppWorkTypes workType { get; set; }

        public List<CaseTypeAssignment> caseTypeAssignments { get; set; }
    }
}
