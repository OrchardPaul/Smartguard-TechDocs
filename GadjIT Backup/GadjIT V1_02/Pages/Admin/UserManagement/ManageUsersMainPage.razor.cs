using GadjIT_V1_02.Services.SessionState;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GadjIT_V1_02.Pages.Admin.UserManagement
{
    public partial class ManageUsersMainPage
    {
        [Inject]
        private IUserSessionState sessionState { get; set; }

        [Inject]
        IPageAuthorisationState PageAuthorisationState { get; set; }

        [Inject]
        NavigationManager NavigationManager { get; set; }

        public bool showDetail { get; set; } = false;

        public void ToggleDetail() 
        {
            showDetail = !showDetail;

            StateHasChanged();
        }

        protected override async Task OnInitializedAsync()
        {
            var authenticationState = await PageAuthorisationState.UserManagementAuthorisation();

            if (!(authenticationState == "Authorised"))
            {
                NavigationManager.NavigateTo(authenticationState, true);
            }


        }


    }
}
