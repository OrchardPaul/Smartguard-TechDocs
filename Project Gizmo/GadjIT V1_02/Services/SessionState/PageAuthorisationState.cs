using GadjIT.GadjitContext.GadjIT_App;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace GadjIT_V1_02.Services.SessionState
{
    public interface IPageAuthorisationState
    {
        Task<bool> ChapterListAuthorisation();
        Task<string> AdminNavAuthorisation();
        Task<string> SystemNavAuthorisation();
        Task<string> UserManagementAuthorisation();
    }

    public class PageAuthorisationState : IPageAuthorisationState
    {
        private readonly AuthenticationStateProvider stateProvider;

        public PageAuthorisationState (AuthenticationStateProvider stateProvider)
        {
            this.stateProvider = stateProvider;
        }

        public async Task<bool> ChapterListAuthorisation()
        {
            var authenticationState = await stateProvider.GetAuthenticationStateAsync();

            bool returnValue = false;

            List<string> roles = new List<string>()
            {
                "Super User"
                ,"Site Admin"
                ,"Manager"
            };

            foreach (var role in roles)
            {
                if (authenticationState.User.IsInRole(role))
                {
                    returnValue = true;
                    break;
                }
            }

            return returnValue;
        }

        public async Task<string> AdminNavAuthorisation()
        {
            var authenticationState = await stateProvider.GetAuthenticationStateAsync();

            List<string> roles = new List<string>()
            {
                "Super User"
                ,"Site Admin"
            };

            foreach (var role in roles)
            {
                if (authenticationState.User.IsInRole(role))
                {
                    return "Authorised";
                }
            }

            /*
             * if not signed in direct user to login
             * else homepage
             */
            if (authenticationState.User.Identity.Name is null)
            {
                string returnUrl = HttpUtility.UrlEncode($"/admin");
                return $"Identity/Account/Login?returnUrl={returnUrl}";
            }
            else
            {
                return "/";
            }
        }

        public async Task<string> SystemNavAuthorisation()
        {
            var authenticationState = await stateProvider.GetAuthenticationStateAsync();

            List<string> roles = new List<string>()
            {
                "Super User"
            };

            foreach (var role in roles)
            {
                if (authenticationState.User.IsInRole(role))
                {
                    return "Authorised";
                }
            }

            /*
             * if not signed in direct user to login
             * else homepage
             */
            if (authenticationState.User.Identity.Name is null)
            {
                string returnUrl = HttpUtility.UrlEncode($"/admin");
                return $"Identity/Account/Login?returnUrl={returnUrl}";
            }
            else
            {
                return "/";
            }
        }


        public async Task<string> UserManagementAuthorisation()
        {
            var authenticationState = await stateProvider.GetAuthenticationStateAsync();

            List<string> roles = new List<string>()
            {
                "Super User"
                ,"Site Admin"
            };

            foreach (var role in roles)
            {
                if (authenticationState.User.IsInRole(role))
                {
                    return "Authorised";
                }
            }

            /*
             * if not signed in direct user to login
             * else homepage
             */
            if (authenticationState.User.Identity.Name is null)
            {
                string returnUrl = HttpUtility.UrlEncode($"/admin");
                return $"Identity/Account/Login?returnUrl={returnUrl}";
            }
            else
            {
                return "/";
            }
        }

    }
}
