using Gizmo.Context.Gizmo_Authentification;
using Gizmo_V1_02.Data.Admin;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gizmo_V1_02.Pages.SystemNav.RoleManagement
{
    public partial class RoleDetail
    {
        [Parameter]
        public AspNetRoles TaskObject { get; set; }

        [Parameter]
        public Action DataChanged { get; set; }

        [Inject]
        private IIdentityRoleAccess service { get; set; }
        private bool ShowForm { get; set; } = true;
        private async Task ClosechapterModal()
        {
            await jsRuntime.InvokeAsync<object>("CloseModal", "roleModal");
        }

        private async void HandleValidSubmit()
        {
            await service.SubmitChanges(TaskObject);

            await ClosechapterModal();
            DataChanged?.Invoke();
        }

        private async void HandleValidDelete()
        {
            await service.Delete(TaskObject);

            ShowForm = true;

            await ClosechapterModal();
            DataChanged?.Invoke();
        }


        private void SubmitForm()
        {
            ShowForm = false;
        }

        private void CancelForm()
        {
            ShowForm = true;
        }
    }
}
