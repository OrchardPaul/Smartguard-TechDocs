using Blazored.Modal;
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
    public partial class ModalErrorInfo
    {
        [CascadingParameter]
        BlazoredModalInstance ModalInstance { get; set; }

        [Parameter]
        public string ErrorDesc { get; set; }

        [Parameter]
        public IList<string> ErrorDetails { get; set; }

        private async void Close()
        {
            await ModalInstance.CloseAsync();
        }

    }
}
