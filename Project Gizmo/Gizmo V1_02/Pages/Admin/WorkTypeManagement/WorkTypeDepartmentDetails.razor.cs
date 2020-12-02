using Gizmo.Context.Gizmo_Authentification;
using Gizmo.Context.Gizmo_Authentification.Custom;
using Gizmo_V1_02.Data.Admin;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace Gizmo_V1_02.Pages.Admin.WorkTypeManagement
{
    public partial class WorkTypeDepartmentDetails 
    {
        [Parameter]
        public AppDepartments TaskObject { get; set; }

        [Parameter]
        public Action DataChanged { get; set; }

        [Inject]
        private ICompanyDbAccess service { get; set; }

        private bool ShowForm { get; set; } = true;
        private async Task ClosechapterModal()
        {
            await jsRuntime.InvokeAsync<object>("CloseModal", "DepartmentModal");
        }

        private async void HandleValidSubmit()
        {
            await service.SubmitDepartment(TaskObject);

            await ClosechapterModal();
            DataChanged?.Invoke();
        }

        private async void HandleValidDelete()
        {
            await service.DeleteDepartment(TaskObject);

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
