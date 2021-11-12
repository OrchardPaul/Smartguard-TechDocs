using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GadjIT_V1_02.Pages.Shared.Modals
{
    public partial class ModalTooltip
    {
        [Parameter] public RenderFragment ChildContent { get; set; }
        [Parameter] public string Text { get; set; }
        [Parameter] public string Direction { get; set; } = "top";
        [Parameter] public string Class { get; set; } = "";


        protected override void OnParametersSet()
        {
         
            Direction = "tooltip-" + Direction.ToLower();
            
            if(Class != "")
            {
                Class = "tooltip-" + Class.ToLower();
            }
        }
    }

    
}
