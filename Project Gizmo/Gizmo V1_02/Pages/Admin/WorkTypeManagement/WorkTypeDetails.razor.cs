using Gizmo.Context.Gizmo_Authentification;
using Gizmo_V1_02.Data.Admin;
using Gizmo_V1_02.Services.SessionState;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gizmo_V1_02.Pages.Admin.WorkTypeManagement
{
    public partial class WorkTypeDetails
    {

        [Parameter]
        public AppWorkTypes TaskObject { get; set; }

        [Parameter]
        public List<AppDepartments> Departments { get; set; }

        [Parameter]
        public Action DataChanged { get; set; }

        [Inject]
        private ICompanyDbAccess service { get; set; }

        [Inject]
        private IUserSessionState sessionState { get; set; }

        private bool ShowForm { get; set; } = true;
        private async Task ClosechapterModal()
        {
            await jsRuntime.InvokeAsync<object>("CloseModal", "TypeModal");
        }

        private async void HandleValidSubmit()
        {
            await service.SubmitWorkType(TaskObject);
            await sessionState.SetSessionState();

            await ClosechapterModal();
            DataChanged?.Invoke();

        }

        private async void HandleValidDelete()
        {
            await service.DeleteWorkType(TaskObject);

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
