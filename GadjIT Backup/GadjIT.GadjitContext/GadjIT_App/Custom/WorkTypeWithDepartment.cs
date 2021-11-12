using System;
using System.Collections.Generic;
using System.Text;

namespace GadjIT.AppContext.GadjIT_App.Custom
{
    public partial class WorkTypeWithDepartment
    {
        public AppWorkTypes workType { get; set; }
        public AppDepartments department { get; set; }

        public bool OnHover { get; set; } = false;
    }
}
