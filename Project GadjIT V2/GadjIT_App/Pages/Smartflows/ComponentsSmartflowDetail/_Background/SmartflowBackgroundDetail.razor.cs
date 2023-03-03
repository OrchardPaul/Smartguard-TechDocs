using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Blazored.Modal;
using Blazored.Modal.Services;
using GadjIT_App.Data.Admin;
using GadjIT_App.Pages._Shared.Modals;
using GadjIT_App.Services;
using GadjIT_App.Services.AppState;
using GadjIT_App.Services.SessionState;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog.Context;
using System.Text.RegularExpressions;
using BlazorInputFile;
using GadjIT_App.FileManagement.FileProcessing.Interface;
using GadjIT_App.FileManagement.FileClassObjects;
using GadjIT_App.Pages.Smartflows.FileHandling;
using GadjIT_App.FileManagement.FileClassObjects.FileOptions;
using GadjIT_App.Shared.StaticObjects;
using GadjIT_ClientContext.Models.Smartflow;
using GadjIT_ClientContext.Models.Smartflow.Client;

namespace GadjIT_App.Pages.Smartflows.ComponentsSmartflowDetail._Background
{
    public partial class SmartflowBackgroundDetail
    {
        [Parameter]
        public Client_SmartflowRecord _Selected_ClientSmartflowRecord { get; set; } 

        [Parameter]
        public Smartflow _SelectedSmartflow { get; set; } 

        [Parameter]
        public bool _SmartflowLockedForEdit {get; set;}
            
        [Inject]
        IModalService Modal { get; set; }

        [Inject]
        public IUserSessionState UserSession { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        private IIdentityUserAccess UserAccess { get; set; }

        [Inject]
        public INotificationManager NotificationManager {get; set;}

        [Inject]
        private ILogger<SmartflowBackgroundDetail> Logger { get; set; }

        [Inject]
        private IClientApiManagementService ClientApiManagementService { get; set; }

        [Inject]
        public IAppSmartflowsState AppSmartflowsState { get; set; }

        [Inject]
        public IFileHelper FileHelper { get; set; }

        [Inject]
        private ISmartflowFileHelper SmartflowFileHelper { get; set; }

        private List<FileDesc> ListFilesForBgImages { get; set; }

        private FileDesc SelectedFileDescription { get; set; }

        protected bool SmartflowIsSelected {get; set;}

        private class ChapterColour
        {
            public string ColourName { get; set; }
            public string ColourCode { get; set; }
        }

        protected override void OnInitialized()
        {
            
            if(_Selected_ClientSmartflowRecord == null)
            {
                //If used outside of a selected Smartflow
                //the following objects are not required but need to exist 
                //to prevent issues within the page
                _Selected_ClientSmartflowRecord = new Client_SmartflowRecord();
                _SelectedSmartflow = new Smartflow();
                SmartflowIsSelected = false;
            }
            else
            {
                SmartflowIsSelected = true;
            }

            SetSmartflowFilePath();
            
            ListFilesForBgImages = FileHelper.GetFileList(); //CustomPath set in above
        }

        

        public bool PreviewSmartflowImage
        {
            get 
            {
                return UserSession.User.DisplaySmartflowPreviewImage; 
            }
            set
            {
                if (value)
                {
                    if(!string.IsNullOrEmpty(_SelectedSmartflow.BackgroundImageName))
                    {
                        UserSession.SetTempBackground(_SelectedSmartflow.BackgroundImage.Replace("/wwwroot",""), NavigationManager.Uri);
                    }
                    else
                    {
                        UserSession.SetTempBackground("", NavigationManager.Uri);
                    }
                }
                else
                {
                    UserSession.TempBackGroundImage = "";
                }

                UserSession.User.DisplaySmartflowPreviewImage = value;
                UserSession.RefreshHome?.Invoke();
                SavePreviewSmartflowImage();
            }

        }

        public string SelectColour { 
            get 
            { 
                return _SelectedSmartflow.BackgroundColourName; 
            }
            set
            {
                SaveSelectedBackgroundColour(value);
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
                    SelectedSystem = UserSession.SelectedSystem
                    // CaseTypeGroup = _SelectedSmartflow.CaseTypeGroup,
                    // CaseType = _SelectedSmartflow.CaseType,
                    // Chapter = _SelectedSmartflow.Name
                };

                SmartflowFileHelper.SetFileHelperCustomPath(chapterFileOption, FileStorageType.BackgroundImages); 
            }
            catch (Exception ex)
            {
                GenericErrorLog(true,ex, "SetSmartflowFilePath", $"Setting Smartflow file path: {ex.Message}");
            }
        }

        

        private async void SavePreviewSmartflowImage()
        {
            bool gotLock = UserAccess.Lock;
            while (gotLock)
            {
                await Task.Yield();
                gotLock = UserAccess.Lock;
            }

            try
            {
                await UserAccess.UpdateUserDetails(UserSession.User).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                GenericErrorLog(true,e, "SavePreviewSmartflowImage", e.Message);
            }
        }

        private async Task HandleBgImageFileSelection(IFileListEntry[] entryFiles)
        {
            try
            {
                ListFilesForBgImages = FileHelper.GetFileList(); //CustomPath set in OnInitialized

                var files = new List<IFileListEntry>();
                IList<string> fileErrorDescs = new List<string>();

                foreach (var file in entryFiles)
                {
                    if (file != null)
                    {
                        if (!(file.Name.Contains(".jpg") || file.Name.Contains(".png")))
                        {
                            fileErrorDescs.Add($"The file: {file.Name} is not a valid image. Image files must be either .jpg or .png");
                        }
                        else
                        {
                            if (ListFilesForBgImages.Where(F => F.FileName == file.Name).FirstOrDefault() is null)
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

                if (fileErrorDescs.Count > 0)
                {
                    ShowErrorModal("File Upload", "The following errors occured during the upload:", fileErrorDescs);
                }

                ListFilesForBgImages = FileHelper.GetFileList(); //CustomPath set in OnInitialized

                StateHasChanged();
            }
            catch(Exception e)
            {
                GenericErrorLog(false,e, "HandleBgImageFileSelection", e.Message);
            }
            
        }

        protected void PrepareBgImageForDelete(FileDesc selectedFile)
        {
            try
            {
                
                SelectedFileDescription = selectedFile;

                string itemName = selectedFile.FileName;

                Action SelectedDeleteAction = HandleDeleteBgImageFile;
                var parameters = new ModalParameters();
                parameters.Add("ItemName", itemName);
                parameters.Add("ModalHeight", "300px");
                parameters.Add("ModalWidth", "500px");
                parameters.Add("DeleteAction", SelectedDeleteAction);
                parameters.Add("InfoText", $"Are you sure you wish to delete the '{itemName}' ?");

                var options = new ModalOptions()
                {
                    Class = "blazored-custom-modal"
                };

                Modal.Show<ModalDelete>("Delete Backup File", parameters, options);
            }
            catch(Exception e)
            {
                GenericErrorLog(false,e, "PrepareBgImageForDelete", e.Message);
            }
        }

        private void HandleDeleteBgImageFile()
        {
            DeleteBgImageFile(SelectedFileDescription);
        }

        private async void SelectBgImage(FileDesc fileDesc)
        {
            try
            {
                _SelectedSmartflow.BackgroundImage = fileDesc.FileURL.Replace("/wwwroot", "");
                _SelectedSmartflow.BackgroundImageName = fileDesc.FileName;

                _Selected_ClientSmartflowRecord.SmartflowData = JsonConvert.SerializeObject(_SelectedSmartflow);

                await ClientApiManagementService.Update(_Selected_ClientSmartflowRecord).ConfigureAwait(false);

                UserSession.SetTempBackground(_SelectedSmartflow.BackgroundImage.Replace("/wwwroot", ""), NavigationManager.Uri);
                UserSession.RefreshHome?.Invoke();

                await InvokeAsync(() =>
                {
                    StateHasChanged();
                });
            }
            catch(HttpRequestException)
            {
                //do nothing, already dealt with
            }
            catch(Exception e)
            {
                GenericErrorLog(false,e, "SelectBgImage", e.Message);
            }
        }

        private void DeleteBgImageFile(FileDesc file)
        {
            try
            {
                SmartflowFileHelper.DeleteFile(file.FilePath); //CustomPath set in OnInitialized

                ListFilesForBgImages = FileHelper.GetFileList(); 
                StateHasChanged();
            }
            catch(Exception e)
            {
                GenericErrorLog(false,e, "DeleteBgImageFile", e.Message);
            }
        }

        

        public async void SaveSelectedBackgroundColour (string colour)
        {
            try
            {
                _SelectedSmartflow.BackgroundColour = ListChapterColours.Where(C => C.ColourName == colour).Select(C => C.ColourCode).FirstOrDefault();
                _SelectedSmartflow.BackgroundColourName = colour;

                _Selected_ClientSmartflowRecord.SmartflowData = JsonConvert.SerializeObject(_SelectedSmartflow);

                await ClientApiManagementService.Update(_Selected_ClientSmartflowRecord).ConfigureAwait(false);

            }
            catch (Exception e)
            {
                GenericErrorLog(true,e, "SaveSelectedBackgroundColour", $"Saving background colour: {e.Message}");
            }
        }

        private List<ChapterColour> ListChapterColours { 
            get
            {
                List<ChapterColour> listChapterColours = new List<ChapterColour>
                {
                    new ChapterColour { ColourName = "", ColourCode = ""},
                    new ChapterColour { ColourName = "Grey", ColourCode = "#3F000000"},
                    new ChapterColour { ColourName = "Blue", ColourCode = "#3F0074FF"},
                    new ChapterColour { ColourName = "Pink", ColourCode = "#3FFD64EF"},
                    new ChapterColour { ColourName = "Peach", ColourCode = "#3FEA9C66"},
                    new ChapterColour { ColourName = "Yellow", ColourCode = "#3FFFFF00"},
                    new ChapterColour { ColourName = "Beige", ColourCode = "#3F957625"},
                    new ChapterColour { ColourName = "Lilac", ColourCode = "#3F6E6FDB"},
                    new ChapterColour { ColourName = "Green", ColourCode = "#3F32EC29"},
                    new ChapterColour { ColourName = "Aqua", ColourCode = "#3F5BDCD0"}
                };


                return listChapterColours;
            }
        } 

        public string getHTMLColourFromAndroid(string colAndroid)
        {
            string colHTML = "#FFFFFFFF";

            try
            {

                if (!string.IsNullOrEmpty(colAndroid) && (Regex.IsMatch(colAndroid, "^#(?:[0-9a-fA-F]{8})$")))
                {
                    colHTML = "#" + colAndroid.Substring(3, 6) + colAndroid.Substring(1, 2);
                }
                else
                {
                    colHTML = "#FFFFFFFF";
                }

            }
            catch(Exception e)
            {
                GenericErrorLog(false,e, "HandleBackupFileSelection", e.Message);
            }

            return colHTML;
        }

        /****************************************/
        /* ERROR HANDLING */
        /****************************************/
        private void GenericErrorLog(bool _showNotificationMsg, Exception e, string _method, string _message)
        {
            using (LogContext.PushProperty("SourceSystem", UserSession.SelectedSystem))
            using (LogContext.PushProperty("SourceCompanyId", UserSession.Company.Id))
            using (LogContext.PushProperty("SourceUserId", UserSession.User.Id))
            using (LogContext.PushProperty("SourceContext", nameof(SmartflowBackgroundDetail)))
            {
                Logger.LogError(e,"Error - Method: {0}, Message: {1}",_method, _message);
            }

            if(_showNotificationMsg)
            {
                NotificationManager.ShowNotification("Danger", $"Oops! Something went wrong.");
            }
        }

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

    }

}

        