using Blazored.Modal;
using GadjIT_ClientContext.P4W;
using GadjIT_ClientContext.P4W.Custom;
using GadjIT_App.Services;
using GadjIT_App.Services.SessionState;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GadjIT_App.Pages.Chapters.ComponentsChapterDetail._SharedItems
{
    public partial class ModalChapterDetailView
    {
        [CascadingParameter]
        BlazoredModalInstance ModalInstance { get; set; }

        [Parameter]
        public VmGenSmartflowItem _Object { get; set; }

        [Parameter]
        public string _SelectedList { get; set; }

        private async Task Close()
        {
            await ModalInstance.CloseAsync();
        }


    }
}
