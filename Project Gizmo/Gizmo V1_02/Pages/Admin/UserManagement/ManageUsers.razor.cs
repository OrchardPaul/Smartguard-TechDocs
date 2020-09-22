using Gizmo.Context.Gizmo_Authentification;
using Gizmo_V1_02.Data.Admin;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Gizmo_V1_02.Pages.Admin.UserManagement
{
    public partial class ManageUsers
    {
        [Inject]
        private IIdentityUserAccess userAccess { get; set; }

        [Inject]
        private IIdentityRoleAccess roleAccess { get; set; }

        public AspNetUsers editObject { get; set; }

        protected IList<string> editObjectRoles { get; set; }

        public string editOption { get; set; }

        public List<AspNetRoles> lstRoles { get; set; }

        protected List<AspNetUsers> lstUsers { get; set; }

        protected override async Task OnInitializedAsync()
        {
            lstUsers = await userAccess.GetUsers();
            lstRoles = await roleAccess.GetUserRoles();
        }

        protected async void PrepareForEdit(AspNetUsers selectedUser)
        {
            editOption = "edit";
            editObject = selectedUser;
            editObjectRoles = await userAccess.GetSelectedUserRoles(selectedUser);
        }

        protected void PrepareForInsert()
        {
            editOption = "Insert";
            editObject = new AspNetUsers();
            editObjectRoles = null;
        }
    }
}
