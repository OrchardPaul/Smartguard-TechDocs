using GadjIT.ClientContext.OR_RESI;
using System;
using System.Collections.Generic;
using System.Text;

namespace GadjIT.GadjitContext.GadjIT_App.Custom
{
    public partial class CaseTypeAssignment
    {
        public CaseTypes CaseType { get; set; }

        public bool IsAssigned { get; set; }
    }
}
