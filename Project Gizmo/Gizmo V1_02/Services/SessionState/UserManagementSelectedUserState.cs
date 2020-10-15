using Gizmo.Context.Gizmo_Authentification;
using Gizmo.Context.Gizmo_Authentification.Custom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gizmo_V1_02.Services.SessionState
{
    public class UserManagementSelectedUserState
    {
        public AspNetUsers TaskObject { get; set; }

        public Action DataChanged { get; set; }
        
        public List<CompanyItem> companies { get; set; }

        public List<RoleItem> selectedRoles { get; set; }

        public string selectedOption { get; set; }
    }
}
