using Blazored.Modal;
using GadjIT_App.Data.Admin;
using GadjIT_App.Services;
using GadjIT_App.Services.SessionState;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GadjIT_ClientContext.Models.Smartflow;
using GadjIT_ClientContext.Models.Smartflow.Client;

namespace GadjIT_App.Pages.Smartflows.ComponentsSmartflowList
{
    public partial class ModalSmartflowCopy
    {
        public class CopyOption
        {
            public string Option { get; set; }
            public bool Selected { get; set; }
        }

        [CascadingParameter]
        BlazoredModalInstance ModalInstance { get; set; }

        [Inject]
        IClientApiManagementService ClientApiManagementService { get; set; }

        [Parameter]
        public Client_SmartflowRecord TaskObject { get; set; }

        [Parameter]
        public Action DataChanged { get; set; }

        [Parameter]
        public List<Client_VmSmartflowRecord> AllChapters { get; set; }

        [Parameter]
        public Smartflow currentChapter { get; set; }

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
            TaskObject = AllChapters.Where(C => C.ClientSmartflowRecord.Id == chapterId).Select(C => C.ClientSmartflowRecord).SingleOrDefault();

            StateHasChanged();
        }

        private void SelectExistingCaseTypeGroup(string caseTypeGroup)
        {
            TaskObject.CaseTypeGroup = caseTypeGroup;
            TaskObject.CaseType = AllChapters
                                                        .Where(C => C.ClientSmartflowRecord.CaseTypeGroup == TaskObject.CaseTypeGroup)
                                                        .Select(C => C.ClientSmartflowRecord.CaseType)
                                                        .FirstOrDefault();
            TaskObject = AllChapters
                                                        .Where(C => C.ClientSmartflowRecord.CaseTypeGroup == TaskObject.CaseTypeGroup)
                                                        .Where(C => C.ClientSmartflowRecord.CaseType == TaskObject.CaseType)
                                                        .Select(C => C.ClientSmartflowRecord)
                                                        .FirstOrDefault();

            StateHasChanged();
        }

        private void SelectExistingCaseType(string caseType)
        {
            TaskObject.CaseType = caseType;
            TaskObject = AllChapters
                                                .Where(C => C.ClientSmartflowRecord.CaseTypeGroup == TaskObject.CaseTypeGroup)
                                                .Where(C => C.ClientSmartflowRecord.CaseType == TaskObject.CaseType)
                                                .Select(C => C.ClientSmartflowRecord)
                                                .FirstOrDefault();

            StateHasChanged();
        }

        private void ResetChapter()
        {
            AddNewChapterOption = !AddNewChapterOption;

            if (!AddNewChapterOption)
            {
                TaskObject = new Client_SmartflowRecord
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

        private async Task HandleValidSubmit()
        {
            var copyToChapter = new Smartflow {
                Items = new List<GenSmartflowItem>(),
                Fees = new List<SmartflowFee>(),
                DataViews = new List<SmartflowDataView>(),
                TickerMessages = new List<SmartflowMessage>()
            };

            if (!(TaskObject.SmartflowData is null))
            {
                copyToChapter = JsonConvert.DeserializeObject<Smartflow>(TaskObject.SmartflowData);
            }


            if(CopyOptions.Where(C => C.Option == "Agenda").Select(C => C.Selected).FirstOrDefault())
            {
                if (copyToChapter.Items is null)
                {
                    copyToChapter.Items = new List<GenSmartflowItem>();
                }

                if (currentChapter.Items is null)
                {
                    currentChapter.Items = new List<GenSmartflowItem>();
                }

                foreach (var item in copyToChapter.Items.Where(C => C.Type == "Agenda").ToList())
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
                if (copyToChapter.Fees is null)
                {
                    copyToChapter.Fees = new List<SmartflowFee>();
                }

                if (currentChapter.Fees is null)
                {
                    currentChapter.Fees = new List<SmartflowFee>();
                }

                foreach (var item in copyToChapter.Fees.ToList())
                {
                    copyToChapter.Fees.Remove(item);
                }

                copyToChapter.Fees.AddRange(currentChapter.Fees.ToList());
            }

            if (CopyOptions.Where(C => C.Option == "Data Views").Select(C => C.Selected).FirstOrDefault())
            {
                if(copyToChapter.DataViews is null)
                {
                    copyToChapter.DataViews = new List<SmartflowDataView>();
                }

                if (currentChapter.DataViews is null)
                {
                    currentChapter.DataViews = new List<SmartflowDataView>();
                }

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
                    copyToChapter.TickerMessages = new List<SmartflowMessage>();
                }


                if (currentChapter.TickerMessages is null)
                {
                    currentChapter.TickerMessages = new List<SmartflowMessage>();
                }

                foreach (var item in copyToChapter.TickerMessages.ToList())
                {
                    copyToChapter.TickerMessages.Remove(item);
                }

                copyToChapter.TickerMessages.AddRange(currentChapter.TickerMessages.ToList());
            }

            TaskObject.SmartflowData = JsonConvert.SerializeObject(new Smartflow
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
                var returnObject = await ClientApiManagementService.Add(TaskObject);
                TaskObject.Id = returnObject.Id;
                await CompanyDbAccess.SaveSmartFlowRecord(TaskObject, sessionState);
            }
            else
            {
                await ClientApiManagementService.UpdateMainItem(TaskObject);
            }

            DataChanged?.Invoke();
            Close();
        }
    }
}
