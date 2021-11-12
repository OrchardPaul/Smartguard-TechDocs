using Blazored.Modal;
using GadjIT.ClientContext.P4W;
using GadjIT.ClientContext.P4W.Custom;
using GadjIT_App.Data.Admin;
using GadjIT_App.Services;
using GadjIT_App.Services.AppState;
using GadjIT_App.Services.SessionState;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GadjIT_App.Pages.Chapters
{
    public partial class ChapterDataViewComparison
    {
        [CascadingParameter]
        BlazoredModalInstance ModalInstance { get; set; }

        [Inject]
        IChapterManagementService chapterManagementService { get; set; }

        [Inject]
        IAppChapterState appChapterState { get; set; }

        [Parameter]
        public IUserSessionState sessionState { get; set; }

        [Parameter]
        public VmDataViews Object { get; set; }

        [Parameter]
        public UsrOrsfSmartflows CurrentChapterRow { get; set; }

        [Parameter]
        public UsrOrsfSmartflows AltChapterRow { get; set; }

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

        [Parameter]
        public ICompanyDbAccess CompanyDbAccess { get; set; }

        [Parameter]
        public bool CreateNewSmartflow { get; set; }

        private async void Close()
        {
            await ModalInstance.CloseAsync();
        }

        private async void HandleValidSubmit(bool TakeAlternate)
        {
            if (TakeAlternate)
            {
                var taskObject = CurrentChapter.DataViews.Where(C => C.ViewName == Object.DataView.ViewName).SingleOrDefault();

                taskObject.BlockNo = Object.AltDataView.BlockNo;
                taskObject.DisplayName = Object.AltDataView.DisplayName;
                taskObject.ViewName = Object.AltDataView.ViewName;

                CurrentChapterRow.SmartflowData = JsonConvert.SerializeObject(CurrentChapter);
                await chapterManagementService.Update(CurrentChapterRow).ConfigureAwait(false);
            }
            else
            {
                var taskObject = AltChapter.DataViews.Where(C => C.ViewName == Object.DataView.ViewName).SingleOrDefault();

                taskObject.BlockNo = Object.DataView.BlockNo;
                taskObject.DisplayName = Object.DataView.DisplayName;
                taskObject.ViewName = Object.DataView.ViewName;


                await sessionState.SwitchSelectedSystem();
                AltChapterRow.SmartflowData = JsonConvert.SerializeObject(AltChapter);
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
                var AltObject = new DataViews
                {
                    BlockNo = Object.AltDataView.BlockNo,
                    DisplayName = Object.AltDataView.DisplayName,
                    ViewName = Object.AltDataView.ViewName
                };

                CurrentChapter.DataViews.Add(AltObject);

                CurrentChapterRow.SmartflowData = JsonConvert.SerializeObject(CurrentChapter);
                await chapterManagementService.Update(CurrentChapterRow).ConfigureAwait(false);
            }
            else
            {
                if (!CreateNewSmartflow)
                {
                    var PushObject = new DataViews
                    {
                        BlockNo = Object.DataView.BlockNo,
                        DisplayName = Object.DataView.DisplayName,
                        ViewName = Object.DataView.ViewName
                    };

                    AltChapter.DataViews = AltChapter.DataViews is null ? new List<DataViews>() : AltChapter.DataViews;
                    AltChapter.DataViews.Add(PushObject);

                    await sessionState.SwitchSelectedSystem();
                    AltChapterRow.SmartflowData = JsonConvert.SerializeObject(AltChapter);
                    await chapterManagementService.Update(AltChapterRow);
                    await sessionState.ResetSelectedSystem();
                }
                else
                {
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
                }
            }

            //keep track of time last updated ready for comparison by other sessions checking for updates
            appChapterState.SetLastUpdated(sessionState, CurrentChapter);

            ComparisonRefresh?.Invoke();
            Close();
        }
    }
}
