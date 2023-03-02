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
using GadjIT_ClientContext.P4W.Custom;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog.Context;
using BlazorInputFile;
using GadjIT_App.FileManagement.FileProcessing.Interface;
using GadjIT_App.FileManagement.FileClassObjects;
using GadjIT_App.Pages.Chapters.FileUpload;
using GadjIT_App.FileManagement.FileClassObjects.FileOptions;
using Microsoft.JSInterop;
using GadjIT_App.Shared.StaticObjects;
using GadjIT_ClientContext.P4W;
using GadjIT_App.Data.Admin;

namespace GadjIT_App.Pages.Chapters.ComponentsCaseTypeDetail._Backup
{
    public partial class CaseTypeBackupDetail
    {
        

        [Parameter]
        public List<VmUsrOrsfSmartflows> _LstChapters { get; set; }

   
        [Parameter]
        public string _SelectedCaseTypeGroup { get; set; }

        [Parameter]
        public string _SelectedCaseType { get; set; }

        [Parameter]
        public EventCallback _RefreshChapters {get; set;}


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
        private ILogger<ChapterList> Logger { get; set; }

        [Inject]
        private IChapterManagementService ChapterManagementService { get; set; }

        [Inject]
        public IAppChapterState AppChapterState { get; set; }

        [Inject]
        private IChapterFileUpload ChapterFileUpload { get; set; }

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
                List<VmUsrOrsfSmartflows> lstCaseTypeSmartflows = _LstChapters
                                                            .Where(C => C.SmartflowObject.CaseTypeGroup == _SelectedCaseTypeGroup)
                                                            .Where(C => C.SmartflowObject.CaseType == _SelectedCaseType)
                                                            .OrderBy(C => C.SmartflowObject.SeqNo)
                                                            .ToList();
                                                            

                if(lstCaseTypeSmartflows != null && lstCaseTypeSmartflows.Count > 0)
                {
                    VmSmartflows vmSmartflows = new VmSmartflows();

                    foreach(var vmSmartflow in lstCaseTypeSmartflows)
                    {
                        VmSmartflow smartflow = JsonConvert.DeserializeObject<VmSmartflow>(vmSmartflow.SmartflowObject.SmartflowData);
                        vmSmartflows.Smartflows.Add(smartflow);

                    }

                    string caseTypeData = JsonConvert.SerializeObject(vmSmartflows);

                    var fileName = _SelectedCaseType + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".txt";

                    SetSmartflowFilePath();

                    ChapterFileUpload.WriteChapterToFile(caseTypeData, fileName);

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
                ChapterFileOptions chapterFileOption;

                chapterFileOption = new ChapterFileOptions
                {
                    Company = UserSession.Company.CompanyName,
                    SelectedSystem = UserSession.SelectedSystem,
                    CaseTypeGroup = _SelectedCaseTypeGroup,
                    CaseType = _SelectedCaseType,
                    Chapter = null
                };

                ChapterFileUpload.SetFileHelperCustomPath(chapterFileOption,FileStorageType.BackupsCaseType);
            }
            catch (Exception ex)
            {
                GenericErrorLog(true,ex, "SetSmartflowFilePath", $"Setting Smartflow file path: {ex.Message}").ConfigureAwait(false);
            }
        }

        private void GetSeletedFileList()
        {
            ListFilesForBackups = ChapterFileUpload.GetFileListForChapter();

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
            var Json = ChapterFileUpload.readJson(SelectedFilePath);

           if(ValidateJson(Json))
           {
                try
                {
                    VmSmartflows vmSmartflows = JsonConvert.DeserializeObject<VmSmartflows>(Json);

                    if(vmSmartflows.Smartflows.Count > 0)
                    {
                        foreach(VmSmartflow vmSmartflow in vmSmartflows.Smartflows.OrderByDescending(S => S.SeqNo))
                        {
                            //Check if already exists
                            VmUsrOrsfSmartflows existingSmartflow = _LstChapters
                                                            .Where(C => C.SmartflowObject.CaseTypeGroup == _SelectedCaseTypeGroup)
                                                            .Where(C => C.SmartflowObject.CaseType == _SelectedCaseType)
                                                            .Where(C => C.SmartflowObject.SmartflowName == vmSmartflow.Name)
                                                            .OrderByDescending(C => C.SmartflowObject.SeqNo)
                                                            .FirstOrDefault();

                            if (existingSmartflow == null)
                            {
                                //Smartflow of same name does not already exist in the selected Case Type
                                //Create New
                                UsrOrsfSmartflows newSmartflow = new UsrOrsfSmartflows();
                                
                                newSmartflow.SmartflowName = vmSmartflow.Name;
                                newSmartflow.CaseType = _SelectedCaseType;
                                newSmartflow.CaseTypeGroup = _SelectedCaseTypeGroup;
                                newSmartflow.SmartflowData = JsonConvert.SerializeObject(vmSmartflow);


                                var returnObject = await ChapterManagementService.Add(newSmartflow);
                                newSmartflow.Id = returnObject.Id;
                                await CompanyDbAccess.SaveSmartFlowRecord(newSmartflow, UserSession);
                            }
                            else
                            {
                                //Existing Smartflow exists, update with data from backup file
                                existingSmartflow.SmartflowObject.SmartflowData = JsonConvert.SerializeObject(vmSmartflow);

                                await ChapterManagementService.UpdateMainItem(existingSmartflow.SmartflowObject);
                            }

                            await NotificationManager.ShowNotification("Success", $"Backup fully restored to {_SelectedCaseType}.");

                            await _RefreshChapters.InvokeAsync();

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
            JsonErrors = ChapterFileUpload.ValidateChapterJSON(Json);

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
            var data = ChapterFileUpload.ReadFileToByteArray(file.FilePath);

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
                                await ChapterFileUpload.UploadChapterFiles(file);
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
            ChapterFileUpload.DeleteFile(file.FilePath);
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
                Class = "blazored-custom-modal modal-chapter-import"
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