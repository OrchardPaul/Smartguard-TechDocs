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
using GadjIT_ClientContext.Models.Smartflow;
using GadjIT_ClientContext.Models.Smartflow.Client;

namespace GadjIT_App.Pages.Smartflows.ComponentsSmartflowDetail._Backup
{
    public partial class SmartflowBackupDetail
    {
        [Parameter]
        public Client_SmartflowRecord _Selected_ClientSmartflowRecord { get; set; }

        [Parameter]
        public SmartflowV2 _SelectedSmartflow { get; set; }

        [Parameter]
        public EventCallback<SmartflowV2> _SmartflowUpdated {get; set;}

        [Parameter]
        public bool _SmartflowLockedForEdit {get; set;}
        

        [Inject]
        IModalService Modal { get; set; }

        [Inject]
        private IJSRuntime JSRuntime {get; set;}
        

        [Inject]
        public IUserSessionState UserSession { get; set; }

        [Inject]
        public INotificationManager NotificationManager {get; set;}

        [Inject]
        private ILogger<SmartflowBackupDetail> Logger { get; set; }

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



        protected override void OnInitialized()
        {
            SetSmartflowFilePath();

            GetSeletedChapterFileList();
        }

        
        public async void WriteChapterJSONToFile()
        {
            try
            {
                var fileName = _SelectedSmartflow.Name + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".txt";

                SmartflowFileHelper.WriteSmartflowToFile(_Selected_ClientSmartflowRecord.SmartflowData, fileName); //CustomPath set in OnInitialized

                GetSeletedChapterFileList();
                StateHasChanged();
            }
            catch(Exception e)
            {
                await GenericErrorLog(false,e, "WriteChapterJSONToFile", e.Message);
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
                    CaseTypeGroup = _SelectedSmartflow.CaseTypeGroup,
                    CaseType = _SelectedSmartflow.CaseType,
                    SmartflowName = _SelectedSmartflow.Name
                };

                SmartflowFileHelper.SetFileHelperCustomPath(chapterFileOption,FileStorageType.BackupsSmartflow);
            }
            catch (Exception ex)
            {
                GenericErrorLog(true,ex, "SetSmartflowFilePath", $"Setting Smartflow file path: {ex.Message}").ConfigureAwait(false);
            }
        }

        private void GetSeletedChapterFileList()
        {
            ListFilesForBackups = SmartflowFileHelper.GetFileListForSmartflow(); //CustomPath set in OnInitialized
        }

        private async void ReadBackUpFile(string filePath)
        {
        var Json = SmartflowFileHelper.ReadJson(filePath); 

           if(ValidateJson(_Selected_ClientSmartflowRecord.SmartflowData))
           {
                _SelectedSmartflow = JsonConvert.DeserializeObject<SmartflowV2>(Json);

                await ChapterUpdated();
           }

        }

        private async Task ChapterUpdated()
        {
            try
            {
                await _SmartflowUpdated.InvokeAsync(_SelectedSmartflow);

                await NotificationManager.ShowNotification("Success", $"Restore complete.");
            }
            catch(Exception e)
            {
                await GenericErrorLog(true,e, "ChapterUpdated", $"Restoring Smartflow: {e.Message}").ConfigureAwait(false);

            }

        }

        private bool ValidateJson(string Json)
        {
            JsonErrors = new List<string>();
            JsonErrors = SmartflowFileHelper.ValidateSmartflowJSON(Json); 

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
            var data = SmartflowFileHelper.ReadFileToByteArray(file.FilePath);

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
                                await SmartflowFileHelper.UploadSmartflowFiles(file);
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

                GetSeletedChapterFileList();

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
            SmartflowFileHelper.DeleteFile(file.FilePath);
            GetSeletedChapterFileList();
            StateHasChanged();
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
            using (LogContext.PushProperty("SourceContext", nameof(SmartflowBackupDetail)))
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