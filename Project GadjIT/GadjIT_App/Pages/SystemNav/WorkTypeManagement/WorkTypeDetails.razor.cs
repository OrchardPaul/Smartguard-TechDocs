using Blazored.Modal;
using GadjIT_AppContext.GadjIT_App;
using GadjIT_App.Data.Admin;
using GadjIT_App.Services.SessionState;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GadjIT_App.Pages.SystemNav.WorkTypeManagement
{
    public partial class WorkTypeDetails
    {
        [CascadingParameter]
        BlazoredModalInstance ModalInstance { get; set; }

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

        private async void Close()
        {
            await ModalInstance.CloseAsync();
        }

        private async Task HandleValidSubmit()
        {
            await service.SubmitWorkType(TaskObject);
            await sessionState.SetSessionState();

            DataChanged?.Invoke();
            Close();

        }

    }
}
