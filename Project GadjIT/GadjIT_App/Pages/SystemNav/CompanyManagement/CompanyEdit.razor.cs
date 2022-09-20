using Blazored.Modal;
using GadjIT.AppContext.GadjIT_App;
using GadjIT_App.Data.Admin;
using GadjIT_App.Services.SessionState;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace GadjIT_App.Pages.SystemNav.CompanyManagement
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

        [Parameter]
         public EventCallback<string> ReturnedColor { get; set; }

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

        void colorChangeButtons(ChangeEventArgs e) => TaskObject.CompCol1 = e.Value.ToString().ToUpper();
        void colorChangeButtonsCont(ChangeEventArgs e) => TaskObject.CompCol1 = e.Value.ToString().ToUpper();
        void colorChangeLinks(ChangeEventArgs e) => TaskObject.CompCol1 = e.Value.ToString().ToUpper();
        void colorChangeLinksCont(ChangeEventArgs e) => TaskObject.CompCol1 = e.Value.ToString().ToUpper();
            



    }
}
