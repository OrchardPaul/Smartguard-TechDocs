using GadjIT_ClientContext.P4W;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GadjIT_App.Services;
using GadjIT_App.Services.SessionState;
using Blazored.Modal.Services;
using Blazored.Modal;
using GadjIT_App.Pages._Shared.Modals;
using GadjIT_ClientContext.P4W.Custom;
using Newtonsoft.Json;
using Microsoft.JSInterop;
using GadjIT_App.Data.Admin;
using System.Globalization;
using System.Timers;
using GadjIT_App.Services.AppState;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using GadjIT_AppContext.GadjIT_App;


namespace GadjIT_App.Pages.Chapters
{
    public partial class CaseTypeDetail
    {
        [Parameter]
        public List<VmUsrOrsfSmartflows> _LstChapters { get; set; }

        [Parameter]
        public string _SelectedCaseTypeGroup { get; set; }

        [Parameter]
        public string _SelectedCaseType { get; set; }

        [Parameter]
        public EventCallback _SelectHome {get; set;}

        [Parameter]
        public EventCallback _RefreshChapters {get; set;}

        [Parameter]
        public EventCallback<UsrOrsfSmartflows> _SelectChapter {get; set;}

        [Parameter]
        public EventCallback _SaveSelectedCaseType {get; set;}

        [Inject]
        private IJSRuntime JSRuntime {get; set;}

        [Inject]
        public INotificationManager NotificationManager {get; set;}



        [Inject]
        public IUserSessionState UserSession { get; set; }


        [Inject]
        public IAppChapterState AppChapterState { get; set; }


        [Inject]
        private ILogger<ChapterList> Logger { get; set; }

        [Inject]
        public IConfiguration Configuration { get; set;}

        
        

        public String UserGuideURL => Configuration["AppSettings:UserGuideURL"];
        
        


        public VmSmartflow SelectedChapter { get; set; } = new VmSmartflow { Items = new List<GenSmartflowItem>() }; //SmartflowData

        int RowChanged { get; set; } = 0; //moved partial


        public string NavDisplay = "Chapter";


        protected bool CompareSystems = false;

        public bool DisplaySpinner = true;







#region Page Events
        protected override async Task OnInitializedAsync()
        {
            NavDisplay = "CaseType";

            await SelectCaseType();

        }

        private Timer timer;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)
            {
                try
                {
                    await JSRuntime.InvokeVoidAsync("showPageAfterFirstRender");
                }
                catch (Exception e)
                {
                    GenericErrorLog(false,e, "OnAfterRenderAsync", e.Message);  
                }
            }
            else
            {
                //TODO: reinstate after testing
                // timer = new Timer();
                // timer.Interval = 10000; //10 seconds
                // timer.Elapsed += OnTimerInterval;
                // timer.AutoReset = true;
                // // Start the timer
                // timer.Enabled = true;
            }
            base.OnAfterRender(firstRender);
        }



        public void Dispose()
        {
            // During prerender, this component is rendered without calling OnAfterRender and then immediately disposed
            // this mean timer will be null so we have to check for null or use the Null-conditional operator ? 
            timer?.Dispose();
            // AppChapterState.DisposeUser(UserSession);
        }
#endregion

#region ParentEvents 


        public async void SelectHome()
        {
            await _SelectHome.InvokeAsync();
        }

        public async void RefreshChapters()
        {
            await _RefreshChapters.InvokeAsync();
        }

        public async Task SelectChapter(UsrOrsfSmartflows smartflow)
        {
            
            await _SelectChapter.InvokeAsync(smartflow);
         
        }
        


    

#endregion

#region Chapter Details

        private async Task SelectCaseType()
        {
            try
            {
                
                DisplaySpinner = true;

                NavDisplay = "CaseType";
                


            }
            catch (Exception ex)
            {

                GenericErrorLog(true,ex, "SelectCaseType", $"Loading selected Case Type: {ex.Message}");

            }
            finally
            {
                DisplaySpinner = false;

                StateHasChanged();


            }


        }


        private async Task ChaseTypeUpdated(string listType)
        {

            // RefreshChapterItems(listType);
            
            // _SelectedChapterObject.SmartflowObject.SmartflowData = JsonConvert.SerializeObject(SelectedChapter);
            // await ChapterManagementService.Update(_SelectedChapterObject.SmartflowObject);

            // AppChapterState.SetLastUpdated(UserSession, SelectedChapter);
            
        }


        
        protected void ShowNav(string displayChange)
        {
            CompareSystems = false;
            RowChanged = 0;

            NavDisplay = displayChange;
        }

        

#endregion


#region Error Handling



        /****************************************/
        /* ERROR HANDLING */
        /****************************************/
        private void GenericErrorLog(bool showNotificationMsg, Exception e, string _method, string _message)
        {
            if(showNotificationMsg)
            {
                NotificationManager.ShowNotification("Danger", $"Oops! Something went wrong.");
            }

            using (LogContext.PushProperty("SourceSystem", UserSession.SelectedSystem))
            using (LogContext.PushProperty("SourceCompanyId", UserSession.Company.Id))
            using (LogContext.PushProperty("SourceUserId", UserSession.User.Id))
            using (LogContext.PushProperty("SourceContext", nameof(CaseTypeDetail)))
            {
                Logger.LogError(e,"Error - Method: {0}, Message: {1}",_method, _message);
            }

        }


#endregion

    }
}
