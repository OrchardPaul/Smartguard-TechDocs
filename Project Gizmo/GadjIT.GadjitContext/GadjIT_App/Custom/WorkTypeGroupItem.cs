using System.Collections.Generic;

namespace GadjIT.GadjitContext.GadjIT_App.Custom
{
    /*
     * Class for single group with list of all of its assigned worktypes
     * along with boolean to toggle whether to show them or not
     */
    public partial class WorkTypeGroupItem
    {
        public AppWorkTypeGroups group { get; set; }
        public AppDepartments department { get; set; }
        public List<WorkTypeItem> workTypes { get; set; }
        public bool showWorkType { get; set; }

        public WorkTypeGroupItem()
        {
            group = new AppWorkTypeGroups();

            department = new AppDepartments();
        }

        public bool OnHover { get; set; } = false;
    }
}
