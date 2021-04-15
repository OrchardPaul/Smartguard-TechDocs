using Blazored.Modal;
using Blazored.Modal.Services;
using Gizmo_V1_02.Pages.Shared.Modals;
using Gizmo_V1_02.Services.SessionState;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gizmo_V1_02.Shared
{
    public partial class NavMenu
    {
        [Inject]
        IModalService Modal { get; set; }

        [Inject]
        public IUserSessionState userSession { get; set; }

        [Inject]
        public NavigationManager navigationManager { get; set; }

        private bool collapseNavMenu = true;

        private string NavMenuCssClass => collapseNavMenu ? "collapse" : null;

        public string ModalInfoHeader { get; set; }
        public string ModalHeight { get; set; }
        public string ModalWidth { get; set; }

        private void ToggleNavMenu()
        {
            collapseNavMenu = !collapseNavMenu;
        }

        private void refreshSmartFlowScreen()
        {
            if (navigationManager.Uri.Contains("smartflow"))
            {
                if(!(userSession.HomeActionSmartflow is null))
                {
                    userSession.HomeActionSmartflow?.Invoke();
                }
            }

            refreshBackGround();
        }

        private void refreshBackGround()
        {
            if (!string.IsNullOrEmpty(userSession.TempBackGroundImage))
            {
                userSession.TempBackGroundImage = "";
                userSession.RefreshHome?.Invoke();
            }
            
        }

        protected void ShowSystemSelectModel()
        {
            var parameters = new ModalParameters();
            parameters.Add("currentUser", userSession.User);

            var options = new ModalOptions()
            {
                Class = "blazored-custom-modal"
            };


            Modal.Show<ModalSystemSelect>("System Select", parameters, options);
        }


        public void NavigateToUserProfile()
        {
            refreshBackGround();
            //set Return URI
            userSession.SetUserProfileReturnURI(navigationManager.Uri);
            navigationManager.NavigateTo("/userprofile");
        }
    }
}
