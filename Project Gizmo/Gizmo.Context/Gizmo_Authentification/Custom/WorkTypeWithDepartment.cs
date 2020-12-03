using System;
using System.Collections.Generic;
using System.Text;

namespace Gizmo.Context.Gizmo_Authentification.Custom
{
    public partial class WorkTypeWithDepartment
    {
        public AppWorkTypes workType { get; set; }
        public AppDepartments department { get; set; }
    }
}
