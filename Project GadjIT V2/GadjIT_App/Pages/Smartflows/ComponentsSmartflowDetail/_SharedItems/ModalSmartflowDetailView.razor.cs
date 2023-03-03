using Blazored.Modal;
using GadjIT_ClientContext.Models.Smartflow;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace GadjIT_App.Pages.Smartflows.ComponentsSmartflowDetail._SharedItems
{
    public partial class ModalSmartflowDetailView
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
