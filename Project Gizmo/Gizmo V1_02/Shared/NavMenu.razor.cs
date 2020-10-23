using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gizmo_V1_02.Shared
{
    public partial class NavMenu
    {
        [Inject]
        public NavigationManager NavigationManager { get; set; }

        private bool collapseNavMenu = true;
        private bool collapseAdminMenu = true;

        private string NavMenuCssClass => collapseNavMenu ? "collapse" : null;

        protected override void OnInitialized()
        {
            var currentUri = NavigationManager.Uri;

            if (currentUri.Contains("manageuserroles") 
                | currentUri.Contains("manageusers")
                | currentUri.Contains("managecompanies")
                | currentUri.Contains("manageworktypes"))
            {
                collapseAdminMenu = false;
            }
        }


        private void ToggleNavMenu()
        {
            collapseNavMenu = !collapseNavMenu;
        }

        private void ToggleAdminMenu()
        {
            collapseAdminMenu = !collapseAdminMenu;
            StateHasChanged();
        }

    }
}
