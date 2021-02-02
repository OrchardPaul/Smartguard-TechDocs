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
        public IUserSessionState userSession { get; set; }

        [Inject]
        public NavigationManager navigationManager { get; set; }

        private bool collapseNavMenu = true;

        private string NavMenuCssClass => collapseNavMenu ? "collapse" : null;

        public string ModalInfoHeader { get; set; }
        public string ModalHeight { get; set; }
        public string ModalWidth { get; set; }

        private async Task ToggleNavMenu()
        {
            collapseNavMenu = !collapseNavMenu;

            await CloseAllModels();
        }

        private async Task CloseAllModels()
        {
            await jsRuntime.InvokeAsync<object>("HideAll");
        }

        private void PrepareModalDelete(string modalHeader
                                        , string modalHeight
                                        , string modalWidth)
        {
            ModalInfoHeader = modalHeader;
            ModalHeight = modalHeight;
            ModalWidth = modalWidth;
        }


        public void NavigateToUserProfile()
        {
            //set Return URI
            userSession.SetUserProfileReturnURI(navigationManager.Uri);
            navigationManager.NavigateTo("/userprofile");
        }
    }
}
