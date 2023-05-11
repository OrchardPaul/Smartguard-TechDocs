using Blazored.Modal;
using GadjIT_App.Data.Admin;
using GadjIT_App.Services;
using GadjIT_App.Services.AppState;
using GadjIT_App.Services.SessionState;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Serilog.Context;
using Microsoft.Extensions.Logging;
using GadjIT_ClientContext.Models.P4W;
using GadjIT_ClientContext.Models.Smartflow;
using GadjIT_ClientContext.Models.Smartflow.Client;
using AutoMapper;


namespace GadjIT_App.Pages.Smartflows.ComponentsSmartflowDetail._Documents
{
    public partial class ModalSmartflowAttachments
    {
        

        [CascadingParameter]
        BlazoredModalInstance ModalInstance { get; set; }

        [Inject]
        ILogger<ModalSmartflowAttachments> Logger {get; set;}

        [Inject]
        INotificationManager NotificationManager {get; set;}

        [Inject]
        IClientApiManagementService ClientApiManagementService { get; set; }

        [Inject]
        IAppSmartflowsState AppSmartflowsState { get; set; }

        [Inject]
        public ICompanyDbAccess CompanyDbAccess { get; set; }

        [Inject]
        public IUserSessionState UserSession { get; set; }

        [Inject]
        IMapper Mapper {get; set;}

        [Parameter]
        public List<P4W_TableDate> _TableDates { get; set; }


        [Parameter]
        public Client_SmartflowRecord _Selected_ClientSmartflowRecord { get; set; }

        [Parameter]
        public SmartflowV2 _SelectedSmartflow { get; set; }

        [Parameter]
        public SmartflowDocument _SelectedDocument { get; set; }



        [Parameter]
        public Action _DataChanged { get; set; }

        [Parameter]
        public Action _RefreshLibraryDocumentsAndSteps { get; set; } 

        [Parameter]
        public List<P4W_DmDocuments> _LibraryDocumentsAndSteps { get; set; }

        [Parameter]
        public List<P4W_CaseTypeGroups> _P4WCaseTypeGroups { get; set; }

        [Parameter]
        public List<VmSmartflowStatus> _ListOfStatus { get; set; }
        
        [Parameter]
        public List<VmSmartflowAgenda> _ListOfAgenda { get; set; }


        protected string CurrentAction {get; set; } = "List";

        private int SelectedCaseTypeGroup { get; set; } = -1;

        public string FilterText { get; set; } = "";
        
        public string FilterTextDataItem { get; set; } = "";

        protected LinkedItem Attachment { get; set; }
        protected LinkedItem CopyAttachment { get; set; }
        
        private int RowChanged = 0;

        public bool SeqMoving {get; set;}


        protected bool DataChanged = false;
         

        public bool UseCustomItem 
        { get { return Attachment.CustomItem == "Y" ? true : false; }
            set {
                if (value)
                {
                    Attachment.CustomItem = "Y";
                    Attachment.Agenda = "";
                    Attachment.DocAsName = "";
                }
                else
                {
                    Attachment.CustomItem = "N";
                }
            }
        }

        public bool OptionalDocument
        {
            get { return (Attachment.OptionalDocument == "Y" ? true : false); }
            set
            {
                if (value)
                {
                    Attachment.OptionalDocument = "Y";
                }
                else
                {
                    Attachment.OptionalDocument = "N";
                }
            }
        }


        List<string> ActionList = new List<string>() { "TAKE", "INSERT", "SCHEDULE" };
        
        List<string> TrackMethodList = new List<string>() { "N/A", "Send Only", "Response Required" };

        public List<string> DocumentList;

        public bool _useCustomReschedule { get; set; }
        public bool UseCustomReschedule
        {
            get { return _useCustomReschedule; }
            set
            {
                Attachment.ScheduleDataItem = !value ? "" : Attachment.ScheduleDataItem;
                _useCustomReschedule = value;
            }
        }



        protected override async Task OnInitializedAsync()
        {
            
            if (!(string.IsNullOrEmpty(_SelectedSmartflow.P4WCaseTypeGroup)) && (_SelectedSmartflow.P4WCaseTypeGroup != "Select"))
            {
                SelectedCaseTypeGroup = _P4WCaseTypeGroups.Where(CT => CT.Name == _SelectedSmartflow.P4WCaseTypeGroup).Select(CT => CT.Id).FirstOrDefault();


                if (_SelectedSmartflow.P4WCaseTypeGroup == "Entity Documents")
                {
                    SelectedCaseTypeGroup = -1;
                }

                if (_SelectedSmartflow.P4WCaseTypeGroup == "Global Documents")
                {
                    SelectedCaseTypeGroup = 0;
                }
            }

            if(_SelectedDocument.LinkedItems.Where(C => C.SeqNo != _SelectedDocument.LinkedItems.IndexOf(C) + 1).Count() > 0) //If any SeqNos are out of sequence
            { 
                await ReSequence();
            }

            
        }


        private async void RefreshDocListOnModel()
        {
            try
            {
                //refresh the dm_documents list here so it is immediately available when displaying the Attachments list
                _LibraryDocumentsAndSteps = await ClientApiManagementService.GetDocumentList(_SelectedSmartflow.CaseType);
                
                _RefreshLibraryDocumentsAndSteps.Invoke();

                CurrentAction = "List";

                await InvokeAsync(() =>
                {
                    StateHasChanged();
                });
            
            }
            catch(Exception e)
            {
                _LibraryDocumentsAndSteps = new List<P4W_DmDocuments>();
                GenericErrorLog(false, e, "RefreshDocListOnModel", e.Message);
            }
        }

        private void CancelUpdate()
        {
            CurrentAction = "List";
        }

        private void PrepareAttachmentForEdit(LinkedItem _attachment)
        {
            Attachment = _attachment;

            CopyAttachment = new LinkedItem();
            Mapper.Map(Attachment, CopyAttachment);

            //Check if document exists in P4W Library
            if (_LibraryDocumentsAndSteps.ToList() is null
                    && Attachment.DocName != ""
                    && !(Attachment.DocName is null)
                    && !_LibraryDocumentsAndSteps.ToList().Select(D => D.Name).Contains(Attachment.DocName))
            {
                //Match found
                UseCustomItem = false;
            }

            if (!string.IsNullOrEmpty(Attachment.ScheduleDataItem)
                && _TableDates.ToList().Select(D => D.TableField).Contains(Attachment.ScheduleDataItem))
            {
                UseCustomReschedule = true;
            }
            else
            {
                UseCustomReschedule = false;
            }

            CurrentAction = "Edit";
        }

        private void PrepareAttachmentForAdd()
        {
            Attachment = new LinkedItem();
            
            Attachment.Action = "INSERT";
            
            CurrentAction = "Add";
        }

        private async void Close()
        {
            if(DataChanged)
            {
                _DataChanged?.Invoke();
            }

            await ModalInstance.CloseAsync();


        }

        private void HandleValidSubmit()
        {
            try
            {
                Dictionary<int?, string> docTypes = new Dictionary<int?, string> { { 1, "Doc" }, { 4, "Form" }, { 6, "Step" }, { 8, "Date" }, { 9, "Email" }, { 11, "Doc" }, { 12, "Email" }, { 13, "Csv" } };

                Attachment.DocType = _LibraryDocumentsAndSteps.Where(D => D.Name.ToUpper() == Attachment.DocName.ToUpper())
                                                                                            .Select(D => docTypes.ContainsKey(D.DocumentType) ? docTypes[D.DocumentType] : "Doc")
                                                                                            .FirstOrDefault();

                if (_SelectedDocument.LinkedItems is null)
                {
                    _SelectedDocument.LinkedItems = new List<LinkedItem> { Attachment };
                }
                else
                {
                    if (CurrentAction != "Edit")
                    {
                        _SelectedDocument.LinkedItems.Add(Attachment);
                    }
                }


                _Selected_ClientSmartflowRecord.SmartflowData = JsonConvert.SerializeObject(_SelectedSmartflow);
                ClientApiManagementService.Update(_Selected_ClientSmartflowRecord).ConfigureAwait(false);

                FilterText = "";

                RefreshDocListOnModel();

                DataChanged = true;

            }
            catch(Exception e)
            {
                GenericErrorLog(true,e, "HandleValidSubmit", e.Message);
            }
            

        }

        private async void RemoveAttachment()
        {
            await RemoveAttachmentTask();
        }

        private async Task RemoveAttachmentTask()
        {
            try
            {
                _SelectedDocument.LinkedItems.Remove(Attachment);

                await SaveUpdatedSmartflow();
                
                FilterText = "";

                RefreshDocListOnModel();

            }
            catch(Exception e)
            {
                GenericErrorLog(true,e, "RemoveAttachmentTask", e.Message);
            }
            
        }

        private async Task SaveUpdatedSmartflow()
        {
            _Selected_ClientSmartflowRecord.SmartflowData = JsonConvert.SerializeObject(_SelectedSmartflow);
            await ClientApiManagementService.Update(_Selected_ClientSmartflowRecord).ConfigureAwait(false);

        }

        

        /****************************************/
        /* LIST ORDERING                        */
        /****************************************/
        public async Task ReSequence(int _seq)
        {
            RowChanged = _seq;

            await ReSequence();
        }

        public async Task ReSequence()
        {
            _SelectedDocument.LinkedItems.Select(C => { C.SeqNo = _SelectedDocument.LinkedItems.IndexOf(C) + 1; return C; }).ToList();

            await SaveUpdatedSmartflow();
        }

        public void ResetRowChanged() 
        {
            RowChanged = 0;
            SeqMoving = false;

            StateHasChanged();

        }  

        /****************************************/
        /* ERROR HANDLING */
        /****************************************/
        private void GenericErrorLog(bool showNotificationMsg, Exception e, string _method, string _message)
        {
            using (LogContext.PushProperty("SourceSystem", UserSession.SelectedSystem))
            using (LogContext.PushProperty("SourceCompanyId", UserSession.Company.Id))
            using (LogContext.PushProperty("SourceUserId", UserSession.User.Id))
            using (LogContext.PushProperty("SourceContext", nameof(ModalSmartflowAttachments)))
            {
                Logger.LogError(e,"Error - Method: {0}, Message: {1}",_method, _message);
            }

            if(showNotificationMsg)
            {
                NotificationManager.ShowNotification("Danger", $"Oops! Something went wrong.");
            }
        }
    }
}
