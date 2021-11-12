using Blazored.Modal;
using GadjIT.AppContext.GadjIT_App;
using GadjIT.AppContext.GadjIT_App.Custom;
using GadjIT_App.Data.Admin;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace GadjIT_App.Pages.SystemNav.WorkTypeManagement
{
    public partial class WorkTypeDepartmentDetails
    {
        [CascadingParameter]
        BlazoredModalInstance ModalInstance { get; set; }

        [Parameter]
        public AppDepartments TaskObject { get; set; }

        [Parameter]
        public Action DataChanged { get; set; }

        [Inject]
        private ICompanyDbAccess service { get; set; }

        private async void Close()
        {
            await ModalInstance.CloseAsync();
        }

        private async Task HandleValidSubmit()
        {
            await service.SubmitDepartment(TaskObject);

            DataChanged?.Invoke();
            Close();
        }
    }
}
