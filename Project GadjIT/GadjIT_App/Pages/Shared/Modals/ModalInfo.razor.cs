using Blazored.Modal;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace GadjIT_App.Pages.Shared.Modals
{
    public partial class ModalInfo
    {
        [CascadingParameter]
        BlazoredModalInstance ModalInstance { get; set; }


        [Parameter]
        public string InfoText { get; set; }

        private async Task Close()
        {
            await ModalInstance.CloseAsync();
        }

    }
}
