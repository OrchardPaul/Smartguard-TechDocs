using Blazored.Modal;
using Gizmo.Context.Gizmo_Authentification.Custom;
using Gizmo_V1_02.Data.Admin;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gizmo_V1_02.Pages.SystemNav.WorkTypeManagement
{
    public partial class WorkTypeGroupingDetails
    {
        [CascadingParameter]
        BlazoredModalInstance ModalInstance { get; set; }

        [Parameter]
        public WorkTypeGroupItem TaskObject { get; set; }

        [Parameter]
        public List<WorkTypeGroupAssignment> Assignments { get; set; }

        [Parameter]
        public Action DataChanged { get; set; }

        [Inject]
        private ICompanyDbAccess service { get; set; }

        private async void Close()
        {
            await ModalInstance.CloseAsync();
        }

        private async void HandleValidSubmit()
        {
            await service.AssignWorkTypeToGroup(TaskObject, Assignments);

            DataChanged?.Invoke();
            Close();
        }

    }
}
