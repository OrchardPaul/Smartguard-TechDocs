using Blazored.Modal;
using BlazorInputFile;
using GadjIT.ClientContext.P4W;
using GadjIT.ClientContext.P4W.Custom;
using GadjIT_V1_02.FileManagement.FileClassObjects;
using GadjIT_V1_02.Services;
using GadjIT_V1_02.Services.AppState;
using GadjIT_V1_02.Services.SessionState;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GadjIT_V1_02.Pages.Chapters
{
    public partial class ChapterImport
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
        IChapterFileUpload ChapterFileUpload { get; set; }

        [Inject]
        IChapterManagementService chapterManagementService { get; set; }

        [Parameter]
        public IUserSessionState sessionState { get; set; }

        [Inject]
        IAppChapterState appChapterState { get; set; }

        [Parameter]
        public UsrOrsfSmartflows TaskObject { get; set; }

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

        private VmChapter ChapterItems { get; set; }

        [Parameter]
        public List<VmDataViews> OriginalDataViews { get; set; }
        
        

        private async void Close()
        {
            await ModalInstance.CloseAsync();
        }

        private void GetSeletedChapterFileList()
        {
            ListFileDescriptions = ChapterFileUpload.GetFileListForChapter();
        }

        private void ToggleErrorList(bool option)
        {
            ToggleError = option;
            StateHasChanged();
        }


        private async void HandleFileSelection(IFileListEntry[] entryFiles)
        {
            var files = new List<IFileListEntry>();
            ToggleError = false;
            ErrorList = new List<string>();

            while(!(ListFileDescriptions.Where(F => F.FilePath.Contains(".xlsx")).FirstOrDefault() is null))
            {
                ChapterFileUpload.DeleteFile(ListFileDescriptions.Where(F => F.FilePath.Contains(".xlsx")).FirstOrDefault().FilePath);
                GetSeletedChapterFileList();
            }

            foreach (var file in entryFiles)
            {
                if (file != null)
                {
                    await ChapterFileUpload.UploadChapterFiles(file);
                    files.Add(file);
                }
            }

            var fileName = files.FirstOrDefault().Name;
            GetSeletedChapterFileList();

            if (ListFileDescriptions.Where(F => F.FilePath.Contains(".xlsx")).FirstOrDefault() is null)
            {
                ErrorList.Add("File uploaded is not an excel spreadsheet");

                ChapterFileUpload.DeleteFile(ListFileDescriptions.Where(F => F.FileName == fileName).FirstOrDefault().FilePath);

                ToggleErrorList(true);
                GetSeletedChapterFileList();
            }
            else
            {
                ErrorList = ChapterFileUpload.ValidateChapterExcel(ListFileDescriptions.Where(F => F.FileName == fileName).FirstOrDefault().FilePath);

                if(ErrorList.Count > 0)
                {
                    ToggleErrorList(true);
                    ChapterFileUpload.DeleteFile(ListFileDescriptions.Where(F => F.FileName == fileName).FirstOrDefault().FilePath);
                    GetSeletedChapterFileList();
                }
                else if (files != null && files.Count > 0)
                {
                    ChapterItems = ChapterFileUpload.readChapterItemsFromExcel(ListFileDescriptions.Where(F => F.FileName == fileName).FirstOrDefault().FilePath);
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

        private async void HandleValidSubmit()
        {
            var originalJson = new string(TaskObject.SmartflowData);
            var SelectedCopyItems = new VmChapter { Items = new List<GenSmartflowItem>(), Fees = new List<Fee>(), DataViews = new List<DataViews>() };

            ToggleSuccess = false;

            if (!(TaskObject.SmartflowData is null))
            {
                SelectedCopyItems = JsonConvert.DeserializeObject<VmChapter>(TaskObject.SmartflowData);
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
                    SelectedCopyItems.Fees = new List<Fee>();
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
                    SelectedCopyItems.DataViews = new List<DataViews>();
                }

                foreach (var item in SelectedCopyItems.DataViews.ToList())
                {
                    SelectedCopyItems.DataViews.Remove(item);
                }

                SelectedCopyItems.DataViews.AddRange(ChapterItems.DataViews);
            }

            ImportedJSON = JsonConvert.SerializeObject(new VmChapter
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
                var jsonErrors = ChapterFileUpload.ValidateChapterJSON(ImportedJSON);

                if (jsonErrors.Count > 0)
                {
                    ErrorList.AddRange(jsonErrors);
                    ToggleErrorList(true);
                }
                else
                {
                    WriteBackUp?.Invoke();

                    TaskObject.SmartflowData = ImportedJSON;

                    ChapterFileUpload.DeleteFile(ListFileDescriptions.Where(F => F.FilePath.Contains(".xlsx")).FirstOrDefault().FilePath);

                    await chapterManagementService.Update(TaskObject);

                    DataChanged?.Invoke();
                    ToggleSuccess = true;

                    StateHasChanged();
                }
            }
            
        }
    }
}
