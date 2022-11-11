using Blazored.Modal;
using GadjIT_AppContext.GadjIT_App.Custom;
using GadjIT_App.Data.Admin;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;

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
        private ICompanyDbAccess Service { get; set; }

        private async void Close()
        {
            await ModalInstance.CloseAsync();
        }

        private async void HandleValidSubmit()
        {
            await Service.AssignWorkTypeToGroup(TaskObject, Assignments);

            DataChanged?.Invoke();
            Close();
        }

    }
}
