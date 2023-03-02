using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Blazored.Modal;
using Blazored.Modal.Services;
using GadjIT_App.Data.Admin;
using GadjIT_App.Pages.Chapters.ComponentsChapterDetail._SharedItems;
using GadjIT_App.Pages._Shared.Modals;
using GadjIT_App.Services;
using GadjIT_App.Services.AppState;
using GadjIT_App.Services.SessionState;
using GadjIT_AppContext.GadjIT_App;
using GadjIT_ClientContext.P4W;
using GadjIT_ClientContext.P4W.Custom;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog.Context;
using Serilog.Core;
using System.Text.RegularExpressions;
using BlazorInputFile;
using GadjIT_App.FileManagement.FileProcessing.Interface;
using GadjIT_App.FileManagement.FileClassObjects;
using GadjIT_App.Pages.Chapters.FileUpload;
using GadjIT_App.FileManagement.FileClassObjects.FileOptions;
using Microsoft.Extensions.Configuration;


namespace GadjIT_App.Pages.Chapters.ComponentsCaseTypeDetail._Header
{
    public partial class CaseTypeHeaderDetail
    {
        [Parameter]
        public List<VmUsrOrsfSmartflows> _LstChapters { get; set; }

        [Parameter]
        public string _SelectedCaseTypeGroup { get; set; }

        [Parameter]
        public string _SelectedCaseType { get; set; }

        [Parameter]
        public EventCallback<string> _ShowNav {get; set;}

        [Parameter]
        public EventCallback<UsrOrsfSmartflows> _SelectChapter {get; set;}

            
        [Inject]
        IModalService Modal { get; set; }

        [Inject]
        public IUserSessionState UserSession { get; set; }

        [Inject]
        private IChapterManagementService ChapterManagementService { get; set; }
       
        [Inject]
        public INotificationManager NotificationManager {get; set;}

        [Inject]
        private ILogger<ChapterList> Logger { get; set; }

        protected List<VmUsrOrsfSmartflows> LstSmartflows {get; set;}

        int RowChanged { get; set; } = 0; 

        private bool SeqMoving = false;

                            
        protected override void OnInitialized()
        {
            LstSmartflows = _LstChapters
                                            .Where(C => C.SmartflowObject.CaseTypeGroup == _SelectedCaseTypeGroup)
                                            .Where(C => C.SmartflowObject.CaseType == _SelectedCaseType)
                                            .OrderBy(C => C.SmartflowObject.SeqNo)
                                            .ToList();

            foreach(VmUsrOrsfSmartflows vmSmartflow in LstSmartflows)
            {
                vmSmartflow.SetSmartflowStatistics();
            }
        }

        private void RefreshSmartflowList()
        {
            LstSmartflows = LstSmartflows
                                        .OrderBy(C => C.SmartflowObject.SeqNo)
                                        .ToList();
        }

        protected async Task ShowNav(string navItem)
        {
            await _ShowNav.InvokeAsync(navItem);
        }

        public async Task ReSequenceSmartFlows(int seq)
        {
            RowChanged = seq;
            await ReSequenceSmartFlows();
        }

        public async Task ReSequenceSmartFlows()
        {
            try
            {
                if(LstSmartflows.Select(C => C.SmartflowObject.SeqNo != LstSmartflows.IndexOf(C) + 1).Count() > 0) //If any SeqNos are out of sequence
                { 
                    LstSmartflows.Select(C => { C.SmartflowObject.SeqNo = LstSmartflows.IndexOf(C) + 1; return C; }).ToList(); //update all SeqNo by setting to the Index (+1)

                    foreach (var smartflowToChange in LstSmartflows)
                    {
                        await ChapterManagementService.UpdateMainItem(smartflowToChange.SmartflowObject).ConfigureAwait(false);

                    }

                    StateHasChanged();
                }

                
            }
            catch
            {
                
            }

        }

        protected async void MoveSmartFlowSeq(UsrOrsfSmartflows selectobject, string listType, string direction)
        {
            try
            {
                ResetRowChanged();

                SeqMoving = true; //prevents changes to the form whilst process of changing seq is carried out

                var lstItems = new List<VmUsrOrsfSmartflows>();
                int incrementBy;

                incrementBy = (direction.ToLower() == "up" ? -1 : 1);

                RowChanged = (int)(selectobject.SeqNo + incrementBy);

                lstItems = LstSmartflows
                            .OrderBy(A => A.SmartflowObject.SeqNo)
                            .ToList();


                var swapItem = lstItems.Where(D => D.SmartflowObject.SeqNo == (selectobject.SeqNo + incrementBy)).SingleOrDefault();
                if (!(swapItem is null))
                {
                    selectobject.SeqNo += incrementBy;
                    swapItem.SmartflowObject.SeqNo = swapItem.SmartflowObject.SeqNo + (incrementBy * -1);

                    await ChapterManagementService.UpdateMainItem(selectobject);
                    await ChapterManagementService.UpdateMainItem(swapItem.SmartflowObject);
                }

                SeqMoving = false;

                RefreshSmartflowList();


                await InvokeAsync(() =>
                {
                    StateHasChanged();
                });
                
                               

            }
            catch (Exception e)
            {
                GenericErrorLog(true,e, "MoveSmartFlowSeq", $"Moving smartflow: {e.Message}");

            }
            finally
            {
                SeqMoving = false;
            }


        }

        public void ResetRowChanged() 
        {
            RowChanged = 0;
            SeqMoving = false;

            StateHasChanged();
        }

        

        public async Task SelectChapter(UsrOrsfSmartflows smartflow)
        {
            await _SelectChapter.InvokeAsync(smartflow);
        }
        

        
        /****************************************/
        /* ERROR HANDLING AND NOTIFICATIONS     */
        /****************************************/
        private async void GenericErrorLog(bool _showNotificationMsg, Exception e, string _method, string _message)
        {
            if(_showNotificationMsg)
            {
                await NotificationManager.ShowNotification("Danger", $"Oops! Something went wrong.");
            }

            using (LogContext.PushProperty("SourceSystem", UserSession.SelectedSystem))
            using (LogContext.PushProperty("SourceCompanyId", UserSession.Company.Id))
            using (LogContext.PushProperty("SourceUserId", UserSession.User.Id))
            using (LogContext.PushProperty("SourceContext", nameof(CaseTypeHeaderDetail)))
            {
                Logger.LogError(e,"Error - Method: {0}, Message: {1}",_method, _message);
            }

        }

        


    }

}