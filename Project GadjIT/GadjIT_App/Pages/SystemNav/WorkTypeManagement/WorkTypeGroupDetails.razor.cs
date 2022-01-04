using Blazored.Modal;
using GadjIT.AppContext.GadjIT_App;
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
    public partial class WorkTypeGroupDetails
    {
        [CascadingParameter]
        BlazoredModalInstance ModalInstance { get; set; }

        [Parameter]
        public WorkTypeGroupItem TaskObject { get; set; }

        [Parameter]
        public List<AppDepartments> Departments { get; set; }

        [Parameter]
        public Action DataChanged { get; set; }

        [Inject]
        private ICompanyDbAccess service { get; set; }

        private async Task HandleValidSubmit()
        {
            await service.SubmitWorkTypeGroup(TaskObject.group);

            Close();
            DataChanged?.Invoke();
        }

        private async void Close()
        {
            await ModalInstance.CloseAsync();
        }


    }
}
