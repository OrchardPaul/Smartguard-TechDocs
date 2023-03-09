using Blazored.Modal;
using GadjIT_App.Data.Admin;
using GadjIT_App.Services;
using GadjIT_App.Services.SessionState;
using GadjIT_ClientContext.Models.Smartflow;
using GadjIT_ClientContext.Models.Smartflow.Client;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GadjIT_App.Pages.Smartflows.ComponentsCaseTypeDetail._Header
{
    public partial class ModalSmartflowEdit
    {
        [CascadingParameter]
        BlazoredModalInstance ModalInstance { get; set; }

        [Inject]
        IClientApiManagementService ClientApiManagementService { get; set; }

        [Inject]
        public IUserSessionState UserSession { get; set; }

        [Inject]
        private ICompanyDbAccess CompanyDbAccess { get; set; }

    
        [Parameter]
        public string _TaskObject { get; set; }

        [Parameter]
        public Client_SmartflowRecord _Smartflow { get; set; }

        [Parameter]
        public List<Client_VmSmartflowRecord> _LstVmClientSmartflowRecord { get; set; }

        [Parameter]
        public Action _DataChanged { get; set; }

        [Parameter]
        public string _TaskType {get; set; }


        private async void Close()
        {
            await ModalInstance.CloseAsync();
        }

        private async Task HandleValidSubmit()
        {
            if(_TaskType == "Edit")
            {
                await EditSmartflowDetails();
            }
            else //Add
            {
                await CreateNewSmartflow();
            }
           
        }

        private async Task CreateNewSmartflow()
        {
            var name = Regex.Replace(_Smartflow.SmartflowName, "[^0-9a-zA-Z-_ (){}!£$%^&*,.#?@<>`: ]+", "");
            var caseType = Regex.Replace(_Smartflow.CaseType, "[^0-9a-zA-Z-_ (){}!£$%^&*,.#?@<>`: ]+", "");
            var caseTypeGroup = Regex.Replace(_Smartflow.CaseTypeGroup, "[^0-9a-zA-Z-_ (){}!£$%^&*,.#?@<>`: ]+", "");

            _Smartflow.SmartflowName = name;
            _Smartflow.CaseType = caseType;
            _Smartflow.CaseTypeGroup = caseTypeGroup;
            _Smartflow.SmartflowData = JsonConvert.SerializeObject(new SmartflowV2
            {
                CaseTypeGroup = caseTypeGroup,
                CaseType = caseType,
                Name = name,
                SeqNo = _Smartflow.SeqNo.GetValueOrDefault(),
                StepName = "",
                ShowPartnerNotes = "N",
                Agendas = new List<SmartflowAgenda>(),
                Status = new List<SmartflowStatus>(),
                Documents = new List<SmartflowDocument>(),
                Fees = new List<SmartflowFee>(),
                DataViews = new List<SmartflowDataView>(),
                Messages = new List<SmartflowMessage>()
            });


            var returnObject = await ClientApiManagementService.Add(_Smartflow);
            _Smartflow.Id = returnObject.Id;
            await CompanyDbAccess.SaveSmartFlowRecord(_Smartflow, UserSession);

            Client_VmSmartflowRecord vmSmartflowRecord = new Client_VmSmartflowRecord{ ClientSmartflowRecord = _Smartflow };
            vmSmartflowRecord.SetSmartflowStatistics();

            _LstVmClientSmartflowRecord.Add(vmSmartflowRecord);

            _DataChanged?.Invoke();
            Close();
        }

        private async Task EditSmartflowDetails()
        {
            
            var updateJson = JsonConvert.DeserializeObject<SmartflowV2>(_Smartflow.SmartflowData);
            updateJson.Name = _Smartflow.SmartflowName;
            _Smartflow.SmartflowData = JsonConvert.SerializeObject(updateJson);
            await ClientApiManagementService.UpdateMainItem(_Smartflow).ConfigureAwait(false);
        
            _DataChanged?.Invoke();
            Close();
        }

  

    }
}
