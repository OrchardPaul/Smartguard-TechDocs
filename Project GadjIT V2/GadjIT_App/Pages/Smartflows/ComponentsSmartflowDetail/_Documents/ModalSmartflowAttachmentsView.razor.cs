using Blazored.Modal;
using GadjIT_ClientContext.Models.Smartflow;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace GadjIT_App.Pages.Smartflows.ComponentsSmartflowDetail._Documents
{
    public partial class ModalSmartflowAttachmentsView
    {
        [CascadingParameter]
        BlazoredModalInstance ModalInstance { get; set; }

        [Parameter]
        public LinkedItem _Attachment { get; set; }

        [Parameter]
        public Smartflow _SelectedSmartflow { get; set; }

        private async Task Close()
        {
            await ModalInstance.CloseAsync();
        }
    }
}
