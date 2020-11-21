using Gizmo.Context.Gizmo_Authentification;
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
    public partial class WorkTypeDetails
    {

        [Parameter]
        public WorkTypeItem TaskObject { get; set; }

        [Parameter]
        public List<WorkTypeAssignment> Groups { get; set; }

        [Parameter]
        public Action DataChanged { get; set; }

        [Inject]
        private ICompanyDbAccess service { get; set; }

        private async Task ClosechapterModal()
        {
            await jsRuntime.InvokeAsync<object>("CloseModal", "TypeModal");
        }

        private async void HandleValidSubmit()
        {
            TaskObject.workType = await service.SubmitWorkType(TaskObject.workType);
            await service.AssignWorkTypeToGroup(TaskObject,Groups);

            await ClosechapterModal();
            DataChanged?.Invoke();
        }

        private async void HandleValidDelete()
        {
            await service.DeleteWorkType(TaskObject.workType);

            await ClosechapterModal();
            DataChanged?.Invoke();
        }
    }
}
