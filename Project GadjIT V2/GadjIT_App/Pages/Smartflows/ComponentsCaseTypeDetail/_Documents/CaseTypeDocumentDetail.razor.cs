using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using Blazored.Modal;
using Blazored.Modal.Services;
using GadjIT_App.Services;
using GadjIT_App.Services.SessionState;
using GadjIT_ClientContext.Models.P4W;
using GadjIT_ClientContext.Models.Smartflow;
using GadjIT_ClientContext.Models.Smartflow.Client;
using GadjIT_App.Pages.Smartflows.FileHandling;
using GadjIT_App.FileManagement.FileClassObjects.FileOptions;
using GadjIT_App.FileManagement.FileProcessing.Interface;
using GadjIT_App.Shared.StaticObjects;
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

        [Inject]
        private ISmartflowFileHelper SmartflowFileHelper { get; set; }

        [Inject]
        public IFileHelper FileHelper { get; set; }



        private List<P4W_DmDocuments> LibraryDocumentsAndSteps {get; set;}

        protected List<LinkedCaseItem> LstDocs {get; set;} = new List<LinkedCaseItem>();

        private LinkedCaseItem SelectedLinkedCaseItem {get; set;}

        public List<P4W_CaseTypeGroups> P4WCaseTypeGroups;

      

        protected override async Task OnInitializedAsync()
        {
            await RefreshLibraryDocumentsAndStepsTask();

            P4WCaseTypeGroups = await PartnerAccessService.GetPartnerCaseTypeGroups();

            await RefreshDocumentList();



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
                                        //.Where(I => I.CustomItem != "Y")
                                        .ToList();
                        
                        foreach(var doc in docs)
                        {
                            if(doc.CustomItem != "Y")
                            {
                                LstDocs.Add(new LinkedCaseItem{
                                                ItemName=doc.Name
                                                ,AltName=doc.AltDisplayName
                                                ,IsAttachment = false
                                                ,OrigItemName=doc.Name
                                                ,OrigSeqNo = doc.SeqNo
                                                ,IsItemLinked=CheckIsItemLinked(doc.Name)
                                                ,SmartflowName=smartflow.Name
                                                ,P4WCaseTypeGroup = string.IsNullOrEmpty(smartflow.P4WCaseTypeGroup)
                                                                    ? -2
                                                                    : smartflow.P4WCaseTypeGroup == "Global Documents"
                                                                    ? 0
                                                                    : smartflow.P4WCaseTypeGroup == "Entity Documents"
                                                                    ? -1
                                                                    : P4WCaseTypeGroups == null ? -2 : P4WCaseTypeGroups
                                                                        .Where(P => P.Name == smartflow.P4WCaseTypeGroup)
                                                                        .Select(P => P.Id)
                                                                        .FirstOrDefault()
                                                    });
                            }
                            if(doc.LinkedItems != null && doc.LinkedItems.Count() > 0)
                            {
                                List<LinkedItem> attachments = doc.LinkedItems
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


        protected void DownloadDocumentPermissionsSQL()
        {
            try
            {
                string sqlTemplate = "INSERT INTO Dm_DocumentsPermissions (doccode, casetype) "
                                + " SELECT Code, @CaseTypeCode "
                                + " FROM Dm_Documents "
                                + " WHERE CaseTypeGroupRef = @CaseTypeGroupCode "
                                + " AND Name = '#DocName#' "
                                + " AND Code NOT IN "
                                + " ( "
                                + " SELECT doccode FROM DM_DocumentsPermissions WHERE CaseType = @CaseTypeCode "
                                + " )";

                string sqlCommand = "/*"
                                + "Replace the following before running in:"
                                + Environment.NewLine     
                                + "*/"
                                + Environment.NewLine     
                                + Environment.NewLine + " DECLARE @CaseTypeGroupCode int = ??     -- (SELECT * FROM CaseTypeGroups ORDER BY Name)"
                                + Environment.NewLine + " DECLARE @CaseTypeCode int = ??          -- (SELECT * FROM CaseTypes ORDER BY Description)"
                                + Environment.NewLine     
                                + Environment.NewLine     
                                ;
                string sqlCommandAll = sqlCommand;

                foreach(LinkedCaseItem doc in LstDocs)
                {
                    sqlCommand = sqlTemplate.Replace("#DocName#",doc.ItemName);
                    sqlCommand = sqlCommand.Replace(@"\n","");
                    sqlCommand = sqlCommand.Replace(@"\r","");
                    sqlCommandAll += sqlCommand + Environment.NewLine;
                }
                
                byte[] fileData = Encoding.ASCII.GetBytes(sqlCommandAll);

                var fileName = "DocumentPermissions_" + _SelectedCaseType + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".txt";

                FileOptions chapterFileOption;

                chapterFileOption = new FileOptions
                {
                    Company = UserSession.Company.CompanyName,
                    SelectedSystem = UserSession.SelectedSystem,
                    CaseTypeGroup = _SelectedCaseTypeGroup,
                    CaseType = _SelectedCaseType,
                };

                SmartflowFileHelper.SetFileHelperCustomPath(chapterFileOption,FileStorageType.TempUploads);

                FileHelper.DownloadFile(fileName, fileData);
            }
            catch (Exception e)
            {
                GenericErrorLog(true,e, "DownloadDocumentPermissionsSQL", $"Downloading Document Permissions SQL command: {e.Message}");
            }
        }


        protected void DownloadDocumentRenameCommands()
        {
            try
            {
                //UPDATE dm_Documents SET Location = 'F:\Partner\RESI_V6.4_DEV\Templates\OR Resi\ZTest.4_DEV\Templates\OR Resi\2412.docx' WHERE CODE = 2412
                string docFileLocation = "";
                string docFileName = "";
                string fileExtention = "";
                string dosCommand = "";
                string dosCommands = "";
                string docErrors = "";

                string sqlCommands = "";

                P4W_DmDocuments p4WDoc;
                foreach(LinkedCaseItem doc in LstDocs)
                {
                    if(doc.P4WCaseTypeGroup > 0) //not Global or Entity doc
                    {
                        p4WDoc = LibraryDocumentsAndSteps.Where(d => d.CaseTypeGroupRef == doc.P4WCaseTypeGroup)
                                                            .Where(d => d.Name == doc.ItemName)
                                                            .Where(d => d.DocumentType == 1) //Word Docs
                                                            .FirstOrDefault();

                        if(p4WDoc != null) 
                        {
                            try
                            {   
                                if(string.IsNullOrEmpty(p4WDoc.Location))
                                    throw new Exception("Location is empty");
                                
                                docFileName = System.IO.Path.GetFileName(p4WDoc.Location);
                                if(docFileName == "")
                                    throw new Exception("Cannot establish file name");

                                docFileLocation =  System.IO.Path.GetDirectoryName(p4WDoc.Location);

                                docFileName = System.IO.Path.GetFileName(p4WDoc.Location);

                                fileExtention = System.IO.Path.GetExtension(docFileName);
                                if(fileExtention == "")
                                    throw new Exception("Cannot establish file extension");

                                if(docFileName != (doc.ItemName + fileExtention)) //only create update commands if the document file name does not match the P4W item name
                                {
                                    dosCommand = "REN \"" + docFileName + "\"" + " \"" + doc.ItemName + fileExtention + "\"";
                                    dosCommands += dosCommand + Environment.NewLine;

                                    sqlCommands += "UPDATE dm_Documents SET Location = '" + docFileLocation + "\\" + doc.ItemName + fileExtention + "' WHERE CODE = " + p4WDoc.Code.ToString() + Environment.NewLine;
                                }
                                
                            }
                            catch(Exception e)
                            {
                                docErrors += doc.ItemName + " - " + e.Message + Environment.NewLine;
                                dosCommand = "";
                            }
                        }

                    }
                }
                
                
                FileOptions chapterFileOption;

                chapterFileOption = new FileOptions
                {
                    Company = UserSession.Company.CompanyName,
                    SelectedSystem = UserSession.SelectedSystem,
                    CaseTypeGroup = _SelectedCaseTypeGroup,
                    CaseType = _SelectedCaseType,
                };

                var curTime = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                SmartflowFileHelper.SetFileHelperCustomPath(chapterFileOption,FileStorageType.TempUploads);

                if(dosCommands == "")
                {
                    dosCommands = "No documents to update. All documents used in the Smartflows within this Case Type are named correctly.";
                    sqlCommands = dosCommands;
                }
                //DOS commands to rename the actual Word document
                byte[] fileDataCmd = Encoding.ASCII.GetBytes(dosCommands);

                var fileName = "CMD_DocumentNameSync_" + _SelectedCaseType + "_" + curTime + ".txt";

                FileHelper.DownloadFile(fileName, fileDataCmd);


                //SQL commands to update the Location field within the DM_Documents table
                byte[] fileDataSql = Encoding.ASCII.GetBytes(sqlCommands);

                fileName = "SQL_DocumentNameSync_" + _SelectedCaseType + "_" + curTime + ".sql";

                FileHelper.DownloadFile(fileName, fileDataSql);


                //if any errors occured
                if(docErrors != "")
                {
                    byte[] fileDataErrors = Encoding.ASCII.GetBytes(docErrors);

                    fileName = "Issues_DocumentNameSync_" + _SelectedCaseType + "_" + curTime + ".txt";

                    FileHelper.DownloadFile(fileName, fileDataErrors);
                }

            }
            catch (Exception e)
            {
                GenericErrorLog(true,e, "DownloadDocumentRenameCommands", $"Downloading Document Rename Commands: {e.Message}");
            }
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