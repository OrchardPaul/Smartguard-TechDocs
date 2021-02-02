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
using System.Security.Claims;
using System.Threading.Tasks;

namespace Gizmo_V1_02.Shared
{
    public partial class LoginDisplay
    {
        [Parameter]
        public string userFullName { get; set; }

        [Inject]
        public IUserSessionState userSession { get; set; }

        [Inject]
        public NavigationManager navigationManager { get; set; }

        public string ModalInfoHeader { get; set; }
        public string ModalHeight { get; set; }
        public string ModalWidth { get; set; }

        public void NavigateToUserProfile()
        {
            //set Return URI
            userSession.SetUserProfileReturnURI(navigationManager.Uri);
            navigationManager.NavigateTo("/userprofile");
        }


        private void PrepareModalDelete(string modalHeader
                                        , string modalHeight
                                        , string modalWidth)
        {
            ModalInfoHeader = modalHeader;
            ModalHeight = modalHeight;
            ModalWidth = modalWidth;
        }

    }
}
