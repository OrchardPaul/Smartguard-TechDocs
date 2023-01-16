using Blazored.Modal;
using GadjIT_ClientContext.P4W;
using GadjIT_App.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GadjIT_App.Pages._Shared.Modals
{
    public partial class ModalErrorInfo
    {
        [CascadingParameter]
        BlazoredModalInstance ModalInstance { get; set; }

        [Parameter]
        public string ErrorDesc { get; set; }

        [Parameter]
        public IList<string> ErrorDetails { get; set; }

        private async Task Close()
        {
            await ModalInstance.CloseAsync();
        }

    }
}
