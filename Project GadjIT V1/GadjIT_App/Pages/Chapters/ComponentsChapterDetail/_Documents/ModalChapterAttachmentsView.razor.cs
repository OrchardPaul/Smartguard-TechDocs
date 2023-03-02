using Blazored.Modal;
using GadjIT_ClientContext.P4W;
using GadjIT_ClientContext.P4W.Custom;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GadjIT_App.Pages.Chapters.ComponentsChapterDetail._Documents
{
    public partial class ModalChapterAttachmentsView
    {
        [CascadingParameter]
        BlazoredModalInstance ModalInstance { get; set; }

        [Parameter]
        public LinkedItem _Attachment { get; set; }

        [Parameter]
        public VmSmartflow _SelectedChapter { get; set; }

        private async Task Close()
        {
            await ModalInstance.CloseAsync();
        }
    }
}
