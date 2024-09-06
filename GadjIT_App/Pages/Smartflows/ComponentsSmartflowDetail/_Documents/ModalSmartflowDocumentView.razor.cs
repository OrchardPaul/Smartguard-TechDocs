using Blazored.Modal;
using GadjIT_ClientContext.Models.Smartflow;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace GadjIT_App.Pages.Smartflows.ComponentsSmartflowDetail._Documents
{
    public partial class ModalSmartflowDocumentView
    {
        [CascadingParameter]
        BlazoredModalInstance ModalInstance { get; set; }

        [Parameter]
        public VmSmartflowDocument _Object { get; set; }

        [Parameter]
        public string _SelectedList { get; set; }

        private async Task Close()
        {
            await ModalInstance.CloseAsync();
        }


    }
}
