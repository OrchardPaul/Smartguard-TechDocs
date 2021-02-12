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
    public partial class ModalConfirm
    {
        [CascadingParameter]
        BlazoredModalInstance ModalInstance { get; set; }

        [Parameter]
        public string InfoHeader { get; set; }

        [Parameter]
        public string InfoText { get; set; }

        [Parameter]
        public string ModalHeight { get; set; }

        [Parameter]
        public string ModalWidth { get; set; }

        [Parameter]
        public Action ConfirmAction { get; set; }

        public bool ConfirmDecision { get; set; } = false;

        public void ToggleConfirmDecision()
        {
            ConfirmDecision = !ConfirmDecision;
        }

        private void HandleValidSubmit()
        {
            ConfirmAction?.Invoke();
            Close();
        }

        private async void Close()
        {
            await ModalInstance.CloseAsync();
        }

    }
}
