using Gizmo.Context.Gizmo_Authentification;
using Gizmo.Context.Gizmo_Authentification.Custom;
using Gizmo_V1_02.Data.Admin;
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
        public List<RoleItem> selectedRoles { get; set; }

        [Parameter]
        public string selectedOption { get; set; }

        [Inject]
        private IIdentityUserAccess service { get; set; }

        [Parameter]
        public bool enablePasswordSet { get; set; } = false;

        string isChecked { get; set; } = "";

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
            await service.SubmitChanges(TaskObject, selectedRoles);

            await ClosechapterModal();
            DataChanged?.Invoke();
        }

        private async void HandleValidDelete()
        {
            await service.Delete(TaskObject);

            await ClosechapterModal();
            DataChanged?.Invoke();
        }



    }
}
