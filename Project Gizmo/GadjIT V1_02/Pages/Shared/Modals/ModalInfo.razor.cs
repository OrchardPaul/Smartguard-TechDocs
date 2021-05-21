using Blazored.Modal;
using GadjIT.ClientContext.P4W;
using GadjIT_V1_02.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GadjIT_V1_02.Pages.Shared.Modals
{
    public partial class ModalInfo
    {
        [CascadingParameter]
        BlazoredModalInstance ModalInstance { get; set; }


        [Parameter]
        public string InfoText { get; set; }

        private async void Close()
        {
            await ModalInstance.CloseAsync();
        }

    }
}
