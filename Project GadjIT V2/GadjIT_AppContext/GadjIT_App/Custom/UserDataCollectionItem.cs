using System;
using System.Collections.Generic;
using System.Text;

namespace GadjIT_AppContext.GadjIT_App.Custom
{
    public partial class UserDataCollectionItem
    {
        public AspNetUser User { get; set; }

        public List<AppCompanyUserRoles> UserRoles { get; set; }

        public List<AppCompanyDetails> UserCompanies { get; set; }

        public bool OnHover { get; set; } = false;
    }
}
