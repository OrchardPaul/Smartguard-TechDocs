using Blazored.Modal;
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
    public partial class ChapterCaseTypeEdit
    {
        [CascadingParameter]
        BlazoredModalInstance ModalInstance { get; set; }

        [Inject]
        IChapterManagementService chapterManagementService { get; set; }

        [Parameter]
        public string TaskObject { get; set; }

        [Parameter]
        public string originalName { get; set; }

        [Parameter]
        public string caseTypeGroupName { get; set; }

        [Parameter]
        public string isCaseTypeOrGroup { get; set; }

        [Parameter]
        public Action DataChanged { get; set; }

        private async void Close()
        {
            await ModalInstance.CloseAsync();
        }

        private async void HandleValidSubmit()
        {
            if (isCaseTypeOrGroup == "CaseType")
            {
                await chapterManagementService.UpdateCaseType(TaskObject,originalName, caseTypeGroupName);
            }
            else
            {
                await chapterManagementService.UpdateCaseTypeGroups(TaskObject, originalName);
            }

            DataChanged?.Invoke();
            Close();
        }

    }
}
