using Gizmo.Context.Gizmo_Authentification;
using Gizmo_V1_02.Data.Admin;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Gizmo.Context.Gizmo_Authentification.Custom;
using Gizmo_V1_02.Services.SessionState;
using System.Security.Claims;

namespace Gizmo_V1_02.Pages.Admin.UserManagement
{
    public partial class ManageUsers
    {
        [Inject]
        private IIdentityUserAccess userAccess { get; set; }

        [Inject]
        private IIdentityRoleAccess roleAccess { get; set; }

        [Inject]
        private ICompanyDbAccess companyDbAccess { get; set; }

        [Inject]
        private IUserSessionState sessionState { get; set; }

        public AspNetUsers editObject { get; set; } = new AspNetUsers();

        protected IList<string> editObjectRoles { get; set; }

        public string editOption { get; set; }

        public List<RoleItem> roles { get; set; } = new List<RoleItem>();

        public List<AspNetRoles> lstRoles { get; set; }

        protected List<AspNetUsers> lstUsers { get; set; }

        private bool ResetPasswordDropBox = false;

        public List<CompanyDetails> companies { get; set; }

        public List<CompanyItem> companyItems { get; set; }

        public IList<Claim> userCliams { get; set; }

        protected override async Task OnInitializedAsync()
        {
            sessionState.OnChange += StateHasChanged;

            lstUsers = await userAccess.GetUsers();
            lstRoles = await roleAccess.GetUserRoles();
        }

        public void Dispose()
        {
            sessionState.OnChange -= StateHasChanged;
        }

        protected async void PrepareForEdit(AspNetUsers selectedUser)
        {
            editOption = "Edit";
            editObject = selectedUser;
            editObject.PasswordHash = "PasswordNotChanged115592!";
            editObjectRoles = await userAccess.GetSelectedUserRoles(selectedUser);

            companies = await companyDbAccess.GetCompanies();
            userCliams = await userAccess.GetCompanyClaims(editObject);

            var userClaimId = userCliams.Select(U => U.Value).ToList();

            companyItems = companies.Select(C => new CompanyItem 
                                        {
                                            Id = C.Id,
                                            CompanyName = C.CompanyName,
                                            IsSubscribed = (userClaimId.Contains(C.Id.ToString())) ? true : false
                                        }).ToList();

            roles = lstRoles
                .Select(L => new RoleItem
                {
                    IsSubscribed = (editObjectRoles.Contains(L.Name)) ? true : false,
                    RoleName = L.Name
                })
                .ToList();
            StateHasChanged();
        }

        protected void PrepareForInsert()
        {
            editOption = "Insert";
            editObject = new AspNetUsers();
            companies = null;
            editObjectRoles = null;
        }

        private async void DataChanged()
        {
            lstUsers = await userAccess.GetUsers();
            lstRoles = await roleAccess.GetUserRoles();

            StateHasChanged();
        }

    }
}
