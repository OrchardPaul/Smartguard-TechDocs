using System;
using System.Collections.Generic;
using System.Text;

namespace GadjIT_AppContext.GadjIT_App.Custom
{
    public partial class WorkTypeMapping
    {
        public AppWorkTypes workType { get; set; }

        public List<CaseTypeAssignment> caseTypeAssignments { get; set; }
    }
}
