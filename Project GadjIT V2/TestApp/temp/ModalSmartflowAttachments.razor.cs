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

        public LinkedItem CopyAttachment { get; set; }

        [Inject]
        IMapper Mapper {get; set;}




        [Parameter]
        public LinkedItem _Attachment { get; set; }

        [Parameter]
        public RenderFragment _CustomHeader { get; set; }

        [Parameter]
        public string _SelectedList { get; set; }

        [Parameter]
        public List<P4W_TableDate> _TableDates { get; set; }

        [Parameter]
        public string _Option { get; set; }

        [Parameter]
        public Client_SmartflowRecord _Selected_ClientSmartflowRecord { get; set; }

        [Parameter]
        public SmartflowV2 _SelectedSmartflow { get; set; }

        [Parameter]
        public SmartflowDocument _TaskObject { get; set; }

        [Parameter]
        public SmartflowDocument _CopyObject { get; set; }


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



        private int SelectedCaseTypeGroup { get; set; } = -1;

        public string FilterText { get; set; } = "";
        
        public string FilterTextDataItem { get; set; } = "";

        public bool UseCustomItem 
        { get { return _Attachment.CustomItem == "Y" ? true : false; }
            set {
                if (value)
                {
                    _Attachment.CustomItem = "Y";
                    _Attachment.Agenda = "";
                    _Attachment.DocAsName = "";
                }
                else
                {
                    _Attachment.CustomItem = "N";
                }
            }
        }

        public bool OptionalDocument
        {
            get { return (_Attachment.OptionalDocument == "Y" ? true : false); }
            set
            {
                if (value)
                {
                    _Attachment.OptionalDocument = "Y";
                }
                else
                {
                    _Attachment.OptionalDocument = "N";
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
                _Attachment.ScheduleDataItem = !value ? "" : _Attachment.ScheduleDataItem;
                _useCustomReschedule = value;
            }
        }



        protected override async Task OnInitializedAsync()
        {
            CopyAttachment = new LinkedItem();
            Mapper.Map(_Attachment, CopyAttachment);

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

            //Check if document exists in P4W Library
            if (_LibraryDocumentsAndSteps.ToList() is null
                    && _Attachment.DocName != ""
                    && !(_Attachment.DocName is null)
                    && !_LibraryDocumentsAndSteps.ToList().Select(D => D.Name).Contains(_Attachment.DocName))
            {
                //Match found
                UseCustomItem = false;
            }

            if (!string.IsNullOrEmpty(_Attachment.ScheduleDataItem)
                && _TableDates.ToList().Select(D => D.TableField).Contains(_Attachment.ScheduleDataItem))
            {
                UseCustomReschedule = true;
            }
            else
            {
                UseCustomReschedule = false;
            }
        }


        private async void RefreshDocListOnModel()
        {
            try
            {
                _LibraryDocumentsAndSteps = await ClientApiManagementService.GetDocumentList(_SelectedSmartflow.CaseType);
                StateHasChanged();

                _RefreshLibraryDocumentsAndSteps.Invoke();
            
            }
            catch(Exception e)
            {
                _LibraryDocumentsAndSteps = new List<P4W_DmDocuments>();
                GenericErrorLog(false, e, "RefreshDocListOnModel", e.Message);
            }
        }

        private async void Close()
        {
            _TaskObject = new SmartflowDocument();
            
            //Mapper.Map(CopyAttachment, _Attachment);  //reset _Attachement

            await ModalInstance.CloseAsync();


        }

        private void HandleValidSubmit()
        {
            try
            {
                Dictionary<int?, string> docTypes = new Dictionary<int?, string> { { 1, "Doc" }, { 4, "Form" }, { 6, "Step" }, { 8, "Date" }, { 9, "Email" }, { 11, "Doc" }, { 12, "Email" }, { 13, "Csv" } };

                _Attachment.DocType = _LibraryDocumentsAndSteps.Where(D => D.Name.ToUpper() == _Attachment.DocName.ToUpper())
                                                                                            .Select(D => docTypes.ContainsKey(D.DocumentType) ? docTypes[D.DocumentType] : "Doc")
                                                                                            .FirstOrDefault();

                if (_CopyObject.LinkedItems is null)
                {
                    _CopyObject.LinkedItems = new List<LinkedItem> { _Attachment };
                }
                else
                {
                    if (_SelectedList != "Edit Attachement")
                    {
                        _CopyObject.LinkedItems.Add(_Attachment);
                    }
                }


                _TaskObject.Name = _CopyObject.Name;
                _TaskObject.EntityType = _CopyObject.EntityType;
                _TaskObject.SeqNo = _CopyObject.SeqNo;
                _TaskObject.AsName = _CopyObject.AsName;
                _TaskObject.RescheduleDays = _CopyObject.RescheduleDays;
                _TaskObject.AltDisplayName = _CopyObject.AltDisplayName;
                _TaskObject.UserMessage = _CopyObject.UserMessage;
                _TaskObject.PopupAlert = _CopyObject.PopupAlert;
                _TaskObject.NextStatus = _CopyObject.NextStatus;

                _TaskObject.LinkedItems = _CopyObject.LinkedItems;

                _Selected_ClientSmartflowRecord.SmartflowData = JsonConvert.SerializeObject(_SelectedSmartflow);
                ClientApiManagementService.Update(_Selected_ClientSmartflowRecord).ConfigureAwait(false);

                _TaskObject = new SmartflowDocument();
                FilterText = "";


                _DataChanged?.Invoke();
            }
            catch(Exception e)
            {
                GenericErrorLog(true,e, "HandleValidSubmit", e.Message);
            }
            finally
            {
                Close();
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
                if (!(_CopyObject.LinkedItems is null))
                {
                    if (_CopyObject.LinkedItems.Select(F => F.DocName).ToList().Contains(_Attachment.DocName))
                    {
                        var updateItem = _CopyObject.LinkedItems.Where(F => F.DocName == _Attachment.DocName).FirstOrDefault();

                        _CopyObject.LinkedItems.Remove(updateItem);
                    }
                }


                _TaskObject.LinkedItems = _CopyObject.LinkedItems;

                _Selected_ClientSmartflowRecord.SmartflowData = JsonConvert.SerializeObject(_SelectedSmartflow);
                await ClientApiManagementService.Update(_Selected_ClientSmartflowRecord).ConfigureAwait(false);

                await CompanyDbAccess.SaveSmartFlowRecord(_Selected_ClientSmartflowRecord, UserSession);

                _TaskObject = new SmartflowDocument();
                FilterText = "";


                _DataChanged?.Invoke();
            }
            catch(Exception e)
            {
                GenericErrorLog(true,e, "RemoveAttachmentTask", e.Message);
            }
            finally
            {
                Close();
            }

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
