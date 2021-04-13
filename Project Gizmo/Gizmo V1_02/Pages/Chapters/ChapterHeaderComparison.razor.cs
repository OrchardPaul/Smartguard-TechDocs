using Blazored.Modal;
using GadjIT.ClientContext.P4W;
using GadjIT.ClientContext.P4W.Custom;
using Gizmo_V1_02.Services;
using Gizmo_V1_02.Services.SessionState;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gizmo_V1_02.Pages.Chapters
{
    public partial class ChapterHeaderComparison
    {
        [CascadingParameter]
        BlazoredModalInstance ModalInstance { get; set; }

        [Inject]
        IChapterManagementService chapterManagementService { get; set; }

        [Parameter]
        public IUserSessionState sessionState { get; set; }

        [Parameter]
        public VmChapterComparison Object { get; set; }

        [Parameter]
        public VmChapter CurrentChapter { get; set; }

        [Parameter]
        public VmChapter AltChapter { get; set; }

        [Parameter]
        public UsrOrDefChapterManagement CurrentChapterRow { get; set; }

        [Parameter]
        public UsrOrDefChapterManagement AltChapterRow { get; set; }

        [Parameter]
        public int CurrentSysParentId { get; set; }

        [Parameter]
        public int? AlternateSysParentId { get; set; }


        private async void Close()
        {
            await ModalInstance.CloseAsync();
        }

        private async void HandleValidSubmit(bool TakeAlternate)
        {
            VmChapter toObject; //= new VmChapter();
            VmChapter fromObject; // = new VmChapter();

            if (TakeAlternate)
            {
                toObject = CurrentChapter;
                fromObject = AltChapter;
            }
            else
            {
                toObject = AltChapter;
                fromObject = CurrentChapter;

            }
            toObject.CaseTypeGroup = fromObject.CaseTypeGroup;
            toObject.CaseType = fromObject.CaseType;
            toObject.StepName = fromObject.StepName;
            toObject.BackgroundColour = fromObject.BackgroundColour;
            toObject.BackgroundColourName = fromObject.BackgroundColourName;
            toObject.BackgroundImage = fromObject.BackgroundImage;
            toObject.BackgroundImageName = fromObject.BackgroundImageName;

            if (TakeAlternate)
            {
                CurrentChapterRow.ChapterData = JsonConvert.SerializeObject(CurrentChapter);
                await chapterManagementService.Update(CurrentChapterRow).ConfigureAwait(false);
            }
            else
            {
                await sessionState.SwitchSelectedSystem();
                AltChapterRow.ChapterData = JsonConvert.SerializeObject(AltChapter);
                await chapterManagementService.Update(AltChapterRow);
                await sessionState.ResetSelectedSystem();
            }

            Close();

        }

    }
}
