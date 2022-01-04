using GadjIT.ClientContext.P4W;
using System;
using System.Collections.Generic;
using System.Text;

namespace GadjIT.AppContext.GadjIT_App.Custom
{
    public partial class CaseTypeAssignment
    {
        public CaseTypes CaseType { get; set; }

        public bool IsAssigned { get; set; }
    }
}
