using GadjIT_AppContext.GadjIT_App;
using GadjIT_AppContext.GadjIT_App.Custom;
using GadjIT_App.Data.Admin;
using GadjIT_App.Services.SessionState;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GadjIT_App.Pages.Admin.UserManagement
{
    public partial class ManageUsersDetail
    {
        [Parameter]
        public Action ToggleDetail { get; set; }

        [Parameter]
        public AspNetUser TaskObject { get; set; }

        [Parameter]
        public List<CompanyItem> CompanyItems { get; set; }

        [Parameter]
        public List<RoleItem> SelectedRoles { get; set; }

        [Parameter]
        public string SelectedOption { get; set; }

        [Parameter]
        public bool EnablePasswordSet { get; set; } = false;

        [Inject]
        private IIdentityUserAccess Service { get; set; }

        [Inject]
        private IIdentityRoleAccess RoleAccess { get; set; }

        [Inject]
        private IUserSessionState SessionState { get; set; }

        [Inject]
        private IUserManagementSelectedUserState SelectedUserState { get; set; }

        private IList<string> EditObjectRoles { get; set; }

        private List<AppCompanyDetails> Companies { get; set; }

        private List<AspNetRoles> lstRoles { get; set; }

        private List<string> UsersClaimId { get; set; }

        private AspNetUser CurrentUser { get; set; }

        protected override async Task OnInitializedAsync()
        {

            //Wait for session state to finish to prevent concurrency error on refresh
            bool gotLock = SessionState.Lock;
            while (gotLock)
            {
                await Task.Yield();
                gotLock = SessionState.Lock;
            }


            SessionState.OnChange += StateHasChanged;

            TaskObject = SelectedUserState.TaskObject;
            SelectedOption = SelectedUserState.selectedOption;
            Companies = SelectedUserState.allCompanies;
            lstRoles = SelectedUserState.allRoles;

            if (SelectedOption == "Edit")
            {

                CurrentUser = await Service.GetUserByName(TaskObject.UserName);

                EditObjectRoles = await RoleAccess.GetCurrentUserRolesForCompany(CurrentUser, CurrentUser.SelectedCompanyId);

                //editObjectRoles = await service.GetSelectedUserRoles(selectedUser);
                var userCliams = await Service.GetCompanyClaims(CurrentUser);
                UsersClaimId = userCliams.Select(U => U.Value).ToList();

                CompanyItems = Companies.Select(C => new CompanyItem
                {
                    Id = C.Id,
                    Company = C,
                    IsSubscribed = (UsersClaimId.Contains(C.Id.ToString())) ? true : false
                }).ToList();

                SelectedRoles = lstRoles
                    .Select(L => new RoleItem
                    {
                        IsSubscribed = (EditObjectRoles.Contains(L.Name)) ? true : false,
                        RoleName = L.Name,
                        RoleId = L.Id
                    })
                    .ToList();
            }
            else
            {
                EditObjectRoles = null;

                CompanyItems = Companies.Select(C => new CompanyItem
                {
                    Id = C.Id,
                    Company = C,
                    IsSubscribed = C.CompanyName == SessionState.Company.CompanyName ? true : false
                }).ToList();

                SelectedRoles = lstRoles
                    .Select(L => new RoleItem
                    {
                        IsSubscribed = false,
                        RoleName = L.Name,
                        RoleId = L.Id
                    })
                    .ToList();
            }

        }

        private void TogglePasswordSet()
        {
            EnablePasswordSet = !EnablePasswordSet;

            if (EnablePasswordSet)
            {
                TaskObject.PasswordHash = "************";
            }
            else
            {
                TaskObject.PasswordHash = "PasswordNotChanged115592!";
            }

            StateHasChanged();
        }

        private async Task ToggleCompany(int selectedId)
        {
            TaskObject.SelectedCompanyId = selectedId;

            EditObjectRoles = await RoleAccess.GetCurrentUserRolesForCompany(CurrentUser, TaskObject.SelectedCompanyId);

            SelectedRoles = lstRoles
                .Select(L => new RoleItem
                {
                    IsSubscribed = (EditObjectRoles.Contains(L.Name)) ? true : false,
                    RoleName = L.Name,
                    RoleId = L.Id
                })
                .ToList();

            StateHasChanged();
        }


        private async Task HandleValidSubmit()
        {
            await SubmitChange();

            NavigateBack();
        }

        private async Task<AspNetUser> SubmitChange()
        {

            var returnObject = await Service.SubmitChanges(TaskObject, SelectedRoles);
            await Service.SubmitCompanyCliams(CompanyItems, returnObject);

            await SessionState.SetSessionState();

            return returnObject;
        }

        private void NavigateBack()
        {
            ToggleDetail?.Invoke();
        }

    }
}
