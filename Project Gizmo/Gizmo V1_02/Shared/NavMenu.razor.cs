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

        private bool collapseNavMenu = true;

        private string NavMenuCssClass => collapseNavMenu ? "collapse" : null;


        private async Task ToggleNavMenu()
        {
            collapseNavMenu = !collapseNavMenu;

            await CloseAllModels();
        }

        private async Task CloseAllModels()
        {
            await jsRuntime.InvokeAsync<object>("HideAll");
        }

    }
}
