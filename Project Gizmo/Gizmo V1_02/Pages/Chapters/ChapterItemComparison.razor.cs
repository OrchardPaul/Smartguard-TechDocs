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
        public UsrOrDefChapterManagement CurrentChapterRow { get; set; }

        [Parameter]
        public UsrOrDefChapterManagement AltChapterRow { get; set; }

        [Parameter]
        public VmChapter CurrentChapter { get; set; }

        [Parameter]
        public VmChapter AltChapter { get; set; }

        [Parameter]
        public int CurrentSysParentId { get; set; }

        [Parameter]
        public int? AlternateSysParentId { get; set; }

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
                var taskObject = CurrentChapter.ChapterItems.Where(C => C.Name == Object.ChapterObject.Name).SingleOrDefault();

                taskObject.SeqNo = Object.AltObject.SeqNo;
                taskObject.SuppressStep = Object.AltObject.SuppressStep;
                taskObject.EntityType = Object.AltObject.EntityType;
                taskObject.CompleteName = Object.AltObject.CompleteName;
                taskObject.AsName = Object.AltObject.AsName;
                taskObject.RescheduleDays = Object.AltObject.RescheduleDays;
                taskObject.AltDisplayName = Object.AltObject.AltDisplayName;
                taskObject.UserMessage = Object.AltObject.UserMessage;
                taskObject.PopupAlert = Object.AltObject.PopupAlert;
                taskObject.NextStatus = Object.AltObject.NextStatus;
                taskObject.NextStatus = Object.AltObject.Action;

                CurrentChapterRow.ChapterData = JsonConvert.SerializeObject(CurrentChapter);
                await chapterManagementService.Update(CurrentChapterRow).ConfigureAwait(false);
            }
            else
            {
                var taskObject = AltChapter.ChapterItems.Where(C => C.Name == Object.AltObject.Name).SingleOrDefault();

                taskObject.SeqNo = Object.ChapterObject.SeqNo;
                taskObject.SuppressStep = Object.ChapterObject.SuppressStep;
                taskObject.EntityType = Object.ChapterObject.EntityType;
                taskObject.CompleteName = Object.ChapterObject.CompleteName;
                taskObject.AsName = Object.ChapterObject.AsName;
                taskObject.RescheduleDays = Object.ChapterObject.RescheduleDays;
                taskObject.AltDisplayName = Object.ChapterObject.AltDisplayName;
                taskObject.UserMessage = Object.ChapterObject.UserMessage;
                taskObject.PopupAlert = Object.ChapterObject.PopupAlert;
                taskObject.NextStatus = Object.ChapterObject.NextStatus;
                taskObject.NextStatus = Object.ChapterObject.Action;


                await sessionState.SwitchSelectedSystem();
                AltChapterRow.ChapterData = JsonConvert.SerializeObject(AltChapter);
                await chapterManagementService.Update(AltChapterRow);
                await sessionState.ResetSelectedSystem();
            }

            ComparisonRefresh?.Invoke();
            Close();

        }


        private async void AddObject(bool TakeAlternate)
        {
            if (TakeAlternate)
            {
                var AltObject = new UsrOrDefChapterManagement
                {
                    ParentId = CurrentSysParentId,
                    CaseType = Object.AltObject.CaseType,
                    CaseTypeGroup = Object.AltObject.CaseTypeGroup,
                    Type = Object.AltObject.Type,
                    Name = Object.AltObject.Name,
                    EntityType = Object.AltObject.EntityType,
                    SeqNo = Object.AltObject.SeqNo,
                    SuppressStep = Object.AltObject.SuppressStep,
                    CompleteName = Object.AltObject.CompleteName,
                    AsName = Object.AltObject.AsName,
                    RescheduleDays = Object.AltObject.RescheduleDays,
                    AltDisplayName = Object.AltObject.AltDisplayName,
                    UserMessage = Object.AltObject.UserMessage,
                    PopupAlert = Object.AltObject.PopupAlert,
                    NextStatus = Object.AltObject.NextStatus,
                    Action = Object.AltObject.Action
                };

                CurrentChapter.ChapterItems.Add(AltObject);

                CurrentChapterRow.ChapterData = JsonConvert.SerializeObject(CurrentChapter);
                await chapterManagementService.Update(CurrentChapterRow).ConfigureAwait(false);
            }
            else
            {
                var PushObject = new UsrOrDefChapterManagement
                {
                    ParentId = AlternateSysParentId,
                    CaseType = Object.ChapterObject.CaseType,
                    CaseTypeGroup = Object.ChapterObject.CaseTypeGroup,
                    Type = Object.ChapterObject.Type,
                    Name = Object.ChapterObject.Name,
                    EntityType = Object.ChapterObject.EntityType,
                    SeqNo = Object.ChapterObject.SeqNo,
                    SuppressStep = Object.ChapterObject.SuppressStep,
                    CompleteName = Object.ChapterObject.CompleteName,
                    AsName = Object.ChapterObject.AsName,
                    RescheduleDays = Object.ChapterObject.RescheduleDays,
                    AltDisplayName = Object.ChapterObject.AltDisplayName,
                    UserMessage = Object.ChapterObject.UserMessage,
                    PopupAlert = Object.ChapterObject.PopupAlert,
                    NextStatus = Object.ChapterObject.NextStatus,
                    Action = Object.ChapterObject.Action
                };

                AltChapter.ChapterItems.Add(PushObject);

                await sessionState.SwitchSelectedSystem();
                AltChapterRow.ChapterData = JsonConvert.SerializeObject(AltChapter);
                await chapterManagementService.Update(AltChapterRow);
                await sessionState.ResetSelectedSystem();
            }

            ComparisonRefresh?.Invoke();
            Close();
        }
    }
}
