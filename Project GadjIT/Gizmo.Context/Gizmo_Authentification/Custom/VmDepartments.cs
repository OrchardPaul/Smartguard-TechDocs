using System;
using System.Collections.Generic;
using System.Text;
using Gizmo.Context.Gizmo_Authentification;

namespace Gizmo.Context.Gizmo_Authentification.Custom
{
    public partial class VmDepartments
    {
        public AppDepartments department { get; set; }

        public bool OnHover { get; set; } = false;
    }
}
