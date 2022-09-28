using Blazored.Modal;
using GadjIT_ClientContext.P4W;
using GadjIT_ClientContext.P4W.Custom;
using GadjIT_App.Data.Admin;
using GadjIT_App.Services;
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
    public partial class TickerMessageComparison
    {
        [CascadingParameter]
        BlazoredModalInstance ModalInstance { get; set; }

        [Inject]
        IChapterManagementService chapterManagementService { get; set; }

        [Parameter]
        public IUserSessionState UserSession { get; set; }

        [Parameter]
        public VmTickerMessages Object { get; set; }

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

        private async Task HandleValidSubmit(bool TakeAlternate)
        {
            if (TakeAlternate)
            {
                var taskObject = CurrentChapter.TickerMessages.Where(C => C.Message == Object.Message.Message).SingleOrDefault();

                taskObject.SeqNo = Object.AltMessage.SeqNo;
                taskObject.Message = Object.AltMessage.Message;
                taskObject.FromDate = Object.AltMessage.FromDate;
                taskObject.ToDate = Object.AltMessage.ToDate;


                CurrentChapterRow.SmartflowData = JsonConvert.SerializeObject(CurrentChapter);
                await chapterManagementService.Update(CurrentChapterRow).ConfigureAwait(false);
            }
            else
            {
                var taskObject = AltChapter.TickerMessages.Where(C => C.Message == Object.Message.Message).SingleOrDefault();

                taskObject.SeqNo = Object.Message.SeqNo;
                taskObject.Message = Object.Message.Message;
                taskObject.FromDate = Object.Message.FromDate;
                taskObject.ToDate = Object.Message.ToDate;

                await UserSession.SwitchSelectedSystem();
                AltChapterRow.SmartflowData = JsonConvert.SerializeObject(AltChapter);
                await chapterManagementService.Update(AltChapterRow);
                await UserSession.ResetSelectedSystem();
            }

            ComparisonRefresh?.Invoke();
            Close();

        }


        private async Task AddObject(bool TakeAlternate)
        {
            if (TakeAlternate)
            {
                var AltObject = new TickerMessages
                {
                    SeqNo = Object.AltMessage.SeqNo,
                    Message = Object.AltMessage.Message,
                    FromDate = Object.AltMessage.FromDate,
                    ToDate = Object.AltMessage.ToDate,
                };

                CurrentChapter.TickerMessages.Add(AltObject);

                CurrentChapterRow.SmartflowData = JsonConvert.SerializeObject(CurrentChapter);
                await chapterManagementService.Update(CurrentChapterRow).ConfigureAwait(false);
            }
            else
            {
                if (!CreateNewSmartflow)
                {
                    var PushObject = new TickerMessages
                    {
                        SeqNo = Object.Message.SeqNo,
                        Message = Object.Message.Message,
                        FromDate = Object.Message.FromDate,
                        ToDate = Object.Message.ToDate
                    };

                    AltChapter.TickerMessages = AltChapter.TickerMessages is null ? new List<TickerMessages>() : AltChapter.TickerMessages;
                    AltChapter.TickerMessages.Add(PushObject);

                    await UserSession.SwitchSelectedSystem();
                    AltChapterRow.SmartflowData = JsonConvert.SerializeObject(AltChapter);
                    await chapterManagementService.Update(AltChapterRow);
                    await UserSession.ResetSelectedSystem();
                }
                else
                {
                    await UserSession.SwitchSelectedSystem();

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
                    await CompanyDbAccess.SaveSmartFlowRecord(AltChapterRow, UserSession);
                    await UserSession.ResetSelectedSystem();
                }
            }

            ComparisonRefresh?.Invoke();
            Close();
        }
    }
}
