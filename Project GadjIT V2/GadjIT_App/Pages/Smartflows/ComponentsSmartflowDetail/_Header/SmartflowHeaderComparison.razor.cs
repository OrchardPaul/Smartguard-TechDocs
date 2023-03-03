using Blazored.Modal;
using GadjIT_App.Services;
using GadjIT_App.Services.SessionState;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using System.Threading.Tasks;
using GadjIT_AppContext.GadjIT_App;
using GadjIT_ClientContext.Models.Smartflow.Client;
using GadjIT_ClientContext.Models.Smartflow;

namespace GadjIT_App.Pages.Smartflows.ComponentsSmartflowDetail._Header
{
    public partial class SmartflowHeaderComparison
    {
        [CascadingParameter]
        BlazoredModalInstance ModalInstance { get; set; }


        [Parameter]
        public Smartflow _SelectedSmartflow { get; set; }

        [Parameter]
        public Smartflow _Alt_Smartflow { get; set; }

        [Parameter]
        public App_SmartflowRecord _Alt_SmartflowRecord {get; set;} //as saved on Company

        

                

        [Inject]
        IClientApiManagementService ClientApiManagementService { get; set; }

        [Inject]
        public IUserSessionState UserSession { get; set; }



        public bool syncCaseTypeGroup { get; set; }
        public bool syncView { get; set; }
        public bool syncStepName { get; set; }
        public bool syncBgColour { get; set; }
        public bool syncBgImage { get; set; }
        public bool syncShowPartnerNotes { get; set; }
        public bool syncShowDocumentTracking { get; set; }

        public Client_SmartflowRecord Alt_ClientSmartflowRecord { get; set; }


        protected override void OnInitialized()
        {
            syncCaseTypeGroup = _SelectedSmartflow.P4WCaseTypeGroup == _Alt_Smartflow.P4WCaseTypeGroup ? false : true;
            syncView  = _SelectedSmartflow.SelectedView == _Alt_Smartflow.SelectedView ? false : true; 
            syncStepName  = _SelectedSmartflow.StepName == _Alt_Smartflow.StepName ? false : true;
            syncBgColour  = _SelectedSmartflow.BackgroundColour == _Alt_Smartflow.BackgroundColour ? false : true;
            syncBgImage  = _SelectedSmartflow.BackgroundImage == _Alt_Smartflow.BackgroundImage ? false : true;
            syncShowPartnerNotes = _SelectedSmartflow.ShowPartnerNotes == _Alt_Smartflow.ShowPartnerNotes ? false : true;
            syncShowDocumentTracking = _SelectedSmartflow.ShowDocumentTracking == _Alt_Smartflow.ShowDocumentTracking ? false : true;
        }

        private async void Close()
        {
            await ModalInstance.CloseAsync();
        }

        
        private async Task HandleValidSubmit()
        {

            if (syncCaseTypeGroup)
            {
                _Alt_Smartflow.P4WCaseTypeGroup = _SelectedSmartflow.P4WCaseTypeGroup;
            }
            if (syncView)
            {
                _Alt_Smartflow.SelectedView = _SelectedSmartflow.SelectedView;
            }
            if (syncStepName)
            {
                _Alt_Smartflow.StepName = _SelectedSmartflow.StepName;
            }
            if (syncBgColour)
            {
                _Alt_Smartflow.BackgroundColour = _SelectedSmartflow.BackgroundColour;
                _Alt_Smartflow.BackgroundColourName = _SelectedSmartflow.BackgroundColourName;
            }
            if (syncBgImage)
            {
                _Alt_Smartflow.BackgroundImage = _SelectedSmartflow.BackgroundImage;
                _Alt_Smartflow.BackgroundImageName = _SelectedSmartflow.BackgroundImageName;
            }
            if (syncShowPartnerNotes)
            {
                _Alt_Smartflow.ShowPartnerNotes = _SelectedSmartflow.ShowPartnerNotes;
            }
            if (syncShowDocumentTracking)
            {
                _Alt_Smartflow.ShowDocumentTracking = _SelectedSmartflow.ShowDocumentTracking;
            }


            Alt_ClientSmartflowRecord = new Client_SmartflowRecord {
                        Id = _Alt_SmartflowRecord.RowId
                        , SeqNo = _Alt_SmartflowRecord.SeqNo
                        , CaseTypeGroup = _Alt_SmartflowRecord.CaseTypeGroup
                        , CaseType = _Alt_SmartflowRecord.CaseType
                        , SmartflowName = _Alt_SmartflowRecord.SmartflowName
                        , SmartflowData = _Alt_SmartflowRecord.SmartflowData
                    };
    
            await UserSession.SwitchSelectedSystem();
            Alt_ClientSmartflowRecord.SmartflowData = JsonConvert.SerializeObject(_Alt_Smartflow);
            await ClientApiManagementService.Update(Alt_ClientSmartflowRecord);
            await UserSession.ResetSelectedSystem();
            
            await ModalInstance.CloseAsync();

        }


    }
}
