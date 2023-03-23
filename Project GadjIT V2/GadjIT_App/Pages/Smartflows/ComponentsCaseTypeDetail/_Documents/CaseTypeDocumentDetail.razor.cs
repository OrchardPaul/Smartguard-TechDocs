using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazored.Modal;
using Blazored.Modal.Services;
using GadjIT_App.Services;
using GadjIT_App.Services.SessionState;
using GadjIT_ClientContext.Models.P4W;
using GadjIT_ClientContext.Models.Smartflow;
using GadjIT_ClientContext.Models.Smartflow.Client;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog.Context;

namespace GadjIT_App.Pages.Smartflows.ComponentsCaseTypeDetail._Documents
{
    


    public partial class CaseTypeDocumentDetail
    {

        [Parameter]
        public List<Client_VmSmartflowRecord> _LstVmClientSmartflowRecord { get; set; }

        [Parameter]
        public string _SelectedCaseTypeGroup { get; set; }

        [Parameter]
        public string _SelectedCaseType { get; set; }   


        [Inject]
        private ILogger<CaseTypeDocumentDetail> Logger { get; set; }

        [Inject]
        public INotificationManager NotificationManager {get; set;}

        [Inject]
        private IClientApiManagementService ClientApiManagementService { get; set; }


        [Inject]
        public IUserSessionState UserSession { get; set; }

        [Inject]
        private IPartnerAccessService PartnerAccessService { get; set; }

        [Inject]
        IModalService Modal { get; set; }

        private List<P4W_DmDocuments> LibraryDocumentsAndSteps {get; set;}

        protected List<LinkedCaseItem> LstDocs {get; set;} = new List<LinkedCaseItem>();

        private LinkedCaseItem SelectedLinkedCaseItem {get; set;}

        public List<P4W_CaseTypeGroups> P4WCaseTypeGroups;

      

        protected override async Task OnInitializedAsync()
        {
            await RefreshLibraryDocumentsAndStepsTask();

            await RefreshDocumentList();


            P4WCaseTypeGroups = await PartnerAccessService.GetPartnerCaseTypeGroups();

        }
        
        private async Task RefreshDocumentList()
        {
            try
            {
                LstDocs = new List<LinkedCaseItem>();

                List<Client_VmSmartflowRecord> lstCaseTypeSmartflows = _LstVmClientSmartflowRecord
                                                            .Where(C => C.ClientSmartflowRecord.CaseTypeGroup == _SelectedCaseTypeGroup)
                                                            .Where(C => C.ClientSmartflowRecord.CaseType == _SelectedCaseType)
                                                            .OrderBy(C => C.ClientSmartflowRecord.SeqNo)
                                                            .ToList();
                                                            

                if(lstCaseTypeSmartflows != null && lstCaseTypeSmartflows.Count > 0)
                {
                    VmSmartflows vmSmartflows = new VmSmartflows();

                    foreach(Client_VmSmartflowRecord Client_VmSmartflowRecord in lstCaseTypeSmartflows)
                    {
                        SmartflowV2 smartflow = JsonConvert.DeserializeObject<SmartflowV2>(Client_VmSmartflowRecord.ClientSmartflowRecord.SmartflowData);
                        smartflow.CaseTypeGroup = Client_VmSmartflowRecord.ClientSmartflowRecord.CaseTypeGroup;
                        smartflow.CaseType = Client_VmSmartflowRecord.ClientSmartflowRecord.CaseType;
                        //vmSmartflows.Smartflows.Add(smartflow);
                        //List<GenSmartflowItem> docs = smartflow.Items.

                        Dictionary<int?, string> docTypes = new Dictionary<int?, string> { { 1, "Doc" }, { 4, "Form" }, { 6, "Step" }, { 8, "Date" }, { 9, "Email" }, { 11, "Doc" }, { 12, "Email" }, { 13, "Csv" } };
                        List<SmartflowDocument> docs = smartflow.Documents
                                        .Where(I => I.CustomItem != "Y")
                                        .ToList();
                        
                        foreach(var doc in docs)
                        {
                            LstDocs.Add(new LinkedCaseItem{
                                                ItemName=doc.Name
                                                ,AltName=doc.AltDisplayName
                                                ,IsAttachment = false
                                                ,OrigItemName=doc.Name
                                                ,OrigSeqNo = doc.SeqNo
                                                ,IsItemLinked=CheckIsItemLinked(doc.Name)
                                                ,SmartflowName=smartflow.Name
                                                    });

                            if(doc.LinkedItems != null && doc.LinkedItems.Count() > 0)
                            {
                                List<LinkedDocument> attachments = doc.LinkedItems
                                            .Where(LI => LI.CustomItem != "Y")
                                            .ToList();

                                foreach(var attach in attachments)
                                {
                                    LstDocs.Add(new LinkedCaseItem{
                                            ItemName=attach.DocName
                                            ,AltName=attach.DocAsName
                                            ,IsAttachment = true
                                            ,OrigItemName=attach.DocName
                                            ,OrigSeqNo = doc.SeqNo
                                            ,IsItemLinked=CheckIsItemLinked(attach.DocName)
                                            ,SmartflowName=smartflow.Name
                                                });
                                }

                            }
                        }
                                        
                        
                    }

                    // Order the list
                    LstDocs = LstDocs
                            .OrderBy(D => D.IsAttachment ? 1 : 0)
                            .OrderBy(D => D.SmartflowName)
                            .ToList();

                    
                }

                await InvokeAsync(StateHasChanged);

            }
            catch (Exception ex)
            {
                GenericErrorLog(true,ex, "RefreshLibraryDocumentsAndStepsTask", $"Refreshing the document list: {ex.Message}");

            }
        }
        
        private bool CheckIsItemLinked(string docName)
        {
            int numMatches = 0;
            numMatches = LibraryDocumentsAndSteps
                                                .Where(D => !(D.Name is null))
                                                .Where(D => D.Name == docName)
                                                .Count();
            
            return numMatches > 0 ? true : false;

        }

        private async Task RefreshLibraryDocumentsAndStepsTask()
        {
            try
            {
                LibraryDocumentsAndSteps = await ClientApiManagementService.GetDocumentList(_SelectedCaseType);
                LibraryDocumentsAndSteps = LibraryDocumentsAndSteps
                                                                .Where(D => !(D.Name is null))
                                                                .OrderBy(D => D.Name)
                                                                .ToList();
                //StateHasChanged();
            }
            catch (Exception ex)
            {
                GenericErrorLog(true,ex, "RefreshLibraryDocumentsAndStepsTask", $"Refreshing the document list: {ex.Message}");

            }
        }

        protected void ShowLinkDocumentModal(LinkedCaseItem _selectedLinkedCaseItem) 
        {
            try
            {
                SelectedLinkedCaseItem = _selectedLinkedCaseItem;

                Action handleUpdate = HandleUpdate;

                Client_VmSmartflowRecord selectedVmUsrClientSmartflowRecord = _LstVmClientSmartflowRecord
                                        .Where(C => C.ClientSmartflowRecord.CaseTypeGroup == _SelectedCaseTypeGroup)
                                        .Where(C => C.ClientSmartflowRecord.CaseType == _SelectedCaseType)
                                        .Where(C => C.ClientSmartflowRecord.SmartflowName == _selectedLinkedCaseItem.SmartflowName)
                                        .FirstOrDefault();

                SmartflowV2 selectedSmartflow = JsonConvert.DeserializeObject<SmartflowV2>(selectedVmUsrClientSmartflowRecord.ClientSmartflowRecord.SmartflowData);
                
                LinkedCaseItem copyLinkedCaseItem = new LinkedCaseItem{
                                                    ItemName = _selectedLinkedCaseItem.ItemName
                                                    ,AltName = _selectedLinkedCaseItem.AltName
                                                    ,IsAttachment = _selectedLinkedCaseItem.IsAttachment
                                                    ,IsItemLinked = _selectedLinkedCaseItem.IsItemLinked
                                                    ,SmartflowName = _selectedLinkedCaseItem.SmartflowName
                                                };

                var parameters = new ModalParameters();
                parameters.Add("_SelectedSmartflow", selectedSmartflow);
                parameters.Add("_Selected_ClientSmartflowRecord", selectedVmUsrClientSmartflowRecord.ClientSmartflowRecord);
                parameters.Add("_TaskObject", SelectedLinkedCaseItem);
                parameters.Add("_CopyObject", copyLinkedCaseItem);
                parameters.Add("_DataChanged", handleUpdate);
                parameters.Add("_LibraryDocumentsAndSteps", LibraryDocumentsAndSteps);
                parameters.Add("_P4WCaseTypeGroups", P4WCaseTypeGroups);
                
                var options = new ModalOptions()
                {
                    Class = "blazored-custom-modal modal-smartflow-doc" 
                };

                Modal.Show<ModalLinkDocument>("Document Linking", parameters, options);
            }
            catch(Exception e)
            {
                GenericErrorLog(false,e, "ShowLinkDocumentModal", e.Message);
            }

        }

        private async void HandleUpdate()
        {
            await InvokeAsync(StateHasChanged);
        }



        /****************************************/
        /* ERROR HANDLING */
        /****************************************/
        private void GenericErrorLog(bool _showNotificationMsg, Exception e, string _method, string _message)
        {
            using (LogContext.PushProperty("SourceSystem", UserSession.SelectedSystem))
            using (LogContext.PushProperty("SourceCompanyId", UserSession.Company.Id))
            using (LogContext.PushProperty("SourceUserId", UserSession.User.Id))
            using (LogContext.PushProperty("SourceContext", nameof(CaseTypeDocumentDetail)))
            {
                Logger.LogError(e,"Error - Method: {0}, Message: {1}",_method, _message);
            }

            if(_showNotificationMsg)
            {
                NotificationManager.ShowNotification("Danger", $"Oops! Something went wrong.");
            }
        }

        
    }
}