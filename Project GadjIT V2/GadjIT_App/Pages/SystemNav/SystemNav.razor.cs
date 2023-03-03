using GadjIT_App.Services.SessionState;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;


namespace GadjIT_App.Pages.SystemNav
{
    public partial class SystemNav
    {
        [Inject]
        IPageAuthorisationState PageAuthorisationState { get; set; }

        [Inject]
        NavigationManager NavigationManager { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var authenticationState = await PageAuthorisationState.SystemNavAuthorisation();

            if (!(authenticationState == "Authorised"))
            {
                NavigationManager.NavigateTo(authenticationState, true);
            }

        }
    }
}
