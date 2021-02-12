using Blazored.Modal;
using GadjIT.ClientContext.P4W;
using GadjIT.ClientContext.P4W.Custom;
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
    public partial class ChapterCopy
    {
        public class CopyOption
        {
            public string Option { get; set; }
            public bool Selected { get; set; }
        }

        [CascadingParameter]
        BlazoredModalInstance ModalInstance { get; set; }

        [Inject]
        IChapterManagementService chapterManagementService { get; set; }

        [Parameter]
        public VmUsrOrDefChapterManagement TaskObject { get; set; }

        [Parameter]
        public Action DataChanged { get; set; }

        [Parameter]
        public List<VmUsrOrDefChapterManagement> AllChapters { get; set; }

        [Parameter]
        public VmChapter currentChapter { get; set; }

        [Parameter]
        public bool addNewCaseTypeGroupOption { get; set; } = false;

        [Parameter]
        public bool addNewCaseTypeOption { get; set; } = false;

        public bool AddNewChapterOption { get; set; }

        public List<CopyOption> CopyOptions { get; set; } = new List<CopyOption>
                                                                    {
                                                                        new CopyOption { Option = "Agenda", Selected = false },
                                                                        new CopyOption { Option = "Status", Selected = false },
                                                                        new CopyOption { Option = "Documents/Steps", Selected = false },
                                                                        new CopyOption { Option = "Fees", Selected = false },
                                                                    };

        public List<string> lstDocTypes { get; set; } = new List<string> { "Doc", "Letter", "Form", "Email", "Step" };

        private void SelectExistingChapter(int chapterId)
        {
            TaskObject = AllChapters.Where(C => C.ChapterObject.Id == chapterId).SingleOrDefault();

            StateHasChanged();
        }

        private void SelectExistingCaseTypeGroup(string caseTypeGroup)
        {
            TaskObject.ChapterObject.CaseTypeGroup = caseTypeGroup;
            TaskObject.ChapterObject.CaseType = AllChapters
                                                        .Where(C => C.ChapterObject.CaseTypeGroup == TaskObject.ChapterObject.CaseTypeGroup)
                                                        .Select(C => C.ChapterObject.CaseType)
                                                        .FirstOrDefault();
            TaskObject.ChapterObject = AllChapters
                                                        .Where(C => C.ChapterObject.CaseTypeGroup == TaskObject.ChapterObject.CaseTypeGroup)
                                                        .Where(C => C.ChapterObject.CaseType == TaskObject.ChapterObject.CaseType)
                                                        .Select(C => C.ChapterObject)
                                                        .FirstOrDefault();

            StateHasChanged();
        }

        private void SelectExistingCaseType(string caseType)
        {
            TaskObject.ChapterObject.CaseType = caseType;
            TaskObject.ChapterObject = AllChapters
                                                .Where(C => C.ChapterObject.CaseTypeGroup == TaskObject.ChapterObject.CaseTypeGroup)
                                                .Where(C => C.ChapterObject.CaseType == TaskObject.ChapterObject.CaseType)
                                                .Select(C => C.ChapterObject)
                                                .FirstOrDefault();

            StateHasChanged();
        }

        private void ResetChapter()
        {
            AddNewChapterOption = !AddNewChapterOption;

            if (!AddNewChapterOption)
            {
                TaskObject = new VmUsrOrDefChapterManagement
                {
                    ChapterObject = new UsrOrDefChapterManagement
                    {
                        CaseType = TaskObject.ChapterObject.CaseType,
                        CaseTypeGroup = TaskObject.ChapterObject.CaseTypeGroup,
                        Type = TaskObject.ChapterObject.Type,
                        ParentId = TaskObject.ChapterObject.ParentId,
                        SeqNo = TaskObject.ChapterObject.SeqNo
                    }
                };
            }
        }


        private async void Close()
        {
            await ModalInstance.CloseAsync();
        }

        private async void HandleValidSubmit()
        {
            var selectedCopyItems = new VmChapter { ChapterItems = new List<UsrOrDefChapterManagement>() };

            if (!(TaskObject.ChapterObject.ChapterData is null))
            {
                selectedCopyItems = JsonConvert.DeserializeObject<VmChapter>(TaskObject.ChapterObject.ChapterData);
            }


            if(CopyOptions.Where(C => C.Option == "Agenda").Select(C => C.Selected).FirstOrDefault())
            {
                foreach(var item in selectedCopyItems.ChapterItems.Where(C => C.Type == "Agenda").ToList())
                {
                    selectedCopyItems.ChapterItems.Remove(item);
                }

                selectedCopyItems.ChapterItems.AddRange(currentChapter.ChapterItems.Where(C => C.Type == "Agenda").ToList());
            }

            if (CopyOptions.Where(C => C.Option == "Status").Select(C => C.Selected).FirstOrDefault())
            {
                foreach (var item in selectedCopyItems.ChapterItems.Where(C => C.Type == "Status").ToList())
                {
                    selectedCopyItems.ChapterItems.Remove(item);
                }

                selectedCopyItems.ChapterItems.AddRange(currentChapter.ChapterItems.Where(C => C.Type == "Status").ToList());
            }

            if (CopyOptions.Where(C => C.Option == "Documents/Steps").Select(C => C.Selected).FirstOrDefault())
            {
                foreach (var item in selectedCopyItems.ChapterItems.Where(C => lstDocTypes.Contains(C.Type)).ToList())
                {
                    selectedCopyItems.ChapterItems.Remove(item);
                }

                selectedCopyItems.ChapterItems.AddRange(currentChapter.ChapterItems.Where(C => lstDocTypes.Contains(C.Type)).ToList());
            }

            if (CopyOptions.Where(C => C.Option == "Fees").Select(C => C.Selected).FirstOrDefault())
            {
                foreach (var item in selectedCopyItems.ChapterItems.Where(C => C.Type == "Fee").ToList())
                {
                    selectedCopyItems.ChapterItems.Remove(item);
                }

                selectedCopyItems.ChapterItems.AddRange(currentChapter.ChapterItems.Where(C => C.Type == "Fee").ToList());
            }

            TaskObject.ChapterObject.ChapterData = JsonConvert.SerializeObject(new VmChapter
            {
                CaseTypeGroup = TaskObject.ChapterObject.CaseTypeGroup,
                CaseType = TaskObject.ChapterObject.CaseType,
                Name = TaskObject.ChapterObject.Name,
                SeqNo = TaskObject.ChapterObject.SeqNo.GetValueOrDefault(),
                ChapterItems = selectedCopyItems.ChapterItems
            });

            if (TaskObject.ChapterObject.Id == 0)
            {
                await chapterManagementService.Add(TaskObject.ChapterObject);
            }
            else
            {
                await chapterManagementService.Update(TaskObject.ChapterObject);
            }

            DataChanged?.Invoke();
            Close();
        }
    }
}
