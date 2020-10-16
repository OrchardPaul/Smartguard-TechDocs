using Gizmo.Context.Gizmo_Authentification;
using Gizmo.Context.Gizmo_Authentification.Custom;
using Gizmo_V1_02.Data.Admin;
using Gizmo_V1_02.Services.SessionState;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gizmo_V1_02.Pages.Admin.UserManagementRework
{
    public partial class UserDetail
    {
        [Parameter]
        public AspNetUsers TaskObject { get; set; }

        [Parameter]
        public Action DataChanged { get; set; }

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

            TaskObject = await service.GetUserByName(TaskObject.UserName);

            if (selectedOption == "Edit")
            {
                editObjectRoles = await service.GetSelectedUserRoles(TaskObject);
                var userCliams = await service.GetCompanyClaims(TaskObject);
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
                        RoleName = L.Name
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
                    IsSubscribed = false
                }).ToList();

                selectedRoles = lstRoles
                    .Select(L => new RoleItem
                    {
                        IsSubscribed = false,
                        RoleName = L.Name
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


        private async void HandleValidSubmit()
        {
            //var Test = SubmitChange();

            selectedUserState.TaskObject = TaskObject;
            selectedUserState.selectedRoles = selectedRoles;
            selectedUserState.companyItems = companyItems;

            //await Test;
            
            NavigateBack();
        }

        private async Task<AspNetUsers> SubmitChange()
        {
            
            var returnObject = await service.SubmitChanges(TaskObject, selectedRoles);
            await service.SubmitCompanyCliams(companyItems, returnObject);

            if (!(auth is null))
            {
                var user = auth.User;
                var userName = user.Identity.Name;

                if (!(userName is null))
                {
                    if (userName == TaskObject.UserName)
                    {
                        var allClaims = await service.GetSignedInUserClaims();
                        var signedInUser = await service.GetUserByName(userName);

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

        private void NavigateBack()
        {
            NavigationManager.NavigateTo($"/manageusersrework/edit");
        }

        private async void HandleValidDelete()
        {
            await service.Delete(TaskObject);

            NavigationManager.NavigateTo($"/manageusersrework", true);
        }

        private void ToggleCompanyStatus(CompanyItem companyItem)
        {
            companyItems.Where(C => C.Id != companyItem.Id).Select(C => C.IsSubscribed = false).ToList();
        }


    }
}
