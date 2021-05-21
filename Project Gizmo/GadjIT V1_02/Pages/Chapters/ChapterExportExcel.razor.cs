using Blazored.Modal;
using GadjIT.ClientContext.P4W;
using GadjIT.ClientContext.P4W.Custom;
using GadjIT_V1_02.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GadjIT_V1_02.Pages.Chapters
{
    public partial class ChapterExportExcel
    {
        [CascadingParameter]
        BlazoredModalInstance ModalInstance { get; set; }

        [Parameter]
        public IChapterFileUpload ChapterFileUpload { get; set; }

        [Parameter]
        public VmChapter SelectedChapter { get; set; }

        [Parameter]
        public List<DmDocuments> Documents { get; set; }

        [Parameter]
        public List<CaseTypeGroups> CaseTypeGroups { get; set; }

        private async void Close()
        {
            await ModalInstance.CloseAsync();
        }

        private async void WriteToExcel()
        {
            await ChapterFileUpload.WriteChapterDataToExcel(SelectedChapter, Documents, CaseTypeGroups);
            Close();
        }

    }
}
