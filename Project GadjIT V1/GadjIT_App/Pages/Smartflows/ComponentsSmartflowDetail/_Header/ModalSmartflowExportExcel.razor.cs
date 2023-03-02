using Blazored.Modal;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;
using GadjIT_App.Pages.Smartflows.FileHandling;
using GadjIT_ClientContext.Models.P4W;
using GadjIT_ClientContext.Models.Smartflow;

namespace GadjIT_App.Pages.Smartflows.ComponentsSmartflowDetail._Header
{
    public partial class ModalSmartflowExportExcel
    {
        [CascadingParameter]
        BlazoredModalInstance ModalInstance { get; set; }

        [Parameter]
        public ISmartflowFileHelper SmartflowFileHelper { get; set; }

        [Parameter]
        public Smartflow SelectedChapter { get; set; }

        [Parameter]
        public List<P4W_DmDocuments> Documents { get; set; }

        [Parameter]
        public List<P4W_CaseTypeGroups> CaseTypeGroups { get; set; }


/// <summary>
/// This modal is currently not active: PT - 27/02/2023
/// </summary>


        private async void Close()
        {
            await ModalInstance.CloseAsync();
        }

        private async Task WriteToExcel()
        {
            await SmartflowFileHelper.WriteSmartflowDataToExcel(SelectedChapter, Documents, CaseTypeGroups); //CustomPath set in Parent Component
            Close();
        }

    }
}
