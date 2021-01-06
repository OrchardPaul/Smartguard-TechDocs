using Gizmo.Context.OR_RESI;
using Gizmo_V1_02.Services;
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
        [Inject]
        IChapterManagementService chapterManagementService { get; set; }

        [Parameter]
        public UsrOrDefChapterManagement TaskObject { get; set; }

        [Parameter]
        public RenderFragment CustomHeader { get; set; }

        [Parameter]
        public string selectedList { get; set; }

        [Parameter]
        public Action DataChanged { get; set; }

        [Parameter]
        public string selectedCaseType { get; set; }

        [Parameter]
        public List<DmDocuments> dropDownDocumentList { get; set; }

        List<string> DocTypeList = new List<string>() { "Chapter", "Doc", "Form", "Letter", "Step" };

        public List<string> documentList;

        private async Task ClosechapterModal()
        {
            await jsRuntime.InvokeAsync<object>("CloseModal", "taskModal");
        }

        private async void HandleValidSubmit()
        {
            if (TaskObject.Id == 0)
            {
                await chapterManagementService.Add(TaskObject);
            }
            else
            {
                await chapterManagementService.Update(TaskObject);
            }

            TaskObject = new UsrOrDefChapterManagement();

            await ClosechapterModal();
            DataChanged?.Invoke();

        }

        private async void Cancel()
        {
            TaskObject = new UsrOrDefChapterManagement();

            await ClosechapterModal();
        }

        private async void HandleValidDelete()
        {
            await chapterManagementService.Delete(TaskObject.Id);

            await ClosechapterModal();
            DataChanged?.Invoke();
        }
    }
}
