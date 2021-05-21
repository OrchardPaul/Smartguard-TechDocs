using Blazored.Modal;
using GadjIT.GadjitContext.GadjIT_App;
using GadjIT.GadjitContext.GadjIT_App.Custom;
using GadjIT_V1_02.Data.Admin;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GadjIT_V1_02.Pages.SystemNav.WorkTypeManagement
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

        private async void HandleValidSubmit()
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
