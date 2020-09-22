using Gizmo.Context.OR_RESI;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gizmo_V1_02.Pages.OR_RESI_Chapters
{
    public partial class ChapterDetail : ComponentBase
    {
        [Parameter]
        public UsrOrDefChapterManagement TaskObject { get; set; }

        [Parameter]
        public RenderFragment CustomHeader { get; set; }

        [Parameter]
        public String selectedList { get; set; }

        [Parameter]
        public Action DataChanged { get; set; }

        [Parameter]
        public String selectedCaseType { get; set; }

        [Parameter]
        public List<String> dropDownDocumentList { get; set; }

        List<string> CaseTypeList = new List<string>() { "Purchase", "Sale", "Remortgage", "Plot Sales", "Transfer" };

        List<string> DocTypeList = new List<string>() { "Chapter", "Doc", "Form", "Letter", "Step" };

        public List<String> documentList;

        private async Task ClosechapterModal()
        {
            await jsRuntime.InvokeAsync<object>("CloseModal", "taskModal");
        }

        private async void HandleValidSubmit()
        {
            if (TaskObject.Id == 0)
            {
                await service.Add(TaskObject);
            }
            else
            {
                await service.Update(TaskObject);
            }
            await ClosechapterModal();
            DataChanged?.Invoke();
        }

        private async void HandleValidDelete()
        {
            await service.Delete(TaskObject.Id);

            await ClosechapterModal();
            DataChanged?.Invoke();
        }
    }
}
