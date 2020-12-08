using System;
using System.Collections.Generic;
using System.Text;

namespace Gizmo.Context.Gizmo_Authentification.Custom
{
    public partial class UserDataCollectionItem
    {
        public AspNetUsers User { get; set; }

        public List<AppCompanyUserRoles> UserRoles { get; set; }

        public List<AppCompanyDetails> UserCompanies { get; set; }
    }
}
