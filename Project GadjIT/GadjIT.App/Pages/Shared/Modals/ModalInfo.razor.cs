using Blazored.Modal;
using GadjIT.ClientContext.P4W;
using GadjIT_App.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GadjIT_App.Pages.Shared.Modals
{
    public partial class ModalInfo
    {
        [CascadingParameter]
        BlazoredModalInstance ModalInstance { get; set; }


        [Parameter]
        public string InfoText { get; set; }

        private async Task Close()
        {
            await ModalInstance.CloseAsync();
        }

    }
}
