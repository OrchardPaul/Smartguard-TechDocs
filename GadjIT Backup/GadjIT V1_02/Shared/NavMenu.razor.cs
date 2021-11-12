using Blazored.Modal;
using Blazored.Modal.Services;
using GadjIT_V1_02.Pages.Shared.Modals;
using GadjIT_V1_02.Services.SessionState;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GadjIT_V1_02.Shared
{
    public partial class NavMenu
    {
        [Inject]
        IModalService Modal { get; set; }

        [Inject]
        public IUserSessionState UserSession { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

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
            if (NavigationManager.Uri.Contains("smartflow"))
            {
                if(!(UserSession.HomeActionSmartflow is null))
                {
                    UserSession.HomeActionSmartflow?.Invoke();
                }
            }

            refreshBackGround();
        }

        private void refreshBackGround()
        {
            if (!string.IsNullOrEmpty(UserSession.TempBackGroundImage))
            {
                UserSession.TempBackGroundImage = "";
                UserSession.RefreshHome?.Invoke();
            }
            
        }

        protected void ShowSystemSelectModel()
        {
            var parameters = new ModalParameters();
            parameters.Add("currentUser", UserSession.User);

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
            UserSession.SetUserProfileReturnURI(NavigationManager.Uri);
            NavigationManager.NavigateTo("/userprofile");
        }
    }
}
