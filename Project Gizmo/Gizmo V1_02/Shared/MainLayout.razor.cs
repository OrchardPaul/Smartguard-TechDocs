using Gizmo.Context.Gizmo_Authentification;
using Gizmo_V1_02.Data;
using Gizmo_V1_02.Services.SessionState;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Server.HttpSys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;

namespace Gizmo_V1_02.Shared
{
    public partial class MainLayout
    {

        [Inject]
        protected IUserSessionState sessionState { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        public AspNetUsers currentUser { get; set; }

        public int selectedCompanyId { get; set; }
        
        protected override async Task OnInitializedAsync()
        {
            await sessionState.SetSessionState();

            currentUser = sessionState.User;

            if(currentUser is null)
            {
                string returnUrl = HttpUtility.UrlEncode($"/admin");
                NavigationManager.NavigateTo($"Identity/Account/Login?returnUrl={returnUrl}", true);
            } 

            StateHasChanged();
        }

        private async void ToggleCompany(int companyId)
        {
            currentUser.SelectedCompanyId = companyId;
            await sessionState.switchSelectedCompany(companyId);

            /*
             * Need to redirect to Identity Section
             * Can't use sign in manager from main blazor section
             * something to do with authentication state cookies
             */

            NavigationManager.NavigateTo("/Identity/Account/LogOutOnGet", true);
        }
    }
}
