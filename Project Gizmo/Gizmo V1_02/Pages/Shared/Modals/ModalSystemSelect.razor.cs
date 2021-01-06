using Gizmo.Context.Gizmo_Authentification;
using Gizmo.Context.OR_RESI;
using Gizmo_V1_02.Services;
using Gizmo_V1_02.Services.SessionState;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gizmo_V1_02.Pages.Shared.Modals
{
    public partial class ModalSystemSelect
    {
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
            sessionState.SetCurrentUser(currentUser);
            await sessionState.switchSelectedCompany();

            NavigationManager.NavigateTo("/Identity/Account/LogOutOnGet", true);
        }

    }
}
