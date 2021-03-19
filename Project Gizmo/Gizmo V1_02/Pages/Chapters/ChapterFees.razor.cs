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
    public partial class ChapterFees
    {
        public bool VATable
        {
            get { return CopyObject.VATable == "Y" ? true : false; }
            set 
            {
                if (value)
                {
                    CopyObject.VATable = "Y";
                }
                else
                {
                    CopyObject.VATable = "N";
                }
            }
        }


        [CascadingParameter]
        BlazoredModalInstance ModalInstance { get; set; }

        [Inject]
        IChapterManagementService chapterManagementService { get; set; }

        [Parameter]
        public string Option { get; set; }

        [Parameter]
        public UsrOrDefChapterManagement SelectedChapterObject { get; set; }

        [Parameter]
        public VmChapter SelectedChapter { get; set; }

        [Parameter]
        public Fee TaskObject { get; set; }

        [Parameter]
        public Fee CopyObject { get; set; }

        [Parameter]
        public Action DataChanged { get; set; }

        private int selectedCaseTypeGroup { get; set; } = -1;

        private List<string> PostingTypes { get; set; } = new List<string> 
                                                                            {
                                                                                "DSO"
                                                                                ,"DSP"
                                                                                ,"WOD"
                                                                                ,"O/N"
                                                                                ,"OCR"
                                                                                ,"OCR"
                                                                                ,"OCP"
                                                                                ,"OTO"
                                                                                ,"OTC"
                                                                                ,"OCR"
                                                                                ,"CDR"
                                                                                ,"CCR"
                                                                                ,"CIN"
                                                                                ,"CTO"
                                                                                ,"CTC"
                                                                                ,"CTD"
                                                                                ,"DFD"
                                                                                ,"DOD"
                                                                            };

        private List<string> FeeCategories { get; set; } = new List<string>
                                                                            {
                                                                                "Disbursement"
                                                                                ,"Additional Fee"
                                                                                ,"Our Fee"
                                                                                ,"Search Fee"
                                                                                ,"Referral Fee"
                                                                                ,"Other"
                                                                            };

        private async void Close()
        {
            TaskObject = new Fee();
            await ModalInstance.CloseAsync();


        }

        private async void HandleValidSubmit()
        {
            TaskObject.FeeName = CopyObject.FeeName;
            TaskObject.FeeCategory = CopyObject.FeeCategory;
            TaskObject.SeqNo = CopyObject.SeqNo;
            TaskObject.Amount = CopyObject.Amount;
            TaskObject.VATable = CopyObject.VATable;
            TaskObject.PostingType = CopyObject.PostingType;
            
            if (Option == "Insert")
            {
                SelectedChapter.Fees.Add(TaskObject);
            }

            SelectedChapterObject.ChapterData = JsonConvert.SerializeObject(SelectedChapter);
            await chapterManagementService.Update(SelectedChapterObject).ConfigureAwait(false);

            TaskObject = new Fee();

            DataChanged?.Invoke();
            Close();

        }

    }
}
