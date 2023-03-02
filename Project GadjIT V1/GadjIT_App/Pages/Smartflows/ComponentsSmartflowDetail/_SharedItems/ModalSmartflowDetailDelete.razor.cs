using Blazored.Modal;
using Microsoft.AspNetCore.Components;
using System;

namespace GadjIT_App.Pages.Smartflows.ComponentsSmartflowDetail._SharedItems
{
    public partial class ModalSmartflowDetailDelete
    {
        [CascadingParameter]
        BlazoredModalInstance ModalInstance { get; set; }

        
        [Parameter]
        public string _ItemName { get; set; }

        [Parameter]
        public string _InfoText { get; set; }


        [Parameter]
        public Action _DeleteAction { get; set; }

        private void HandleValidSubmit()
        {
            _DeleteAction?.Invoke();
            Close();
        }

        private async void Close()
        {
            await ModalInstance.CloseAsync();
        }

    }
}
