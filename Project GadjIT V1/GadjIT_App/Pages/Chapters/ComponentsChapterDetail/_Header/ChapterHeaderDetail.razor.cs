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
using GadjIT_App.Shared.StaticObjects;

namespace GadjIT_App.Pages.Chapters.ComponentsChapterDetail._Header
{
    public partial class ChapterHeaderDetail
    {
        [Parameter]
        public UsrOrsfSmartflows _SelectedChapterObject { get; set; }

        [Parameter]
        public VmSmartflow _SelectedChapter { get; set; }

        [Parameter]
        public List<CaseTypeGroups> _P4WCaseTypeGroups {get; set;}

        [Parameter]
        public List<MpSysViews> _ListP4WViews {get; set;}
    
        [Parameter]
        public List<DmDocuments> _LibraryDocumentsAndSteps {get; set;}

        [Parameter]
        public bool _PreviewChapterImage {get; set;}

        [Parameter]
        public EventCallback<string> _ShowNav {get; set;}

        [Parameter]
        public EventCallback<bool> _UpdatePreviewImage {get; set;}

            
        [Inject]
        IModalService Modal { get; set; }

        [Inject]
        public IUserSessionState UserSession { get; set; }

       
        [Inject]
        public INotificationManager NotificationManager {get; set;}

        [Inject]
        private ILogger<ChapterList> Logger { get; set; }

        [Inject]
        private IChapterManagementService ChapterManagementService { get; set; }

        [Inject]
        private ICompanyDbAccess CompanyDbAccess { get; set; }

        [Inject]
        public IAppChapterState AppChapterState { get; set; }

        [Inject]
        public IFileHelper FileHelper { get; set; }

        [Inject]
        private IChapterFileUpload ChapterFileUpload { get; set; }

        [Inject]
        public IConfiguration Configuration { get; set;}

        

        private List<FileDesc> ListFilesForBgImages { get; set; }


        private List<FileDesc> ListFilesForBackups { get; set; }

        
        private bool PreviewChapterImage {
            get{return _PreviewChapterImage;} 
            set{_UpdatePreviewImage.InvokeAsync(value);}
        }




        private bool ShowJSON = false;


        
        public bool PartnerShowNotes
        {
            get { return (_SelectedChapter.ShowPartnerNotes == "Y" ? true : false); }
            set
            {
                if (value)
                {
                    _SelectedChapter.ShowPartnerNotes = "Y";
                }
                else
                {
                    _SelectedChapter.ShowPartnerNotes = "N";
                }
                
                SaveChapterDetails(true).ConfigureAwait(false);
            }

        }

        public bool ShowDocumentTracking
        {
            get { return (_SelectedChapter.ShowDocumentTracking == "Y" ? true : false); }
            set
            {
                if (value)
                {
                    _SelectedChapter.ShowDocumentTracking = "Y";
                }
                else
                {
                    _SelectedChapter.ShowDocumentTracking = "N";
                }

                SaveChapterDetails(true).ConfigureAwait(false);
            }

        }



        


        
        protected override async Task OnInitializedAsync()
        {
            //set path to point to the BackgroundImage path for the current company
            FileHelper.CustomPath = $"wwwroot/images/Companies/{UserSession.Company.CompanyName}/BackgroundImages";
            ListFilesForBgImages = FileHelper.GetFileList();
        }

        protected async Task ShowNav(string navItem)
        {
            await _ShowNav.InvokeAsync(navItem);
        }

        private void SetSmartflowFilePath()
        {
            try
            {
                ChapterFileOptions chapterFileOption;

                chapterFileOption = new ChapterFileOptions
                {
                    Company = UserSession.Company.CompanyName,
                    SelectedSystem = UserSession.SelectedSystem,
                    CaseTypeGroup = _SelectedChapter.CaseTypeGroup,
                    CaseType = _SelectedChapter.CaseType,
                    Chapter = _SelectedChapter.Name
                };

                ChapterFileUpload.SetFileHelperCustomPath(chapterFileOption,FileStorageType.BackupsSmartflow);
            }
            catch (Exception ex)
            {
                GenericErrorLog(true,ex, "SetSmartflowFilePath", $"Setting Smartflow file path: {ex.Message}");
            }
        }


        private async Task SaveChapterDetails()
        {
            await SaveChapterDetails(false);
        }

        /// <summary>
        /// Saves the Smartflow main details after changes to the Home tab
        /// </summary>
        /// <returns></returns>
        private async Task SaveChapterDetails(bool showNotification)
        {
            try
            {
                _SelectedChapterObject.SmartflowName = _SelectedChapter.Name;

                _SelectedChapterObject.SmartflowData = JsonConvert.SerializeObject(_SelectedChapter);
                await ChapterManagementService.Update(_SelectedChapterObject);


                //keep track of time last updated ready for comparison by other sessions checking for updates
                AppChapterState.SetLastUpdated(UserSession, _SelectedChapter);

                if(showNotification)
                {
                    await NotificationManager.ShowNotification("Success", $"Smartflow updated",1).ConfigureAwait(false);
                }

            }
            catch (Exception e)
            {
                GenericErrorLog(true,e, "SaveChapterDetails", $"Saving Smartflow base details: {e.Message}");
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
                _SelectedChapter.P4WCaseTypeGroup = caseTypeGroup;

                _SelectedChapterObject.SmartflowData = JsonConvert.SerializeObject(_SelectedChapter);

                await ChapterManagementService.Update(_SelectedChapterObject).ConfigureAwait(false);


                //keep track of time last updated ready for comparison by other sessions checking for updates
                AppChapterState.SetLastUpdated(UserSession, _SelectedChapter);
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
                _SelectedChapter.SelectedView = view;

                _SelectedChapterObject.SmartflowData = JsonConvert.SerializeObject(_SelectedChapter);

                await ChapterManagementService.Update(_SelectedChapterObject).ConfigureAwait(false);


                //keep track of time last updated ready for comparison by other sessions checking for updates
                AppChapterState.SetLastUpdated(UserSession, _SelectedChapter);
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
                _SelectedChapter.SelectedStep = step;

                _SelectedChapterObject.SmartflowData = JsonConvert.SerializeObject(_SelectedChapter);

                await ChapterManagementService.Update(_SelectedChapterObject).ConfigureAwait(false);


                //keep track of time last updated ready for comparison by other sessions checking for updates
                AppChapterState.SetLastUpdated(UserSession, _SelectedChapter);
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
            _SelectedChapter.StepName = "";
            _SelectedChapter.SelectedStep = "";
            _SelectedChapter.StepName = $"SF {_SelectedChapter.Name} Smartflow";
            StateHasChanged();
        }

        protected async void CreateP4WSmartflowStep()
        {
            try
            {
                IList<string> Errors = new List<string>();

                string confirmText = "";
                string confirmHeader = "";

                if (string.IsNullOrEmpty(_SelectedChapter.P4WCaseTypeGroup))
                {
                    Errors.Add("Missing Case Type Group");
                }

                if (string.IsNullOrEmpty(_SelectedChapter.SelectedView))
                {
                    Errors.Add("Missing View");
                }

                if (string.IsNullOrEmpty(_SelectedChapter.StepName))
                {
                    Errors.Add("Missing Step Name");
                }
                else if(_LibraryDocumentsAndSteps
                    .Where(D => D.DocumentType == 6)
                    .Where(D => D.Notes != null && D.Notes.Contains("Smartflow:"))
                    .Where(V => V.CaseTypeGroupRef == (string.IsNullOrEmpty(_SelectedChapter.P4WCaseTypeGroup)
                                                        ? -2
                                                        : _SelectedChapter.P4WCaseTypeGroup == "Global Documents"
                                                        ? 0
                                                        : _SelectedChapter.P4WCaseTypeGroup == "Entity Documents"
                                                        ? -1
                                                        : _P4WCaseTypeGroups
                                                            .Where(P => P.Name == _SelectedChapter.P4WCaseTypeGroup)
                                                            .Select(P => P.Id)
                                                            .FirstOrDefault()))
                    .OrderBy(D => D.Name)
                    .Select(D => D.Name)
                    .ToList()
                    .Contains(_SelectedChapter.StepName))
                {
                    confirmHeader = "Possible Conflict?";
                    confirmText = $"This Step already exists in the case type group: {_SelectedChapter.P4WCaseTypeGroup}. Do you still wish to create?";
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
            try
            {
                ChapterP4WStepSchema chapterP4WStep;

                if (_SelectedChapter.P4WCaseTypeGroup == "Entity Documents")
                {
                    chapterP4WStep = new ChapterP4WStepSchema
                    {
                        StepName = _SelectedChapter.StepName,
                        P4WCaseTypeGroup = _SelectedChapter.P4WCaseTypeGroup,
                        GadjITCaseTypeGroup = _SelectedChapter.CaseTypeGroup,
                        GadjITCaseType = _SelectedChapter.CaseType,
                        Smartflow = _SelectedChapter.Name,
                        SFVersion = Configuration["AppSettings:Version"],
                        Questions = new List<ChapterP4WStepQuestion>{
                                        new ChapterP4WStepQuestion {QNo = 1, QText= "HQ - Set Current Chapter Details" }
                                        ,new ChapterP4WStepQuestion {QNo = 2, QText= "HQ - Show View" }
                                        ,new ChapterP4WStepQuestion {QNo = 3, QText= "HQ - Run Required Steps" }
                                        ,new ChapterP4WStepQuestion {QNo = 4, QText= "HQ - Update reschedule date" }
                                        ,new ChapterP4WStepQuestion {QNo = 5, QText= "HQ - Reinsert this step with asname" }
                                        ,new ChapterP4WStepQuestion {QNo = 6, QText= "HQ - Rename Step" }
                                        ,new ChapterP4WStepQuestion {QNo = 7, QText= "HQ - Delete Previous Instances if Suppressed Step" }
                                        ,new ChapterP4WStepQuestion {QNo = 8, QText= "HQ - Check if completion name exists" }
                                        ,new ChapterP4WStepQuestion {QNo = 9, QText= "HQ - Delete Step" }
                                        },
                        Answers = new List<ChapterP4WStepAnswer>{
                                        new ChapterP4WStepAnswer {QNo = 1, GoToData= $"2 [SQL: EXEC Up_ORSF_MoveDocsToAgendas '[entity.code]', 0] [SQL: EXEC up_ORSF_CreateTableEntries '[entity.code]', 0] [SQL: UPDATE Usr_ORSF_ENT_Control SET Current_SF = '{_SelectedChapter.Name}', Current_Case_Type_Group = '{_SelectedChapter.CaseTypeGroup}', Current_Case_Type = '{_SelectedChapter.CaseType}', Default_Step = '{_SelectedChapter.StepName}', Date_Schedule_For = DATEADD(d, 7, getdate()), Steps_To_Run = '', Schedule_AsName = '{_SelectedChapter.StepName}|' + (SELECT CASE WHEN dbo.fn_ORSF_IsAllCap (Description) = 1 THEN '{_SelectedChapter.StepName}' + '|' + CONVERT(VARCHAR(20),ISNULL(Date_Schedule_For,DATEADD(d, 7, getdate())),103) ELSE Description + '|' + CONVERT(VARCHAR(20),ISNULL(s.DiaryDate, getdate()),103) END + '|' + '[CurrentUser.Code]' FROM Cm_CaseItems i INNER JOIN Cm_Steps s on s.ItemID = i.ItemID WHERE i.ItemID = [currentstep.stepid]), Complete_AsName = '' WHERE EntityRef = '[Entity.Code]'] [SQL: UPDATE Usr_ORSF_ENT_Control SET Screen_Opened_Via_Step = 'Y' WHERE EntityRef = '[Entity.Code]' ]" }
                                        ,new ChapterP4WStepAnswer {QNo = 2, GoToData= $"7 [VIEW: '{_SelectedChapter.SelectedView}' UPDATE=Yes]" }
                                        ,new ChapterP4WStepAnswer {QNo = 3, GoToData= $"4 [TAKE: 'SF-Admin Save Items for Agenda Management' INSERT=END]  [SQL: EXEC up_ORSF_GetStepsFromList '[entity.code]', 0] [SQL: UPDATE Usr_ORSF_ENT_Control SET Screen_Opened_Via_Step = null WHERE EntityRef='[entity.code]']" }
                                        ,new ChapterP4WStepAnswer {QNo = 4, GoToData= $"5 [SQL: UPDATE Usr_ORSF_ENT_Control SET Date_Schedule_For = isnull(Date_Schedule_For, Cast(getdate() as Date)) WHERE EntityRef = '[Entity.Code]']" }
                                        ,new ChapterP4WStepAnswer {QNo = 5, GoToData= $"8 [SQL: SELECT ScheduleCommand FROM fn_ORSF_GetScheduleItems('[Entity.Code]', 0)]" }
                                        ,new ChapterP4WStepAnswer {QNo = 6, GoToData= $"[SQL: UPDATE cm_caseitems set CompletionDate = GETDATE(), description = UPPER('[!Usr_ORSF_ENT_Control.Complete_AsName]') where itemid = [currentstep.stepid]] [SQL: exec up_ORSF_CompleteStep [currentStep.stepid], '', 'Y']" }
                                        ,new ChapterP4WStepAnswer {QNo = 7, GoToData= $"3 [[SQL: SELECT CASE WHEN ISNULL(Do_Not_Reschedule,'N') = 'Y' THEN 'SQL: exec up_ORSF_DeleteDueStep '''', [currentstep.stepid], ''{_SelectedChapter.Name}''' ELSE '' END FROM Usr_ORSF_MT_Control WHERE EntityRef = '[matters.entityref]' AND MatterNo = [matters.number]]]" }
                                        ,new ChapterP4WStepAnswer {QNo = 8, GoToData= $"[SQL: SELECT CASE WHEN ISNULL('[!Usr_ORSF_ENT_Control.Complete_AsName]','') <> '' THEN 6 ELSE 9 END]" }
                                        ,new ChapterP4WStepAnswer {QNo = 9, GoToData= $"[SQL: exec up_ORSF_DeleteStep [currentstep.stepid]]" }
                                        }
                    };
                }
                else
                {
                    chapterP4WStep = new ChapterP4WStepSchema
                    {
                        StepName = _SelectedChapter.StepName,
                        P4WCaseTypeGroup = _SelectedChapter.P4WCaseTypeGroup,
                        GadjITCaseTypeGroup = _SelectedChapter.CaseTypeGroup,
                        GadjITCaseType = _SelectedChapter.CaseType,
                        Smartflow = _SelectedChapter.Name,
                        SFVersion = Configuration["AppSettings:Version"],
                        Questions = new List<ChapterP4WStepQuestion>{
                                        new ChapterP4WStepQuestion {QNo = 1, QText= "HQ - Set Current Chapter Details" }
                                        ,new ChapterP4WStepQuestion {QNo = 2, QText= "HQ - Show View" }
                                        ,new ChapterP4WStepQuestion {QNo = 3, QText= "HQ - Run Required Steps" }
                                        ,new ChapterP4WStepQuestion {QNo = 4, QText= "HQ - Update reschedule date" }
                                        ,new ChapterP4WStepQuestion {QNo = 5, QText= "HQ - Reinsert this step with asname" }
                                        ,new ChapterP4WStepQuestion {QNo = 6, QText= "HQ - Rename Step" }
                                        ,new ChapterP4WStepQuestion {QNo = 7, QText= "HQ - Delete Previous Instances if Suppressed Step" }
                                        ,new ChapterP4WStepQuestion {QNo = 8, QText= "HQ - Check if completion name exists" }
                                        ,new ChapterP4WStepQuestion {QNo = 9, QText= "HQ - Delete Step" }
                                        },
                        Answers = new List<ChapterP4WStepAnswer>{
                                        new ChapterP4WStepAnswer {QNo = 1, GoToData= $"2 [SQL: EXEC Up_ORSF_MoveDocsToAgendas '[matters.entityref]', [matters.number]] [SQL: EXEC up_ORSF_CreateTableEntries '[matters.entityref]', [matters.number]] [SQL: UPDATE Usr_ORSF_MT_Control SET Current_SF = '{_SelectedChapter.Name}', Current_Case_Type_Group = '{_SelectedChapter.CaseTypeGroup}', Current_Case_Type = '{_SelectedChapter.CaseType}', Default_Step = '{_SelectedChapter.StepName}', Date_Schedule_For = DATEADD(d, 7, getdate()), Steps_To_Run = '', Schedule_AsName = '{_SelectedChapter.StepName}|' + (SELECT CASE WHEN dbo.fn_ORSF_IsAllCap (Description) = 1 THEN '{_SelectedChapter.StepName}' + '|' + CONVERT(VARCHAR(20),ISNULL(Date_Schedule_For,DATEADD(d, 7, getdate())),103) ELSE Description + '|' + CONVERT(VARCHAR(20),ISNULL(s.DiaryDate, getdate()),103) END + '|' + '[matters.feeearnerref]' FROM Cm_CaseItems i INNER JOIN Cm_Steps s on s.ItemID = i.ItemID WHERE i.ItemID = [currentstep.stepid]), Complete_AsName = '' WHERE EntityRef = '[matters.entityref]' AND MatterNo = [matters.number]] [SQL: UPDATE Usr_ORSF_MT_Control SET Screen_Opened_Via_Step = 'Y' WHERE EntityRef = '[matters.entityref]' AND matterNo =[matters.number]]" }
                                        ,new ChapterP4WStepAnswer {QNo = 2, GoToData= $"7 [VIEW: '{_SelectedChapter.SelectedView}' UPDATE=Yes]" }
                                        ,new ChapterP4WStepAnswer {QNo = 3, GoToData= $"4 [TAKE: 'SF-Admin Save Items for Agenda Management' INSERT=END]  [SQL: EXEC up_ORSF_GetStepsFromList '[matters.entityref]', [matters.number]] [SQL: UPDATE Usr_ORSF_MT_Control SET Screen_Opened_Via_Step = null WHERE EntityRef='[matters.entityref]' AND matterNo=[matters.number]]" }
                                        ,new ChapterP4WStepAnswer {QNo = 4, GoToData= $"5 [SQL: UPDATE Usr_ORSF_MT_Control SET Date_Schedule_For = isnull(Date_Schedule_For, Cast(getdate() as Date)) WHERE EntityRef = '[matters.entityref]' AND MatterNo = [matters.number]]" }
                                        ,new ChapterP4WStepAnswer {QNo = 5, GoToData= $"8 [SQL: SELECT ScheduleCommand FROM fn_ORSF_GetScheduleItems('[matters.entityref]', [matters.number])]" }
                                        ,new ChapterP4WStepAnswer {QNo = 6, GoToData= $"[SQL: UPDATE cm_caseitems set CompletionDate = GETDATE(), description = UPPER('[!Usr_ORSF_MT_Control.Complete_AsName]') where itemid = [currentstep.stepid]] [SQL: exec up_ORSF_CompleteStep [currentStep.stepid], '', 'Y']" }
                                        ,new ChapterP4WStepAnswer {QNo = 7, GoToData= $"3 [[SQL: SELECT CASE WHEN ISNULL(Do_Not_Reschedule,'N') = 'Y' THEN 'SQL: exec up_ORSF_DeleteDueStep '''', [currentstep.stepid], ''{_SelectedChapter.Name}''' ELSE '' END FROM Usr_ORSF_MT_Control WHERE EntityRef = '[matters.entityref]' AND MatterNo = [matters.number]]]" }
                                        ,new ChapterP4WStepAnswer {QNo = 8, GoToData= $"[SQL: SELECT CASE WHEN ISNULL('[!Usr_ORSF_MT_Control.Complete_AsName]','') <> '' THEN 6 ELSE 9 END]" }
                                        ,new ChapterP4WStepAnswer {QNo = 9, GoToData= $"[SQL: exec up_ORSF_DeleteStep [currentstep.stepid]]" }
                                        }
                    };
                }

                string stepJSON = JsonConvert.SerializeObject(chapterP4WStep);

                bool creationSuccess;

                creationSuccess = await ChapterManagementService.CreateStep(new VmSmartflowP4WStepSchemaJSONObject { StepSchemaJSON = stepJSON });

                if (creationSuccess)
                {
                    _LibraryDocumentsAndSteps = await ChapterManagementService.GetDocumentList(_SelectedChapter.CaseType);
                    
                    _SelectedChapter.SelectedStep = _SelectedChapter.StepName;

                    _SelectedChapterObject.SmartflowData = JsonConvert.SerializeObject(_SelectedChapter);

                    bool gotLock = ChapterManagementService.Lock;
                    while (gotLock)
                    {
                        await Task.Yield();
                        gotLock = ChapterManagementService.Lock;
                    }

                    await ChapterManagementService.Update(_SelectedChapterObject);

                    //keep track of time last updated ready for comparison by other sessions checking for updates
                    AppChapterState.SetLastUpdated(UserSession, _SelectedChapter);

                    StateHasChanged();
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

                var altChapterRecord = await CompanyDbAccess.GetSmartflow(UserSession
                                                            ,_SelectedChapterObject.CaseTypeGroup
                                                            ,_SelectedChapterObject.CaseType
                                                            ,_SelectedChapterObject.SmartflowName
                                                            );
                
                await UserSession.ResetSelectedSystem();
                
                if(altChapterRecord == null || altChapterRecord.SmartflowData == null)
                {
                    //Smartflow does not exist on Alt System 
                    await NotificationManager.ShowNotification("Warning", $"A corresponding Smartflow must exist on the {UserSession.AltSystem} system.");
                }
                else
                {

                    var altChapter = JsonConvert.DeserializeObject<VmSmartflow>(altChapterRecord.SmartflowData);

                    var parameters = new ModalParameters();
                    parameters.Add("_SelectedChapter", _SelectedChapter);
                    parameters.Add("_AltChapter", altChapter);
                    parameters.Add("_AltChapterRecord", altChapterRecord);


                    var options = new ModalOptions()
                    {
                        Class = "blazored-custom-modal modal-chapter-comparison"
                    };

                    Modal.Show<ChapterHeaderComparison>("Synchronise Smartflow Item", parameters, options);
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
            WriteChapterJSONToFile();
            await ChapterFileUpload.WriteChapterDataToExcel(_SelectedChapter, _LibraryDocumentsAndSteps, _P4WCaseTypeGroups);

        }

        protected void ShowChapterImportModal()
        {

            try
            {

                SetSmartflowFilePath();
                GetSeletedChapterFileList(); //populates ListFilesForBackups

                //clear all existing spreadsheets to make way for the new instance
                //in case a spreadsheet is locked and can't be deleted, work through a list of docs
                //report error if any remain.
                ListFilesForBackups.Where(F => F.FilePath.Contains(".xlsx")).ToList();
                foreach(var fileItem in ListFilesForBackups)
                {
                    ChapterFileUpload.DeleteFile(fileItem.FilePath);
                }

                GetSeletedChapterFileList();
                
                if(ListFilesForBackups.Where(F => F.FilePath.Contains(".xlsx")).Count() > 0)
                {
                    throw new Exception("Excel Spreadsheet deletion failed");
                }


                Action WriteBackUp = WriteChapterJSONToFile;

                Action SelectedAction = RefreshJson;
                var parameters = new ModalParameters();
                parameters.Add("TaskObject", _SelectedChapterObject);
                parameters.Add("ListFileDescriptions", ListFilesForBackups);
                parameters.Add("DataChanged", SelectedAction);
                parameters.Add("WriteBackUp", WriteBackUp);
                //parameters.Add("OriginalDataViews", LstDataViews);

                var options = new ModalOptions()
                {
                    Class = "blazored-custom-modal modal-chapter-import"
                };

                Modal.Show<ModalChapterImport>("Excel Import", parameters, options);
            }
            catch(Exception e)
            {
                GenericErrorLog(true,e, "ShowChapterImportModal", e.Message);
            }
        }

        public void WriteChapterJSONToFile()
        {
            try
            {
                SetSmartflowFilePath();

                var fileName = _SelectedChapter.Name + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".txt";

                ChapterFileUpload.WriteChapterToFile(_SelectedChapterObject.SmartflowData, fileName);

                GetSeletedChapterFileList();
                StateHasChanged();
            }
            catch(Exception e)
            {
                GenericErrorLog(false,e, "WriteChapterJSONToFile", e.Message);
            }
            
        }

#endregion

#region FileHandling

        /********************************************************************************************
        /* FILE HANDLING
        /********************************************************************************************/

        private void GetSeletedChapterFileList()
        {
            ListFilesForBackups = ChapterFileUpload.GetFileListForChapter();
        }

        private void RefreshJson()
        {
            SaveJson(_SelectedChapterObject.SmartflowData);
        }

        private async void SaveJson(string Json)
        {
            IList<string> jsonErrors;

            jsonErrors = new List<string>();
            jsonErrors = ChapterFileUpload.ValidateChapterJSON(Json);

            if (jsonErrors.Count == 0)
            {
                try
                {
                    var chapterData = JsonConvert.DeserializeObject<VmSmartflow>(Json);
                    _SelectedChapter.Items = chapterData.Items;
                    _SelectedChapter.DataViews = chapterData.DataViews;
                    _SelectedChapter.Fees = chapterData.Fees;
                    _SelectedChapter.TickerMessages = chapterData.TickerMessages;
                    _SelectedChapter.P4WCaseTypeGroup = chapterData.P4WCaseTypeGroup;
                    _SelectedChapter.SelectedStep = chapterData.SelectedStep;
                    _SelectedChapter.SelectedView = chapterData.SelectedView;
                    _SelectedChapter.ShowPartnerNotes = chapterData.ShowPartnerNotes;
                    _SelectedChapter.ShowDocumentTracking = chapterData.ShowDocumentTracking;

                    _SelectedChapterObject.SmartflowData = JsonConvert.SerializeObject(_SelectedChapter);
                    
                    await ChapterManagementService.Update(_SelectedChapterObject);


                    //keep track of time last updated ready for comparison by other sessions checking for updates
                    AppChapterState.SetLastUpdated(UserSession, _SelectedChapter);

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

       



        

        
        /****************************************/
        /* ERROR HANDLING AND NOTIFICATIONS     */
        /****************************************/
        private void GenericErrorLog(bool _showNotificationMsg, Exception e, string _method, string _message)
        {
            using (LogContext.PushProperty("SourceSystem", UserSession.SelectedSystem))
            using (LogContext.PushProperty("SourceCompanyId", UserSession.Company.Id))
            using (LogContext.PushProperty("SourceUserId", UserSession.User.Id))
            using (LogContext.PushProperty("SourceContext", nameof(ChapterHeaderDetail)))
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
                Class = "blazored-custom-modal modal-chapter-import"
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