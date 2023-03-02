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
using System.Threading.Tasks;

namespace GadjIT_App.Pages.Smartflows.ComponentsSmartflowList
{
    public partial class ModalSmartflowCaseTypeEdit
    {
        [CascadingParameter]
        BlazoredModalInstance ModalInstance { get; set; }

        [Inject]
        IClientApiManagementService ClientApiManagementService { get; set; }

        [Parameter]
        public List<Client_VmSmartflowRecord> ListSmartflows { get; set; }

        [Parameter]
        public string TaskObject { get; set; }

        [Parameter]
        public Client_SmartflowRecord Smartflow { get; set; }

        [Parameter]
        public string originalName { get; set; }

        [Parameter]
        public string caseTypeGroupName { get; set; }

        [Parameter]
        public string isCaseTypeOrGroup { get; set; }

        [Parameter]
        public Action DataChanged { get; set; }

        [Parameter]
        public ICompanyDbAccess CompanyDbAccess { get; set; }

        [Parameter]
        public IUserSessionState UserSession { get; set; }


        private async void Close()
        {
            await ModalInstance.CloseAsync();
        }

        private async Task HandleValidSubmit()
        {
            if (isCaseTypeOrGroup == "CaseType")
            {
                var chapters = ListSmartflows.Where(C => C.ClientSmartflowRecord.CaseType == originalName).ToList();

                foreach (var chapter in chapters)
                {
                    var updateJson = JsonConvert.DeserializeObject<Smartflow>(chapter.ClientSmartflowRecord.SmartflowData);
                    updateJson.CaseType = TaskObject;
                    chapter.ClientSmartflowRecord.CaseType = TaskObject;
                    chapter.ClientSmartflowRecord.SmartflowData = JsonConvert.SerializeObject(updateJson);
                    await ClientApiManagementService.UpdateMainItem(chapter.ClientSmartflowRecord).ConfigureAwait(false);
                }
            }
            else if (isCaseTypeOrGroup == "CaseTypeGroup")
            {
                var chapters = ListSmartflows.Where(C => C.ClientSmartflowRecord.CaseTypeGroup == originalName).ToList();

                foreach (var chapter in chapters)
                {
                    var updateJson = JsonConvert.DeserializeObject<Smartflow>(chapter.ClientSmartflowRecord.SmartflowData);
                    updateJson.CaseTypeGroup = TaskObject;
                    chapter.ClientSmartflowRecord.CaseTypeGroup = TaskObject;
                    chapter.ClientSmartflowRecord.SmartflowData = JsonConvert.SerializeObject(updateJson);
                    await ClientApiManagementService.UpdateMainItem(chapter.ClientSmartflowRecord).ConfigureAwait(false);
                }
            }
            else
            {
                var updateJson = JsonConvert.DeserializeObject<Smartflow>(Smartflow.SmartflowData);
                updateJson.Name = Smartflow.SmartflowName;
                Smartflow.SmartflowData = JsonConvert.SerializeObject(updateJson);
                await ClientApiManagementService.UpdateMainItem(Smartflow).ConfigureAwait(false);
            }

            DataChanged?.Invoke();
            Close();
        }

  

    }
}
