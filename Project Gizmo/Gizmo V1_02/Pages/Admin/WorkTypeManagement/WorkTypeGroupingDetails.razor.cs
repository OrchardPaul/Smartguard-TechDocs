using Gizmo.Context.Gizmo_Authentification.Custom;
using Gizmo_V1_02.Data.Admin;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gizmo_V1_02.Pages.Admin.WorkTypeManagement
{
    public partial class WorkTypeGroupingDetails
    {
        [Parameter]
        public WorkTypeGroupItem TaskObject { get; set; }

        [Parameter]
        public List<WorkTypeGroupAssignment> Assignments { get; set; }

        [Parameter]
        public Action DataChanged { get; set; }

        [Inject]
        private ICompanyDbAccess service { get; set; }

        private bool ShowForm { get; set; } = true;
        private async Task ClosechapterModal()
        {
            await jsRuntime.InvokeAsync<object>("CloseModal", "GroupModal");
        }

        private async void HandleValidSubmit()
        {
            await service.AssignWorkTypeToGroup(TaskObject, Assignments);

            await ClosechapterModal();
            DataChanged?.Invoke();
        }

        private async void HandleValidDelete()
        {
            await service.DeleteWorkTypeGroup(TaskObject.group);

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
