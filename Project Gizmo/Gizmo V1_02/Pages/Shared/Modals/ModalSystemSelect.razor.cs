using GadjIT.GadjitContext.GadjIT_App;
using GadjIT.ClientContext.P4W;
using Gizmo_V1_02.Services;
using Gizmo_V1_02.Services.SessionState;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazored.Modal;

namespace Gizmo_V1_02.Pages.Shared.Modals
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

        private async void HandleValidSubmit()
        {
            try
            {
                sessionState.SetCurrentUser(currentUser);
                await sessionState.switchSelectedCompany();
            }
            catch
            {
                Console.WriteLine("Switch system clicked too many times");
            }
            

            NavigationManager.NavigateTo("/Identity/Account/LogOutOnGet", true);
        }


        private async void Close()
        {
            await ModalInstance.CloseAsync();
        }


    }
}
