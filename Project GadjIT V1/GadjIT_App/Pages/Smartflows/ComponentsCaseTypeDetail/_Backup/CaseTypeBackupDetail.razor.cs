using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazored.Modal;
using Blazored.Modal.Services;
using GadjIT_App.Pages._Shared.Modals;
using GadjIT_App.Services;
using GadjIT_App.Services.AppState;
using GadjIT_App.Services.SessionState;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog.Context;
using BlazorInputFile;
using GadjIT_App.FileManagement.FileProcessing.Interface;
using GadjIT_App.FileManagement.FileClassObjects;
using GadjIT_App.Pages.Smartflows.FileHandling;
using GadjIT_App.FileManagement.FileClassObjects.FileOptions;
using Microsoft.JSInterop;
using GadjIT_App.Shared.StaticObjects;
using GadjIT_App.Data.Admin;
using GadjIT_ClientContext.Models.Smartflow;
using GadjIT_ClientContext.Models.Smartflow.Client;

namespace GadjIT_App.Pages.Smartflows.ComponentsCaseTypeDetail._Backup
{
    public partial class CaseTypeBackupDetail
    {
        

        [Parameter]
        public List<Client_VmSmartflowRecord> _LstVmClientSmartflowRecord { get; set; }

   
        [Parameter]
        public string _SelectedCaseTypeGroup { get; set; }

        [Parameter]
        public string _SelectedCaseType { get; set; }

        [Parameter]
        public EventCallback _RefreshSmartflowsTask {get; set;}


        [Inject]
        IModalService Modal { get; set; }

        [Inject]
        private IJSRuntime JSRuntime {get; set;}
        
         [Inject]
        private ICompanyDbAccess CompanyDbAccess { get; set; }

        [Inject]
        public IUserSessionState UserSession { get; set; }

        [Inject]
        public INotificationManager NotificationManager {get; set;}

        [Inject]
        private ILogger<CaseTypeBackupDetail> Logger { get; set; }

        [Inject]
        private IClientApiManagementService ClientApiManagementService { get; set; }

        [Inject]
        public IAppSmartflowsState AppSmartflowsState { get; set; }

        [Inject]
        private ISmartflowFileHelper SmartflowFileHelper { get; set; }

        [Inject]
        public IFileHelper FileHelper { get; set; }

        private FileDesc SelectedFileDescription { get; set; }


        private List<FileDesc> ListFilesForBackups { get; set; }

        public IList<string> JsonErrors { get; set; }

        private string SelectedFilePath = "";



        protected override async Task OnInitializedAsync()
        {
            SetSmartflowFilePath();

            GetSeletedFileList();
        }

        
        public async Task WriteCaseTypeJSONToFile()
        {
            try
            {
                List<Client_VmSmartflowRecord> lstCaseTypeSmartflows = _LstVmClientSmartflowRecord
                                                            .Where(C => C.ClientSmartflowRecord.CaseTypeGroup == _SelectedCaseTypeGroup)
                                                            .Where(C => C.ClientSmartflowRecord.CaseType == _SelectedCaseType)
                                                            .OrderBy(C => C.ClientSmartflowRecord.SeqNo)
                                                            .ToList();
                                                            

                if(lstCaseTypeSmartflows != null && lstCaseTypeSmartflows.Count > 0)
                {
                    VmSmartflows vmSmartflows = new VmSmartflows();
                    
                    foreach(var vmSmartflow in lstCaseTypeSmartflows)
                    {
                        Smartflow smartflow = JsonConvert.DeserializeObject<Smartflow>(vmSmartflow.ClientSmartflowRecord.SmartflowData);
                        vmSmartflows.Smartflows.Add(smartflow);

                    }

                    vmSmartflows.Smartflows.Select(S => { S.SeqNo = vmSmartflows.Smartflows.IndexOf(S) + 1; return S; }).ToList();

                    string caseTypeData = JsonConvert.SerializeObject(vmSmartflows);

                    var fileName = _SelectedCaseType + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".txt";

                    SmartflowFileHelper.WriteSmartflowToFile(caseTypeData, fileName); //CustomPath set in OnInitialized

                    await NotificationManager.ShowNotification("Success", $"Backup complete.");

                    GetSeletedFileList();

                    
                }
                
            }
            catch(Exception e)
            {
                await GenericErrorLog(false,e, "WriteCaseTypeJSONToFile", e.Message);
            }
            
        }

        
        private void SetSmartflowFilePath()
        {
            try
            {
                FileOptions chapterFileOption;

                chapterFileOption = new FileOptions
                {
                    Company = UserSession.Company.CompanyName,
                    SelectedSystem = UserSession.SelectedSystem,
                    CaseTypeGroup = _SelectedCaseTypeGroup,
                    CaseType = _SelectedCaseType,
                    SmartflowName = null
                };

                SmartflowFileHelper.SetFileHelperCustomPath(chapterFileOption,FileStorageType.BackupsCaseType);
            }
            catch (Exception ex)
            {
                GenericErrorLog(true,ex, "SetSmartflowFilePath", $"Setting Smartflow file path: {ex.Message}").ConfigureAwait(false);
            }
        }

        private void GetSeletedFileList()
        {
            ListFilesForBackups = SmartflowFileHelper.GetFileListForSmartflow(); //CustomPath set in OnInitialized

            StateHasChanged();

        }

        protected async Task PrepareBackUpForRestore(string _filePath)
        {
            try
            {
                SelectedFilePath = _filePath;
                
                Action handleRestoreBackUpFile = HandleRestoreBackUpFile;
                var parameters = new ModalParameters();
                parameters.Add("InfoHeader", "Restore Case Type");
                parameters.Add("ModalHeight", "300px");
                parameters.Add("ModalWidth", "500px");
                parameters.Add("ConfirmAction", handleRestoreBackUpFile);
                parameters.Add("InfoText", $"Are you sure you wish to retore the backup file? This may overwrite any existing Smartflows in the selected Case Type.");

                var options = new ModalOptions()
                {
                    Class = "blazored-custom-modal"
                };

                Modal.Show<ModalConfirm>("Delete Backup File", parameters, options);
            }
            catch(Exception e)
            {
                await GenericErrorLog(false,e, "PrepareBackUpForDelete", e.Message);
            }
        }

        private async void HandleRestoreBackUpFile()
        {
            await RestoreBackUpFile();
        }

        private async Task RestoreBackUpFile()
        {
            var Json = SmartflowFileHelper.ReadJson(SelectedFilePath); //CustomPath set in OnInitialized

           if(ValidateJson(Json))
           {
                try
                {
                    VmSmartflows vmSmartflows = JsonConvert.DeserializeObject<VmSmartflows>(Json);

                    

                    if(vmSmartflows.Smartflows.Count > 0)
                    {
                        int lastSeqNo = _LstVmClientSmartflowRecord
                                                            .Where(C => C.ClientSmartflowRecord.CaseTypeGroup == _SelectedCaseTypeGroup)
                                                            .Where(C => C.ClientSmartflowRecord.CaseType == _SelectedCaseType)
                                                            .OrderByDescending(C => C.ClientSmartflowRecord.SeqNo)
                                                            .Count();

                        foreach(Smartflow vmSmartflow in vmSmartflows.Smartflows.OrderBy(S => S.SeqNo))
                        {
                            //Check if already exists
                            Client_VmSmartflowRecord existingSmartflow = _LstVmClientSmartflowRecord
                                                            .Where(C => C.ClientSmartflowRecord.CaseTypeGroup == _SelectedCaseTypeGroup)
                                                            .Where(C => C.ClientSmartflowRecord.CaseType == _SelectedCaseType)
                                                            .Where(C => C.ClientSmartflowRecord.SmartflowName == vmSmartflow.Name)
                                                            .OrderByDescending(C => C.ClientSmartflowRecord.SeqNo)
                                                            .FirstOrDefault();

                            if (existingSmartflow == null)
                            {
                                //Smartflow of same name does not already exist in the selected Case Type
                                //Create New
                                Client_SmartflowRecord newSmartflow = new Client_SmartflowRecord();
                                
                                newSmartflow.SmartflowName = vmSmartflow.Name;
                                newSmartflow.CaseType = _SelectedCaseType;
                                newSmartflow.CaseTypeGroup = _SelectedCaseTypeGroup;
                                newSmartflow.SeqNo = vmSmartflow.SeqNo + lastSeqNo;
                                newSmartflow.SmartflowData = JsonConvert.SerializeObject(vmSmartflow);


                                var returnObject = await ClientApiManagementService.Add(newSmartflow);
                                newSmartflow.Id = returnObject.Id;
                                await CompanyDbAccess.SaveSmartFlowRecord(newSmartflow, UserSession);
                            }
                            else
                            {
                                //Existing Smartflow exists, update with data from backup file
                                existingSmartflow.ClientSmartflowRecord.SmartflowData = JsonConvert.SerializeObject(vmSmartflow);

                                await ClientApiManagementService.UpdateMainItem(existingSmartflow.ClientSmartflowRecord);
                            }

                            await NotificationManager.ShowNotification("Success", $"Backup fully restored to {_SelectedCaseType}.");

                            await _RefreshSmartflowsTask.InvokeAsync();

                        }
                    }
                }
                catch (Exception ex)
                {
                    await GenericErrorLog(true,ex, "RestoreBackUpFile", $"Restoring Smartflows (Case Type): {ex.Message}").ConfigureAwait(false);
                }
           }

        }

        
        private bool ValidateJson(string Json)
        {
            JsonErrors = new List<string>();
            JsonErrors = SmartflowFileHelper.ValidateSmartflowJSON(Json); //CustomPath set in OnInitialized

            if (JsonErrors.Count == 0)
            {
                return true;

            }
            else
            {
                ShowErrorModal("Backup Restore Error", "The following errors occured during the restore:", JsonErrors);

                return false;
            }
        }


        private async void DownloadFile(FileDesc file)
        {
            var data = SmartflowFileHelper.ReadFileToByteArray(file.FilePath); //CustomPath set in OnInitialized

            await JSRuntime.InvokeAsync<object>(
                 "DownloadTextFile",
                 file.FileName,
                 Convert.ToBase64String(data));


        }

        

        private async void HandleBackupFileSelection(IFileListEntry[] entryFiles)
        {
            await BackupFileSelection(entryFiles);
        }

        private async Task BackupFileSelection(IFileListEntry[] entryFiles)
        {
            try
            {
                var files = new List<IFileListEntry>();
                IList<string> fileErrorDescs = new List<string>();

                foreach (var file in entryFiles)
                {
                    if (file != null)
                    {
                        if(!(file.Name.Contains(".txt") || file.Name.Contains(".JSON")))
                        {
                            fileErrorDescs.Add($"The file: {file.Name} is not the correct type for a backup. Backup files must be either .txt or .JSON");
                        }
                        else
                        {
                            if(ListFilesForBackups.Where(F => F.FileName == file.Name).FirstOrDefault() is null)
                            {
                                await SmartflowFileHelper.UploadSmartflowFiles(file); //CustomPath set in OnInitialized
                                files.Add(file);
                            }
                            else
                            {
                                fileErrorDescs.Add($"The file: {file.Name} already exists on the system");
                            }

                            
                        }
                        
                    }

                }
                if (files != null && files.Count > 0)
                {
                    StateHasChanged();
                }

                if(fileErrorDescs.Count > 0)
                {
                    ShowErrorModal("File Upload Error", "The following errors occured during the upload:", fileErrorDescs);
                }

                GetSeletedFileList();

                StateHasChanged();
            }
            catch(Exception e)
            {
                await GenericErrorLog(false,e, "HandleBackupFileSelection", e.Message);
            }
        }


        protected async Task PrepareBackUpForDelete(FileDesc selectedFile)
        {
            try
            {
                
                SelectedFileDescription = selectedFile;

                string itemName = selectedFile.FileName;

                Action SelectedDeleteAction = HandleDeleteBackupFile;
                var parameters = new ModalParameters();
                parameters.Add("ItemName", itemName);
                parameters.Add("ModalHeight", "300px");
                parameters.Add("ModalWidth", "500px");
                parameters.Add("DeleteAction", SelectedDeleteAction);
                parameters.Add("InfoText", $"Are you sure you wish to delete the '{itemName}' backup file?");

                var options = new ModalOptions()
                {
                    Class = "blazored-custom-modal"
                };

                Modal.Show<ModalDelete>("Delete Backup File", parameters, options);
            }
            catch(Exception e)
            {
                await GenericErrorLog(false,e, "PrepareBackUpForDelete", e.Message);
            }
        }

        private void HandleDeleteBackupFile()
        {
            DeleteBackupFile(SelectedFileDescription);
        }

        private void DeleteBackupFile(FileDesc file)
        {
            SmartflowFileHelper.DeleteFile(file.FilePath); //CustomPath set in OnInitialized
            GetSeletedFileList();
            // StateHasChanged();
        }
        
        
       


        /****************************************/
        /* ERROR HANDLING */
        /****************************************/

        protected void ShowErrorModal(string header, string errorDesc, IList<string> errorDets)
        {
            var parameters = new ModalParameters();
            parameters.Add("ErrorDesc", errorDesc);
            parameters.Add("ErrorDetails", errorDets);

            var options = new ModalOptions()
            {
                Class = "blazored-custom-modal modal-smartflow-import"
            };

            Modal.Show<ModalErrorInfo>(header, parameters, options);
        }

        private async Task GenericErrorLog(bool showNotificationMsg, Exception e, string _method, string _message)
        {
            using (LogContext.PushProperty("SourceSystem", UserSession.SelectedSystem))
            using (LogContext.PushProperty("SourceCompanyId", UserSession.Company.Id))
            using (LogContext.PushProperty("SourceUserId", UserSession.User.Id))
            using (LogContext.PushProperty("SourceContext", nameof(CaseTypeBackupDetail)))
            {
                Logger.LogError(e,"Error - Method: {0}, Message: {1}",_method, _message);
            }

            if(showNotificationMsg)
            {
                await NotificationManager.ShowNotification("Danger", $"Oops! Something went wrong.");
            }
        }


    }
}