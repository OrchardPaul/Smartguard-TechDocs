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
using Microsoft.AspNetCore.Components.Authorization;

namespace Gizmo_V1_02.Pages.Admin.UserManagementRework
{
    public partial class ManageUsers
    {
        [Parameter]
        public string submitChanges { get; set; }

        [Inject]
        private IUserManagementSelectedUserState selectedUserState { get; set; }

        [Inject]
        private IIdentityUserAccess userAccess { get; set; }

        [Inject]
        private IIdentityRoleAccess roleAccess { get; set; }

        [Inject]
        private ICompanyDbAccess companyDbAccess { get; set; }

        [Inject]
        private IUserSessionState sessionState { get; set; }

        [Inject]
        protected AuthenticationStateProvider authenticationStateProvider { get; set; }

        public AspNetUsers editObject { get; set; } = new AspNetUsers();

        protected IList<string> editObjectRoles { get; set; }

        public string editOption { get; set; }

        public List<RoleItem> roles { get; set; } = new List<RoleItem>();

        public List<AspNetRoles> lstRoles { get; set; }

        protected List<AspNetUsers> lstUsers { get; set; }

        private bool ResetPasswordDropBox = false;

        private AuthenticationState auth { get; set; }

        public List<AppCompanyDetails> companies { get; set; }

        public List<CompanyItem> companyItems { get; set; }

        public IList<Claim> userCliams { get; set; }

        protected override async Task OnInitializedAsync()
        {
            auth = await authenticationStateProvider.GetAuthenticationStateAsync();

            sessionState.OnChange += StateHasChanged;

            if(!(submitChanges is null))
            {
                await SubmitChange();
            }

            lstUsers = await userAccess.GetUsers();
            lstRoles = await roleAccess.GetUserRoles();
            companies = await companyDbAccess.GetCompanies();

            selectedUserState.DataChanged = DataChanged;
            selectedUserState.allCompanies = companies;
            selectedUserState.allRoles = lstRoles;
        }


        public void Dispose()
        {
            sessionState.OnChange -= StateHasChanged;
        }

        private async Task<AspNetUsers> SubmitChange()
        {
            var returnObject = await userAccess.SubmitChanges(selectedUserState.TaskObject, selectedUserState.selectedRoles);
            await userAccess.SubmitCompanyCliams(selectedUserState.companyItems, returnObject);

            if (!(auth is null))
            {
                var user = auth.User;
                var userName = user.Identity.Name;

                if (!(userName is null))
                {
                    if (userName == selectedUserState.TaskObject.UserName)
                    {
                        var allClaims = await userAccess.GetSignedInUserClaims();
                        var signedInUser = await userAccess.GetUserByName(userName);

                        if (!(allClaims is null))
                        {
                            sessionState.SetClaims(allClaims);

                            var companyClaim = allClaims.Where(A => A.Type == "Company").SingleOrDefault();

                            var baseUri = await companyDbAccess.GetCompanyBaseUri((companyClaim is null) ? 0 : Int32.Parse(companyClaim.Value)
                                                                                , (signedInUser.SelectedUri is null) ? "" : signedInUser.SelectedUri);

                            if (!(baseUri is null))
                            {
                                sessionState.SetBaseUri(baseUri);
                            }
                            else
                            {
                                sessionState.SetBaseUri("Not Set");
                            }
                        }
                    }
                }
            }

            return returnObject;
        }


        protected void PrepareForEdit(AspNetUsers selectedUser)
        {
            editOption = "Edit";
            editObject = selectedUser;
            editObject.SelectedUri = "Live";
            editObject.PasswordHash = "PasswordNotChanged115592!";

            selectedUserState.TaskObject = editObject;
            selectedUserState.selectedOption = editOption;
        }

        protected void PrepareForInsert()
        {
            editOption = "Insert";
            editObject = new AspNetUsers();
            editObject.SelectedUri = "Live";

            selectedUserState.TaskObject = editObject;
            selectedUserState.selectedOption = editOption;
        }

        private async void DataChanged()
        {
            lstUsers = await userAccess.GetUsers();
            lstRoles = await roleAccess.GetUserRoles();

            StateHasChanged();
        }

    }
}
