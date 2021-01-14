using Blazored.Modal;
using Gizmo.Context.OR_RESI;
using Gizmo.Context.OR_RESI.Custom;
using Gizmo_V1_02.Services;
using Gizmo_V1_02.Services.SessionState;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gizmo_V1_02.Pages.OR_RESI_Chapters
{
    public partial class ChapterItemComparison
    {
        [CascadingParameter]
        BlazoredModalInstance ModalInstance { get; set; }

        [Inject]
        IChapterManagementService chapterManagementService { get; set; }

        [Parameter]
        public IUserSessionState sessionState { get; set; }

        [Parameter]
        public VmUsrOrDefChapterManagement Object { get; set; }

        [Parameter]
        public Action DataChanged { get; set; }

        [Parameter]
        public Action ComparisonRefresh { get; set; }

        private async void Close()
        {
            await ModalInstance.CloseAsync();
        }

        private async void HandleValidSubmit(bool TakeAlternate)
        {
            if (TakeAlternate)
            {
                Object.ChapterObject.SeqNo = Object.AltObject.SeqNo;
                Object.ChapterObject.SuppressStep = Object.AltObject.SuppressStep;
                Object.ChapterObject.CompleteName = Object.AltObject.CompleteName;
                Object.ChapterObject.AsName = Object.AltObject.AsName;
                Object.ChapterObject.RescheduleDays = Object.AltObject.RescheduleDays;

                await chapterManagementService.Update(Object.ChapterObject);
            }
            else
            {
                Object.AltObject.SeqNo = Object.ChapterObject.SeqNo;
                Object.AltObject.SuppressStep = Object.ChapterObject.SuppressStep;
                Object.AltObject.CompleteName = Object.ChapterObject.CompleteName;
                Object.AltObject.AsName = Object.ChapterObject.AsName;
                Object.AltObject.RescheduleDays = Object.ChapterObject.RescheduleDays;

                await chapterManagementService.Update(Object.AltObject);
            }

            DataChanged?.Invoke();
            ComparisonRefresh?.Invoke();
            Close();

        }
    }
}
