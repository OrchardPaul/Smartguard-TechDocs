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
    public partial class ChapterItemComparison
    {
        [CascadingParameter]
        BlazoredModalInstance ModalInstance { get; set; }

        [Inject]
        IChapterManagementService chapterManagementService { get; set; }

        [Parameter]
        public IUserSessionState UserSession { get; set; }

        [Parameter]
        public VmUsrOrDefChapterManagement Object { get; set; }

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
                var taskObject = CurrentChapter.Items
                                                .Where(C => C.Type == Object.ChapterObject.Type)
                                                .Where(C => C.Name == Object.ChapterObject.Name)
                                                .FirstOrDefault();

                taskObject.SeqNo = Object.AltObject.SeqNo;
                taskObject.SuppressStep = Object.AltObject.SuppressStep;
                taskObject.EntityType = Object.AltObject.EntityType;
                taskObject.CompleteName = Object.AltObject.CompleteName;
                taskObject.AsName = Object.AltObject.AsName;
                taskObject.RescheduleDays = Object.AltObject.RescheduleDays;
                taskObject.RescheduleDataItem = Object.AltObject.RescheduleDataItem;
                taskObject.AltDisplayName = Object.AltObject.AltDisplayName;
                taskObject.UserMessage = Object.AltObject.UserMessage;
                taskObject.PopupAlert = Object.AltObject.PopupAlert;
                taskObject.NextStatus = Object.AltObject.NextStatus;
                taskObject.Action = Object.AltObject.Action;
                taskObject.LinkedItems = Object.AltObject.LinkedItems;

                CurrentChapterRow.SmartflowData = JsonConvert.SerializeObject(CurrentChapter);
                await chapterManagementService.Update(CurrentChapterRow).ConfigureAwait(false);
            }
            else
            {
                var taskObject = AltChapter.Items
                                            .Where(C => C.Type == Object.AltObject.Type)
                                            .Where(C => C.Name == Object.AltObject.Name)
                                            .FirstOrDefault();

                taskObject.SeqNo = Object.ChapterObject.SeqNo;
                taskObject.SuppressStep = Object.ChapterObject.SuppressStep;
                taskObject.EntityType = Object.ChapterObject.EntityType;
                taskObject.CompleteName = Object.ChapterObject.CompleteName;
                taskObject.AsName = Object.ChapterObject.AsName;
                taskObject.RescheduleDays = Object.ChapterObject.RescheduleDays;
                taskObject.RescheduleDataItem = Object.ChapterObject.RescheduleDataItem;
                taskObject.AltDisplayName = Object.ChapterObject.AltDisplayName;
                taskObject.UserMessage = Object.ChapterObject.UserMessage;
                taskObject.PopupAlert = Object.ChapterObject.PopupAlert;
                taskObject.NextStatus = Object.ChapterObject.NextStatus;
                taskObject.Action = Object.ChapterObject.Action;
                taskObject.LinkedItems = Object.ChapterObject.LinkedItems;


                bool gotLock = UserSession.Lock;
                while (gotLock)
                {
                    await Task.Yield();
                    gotLock = UserSession.Lock;
                }


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
                var AltObject = new GenSmartflowItem
                {
                    Type = Object.AltObject.Type,
                    Name = Object.AltObject.Name,
                    EntityType = Object.AltObject.EntityType,
                    SeqNo = Object.AltObject.SeqNo,
                    SuppressStep = Object.AltObject.SuppressStep,
                    CompleteName = Object.AltObject.CompleteName,
                    AsName = Object.AltObject.AsName,
                    RescheduleDays = Object.AltObject.RescheduleDays,
                    RescheduleDataItem = Object.AltObject.RescheduleDataItem,
                    AltDisplayName = Object.AltObject.AltDisplayName,
                    UserMessage = Object.AltObject.UserMessage,
                    PopupAlert = Object.AltObject.PopupAlert,
                    NextStatus = Object.AltObject.NextStatus,
                    Action = Object.AltObject.Action,
                    LinkedItems = Object.AltObject.LinkedItems
                };

                CurrentChapter.Items.Add(AltObject);

                CurrentChapterRow.SmartflowData = JsonConvert.SerializeObject(CurrentChapter);
                await chapterManagementService.Update(CurrentChapterRow).ConfigureAwait(false);
            }
            else
            {
                if (!CreateNewSmartflow)
                {
                    var PushObject = new GenSmartflowItem
                    {
                        Type = Object.ChapterObject.Type,
                        Name = Object.ChapterObject.Name,
                        EntityType = Object.ChapterObject.EntityType,
                        SeqNo = Object.ChapterObject.SeqNo,
                        SuppressStep = Object.ChapterObject.SuppressStep,
                        CompleteName = Object.ChapterObject.CompleteName,
                        AsName = Object.ChapterObject.AsName,
                        RescheduleDays = Object.ChapterObject.RescheduleDays,
                        RescheduleDataItem = Object.ChapterObject.RescheduleDataItem,
                        AltDisplayName = Object.ChapterObject.AltDisplayName,
                        UserMessage = Object.ChapterObject.UserMessage,
                        PopupAlert = Object.ChapterObject.PopupAlert,
                        NextStatus = Object.ChapterObject.NextStatus,
                        Action = Object.ChapterObject.Action,
                        LinkedItems = Object.ChapterObject.LinkedItems
                    };

                    AltChapter.Items.Add(PushObject);

                    bool gotLock = UserSession.Lock;
                    while (gotLock)
                    {
                        await Task.Yield();
                        gotLock = UserSession.Lock;
                    }


                    await UserSession.SwitchSelectedSystem();
                    AltChapterRow.SmartflowData = JsonConvert.SerializeObject(AltChapter);
                    await chapterManagementService.Update(AltChapterRow);
                    await UserSession.ResetSelectedSystem();
                }
                else
                {

                    bool gotLock = UserSession.Lock;
                    while (gotLock)
                    {
                        await Task.Yield();
                        gotLock = UserSession.Lock;
                    }


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
