using Blazored.Modal;
using GadjIT_ClientContext.P4W;
using GadjIT_App.Services;
using GadjIT_App.Services.SessionState;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Serilog.Context;
using Microsoft.Extensions.Logging;
using GadjIT_ClientContext.P4W.Custom;
using Newtonsoft.Json;

namespace GadjIT_App.Pages.Chapters.ComponentsCaseTypeDetail._Documents
{

    public partial class ModalLinkDocument : ComponentBase
    {

        [CascadingParameter]
        BlazoredModalInstance ModalInstance { get; set; }

        [Parameter]
        public UsrOrsfSmartflows _SelectedChapterObject { get; set; }

        [Parameter]
        public VmSmartflow _SelectedChapter { get; set; }


        [Parameter]
        public LinkedCaseItem _TaskObject { get; set; }
                
        [Parameter]
        public LinkedCaseItem _CopyObject { get; set; }


        [Parameter]
        public Action _DataChanged { get; set; }

                
        [Parameter]
        public List<DmDocuments> _LibraryDocumentsAndSteps { get; set; } = new List<DmDocuments>();
        
        [Parameter]
        public List<CaseTypeGroups> _P4WCaseTypeGroups { get; set; }

        
        [Inject]
        ILogger<ModalLinkDocument> Logger {get; set;}

        [Inject]
        INotificationManager NotificationManager {get; set;}

        [Inject]
        IChapterManagementService ChapterManagementService { get; set; }


        [Inject]
        IUserSessionState UserSession { get; set; }


        
        
        public int Error { get; set; } = 0;

        public string FilterText { get; set; } = "";
        
        private int SelectedCaseTypeGroup { get; set; } = -2;

        public bool _setDisplayName { get; set; } 

        public bool SetDisplayName
        {
            get { return _CopyObject.AltName == _TaskObject.ItemName ? true : false; }
            set
            {
                if (value)
                {
                    _CopyObject.AltName = _TaskObject.ItemName;
                }
                else
                {
                   _CopyObject.AltName = _TaskObject.AltName; 
                }
                _setDisplayName = value;
            }
        }


        
        protected override async Task OnInitializedAsync()
        {
            try
            {

                SelectedCaseTypeGroup = _P4WCaseTypeGroups.Where(CT => CT.Name == _SelectedChapter.P4WCaseTypeGroup).Select(CT => CT.Id).FirstOrDefault();

                if (_SelectedChapter.P4WCaseTypeGroup == "Global Documents")
                {
                    SelectedCaseTypeGroup = 0;
                }

                if (_SelectedChapter.P4WCaseTypeGroup == "Entity Documents")
                {
                    SelectedCaseTypeGroup = -1;
                }

            }
            catch(Exception e)
            {
                SelectedCaseTypeGroup = 0; 
                GenericErrorLog(false, e, "OnInitialized", e.Message);
            }

        }


        private async Task RefreshDocListOnModel()
        {
            try
            {
                _LibraryDocumentsAndSteps = await ChapterManagementService.GetDocumentList(_SelectedChapter.CaseType);
            
                
            }
            catch(Exception e)
            {
                _LibraryDocumentsAndSteps = new List<DmDocuments>();
                GenericErrorLog(false, e, "RefreshDocListOnModel", e.Message);
            }
        }

        private async void Close()
        {
            await ModalInstance.CloseAsync();
        }

        private async void HandleValidSubmit()
        {
            try
            {
                _CopyObject.ItemName = Regex.Replace(_CopyObject.ItemName, "[^0-9a-zA-Z-_ (){}!£$%^&*,./#?@<>`:]+", "");
                _CopyObject.AltName = Regex.Replace(_CopyObject.AltName, "[^0-9a-zA-Z-_ (){}!£$%^&*,./#?@<>`:]+", "");

                _TaskObject.ItemName = _CopyObject.ItemName;
                _TaskObject.AltName = _CopyObject.AltName;
                _TaskObject.IsItemLinked = true;

                await ModalInstance.CloseAsync();

                _DataChanged.Invoke();

                GenSmartflowItem docItem = _SelectedChapter.Items
                                            .Where(I => I.Type == "Doc")
                                            .Where(I => I.SeqNo == _TaskObject.OrigSeqNo)
                                            .FirstOrDefault();
                
                if(!_CopyObject.IsAttachment)
                {
                    docItem.Name = _CopyObject.ItemName;
                    docItem.AltDisplayName = _CopyObject.AltName;
                }
                else
                {
                    LinkedItem attachItem = docItem.LinkedItems
                                            .Where(LI => LI.DocName == _TaskObject.OrigItemName)
                                            .FirstOrDefault();

                    attachItem.DocName = _CopyObject.ItemName;
                    attachItem.DocAsName = _CopyObject.AltName;
                }

                _SelectedChapterObject.SmartflowData = JsonConvert.SerializeObject(_SelectedChapter);
                await ChapterManagementService.Update(_SelectedChapterObject);


            }
            catch(Exception e)
            {
                GenericErrorLog(true,e, "HandleValidSubmit", e.Message);

                await ModalInstance.CloseAsync();
            }

        }

        private void ResetError()
        {
            Error = 0;
            StateHasChanged();
        }

        /****************************************/
        /* ERROR HANDLING */
        /****************************************/
        private async void GenericErrorLog(bool showNotificationMsg, Exception e, string _method, string _message)
        {
            if(showNotificationMsg)
            {
                await NotificationManager.ShowNotification("Danger", $"Oops! Something went wrong.").ConfigureAwait(false);
            }
            
            using (LogContext.PushProperty("SourceSystem", UserSession.SelectedSystem))
            using (LogContext.PushProperty("SourceCompanyId", UserSession.Company.Id))
            using (LogContext.PushProperty("SourceUserId", UserSession.User.Id))
            using (LogContext.PushProperty("SourceContext", nameof(ModalLinkDocument)))
            {
                Logger.LogError(e,"Error - Method: {0}, Message: {1}",_method, _message);
            }

        }

    }

    
}
