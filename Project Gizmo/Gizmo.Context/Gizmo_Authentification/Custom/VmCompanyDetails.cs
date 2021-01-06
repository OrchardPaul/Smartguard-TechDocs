using System;
using System.Collections.Generic;
using System.Text;
using Gizmo.Context.Gizmo_Authentification;

namespace Gizmo.Context.Gizmo_Authentification.Custom
{
    public partial class VmCompanyDetails
    {
        public AppCompanyDetails Company { get; set; }

        public bool OnHover { get; set; } = false;
    }
}
