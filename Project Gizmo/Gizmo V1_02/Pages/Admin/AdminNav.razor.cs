using Gizmo_V1_02.Services.SessionState;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Gizmo_V1_02.Pages.Admin
{
    public partial class AdminNav
    {
        [Inject]
        IPageAuthorisationState PageAuthorisationState { get; set; }

        [Inject]
        NavigationManager NavigationManager { get; set; }

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
