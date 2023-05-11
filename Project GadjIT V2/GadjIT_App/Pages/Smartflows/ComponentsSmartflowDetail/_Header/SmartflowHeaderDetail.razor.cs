using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Blazored.Modal;
using Blazored.Modal.Services;
using GadjIT_App.Data.Admin;
using GadjIT_App.Pages._Shared.Modals;
using GadjIT_App.Services;
using GadjIT_App.Services.AppState;
using GadjIT_App.Services.SessionState;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog.Context;
using GadjIT_App.FileManagement.FileProcessing.Interface;
using GadjIT_App.FileManagement.FileClassObjects;
using GadjIT_App.Pages.Smartflows.FileHandling;
using GadjIT_App.FileManagement.FileClassObjects.FileOptions;
using Microsoft.Extensions.Configuration;
using GadjIT_App.Shared.StaticObjects;
using GadjIT_ClientContext.Models.P4W;
using GadjIT_ClientContext.Models.Smartflow.Client;
using GadjIT_ClientContext.Models.Smartflow;
using System.Text;

namespace GadjIT_App.Pages.Smartflows.ComponentsSmartflowDetail._Header
{
    public partial class SmartflowHeaderDetail
    {
        [Parameter]
        public Client_SmartflowRecord _Selected_ClientSmartflowRecord { get; set; }

        [Parameter]
        public SmartflowV2 _SelectedSmartflow { get; set; }

        [Parameter]
        public List<P4W_CaseTypeGroups> _P4WCaseTypeGroups {get; set;}

        [Parameter]
        public List<P4W_MpSysViews> _ListP4WViews {get; set;}
    
        [Parameter]
        public List<P4W_DmDocuments> _LibraryDocumentsAndSteps {get; set;}

        [Parameter]
        public bool _PreviewSmartflowImage {get; set;}

        [Parameter]
        public EventCallback<string> _ShowNav {get; set;}

        [Parameter]
        public EventCallback<bool> _UpdatePreviewImage {get; set;}

        [Parameter]
        public EventCallback _RefreshLibraryDocumentsAndSteps {get; set;}

        [Parameter]
        public bool _SmartflowLockedForEdit {get; set;}
        

            
        [Inject]
        IModalService Modal { get; set; }

        [Inject]
        public IUserSessionState UserSession { get; set; }

       
        [Inject]
        public INotificationManager NotificationManager {get; set;}

        [Inject]
        private ILogger<SmartflowHeaderDetail> Logger { get; set; }

        [Inject]
        private IClientApiManagementService ClientApiManagementService { get; set; }

        [Inject]
        private ICompanyDbAccess CompanyDbAccess { get; set; }

        [Inject]
        public IAppSmartflowsState AppSmartflowsState { get; set; }

        [Inject]
        public IFileHelper FileHelper { get; set; }

        [Inject]
        private ISmartflowFileHelper SmartflowFileHelper { get; set; }

        [Inject]
        public IConfiguration Configuration { get; set;}

        

        private List<FileDesc> ListFilesForBgImages { get; set; }


        private List<FileDesc> ListFilesForBackups { get; set; }

        
        private bool PreviewSmartflowImage {
            get{return _PreviewSmartflowImage;} 
            set{_UpdatePreviewImage.InvokeAsync(value);}
        }




        private bool ShowJSON = false;


        
        public bool PartnerShowNotes
        {
            get { return (_SelectedSmartflow.ShowPartnerNotes == "Y" ? true : false); }
            set
            {
                if (value)
                {
                    _SelectedSmartflow.ShowPartnerNotes = "Y";
                }
                else
                {
                    _SelectedSmartflow.ShowPartnerNotes = "N";
                }
                
                SaveSmartflowDetails(true).ConfigureAwait(false);
            }

        }

        public bool ShowDocumentTracking
        {
            get { return (_SelectedSmartflow.ShowDocumentTracking == "Y" ? true : false); }
            set
            {
                if (value)
                {
                    _SelectedSmartflow.ShowDocumentTracking = "Y";
                }
                else
                {
                    _SelectedSmartflow.ShowDocumentTracking = "N";
                }

                SaveSmartflowDetails(true).ConfigureAwait(false);
            }

        }



        


        
        protected override async Task OnInitializedAsync()
        {
            //set path to point to the BackgroundImage path for the current company
            FileOptions chapterFileOption;

            chapterFileOption = new FileOptions
            {
                Company = UserSession.Company.CompanyName,
                SelectedSystem = UserSession.SelectedSystem
            };

            SmartflowFileHelper.SetFileHelperCustomPath(chapterFileOption,FileStorageType.BackgroundImages);

            //FileHelper.CustomPath = $"wwwroot/images/Companies/{UserSession.Company.CompanyName}/BackgroundImages"; 
            ListFilesForBgImages = FileHelper.GetFileList();
        }

        protected async Task ShowNav(string navItem)
        {
            await _ShowNav.InvokeAsync(navItem);
        }

        

        private async Task SaveSmartflowDetails()
        {
            await SaveSmartflowDetails(false);
        }

        /// <summary>
        /// Saves the Smartflow main details after changes to the Home tab
        /// </summary>
        /// <returns></returns>
        private async Task SaveSmartflowDetails(bool showNotification)
        {
            try
            {
                _Selected_ClientSmartflowRecord.SmartflowName = _SelectedSmartflow.Name;

                _Selected_ClientSmartflowRecord.SmartflowData = JsonConvert.SerializeObject(_SelectedSmartflow);
                await ClientApiManagementService.Update(_Selected_ClientSmartflowRecord);


                if(showNotification)
                {
                    await NotificationManager.ShowNotification("Success", $"Smartflow updated",1).ConfigureAwait(false);
                }

            }
            catch (Exception e)
            {
                GenericErrorLog(true,e, "SaveSmartflowDetails", $"Saving Smartflow base details: {e.Message}");
            }

            await InvokeAsync(() =>
            {
                StateHasChanged();
            });
        }

        private async void SaveP4WCaseTypeGroup(string caseTypeGroup)
        {
            try
            {
                _SelectedSmartflow.P4WCaseTypeGroup = caseTypeGroup;

                _Selected_ClientSmartflowRecord.SmartflowData = JsonConvert.SerializeObject(_SelectedSmartflow);

                await ClientApiManagementService.Update(_Selected_ClientSmartflowRecord).ConfigureAwait(false);

            }
            catch(HttpRequestException)
            {
                //do nothing, already dealt with
            }
            catch(Exception e)
            {
                GenericErrorLog(true,e, "SaveP4WCaseTypeGroup", e.Message);
            }
        }

        private async void SaveSelectedView(string view)
        {
            try
            {
                _SelectedSmartflow.SelectedView = view;

                _Selected_ClientSmartflowRecord.SmartflowData = JsonConvert.SerializeObject(_SelectedSmartflow);

                await ClientApiManagementService.Update(_Selected_ClientSmartflowRecord).ConfigureAwait(false);

            }
            catch(HttpRequestException)
            {
                //do nothing, already dealt with
            }
            catch(Exception e)
            {
                GenericErrorLog(true,e, "SaveSelectedView", e.Message);
            }
        }
        
        private async void SaveSelectedStep(string step)
        {
            try
            {
                _SelectedSmartflow.SelectedStep = step;

                _Selected_ClientSmartflowRecord.SmartflowData = JsonConvert.SerializeObject(_SelectedSmartflow);

                await ClientApiManagementService.Update(_Selected_ClientSmartflowRecord).ConfigureAwait(false);

            }
            catch(HttpRequestException)
            {
                //do nothing, already dealt with
            }
            catch(Exception e)
            {
                GenericErrorLog(true,e, "SaveSelectedStep", e.Message);
            }
        }

        public void CancelCreateP4WStep()
        {
            _SelectedSmartflow.StepName = "";
            _SelectedSmartflow.SelectedStep = "";
            _SelectedSmartflow.StepName = $"SF {_SelectedSmartflow.Name} Smartflow";
            StateHasChanged();
        }

        protected async void CreateP4WSmartflowStep()
        {
            try
            {
                IList<string> Errors = new List<string>();

                string confirmText = "";
                string confirmHeader = "";

                if (string.IsNullOrEmpty(_SelectedSmartflow.P4WCaseTypeGroup))
                {
                    Errors.Add("Missing Case Type Group");
                }

                if (string.IsNullOrEmpty(_SelectedSmartflow.SelectedView))
                {
                    Errors.Add("Missing View");
                }

                if (string.IsNullOrEmpty(_SelectedSmartflow.StepName))
                {
                    Errors.Add("Missing Step Name");
                }
                else if(_LibraryDocumentsAndSteps
                    .Where(D => D.DocumentType == 6)
                    .Where(D => D.Notes != null && D.Notes.Contains("Smartflow:"))
                    .Where(V => V.CaseTypeGroupRef == (string.IsNullOrEmpty(_SelectedSmartflow.P4WCaseTypeGroup)
                                                        ? -2
                                                        : _SelectedSmartflow.P4WCaseTypeGroup == "Global Documents"
                                                        ? 0
                                                        : _SelectedSmartflow.P4WCaseTypeGroup == "Entity Documents"
                                                        ? -1
                                                        : _P4WCaseTypeGroups
                                                            .Where(P => P.Name == _SelectedSmartflow.P4WCaseTypeGroup)
                                                            .Select(P => P.Id)
                                                            .FirstOrDefault()))
                    .OrderBy(D => D.Name)
                    .Select(D => D.Name)
                    .ToList()
                    .Contains(_SelectedSmartflow.StepName))
                {
                    confirmHeader = "Possible Conflict?";
                    confirmText = $"This Step already exists in the case type group: {_SelectedSmartflow.P4WCaseTypeGroup}. Do you still wish to create?";
                }


                if (Errors.Count == 0 && confirmHeader == "")
                {
                    CreateStep();
                }
                else if (Errors.Count > 0)
                {
                    ShowErrorModal("Step Creation", "Step creation could not be completed:", Errors);
                }
                else
                {
                    ShowModalConfirm(confirmText, confirmHeader, CreateStep);
                }
            }
            catch(Exception e)
            {
                GenericErrorLog(true,e, "CreateP4WSmartflowStep", e.Message);
            }

        }


        /// <summary>
        /// Update/Create P4W step
        /// </summary>
        /// <returns></returns>
        private async void CreateStep()
        {
            await CreateStepTask();
        }
        private async Task CreateStepTask()
        {
            try
            {
                SmartflowP4WStepSchema SmartflowP4WStep;

                if (_SelectedSmartflow.P4WCaseTypeGroup == "Entity Documents")
                {
                    SmartflowP4WStep = new SmartflowP4WStepSchema
                    {
                        StepName = _SelectedSmartflow.StepName,
                        P4WCaseTypeGroup = _SelectedSmartflow.P4WCaseTypeGroup,
                        GadjITCaseTypeGroup = _SelectedSmartflow.CaseTypeGroup,
                        GadjITCaseType = _SelectedSmartflow.CaseType,
                        Smartflow = _SelectedSmartflow.Name,
                        SFVersion = Configuration["AppSettings:Version"],
                        Questions = new List<SmartflowP4WStepQuestion>{
                                        new SmartflowP4WStepQuestion {QNo = 1, QText= "HQ - Set Current Chapter Details" }
                                        ,new SmartflowP4WStepQuestion {QNo = 2, QText= "HQ - Show View" }
                                        ,new SmartflowP4WStepQuestion {QNo = 3, QText= "HQ - Run Required Steps" }
                                        ,new SmartflowP4WStepQuestion {QNo = 4, QText= "HQ - Update reschedule date" }
                                        ,new SmartflowP4WStepQuestion {QNo = 5, QText= "HQ - Reinsert this step with asname" }
                                        ,new SmartflowP4WStepQuestion {QNo = 6, QText= "HQ - Rename Step" }
                                        ,new SmartflowP4WStepQuestion {QNo = 7, QText= "HQ - Delete Previous Instances if Suppressed Step" }
                                        ,new SmartflowP4WStepQuestion {QNo = 8, QText= "HQ - Check if completion name exists" }
                                        ,new SmartflowP4WStepQuestion {QNo = 9, QText= "HQ - Delete Step" }
                                        },
                        Answers = new List<SmartflowP4WStepAnswer>{
                                        new SmartflowP4WStepAnswer {QNo = 1, GoToData= $"2 [SQL: EXEC Up_ORSF_MoveDocsToAgendas '[entity.code]', 0] [SQL: EXEC up_ORSF_CreateTableEntries '[entity.code]', 0] [SQL: UPDATE Usr_ORSF_ENT_Control SET Current_SF = '{_SelectedSmartflow.Name}', Current_Case_Type_Group = '{_SelectedSmartflow.CaseTypeGroup}', Current_Case_Type = '{_SelectedSmartflow.CaseType}', Default_Step = '{_SelectedSmartflow.StepName}', Date_Schedule_For = DATEADD(d, 7, getdate()), Steps_To_Run = '', Schedule_AsName = '{_SelectedSmartflow.StepName}|' + (SELECT CASE WHEN dbo.fn_ORSF_IsAllCap (Description) = 1 THEN '{_SelectedSmartflow.StepName}' + '|' + CONVERT(VARCHAR(20),ISNULL(Date_Schedule_For,DATEADD(d, 7, getdate())),103) ELSE Description + '|' + CONVERT(VARCHAR(20),ISNULL(s.DiaryDate, getdate()),103) END + '|' + '[CurrentUser.Code]' FROM Cm_CaseItems i INNER JOIN Cm_Steps s on s.ItemID = i.ItemID WHERE i.ItemID = [currentstep.stepid]), Complete_AsName = '' WHERE EntityRef = '[Entity.Code]'] [SQL: UPDATE Usr_ORSF_ENT_Control SET Screen_Opened_Via_Step = 'Y' WHERE EntityRef = '[Entity.Code]' ]" }
                                        ,new SmartflowP4WStepAnswer {QNo = 2, GoToData= $"7 [VIEW: '{_SelectedSmartflow.SelectedView}' UPDATE=Yes]" }
                                        ,new SmartflowP4WStepAnswer {QNo = 3, GoToData= $"4 [TAKE: 'SF-Admin Save Items for Agenda Management' INSERT=END]  [SQL: EXEC up_ORSF_GetStepsFromList '[entity.code]', 0] [SQL: UPDATE Usr_ORSF_ENT_Control SET Screen_Opened_Via_Step = null WHERE EntityRef='[entity.code]']" }
                                        ,new SmartflowP4WStepAnswer {QNo = 4, GoToData= $"5 [SQL: UPDATE Usr_ORSF_ENT_Control SET Date_Schedule_For = isnull(Date_Schedule_For, Cast(getdate() as Date)) WHERE EntityRef = '[Entity.Code]']" }
                                        ,new SmartflowP4WStepAnswer {QNo = 5, GoToData= $"8 [SQL: SELECT ScheduleCommand FROM fn_ORSF_GetScheduleItems('[Entity.Code]', 0)]" }
                                        ,new SmartflowP4WStepAnswer {QNo = 6, GoToData= $"[SQL: UPDATE cm_caseitems set CompletionDate = GETDATE(), description = UPPER('[!Usr_ORSF_ENT_Control.Complete_AsName]') where itemid = [currentstep.stepid]] [SQL: exec up_ORSF_CompleteStep [currentStep.stepid], '', 'Y']" }
                                        ,new SmartflowP4WStepAnswer {QNo = 7, GoToData= $"3 [[SQL: SELECT CASE WHEN ISNULL(Do_Not_Reschedule,'N') = 'Y' THEN 'SQL: exec up_ORSF_DeleteDueStep '''', [currentstep.stepid], ''{_SelectedSmartflow.Name}''' ELSE '' END FROM Usr_ORSF_MT_Control WHERE EntityRef = '[matters.entityref]' AND MatterNo = [matters.number]]]" }
                                        ,new SmartflowP4WStepAnswer {QNo = 8, GoToData= $"[SQL: SELECT CASE WHEN ISNULL('[!Usr_ORSF_ENT_Control.Complete_AsName]','') <> '' THEN 6 ELSE 9 END]" }
                                        ,new SmartflowP4WStepAnswer {QNo = 9, GoToData= $"[SQL: exec up_ORSF_DeleteStep [currentstep.stepid]]" }
                                        }
                    };
                }
                else
                {
                    SmartflowP4WStep = new SmartflowP4WStepSchema
                    {
                        StepName = _SelectedSmartflow.StepName,
                        P4WCaseTypeGroup = _SelectedSmartflow.P4WCaseTypeGroup,
                        GadjITCaseTypeGroup = _SelectedSmartflow.CaseTypeGroup,
                        GadjITCaseType = _SelectedSmartflow.CaseType,
                        Smartflow = _SelectedSmartflow.Name,
                        SFVersion = Configuration["AppSettings:Version"],
                        Questions = new List<SmartflowP4WStepQuestion>{
                                        new SmartflowP4WStepQuestion {QNo = 1, QText= "HQ - Set Current Chapter Details" }
                                        ,new SmartflowP4WStepQuestion {QNo = 2, QText= "HQ - Show View" }
                                        ,new SmartflowP4WStepQuestion {QNo = 3, QText= "HQ - Run Required Steps" }
                                        ,new SmartflowP4WStepQuestion {QNo = 4, QText= "HQ - Update reschedule date" }
                                        ,new SmartflowP4WStepQuestion {QNo = 5, QText= "HQ - Reinsert this step with asname" }
                                        ,new SmartflowP4WStepQuestion {QNo = 6, QText= "HQ - Rename Step" }
                                        ,new SmartflowP4WStepQuestion {QNo = 7, QText= "HQ - Delete Previous Instances if Suppressed Step" }
                                        ,new SmartflowP4WStepQuestion {QNo = 8, QText= "HQ - Check if completion name exists" }
                                        ,new SmartflowP4WStepQuestion {QNo = 9, QText= "HQ - Delete Step" }
                                        },
                        Answers = new List<SmartflowP4WStepAnswer>{
                                        new SmartflowP4WStepAnswer {QNo = 1, GoToData= $"2 [SQL: EXEC Up_ORSF_MoveDocsToAgendas '[matters.entityref]', [matters.number]] [SQL: EXEC up_ORSF_CreateTableEntries '[matters.entityref]', [matters.number]] [SQL: UPDATE Usr_ORSF_MT_Control SET Current_SF = '{_SelectedSmartflow.Name}', Current_Case_Type_Group = '{_SelectedSmartflow.CaseTypeGroup}', Current_Case_Type = '{_SelectedSmartflow.CaseType}', Default_Step = '{_SelectedSmartflow.StepName}', Date_Schedule_For = DATEADD(d, 7, getdate()), Steps_To_Run = '', Schedule_AsName = '{_SelectedSmartflow.StepName}|' + (SELECT CASE WHEN dbo.fn_ORSF_IsAllCap (Description) = 1 THEN '{_SelectedSmartflow.StepName}' + '|' + CONVERT(VARCHAR(20),ISNULL(Date_Schedule_For,DATEADD(d, 7, getdate())),103) ELSE Description + '|' + CONVERT(VARCHAR(20),ISNULL(s.DiaryDate, getdate()),103) END + '|' + '[matters.feeearnerref]' FROM Cm_CaseItems i INNER JOIN Cm_Steps s on s.ItemID = i.ItemID WHERE i.ItemID = [currentstep.stepid]), Complete_AsName = '' WHERE EntityRef = '[matters.entityref]' AND MatterNo = [matters.number]] [SQL: UPDATE Usr_ORSF_MT_Control SET Screen_Opened_Via_Step = 'Y' WHERE EntityRef = '[matters.entityref]' AND matterNo =[matters.number]]" }
                                        ,new SmartflowP4WStepAnswer {QNo = 2, GoToData= $"7 [VIEW: '{_SelectedSmartflow.SelectedView}' UPDATE=Yes]" }
                                        ,new SmartflowP4WStepAnswer {QNo = 3, GoToData= $"4 [TAKE: 'SF-Admin Save Items for Agenda Management' INSERT=END]  [SQL: EXEC up_ORSF_GetStepsFromList '[matters.entityref]', [matters.number]] [SQL: UPDATE Usr_ORSF_MT_Control SET Screen_Opened_Via_Step = null WHERE EntityRef='[matters.entityref]' AND matterNo=[matters.number]]" }
                                        ,new SmartflowP4WStepAnswer {QNo = 4, GoToData= $"5 [SQL: UPDATE Usr_ORSF_MT_Control SET Date_Schedule_For = isnull(Date_Schedule_For, Cast(getdate() as Date)) WHERE EntityRef = '[matters.entityref]' AND MatterNo = [matters.number]]" }
                                        ,new SmartflowP4WStepAnswer {QNo = 5, GoToData= $"8 [SQL: SELECT ScheduleCommand FROM fn_ORSF_GetScheduleItems('[matters.entityref]', [matters.number])]" }
                                        ,new SmartflowP4WStepAnswer {QNo = 6, GoToData= $"[SQL: UPDATE cm_caseitems set CompletionDate = GETDATE(), description = UPPER('[!Usr_ORSF_MT_Control.Complete_AsName]') where itemid = [currentstep.stepid]] [SQL: exec up_ORSF_CompleteStep [currentStep.stepid], '', 'Y']" }
                                        ,new SmartflowP4WStepAnswer {QNo = 7, GoToData= $"3 [[SQL: SELECT CASE WHEN ISNULL(Do_Not_Reschedule,'N') = 'Y' THEN 'SQL: exec up_ORSF_DeleteDueStep '''', [currentstep.stepid], ''{_SelectedSmartflow.Name}''' ELSE '' END FROM Usr_ORSF_MT_Control WHERE EntityRef = '[matters.entityref]' AND MatterNo = [matters.number]]]" }
                                        ,new SmartflowP4WStepAnswer {QNo = 8, GoToData= $"[SQL: SELECT CASE WHEN ISNULL('[!Usr_ORSF_MT_Control.Complete_AsName]','') <> '' THEN 6 ELSE 9 END]" }
                                        ,new SmartflowP4WStepAnswer {QNo = 9, GoToData= $"[SQL: exec up_ORSF_DeleteStep [currentstep.stepid]]" }
                                        }
                    };
                }

                string stepJSON = JsonConvert.SerializeObject(SmartflowP4WStep);

                bool creationSuccess;

                creationSuccess = await ClientApiManagementService.CreateStep(new P4W_SmartflowStepSchemaJSONObject { StepSchemaJSON = stepJSON });

                if (creationSuccess)
                {
                    await _RefreshLibraryDocumentsAndSteps.InvokeAsync();
                    
                    _SelectedSmartflow.SelectedStep = _SelectedSmartflow.StepName;

                    _Selected_ClientSmartflowRecord.SmartflowData = JsonConvert.SerializeObject(_SelectedSmartflow);

                    bool gotLock = ClientApiManagementService.Lock;
                    while (gotLock)
                    {
                        await Task.Yield();
                        gotLock = ClientApiManagementService.Lock;
                    }

                    await ClientApiManagementService.Update(_Selected_ClientSmartflowRecord);

                    await InvokeAsync(() =>
                    {
                        StateHasChanged();
                    });
                }
            }
            catch (Exception e)
            {
                GenericErrorLog(true,e, "CreateStep", $"Creating/Updating current P4W Smartflow step: {e.Message}");
                
            }
        }

        




#region Comparisons
        /********************************************************************************************
        /* COMPARISONS
        /********************************************************************************************/

        protected async Task ShowHeaderComparisonModal()
        {
            try
            {

                await UserSession.SwitchSelectedSystem();

                bool gotLock = CompanyDbAccess.Lock;
                while (gotLock)
                {
                    await Task.Yield();
                    gotLock = CompanyDbAccess.Lock;
                }

                var Alt_AppSmartflowRecord = await CompanyDbAccess.GetSmartflow(UserSession
                                                            ,_Selected_ClientSmartflowRecord.CaseTypeGroup
                                                            ,_Selected_ClientSmartflowRecord.CaseType
                                                            ,_Selected_ClientSmartflowRecord.SmartflowName
                                                            );
                
                await UserSession.ResetSelectedSystem();
                
                if(Alt_AppSmartflowRecord == null || Alt_AppSmartflowRecord.SmartflowData == null)
                {
                    //Smartflow does not exist on Alt System 
                    await NotificationManager.ShowNotification("Warning", $"A corresponding Smartflow must exist on the {UserSession.AltSystem} system.");
                }
                else
                {

                    var AltSmartflow = JsonConvert.DeserializeObject<SmartflowV2>(Alt_AppSmartflowRecord.SmartflowData);

                    var parameters = new ModalParameters();
                    parameters.Add("_SelectedSmartflow", _SelectedSmartflow);
                    parameters.Add("_Alt_Smartflow", AltSmartflow);
                    parameters.Add("_Alt_SmartflowRecord", Alt_AppSmartflowRecord);


                    var options = new ModalOptions()
                    {
                        Class = "blazored-custom-modal modal-smartflow-comparison"
                    };

                    Modal.Show<SmartflowHeaderComparison>("Synchronise Smartflow Item", parameters, options);
                }
            }
            catch(Exception e)
            {
                GenericErrorLog(false,e, "ShowHeaderComparisonModal", e.Message);
            }
        }

        

        


#endregion

#region Exports

        /********************************************************************************************
        /* EXPORTS
        /********************************************************************************************/
        private async void ExportSmartflowToExcel()
        {
            //WriteChapterJSONToFile();
            await SmartflowFileHelper.WriteSmartflowDataToExcel(_SelectedSmartflow, _LibraryDocumentsAndSteps, _P4WCaseTypeGroups);

        }

        protected void ShowSmartflowImportModal()
        {

            try
            {

                FileOptions chapterFileOption;

                chapterFileOption = new FileOptions
                {
                    Company = UserSession.Company.CompanyName
                    , SelectedSystem = UserSession.SelectedSystem
                    , CaseTypeGroup = _Selected_ClientSmartflowRecord.CaseTypeGroup
                    , CaseType = _Selected_ClientSmartflowRecord.CaseType
                    , SmartflowName = _Selected_ClientSmartflowRecord.SmartflowName
                };

                SmartflowFileHelper.SetFileHelperCustomPath(chapterFileOption,FileStorageType.TempUploads); 

                //Action WriteBackUp = WriteChapterJSONToFile;

                Action SelectedAction = RefreshJson;
                var parameters = new ModalParameters();
                parameters.Add("TaskObject", _Selected_ClientSmartflowRecord);
                parameters.Add("ListFileDescriptions", ListFilesForBackups);
                parameters.Add("DataChanged", SelectedAction);
                //parameters.Add("WriteBackUp", WriteBackUp);
                //parameters.Add("OriginalDataViews", LstDataViews);

                var options = new ModalOptions()
                {
                    Class = "blazored-custom-modal modal-smartflow-import"
                };

                Modal.Show<ModalSmartflowImport>("Excel Import", parameters, options);
            }
            catch(Exception e)
            {
                GenericErrorLog(true,e, "ShowSmartflowImportModal", e.Message);
            }
        }

        // public void WriteChapterJSONToFile()
        // {
        //     try
        //     {
        //         var fileName = _SelectedSmartflow.Name + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".txt";

        //         SmartflowFileHelper.WriteSmartflowToFile(_Selected_ClientSmartflowRecord.SmartflowData, fileName); //CustomPath set ShowSmartflowImportModal

        //         GetSeletedSmartflowFileList();
        //         StateHasChanged();
        //     }
        //     catch(Exception e)
        //     {
        //         GenericErrorLog(false,e, "WriteChapterJSONToFile", e.Message);
        //     }
            
        // }

#endregion

#region FileHandling

        /********************************************************************************************
        /* FILE HANDLING
        /********************************************************************************************/

        // private void GetSeletedSmartflowFileList()
        // {
        //     ListFilesForBackups = SmartflowFileHelper.GetFileListForSmartflow();
        // }

        private void RefreshJson()
        {
            SaveJson(_Selected_ClientSmartflowRecord.SmartflowData);
        }

        private async void SaveJson(string Json)
        {
            IList<string> jsonErrors;

            jsonErrors = new List<string>();
            jsonErrors = SmartflowFileHelper.ValidateSmartflowJSON(Json);

            if (jsonErrors.Count == 0)
            {
                try
                {
                    var smartflowData = JsonConvert.DeserializeObject<SmartflowV2>(Json);
                    _SelectedSmartflow.Agendas = smartflowData.Agendas;
                    _SelectedSmartflow.Status = smartflowData.Status;
                    _SelectedSmartflow.Documents = smartflowData.Documents;
                    _SelectedSmartflow.DataViews = smartflowData.DataViews;
                    _SelectedSmartflow.Fees = smartflowData.Fees;
                    _SelectedSmartflow.Messages = smartflowData.Messages;
                    _SelectedSmartflow.P4WCaseTypeGroup = smartflowData.P4WCaseTypeGroup;
                    _SelectedSmartflow.SelectedStep = smartflowData.SelectedStep;
                    _SelectedSmartflow.SelectedView = smartflowData.SelectedView;
                    _SelectedSmartflow.ShowPartnerNotes = smartflowData.ShowPartnerNotes;
                    _SelectedSmartflow.ShowDocumentTracking = smartflowData.ShowDocumentTracking;

                    _Selected_ClientSmartflowRecord.SmartflowData = JsonConvert.SerializeObject(_SelectedSmartflow);
                    
                    await ClientApiManagementService.Update(_Selected_ClientSmartflowRecord);

                    //SelectChapter();

                    ShowJSON = false;

                    await InvokeAsync(() =>
                    {
                        StateHasChanged();
                    });
                }
                catch(Exception e)
                {
                    jsonErrors.Add("Error processing data");
                    ShowErrorModal("Backup Restore Error", "The following errors occured during the restore:", jsonErrors);
                }

            }
            else
            {
                ShowErrorModal("Backup Restore Error", "The following errors occured during the restore:", jsonErrors);
            }
        }

        protected void DownloadTrackingSQL()
        {
            try
            {
                string entityRef = "[Matters.EntityRef]";
                string matterNo = "[Matters.Number]";
                string caseTypeGroup = _Selected_ClientSmartflowRecord.CaseTypeGroup;
                string caseType = _Selected_ClientSmartflowRecord.CaseType;
                string smartflow = _Selected_ClientSmartflowRecord.SmartflowName;
                string status = "Awaiting";

                //Cater for single quotes in variable names
                caseTypeGroup = caseTypeGroup.Replace("'","''");
                caseType = caseType.Replace("'","''");
                smartflow = smartflow.Replace("'","''");

                string sqlCommand = $"[SQL: SELECT Chasing_Description FROM Usr_ORSF_MT_Document_Status WHERE EntityRef = '{entityRef}' AND MatterNo = {matterNo} AND CaseTypeGroup = '{caseTypeGroup}' AND CaseType = '{caseType}' AND Smartflow = '{smartflow}' AND ISNULL(Document_Status,'') = '{status}']";

                byte[] fileData = Encoding.ASCII.GetBytes(sqlCommand);

                var fileName = _Selected_ClientSmartflowRecord.SmartflowName + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".txt";

                FileOptions chapterFileOption;

                chapterFileOption = new FileOptions
                {
                    Company = UserSession.Company.CompanyName,
                    SelectedSystem = UserSession.SelectedSystem,
                    CaseTypeGroup = _Selected_ClientSmartflowRecord.CaseTypeGroup,
                    CaseType = _Selected_ClientSmartflowRecord.CaseType,
                };

                SmartflowFileHelper.SetFileHelperCustomPath(chapterFileOption,FileStorageType.TempUploads);

                FileHelper.DownloadFile(fileName, fileData);
            }
            catch (Exception e)
            {
                GenericErrorLog(true,e, "DownloadTrackingSQL", $"Downloading tracking SQL command: {e.Message}");
            }
        }


        

        
        /****************************************/
        /* ERROR HANDLING AND NOTIFICATIONS     */
        /****************************************/
        private void GenericErrorLog(bool _showNotificationMsg, Exception e, string _method, string _message)
        {
            using (LogContext.PushProperty("SourceSystem", UserSession.SelectedSystem))
            using (LogContext.PushProperty("SourceCompanyId", UserSession.Company.Id))
            using (LogContext.PushProperty("SourceUserId", UserSession.User.Id))
            using (LogContext.PushProperty("SourceContext", nameof(SmartflowHeaderDetail)))
            {
                Logger.LogError(e,"Error - Method: {0}, Message: {1}",_method, _message);
            }

            if(_showNotificationMsg)
            {
                NotificationManager.ShowNotification("Danger", $"Oops! Something went wrong.");
            }
        }

        protected void ShowErrorModal(string header, string errorDesc, IList<string> errorDets)
        {
            var parameters = new ModalParameters();
            parameters.Add("ErrorDesc", errorDesc);
            parameters.Add("ErrorDetails", errorDets);

            var options = new ModalOptions()
            {
                Class = "blazored-custom-modal modal-smartflow-import"
            };

            Modal.Show<ModalErrorInfo>(header, parameters, options);
        }

        private void ShowModalConfirm(string infoText, string infoHeader, Action selectedAction)
        {
            var parameters = new ModalParameters();
            parameters.Add("InfoHeader", infoHeader);
            parameters.Add("InfoText", infoText);
            parameters.Add("ConfirmAction", selectedAction);

            var options = new ModalOptions()
            {
                Class = "blazored-custom-modal modal-confirm"
            };

            Modal.Show<ModalConfirm>(infoHeader, parameters, options);
        }

    }

}

#endregion