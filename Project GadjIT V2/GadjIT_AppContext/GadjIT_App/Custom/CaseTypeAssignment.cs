using GadjIT_ClientContext.Models.P4W;

namespace GadjIT_AppContext.GadjIT_App.Custom
{
    public partial class CaseTypeAssignment
    {
        public P4W_CaseTypes CaseType { get; set; }

        public bool IsAssigned { get; set; }
    }
}
