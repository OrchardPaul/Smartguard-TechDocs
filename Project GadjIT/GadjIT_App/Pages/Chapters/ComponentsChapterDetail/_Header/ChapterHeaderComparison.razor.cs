using Blazored.Modal;
using GadjIT_ClientContext.P4W;
using GadjIT_ClientContext.P4W.Custom;
using GadjIT_App.Data.Admin;
using GadjIT_App.Services;
using GadjIT_App.Services.SessionState;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GadjIT_AppContext.GadjIT_App;

namespace GadjIT_App.Pages.Chapters.ComponentsChapterDetail._Header
{
    public partial class ChapterHeaderComparison
    {
        [CascadingParameter]
        BlazoredModalInstance ModalInstance { get; set; }


        [Parameter]
        public VmChapter _SelectedChapter { get; set; }

        [Parameter]
        public VmChapter _AltChapter { get; set; }

        [Parameter]
        public SmartflowRecords _AltChapterRecord {get; set;} //as saved on Company

        

                

        [Inject]
        IChapterManagementService ChapterManagementService { get; set; }

        [Inject]
        public IUserSessionState UserSession { get; set; }



        public bool syncCaseTypeGroup { get; set; }
        public bool syncView { get; set; }
        public bool syncStepName { get; set; }
        public bool syncBgColour { get; set; }
        public bool syncBgImage { get; set; }
        public bool syncShowPartnerNotes { get; set; }
        public bool syncShowDocumentTracking { get; set; }

        public UsrOrsfSmartflows AltChapterObject { get; set; }


        protected override void OnInitialized()
        {
            syncCaseTypeGroup = _SelectedChapter.P4WCaseTypeGroup == _AltChapter.P4WCaseTypeGroup ? false : true;
            syncView  = _SelectedChapter.SelectedView == _AltChapter.SelectedView ? false : true; 
            syncStepName  = _SelectedChapter.StepName == _AltChapter.StepName ? false : true;
            syncBgColour  = _SelectedChapter.BackgroundColour == _AltChapter.BackgroundColour ? false : true;
            syncBgImage  = _SelectedChapter.BackgroundImage == _AltChapter.BackgroundImage ? false : true;
            syncShowPartnerNotes = _SelectedChapter.ShowPartnerNotes == _AltChapter.ShowPartnerNotes ? false : true;
            syncShowDocumentTracking = _SelectedChapter.ShowDocumentTracking == _AltChapter.ShowDocumentTracking ? false : true;
        }

        private async void Close()
        {
            await ModalInstance.CloseAsync();
        }

        
        private async Task HandleValidSubmit()
        {

            if (syncCaseTypeGroup)
            {
                _AltChapter.P4WCaseTypeGroup = _SelectedChapter.P4WCaseTypeGroup;
            }
            if (syncView)
            {
                _AltChapter.SelectedView = _SelectedChapter.SelectedView;
            }
            if (syncStepName)
            {
                _AltChapter.StepName = _SelectedChapter.StepName;
            }
            if (syncBgColour)
            {
                _AltChapter.BackgroundColour = _SelectedChapter.BackgroundColour;
                _AltChapter.BackgroundColourName = _SelectedChapter.BackgroundColourName;
            }
            if (syncBgImage)
            {
                _AltChapter.BackgroundImage = _SelectedChapter.BackgroundImage;
                _AltChapter.BackgroundImageName = _SelectedChapter.BackgroundImageName;
            }
            if (syncShowPartnerNotes)
            {
                _AltChapter.ShowPartnerNotes = _SelectedChapter.ShowPartnerNotes;
            }
            if (syncShowDocumentTracking)
            {
                _AltChapter.ShowDocumentTracking = _SelectedChapter.ShowDocumentTracking;
            }


            AltChapterObject = new UsrOrsfSmartflows {
                        Id = _AltChapterRecord.RowId
                        , SeqNo = _AltChapterRecord.SeqNo
                        , CaseTypeGroup = _AltChapterRecord.CaseTypeGroup
                        , CaseType = _AltChapterRecord.CaseType
                        , SmartflowName = _AltChapterRecord.SmartflowName
                        , SmartflowData = _AltChapterRecord.SmartflowData
                    };
    
            await UserSession.SwitchSelectedSystem();
            AltChapterObject.SmartflowData = JsonConvert.SerializeObject(_AltChapter);
            await ChapterManagementService.Update(AltChapterObject);
            await UserSession.ResetSelectedSystem();
            
            await ModalInstance.CloseAsync();

        }


    }
}
