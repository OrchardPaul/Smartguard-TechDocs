using Blazored.Modal;
using GadjIT_ClientContext.Models.Smartflow;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace GadjIT_App.Pages.Smartflows.ComponentsSmartflowDetail._DataView
{
    public partial class ModalSmartflowDataViewDisplay
    {
        [CascadingParameter]
        BlazoredModalInstance ModalInstance { get; set; }

        [Parameter]
        public VmSmartflowDataView _Object { get; set; }

        private async Task Close()
        {
            await ModalInstance.CloseAsync();
        }


    }
}
