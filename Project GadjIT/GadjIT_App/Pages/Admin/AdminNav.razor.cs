using GadjIT_App.Services.SessionState;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace GadjIT_App.Pages.Admin
{
    public partial class AdminNav
    {
        [Inject]
        IPageAuthorisationState PageAuthorisationState { get; set; }

        [Inject]
        NavigationManager NavigationManager { get; set; }

        [Inject]
        public IUserSessionState UserSession { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var authenticationState = await PageAuthorisationState.AdminNavAuthorisation();

            if (!(authenticationState == "Authorised"))
            {
                NavigationManager.NavigateTo(authenticationState, true);
            }

        }
    }
}
