using Blazored.Modal;
using GadjIT_ClientContext.P4W;
using GadjIT_ClientContext.P4W.Custom;
using GadjIT_App.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GadjIT_App.Pages.Chapters.FileUpload;

namespace GadjIT_App.Pages.Chapters.ComponentsChapterDetail._Header
{
    public partial class ModalChapterExportExcel
    {
        [CascadingParameter]
        BlazoredModalInstance ModalInstance { get; set; }

        [Parameter]
        public IChapterFileUpload ChapterFileUpload { get; set; }

        [Parameter]
        public VmSmartflow SelectedChapter { get; set; }

        [Parameter]
        public List<DmDocuments> Documents { get; set; }

        [Parameter]
        public List<CaseTypeGroups> CaseTypeGroups { get; set; }

        private async void Close()
        {
            await ModalInstance.CloseAsync();
        }

        private async Task WriteToExcel()
        {
            await ChapterFileUpload.WriteChapterDataToExcel(SelectedChapter, Documents, CaseTypeGroups);
            Close();
        }

    }
}
