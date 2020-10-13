using System.Collections.Generic;

namespace Gizmo.Context.Gizmo_Authentification.Custom
{
    /*
     * Class for single work type along with all of its current assignments
    */
    public partial class WorkTypeItem
    {
        public AppWorkTypes workType { get; set; }

        public List<AppWorkTypeGroupsTypeAssignments> assignment { get; set; }

        public WorkTypeItem()
        {
            workType = new AppWorkTypes();
        }
    }
}
