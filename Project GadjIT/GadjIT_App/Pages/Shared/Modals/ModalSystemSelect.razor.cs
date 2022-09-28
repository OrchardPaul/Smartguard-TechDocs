using GadjIT_AppContext.GadjIT_App;
using GadjIT_App.Services.SessionState;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;
using Blazored.Modal;

namespace GadjIT_App.Pages.Shared.Modals
{
    public partial class ModalSystemSelect
    {
        [CascadingParameter]
        BlazoredModalInstance ModalInstance { get; set; }

        [Parameter]
        public string InfoHeader { get; set; }

        [Parameter]
        public string ModalHeight { get; set; }

        [Parameter]
        public string ModalWidth { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        protected IUserSessionState sessionState { get; set; }

        [Parameter]
        public AspNetUsers currentUser { get; set; }

        private void ToggleCompany(int selectedId)
        {
            currentUser.SelectedCompanyId = selectedId;

            StateHasChanged();
        }

        private async Task HandleValidSubmit()
        {
            try
            {
                sessionState.SuppressChangeSystemError = true;

                sessionState.SetCurrentUser(currentUser);
                await sessionState.switchSelectedCompany();


                NavigationManager.NavigateTo("/Identity/Account/LogOutOnGet", true);
            }
            catch
            {
                Console.WriteLine("Switch system clicked too many times");
            }
            

        }


        private async Task Close()
        {
            await ModalInstance.CloseAsync();
        }


    }
}
