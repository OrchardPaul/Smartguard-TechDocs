using Gizmo.Context.Gizmo_Authentification;
using Gizmo.Context.Gizmo_Authentification.Custom;
using Gizmo_V1_02.Data.Admin;
using Gizmo_V1_02.Services.SessionState;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gizmo_V1_02.Pages.Admin.UserManagement
{
    public partial class ManageUsersDetail
    {
        [Parameter]
        public Action ToggleDetail { get; set; }

        [Parameter]
        public AspNetUsers TaskObject { get; set; }

        [Parameter]
        public List<CompanyItem> companyItems { get; set; }

        [Parameter]
        public List<RoleItem> selectedRoles { get; set; }

        [Parameter]
        public string selectedOption { get; set; }

        [Parameter]
        public bool enablePasswordSet { get; set; } = false;

        [Inject]
        private IIdentityUserAccess service { get; set; }

        [Inject]
        private IIdentityRoleAccess roleAccess { get; set; }

        [Inject]
        private ICompanyDbAccess companyDbAccess { get; set; }

        [Inject]
        private IUserSessionState sessionState { get; set; }

        [Inject]
        private IUserManagementSelectedUserState selectedUserState { get; set; }

        [Inject]
        protected AuthenticationStateProvider authenticationStateProvider { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        private IList<string> editObjectRoles { get; set; }

        private List<AppCompanyDetails> companies { get; set; }

        private List<AspNetRoles> lstRoles { get; set; }

        private List<string> usersClaimId { get; set; }

        private AspNetUsers currentUser { get; set; }

        string isChecked { get; set; } = "";

        private AuthenticationState auth { get; set; }

        protected override async Task OnInitializedAsync()
        {
            sessionState.OnChange += StateHasChanged;

            auth = await authenticationStateProvider.GetAuthenticationStateAsync();

            TaskObject = selectedUserState.TaskObject;
            selectedOption = selectedUserState.selectedOption;
            companies = selectedUserState.allCompanies;
            lstRoles = selectedUserState.allRoles;

            if (selectedOption == "Edit")
            {

                currentUser = await service.GetUserByName(TaskObject.UserName);

                editObjectRoles = await roleAccess.GetCurrentUserRolesForCompany(currentUser, currentUser.SelectedCompanyId);

                //editObjectRoles = await service.GetSelectedUserRoles(selectedUser);
                var userCliams = await service.GetCompanyClaims(currentUser);
                usersClaimId = userCliams.Select(U => U.Value).ToList();

                companyItems = companies.Select(C => new CompanyItem
                {
                    Id = C.Id,
                    Company = C,
                    IsSubscribed = (usersClaimId.Contains(C.Id.ToString())) ? true : false
                }).ToList();

                selectedRoles = lstRoles
                    .Select(L => new RoleItem
                    {
                        IsSubscribed = (editObjectRoles.Contains(L.Name)) ? true : false,
                        RoleName = L.Name,
                        RoleId = L.Id
                    })
                    .ToList();
            }
            else
            {
                editObjectRoles = null;

                companyItems = companies.Select(C => new CompanyItem
                {
                    Id = C.Id,
                    Company = C,
                    IsSubscribed = C.CompanyName == sessionState.Company.CompanyName ? true : false
                }).ToList();

                selectedRoles = lstRoles
                    .Select(L => new RoleItem
                    {
                        IsSubscribed = false,
                        RoleName = L.Name,
                        RoleId = L.Id
                    })
                    .ToList();
            }

        }

        public void Dispose()
        {
            sessionState.OnChange -= StateHasChanged;
        }


        private void TogglePasswordSet()
        {
            enablePasswordSet = !enablePasswordSet;

            if (enablePasswordSet)
            {
                TaskObject.PasswordHash = "************";
            }
            else
            {
                TaskObject.PasswordHash = "PasswordNotChanged115592!";
            }

            StateHasChanged();
        }

        private async void ToggleCompany(int selectedId)
        {
            TaskObject.SelectedCompanyId = selectedId;

            editObjectRoles = await roleAccess.GetCurrentUserRolesForCompany(currentUser, TaskObject.SelectedCompanyId);

            selectedRoles = lstRoles
                .Select(L => new RoleItem
                {
                    IsSubscribed = (editObjectRoles.Contains(L.Name)) ? true : false,
                    RoleName = L.Name,
                    RoleId = L.Id
                })
                .ToList();

            StateHasChanged();
        }


        private async void HandleValidSubmit()
        {
            await SubmitChange();

            NavigateBack();
        }

        private async Task<AspNetUsers> SubmitChange()
        {

            var returnObject = await service.SubmitChanges(TaskObject, selectedRoles);
            await service.SubmitCompanyCliams(companyItems, returnObject);

            await sessionState.SetSessionState();

            return returnObject;
        }

        private void NavigateBack()
        {
            ToggleDetail?.Invoke();
        }

        private async void HandleValidDelete()
        {
            await service.Delete(TaskObject);

            NavigateBack();
        }

        private async Task ShowModal()
        {
            await jsRuntime.InvokeAsync<object>("ShowModal", "modalDelete");
        }

    }
}
