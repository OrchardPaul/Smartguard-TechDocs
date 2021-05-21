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
    public partial class ChapterFeeComparison
    {
        [CascadingParameter]
        BlazoredModalInstance ModalInstance { get; set; }

        [Inject]
        IChapterManagementService chapterManagementService { get; set; }

        [Parameter]
        public IUserSessionState sessionState { get; set; }



        [Parameter]
        public VmFee Object { get; set; }

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
                var taskObject = CurrentChapter.Fees.Where(C => C.FeeName == Object.FeeObject.FeeName).SingleOrDefault();

                taskObject.SeqNo = Object.AltObject.SeqNo;
                taskObject.FeeName = Object.AltObject.FeeName;
                taskObject.FeeCategory = Object.AltObject.FeeCategory;
                taskObject.Amount = Object.AltObject.Amount;
                taskObject.VATable = Object.AltObject.VATable;
                taskObject.PostingType = Object.AltObject.PostingType;

                CurrentChapterRow.SmartflowData = JsonConvert.SerializeObject(CurrentChapter);
                await chapterManagementService.Update(CurrentChapterRow).ConfigureAwait(false);
            }
            else
            {
                var taskObject = AltChapter.Fees.Where(C => C.FeeName == Object.FeeObject.FeeName).SingleOrDefault();

                taskObject.SeqNo = Object.FeeObject.SeqNo;
                taskObject.FeeName = Object.FeeObject.FeeName;
                taskObject.FeeCategory = Object.FeeObject.FeeCategory;
                taskObject.Amount = Object.FeeObject.Amount;
                taskObject.VATable = Object.FeeObject.VATable;
                taskObject.PostingType = Object.FeeObject.PostingType;

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
                var AltObject = new Fee
                {
                    SeqNo = Object.AltObject.SeqNo,
                    FeeName = Object.AltObject.FeeName,
                    FeeCategory = Object.AltObject.FeeCategory,
                    Amount = Object.AltObject.Amount,
                    VATable = Object.AltObject.VATable,
                    PostingType = Object.AltObject.PostingType,
                };

                CurrentChapter.Fees.Add(AltObject);

                CurrentChapterRow.SmartflowData = JsonConvert.SerializeObject(CurrentChapter);
                await chapterManagementService.Update(CurrentChapterRow).ConfigureAwait(false);
            }
            else
            {
                if (!CreateNewSmartflow)
                {
                    var PushObject = new Fee
                    {
                        SeqNo = Object.FeeObject.SeqNo,
                        FeeName = Object.FeeObject.FeeName,
                        FeeCategory = Object.FeeObject.FeeCategory,
                        Amount = Object.FeeObject.Amount,
                        VATable = Object.FeeObject.VATable,
                        PostingType = Object.FeeObject.PostingType,
                    };

                    AltChapter.Fees = AltChapter.Fees is null ? new List<Fee>() : AltChapter.Fees;
                    AltChapter.Fees.Add(PushObject);

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

            ComparisonRefresh?.Invoke();
            Close();
        }
    }
}
