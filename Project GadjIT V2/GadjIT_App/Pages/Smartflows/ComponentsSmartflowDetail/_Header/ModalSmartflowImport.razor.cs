using Blazored.Modal;
using BlazorInputFile;
using GadjIT_App.FileManagement.FileClassObjects;
using GadjIT_App.Services;
using GadjIT_App.Services.AppState;
using GadjIT_App.Services.SessionState;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GadjIT_App.Pages.Smartflows.FileHandling;
using GadjIT_ClientContext.Models.Smartflow;
using GadjIT_ClientContext.Models.Smartflow.Client;
using GadjIT_App.FileManagement.FileClassObjects.FileOptions;
using GadjIT_App.Shared.StaticObjects;

namespace GadjIT_App.Pages.Smartflows.ComponentsSmartflowDetail._Header
{
    public partial class ModalSmartflowImport
    {
        public class CopyOption
        {
            public string Option { get; set; }
            public bool Selected { get; set; }
            public int OptionCount { get; set; }
        }

        [CascadingParameter]
        BlazoredModalInstance ModalInstance { get; set; }

        [Inject]
        ISmartflowFileHelper SmartflowFileHelper { get; set; }

        [Inject]
        IClientApiManagementService ClientApiManagementService { get; set; }

        [Parameter]
        public IUserSessionState UserSession { get; set; }

        [Inject]
        IAppSmartflowsState AppSmartflowsState { get; set; }

        [Parameter]
        public Client_SmartflowRecord TaskObject { get; set; }

        [Parameter]
        public Action DataChanged { get; set; }

        [Parameter]
        public Action WriteBackUp { get; set; }

        [Parameter]
        public List<FileDesc> ListFileDescriptions { get; set; }

        public List<CopyOption> CopyOptions { get; set; } 
        
        public List<string> ErrorList { get; set; } = new List<string>();

        public bool ToggleError { get; set; }

        public bool ToggleSuccess { get; set; }

        private string ImportedJSON { get; set; }

        private Smartflow ChapterItems { get; set; }

        [Parameter]
        public List<VmSmartflowDataView> OriginalDataViews { get; set; }
        
        

        private async Task Close()
        {
            await ModalInstance.CloseAsync();
        }

        private void GetSeletedSmartflowFileList()
        {
            ListFileDescriptions = SmartflowFileHelper.GetFileListForSmartflow();  //CustomPath set in Parent Component
        }

        private void ToggleErrorList(bool option)
        {
            ToggleError = option;
            StateHasChanged();
        }


        private async Task HandleFileSelection(IFileListEntry[] entryFiles)
        {
            var files = new List<IFileListEntry>();
            ToggleError = false;
            ErrorList = new List<string>();

            
            GetSeletedSmartflowFileList(); //populates ListFilesForBackups

            //clear all existing spreadsheets to make way for the new instance
            //in case a spreadsheet is locked and can't be deleted, work through a list of docs
            //report error if any remain.
            foreach(var fileItem in ListFileDescriptions.Where(F => F.FilePath.Contains(".xlsx")).ToList())
            {
                SmartflowFileHelper.DeleteFile(fileItem.FilePath);
            }

            GetSeletedSmartflowFileList();
            
            if(ListFileDescriptions.Where(F => F.FilePath.Contains(".xlsx")).Count() > 0)
            {
                throw new Exception("Excel Spreadsheet deletion failed");
            }

            foreach (var file in entryFiles)
            {
                if (file != null)
                {
                    await SmartflowFileHelper.UploadSmartflowFiles(file); //CustomPath set in parent component
                    files.Add(file);
                }
            }

            var fileName = files.FirstOrDefault().Name;
            GetSeletedSmartflowFileList();

            if (ListFileDescriptions.Where(F => F.FilePath.Contains(".xlsx")).FirstOrDefault() is null)
            {
                ErrorList.Add("File uploaded is not an excel spreadsheet");

                SmartflowFileHelper.DeleteFile(ListFileDescriptions.Where(F => F.FileName == fileName).FirstOrDefault().FilePath);

                ToggleErrorList(true);
                GetSeletedSmartflowFileList();
            }
            else
            {
                ErrorList = SmartflowFileHelper.ValidateSmartflowExcel(ListFileDescriptions.Where(F => F.FileName == fileName).FirstOrDefault().FilePath);

                if(ErrorList.Count > 0)
                {
                    ToggleErrorList(true);
                    SmartflowFileHelper.DeleteFile(ListFileDescriptions.Where(F => F.FileName == fileName).FirstOrDefault().FilePath);
                    GetSeletedSmartflowFileList();
                }
                else if (files != null && files.Count > 0)
                {
                    ChapterItems = SmartflowFileHelper.ReadSmartflowItemsFromExcel(ListFileDescriptions.Where(F => F.FileName == fileName).FirstOrDefault().FilePath);
                    CopyOptions = new List<CopyOption>
                                                {
                                                    new CopyOption { Option = "Agenda", Selected = false, OptionCount = ChapterItems.Items.Where(C => C.Type == "Agenda").ToList().Count() },
                                                    new CopyOption { Option = "Status", Selected = false, OptionCount = ChapterItems.Items.Where(C => C.Type == "Status").ToList().Count() },
                                                    new CopyOption { Option = "Documents/Steps", Selected = false, OptionCount = ChapterItems.Items.Where(C => C.Type == "Doc").ToList().Count() },
                                                    new CopyOption { Option = "Fees", Selected = false, OptionCount = ChapterItems.Fees.Count() },
                                                    new CopyOption { Option = "Data Views", Selected = false, OptionCount = ChapterItems.DataViews.Count() },
                                                };

                    StateHasChanged();
                }
            }
        }

        /// <summary>
        /// First grabs the list of chapter items (Docs,Agendas,Fees etc.) from the uploaded excel
        /// along with the currently held data on the chapter
        /// 
        /// Then checks for each option selected whether to replace the current data with the one uploaded
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <returns>string: row-changed or row-changedx</returns>

        private async Task HandleValidSubmit()
        {
            var originalJson = new string(TaskObject.SmartflowData);
            var SelectedCopyItems = new Smartflow { Items = new List<GenSmartflowItem>(), Fees = new List<SmartflowFee>(), DataViews = new List<SmartflowDataView>() };

            ToggleSuccess = false;

            if (!(TaskObject.SmartflowData is null))
            {
                SelectedCopyItems = JsonConvert.DeserializeObject<Smartflow>(TaskObject.SmartflowData);
            }


            if (CopyOptions.Where(C => C.Option == "Agenda").Select(C => C.Selected).FirstOrDefault())
            {
                foreach (var item in SelectedCopyItems.Items.Where(C => C.Type == "Agenda").ToList())
                {
                    SelectedCopyItems.Items.Remove(item);
                }

                SelectedCopyItems.Items.AddRange(ChapterItems.Items.Where(C => C.Type == "Agenda").ToList());
            }

            if (CopyOptions.Where(C => C.Option == "Status").Select(C => C.Selected).FirstOrDefault())
            {
                foreach (var item in SelectedCopyItems.Items.Where(C => C.Type == "Status").ToList())
                {
                    SelectedCopyItems.Items.Remove(item);
                }

                SelectedCopyItems.Items.AddRange(ChapterItems.Items.Where(C => C.Type == "Status").ToList());
            }

            if (CopyOptions.Where(C => C.Option == "Documents/Steps").Select(C => C.Selected).FirstOrDefault())
            {
                foreach (var item in SelectedCopyItems.Items.Where(C => C.Type == "Doc").ToList())
                {
                    SelectedCopyItems.Items.Remove(item);
                }

                SelectedCopyItems.Items.AddRange(ChapterItems.Items.Where(C => C.Type == "Doc").ToList());
            }

            if (CopyOptions.Where(C => C.Option == "Fees").Select(C => C.Selected).FirstOrDefault())
            {
                if(SelectedCopyItems.Fees is null)
                {
                    SelectedCopyItems.Fees = new List<SmartflowFee>();
                }

                foreach (var item in SelectedCopyItems.Fees.ToList())
                {
                    SelectedCopyItems.Fees.Remove(item);
                }

                SelectedCopyItems.Fees.AddRange(ChapterItems.Fees);
            }

            if (CopyOptions.Where(C => C.Option == "Data Views").Select(C => C.Selected).FirstOrDefault())
            {
                if (SelectedCopyItems.DataViews is null)
                {
                    SelectedCopyItems.DataViews = new List<SmartflowDataView>();
                }

                foreach (var item in SelectedCopyItems.DataViews.ToList())
                {
                    SelectedCopyItems.DataViews.Remove(item);
                }

                SelectedCopyItems.DataViews.AddRange(ChapterItems.DataViews);
            }

            ImportedJSON = JsonConvert.SerializeObject(new Smartflow
            {
                CaseTypeGroup = TaskObject.CaseTypeGroup,
                CaseType = TaskObject.CaseType,
                Name = TaskObject.SmartflowName,
                SeqNo = TaskObject.SeqNo.GetValueOrDefault(),
                ShowDocumentTracking = ChapterItems.ShowDocumentTracking,
                ShowPartnerNotes = ChapterItems.ShowPartnerNotes,
                P4WCaseTypeGroup = SelectedCopyItems.P4WCaseTypeGroup,
                SelectedStep = SelectedCopyItems.SelectedStep,
                SelectedView = SelectedCopyItems.SelectedView,
                BackgroundColour = SelectedCopyItems.BackgroundColour,
                BackgroundColourName = SelectedCopyItems.BackgroundColourName,
                BackgroundImage = SelectedCopyItems.BackgroundImage,
                BackgroundImageName = SelectedCopyItems.BackgroundImageName,
                Items = SelectedCopyItems.Items,
                Fees = SelectedCopyItems.Fees,
                DataViews = SelectedCopyItems.DataViews
            });


            if (ImportedJSON == originalJson)
            {
                ErrorList.Add("No new updates are present in the import.");
                ToggleErrorList(true);
            }
            else
            {
                var jsonErrors = SmartflowFileHelper.ValidateSmartflowJSON(ImportedJSON);

                if (jsonErrors.Count > 0)
                {
                    ErrorList.AddRange(jsonErrors);
                    ToggleErrorList(true);
                }
                else
                {
                    //WriteBackUp?.Invoke(); //Backup 

                    TaskObject.SmartflowData = ImportedJSON;

                    SmartflowFileHelper.DeleteFile(ListFileDescriptions.Where(F => F.FilePath.Contains(".xlsx")).FirstOrDefault().FilePath);

                    await ClientApiManagementService.Update(TaskObject);

                    DataChanged?.Invoke();
                    ToggleSuccess = true;

                    StateHasChanged();
                }
            }
            
        }
    }
}
