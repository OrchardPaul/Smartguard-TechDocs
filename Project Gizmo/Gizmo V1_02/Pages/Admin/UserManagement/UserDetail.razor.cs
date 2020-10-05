using Gizmo.Context.Gizmo_Authentification;
using Gizmo.Context.Gizmo_Authentification.Custom;
using Gizmo_V1_02.Data.Admin;
using Gizmo_V1_02.Services.SessionState;
using Microsoft.AspNetCore.Components;
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
        private IUserSessionState sessionState { get; set; }

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
            await service.SubmitCompanyCliams(companies, TaskObject);
            await service.SubmitChanges(TaskObject, selectedRoles);

            var allClaims = await service.GetSignedInUserClaims();

            if(!(allClaims is null))
            {
                sessionState.SetClaims(allClaims);
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
