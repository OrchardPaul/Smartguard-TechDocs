
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace GadjIT_App.Shared
{

    public partial class RedirectToLogin
    {
        [Inject]
        private NavigationManager Navigation { get; set; }

        [CascadingParameter]
        public Task<AuthenticationState> AuthenticationStateTask { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            var authenticationState = await AuthenticationStateTask;
            try
            {
                if (authenticationState?.User?.Identity is null || !authenticationState.User.Identity.IsAuthenticated)
                    {
                        var returnUrl = Navigation.ToBaseRelativePath(Navigation.Uri);

                        if (string.IsNullOrWhiteSpace(returnUrl))
                            Navigation.NavigateTo("/Identity/Account/Login", true);
                        else
                            Navigation.NavigateTo($"/Identity/Account/Login?returnUrl=~/{returnUrl}", true);
                    }
            }
            catch (Exception e)
            {

                
            }
        
        }
    }        

}        
