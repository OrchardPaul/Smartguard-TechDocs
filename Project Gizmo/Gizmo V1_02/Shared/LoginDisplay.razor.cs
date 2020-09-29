using Gizmo_V1_02.Data;
using Gizmo_V1_02.Data.Admin;
using Gizmo_V1_02.Services.SessionState;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gizmo_V1_02.Shared
{
    public partial class LoginDisplay
    {
        [Inject]
        protected IUserSessionState sessionState { get; set; }

        [Inject]
        protected IIdentityUserAccess userAccess { get; set; }

        [Inject]
        protected AuthenticationStateProvider authenticationStateProvider { get; set; }

        protected override async Task OnInitializedAsync()
        {
            sessionState.OnChange += StateHasChanged;

            if (string.IsNullOrWhiteSpace(sessionState.FullName))
            {
                var auth = await authenticationStateProvider.GetAuthenticationStateAsync();

                if (!(auth is null))
                {
                    var user = auth.User;
                    var userName = user.Identity.Name;

                    if (!(userName is null))
                    {
                        var currentUser = await userAccess.GetUserByName(userName);

                        if (!(currentUser is null))
                        {
                            sessionState.SetFullName(currentUser.FullName);
                        }
                    }


                }
            }
        }

        public void Dispose()
        {
            sessionState.OnChange -= StateHasChanged;
        }




    }
}
