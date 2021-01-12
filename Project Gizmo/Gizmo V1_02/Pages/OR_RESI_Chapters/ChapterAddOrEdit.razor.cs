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
    public partial class ChapterAddOrEdit
    {
        [CascadingParameter]
        BlazoredModalInstance ModalInstance { get; set; }

        [Inject]
        IChapterManagementService chapterManagementService { get; set; }

        [Parameter]
        public UsrOrDefChapterManagement TaskObject { get; set; }

        [Parameter]
        public Action DataChanged { get; set; }

        [Parameter]
        public List<UsrOrDefChapterManagement> AllObjects { get; set; }

        [Parameter]
        public bool addNewCaseTypeGroupOption { get; set; } = false;

        [Parameter]
        public bool addNewCaseTypeOption { get; set; } = false;

        private async void Close()
        {
            await ModalInstance.CloseAsync();
        }

        private void ToggleNewCaseTypeGroupOption()
        {
            addNewCaseTypeGroupOption = !addNewCaseTypeGroupOption;
            addNewCaseTypeOption = (addNewCaseTypeGroupOption) ? true : false;
        }

        private void ToggleNewCaseTypeOption()
        {
            addNewCaseTypeOption = !addNewCaseTypeOption;
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
            DataChanged?.Invoke();
            Close();
        }

        private async void HandleValidDelete()
        {
            await chapterManagementService.DeleteChapter(TaskObject.Id);

            DataChanged?.Invoke();
            Close();
        }
    }
}
