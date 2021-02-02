using GadjIT.ClientContext.P4W;
using Gizmo_V1_02.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gizmo_V1_02.Pages.Shared.Modals
{
    public partial class ModalInfo
    {
        [Parameter]
        public string InfoHeader { get; set; }

        [Parameter]
        public string InfoText { get; set; }

        [Parameter]
        public string ModalHeight { get; set; }

        [Parameter]
        public string ModalWidth { get; set; }


    }
}
