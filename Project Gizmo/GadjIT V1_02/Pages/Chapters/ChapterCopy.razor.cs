using Blazored.Modal;
using GadjIT.ClientContext.P4W;
using GadjIT.ClientContext.P4W.Custom;
using GadjIT_V1_02.Data.Admin;
using GadjIT_V1_02.Services;
using GadjIT_V1_02.Services.SessionState;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace GadjIT_V1_02.Pages.Chapters
{
    public partial class ChapterCopy
    {
        public class CopyOption
        {
            public string Option { get; set; }
            public bool Selected { get; set; }
        }

        [CascadingParameter]
        BlazoredModalInstance ModalInstance { get; set; }

        [Inject]
        IChapterManagementService chapterManagementService { get; set; }

        [Parameter]
        public UsrOrsfSmartflows TaskObject { get; set; }

        [Parameter]
        public Action DataChanged { get; set; }

        [Parameter]
        public List<VmUsrOrsfSmartflows> AllChapters { get; set; }

        [Parameter]
        public VmChapter currentChapter { get; set; }

        [Parameter]
        public bool addNewCaseTypeGroupOption { get; set; } = false;

        [Parameter]
        public bool addNewCaseTypeOption { get; set; } = false;

        [Parameter]
        public ICompanyDbAccess CompanyDbAccess { get; set; }


        [Parameter]
        public IUserSessionState sessionState { get; set; }

        public bool AddNewChapterOption { get; set; }

        public List<CopyOption> CopyOptions { get; set; } = new List<CopyOption>
                                                                    {
                                                                        new CopyOption { Option = "Agenda", Selected = false },
                                                                        new CopyOption { Option = "Status", Selected = false },
                                                                        new CopyOption { Option = "Documents/Steps", Selected = false },
                                                                        new CopyOption { Option = "Fees", Selected = false },
                                                                        new CopyOption { Option = "Data Views", Selected = false },
                                                                        new CopyOption { Option = "Messages", Selected = false }
                                                                    };

        
        private void SelectExistingChapter(int chapterId)
        {
            TaskObject = AllChapters.Where(C => C.SmartflowObject.Id == chapterId).Select(C => C.SmartflowObject).SingleOrDefault();

            StateHasChanged();
        }

        private void SelectExistingCaseTypeGroup(string caseTypeGroup)
        {
            TaskObject.CaseTypeGroup = caseTypeGroup;
            TaskObject.CaseType = AllChapters
                                                        .Where(C => C.SmartflowObject.CaseTypeGroup == TaskObject.CaseTypeGroup)
                                                        .Select(C => C.SmartflowObject.CaseType)
                                                        .FirstOrDefault();
            TaskObject = AllChapters
                                                        .Where(C => C.SmartflowObject.CaseTypeGroup == TaskObject.CaseTypeGroup)
                                                        .Where(C => C.SmartflowObject.CaseType == TaskObject.CaseType)
                                                        .Select(C => C.SmartflowObject)
                                                        .FirstOrDefault();

            StateHasChanged();
        }

        private void SelectExistingCaseType(string caseType)
        {
            TaskObject.CaseType = caseType;
            TaskObject = AllChapters
                                                .Where(C => C.SmartflowObject.CaseTypeGroup == TaskObject.CaseTypeGroup)
                                                .Where(C => C.SmartflowObject.CaseType == TaskObject.CaseType)
                                                .Select(C => C.SmartflowObject)
                                                .FirstOrDefault();

            StateHasChanged();
        }

        private void ResetChapter()
        {
            AddNewChapterOption = !AddNewChapterOption;

            if (!AddNewChapterOption)
            {
                TaskObject = new UsrOrsfSmartflows
                {
                    CaseType = TaskObject.CaseType,
                    CaseTypeGroup = TaskObject.CaseTypeGroup,
                    SeqNo = TaskObject.SeqNo
                };
            }
        }


        private async void Close()
        {
            await ModalInstance.CloseAsync();
        }

        private async void HandleValidSubmit()
        {
            var copyToChapter = new VmChapter {
                Items = new List<GenSmartflowItem>(),
                Fees = new List<Fee>(),
                DataViews = new List<DataViews>(),
                TickerMessages = new List<TickerMessages>()
            };

            if (!(TaskObject.SmartflowData is null))
            {
                copyToChapter = JsonConvert.DeserializeObject<VmChapter>(TaskObject.SmartflowData);
            }


            if(CopyOptions.Where(C => C.Option == "Agenda").Select(C => C.Selected).FirstOrDefault())
            {
                foreach(var item in copyToChapter.Items.Where(C => C.Type == "Agenda").ToList())
                {
                    copyToChapter.Items.Remove(item);
                }

                copyToChapter.Items.AddRange(currentChapter.Items.Where(C => C.Type == "Agenda").ToList());
            }

            if (CopyOptions.Where(C => C.Option == "Status").Select(C => C.Selected).FirstOrDefault())
            {
                foreach (var item in copyToChapter.Items.Where(C => C.Type == "Status").ToList())
                {
                    copyToChapter.Items.Remove(item);
                }

                copyToChapter.Items.AddRange(currentChapter.Items.Where(C => C.Type == "Status").ToList());
            }

            if (CopyOptions.Where(C => C.Option == "Documents/Steps").Select(C => C.Selected).FirstOrDefault())
            {
                foreach (var item in copyToChapter.Items.Where(C => C.Type == "Doc").ToList())
                {
                    copyToChapter.Items.Remove(item);
                }

                copyToChapter.Items.AddRange(currentChapter.Items.Where(C => C.Type == "Doc").ToList());
            }

            if (CopyOptions.Where(C => C.Option == "Fees").Select(C => C.Selected).FirstOrDefault())
            {
                foreach (var item in copyToChapter.Fees.ToList())
                {
                    copyToChapter.Fees.Remove(item);
                }

                copyToChapter.Fees.AddRange(currentChapter.Fees.ToList());
            }

            if (CopyOptions.Where(C => C.Option == "Data Views").Select(C => C.Selected).FirstOrDefault())
            {
                foreach (var item in copyToChapter.DataViews.ToList())
                {
                    copyToChapter.DataViews.Remove(item);
                }

                copyToChapter.DataViews.AddRange(currentChapter.DataViews.ToList());
            }

            if (CopyOptions.Where(C => C.Option == "Messages").Select(C => C.Selected).FirstOrDefault())
            {
                //Temp measure until all Smartflows have been Serialised: post 22/4/2021
                if(copyToChapter.TickerMessages is null)
                {
                    copyToChapter.TickerMessages = new List<TickerMessages>();
                }

                foreach (var item in copyToChapter.TickerMessages.ToList())
                {
                    copyToChapter.TickerMessages.Remove(item);
                }

                copyToChapter.TickerMessages.AddRange(currentChapter.TickerMessages.ToList());
            }

            TaskObject.SmartflowData = JsonConvert.SerializeObject(new VmChapter
            {
                CaseTypeGroup = TaskObject.CaseTypeGroup,
                CaseType = TaskObject.CaseType,
                Name = TaskObject.SmartflowName,
                SeqNo = TaskObject.SeqNo.GetValueOrDefault(),
                P4WCaseTypeGroup = copyToChapter.P4WCaseTypeGroup,
                SelectedView = copyToChapter.SelectedView,
                SelectedStep = copyToChapter.SelectedStep,
                BackgroundColour = copyToChapter.BackgroundColour,
                BackgroundColourName = copyToChapter.BackgroundColourName,
                BackgroundImage = copyToChapter.BackgroundImage,
                BackgroundImageName = copyToChapter.BackgroundImageName,
                ShowPartnerNotes = copyToChapter.ShowPartnerNotes,
                Items = copyToChapter.Items,
                Fees = copyToChapter.Fees,
                DataViews = copyToChapter.DataViews,
                TickerMessages = copyToChapter.TickerMessages
            });

            if (TaskObject.Id == 0)
            {
                var returnObject = await chapterManagementService.Add(TaskObject);
                TaskObject.Id = returnObject.Id;
                await CompanyDbAccess.SaveSmartFlowRecord(TaskObject, sessionState);
            }
            else
            {
                await chapterManagementService.UpdateMainItem(TaskObject);
            }

            DataChanged?.Invoke();
            Close();
        }
    }
}
