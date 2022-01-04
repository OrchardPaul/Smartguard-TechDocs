using Blazored.Modal;
using GadjIT.ClientContext.P4W;
using GadjIT.ClientContext.P4W.Custom;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GadjIT_App.Pages.Chapters
{
    public partial class ChapterAttachmentsView
    {
        [CascadingParameter]
        BlazoredModalInstance ModalInstance { get; set; }

        [Parameter]
        public LinkedItems Attachment { get; set; }

        [Parameter]
        public VmChapter SelectedChapter { get; set; }

        private async Task Close()
        {
            await ModalInstance.CloseAsync();
        }
    }
}
