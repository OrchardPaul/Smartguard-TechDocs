using Blazored.Modal;
using GadjIT.AppContext.GadjIT_App.Custom;
using GadjIT_App.Data.Admin;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GadjIT_App.Pages.SystemNav.WorkTypeManagement
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
