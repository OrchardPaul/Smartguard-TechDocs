using System;
using System.Collections.Generic;
using System.Text;

namespace GadjIT.GadjitContext.GadjIT_App.Custom
{
    public partial class WorkTypeGroupAssignment
    {
        public AppWorkTypes WorkType { get; set; }
        public bool IsAssigned { get; set; }
    }
}
