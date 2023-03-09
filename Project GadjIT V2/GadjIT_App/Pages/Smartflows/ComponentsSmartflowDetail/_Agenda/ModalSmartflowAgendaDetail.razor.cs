using Blazored.Modal;
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
using GadjIT_ClientContext.Models.Smartflow;
using GadjIT_ClientContext.Models.P4W;
using GadjIT_ClientContext.Models.Smartflow.Client;

namespace GadjIT_App.Pages.Smartflows.ComponentsSmartflowDetail._Agenda
{

    public partial class ModalSmartflowAgendaDetail : ComponentBase
    {

        [CascadingParameter]
        BlazoredModalInstance ModalInstance { get; set; }


        [Parameter]
        public string _Option { get; set; }


        [Parameter]
        public SmartflowV2 _SelectedSmartflow { get; set; }

        [Parameter]
        public SmartflowAgenda _TaskObject { get; set; }

        [Parameter]
        public SmartflowAgenda _CopyObject { get; set; }

        [Parameter]
        public Action _DataChanged { get; set; } 
        
        [Inject]
        ILogger<ModalSmartflowAgendaDetail> Logger {get; set;}

        [Inject]
        INotificationManager NotificationManager {get; set;}

        [Inject]
        IUserSessionState UserSession { get; set; }


        
        public int Error { get; set; } = 0;

        public string FilterText { get; set; } = "";
        
      

        private async void Close()
        {
            _TaskObject = new SmartflowAgenda();
            await ModalInstance.CloseAsync();


        }

        private void HandleValidSubmit()
        {
            try
            {

                
                if (_SelectedSmartflow.Agendas.Where(A => A.Name != _CopyObject.Name).Select(A => A.Name).Contains(_CopyObject.Name))
                {
                    Error = 1;
                    StateHasChanged();
                }
                else
                {

                    _TaskObject.Name = _CopyObject.Name;
                    
                    if (_Option == "Insert")
                    {
                        _SelectedSmartflow.Agendas.Add(_TaskObject);
                    }

                    _TaskObject = new SmartflowAgenda();
                    FilterText = "";

                    _DataChanged.Invoke();
                    Close();
                    
                }

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
            using (LogContext.PushProperty("SourceContext", nameof(ModalSmartflowAgendaDetail)))
            {
                Logger.LogError(e,"Error - Method: {0}, Message: {1}",_method, _message);
            }

        }

    }

    
}
