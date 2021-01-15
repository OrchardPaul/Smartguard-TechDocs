using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gizmo_V1_02.Pages.Shared.Modals
{
    public partial class ModalTooltip
    {
        [Parameter] public RenderFragment ChildContent { get; set; }
        [Parameter] public string Text { get; set; }
        [Parameter] public string Direction { get; set; } = "top";

        protected override void OnInitialized()
        {
            Direction = "tooltip-" + Direction.ToLower();
        }
    }

    
}
