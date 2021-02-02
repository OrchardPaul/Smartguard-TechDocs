using Blazored.Modal;
using GadjIT.ClientContext.P4W;
using GadjIT.ClientContext.P4W.Custom;
using Gizmo_V1_02.Services;
using Gizmo_V1_02.Services.SessionState;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
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
                Object.ChapterObject.SeqNo = Object.AltObject.SeqNo;
                Object.ChapterObject.SuppressStep = Object.AltObject.SuppressStep;
                Object.ChapterObject.EntityType = Object.AltObject.EntityType;
                Object.ChapterObject.CompleteName = Object.AltObject.CompleteName;
                Object.ChapterObject.AsName = Object.AltObject.AsName;
                Object.ChapterObject.RescheduleDays = Object.AltObject.RescheduleDays;

                await chapterManagementService.Update(Object.ChapterObject);
            }
            else
            {
                Object.AltObject.SeqNo = Object.ChapterObject.SeqNo;
                Object.AltObject.SuppressStep = Object.ChapterObject.SuppressStep;
                Object.AltObject.EntityType = Object.ChapterObject.EntityType;
                Object.AltObject.CompleteName = Object.ChapterObject.CompleteName;
                Object.AltObject.AsName = Object.ChapterObject.AsName;
                Object.AltObject.RescheduleDays = Object.ChapterObject.RescheduleDays;

                await sessionState.SwitchSelectedSystem();
                await chapterManagementService.Update(Object.AltObject);
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
                    RescheduleDays = Object.AltObject.RescheduleDays
                };

                await chapterManagementService.Add(AltObject);
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
                    RescheduleDays = Object.ChapterObject.RescheduleDays
                };

                await sessionState.SwitchSelectedSystem();
                await chapterManagementService.Add(PushObject);
                await sessionState.ResetSelectedSystem();
            }

            ComparisonRefresh?.Invoke();
            Close();
        }
    }
}
