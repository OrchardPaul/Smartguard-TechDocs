using Blazored.Modal;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;
using GadjIT_ClientContext.Models.Smartflow;

namespace GadjIT_App.Pages.Smartflows.ComponentsSmartflowDetail._Fees
{
    public partial class ModalSmartflowFeeView
    {


        [CascadingParameter]
        BlazoredModalInstance ModalInstance { get; set; }

        [Parameter]
        public VmSmartflowFee _Object { get; set; }

        private async Task Close()
        {
            await ModalInstance.CloseAsync();
        }

   
    }
}
