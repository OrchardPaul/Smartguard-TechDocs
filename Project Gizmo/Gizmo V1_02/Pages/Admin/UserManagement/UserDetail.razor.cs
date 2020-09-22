using Gizmo.Context.Gizmo_Authentification;
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
        public List<AspNetRoles> allRoles { get; set; }
        
        [Parameter]
        public IList<string> selectedRoles { get; set; }

        [Parameter]
        public string selectedOption { get; set; }

        [Inject]
        private IIdentityUserAccess service { get; set; }

        private async Task ClosechapterModal()
        {
            await jsRuntime.InvokeAsync<object>("CloseModal", "roleModal");
        }

        private async void HandleValidSubmit()
        {
            await service.SubmitChanges(TaskObject, allRoles, selectedRoles);

            await ClosechapterModal();
            DataChanged?.Invoke();
        }

        private async void HandleValidDelete()
        {
            await service.Delete(TaskObject);

            await ClosechapterModal();
            DataChanged?.Invoke();
        }

        private void RoleChecked(bool IsChecked, string selectedRole)
        {
            if (IsChecked)
            {
                if (!selectedRoles.Contains(selectedRole))
                {
                    selectedRoles.Add(selectedRole);
                }
            }
            else
            {
                if (selectedRoles.Contains(selectedRole))
                {
                    selectedRoles.Remove(selectedRole);
                }
            }
        }

    }
}
