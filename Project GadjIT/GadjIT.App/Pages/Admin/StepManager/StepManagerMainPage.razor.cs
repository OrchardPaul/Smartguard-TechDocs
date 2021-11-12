using GadjIT.ClientContext.P4W;
using GadjIT_App.Data.CustomFormObjects;
using GadjIT_App.Services;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GadjIT_App.Pages.Admin.StepManager
{
    public class StepObject
    {
        public bool IsSelected { get; set; } = false;

        public DmDocuments Step { get; set; }
    }

    public partial class StepManagerMainPage
    {



        [Inject]
        private IChapterManagementService chapterManagementService { get; set; }

        [Inject]
        private IPartnerAccessService partnerAccessService { get; set; }

        private string SelectedCaseType { get; set; } = "";
        private int SelectedCaseTypeCode { get; set; } = 0;

        private int uSelectedCaseType { get { return SelectedCaseTypeCode; } 
            set {
                SelectedCaseTypeCode = value;
                SelectedCaseType = PartnerCaseTypes.Where(P => P.Id == value).Select(P => P.Name).FirstOrDefault();
                
                LoadSteps();
            } }

        public List<CaseTypeGroups> PartnerCaseTypes { get; set; } = new List<CaseTypeGroups>();

        public List<DmDocuments> DropDownChapterList { get; set; } = new List<DmDocuments>();

        public List<StepObject> StepObjects { get; set; } = new List<StepObject>();

        public int PageNumber { get; set; } = 1;

        protected override async Task OnInitializedAsync()
        {
            PartnerCaseTypes = await partnerAccessService.GetPartnerCaseTypeGroups();
            DropDownChapterList = await chapterManagementService.GetDocumentList("Case");
        }

        protected void LoadSteps()
        {
            StepObjects = DropDownChapterList
                            .Select(D => new StepObject
                            {
                                IsSelected = false
                                ,
                                Step = D
                            })
                            .Where(D => D.Step.CaseTypeGroupRef == SelectedCaseTypeCode)
                            .ToList();

            StateHasChanged();
        }

        protected void ChangePage(int pageNumber)
        {
            PageNumber = pageNumber;
            StateHasChanged();
        }


    }
}
