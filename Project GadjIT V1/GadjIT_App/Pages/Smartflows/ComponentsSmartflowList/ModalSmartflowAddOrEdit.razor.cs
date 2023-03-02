using Blazored.Modal;
using GadjIT_App.Data.Admin;
using GadjIT_App.Services;
using GadjIT_App.Services.SessionState;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using GadjIT_ClientContext.Models.Smartflow;
using GadjIT_ClientContext.Models.Smartflow.Client;

namespace GadjIT_App.Pages.Smartflows.ComponentsSmartflowList
{
    public partial class ModalSmartflowAddOrEdit
    {
        [CascadingParameter]
        BlazoredModalInstance ModalInstance { get; set; }

        [Inject]
        IClientApiManagementService ClientApiManagementService { get; set; }

        [Parameter]
        public Client_SmartflowRecord TaskObject { get; set; }

        [Parameter]
        public Action DataChanged { get; set; }

        [Parameter]
        public List<Client_VmSmartflowRecord> AllObjects { get; set; }

        [Parameter]
        public bool addNewCaseTypeGroupOption { get; set; } = false;

        [Parameter]
        public bool addNewCaseTypeOption { get; set; } = false;

        [Parameter]
        public IUserSessionState UserSession { get; set; }

        [Parameter]
        public ICompanyDbAccess CompanyDbAccess { get; set; }


        public int Error { get; set; } = 0;


        private async void Close()
        {
            await ModalInstance.CloseAsync();


        }

        private async void CreateSmartFlow()
        {
            if (TaskObject.Id == 0)
            {

                var name = Regex.Replace(TaskObject.SmartflowName, "[^0-9a-zA-Z-_ (){}!£$%^&*,.#?@<>`: ]+", "");
                var caseType = Regex.Replace(TaskObject.CaseType, "[^0-9a-zA-Z-_ (){}!£$%^&*,.#?@<>`: ]+", "");
                var caseTypeGroup = Regex.Replace(TaskObject.CaseTypeGroup, "[^0-9a-zA-Z-_ (){}!£$%^&*,.#?@<>`: ]+", "");

                TaskObject.SmartflowName = name;
                TaskObject.CaseType = caseType;
                TaskObject.CaseTypeGroup = caseTypeGroup;
                TaskObject.SmartflowData = JsonConvert.SerializeObject(new Smartflow
                {
                    CaseTypeGroup = caseTypeGroup,
                    CaseType = caseType,
                    Name = name,
                    SeqNo = TaskObject.SeqNo.GetValueOrDefault(),
                    StepName = "",
                    ShowPartnerNotes = "N",
                    Items = new List<GenSmartflowItem>(),
                    Fees = new List<SmartflowFee>(),
                    DataViews = new List<SmartflowDataView>(),
                    TickerMessages = new List<SmartflowMessage>()
                });


                var returnObject = await ClientApiManagementService.Add(TaskObject);
                TaskObject.Id = returnObject.Id;
                await CompanyDbAccess.SaveSmartFlowRecord(TaskObject, UserSession);
            }
            else
            {
                await ClientApiManagementService.UpdateMainItem(TaskObject);
            }

            DataChanged?.Invoke();
            Close();
        }


        private void HandleValidSubmit()
        {
            if (AllObjects
                .Where(A => A.ClientSmartflowRecord.CaseTypeGroup == TaskObject.CaseTypeGroup)
                .Select(A => A.ClientSmartflowRecord.SmartflowName)
                .Contains(TaskObject.SmartflowName))
            {
                Error = 1; //Smartflow with same name already exists
                StateHasChanged();
            }
            else
            {

                CreateSmartFlow();
            }

        }

        private async void HandleValidDelete()
        {
            await ClientApiManagementService.DeleteChapter(TaskObject.Id);

            DataChanged?.Invoke();
            Close();
        }

        private void ResetError()
        {
            Error = 0;
            StateHasChanged();
        }
    }
}
