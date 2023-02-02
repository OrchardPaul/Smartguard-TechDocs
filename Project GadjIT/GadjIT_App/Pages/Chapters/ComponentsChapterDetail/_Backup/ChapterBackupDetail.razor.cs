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
using GadjIT_ClientContext.P4W;
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

namespace GadjIT_App.Pages.Chapters.ComponentsChapterDetail._Backup
{
    public partial class ChapterBackupDetail
    {
        [Parameter]
        public UsrOrsfSmartflows _SelectedChapterObject { get; set; }

        [Parameter]
        public VmSmartflow _SelectedChapter { get; set; }

        [Parameter]
        public EventCallback<VmSmartflow> _ChapterUpdated {get; set;}

        [Inject]
        IModalService Modal { get; set; }

        [Inject]
        private IJSRuntime JSRuntime {get; set;}
        

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



        protected override void OnInitialized()
        {
            SetSmartflowFilePath();

            GetSeletedChapterFileList();
        }

        
        public async void WriteChapterJSONToFile()
        {
            try
            {
                var fileName = _SelectedChapter.Name + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".txt";

                ChapterFileUpload.WriteChapterToFile(_SelectedChapterObject.SmartflowData, fileName);

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
                ChapterFileOptions chapterFileOption;

                chapterFileOption = new ChapterFileOptions
                {
                    Company = UserSession.Company.CompanyName,
                    SelectedSystem = UserSession.SelectedSystem,
                    CaseTypeGroup = _SelectedChapter.CaseTypeGroup,
                    CaseType = _SelectedChapter.CaseType,
                    Chapter = _SelectedChapter.Name
                };

                ChapterFileUpload.SetFileHelperCustomPath(chapterFileOption,FileStorageType.BackupsSmartflow);
            }
            catch (Exception ex)
            {
                GenericErrorLog(true,ex, "SetSmartflowFilePath", $"Setting Smartflow file path: {ex.Message}").ConfigureAwait(false);
            }
        }

        private void GetSeletedChapterFileList()
        {
            ListFilesForBackups = ChapterFileUpload.GetFileListForChapter();
        }

        private async void ReadBackUpFile(string filePath)
        {
        var Json = ChapterFileUpload.readJson(filePath);

           if(ValidateJson(_SelectedChapterObject.SmartflowData))
           {
                _SelectedChapter = JsonConvert.DeserializeObject<VmSmartflow>(Json);

                await ChapterUpdated();
           }

        }

        private async Task ChapterUpdated()
        {
            try
            {
                await _ChapterUpdated.InvokeAsync(_SelectedChapter);

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
            ChapterFileUpload.DeleteFile(file.FilePath);
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
                Class = "blazored-custom-modal modal-chapter-import"
            };

            Modal.Show<ModalErrorInfo>(header, parameters, options);
        }

        private async Task GenericErrorLog(bool showNotificationMsg, Exception e, string _method, string _message)
        {
            using (LogContext.PushProperty("SourceSystem", UserSession.SelectedSystem))
            using (LogContext.PushProperty("SourceCompanyId", UserSession.Company.Id))
            using (LogContext.PushProperty("SourceUserId", UserSession.User.Id))
            using (LogContext.PushProperty("SourceContext", nameof(ChapterBackupDetail)))
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