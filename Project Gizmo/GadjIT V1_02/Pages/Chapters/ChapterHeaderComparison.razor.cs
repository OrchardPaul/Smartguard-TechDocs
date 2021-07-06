using Blazored.Modal;
using GadjIT.ClientContext.P4W;
using GadjIT.ClientContext.P4W.Custom;
using GadjIT_V1_02.Data.Admin;
using GadjIT_V1_02.Services;
using GadjIT_V1_02.Services.SessionState;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GadjIT_V1_02.Pages.Chapters
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
        public Action ComparisonRefresh { get; set; }


        [Parameter]
        public ICompanyDbAccess CompanyDbAccess { get; set; }



        [Parameter]
        public VmChapter AltChapter { get; set; }

        [Parameter]
        public UsrOrsfSmartflows CurrentChapterRow { get; set; }

        [Parameter]
        public UsrOrsfSmartflows AltChapterRow { get; set; }

        [Parameter]
        public int CurrentSysParentId { get; set; }

        [Parameter]
        public int? AlternateSysParentId { get; set; }

        public bool syncCaseTypeGroup { get; set; }
        public bool syncView { get; set; }
        public bool syncStepName { get; set; }
        public bool syncBgColour { get; set; }
        public bool syncBgImage { get; set; }
        public bool syncShowPartnerNotes { get; set; }
        public bool syncShowDocumentTracking { get; set; }


        protected override void OnInitialized()
        {
            syncCaseTypeGroup = CurrentChapter.P4WCaseTypeGroup == AltChapter.P4WCaseTypeGroup ? false : true;
            syncView  = CurrentChapter.SelectedView == AltChapter.SelectedView ? false : true; 
            syncStepName  = CurrentChapter.StepName == AltChapter.StepName ? false : true;
            syncBgColour  = CurrentChapter.BackgroundColour == AltChapter.BackgroundColour ? false : true;
            syncBgImage  = CurrentChapter.BackgroundImage == AltChapter.BackgroundImage ? false : true;
            syncShowPartnerNotes = CurrentChapter.ShowPartnerNotes == AltChapter.ShowPartnerNotes ? false : true;
            syncShowDocumentTracking = CurrentChapter.ShowDocumentTracking == AltChapter.ShowDocumentTracking ? false : true;
        }

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
            if (syncCaseTypeGroup)
            {
                toObject.P4WCaseTypeGroup = fromObject.P4WCaseTypeGroup;
            }
            if (syncView)
            {
                toObject.SelectedView = fromObject.SelectedView;
            }
            if (syncStepName)
            {
                toObject.StepName = fromObject.StepName;
            }
            if (syncBgColour)
            {
                toObject.BackgroundColour = fromObject.BackgroundColour;
                toObject.BackgroundColourName = fromObject.BackgroundColourName;
            }
            if (syncBgImage)
            {
                toObject.BackgroundImage = fromObject.BackgroundImage;
                toObject.BackgroundImageName = fromObject.BackgroundImageName;
            }
            if (syncShowPartnerNotes)
            {
                toObject.ShowPartnerNotes = fromObject.ShowPartnerNotes;
            }
            if (syncShowDocumentTracking)
            {
                toObject.ShowDocumentTracking = fromObject.ShowDocumentTracking;
            }


            if (TakeAlternate)
            {
                CurrentChapterRow.SmartflowData = JsonConvert.SerializeObject(CurrentChapter);
                await chapterManagementService.Update(CurrentChapterRow).ConfigureAwait(false);
            }
            else
            {
                await sessionState.SwitchSelectedSystem();
                AltChapterRow.SmartflowData = JsonConvert.SerializeObject(AltChapter);
                await chapterManagementService.Update(AltChapterRow);
                await sessionState.ResetSelectedSystem();
            }

            ComparisonRefresh?.Invoke();
            Close();

        }

        


        private async void AddObject()
        {
            bool gotLock = sessionState.Lock;
            while (gotLock)
            {
                await Task.Yield();
                gotLock = sessionState.Lock;
            }


            await sessionState.SwitchSelectedSystem();
                
            AltChapterRow = new UsrOrsfSmartflows
            {
                CaseTypeGroup = CurrentChapterRow.CaseTypeGroup,
                CaseType = CurrentChapterRow.CaseType,
                SmartflowName = CurrentChapterRow.SmartflowName,
                SeqNo = CurrentChapterRow.SeqNo,
                SmartflowData = CurrentChapterRow.SmartflowData,
                VariantName = CurrentChapterRow.VariantName,
                VariantNo = CurrentChapterRow.VariantNo
            };

            var returnObject = await chapterManagementService.Add(AltChapterRow);
            AltChapterRow.Id = returnObject.Id;
            await CompanyDbAccess.SaveSmartFlowRecord(AltChapterRow, sessionState);
            await sessionState.ResetSelectedSystem();
    
            ComparisonRefresh?.Invoke();
            Close();
        }


    }
}
