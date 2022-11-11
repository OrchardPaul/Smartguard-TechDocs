using Blazored.Modal;
using GadjIT_AppContext.GadjIT_App;
using GadjIT_App.Data.Admin;
using Microsoft.AspNetCore.Components;
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
        private ICompanyDbAccess Service { get; set; }

        private async void Close()
        {
            await ModalInstance.CloseAsync();
        }

        private async Task HandleValidSubmit()
        {
            await Service.SubmitDepartment(TaskObject);

            DataChanged?.Invoke();
            Close();
        }
    }
}
