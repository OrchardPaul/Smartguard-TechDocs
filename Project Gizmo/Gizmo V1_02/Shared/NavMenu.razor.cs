using Microsoft.AspNetCore.Components;
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


        private void ToggleNavMenu()
        {
            collapseNavMenu = !collapseNavMenu;
        }

    }
}
