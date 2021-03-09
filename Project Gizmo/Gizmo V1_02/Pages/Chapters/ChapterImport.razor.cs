using Blazored.Modal;
using BlazorInputFile;
using GadjIT.ClientContext.P4W;
using GadjIT.ClientContext.P4W.Custom;
using Gizmo_V1_02.FileManagement.FileClassObjects;
using Gizmo_V1_02.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gizmo_V1_02.Pages.Chapters
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
        public UsrOrDefChapterManagement TaskObject { get; set; }

        [Parameter]
        public Action DataChanged { get; set; }

        [Parameter]
        public Action WriteBackUp { get; set; }

        [Parameter]
        public List<FileDesc> ListFileDescriptions { get; set; }

        public List<CopyOption> CopyOptions { get; set; } 
        public List<string> lstDocTypes { get; set; } = new List<string> { "Doc", "Letter", "Form", "Email", "Step" };

        public List<string> ErrorList { get; set; } = new List<string>();

        public bool ToggleError { get; set; }

        private string ImportedJSON { get; set; }

        private List<UsrOrDefChapterManagement> ChapterItems { get; set; }

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
                                                    new CopyOption { Option = "Agenda", Selected = false, OptionCount = ChapterItems.Where(C => C.Type == "Agenda").ToList().Count() },
                                                    new CopyOption { Option = "Status", Selected = false, OptionCount = ChapterItems.Where(C => C.Type == "Status").ToList().Count() },
                                                    new CopyOption { Option = "Documents/Steps", Selected = false, OptionCount = ChapterItems.Where(C => lstDocTypes.Contains(C.Type)).ToList().Count() },
                                                    new CopyOption { Option = "Fees", Selected = false, OptionCount = ChapterItems.Where(C => C.Type == "Fee").ToList().Count() },
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
            var originalJson = new string(TaskObject.ChapterData);
            var SelectedCopyItems = new VmChapter { ChapterItems = new List<UsrOrDefChapterManagement>() };

            if (!(TaskObject.ChapterData is null))
            {
                SelectedCopyItems = JsonConvert.DeserializeObject<VmChapter>(TaskObject.ChapterData);
            }


            if (CopyOptions.Where(C => C.Option == "Agenda").Select(C => C.Selected).FirstOrDefault())
            {
                foreach (var item in SelectedCopyItems.ChapterItems.Where(C => C.Type == "Agenda").ToList())
                {
                    SelectedCopyItems.ChapterItems.Remove(item);
                }

                SelectedCopyItems.ChapterItems.AddRange(ChapterItems.Where(C => C.Type == "Agenda").ToList());
            }

            if (CopyOptions.Where(C => C.Option == "Status").Select(C => C.Selected).FirstOrDefault())
            {
                foreach (var item in SelectedCopyItems.ChapterItems.Where(C => C.Type == "Status").ToList())
                {
                    SelectedCopyItems.ChapterItems.Remove(item);
                }

                SelectedCopyItems.ChapterItems.AddRange(ChapterItems.Where(C => C.Type == "Status").ToList());
            }

            if (CopyOptions.Where(C => C.Option == "Documents/Steps").Select(C => C.Selected).FirstOrDefault())
            {
                foreach (var item in SelectedCopyItems.ChapterItems.Where(C => lstDocTypes.Contains(C.Type)).ToList())
                {
                    SelectedCopyItems.ChapterItems.Remove(item);
                }

                SelectedCopyItems.ChapterItems.AddRange(ChapterItems.Where(C => lstDocTypes.Contains(C.Type)).ToList());
            }

            if (CopyOptions.Where(C => C.Option == "Fees").Select(C => C.Selected).FirstOrDefault())
            {
                foreach (var item in SelectedCopyItems.ChapterItems.Where(C => C.Type == "Fee").ToList())
                {
                    SelectedCopyItems.ChapterItems.Remove(item);
                }

                SelectedCopyItems.ChapterItems.AddRange(ChapterItems.Where(C => C.Type == "Fee").ToList());
            }


            ImportedJSON = JsonConvert.SerializeObject(new VmChapter
            {
                CaseTypeGroup = TaskObject.CaseTypeGroup,
                CaseType = TaskObject.CaseType,
                Name = TaskObject.Name,
                SeqNo = TaskObject.SeqNo.GetValueOrDefault(),
                ChapterItems = SelectedCopyItems.ChapterItems
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

                    TaskObject.ChapterData = ImportedJSON;

                    ChapterFileUpload.DeleteFile(ListFileDescriptions.Where(F => F.FilePath.Contains(".xlsx")).FirstOrDefault().FilePath);

                    await chapterManagementService.Update(TaskObject);

                    DataChanged?.Invoke();
                    Close();
                }
            }
            
        }
    }
}
