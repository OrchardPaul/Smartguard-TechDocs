using Blazored.Modal;
using GadjIT.AppContext.GadjIT_App;
using GadjIT_V1_02.Data.Admin;
using GadjIT_V1_02.Services.SessionState;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GadjIT_V1_02.Pages.SystemNav.CompanyManagement
{
    public partial class CompanyEdit
    {
        [CascadingParameter] 
        BlazoredModalInstance ModalInstance { get; set; }

        [Parameter]
        public AppCompanyDetails TaskObject { get; set; }

        [Parameter]
        public Action DataChanged { get; set; }

        [Inject]
        private ICompanyDbAccess service { get; set; }

        [Inject]
        private IUserSessionState sessionState { get; set; }


        private async Task HandleValidSubmit()
        {
            await service.SubmitChanges(TaskObject);
            await sessionState.SetSessionState();

            DataChanged?.Invoke();

            Close();
        }

        private async void Close()
        {
            await ModalInstance.CloseAsync();
        }


    }
}
