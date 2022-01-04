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
    public partial class ModalDelete
    {
        [CascadingParameter]
        BlazoredModalInstance ModalInstance { get; set; }

        [Parameter]
        public string InfoHeader { get; set; }
        
        [Parameter]
        public string ItemName { get; set; }

        [Parameter]
        public string InfoText { get; set; }

        [Parameter]
        public string ModalHeight { get; set; }

        [Parameter]
        public string ModalWidth { get; set; }

        [Parameter]
        public Action DeleteAction { get; set; }

        private void HandleValidSubmit()
        {
            DeleteAction?.Invoke();
            Close();
        }

        private async void Close()
        {
            await ModalInstance.CloseAsync();
        }

    }
}
