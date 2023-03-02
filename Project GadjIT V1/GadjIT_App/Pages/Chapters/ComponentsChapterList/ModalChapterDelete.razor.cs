using Blazored.Modal;
using GadjIT_ClientContext.P4W;
using GadjIT_App.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GadjIT_ClientContext.P4W.Custom;

namespace GadjIT_App.Pages.Chapters.ComponentsChapterList
{
    public partial class ModalChapterDelete
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
