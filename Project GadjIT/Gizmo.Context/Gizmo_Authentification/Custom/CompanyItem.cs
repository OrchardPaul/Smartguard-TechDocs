using System;
using System.Collections.Generic;
using System.Text;

namespace Gizmo.Context.Gizmo_Authentification.Custom
{
    public partial class CompanyItem
    {
        public int Id { get; set; }

        public AppCompanyDetails Company { get; set; }

        public bool IsSubscribed { get; set; }
    }
}
