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
    public partial class UserDetail
    {
        [Parameter]
        public AspNetUsers TaskObject { get; set; }

        [Parameter]
        public Action DataChanged { get; set; }

        [Parameter]
        public List<CompanyItem> companies { get; set; }

        [Parameter]
        public List<RoleItem> selectedRoles { get; set; }

        [Parameter]
        public string selectedOption { get; set; }

        [Inject]
        private IIdentityUserAccess service { get; set; }

        [Inject]
        private ICompanyDbAccess companyDbAccess { get; set; }

        [Inject]
        private IUserSessionState sessionState { get; set; }

        [Inject]
        protected AuthenticationStateProvider authenticationStateProvider { get; set; }

        [Parameter]
        public bool enablePasswordSet { get; set; } = false;

        string isChecked { get; set; } = "";

        protected override async Task OnInitializedAsync()
        {
            sessionState.OnChange += StateHasChanged;
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


        private async Task ClosechapterModal()
        {
            await jsRuntime.InvokeAsync<object>("CloseModal", "userModal");
        }

        private async void HandleValidSubmit()
        {
            TaskObject = await service.SubmitChanges(TaskObject, selectedRoles);
            await service.SubmitCompanyCliams(companies, TaskObject);

            var auth = await authenticationStateProvider.GetAuthenticationStateAsync();

            if (!(auth is null))
            {
                var user = auth.User;
                var userName = user.Identity.Name;

                if (!(userName is null))
                {
                    if(userName == TaskObject.UserName)
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

            await ClosechapterModal();
            DataChanged?.Invoke();
        }

        private async void HandleValidDelete()
        {
            await service.Delete(TaskObject);

            await ClosechapterModal();
            DataChanged?.Invoke();
        }

        private void ToggleCompanyStatus(CompanyItem companyItem)
        {
            companies.Where(C => C.Id != companyItem.Id).Select(C => C.IsSubscribed = false).ToList();
        }


    }
}
