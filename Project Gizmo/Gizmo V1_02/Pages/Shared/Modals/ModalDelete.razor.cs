using Blazored.Modal;
using Gizmo.Context.OR_RESI;
using Gizmo_V1_02.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Gizmo_V1_02.Pages.Shared.Modals
{
    public partial class ModalDelete
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
        public Action DeleteAction { get; set; }
        public bool WishToDelete { get; set; } = false;

        public void ToggleDeleteWish()
        {
            WishToDelete = !WishToDelete;
        }

        private async Task HandleValidSubmit()
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
