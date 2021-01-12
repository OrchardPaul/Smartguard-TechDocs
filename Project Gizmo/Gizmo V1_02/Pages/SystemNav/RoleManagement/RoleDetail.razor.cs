using Blazored.Modal;
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
        [CascadingParameter]
        BlazoredModalInstance ModalInstance { get; set; }

        [Parameter]
        public AspNetRoles TaskObject { get; set; }

        [Parameter]
        public Action DataChanged { get; set; }

        [Inject]
        private IIdentityRoleAccess service { get; set; }

        private async void HandleValidSubmit()
        {
            await service.SubmitChanges(TaskObject);

            DataChanged?.Invoke();
            Close();
        }

        private async void Close()
        {
            await ModalInstance.CloseAsync();
        }


    }
}
