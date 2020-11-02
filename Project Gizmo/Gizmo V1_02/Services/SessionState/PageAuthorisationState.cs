using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gizmo_V1_02.Services.SessionState
{
    public interface IPageAuthorisationState
    {
        bool ChapterListAuthorisation(AuthenticationState authenticationState);
    }

    public class PageAuthorisationState : IPageAuthorisationState
    {
        public bool ChapterListAuthorisation(AuthenticationState authenticationState)
        {
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
    }
}
