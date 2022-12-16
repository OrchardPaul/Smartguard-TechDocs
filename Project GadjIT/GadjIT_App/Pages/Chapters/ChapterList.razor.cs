using GadjIT_ClientContext.P4W;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GadjIT_App.Services;
using System.Web;
using GadjIT_App.Services.SessionState;
using Blazored.Modal.Services;
using Blazored.Modal;
using GadjIT_App.Pages.Shared.Modals;
using GadjIT_ClientContext.P4W.Custom;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Hosting;
using BlazorInputFile;
using GadjIT_App.FileManagement.FileClassObjects.FileOptions;
using GadjIT_App.FileManagement.FileClassObjects;
using Microsoft.JSInterop;
using GadjIT_App.FileManagement.FileProcessing.Interface;
using GadjIT_App.Data.Admin;
using System.Globalization;
using AutoMapper;
using System.Timers;
using GadjIT_App.Services.AppState;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using Microsoft.Extensions.Configuration;
using System.Net.Http;

namespace GadjIT_App.Pages.Chapters
{
    public partial class ChapterList
    {
        private class ChapterColour
        {
            public string ColourName { get; set; }
            public string ColourCode { get; set; }
        }
        


        [Inject]
        IModalService Modal { get; set; }

        [Inject]
        public IMapper Mapper { get; set; }

        [Inject]
        private IJSRuntime JSRuntime {get; set;}

        [Inject]
        public INotificationManager NotificationManager {get; set;}

        [Inject]
        private IChapterManagementService ChapterManagementService { get; set; }

        [Inject]
        private IPartnerAccessService PartnerAccessService { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        public IUserSessionState UserSession { get; set; }


        [Inject]
        public IWebHostEnvironment WebHostEnvironment { get; set; }

        [Inject]
        private IChapterFileUpload ChapterFileUpload { get; set; }

        [Inject]
        private IIdentityUserAccess UserAccess { get; set; }

        [Inject]
        public IAppChapterState AppChapterState { get; set; }

        [Inject]
        private ICompanyDbAccess CompanyDbAccess { get; set; }

        [Inject]
        private ILogger<ChapterList> Logger { get; set; }


        [Inject]
        private IPageAuthorisationState pageAuthorisationState { get; set; }

        [Inject]
        public IConfiguration Configuration { get; set;}

        private ChapterFileOptions ChapterFileOption { get; set; }

        private List<FileDesc> ListFilesForBackups { get; set; }

        private List<FileDesc> ListFilesForBgImages { get; set; }

        private FileDesc SelectedFileDescription { get; set; }

        private int ValidTicketMessageCount { get; set; } 

        private List<VmUsrOrsfSmartflows> LstChapters { get; set; } = new List<VmUsrOrsfSmartflows>();

        private List<VmUsrOrsfSmartflows> LstAltSystemChapters { get; set; } = new List<VmUsrOrsfSmartflows>();

        private List<VmUsrOrDefChapterManagement> LstAllChapterItems { get; set; } = new List<VmUsrOrDefChapterManagement>();
        private List<VmUsrOrDefChapterManagement> LstAltSystemChapterItems { get; set; } = new List<VmUsrOrDefChapterManagement>();
        private List<VmFee> LstAltSystemFeeItems { get; set; } = new List<VmFee>();

        private List<VmDataViews> LstAltSystemDataViews { get; set; } = new List<VmDataViews>();

        private List<VmTickerMessages> LstAltSystemTickerMessages { get; set; } = new List<VmTickerMessages>();

        private List<VmUsrOrDefChapterManagement> LstAgendas { get; set; } = new List<VmUsrOrDefChapterManagement>();

        public List<VmUsrOrsfSmartflows> LstSelectedChapters { get; set; } = new List<VmUsrOrsfSmartflows>();
        private List<VmFee> LstFees { get; set; } = new List<VmFee>();
        private List<VmUsrOrDefChapterManagement> LstDocs { get; set; } = new List<VmUsrOrDefChapterManagement>();
        private List<VmUsrOrDefChapterManagement> LstStatus { get; set; } = new List<VmUsrOrDefChapterManagement>();

        private List<VmDataViews> ListVmDataViews { get; set; } = new List<VmDataViews>();
        private List<VmTickerMessages> ListVmTickerMessages { get; set; } = new List<VmTickerMessages>();

        public List<MpSysViews> ListP4WViews;
        public List<DmDocuments> DropDownChapterList;
        public List<TableDate> TableDates;
        public List<CaseTypeGroups> PartnerCaseTypeGroups;

        public string EditCaseType { get; set; } = "";
        public string UpdateJSON { get; set; } = "";
        public bool CreateNewSmartflow { get; set; }

        public float ScrollPosition { get; set; }

        public string SelectColour { 
            get 
            { 
                return SelectedChapter.BackgroundColourName; 
            }
            set
            {
                SaveSelectedBackgroundColour(value);
            }
        }

        public bool SmartflowComparison 
        {
            get 
            {
                return CompareSystems;
            }
            set
            {
                CompareSystems = value;
                if(CompareSystems)
                {
                    CompareAllChapters();
                }
                else{
                    StateHasChanged();
                }
            }
        }



        public String UserGuideURL => Configuration["AppSettings:UserGuideURL"];
        

        public UsrOrsfSmartflows EditChapter { get; set; }
        public string IsCaseTypeOrGroup { get; set; } = "";

        public VmDataViews EditDataViewObject = new VmDataViews { DataView = new DataViews() };
        public VmTickerMessages EditTickerMessageObject = new VmTickerMessages { Message = new TickerMessages() };
        
        public VmChapterComparison EditChapterComparison = new VmChapterComparison();

        public VmUsrOrDefChapterManagement EditObject = new VmUsrOrDefChapterManagement { ChapterObject = new GenSmartflowItem() };

        public LinkedItems AttachObject = new LinkedItems();

        public VmFee EditFeeObject = new VmFee { FeeObject = new Fee() };

        public UsrOrsfSmartflows EditChapterObject = new UsrOrsfSmartflows ();


        string SelectedList = string.Empty;

        string DisplaySection { get; set; } = "";

        [Parameter]
        public string UrlCaseTypeGroup { set { SelectedChapter.CaseTypeGroup = value; } }

        [Parameter]
        public string UrlCaseType { set { SelectedChapter.CaseType = value; } }

        [Parameter]
        public string UrlChapter { set { SelectedChapter.Name = value; } }

        [Parameter]
        public VmChapter SelectedChapter { get; set; } = new VmChapter { Items = new List<GenSmartflowItem>() };

        

        public VmChapter altChapter { get; set; } = new VmChapter();

        public UsrOrsfSmartflows SelectedChapterObject { get; set; } = new UsrOrsfSmartflows();

        public UsrOrsfSmartflows AltChapterObject { get; set; } = new UsrOrsfSmartflows();

        int RowChanged { get; set; } = 0;

        private int SelectedChapterId { get; set; } = -1;

        private int? altSysSelectedChapterId { get; set; }


        public string ModalInfoHeader { get; set; }
        public string ModalInfoText { get; set; }
        public string ModalHeight { get; set; }
        public string ModalWidth { get; set; }

        public string NavDisplay = "Chapter";

        private bool SeqMoving = false;

        public bool CompareSystems = false;

        private string RowChangedClass { get; set; } = "row-changed-nav3";


        public bool DisplaySpinner = true;

       
        public bool ListChapterLoaded = false;

        public string AlertMsgJSOM { get; set; }

        public bool ShowJSON = false;


        public IList<string> JsonErrors { get; set; }

        public ChapterP4WStepSchema ChapterP4WStep { get; set; }

        public bool ShowNewStep { get; set; } = false;

        [Inject]
        public IFileHelper FileHelper { get; set; }
        private List<ChapterColour> ListChapterColours { 
            get
            {
                List<ChapterColour> listChapterColours = new List<ChapterColour>
                {
                    new ChapterColour { ColourName = "", ColourCode = ""},
                    new ChapterColour { ColourName = "Grey", ColourCode = "#3F000000"},
                    new ChapterColour { ColourName = "Blue", ColourCode = "#3F0074FF"},
                    new ChapterColour { ColourName = "Pink", ColourCode = "#3FFD64EF"},
                    new ChapterColour { ColourName = "Peach", ColourCode = "#3FEA9C66"},
                    new ChapterColour { ColourName = "Yellow", ColourCode = "#3FFFFF00"},
                    new ChapterColour { ColourName = "Beige", ColourCode = "#3F957625"},
                    new ChapterColour { ColourName = "Lilac", ColourCode = "#3F6E6FDB"},
                    new ChapterColour { ColourName = "Green", ColourCode = "#3F32EC29"},
                    new ChapterColour { ColourName = "Aqua", ColourCode = "#3F5BDCD0"}
                };



                //foreach ( FileDesc bgImage in ListFileImages)
                //{
                //    string imgName = bgImage.FileName;
                //    string imgPath = @"/images/backgroundimages/" + imgName;
                //    //imgName = imgName.Replace(".jpg", "");
                //    listChapterColours.Add(new ChapterColour { ColourName = imgName, ColourCode = imgPath });
                //}
                return listChapterColours;
            }
        } 




        public bool PreviewChapterImage
        {
            get 
            {
                return UserSession.User.DisplaySmartflowPreviewImage; 
            }
            set
            {
                if (value)
                {
                    if(!string.IsNullOrEmpty(SelectedChapter.BackgroundImageName))
                    {
                        UserSession.SetTempBackground(SelectedChapter.BackgroundImage.Replace("/wwwroot",""), NavigationManager.Uri);
                    }
                    else
                    {
                        UserSession.SetTempBackground("", NavigationManager.Uri);
                    }
                }
                else
                {
                    UserSession.TempBackGroundImage = "";
                }

                UserSession.User.DisplaySmartflowPreviewImage = value;
                UserSession.RefreshHome?.Invoke();
                SavePreviewChapterImage();
            }

        }

        public bool PartnerShowNotes
        {
            get { return (SelectedChapter.ShowPartnerNotes == "Y" ? true : false); }
            set
            {
                if (value)
                {
                    SelectedChapter.ShowPartnerNotes = "Y";
                }
                else
                {
                    SelectedChapter.ShowPartnerNotes = "N";
                }
                
                SaveShowPartnerNotes();
            }

        }

        public bool ShowDocumentTracking
        {
            get { return (SelectedChapter.ShowDocumentTracking == "Y" ? true : false); }
            set
            {
                if (value)
                {
                    SelectedChapter.ShowDocumentTracking = "Y";
                }
                else
                {
                    SelectedChapter.ShowDocumentTracking = "N";
                }

                SaveAttachmentTracking();
            }

        }



#region Page Events
        protected override async Task OnInitializedAsync()
        {
            var authenticationState = await pageAuthorisationState.IsSignedIn();

 
            if (authenticationState)
            {
                bool gotLock = UserSession.Lock;
                while (gotLock)
                {
                    await Task.Yield();
                    gotLock = UserSession.Lock;
                }


                try
                {
                    RefreshChapters();
                    PartnerCaseTypeGroups = await PartnerAccessService.GetPartnerCaseTypeGroups();
                    ListP4WViews = await PartnerAccessService.GetPartnerViews();
                    UserSession.HomeActionSmartflow = SelectHome;

                }
                catch (Exception e)
                {
                    //Note: do not show notification as JsRuntime is not available until After Render
                    await GenericErrorLog(false,e, "OnInitializedAsync", $"Loading initial Smartflow list: {e.Message}");
                    
                }
            }

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
                    await GenericErrorLog(false,e, "OnAfterRenderAsync", e.Message);  
                }
            }
            else
            {
                timer = new Timer();
                timer.Interval = 10000; //10 seconds
                timer.Elapsed += OnTimerInterval;
                timer.AutoReset = true;
                // Start the timer
                timer.Enabled = true;
            }
            base.OnAfterRender(firstRender);
        }

        private async void OnTimerInterval(object sender, ElapsedEventArgs eventArgs)
        {
            //Compare current session with Chapter State to see if any other users have updated the Chapter
            //If an update is detected (ChapterLastUpdated is greater than session's ChapterLastCompared) then 
            // reload the data and invoke StateHasChanged to refresh the page.
            bool gotLock = AppChapterState.Lock;
            while (gotLock)
            {
                await Task.Yield();
                gotLock = AppChapterState.Lock;
            }

            gotLock = UserSession.Lock;
            while (gotLock)
            {
                await Task.Yield();
                gotLock = UserSession.Lock;
            }

            try
            {
                if (!(SelectedChapter is null))
                {
                    if (!(SelectedChapter.Name == "" | SelectedChapter.Name is null))
                    {
                        var ChapterLastUpdated = AppChapterState.GetLastUpdatedDate(UserSession, SelectedChapter);

                        if (UserSession.ChapterLastCompared != ChapterLastUpdated)
                        {
                            gotLock = CompanyDbAccess.Lock;
                            while (gotLock)
                            {
                                await Task.Yield();
                                gotLock = CompanyDbAccess.Lock;
                            }


                            var lstC = await ChapterManagementService.GetAllChapters();

                            await CompanyDbAccess.SyncAdminSysToClient(lstC, UserSession);

                            var id = SelectedChapterObject.Id;

                            var LstAllChapterItemsChapters = await CompanyDbAccess.GetAllSmartflowRecords(UserSession);
                            LstChapters = LstAllChapterItemsChapters.Select(A => new VmUsrOrsfSmartflows { SmartflowObject = Mapper.Map(A, new UsrOrsfSmartflows()) }).ToList();

                            SelectedChapterObject = LstChapters.Where(C => C.SmartflowObject.Id == id).Select(C => C.SmartflowObject).FirstOrDefault();

                            if (!(SelectedChapterObject.SmartflowData is null))
                            {
                                SelectedChapter = JsonConvert.DeserializeObject<VmChapter>(SelectedChapterObject.SmartflowData);
                            }
                            else
                            {
                                //Initialise the VmChapter in case of null Json
                                SelectedChapter = new VmChapter
                                {
                                    Items = new List<GenSmartflowItem>()
                                                                 ,
                                    DataViews = new List<DataViews>()
                                                                 ,
                                    TickerMessages = new List<TickerMessages>()
                                                                 ,
                                    Fees = new List<Fee>()
                                };
                                SelectedChapter.CaseTypeGroup = SelectedChapterObject.CaseTypeGroup;
                                SelectedChapter.CaseType = SelectedChapterObject.CaseType;
                                SelectedChapter.Name = SelectedChapterObject.SmartflowName;
                            }


                            await RefreshChapterItems("All");
                            await InvokeAsync(() =>
                            {
                                StateHasChanged();
                            });

                            UserSession.ChapterLastCompared = ChapterLastUpdated;
                        }
                    }
                }


                gotLock = AppChapterState.Lock;
                while (gotLock)
                {
                    await Task.Yield();
                    gotLock = AppChapterState.Lock;
                }

                AppChapterState.SetUsersCurrentChapter(UserSession, SelectedChapter);
            }
            catch(HttpRequestException)
            {
                //do nothing, already dealt with
            }
            catch (Exception e)
            {
                await GenericErrorLog(true,e, "OnTimerInterval", e.Message);
            }


        }

        public void Dispose()
        {
            // During prerender, this component is rendered without calling OnAfterRender and then immediately disposed
            // this mean timer will be null so we have to check for null or use the Null-conditional operator ? 
            timer?.Dispose();
            AppChapterState.DisposeUser(UserSession);
        }
#endregion

#region Navigation and Drag Drop

        public void DirectToLogin()
        {
            string returnUrl = HttpUtility.UrlEncode($"/");
            NavigationManager.NavigateTo($"Identity/Account/Login?returnUrl={returnUrl}", true);
        }

        public async void SelectHome()
        {
            try
            {
                //NavigationManager.NavigateTo($"Smartflow/{SelectedChapter.CaseTypeGroup}/{SelectedChapter.CaseType}",true);
                if (!string.IsNullOrEmpty(UserSession.TempBackGroundImage))
                {
                    UserSession.TempBackGroundImage = "";
                    UserSession.RefreshHome?.Invoke();
                }
                CompareSystems = false;
                SelectedChapter.Name = "";
                RowChanged = 0;

                StateHasChanged();


                await MovePos().ConfigureAwait(false);
            }
            catch (Exception e)
            {
                await GenericErrorLog(false,e, "SelectHome", e.Message);
            }
          
        }

        private void NavigateToChapter(UsrOrsfSmartflows chapter)
        {
            NavigationManager.NavigateTo($"Smartflow/{chapter.CaseTypeGroup}/{chapter.CaseType}/{chapter.SmartflowName}",true);
        }

        public async Task MovePos()
        {
            try
            {
                await Task.Delay(1);
                await JSRuntime.InvokeVoidAsync("moveToPosition", ScrollPosition);
            }
            catch (Exception e)
            {
                await GenericErrorLog(false,e, "MovePos", e.Message);
            }
        }


        /****************************************/
        /* DRAG DROP EVENTS */
        /****************************************/
        private int? droppedItem = 0;
        //private VmUsrOrsfSmartflows replacedItem = new VmUsrOrsfSmartflows {SmartflowObject = new UsrOrsfSmartflows { SeqNo = 0} };
        private int? replacedItem = 0;

        public async Task HandleDrop(VmUsrOrsfSmartflows smartflow)
        {
            droppedItem = smartflow.SmartflowObject.SeqNo;
            replacedItem = LstSelectedChapters.IndexOf(smartflow) + 1;

            int intAdjust = (droppedItem < replacedItem ? -1 : 1);

            var smartflowsToChange = LstSelectedChapters.Where(C => droppedItem < replacedItem ? C.SmartflowObject.SeqNo > droppedItem && C.SmartflowObject.SeqNo <= replacedItem
                                                                        : C.SmartflowObject.SeqNo < droppedItem && C.SmartflowObject.SeqNo >= replacedItem)
                                                .Select(C => { C.SmartflowObject.SeqNo += intAdjust; return C; })
                                                .ToList();

            foreach (var smartflowToChange in smartflowsToChange)
            {
                await ChapterManagementService.UpdateMainItem(smartflowToChange.SmartflowObject).ConfigureAwait(false);

            }


            smartflow.SmartflowObject.SeqNo = LstSelectedChapters.IndexOf(smartflow) + 1;

            await ChapterManagementService.UpdateMainItem(smartflow.SmartflowObject).ConfigureAwait(false);
            
            await InvokeAsync(() =>
            {
                StateHasChanged();
            })
            .ConfigureAwait(false);

            /*            
            foreach(var smartflowSelected in LstSelectedChapters)
            {
                smartflowSelected.SmartflowObject.SeqNo = LstSelectedChapters.IndexOf(smartflowSelected) + 1;
                await ChapterManagementService.UpdateMainItem(smartflowSelected.SmartflowObject).ConfigureAwait(false);
            }

            await InvokeAsync(() =>
            {
                StateHasChanged();
            })
            .ConfigureAwait(false);
            */
        }


        public async Task HandleDocDrop(VmUsrOrDefChapterManagement doc)
        {
            droppedItem = doc.ChapterObject.SeqNo;
            replacedItem = LstDocs.IndexOf(doc) + 1;

            int intAdjust = (droppedItem < replacedItem ? -1 : 1);

            var smartflowsToChange = LstDocs.Where(C => droppedItem < replacedItem ? C.ChapterObject.SeqNo > droppedItem && C.ChapterObject.SeqNo <= replacedItem
                                                                        : C.ChapterObject.SeqNo < droppedItem && C.ChapterObject.SeqNo >= replacedItem)
                                                .Select(C => { C.ChapterObject.SeqNo += intAdjust; return C; })
                                                .ToList();


            doc.ChapterObject.SeqNo = LstDocs.IndexOf(doc) + 1;

            SelectedChapterObject.SmartflowData = JsonConvert.SerializeObject(SelectedChapter);
            await ChapterManagementService.Update(SelectedChapterObject).ConfigureAwait(false);


            await InvokeAsync(() =>
            {
                StateHasChanged();
            });

            /*
            * 1 2 dropped = 1
            * 2 3 replaced = 3
            * 3 1 target = 2,3
            * 4 4
            *
            * */
            /*
            * 1 4 dropped = 4
            * 2 1 replaced = 1
            * 3 2 target = 1,2,3
            * 4 3
            *
            * */
        }
        

         /// <summary>
        /// swaps the CSS class for indicating that a row has changed.  
        /// This ensures that CSS recognises a new change even if the change occurs on the same row 
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <returns>string: row-changed or row-changedx</returns>
        private string ToggleRowChangedClass()
        {
            switch (RowChangedClass)
            {
                case "row-changed-nav3":
                    RowChangedClass = "row-changed-nav3x";
                    break;
                case "row-changed-nav3x":
                    RowChangedClass = "row-changed-nav3xx";
                    break;
                default:
                    RowChangedClass = "row-changed-nav3";
                    break;
            }

            return RowChangedClass;

        }

        public async Task<string> getHTMLColourFromAndroid(string colAndroid)
        {
            string colHTML = "#FFFFFFFF";

            try
            {

                if (!string.IsNullOrEmpty(colAndroid) && (Regex.IsMatch(colAndroid, "^#(?:[0-9a-fA-F]{8})$")))
                {
                    colHTML = "#" + colAndroid.Substring(3, 6) + colAndroid.Substring(1, 2);
                }
                else
                {
                    colHTML = "#FFFFFFFF";
                }

            }
            catch(Exception e)
            {
                await GenericErrorLog(false,e, "HandleBackupFileSelection", e.Message).ConfigureAwait(false);
            }

            return colHTML;
        }


        private bool SequenceIsValid(string listType)
        {
            if (SeqMoving == false | CompareSystems == true)
            {
                List<int?> listItems = GetSeqNumbers(listType);

                bool isValid = true;

                for (int i = 0; i < listItems.Count; i++)
                {
                    if (listItems[i] != i + 1)
                    {
                        isValid = false;
                    }

                }

                return isValid;
            }
            else
            {
                return true;
            }
        }

        private bool BlockNoIsValid()
        {
            if (SeqMoving == false | CompareSystems == true)
            {

                bool isValid = true;

                for (int i = 0; i < ListVmDataViews.Count; i++)
                {
                    if (ListVmDataViews[i].DataView.BlockNo != i + 1)
                    {
                        isValid = false;
                    }

                }

                return isValid;
            }
            else
            {
                return true;
            }
        }

        private bool FeeSeqNoIsValid()
        {
            if (SeqMoving == false | CompareSystems == true)
            {

                bool isValid = true;

                for (int i = 0; i < LstFees.Count; i++)
                {
                    if (LstFees[i].FeeObject.SeqNo != i + 1)
                    {
                        isValid = false;
                    }

                }

                return isValid;
            }
            else
            {
                return true;
            }
        }

        private bool MessagesSeqNoIsValid()
        {
            if (SeqMoving == false | CompareSystems == true)
            {

                bool isValid = true;

                for (int i = 0; i < ListVmTickerMessages.Count; i++)
                {
                    if (ListVmTickerMessages[i].Message.SeqNo != i + 1)
                    {
                        isValid = false;
                    }

                }

                return isValid;
            }
            else
            {
                return true;
            }
        }
        /// <summary>
        /// moves the AA (transparancy) element of an android hex color to the end of the string
        /// XAML Forms use Hex color but in format #aarrggbb
        /// HTML hex is in format #rrggbbaa
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <returns>string: row-changed or row-changedx</returns>


        public void ResetRowChanged()
        {
            RowChanged = 0;
        }


       public async Task ReSequenceSmartFlows(int seq)
        {
            RowChanged = seq;


            LstSelectedChapters.Select(C => { C.SmartflowObject.SeqNo = LstSelectedChapters.IndexOf(C) + 1; return C; }).ToList();

            foreach (var smartflowToChange in LstSelectedChapters)
            {
                await ChapterManagementService.UpdateMainItem(smartflowToChange.SmartflowObject).ConfigureAwait(false);

            }

            await InvokeAsync(() =>
            {
                StateHasChanged();
            });

        }


        public async Task ReSequenceDocs(int seq)
        {
            RowChanged = seq;

            LstDocs.Select(C => { C.ChapterObject.SeqNo = LstDocs.IndexOf(C) + 1; return C; }).ToList();

            SelectedChapterObject.SmartflowData = JsonConvert.SerializeObject(SelectedChapter);
            await ChapterManagementService.Update(SelectedChapterObject).ConfigureAwait(false);


            await InvokeAsync(() =>
            {
                StateHasChanged();
            });
        }

        public async Task ReSequenceStatus(int seq)
        {
            RowChanged = seq;

            LstStatus.Select(C => { C.ChapterObject.SeqNo = LstStatus.IndexOf(C) + 1; return C; }).ToList();

            SelectedChapterObject.SmartflowData = JsonConvert.SerializeObject(SelectedChapter);
            await ChapterManagementService.Update(SelectedChapterObject).ConfigureAwait(false);

            await InvokeAsync(() =>
            {
                StateHasChanged();
            });
        }

        public async Task ReSequenceFee(int seq)
        {
            RowChanged = seq;

            LstFees.Select(C => { C.FeeObject.SeqNo = LstFees.IndexOf(C) + 1; return C; }).ToList();

            SelectedChapterObject.SmartflowData = JsonConvert.SerializeObject(SelectedChapter);
            await ChapterManagementService.Update(SelectedChapterObject).ConfigureAwait(false);

            await InvokeAsync(() =>
            {
                StateHasChanged();
            });
        }

        public async Task ReSequenceDataViews(int seq)
        {
            RowChanged = seq;

            ListVmDataViews.Select(C => { C.DataView.BlockNo = ListVmDataViews.IndexOf(C) + 1; return C; }).ToList();

            SelectedChapterObject.SmartflowData = JsonConvert.SerializeObject(SelectedChapter);
            await ChapterManagementService.Update(SelectedChapterObject).ConfigureAwait(false);

            await InvokeAsync(() =>
            {
                StateHasChanged();
            });
        }

        public async Task ReSequenceMessages(int seq)
        {
            RowChanged = seq;

            ListVmTickerMessages.Select(C => { C.Message.SeqNo = ListVmTickerMessages.IndexOf(C) + 1; return C; }).ToList();

            SelectedChapterObject.SmartflowData = JsonConvert.SerializeObject(SelectedChapter);
            await ChapterManagementService.Update(SelectedChapterObject).ConfigureAwait(false);

            await InvokeAsync(() =>
            {
                StateHasChanged();
            });
        }

#endregion

#region Chapter Listing


        private async void RefreshChapters()
        {
            ListChapterLoaded = false;

            bool gotLock = CompanyDbAccess.Lock;
            while (gotLock)
            {
                await Task.Yield();
                gotLock = CompanyDbAccess.Lock;
            }

            try
            {
                var LstAllChapterItemsChapters = await CompanyDbAccess.GetAllSmartflowRecords(UserSession);
                //generate a copy of the Smartflow Records but converted to VmUsrOrsfSmartflows 
                LstChapters = LstAllChapterItemsChapters.Select(A => new VmUsrOrsfSmartflows { SmartflowObject = Mapper.Map(A, new UsrOrsfSmartflows()) }).ToList();

                LstSelectedChapters = LstChapters.Where(C => C.SmartflowObject.CaseType == SelectedChapter.CaseType)
                                        .Where(C => C.SmartflowObject.CaseTypeGroup == SelectedChapter.CaseTypeGroup)
                                        .OrderBy(C => C.SmartflowObject.SeqNo)
                                        .ToList();
            
                //TODO: run integrity check on list and rename any Smartflow that has a duplicate name e.g. Smartflow Name (2)
                //TODO: OR simply flag such issues in the list 

                if (!(SelectedChapter.Name is null) & SelectedChapter.Name != "") 
                {
                    //Single Smartflow selected, display details
                    PartnerCaseTypeGroups = await PartnerAccessService.GetPartnerCaseTypeGroups();
                    ListP4WViews = await PartnerAccessService.GetPartnerViews();

                    SelectChapter(LstSelectedChapters
                                        .Where(C => C.SmartflowObject.CaseTypeGroup == SelectedChapter.CaseTypeGroup)
                                        .Where(C => C.SmartflowObject.CaseType == SelectedChapter.CaseType)
                                        .Where(C => C.SmartflowObject.SmartflowName == SelectedChapter.Name)
                                        .Select(C => C.SmartflowObject)
                                        .FirstOrDefault()); //first instead of single to cater for duplicate Smartflow names
                }

            }
            catch(Exception e)
            {
                LstChapters = new List<VmUsrOrsfSmartflows>();
                LstSelectedChapters = new List<VmUsrOrsfSmartflows>();
                PartnerCaseTypeGroups = new List<CaseTypeGroups>();
                ListP4WViews = new List<MpSysViews>();

                await GenericErrorLog(true,e, "RefreshChapters", $"Refreshing Smartflow list: {e.Message}");
            }

            ListChapterLoaded = true;

            await InvokeAsync(() =>
            {
                StateHasChanged();
            });
        }

        private async Task<bool> RefreshChapterIssues()
        {
            try
            {
                
                if(!(LstSelectedChapters is null) && LstSelectedChapters.Count > 0)
                {
                    foreach (var chapter in LstSelectedChapters)
                    {
                        chapter.ComparisonList.Clear();

                        //check for duplicate Smartflow names
                        var numDuplicates = LstSelectedChapters
                                            .Where(A => A.SmartflowObject.SmartflowName == chapter.SmartflowObject.SmartflowName)
                                            .Where(A => A.SmartflowObject.CaseType == chapter.SmartflowObject.CaseType)
                                            .Where(A => A.SmartflowObject.CaseTypeGroup == chapter.SmartflowObject.CaseTypeGroup)
                                            .Where(A => A.SmartflowObject.SeqNo < chapter.SmartflowObject.SeqNo)
                                            .Count();
                                            
                        if (numDuplicates > 0)
                        {
                            chapter.ComparisonList.Add("Duplicate name");
                            chapter.ComparisonResult = "Duplicate name";
                            chapter.ComparisonIcon = "exclamation";
                        }
                            
                            
                        
                    }
                }
                
            }
            catch(Exception e)
            {
                await GenericErrorLog(true,e, "RefreshChapterIssues", $"Checking Smartflows for basic issues from current listing: {e.Message}");

                return false;
            }

            return true;
        }

        private async Task<bool> RefreshChapterItems(string listType)
        {
            try
            {
                if (listType == "Chapters")
                {
                    bool gotLock = CompanyDbAccess.Lock;
                    while (gotLock)
                    {
                        await Task.Yield();
                        gotLock = CompanyDbAccess.Lock;
                    }

                    var LstAllChapterItemsChapters = await CompanyDbAccess.GetAllSmartflowRecords(UserSession);
                    LstChapters = LstAllChapterItemsChapters.Select(A => new VmUsrOrsfSmartflows { SmartflowObject = Mapper.Map(A, new UsrOrsfSmartflows()) }).ToList();


                    LstSelectedChapters = LstChapters.Where(C => C.SmartflowObject.CaseType == SelectedChapter.CaseType)
                                            .Where(C => C.SmartflowObject.CaseTypeGroup == SelectedChapter.CaseTypeGroup)
                                            .OrderBy(C => C.SmartflowObject.SeqNo)
                                            .ToList();

                }
                else
                {
                    var lst = SelectedChapter.Items;
                    Dictionary<int?, string> docTypes = new Dictionary<int?, string> { { 1, "Doc" }, { 4, "Form" }, { 6, "Step" }, { 8, "Date" }, { 9, "Email" }, { 11, "Doc" }, { 12, "Email" }, { 13, "Csv" } };

                    LstAllChapterItems = lst.Select(L => new VmUsrOrDefChapterManagement { ChapterObject = L })
                                    .ToList();

                    /*
                     * listType = All when chapter is selected, 
                     * listType = nav selected e.g. Agenda when and object in a specific list has been altered
                     * 
                     */
                    if (listType == "Agenda" | listType == "All")
                    {
                        LstAgendas = LstAllChapterItems
                                            .OrderBy(A => A.ChapterObject.SeqNo)
                                            .Where(A => A.ChapterObject.Type == "Agenda")
                                            .ToList();

                    }
                    if (listType == "Docs" | listType == "All")
                    {

                        LstDocs = LstAllChapterItems
                                            .OrderBy(A => A.ChapterObject.SeqNo)
                                            .Where(A => A.ChapterObject.Type == "Doc")
                                            .Select(A => {
                                            //Make sure all Items have the DocType set by comparing against dm_documents for matches
                                            A.DocType = DropDownChapterList.Where(D => D.Name.ToUpper() == A.ChapterObject.Name.ToUpper())
                                                                                        .Select(D => docTypes.ContainsKey(D.DocumentType) ? docTypes[D.DocumentType] : "Doc")
                                                                                        .FirstOrDefault();
                                                A.ChapterObject.RescheduleDays = !string.IsNullOrEmpty(A.ChapterObject.AsName) && A.ChapterObject.RescheduleDays is null ? 0 : A.ChapterObject.RescheduleDays;
                                                A.ChapterObject.Action = (A.ChapterObject.Action == "" ? "INSERT" : A.ChapterObject.Action);
                                            //Make sure all Linked Items have the DocType set by comparing against dm_documents for matches
                                            A.ChapterObject.LinkedItems = A.ChapterObject.LinkedItems == null
                                                                                        ? null
                                                                                        : A.ChapterObject.LinkedItems
                                                                                                        .Select(L => {
                                                                                                            L.DocType = DropDownChapterList
                                                                                                                    .Where(D => D.Name.ToUpper() == L.DocName.ToUpper())
                                                                                                                    .Select(D => docTypes.ContainsKey(D.DocumentType) ? docTypes[D.DocumentType] : "Doc")
                                                                                                                    .FirstOrDefault();
                                                                                                            return L;
                                                                                                        }
                                                                                            ).ToList();
                                                return A;
                                            })
                                            .ToList();



                    }
                    if (listType == "Fees" | listType == "All")
                    {
                        LstFees = SelectedChapter.Fees.Select(F => new VmFee { FeeObject = F }).ToList();


                    }
                    if (listType == "Status" | listType == "All")
                    {
                        LstStatus = LstAllChapterItems
                                            .OrderBy(A => A.ChapterObject.SeqNo)
                                            .Where(A => A.ChapterObject.Type == "Status")
                                            .ToList();
                    }
                    if (listType == "DataViews" | listType == "All")
                    {
                        ListVmDataViews = (SelectedChapter.DataViews is null)
                                                        ? new List<VmDataViews>()
                                                        : SelectedChapter
                                                                .DataViews
                                                                .Select(D => new VmDataViews { DataView = D })
                                                                .OrderBy(D => D.DataView.BlockNo)
                                                                .ToList();
                    }
                    if (listType == "TickerMessages" | listType == "All")
                    {
                        ListVmTickerMessages = (SelectedChapter.TickerMessages is null)
                                                        ? new List<VmTickerMessages>()
                                                        : SelectedChapter
                                                                .TickerMessages
                                                                .Select(D => new VmTickerMessages { Message = D })
                                                                .OrderBy(D => D.Message.SeqNo)
                                                                .ToList();
                        TickerValidation();

                    }
                }
            }
            catch (Exception e)
            {
                await GenericErrorLog(true,e, "RefreshChapterItems", $"Refreshing Smartflow tab items: {e.Message}");

                return false;
            }
            
           

            return true;
        }

        public async Task SelectCaseTypeGroup(string caseTypeGroup)
        {
            try
            {
                SelectedChapter.CaseTypeGroup = (SelectedChapter.CaseTypeGroup == caseTypeGroup) ? "" : caseTypeGroup;
                SelectedChapter.CaseType = "";
                SelectedChapter.Name = "";
            }
            catch (Exception e)
            {
                await GenericErrorLog(true,e, "SelectCaseTypeGroup", $"Selecting Case Type Group: {e.Message}");
                
            }
        }

        public async Task SelectCaseType(string caseType)
        {
            try
            {
                SelectedChapter.CaseType = (SelectedChapter.CaseType == caseType) ? "" : caseType; 
                
                LstSelectedChapters = LstChapters.Where(C => C.SmartflowObject.CaseType == SelectedChapter.CaseType)
                                        .Where(C => C.SmartflowObject.CaseTypeGroup == SelectedChapter.CaseTypeGroup)
                                        .OrderBy(C => C.SmartflowObject.SeqNo)
                                        .ToList(); 
                

                SelectedChapter.Name = "";

                if(LstSelectedChapters.Count > 0)
                {
                    if(!(SequenceIsValid("Chapters")))
                    {
                        ReSeqSmartflows();
                    }

                    if(SmartflowComparison)
                    {
                        CompareAllChapters();
                    }
                    else
                    {
                        await RefreshChapterIssues();
                    }
                }
            }
            catch (Exception e)
            {
                await GenericErrorLog(false,e, "SelectCaseType", e.Message);
            }

        }

        private async void SelectChapter(UsrOrsfSmartflows chapter)
        {
            try
            {

                ScrollPosition = await JSRuntime.InvokeAsync<float>("getElementPosition");

                DisplaySpinner = true;

                LstAllChapterItems = new List<VmUsrOrDefChapterManagement>();

                SelectedChapterObject = chapter;

                if (!(chapter.SmartflowData is null))
                {
                    SelectedChapter = JsonConvert.DeserializeObject<VmChapter>(chapter.SmartflowData);

                }
                else
                {

                    //Initialise the VmChapter in case of null Json
                    SelectedChapter = new VmChapter
                    {
                        Items = new List<GenSmartflowItem>()
                                                     ,
                        DataViews = new List<DataViews>()
                                                     ,
                        TickerMessages = new List<TickerMessages>()
                                                     ,
                        Fees = new List<Fee>()
                    };
                    SelectedChapter.CaseTypeGroup = chapter.CaseTypeGroup;
                    SelectedChapter.CaseType = chapter.CaseType;
                    SelectedChapter.Name = chapter.SmartflowName;

                }

                SelectedChapter.StepName = $"SF {chapter.SmartflowName} Smartflow";
                SelectedChapter.SelectedStep = SelectedChapter.SelectedStep is null || SelectedChapter.SelectedStep == "Create New" ? "" : SelectedChapter.SelectedStep;
                SelectedChapter.Fees = SelectedChapter.Fees is null ? new List<Fee>() : SelectedChapter.Fees;
                SelectedChapter.Items = SelectedChapter.Items is null ? new List<GenSmartflowItem>() : SelectedChapter.Items;
                SelectedChapter.TickerMessages = SelectedChapter.TickerMessages is null ? new List<TickerMessages>() : SelectedChapter.TickerMessages;
                SelectedChapter.DataViews = SelectedChapter.DataViews is null ? new List<DataViews>() : SelectedChapter.DataViews;
                SelectedChapterId = chapter.Id;
                CompareSystems = false;
                RowChanged = 0;
                NavDisplay = "Chapter";
                ShowJSON = false;


                try
                {
                    DropDownChapterList = await ChapterManagementService.GetDocumentList(SelectedChapter.CaseType);
                    //DropDownChapterList = DropDownChapterList.Where(D => !(D.Name is null)).ToList();
                    TableDates = await ChapterManagementService.GetDatabaseTableDateFields();
                }
                catch (Exception e)
                {
                    await NotificationManager.ShowNotification("Danger", $"Oops! Something went wrong accessing the Smartflow details");

                    await GenericErrorLog(false,e, "SelectChapter", $"Getting document list after Smartflow selected for edit: {e.Message}");

                    DisplaySpinner = false;

                    //return user to the full list of Smartflows. Do not present the user with the details of the Smartflow as none of the features will work.
                    SelectHome();

                    return;
                }


                await RefreshChapterItems("All");


                //set path to point to the BackgroundImage path for the current company
                FileHelper.CustomPath = $"wwwroot/images/Companies/{UserSession.Company.CompanyName}/BackgroundImages";
                ListFilesForBgImages = FileHelper.GetFileList();

                if (!string.IsNullOrEmpty(SelectedChapter.BackgroundImage))
                {
                    UserSession.SetTempBackground(SelectedChapter.BackgroundImage.Replace("/wwwroot", ""), NavigationManager.Uri);
                    UserSession.RefreshHome?.Invoke();
                }

                SetSmartflowFilePath();
                GetSeletedChapterFileList();


                //set the session ChapterLastCompared to be the same as the Chapter State value to prevent an immediate 
                // refresh of the page (by OnTimerInterval)
                if (!(SelectedChapter.Name == "" | SelectedChapter.Name is null))
                {

                    bool gotLock = AppChapterState.Lock;
                    while (gotLock)
                    {
                        await Task.Yield();
                        gotLock = AppChapterState.Lock;
                    }

                    UserSession.ChapterLastCompared = AppChapterState.GetLastUpdatedDate(UserSession, SelectedChapter);
                }

                await JSRuntime.InvokeVoidAsync("moveToPosition", 0);

            }
            catch (Exception ex)
            {

                await GenericErrorLog(true,ex, "SelectChapter", $"Loading selected Smartflow: {ex.Message}");

            }
            finally
            {
                DisplaySpinner = false;

                StateHasChanged();


            }


        }

        private async void ReSeqSmartflows()
        {
            try
            {
                foreach(var smartflowSelected in LstSelectedChapters)
                {
                    smartflowSelected.SmartflowObject.SeqNo = LstSelectedChapters.IndexOf(smartflowSelected) + 1;
                    await ChapterManagementService.UpdateMainItem(smartflowSelected.SmartflowObject).ConfigureAwait(false);
                }
            }
            catch (Exception e)
            {
                await GenericErrorLog(false,e, "ReSeqSmartflows", $"Refreshing the Smartflow sequence numbers: {e.Message}");
            }
            
            await InvokeAsync(() =>
            {
                StateHasChanged();
            })
            .ConfigureAwait(false);
        }

        protected async void MoveSmartFlowSeq(UsrOrsfSmartflows selectobject, string listType, string direction)
        {
            try
            {
                SeqMoving = true; //prevents changes to the form whilst process of changing seq is carried out

                var lstItems = new List<VmUsrOrsfSmartflows>();
                int incrementBy;

                incrementBy = (direction.ToLower() == "up" ? -1 : 1);

                RowChanged = (int)(selectobject.SeqNo + incrementBy);

                lstItems = LstSelectedChapters
                            .OrderBy(A => A.SmartflowObject.SeqNo)
                            .ToList();


                var swapItem = lstItems.Where(D => D.SmartflowObject.SeqNo == (selectobject.SeqNo + incrementBy)).SingleOrDefault();
                if (!(swapItem is null))
                {
                    selectobject.SeqNo += incrementBy;
                    swapItem.SmartflowObject.SeqNo = swapItem.SmartflowObject.SeqNo + (incrementBy * -1);

                    await ChapterManagementService.UpdateMainItem(selectobject).ConfigureAwait(false);
                    await ChapterManagementService.UpdateMainItem(swapItem.SmartflowObject).ConfigureAwait(false);
                }



                await RefreshChapterItems(listType);
                await InvokeAsync(() =>
                {
                    StateHasChanged();
                });

            }
            catch (Exception e)
            {
                await GenericErrorLog(true,e, "MoveSmartFlowSeq", $"Moving smartflow: {e.Message}");

            }
            finally
            {
                SeqMoving = false;
            }



        }

        private async Task PrepNewChapter()
        {
            try
            {

                EditChapterObject = new UsrOrsfSmartflows();

                if (!(SelectedChapterObject.CaseTypeGroup == ""))
                {
                    EditChapterObject.CaseTypeGroup = SelectedChapter.CaseTypeGroup;
                }
                else
                {
                    EditChapterObject.CaseTypeGroup = "";
                }

                if (!(SelectedChapterObject.CaseType == ""))
                {
                    EditChapterObject.CaseType = SelectedChapter.CaseType;
                }
                else
                {
                    EditChapterObject.CaseType = "";
                }

                if (!string.IsNullOrWhiteSpace(SelectedChapter.CaseTypeGroup) & !string.IsNullOrWhiteSpace(SelectedChapter.CaseType))
                {
                    EditChapterObject.SeqNo = LstSelectedChapters
                                                    .OrderByDescending(C => C.SmartflowObject.SeqNo)
                                                    .Select(C => C.SmartflowObject.SeqNo)
                                                    .FirstOrDefault() + 1;
                }
                else
                {
                    EditChapterObject.SeqNo = 1;
                }
            }
            catch(Exception e)
            {
                await GenericErrorLog(false, e, "PrepNewChapter", e.Message);
            }
        }

        private void PrepareChapterForInsert()
        {
            PrepNewChapter();
            ShowChapterAddOrEditModel();
        }

        private void PrepareChapterForCopy()
        {
            PrepNewChapter();
            ShowChapterCopyModel();
        }

        private void PrepareCaseTypeForEdit(string caseType, string option)
        {
            EditCaseType = caseType;
            IsCaseTypeOrGroup = option;

            ShowCaseTypeEditModal();
        }

        private void PrepareChapterForEdit(UsrOrsfSmartflows chapter, string option)
        {
            EditChapter = chapter;
            IsCaseTypeOrGroup = option;

            ShowCaseTypeEditModal();
        }

        private async void PrepChapterList()
        {
            try
            {
                if (!(SelectedChapter.CaseType == ""))
                {
                    DropDownChapterList = await ChapterManagementService.GetDocumentList(SelectedChapterObject.CaseType);
                    TableDates = await ChapterManagementService.GetDatabaseTableDateFields();
                    StateHasChanged();
                }

            }
            catch(Exception e)
            {
                await GenericErrorLog(false, e, "PrepChapterList", e.Message);
            }
        }

        private List<VmUsrOrDefChapterManagement> GetRelevantChapterList(string listType)
        {
            var listItems = new List<VmUsrOrDefChapterManagement>();

            switch (listType)
            {
                case "Docs":
                    listItems = LstDocs;
                    break;
                case "Status":
                    listItems = LstStatus;
                    break;
            }

            return listItems;
        }

        public async void RefreshSelectedList()
        {
            await RefreshChapterItems("All");
            //CondenseSeq(NavDisplay);

            await InvokeAsync(() =>
            {
                StateHasChanged();
            });
        }

        
        private async void HandleChapterDelete()
        {
            await ChapterManagementService.Delete(EditChapter.Id);

            var chapterToRemove = LstSelectedChapters.Where(S => S.SmartflowObject.Id == EditChapter.Id).First();

            LstSelectedChapters.Remove(chapterToRemove);
            LstChapters.Remove(chapterToRemove);
            ReSeqSmartflows();
        }


        protected void PrepareChapterSync()
        {
            string infoText;

            switch (NavDisplay)
            {
                case "Chapter":
                    infoText = $"Make the {(UserSession.SelectedSystem == "Live" ? "Dev" : "Live")} system the same as {UserSession.SelectedSystem} for all chapter items.";
                    break;
                default:
                    infoText = $"Make the {(UserSession.SelectedSystem == "Live" ? "Dev" : "Live")} system the same as {UserSession.SelectedSystem} for all {NavDisplay}.";
                    break;
            }

            Action SelectedAction = HandleChapterSync;
            var parameters = new ModalParameters();
            parameters.Add("InfoHeader", "Confirm Action");
            parameters.Add("InfoText", infoText);
            parameters.Add("ConfirmAction", SelectedAction);

            var options = new ModalOptions()
            {
                Class = "blazored-custom-modal modal-confirm"
            };

            Modal.Show<ModalConfirm>("Confirm", parameters, options);
        }

#endregion

#region Chapter Details

        private async void SaveAttachmentTracking()
        {
            try
            {
                SelectedChapterObject.SmartflowData = JsonConvert.SerializeObject(SelectedChapter);

                await ChapterManagementService.Update(SelectedChapterObject).ConfigureAwait(false);

                //keep track of time last updated ready for comparison by other sessions checking for updates
                AppChapterState.SetLastUpdated(UserSession, SelectedChapter);
            }
            catch (Exception e)
            {

                await GenericErrorLog(true,e, "SaveAttachmentTracking", $"Toggling document tracking: {e.Message}");

            }
        }

        public async void SaveSelectedBackgroundColour (string colour)
        {
            try
            {
                SelectedChapter.BackgroundColour = ListChapterColours.Where(C => C.ColourName == colour).Select(C => C.ColourCode).FirstOrDefault();
                SelectedChapter.BackgroundColourName = colour;

                //if (ListFileImages.Select(I => I.FileName).ToList().Contains(colour))
                //{
                //    UserSession.SetTempBackground(ListFileImages.Where(I => I.FileName == colour).Select(F => F.FileDirectory.Replace("\\", "/").Replace("wwwroot/", "") + "/" + F.FileName).SingleOrDefault(), NavigationManager.Uri);
                //}
                //else
                //{
                //    UserSession.SetTempBackground(SelectedChapter.BackgroundColour, NavigationManager.Uri);
                //}

                SelectedChapterObject.SmartflowData = JsonConvert.SerializeObject(SelectedChapter);

                await ChapterManagementService.Update(SelectedChapterObject).ConfigureAwait(false);


                //keep track of time last updated ready for comparison by other sessions checking for updates
                AppChapterState.SetLastUpdated(UserSession, SelectedChapter);
            }
            catch (Exception e)
            {
                await GenericErrorLog(true,e, "SaveSelectedBackgroundColour", $"Saving background colour: {e.Message}");
            }
        }

        private async void SavePreviewChapterImage()
        {
            bool gotLock = UserAccess.Lock;
            while (gotLock)
            {
                await Task.Yield();
                gotLock = UserAccess.Lock;
            }

            try
            {
                await UserAccess.UpdateUserDetails(UserSession.User).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                await GenericErrorLog(true,e, "SavePreviewChapterImage", e.Message);
            }
        }

        private async void SaveShowPartnerNotes()
        {
            try
            {
                SelectedChapterObject.SmartflowData = JsonConvert.SerializeObject(SelectedChapter);

                await ChapterManagementService.Update(SelectedChapterObject).ConfigureAwait(false);

                //keep track of time last updated ready for comparison by other sessions checking for updates
                AppChapterState.SetLastUpdated(UserSession, SelectedChapter);
            }
            catch (Exception e)
            {
                await GenericErrorLog(true,e, "SaveShowPartnerNotes", $"Saving notes: {e.Message}");
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
                if (SelectedChapter.P4WCaseTypeGroup == "Entity Documents")
                {
                    ChapterP4WStep = new ChapterP4WStepSchema
                    {
                        StepName = SelectedChapter.StepName,
                        P4WCaseTypeGroup = SelectedChapter.P4WCaseTypeGroup,
                        GadjITCaseTypeGroup = SelectedChapter.CaseTypeGroup,
                        GadjITCaseType = SelectedChapter.CaseType,
                        Smartflow = SelectedChapter.Name,
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
                                        new ChapterP4WStepAnswer {QNo = 1, GoToData= $"2 [SQL: EXEC Up_ORSF_MoveDocsToAgendas '[entity.code]', 0] [SQL: EXEC up_ORSF_CreateTableEntries '[entity.code]', 0] [SQL: UPDATE Usr_ORSF_ENT_Control SET Current_SF = '{SelectedChapter.Name}', Current_Case_Type_Group = '{SelectedChapter.CaseTypeGroup}', Current_Case_Type = '{SelectedChapter.CaseType}', Default_Step = '{SelectedChapter.StepName}', Date_Schedule_For = DATEADD(d, 7, getdate()), Steps_To_Run = '', Schedule_AsName = '{SelectedChapter.StepName}|' + (SELECT CASE WHEN dbo.fn_ORSF_IsAllCap (Description) = 1 THEN '{SelectedChapter.StepName}' + '|' + CONVERT(VARCHAR(20),ISNULL(Date_Schedule_For,DATEADD(d, 7, getdate())),103) ELSE Description + '|' + CONVERT(VARCHAR(20),ISNULL(s.DiaryDate, getdate()),103) END + '|' + '[CurrentUser.Code]' FROM Cm_CaseItems i INNER JOIN Cm_Steps s on s.ItemID = i.ItemID WHERE i.ItemID = [currentstep.stepid]), Complete_AsName = '' WHERE EntityRef = '[Entity.Code]'] [SQL: UPDATE Usr_ORSF_ENT_Control SET Screen_Opened_Via_Step = 'Y' WHERE EntityRef = '[Entity.Code]' ]" }
                                        ,new ChapterP4WStepAnswer {QNo = 2, GoToData= $"7 [VIEW: '{SelectedChapter.SelectedView}' UPDATE=Yes]" }
                                        ,new ChapterP4WStepAnswer {QNo = 3, GoToData= $"4 [TAKE: 'SF-Admin Save Items for Agenda Management' INSERT=END]  [SQL: EXEC up_ORSF_GetStepsFromList '[entity.code]', 0] [SQL: UPDATE Usr_ORSF_ENT_Control SET Screen_Opened_Via_Step = null WHERE EntityRef='[entity.code]']" }
                                        ,new ChapterP4WStepAnswer {QNo = 4, GoToData= $"5 [SQL: UPDATE Usr_ORSF_ENT_Control SET Date_Schedule_For = isnull(Date_Schedule_For, Cast(getdate() as Date)) WHERE EntityRef = '[Entity.Code]']" }
                                        ,new ChapterP4WStepAnswer {QNo = 5, GoToData= $"8 [SQL: SELECT ScheduleCommand FROM fn_ORSF_GetScheduleItems('[Entity.Code]', 0)]" }
                                        ,new ChapterP4WStepAnswer {QNo = 6, GoToData= $"[SQL: UPDATE cm_caseitems set CompletionDate = GETDATE(), description = UPPER('[!Usr_ORSF_ENT_Control.Complete_AsName]') where itemid = [currentstep.stepid]] [SQL: exec up_ORSF_CompleteStep [currentStep.stepid], '', 'Y']" }
                                        ,new ChapterP4WStepAnswer {QNo = 7, GoToData= $"3 [[SQL: SELECT CASE WHEN ISNULL(Do_Not_Reschedule,'N') = 'Y' THEN 'SQL: exec up_ORSF_DeleteDueStep '''', [currentstep.stepid], ''{SelectedChapter.Name}''' ELSE '' END FROM Usr_ORSF_MT_Control WHERE EntityRef = '[matters.entityref]' AND MatterNo = [matters.number]]]" }
                                        ,new ChapterP4WStepAnswer {QNo = 8, GoToData= $"[SQL: SELECT CASE WHEN ISNULL('[!Usr_ORSF_ENT_Control.Complete_AsName]','') <> '' THEN 6 ELSE 9 END]" }
                                        ,new ChapterP4WStepAnswer {QNo = 9, GoToData= $"[SQL: exec up_ORSF_DeleteStep [currentstep.stepid]]" }
                                        }
                    };
                }
                else
                {
                    ChapterP4WStep = new ChapterP4WStepSchema
                    {
                        StepName = SelectedChapter.StepName,
                        P4WCaseTypeGroup = SelectedChapter.P4WCaseTypeGroup,
                        GadjITCaseTypeGroup = SelectedChapter.CaseTypeGroup,
                        GadjITCaseType = SelectedChapter.CaseType,
                        Smartflow = SelectedChapter.Name,
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
                                        new ChapterP4WStepAnswer {QNo = 1, GoToData= $"2 [SQL: EXEC Up_ORSF_MoveDocsToAgendas '[matters.entityref]', [matters.number]] [SQL: EXEC up_ORSF_CreateTableEntries '[matters.entityref]', [matters.number]] [SQL: UPDATE Usr_ORSF_MT_Control SET Current_SF = '{SelectedChapter.Name}', Current_Case_Type_Group = '{SelectedChapter.CaseTypeGroup}', Current_Case_Type = '{SelectedChapter.CaseType}', Default_Step = '{SelectedChapter.StepName}', Date_Schedule_For = DATEADD(d, 7, getdate()), Steps_To_Run = '', Schedule_AsName = '{SelectedChapter.StepName}|' + (SELECT CASE WHEN dbo.fn_ORSF_IsAllCap (Description) = 1 THEN '{SelectedChapter.StepName}' + '|' + CONVERT(VARCHAR(20),ISNULL(Date_Schedule_For,DATEADD(d, 7, getdate())),103) ELSE Description + '|' + CONVERT(VARCHAR(20),ISNULL(s.DiaryDate, getdate()),103) END + '|' + '[matters.feeearnerref]' FROM Cm_CaseItems i INNER JOIN Cm_Steps s on s.ItemID = i.ItemID WHERE i.ItemID = [currentstep.stepid]), Complete_AsName = '' WHERE EntityRef = '[matters.entityref]' AND MatterNo = [matters.number]] [SQL: UPDATE Usr_ORSF_MT_Control SET Screen_Opened_Via_Step = 'Y' WHERE EntityRef = '[matters.entityref]' AND matterNo =[matters.number]]" }
                                        ,new ChapterP4WStepAnswer {QNo = 2, GoToData= $"7 [VIEW: '{SelectedChapter.SelectedView}' UPDATE=Yes]" }
                                        ,new ChapterP4WStepAnswer {QNo = 3, GoToData= $"4 [TAKE: 'SF-Admin Save Items for Agenda Management' INSERT=END]  [SQL: EXEC up_ORSF_GetStepsFromList '[matters.entityref]', [matters.number]] [SQL: UPDATE Usr_ORSF_MT_Control SET Screen_Opened_Via_Step = null WHERE EntityRef='[matters.entityref]' AND matterNo=[matters.number]]" }
                                        ,new ChapterP4WStepAnswer {QNo = 4, GoToData= $"5 [SQL: UPDATE Usr_ORSF_MT_Control SET Date_Schedule_For = isnull(Date_Schedule_For, Cast(getdate() as Date)) WHERE EntityRef = '[matters.entityref]' AND MatterNo = [matters.number]]" }
                                        ,new ChapterP4WStepAnswer {QNo = 5, GoToData= $"8 [SQL: SELECT ScheduleCommand FROM fn_ORSF_GetScheduleItems('[matters.entityref]', [matters.number])]" }
                                        ,new ChapterP4WStepAnswer {QNo = 6, GoToData= $"[SQL: UPDATE cm_caseitems set CompletionDate = GETDATE(), description = UPPER('[!Usr_ORSF_MT_Control.Complete_AsName]') where itemid = [currentstep.stepid]] [SQL: exec up_ORSF_CompleteStep [currentStep.stepid], '', 'Y']" }
                                        ,new ChapterP4WStepAnswer {QNo = 7, GoToData= $"3 [[SQL: SELECT CASE WHEN ISNULL(Do_Not_Reschedule,'N') = 'Y' THEN 'SQL: exec up_ORSF_DeleteDueStep '''', [currentstep.stepid], ''{SelectedChapter.Name}''' ELSE '' END FROM Usr_ORSF_MT_Control WHERE EntityRef = '[matters.entityref]' AND MatterNo = [matters.number]]]" }
                                        ,new ChapterP4WStepAnswer {QNo = 8, GoToData= $"[SQL: SELECT CASE WHEN ISNULL('[!Usr_ORSF_MT_Control.Complete_AsName]','') <> '' THEN 6 ELSE 9 END]" }
                                        ,new ChapterP4WStepAnswer {QNo = 9, GoToData= $"[SQL: exec up_ORSF_DeleteStep [currentstep.stepid]]" }
                                        }
                    };
                }

                string stepJSON = JsonConvert.SerializeObject(ChapterP4WStep);

                bool creationSuccess;

                creationSuccess = await ChapterManagementService.CreateStep(new VmChapterP4WStepSchemaJSONObject { StepSchemaJSON = stepJSON });

                if (creationSuccess)
                {
                    DropDownChapterList = await ChapterManagementService.GetDocumentList(SelectedChapter.CaseType);
                    TableDates = await ChapterManagementService.GetDatabaseTableDateFields();

                    SelectedChapter.SelectedStep = SelectedChapter.StepName;

                    SelectedChapterObject.SmartflowData = JsonConvert.SerializeObject(SelectedChapter);

                    bool gotLock = ChapterManagementService.Lock;
                    while (gotLock)
                    {
                        await Task.Yield();
                        gotLock = ChapterManagementService.Lock;
                    }

                    await ChapterManagementService.Update(SelectedChapterObject);

                    //keep track of time last updated ready for comparison by other sessions checking for updates
                    AppChapterState.SetLastUpdated(UserSession, SelectedChapter);

                    StateHasChanged();
                }
            }
            catch (Exception e)
            {
                await GenericErrorLog(true,e, "CreateStep", $"Creating/Updating current P4W Smartflow step: {e.Message}");
                
            }
        }

        /// <summary>
        /// Update all P4W steps for the current system
        /// </summary>
        /// <returns></returns>
        private async Task UpdateSteps()
        {
            bool overAllCreationSuccess = true;
            bool creationSuccess = true;
            int creationCount = 0;
            string stepJSON = "";

            try
            {
                foreach (var chapter in LstChapters)
                {
                    var decodedChapter = JsonConvert.DeserializeObject<VmChapter>(chapter.SmartflowObject.SmartflowData);

                    if (!string.IsNullOrEmpty(decodedChapter.SelectedStep) 
                        && !string.IsNullOrEmpty(decodedChapter.SelectedView)
                        && !string.IsNullOrEmpty(decodedChapter.Name)
                        && !string.IsNullOrEmpty(decodedChapter.StepName)
                        && !string.IsNullOrEmpty(decodedChapter.P4WCaseTypeGroup)
                        && !string.IsNullOrEmpty(decodedChapter.CaseTypeGroup))
                    {
                        if (decodedChapter.P4WCaseTypeGroup == "Entity Documents")
                        {
                            ChapterP4WStep = new ChapterP4WStepSchema
                            {
                                StepName = decodedChapter.StepName,
                                P4WCaseTypeGroup = decodedChapter.P4WCaseTypeGroup,
                                GadjITCaseTypeGroup = decodedChapter.CaseTypeGroup,
                                GadjITCaseType = decodedChapter.CaseType,
                                Smartflow = decodedChapter.Name,
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
                                        new ChapterP4WStepAnswer {QNo = 1, GoToData= $"2 [SQL: EXEC Up_ORSF_MoveDocsToAgendas '[entity.code]', 0] [SQL: EXEC up_ORSF_CreateTableEntries '[entity.code]', 0] [SQL: UPDATE Usr_ORSF_ENT_Control SET Current_SF = '{decodedChapter.Name}', Current_Case_Type_Group = '{decodedChapter.CaseTypeGroup}', Current_Case_Type = '{decodedChapter.CaseType}', Default_Step = '{decodedChapter.StepName}', Date_Schedule_For = DATEADD(d, 7, getdate()), Steps_To_Run = '', Schedule_AsName = '{decodedChapter.StepName}|' + (SELECT CASE WHEN dbo.fn_ORSF_IsAllCap (Description) = 1 THEN '{decodedChapter.StepName}' + '|' + CONVERT(VARCHAR(20),ISNULL(Date_Schedule_For,DATEADD(d, 7, getdate())),103) ELSE Description + '|' + CONVERT(VARCHAR(20),ISNULL(s.DiaryDate, getdate()),103) END + '|' + '[CurrentUser.Code]' FROM Cm_CaseItems i INNER JOIN Cm_Steps s on s.ItemID = i.ItemID WHERE i.ItemID = [currentstep.stepid]), Complete_AsName = '' WHERE EntityRef = '[Entity.Code]'] [SQL: UPDATE Usr_ORSF_ENT_Control SET Screen_Opened_Via_Step = 'Y' WHERE EntityRef = '[Entity.Code]' ]" }
                                        ,new ChapterP4WStepAnswer {QNo = 2, GoToData= $"7 [VIEW: '{decodedChapter.SelectedView}' UPDATE=Yes]" }
                                        ,new ChapterP4WStepAnswer {QNo = 3, GoToData= $"4 [TAKE: 'SF-Admin Save Items for Agenda Management' INSERT=END]  [SQL: EXEC up_ORSF_GetStepsFromList '[entity.code]', 0] [SQL: UPDATE Usr_ORSF_ENT_Control SET Screen_Opened_Via_Step = null WHERE EntityRef='[entity.code]']" }
                                        ,new ChapterP4WStepAnswer {QNo = 4, GoToData= $"5 [SQL: UPDATE Usr_ORSF_ENT_Control SET Date_Schedule_For = isnull(Date_Schedule_For, Cast(getdate() as Date)) WHERE EntityRef = '[Entity.Code]']" }
                                        ,new ChapterP4WStepAnswer {QNo = 5, GoToData= $"8 [SQL: SELECT ScheduleCommand FROM fn_ORSF_GetScheduleItems('[Entity.Code]', 0)]" }
                                        ,new ChapterP4WStepAnswer {QNo = 6, GoToData= $"[SQL: UPDATE cm_caseitems set CompletionDate = GETDATE(), description = UPPER('[!Usr_ORSF_ENT_Control.Complete_AsName]') where itemid = [currentstep.stepid]] [SQL: exec up_ORSF_CompleteStep [currentStep.stepid], '', 'Y']" }
                                        ,new ChapterP4WStepAnswer {QNo = 7, GoToData= $"3 [[SQL: SELECT CASE WHEN ISNULL(Do_Not_Reschedule,'N') = 'Y' THEN 'SQL: exec up_ORSF_DeleteDueStep '''', [currentstep.stepid], ''{decodedChapter.Name}''' ELSE '' END FROM Usr_ORSF_MT_Control WHERE EntityRef = '[matters.entityref]' AND MatterNo = [matters.number]]]" }
                                        ,new ChapterP4WStepAnswer {QNo = 8, GoToData= $"[SQL: SELECT CASE WHEN ISNULL('[!Usr_ORSF_ENT_Control.Complete_AsName]','') <> '' THEN 6 ELSE 9 END]" }
                                        ,new ChapterP4WStepAnswer {QNo = 9, GoToData= $"[SQL: exec up_ORSF_DeleteStep [currentstep.stepid]]" }
                                        }
                            };
                        }
                        else
                        {
                            ChapterP4WStep = new ChapterP4WStepSchema
                            {
                                StepName = decodedChapter.StepName,
                                P4WCaseTypeGroup = decodedChapter.P4WCaseTypeGroup,
                                GadjITCaseTypeGroup = decodedChapter.CaseTypeGroup,
                                GadjITCaseType = decodedChapter.CaseType,
                                Smartflow = decodedChapter.Name,
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
                                        new ChapterP4WStepAnswer {QNo = 1, GoToData= $"2 [SQL: EXEC Up_ORSF_MoveDocsToAgendas '[matters.entityref]', [matters.number]] [SQL: EXEC up_ORSF_CreateTableEntries '[matters.entityref]', [matters.number]] [SQL: UPDATE Usr_ORSF_MT_Control SET Current_SF = '{decodedChapter.Name}', Current_Case_Type_Group = '{decodedChapter.CaseTypeGroup}', Current_Case_Type = '{decodedChapter.CaseType}', Default_Step = '{decodedChapter.StepName}', Date_Schedule_For = DATEADD(d, 7, getdate()), Steps_To_Run = '', Schedule_AsName = '{decodedChapter.StepName}|' + (SELECT CASE WHEN dbo.fn_ORSF_IsAllCap (Description) = 1 THEN '{decodedChapter.StepName}' + '|' + CONVERT(VARCHAR(20),ISNULL(Date_Schedule_For,DATEADD(d, 7, getdate())),103) ELSE Description + '|' + CONVERT(VARCHAR(20),ISNULL(s.DiaryDate, getdate()),103) END + '|' + '[matters.feeearnerref]' FROM Cm_CaseItems i INNER JOIN Cm_Steps s on s.ItemID = i.ItemID WHERE i.ItemID = [currentstep.stepid]), Complete_AsName = '' WHERE EntityRef = '[matters.entityref]' AND MatterNo = [matters.number]] [SQL: UPDATE Usr_ORSF_MT_Control SET Screen_Opened_Via_Step = 'Y' WHERE EntityRef = '[matters.entityref]' AND matterNo =[matters.number]]" }
                                        ,new ChapterP4WStepAnswer {QNo = 2, GoToData= $"7 [VIEW: '{decodedChapter.SelectedView}' UPDATE=Yes]" }
                                        ,new ChapterP4WStepAnswer {QNo = 3, GoToData= $"4 [TAKE: 'SF-Admin Save Items for Agenda Management' INSERT=END]  [SQL: EXEC up_ORSF_GetStepsFromList '[matters.entityref]', [matters.number]] [SQL: UPDATE Usr_ORSF_MT_Control SET Screen_Opened_Via_Step = null WHERE EntityRef='[matters.entityref]' AND matterNo=[matters.number]]" }
                                        ,new ChapterP4WStepAnswer {QNo = 4, GoToData= $"5 [SQL: UPDATE Usr_ORSF_MT_Control SET Date_Schedule_For = isnull(Date_Schedule_For, Cast(getdate() as Date)) WHERE EntityRef = '[matters.entityref]' AND MatterNo = [matters.number]]" }
                                        ,new ChapterP4WStepAnswer {QNo = 5, GoToData= $"8 [SQL: SELECT ScheduleCommand FROM fn_ORSF_GetScheduleItems('[matters.entityref]', [matters.number])]" }
                                        ,new ChapterP4WStepAnswer {QNo = 6, GoToData= $"[SQL: UPDATE cm_caseitems set CompletionDate = GETDATE(), description = UPPER('[!Usr_ORSF_MT_Control.Complete_AsName]') where itemid = [currentstep.stepid]] [SQL: exec up_ORSF_CompleteStep [currentStep.stepid], '', 'Y']" }
                                        ,new ChapterP4WStepAnswer {QNo = 7, GoToData= $"3 [[SQL: SELECT CASE WHEN ISNULL(Do_Not_Reschedule,'N') = 'Y' THEN 'SQL: exec up_ORSF_DeleteDueStep '''', [currentstep.stepid], ''{decodedChapter.Name}''' ELSE '' END FROM Usr_ORSF_MT_Control WHERE EntityRef = '[matters.entityref]' AND MatterNo = [matters.number]]]" }
                                        ,new ChapterP4WStepAnswer {QNo = 8, GoToData= $"[SQL: SELECT CASE WHEN ISNULL('[!Usr_ORSF_MT_Control.Complete_AsName]','') <> '' THEN 6 ELSE 9 END]" }
                                        ,new ChapterP4WStepAnswer {QNo = 9, GoToData= $"[SQL: exec up_ORSF_DeleteStep [currentstep.stepid]]" }
                                        }
                            };
                        }

                        stepJSON = JsonConvert.SerializeObject(ChapterP4WStep);

                        

                        creationSuccess = await ChapterManagementService.CreateStep(new VmChapterP4WStepSchemaJSONObject { StepSchemaJSON = stepJSON });

                        if (!creationSuccess)
                        {
                            overAllCreationSuccess = false;
                        }

                        creationCount += 1;

                    }

                }

                //Refresh the global step that saves "Completed" docs into the relevant Agendas
                //This step will be called by each Smartflow
                ChapterP4WStep = new ChapterP4WStepSchema
                {
                    StepName = "SF-Admin Save Items for Agenda Management",
                    P4WCaseTypeGroup = "Global Documents",
                    GadjITCaseTypeGroup = "Global",
                    GadjITCaseType = "Global Documents",
                    Smartflow = "Admin Save Items for Agenda Management",
                    SFVersion = Configuration["AppSettings:Version"],
                    Questions = new List<ChapterP4WStepQuestion>{
                            new ChapterP4WStepQuestion {QNo = 1, QText= "HQ - Save Items for Agenda Management" }
                            ,new ChapterP4WStepQuestion {QNo = 2, QText= "HQ - Delete Me" }
                            },
                    Answers = new List<ChapterP4WStepAnswer>{
                            new ChapterP4WStepAnswer {QNo = 1, GoToData= $"2 [SQL: exec up_ORSF_Agenda_Control '[matters.entityref]', [matters.number], [currentstep.stepid] ]" }
                            ,new ChapterP4WStepAnswer {QNo = 2, GoToData= $"[SQL: EXEC dbo.up_ORSF_DeleteStep [currentstep.stepid]]" }
                            }
                };

                stepJSON = JsonConvert.SerializeObject(ChapterP4WStep);

                creationSuccess = await ChapterManagementService.CreateStep(new VmChapterP4WStepSchemaJSONObject { StepSchemaJSON = stepJSON });

                if (!creationSuccess)
                {
                    overAllCreationSuccess = false;
                }

                var parameters = new ModalParameters();
                var message = overAllCreationSuccess ? "Creation Successfull" : "Creation Unsuccessfull";
                parameters.Add("InfoText", message);

                var options = new ModalOptions()
                {
                    Class = "blazored-custom-modal modal-chapter-chapter"
                };
                string title = $"Updated {creationCount} Steps";
                Modal.Show<ModalInfo>(title, parameters, options);
            }
            catch (Exception e)
            {
                await GenericErrorLog(true,e, "UpdateSteps", $"Creating/Updating all P4W Smartflow steps: {e.Message}");

            }

        }

        private async void RefreshDocList()
        {
            try
            {
                DropDownChapterList = await ChapterManagementService.GetDocumentList(SelectedChapter.CaseType);
                DropDownChapterList = DropDownChapterList.Where(D => !(D.Name is null)).ToList();
                StateHasChanged();
            }
            catch (Exception ex)
            {
                await GenericErrorLog(true,ex, "DisplaySmartflowLoadError", $"Refreshing the document list: {ex.Message}");

            }
        }

        /// <summary>
        /// Saves the Smartflow main details after changes to the Home tab
        /// </summary>
        /// <returns></returns>
        private async void SaveChapterDetails()
        {
            try
            {
                SelectedChapterObject.SmartflowName = SelectedChapter.Name;

                SelectedChapterObject.SmartflowData = JsonConvert.SerializeObject(SelectedChapter);
                await ChapterManagementService.Update(SelectedChapterObject).ConfigureAwait(false);


                //keep track of time last updated ready for comparison by other sessions checking for updates
                AppChapterState.SetLastUpdated(UserSession, SelectedChapter);

            }
            catch (Exception e)
            {
                await GenericErrorLog(true,e, "SaveChapterDetails", $"Saving Smartflow base details: {e.Message}");
            }

            await RefreshChapterItems("All");
            await InvokeAsync(() =>
            {
                StateHasChanged();
            });
        }

        protected void ShowNav(string displayChange)
        {
            CompareSystems = false;
            RowChanged = 0;
            NavDisplay = displayChange;
        }


        /// <summary>
        /// Moves a sequecnce item up or down a list of type [GenSmartflowItem]
        /// </summary>
        /// <remarks>
        /// <para>Up: swaps the item with the preceding item in the lest by reducing sequence number by 1 </para>
        /// <para>Up: swaps the item with the following item in the lest by increasing sequence number by 1 </para>
        /// </remarks>
        /// <param name="selectobject">: current list item</param>
        /// <param name="listType">: Docs or Fees</param>
        /// <param name="direction">: Up or Down</param>
        /// <returns>No return</returns>
        protected async void MoveSeq(GenSmartflowItem selectobject, string listType, string direction)
        {
            try
            {
                SeqMoving = true; //prevents changes to the form whilst process of changing seq is carried out

                var lstItems = new List<VmUsrOrDefChapterManagement>();
                int incrementBy;

                incrementBy = (direction.ToLower() == "up" ? -1 : 1);

                RowChanged = (int)(selectobject.SeqNo + incrementBy);

                switch (listType)
                {
                    case "Docs":
                        lstItems = LstDocs;
                        break;
                    //case "Fees":
                    //    lstItems = LstFees;
                    //    break;
                    case "Status":
                        lstItems = LstStatus;
                        break;

                    //case "Chapters":
                    //    lstItems = LstChapters
                    //                        .Where(A => A.ChapterObject.CaseTypeGroup == SelectedChapter.CaseTypeGroup)
                    //                        .Where(A => A.ChapterObject.CaseType == SelectedChapter.CaseType)
                    //                        .OrderBy(A => A.ChapterObject.SeqNo)
                    //                        .ToList();
                    //    break;
                }

                var swapItem = lstItems.Where(D => D.ChapterObject.SeqNo == (selectobject.SeqNo + incrementBy)).SingleOrDefault();
                if (!(swapItem is null))
                {
                    selectobject.SeqNo += incrementBy;
                    swapItem.ChapterObject.SeqNo = swapItem.ChapterObject.SeqNo + (incrementBy * -1);

                    //if (listType == "Chapters")
                    //{
                    //    await ChapterManagementService.UpdateMainItem(selectobject).ConfigureAwait(false);
                    //    await ChapterManagementService.UpdateMainItem(swapItem.ChapterObject).ConfigureAwait(false);
                    //}

                    SelectedChapterObject.SmartflowData = JsonConvert.SerializeObject(SelectedChapter);
                    await ChapterManagementService.Update(SelectedChapterObject).ConfigureAwait(false);


                    //keep track of time last updated ready for comparison by other sessions checking for updates
                    AppChapterState.SetLastUpdated(UserSession, SelectedChapter);

                }

                await RefreshChapterItems(listType);
                await InvokeAsync(() =>
                {
                    StateHasChanged();
                });
            }
            catch(Exception e)
            {
                await GenericErrorLog(false, e, "MoveSeq", e.Message);
            }
            finally
            {
                SeqMoving = false;
            }

        }

        protected async Task MoveBlockNo(DataViews selectobject, string listType, string direction) //Dataviews
        {
            try
            {

                SeqMoving = true; //prevents changes to the form whilst process of changing seq is carried out

                int incrementBy;

                incrementBy = (direction.ToLower() == "up" ? -1 : 1);

                RowChanged = (int)(selectobject.BlockNo + incrementBy);

                var swapItem = ListVmDataViews.Where(D => D.DataView.BlockNo == (selectobject.BlockNo + incrementBy)).SingleOrDefault();
                if (!(swapItem is null))
                {
                    selectobject.BlockNo += incrementBy;
                    swapItem.DataView.BlockNo = swapItem.DataView.BlockNo + (incrementBy * -1);

                    SelectedChapterObject.SmartflowData = JsonConvert.SerializeObject(SelectedChapter);
                    await ChapterManagementService.Update(SelectedChapterObject).ConfigureAwait(false);


                    //keep track of time last updated ready for comparison by other sessions checking for updates
                    AppChapterState.SetLastUpdated(UserSession, SelectedChapter);

                }

                await RefreshChapterItems(listType);
                await InvokeAsync(() =>
                {
                    StateHasChanged();
                });

            }
            catch (Exception e)
            {
                await GenericErrorLog(true,e, "MoveBlockNo", $"Moving Dataview: {e.Message}");

            }
            finally
            {
                SeqMoving = false;
            }

        }

        protected async void MoveMessageSeqNo(TickerMessages selectobject, string listType, string direction)
        {
            try
            {

                SeqMoving = true; //prevents changes to the form whilst process of changing seq is carried out

                int incrementBy;

                incrementBy = (direction.ToLower() == "up" ? -1 : 1);

                RowChanged = (int)(selectobject.SeqNo + incrementBy);

                var swapItem = ListVmTickerMessages.Where(D => D.Message.SeqNo == (selectobject.SeqNo + incrementBy)).SingleOrDefault();
                if (!(swapItem is null))
                {
                    selectobject.SeqNo += incrementBy;
                    swapItem.Message.SeqNo = swapItem.Message.SeqNo + (incrementBy * -1);

                    SelectedChapterObject.SmartflowData = JsonConvert.SerializeObject(SelectedChapter);
                    await ChapterManagementService.Update(SelectedChapterObject).ConfigureAwait(false);


                    //keep track of time last updated ready for comparison by other sessions checking for updates
                    AppChapterState.SetLastUpdated(UserSession, SelectedChapter);

                }

                await RefreshChapterItems(listType);
                await InvokeAsync(() =>
                {
                    StateHasChanged();
                });
            }
            catch (Exception e)
            {
                await GenericErrorLog(true,e, "MoveMessageSeqNo", $"Moving message: {e.Message}");

            }
            finally
            {
                SeqMoving = false;
            }
            

        }

        protected async void MoveFeeSeqNo(Fee selectobject, string listType, string direction)
        {
            try
            {
                    
                SeqMoving = true; //prevents changes to the form whilst process of changing seq is carried out

                int incrementBy;

                incrementBy = (direction.ToLower() == "up" ? -1 : 1);

                RowChanged = (int)(selectobject.SeqNo + incrementBy);

                var swapItem = LstFees.Where(D => D.FeeObject.SeqNo == (selectobject.SeqNo + incrementBy)).SingleOrDefault();
                if (!(swapItem is null))
                {
                    selectobject.SeqNo += incrementBy;
                    swapItem.FeeObject.SeqNo = swapItem.FeeObject.SeqNo + (incrementBy * -1);

                    SelectedChapterObject.SmartflowData = JsonConvert.SerializeObject(SelectedChapter);
                    await ChapterManagementService.Update(SelectedChapterObject).ConfigureAwait(false);


                    //keep track of time last updated ready for comparison by other sessions checking for updates
                    AppChapterState.SetLastUpdated(UserSession, SelectedChapter);

                }

                await RefreshChapterItems(listType);
                await InvokeAsync(() =>
                {
                    StateHasChanged();
                });
            }
            catch (Exception e)
            {
                await GenericErrorLog(true,e, "MoveFeeSeqNo", $"Moving fee: {e.Message}");

            }
            finally
            {
                SeqMoving = false;
            }


        }

        private List<int?> GetSeqNumbers(string listType)
        {
            var listItems = new List<int?>();

            try
            {

                switch (listType)
                {
                    case "Docs":
                        listItems = LstDocs.OrderBy(D => D.ChapterObject.SeqNo).Select(D => D.ChapterObject.SeqNo).ToList();
                        break;
                    case "Status":
                        listItems = LstStatus.OrderBy(D => D.ChapterObject.SeqNo).Select(D => D.ChapterObject.SeqNo).ToList();
                        break;
                    case "Chapters":
                        listItems = LstSelectedChapters
                            .OrderBy(D => D.SmartflowObject.SeqNo)
                            .Select(D => D.SmartflowObject.SeqNo).ToList();
                        break;
                }
            }
            catch (Exception)
            {

            }

            return listItems;
        }

        protected async void CondenseSeq(string ListType)
        {
            try
            {
                await RefreshChapterItems(ListType);

                var ListItems = GetRelevantChapterList(ListType);

                int seqNo = 0;

                foreach (VmUsrOrDefChapterManagement item in ListItems.OrderBy(A => A.ChapterObject.SeqNo))
                {
                    seqNo += 1;
                    item.ChapterObject.SeqNo = seqNo;
                }

                SelectedChapterObject.SmartflowData = JsonConvert.SerializeObject(SelectedChapter);
                await ChapterManagementService.Update(SelectedChapterObject).ConfigureAwait(false);

                
                //keep track of time last updated ready for comparison by other sessions checking for updates
                AppChapterState.SetLastUpdated(UserSession, SelectedChapter);

                await RefreshChapterItems(ListType);

            }
            catch (Exception e)
            {
                await GenericErrorLog(false,e, "CondenseSeq", $"{e.Message}");

            }


            await InvokeAsync(() =>
            {
                StateHasChanged();
            });
        }

        protected async void CondenseBlockNo(string ListType)
        {
            try
            {
                await RefreshChapterItems(ListType);

                int seqNo = 0;

                foreach (var item in ListVmDataViews.OrderBy(A => A.DataView.BlockNo))
                {
                    seqNo += 1;
                    item.DataView.BlockNo = seqNo;
                }

                SelectedChapterObject.SmartflowData = JsonConvert.SerializeObject(SelectedChapter);
                await ChapterManagementService.Update(SelectedChapterObject).ConfigureAwait(false);


                //keep track of time last updated ready for comparison by other sessions checking for updates
                AppChapterState.SetLastUpdated(UserSession, SelectedChapter);

                await RefreshChapterItems(ListType);

            }
            catch (Exception e)
            {
                await GenericErrorLog(false,e, "CondenseBlockNo", $"{e.Message}");

            }


            await InvokeAsync(() =>
            {
                StateHasChanged();
            });
        }

        protected async void CondenseMessageSeqNo(string ListType)
        {
            try
            {
                await RefreshChapterItems(ListType);

                int seqNo = 0;

                foreach (var item in ListVmTickerMessages.OrderBy(A => A.Message.SeqNo))
                {
                    seqNo += 1;
                    item.Message.SeqNo = seqNo;
                }

                SelectedChapterObject.SmartflowData = JsonConvert.SerializeObject(SelectedChapter);
                await ChapterManagementService.Update(SelectedChapterObject).ConfigureAwait(false);


                //keep track of time last updated ready for comparison by other sessions checking for updates
                AppChapterState.SetLastUpdated(UserSession, SelectedChapter);

                await RefreshChapterItems(ListType);

            }
            catch (Exception e)
            {
                await GenericErrorLog(false,e, "CondenseMessageSeqNo", $"{e.Message}");

            }

            await InvokeAsync(() =>
            {
                StateHasChanged();
            });
        }

        protected async void CondenseFeeSeq(string ListType)
        {
            try
            {
                await RefreshChapterItems(ListType);

                var ListItems = LstFees;

                int seqNo = 0;

                foreach (VmFee item in ListItems.OrderBy(A => A.FeeObject.SeqNo))
                {
                    seqNo += 1;
                    item.FeeObject.SeqNo = seqNo;
                }

                SelectedChapterObject.SmartflowData = JsonConvert.SerializeObject(SelectedChapter);
                await ChapterManagementService.Update(SelectedChapterObject).ConfigureAwait(false);


                //keep track of time last updated ready for comparison by other sessions checking for updates
                AppChapterState.SetLastUpdated(UserSession, SelectedChapter);

                await RefreshChapterItems(ListType);

            }
            catch (Exception e)
            {
                await GenericErrorLog(false,e, "CondenseFeeSeq", $"{e.Message}");

            }

            await InvokeAsync(() =>
            {
                StateHasChanged();
            });
        }

        protected async void CondenseChapterSeq()
        {
            try
            {
                var ListItems = LstSelectedChapters;

                int seqNo = 0;

                foreach (VmUsrOrsfSmartflows item in ListItems.OrderBy(A => A.SmartflowObject.SeqNo))
                {
                    seqNo += 1;
                    item.SmartflowObject.SeqNo = seqNo;

                    await ChapterManagementService.UpdateMainItem(item.SmartflowObject);
                }

                RefreshChapters();
            }
            catch(Exception e)
            {
                await GenericErrorLog(true,e, "CondenseChapterSeq", $"Correcting Smartflow seq numbers: {e.Message}");
            }


        }

        protected void CondenseFeeSeq()
        {
            CondenseSeq("Fees");
        }

        public async void RefreshViewList()
        {
            try
            {
                ListP4WViews = await PartnerAccessService.GetPartnerViews();
                StateHasChanged();
            }
            catch(Exception e)
            {
                await GenericErrorLog(false,e, "RefreshViewList", e.Message);
            }
        }

        protected async Task PrepareBackUpForDelete(FileDesc selectedFile)
        {
            try
            {
                await SetSmartflowFilePath();

                SelectedFileDescription = selectedFile;

                string itemName = selectedFile.FileName;

                Action SelectedDeleteAction = HandleDeleteBackupFile;
                var parameters = new ModalParameters();
                parameters.Add("ItemName", itemName);
                parameters.Add("ModalHeight", "300px");
                parameters.Add("ModalWidth", "500px");
                parameters.Add("DeleteAction", SelectedDeleteAction);
                parameters.Add("InfoText", $"Are you sure you wish to delete the '{itemName}' backup file?");

                var options = new ModalOptions()
                {
                    Class = "blazored-custom-modal"
                };

                Modal.Show<ModalDelete>("Delete Backup File", parameters, options);
            }
            catch(Exception e)
            {
                await GenericErrorLog(false,e, "PrepareBackUpForDelete", e.Message);
            }
        }


        public void CancelCreateP4WStep()
        {
            SelectedChapter.StepName = "";
            SelectedChapter.SelectedStep = "";
            SelectedChapter.StepName = $"SF {SelectedChapter.Name} Smartflow";
            StateHasChanged();
        }

        protected async void CreateP4WSmartflowStep()
        {
            try
            {
                IList<string> Errors = new List<string>();

                string confirmText = "";
                string confirmHeader = "";

                if (string.IsNullOrEmpty(SelectedChapter.P4WCaseTypeGroup))
                {
                    Errors.Add("Missing Case Type Group");
                }

                if (string.IsNullOrEmpty(SelectedChapter.SelectedView))
                {
                    Errors.Add("Missing View");
                }

                if (string.IsNullOrEmpty(SelectedChapter.StepName))
                {
                    Errors.Add("Missing Step Name");
                }
                else if(DropDownChapterList
                    .Where(D => D.DocumentType == 6)
                    .Where(D => D.Notes != null && D.Notes.Contains("Smartflow:"))
                    .Where(V => V.CaseTypeGroupRef == (string.IsNullOrEmpty(SelectedChapter.P4WCaseTypeGroup)
                                                        ? -2
                                                        : SelectedChapter.P4WCaseTypeGroup == "Global Documents"
                                                        ? 0
                                                        : SelectedChapter.P4WCaseTypeGroup == "Entity Documents"
                                                        ? -1
                                                        : PartnerCaseTypeGroups
                                                            .Where(P => P.Name == SelectedChapter.P4WCaseTypeGroup)
                                                            .Select(P => P.Id)
                                                            .FirstOrDefault()))
                    .OrderBy(D => D.Name)
                    .Select(D => D.Name)
                    .ToList()
                    .Contains(SelectedChapter.StepName))
                {
                    confirmHeader = "Possible Conflict?";
                    confirmText = $"This Step already exists in the case type group: {SelectedChapter.P4WCaseTypeGroup}. Do you still wish to create?";
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
                await GenericErrorLog(true,e, "CreateP4WSmartflowStep", e.Message);
            }

        }
        
        private async void SaveP4WCaseTypeGroup(string caseTypeGroup)
        {
            try
            {
                SelectedChapter.P4WCaseTypeGroup = caseTypeGroup;

                SelectedChapterObject.SmartflowData = JsonConvert.SerializeObject(SelectedChapter);

                await ChapterManagementService.Update(SelectedChapterObject).ConfigureAwait(false);


                //keep track of time last updated ready for comparison by other sessions checking for updates
                AppChapterState.SetLastUpdated(UserSession, SelectedChapter);
            }
            catch(HttpRequestException)
            {
                //do nothing, already dealt with
            }
            catch(Exception e)
            {
                await GenericErrorLog(true,e, "SaveP4WCaseTypeGroup", e.Message);
            }
        }

        private async void SaveSelectedView(string view)
        {
            try
            {
                SelectedChapter.SelectedView = view;

                SelectedChapterObject.SmartflowData = JsonConvert.SerializeObject(SelectedChapter);

                await ChapterManagementService.Update(SelectedChapterObject).ConfigureAwait(false);


                //keep track of time last updated ready for comparison by other sessions checking for updates
                AppChapterState.SetLastUpdated(UserSession, SelectedChapter);
            }
            catch(HttpRequestException)
            {
                //do nothing, already dealt with
            }
            catch(Exception e)
            {
                await GenericErrorLog(true,e, "SaveSelectedView", e.Message);
            }
        }
        
        private async void SaveSelectedStep(string step)
        {
            try
            {
                SelectedChapter.SelectedStep = step;

                SelectedChapterObject.SmartflowData = JsonConvert.SerializeObject(SelectedChapter);

                await ChapterManagementService.Update(SelectedChapterObject).ConfigureAwait(false);


                //keep track of time last updated ready for comparison by other sessions checking for updates
                AppChapterState.SetLastUpdated(UserSession, SelectedChapter);
            }
            catch(HttpRequestException)
            {
                //do nothing, already dealt with
            }
            catch(Exception e)
            {
                await GenericErrorLog(true,e, "SaveSelectedStep", e.Message);
            }
        }

        private void TickerValidation()
        {
            var currentDate = DateTime.Now.Date;

            ValidTicketMessageCount = 1;
            foreach (VmTickerMessages msg in ListVmTickerMessages)
            {

                if (DateTime.ParseExact(msg.Message.ToDate, "yyyyMMdd", CultureInfo.InvariantCulture) < DateTime.ParseExact(msg.Message.FromDate, "yyyyMMdd", CultureInfo.InvariantCulture))
                {
                    msg.MsgValidation = "Invalid";
                    msg.MsgTooltip = "Invalid date range.";
                }
                else if (DateTime.ParseExact(msg.Message.FromDate, "yyyyMMdd", CultureInfo.InvariantCulture) > currentDate)
                {
                    msg.MsgValidation = "Future";
                    msg.MsgTooltip = "Message set for future date.";
                }
                else if (DateTime.ParseExact(msg.Message.ToDate, "yyyyMMdd", CultureInfo.InvariantCulture) < currentDate)
                {
                    msg.MsgValidation = "Expired";
                    msg.MsgTooltip = "Message expired.";
                }
                else if (ValidTicketMessageCount > 3)
                {
                    msg.MsgValidation = "Exceeded";
                    msg.MsgTooltip = "The maximum number of valid ticker messages are three.";
                }
                else
                {
                    ValidTicketMessageCount += 1;
                    msg.MsgValidation = "Valid";
                    msg.MsgTooltip = "Message will be shown on the Smarflow screen.";
                }
            }

        }


        private async void HandleChapterDetailDelete()
        {

            SelectedChapter.Items.Remove(EditObject.ChapterObject);
            SelectedChapterObject.SmartflowData = JsonConvert.SerializeObject(SelectedChapter);
            await ChapterManagementService.Update(SelectedChapterObject);

            //keep track of time last updated ready for comparison by other sessions checking for updates
            AppChapterState.SetLastUpdated(UserSession, SelectedChapter);

            CondenseSeq(NavDisplay);

            StateHasChanged();
        }

        private async void HandleChapterFeeDelete()
        {
            SelectedChapter.Fees.Remove(EditFeeObject.FeeObject);
            SelectedChapterObject.SmartflowData = JsonConvert.SerializeObject(SelectedChapter);
            await ChapterManagementService.Update(SelectedChapterObject);

            //keep track of time last updated ready for comparison by other sessions checking for updates
            AppChapterState.SetLastUpdated(UserSession, SelectedChapter);

            await RefreshChapterItems(NavDisplay);
            StateHasChanged();
        }

        private async void HandleDataViewDelete()
        {
            SelectedChapter.DataViews.Remove(EditDataViewObject.DataView);
            SelectedChapterObject.SmartflowData = JsonConvert.SerializeObject(SelectedChapter);
            await ChapterManagementService.Update(SelectedChapterObject);

            //keep track of time last updated ready for comparison by other sessions checking for updates
            AppChapterState.SetLastUpdated(UserSession, SelectedChapter);

            await RefreshChapterItems(NavDisplay);
            StateHasChanged();
        }

        private async void HandleTickerMessageDelete()
        {
            SelectedChapter.TickerMessages.Remove(EditTickerMessageObject.Message);
            SelectedChapterObject.SmartflowData = JsonConvert.SerializeObject(SelectedChapter);
            await ChapterManagementService.Update(SelectedChapterObject);

            //keep track of time last updated ready for comparison by other sessions checking for updates
            AppChapterState.SetLastUpdated(UserSession, SelectedChapter);

            await RefreshChapterItems(NavDisplay);
            StateHasChanged();
        }

#endregion

#region Images

        private async void HandleBgImageFileSelection(IFileListEntry[] entryFiles)
        {
            try
            {
                FileHelper.CustomPath = $"wwwroot/images/Companies/{UserSession.Company.CompanyName}/BackgroundImages";
                ListFilesForBgImages = FileHelper.GetFileList();

                var files = new List<IFileListEntry>();
                IList<string> fileErrorDescs = new List<string>();

                foreach (var file in entryFiles)
                {
                    if (file != null)
                    {
                        if (!(file.Name.Contains(".jpg") || file.Name.Contains(".png")))
                        {
                            fileErrorDescs.Add($"The file: {file.Name} is not a valid image. Image files must be either .jpg or .png");
                        }
                        else
                        {
                            if (ListFilesForBgImages.Where(F => F.FileName == file.Name).FirstOrDefault() is null)
                            {
                                await ChapterFileUpload.UploadChapterFiles(file);
                                files.Add(file);
                            }
                            else
                            {
                                fileErrorDescs.Add($"The file: {file.Name} already exists on the system");
                            }


                        }

                    }

                }
                if (files != null && files.Count > 0)
                {
                    StateHasChanged();
                }

                if (fileErrorDescs.Count > 0)
                {
                    ShowErrorModal("File Upload", "The following errors occured during the upload:", fileErrorDescs);
                }

                ListFilesForBgImages = FileHelper.GetFileList();

                StateHasChanged();
            }
            catch(Exception e)
            {
                await GenericErrorLog(false,e, "HandleBgImageFileSelection", e.Message);
            }
            
        }

        protected async Task PrepareBgImageForDelete(FileDesc selectedFile)
        {
            try
            {
                FileHelper.CustomPath = $"wwwroot/images/Companies/{UserSession.Company.CompanyName}/BackgroundImages";

                SelectedFileDescription = selectedFile;

                string itemName = selectedFile.FileName;

                Action SelectedDeleteAction = HandleDeleteBgImageFile;
                var parameters = new ModalParameters();
                parameters.Add("ItemName", itemName);
                parameters.Add("ModalHeight", "300px");
                parameters.Add("ModalWidth", "500px");
                parameters.Add("DeleteAction", SelectedDeleteAction);
                parameters.Add("InfoText", $"Are you sure you wish to delete the '{itemName}' ?");

                var options = new ModalOptions()
                {
                    Class = "blazored-custom-modal"
                };

                Modal.Show<ModalDelete>("Delete Backup File", parameters, options);
            }
            catch(Exception e)
            {
                await GenericErrorLog(false,e, "PrepareBgImageForDelete", e.Message);
            }
        }

        private void HandleDeleteBgImageFile()
        {
            DeleteBgImageFile(SelectedFileDescription);
        }

        private async void SelectBgImage(FileDesc fileDesc)
        {
            try
            {
                SelectedChapter.BackgroundImage = fileDesc.FileURL.Replace("/wwwroot", "");
                SelectedChapter.BackgroundImageName = fileDesc.FileName;

                SelectedChapterObject.SmartflowData = JsonConvert.SerializeObject(SelectedChapter);

                await ChapterManagementService.Update(SelectedChapterObject).ConfigureAwait(false);


                //keep track of time last updated ready for comparison by other sessions checking for updates
                AppChapterState.SetLastUpdated(UserSession, SelectedChapter);

                UserSession.SetTempBackground(SelectedChapter.BackgroundImage.Replace("/wwwroot", ""), NavigationManager.Uri);
                UserSession.RefreshHome?.Invoke();

                await InvokeAsync(() =>
                {
                    StateHasChanged();
                });
            }
            catch(HttpRequestException)
            {
                //do nothing, already dealt with
            }
            catch(Exception e)
            {
                await GenericErrorLog(false,e, "SelectBgImage", e.Message);
            }
        }

        private async Task DeleteBgImageFile(FileDesc file)
        {
            try
            {
                FileHelper.CustomPath = $"wwwroot/images/Companies/{UserSession.Company.CompanyName}/BackgroundImages";

                ChapterFileUpload.DeleteFile(file.FilePath);

                ListFilesForBgImages = FileHelper.GetFileList();
                StateHasChanged();
            }
            catch(Exception e)
            {
                await GenericErrorLog(false,e, "DeleteBgImageFile", e.Message);
            }
        }

#endregion

#region Modals

        private void PrepareForEdit(VmUsrOrDefChapterManagement item, string header)
        {
            SelectedList = header;
            EditObject = item;

            ShowChapterDetailModal("Edit");
        }

        private void PrepareDataViewForEdit(VmDataViews item, string header)
        {
            SelectedList = header;
            EditDataViewObject = item;

            ShowDataViewDetailModal("Edit");
        }

        private void PrepareTickerMessageForEdit(VmTickerMessages item, string header)
        {
            SelectedList = header;
            EditTickerMessageObject = item;

            ShowTickerMessageDetailModal("Edit");
        }

        private async void PrepareForInsert(string header, string type)
        {
            try
            {
                SelectedList = type;

                EditObject = new VmUsrOrDefChapterManagement { ChapterObject = new GenSmartflowItem() };
                EditObject.ChapterObject.Type = (type == "Steps and Documents") ? "Doc" : type;
                EditObject.ChapterObject.Action = "INSERT";
                
                if (type == "Steps and Documents")
                {
                    EditObject.ChapterObject.SeqNo = LstDocs
                                                        .OrderByDescending(A => A.ChapterObject.SeqNo)
                                                        .Select(A => A.ChapterObject.SeqNo)
                                                        .FirstOrDefault() + 1;
                }
                else if (type == "Status")
                {
                    EditObject.ChapterObject.SeqNo = LstStatus
                                                        .OrderByDescending(A => A.ChapterObject.SeqNo)
                                                        .Select(A => A.ChapterObject.SeqNo)
                                                        .FirstOrDefault() + 1;

                }
                else
                {
                    EditFeeObject.FeeObject.SeqNo = LstFees
                                        .OrderByDescending(A => A.FeeObject.SeqNo)
                                        .Select(A => A.FeeObject.SeqNo)
                                        .FirstOrDefault() + 1;
                }

                EditObject.ChapterObject.SeqNo = EditObject.ChapterObject.SeqNo is null
                                                            ? 1
                                                            : EditObject.ChapterObject.SeqNo;

                EditFeeObject.FeeObject.SeqNo = EditFeeObject.FeeObject.SeqNo is null
                                                            ? 1
                                                            : EditFeeObject.FeeObject.SeqNo;

                ShowChapterDetailModal("Insert");
            }
            catch(Exception e)
            {
                await GenericErrorLog(false, e, "PrepareForInsert", e.Message);
            }
            
        }

        private async Task PrepareDataViewForInsert(string header)
        {
            try
            {
                SelectedList = header;
                EditDataViewObject = new VmDataViews { DataView = new DataViews() } ;

                if(ListVmDataViews.Count > 0)
                {

                    EditDataViewObject.DataView.BlockNo = ListVmDataViews
                                                        .OrderByDescending(D => D.DataView.BlockNo)
                                                        .Select(D => D.DataView.BlockNo)
                                                        .FirstOrDefault() + 1;
                }
                else
                {
                    EditDataViewObject.DataView.BlockNo = 1;
                }


                ShowDataViewDetailModal("Insert");
            }
            catch(Exception e)
            {
                await GenericErrorLog(false, e, "PrepareDataViewForInsert", e.Message);
            }
        }

        //TODO: Change display to reflect the top 3 items and from-to dates
        private async Task PrepareTickerMessageForInsert(string header)
        {
            try{

                
                SelectedList = header;
                EditTickerMessageObject = new VmTickerMessages { Message = new TickerMessages() };

                EditTickerMessageObject.Message.FromDate = DateTime.Now.ToString("yyyyMMdd");
                EditTickerMessageObject.Message.ToDate = DateTime.Now.ToString("yyyyMMdd");

                if (ListVmTickerMessages.Count > 0)
                {

                    EditTickerMessageObject.Message.SeqNo = ListVmTickerMessages
                                                        .OrderByDescending(D => D.Message.SeqNo)
                                                        .Select(D => D.Message.SeqNo)
                                                        .FirstOrDefault() + 1;
                }
                else
                {
                    EditTickerMessageObject.Message.SeqNo = 1;
                }


                ShowTickerMessageDetailModal("Insert");
            }
            catch(Exception e)
            {
                await GenericErrorLog(false, e, "PrepareTickerMessageForInsert", e.Message);
            }
        }


        private void PrepareAttachmentForAdd(VmUsrOrDefChapterManagement item)
        {
            SelectedList = "New Attachement";
            EditObject = item;
            AttachObject = null;

            ShowChapterAttachmentModal();
        }

        private void PrepareAttachmentForEdit(VmUsrOrDefChapterManagement item, LinkedItems LinkedItems)
        {
            SelectedList = "Edit Attachement";
            EditObject = item;
            AttachObject = LinkedItems;

            ShowChapterAttachmentModal();
        }

        private void PrepareAttachmentForView(VmUsrOrDefChapterManagement item, LinkedItems LinkedItems)
        {
            SelectedList = "Edit Attachement";
            EditObject = item;
            AttachObject = LinkedItems;

            ShowChapterAttachmentViewModal();
        }

        protected void PrepareFeeForInsert (string option)
        {
            Fee taskObject = new Fee();
            if (!(LstFees is null ) && LstFees.Count() > 0)
            {
                taskObject.SeqNo = LstFees.Select(F => F.FeeObject.SeqNo).OrderByDescending(F => F).FirstOrDefault() + 1;
            }
            else
            {
                taskObject.SeqNo = 1;
            }
            ShowChapterFeesModal(option, taskObject);
        }

        protected void PrepareChapterDelete(VmUsrOrsfSmartflows SelectedChapterItem)
        {
            EditChapter = SelectedChapterItem.SmartflowObject;

            string itemName = SelectedChapterItem.SmartflowObject.SmartflowName;

            Action SelectedDeleteAction = HandleChapterDelete;
            var parameters = new ModalParameters();
            parameters.Add("ItemName", itemName);
            parameters.Add("ModalHeight", "300px");
            parameters.Add("ModalWidth", "500px");
            parameters.Add("DeleteAction", SelectedDeleteAction);
            parameters.Add("InfoText", $"Are you sure you wish to delete the '{itemName}' smartflow?");


            var options = new ModalOptions()
            {
                Class = "blazored-custom-modal"
            };

            Modal.Show<ModalDelete>("Delete Smartflow", parameters, options);
        }

        protected void PrepareChapterDetailDelete(VmUsrOrDefChapterManagement SelectedChapterItem)
        {
            EditObject = SelectedChapterItem;

            string itemName = (string.IsNullOrEmpty(SelectedChapterItem.ChapterObject.AltDisplayName) ? SelectedChapterItem.ChapterObject.Name : SelectedChapterItem.ChapterObject.AltDisplayName);
            string itemType = SelectedChapterItem.ChapterObject.Type;

            Action SelectedDeleteAction = HandleChapterDetailDelete;
            var parameters = new ModalParameters();
            parameters.Add("ItemName", itemName);
            parameters.Add("ModalHeight", "300px");
            parameters.Add("ModalWidth", "500px");
            parameters.Add("DeleteAction", SelectedDeleteAction);
            parameters.Add("InfoText", $"Are you sure you wish to delete the '{itemName}' {itemType.ToLower()}? ");

            var options = new ModalOptions()
            {
                Class = "blazored-custom-modal"
            };

            Modal.Show<ModalDelete>($"Delete {itemType}", parameters, options);
        }

        protected void PrepareChapterFeeDelete(VmFee SelectedChapterItem)
        {
            EditFeeObject = SelectedChapterItem;

            string itemName = SelectedChapterItem.FeeObject.FeeName;

            Action SelectedDeleteAction = HandleChapterFeeDelete;
            var parameters = new ModalParameters();
            parameters.Add("ItemName", itemName);
            parameters.Add("ModalHeight", "300px");
            parameters.Add("ModalWidth", "500px");
            parameters.Add("DeleteAction", SelectedDeleteAction);
            parameters.Add("InfoText", $"Are you sure you wish to delete the '{itemName}' fee?");

            var options = new ModalOptions()
            {
                Class = "blazored-custom-modal"
            };

            Modal.Show<ModalDelete>("Delete Fee", parameters, options);
        }

        protected void PrepareDataViewDelete(VmDataViews selectedDataView)
        {
            EditDataViewObject = selectedDataView;

            string itemName = (string.IsNullOrEmpty(selectedDataView.DataView.DisplayName) ? selectedDataView.DataView.ViewName : selectedDataView.DataView.DisplayName);

            Action SelectedDeleteAction = HandleDataViewDelete;
            var parameters = new ModalParameters();
            parameters.Add("ItemName", itemName);
            parameters.Add("ModalHeight", "300px");
            parameters.Add("ModalWidth", "500px");
            parameters.Add("DeleteAction", SelectedDeleteAction);
            parameters.Add("InfoText", $"Are you sure you wish to delete the '{itemName}' data view?");

            var options = new ModalOptions()
            {
                Class = "blazored-custom-modal"
            };

            Modal.Show<ModalDelete>($"Delete Data View", parameters, options);
        }

        protected void PrepareTickerMessageDelete(VmTickerMessages selectedTickerMessage)
        {
            EditTickerMessageObject = selectedTickerMessage;
            
            string itemName = selectedTickerMessage.Message.Message;

            Action SelectedDeleteAction = HandleTickerMessageDelete;
            var parameters = new ModalParameters();
            parameters.Add("ItemName", itemName);
            parameters.Add("ModalHeight", "300px");
            parameters.Add("ModalWidth", "500px");
            parameters.Add("DeleteAction", SelectedDeleteAction);
            parameters.Add("InfoText", $"Are you sure you wish to delete the message?");

            var options = new ModalOptions()
            {
                Class = "blazored-custom-modal"
            };

            Modal.Show<ModalDelete>($"Delete Message", parameters, options);
        }

        protected async Task ShowDataViewDetailModal(string option)
        {
            try
            {

                Action action = RefreshSelectedList;
                Action refreshViewList = RefreshViewList; 

                var copyObject = new DataViews
                {
                    BlockNo = EditDataViewObject.DataView.BlockNo,
                    DisplayName = EditDataViewObject.DataView.DisplayName,
                    ViewName = EditDataViewObject.DataView.ViewName
                };

                var parameters = new ModalParameters();
                parameters.Add("TaskObject", EditDataViewObject.DataView);
                parameters.Add("ListPartnerViews", ListP4WViews);
                parameters.Add("CopyObject", copyObject);
                parameters.Add("DataChanged", action);
                parameters.Add("SelectedChapter", SelectedChapter);
                parameters.Add("SelectedChapterObject", SelectedChapterObject);
                parameters.Add("Option", option);
                parameters.Add("PartnerAccessService", PartnerAccessService);
                parameters.Add("RefreshViewList", refreshViewList);

                var options = new ModalOptions()
                {
                    Class = "blazored-custom-modal modal-chapter-data"
                };

                Modal.Show<DataViewDetail>("Data View", parameters, options);
            }
            catch(Exception e)
            {
                await GenericErrorLog(false,e, "ShowDataViewDetailModal", e.Message);
            }
        }

        protected async Task ShowTickerMessageDetailModal(string option)
        {
            try
            {
                Action action = RefreshSelectedList;

                var copyObject = new TickerMessages
                {
                    SeqNo = EditTickerMessageObject.Message.SeqNo,
                    Message = EditTickerMessageObject.Message.Message,
                    FromDate = EditTickerMessageObject.Message.FromDate,
                    ToDate = EditTickerMessageObject.Message.ToDate
                };

                var parameters = new ModalParameters();
                parameters.Add("TaskObject", EditTickerMessageObject.Message);
                parameters.Add("CopyObject", copyObject);
                parameters.Add("DataChanged", action);
                parameters.Add("SelectedChapter", SelectedChapter);
                parameters.Add("SelectedChapterObject", SelectedChapterObject);
                parameters.Add("Option", option);

                var options = new ModalOptions()
                {
                    Class = "blazored-custom-modal modal-chapter-item"
                };

                Modal.Show<TickerMessageDetail>("Ticker Messages", parameters, options);
            }
            catch(Exception e)
            {
                await GenericErrorLog(false,e, "ShowTickerMessageDetailModal", e.Message);
            }
        }

        protected void ShowChapterAttachmentModal()
        {
            Action action = RefreshSelectedList;
            Action RefreshDocList = this.RefreshDocList;

            var copyObject = new GenSmartflowItem
            {
                Type = EditObject.ChapterObject.Type,
                Name = EditObject.ChapterObject.Name,
                EntityType = EditObject.ChapterObject.EntityType,
                SeqNo = EditObject.ChapterObject.SeqNo,
                SuppressStep = EditObject.ChapterObject.SuppressStep,
                CompleteName = EditObject.ChapterObject.CompleteName,
                AsName = EditObject.ChapterObject.AsName,
                RescheduleDays = EditObject.ChapterObject.RescheduleDays,
                AltDisplayName = EditObject.ChapterObject.AltDisplayName,
                UserMessage = EditObject.ChapterObject.UserMessage,
                PopupAlert = EditObject.ChapterObject.PopupAlert,
                NextStatus = EditObject.ChapterObject.NextStatus,
                LinkedItems = EditObject.ChapterObject.LinkedItems is null ? new List<LinkedItems>() : EditObject.ChapterObject.LinkedItems
            };

            
            var attachment = AttachObject is null 
                ? new LinkedItems 
                {
                    Action = "INSERT",
                    ChaserDesc = "",
                    DocAsName = "",
                    DocName = "",
                    ScheduleDataItem = "",
                    TrackingMethod = "",
                    CustomItem = "N"
                } 
                : copyObject.LinkedItems.Where(F => F.DocName == AttachObject.DocName).FirstOrDefault(); 


            var parameters = new ModalParameters();
            parameters.Add("TaskObject", EditObject.ChapterObject);
            parameters.Add("CopyObject", copyObject);
            parameters.Add("DataChanged", action);
            parameters.Add("SelectedList", SelectedList);
            parameters.Add("DropDownChapterList", DropDownChapterList);
            parameters.Add("CaseTypeGroups", PartnerCaseTypeGroups);
            parameters.Add("ListOfStatus", LstStatus);
            parameters.Add("ListOfAgenda", LstAgendas);
            parameters.Add("SelectedChapter", SelectedChapter);
            parameters.Add("SelectedChapterObject", SelectedChapterObject);
            parameters.Add("CompanyDbAccess", CompanyDbAccess);
            parameters.Add("UserSession", UserSession);
            parameters.Add("RefreshDocList", RefreshDocList);
            parameters.Add("Attachment", attachment);
            parameters.Add("TableDates", TableDates);

            var options = new ModalOptions()
            {
                Class = "blazored-custom-modal modal-chapter-doc"
            };

            Modal.Show<ChapterAttachments>("Linked Item", parameters, options);
        }

        protected void ShowChapterAttachmentViewModal()
        {
            var attachment = AttachObject is null ? new LinkedItems { Action = "INSERT" } : AttachObject;

            var parameters = new ModalParameters();
            parameters.Add("Attachment", attachment);
            parameters.Add("SelectedChapter", SelectedChapter);

            var options = new ModalOptions()
            {
                Class = "blazored-custom-modal modal-chapter-comparison"
            };

            Modal.Show<ChapterAttachmentsView>("Linked Item", parameters, options);
        }

        protected void ShowChapterFeesModal(string option, Fee taskObject)
        {
            Action dataChanged = CondenseFeeSeq;
            Fee copyObject = new Fee { 
                FeeName = taskObject.FeeName 
                , FeeCategory = taskObject.FeeCategory
                , SeqNo = taskObject.SeqNo
                , Amount = taskObject.Amount
                , VATable = taskObject.VATable
                , PostingType = taskObject.PostingType
            };
            var parameters = new ModalParameters();
            parameters.Add("Option", option);
            parameters.Add("TaskObject", taskObject);
            parameters.Add("CopyObject", copyObject);
            parameters.Add("SelectedChapter", SelectedChapter);
            parameters.Add("SelectedChapterObject", SelectedChapterObject);
            parameters.Add("DataChanged", dataChanged);

            var options = new ModalOptions()
            {
                Class = "blazored-custom-modal modal-chapter-fees"
            };


            Modal.Show<ChapterFees>("Fees", parameters, options);
        }

        protected void ShowChapterFeeViewModal(VmFee selectedObject)
        {
            var parameters = new ModalParameters();
            parameters.Add("Object", selectedObject);

            var options = new ModalOptions()
            {
                Class = "blazored-custom-modal modal-chapter-comparison"
            };

            Modal.Show<ChapterFeeView>("Fee", parameters, options);
        }

        protected void ShowDataViewDisplayModal(VmDataViews selectedObject)
        {
            var parameters = new ModalParameters();
            parameters.Add("Object", selectedObject);

            var options = new ModalOptions()
            {
                Class = "blazored-custom-modal modal-chapter-comparison"
            };

            Modal.Show<DataViewDisplay>("Data View", parameters, options);
        }

        protected void ShowTickerMessageDisplayModal(VmTickerMessages selectedObject)
        {
            var parameters = new ModalParameters();
            parameters.Add("Object", selectedObject);

            var options = new ModalOptions()
            {
                Class = "blazored-custom-modal modal-chapter-comparison"
            };

            Modal.Show<TickerMessageDisplay>("Ticker Message", parameters, options);
        }


        protected void ShowChapterCopyModel()
        {
            Action Action = RefreshChapters;

            var parameters = new ModalParameters();
            parameters.Add("TaskObject", EditChapterObject);
            parameters.Add("AllChapters", LstSelectedChapters);
            parameters.Add("currentChapter", SelectedChapter);
            parameters.Add("DataChanged", Action);
            parameters.Add("UserSession", UserSession);
            parameters.Add("CompanyDbAccess", CompanyDbAccess);

            var options = new ModalOptions()
            {
                Class = "blazored-custom-modal modal-chapter-chapter"
            };
            string title = $"Copy {SelectedChapter.Name} to...";
            Modal.Show<ChapterCopy>(title, parameters, options);
        }

        protected void ShowChapterAddOrEditModel()
        {
            Action Action = RefreshChapters;

            var parameters = new ModalParameters();
            parameters.Add("TaskObject", EditChapterObject);
            parameters.Add("DataChanged", Action);
            parameters.Add("AllObjects", LstChapters);
            parameters.Add("UserSession", UserSession);
            parameters.Add("CompanyDbAccess", CompanyDbAccess);

            var options = new ModalOptions()
            {
                Class = "blazored-custom-modal modal-chapter-chapter"
            };

            Modal.Show<ChapterAddOrEdit>("Smartflow", parameters, options);
        }

        protected async Task ShowCaseTypeEditModal()
        {
            try
            {
                Action Action = RefreshChapters;

                var parameters = new ModalParameters();
                parameters.Add("TaskObject", (IsCaseTypeOrGroup == "Chapter") ? EditChapter.SmartflowData : EditCaseType);
                parameters.Add("originalName", (IsCaseTypeOrGroup == "Chapter") ? EditChapter.SmartflowData : EditCaseType);
                if (IsCaseTypeOrGroup == "Chapter")
                {
                    parameters.Add("Chapter", EditChapter);
                }
                parameters.Add("DataChanged", Action);
                parameters.Add("IsCaseTypeOrGroup", IsCaseTypeOrGroup);
                parameters.Add("caseTypeGroupName", SelectedChapter.CaseTypeGroup);
                parameters.Add("ListChapters", LstChapters);
                parameters.Add("CompanyDbAccess", CompanyDbAccess);
                parameters.Add("UserSession", UserSession);

                var options = new ModalOptions()
                {
                    Class = "blazored-custom-modal modal-chapter-casetype"
                };

                Modal.Show<ChapterCaseTypeEdit>("Smartflow", parameters, options);
            }
            catch(Exception e)
            {
                await GenericErrorLog(false,e, "ShowCaseTypeEditModal", e.Message);
            }
        }

        protected async Task ShowChapterDetailViewModal(VmUsrOrDefChapterManagement selectedObject, string type)
        {
            try
            {
                SelectedList = type;

                var parameters = new ModalParameters();
                parameters.Add("Object", selectedObject);
                parameters.Add("SelectedList", SelectedList);

                var options = new ModalOptions()
                {
                    Class = "blazored-custom-modal modal-chapter-comparison"
                };

                Modal.Show<ChapterDetailView>(SelectedList, parameters, options);
            }
            catch(Exception e)
            {
                await GenericErrorLog(false,e, "ShowChapterDetailViewModal", e.Message);
            }
        }

        protected async Task ShowChapterDetailModal(string option)
        {
            try
            {

                Action action = RefreshSelectedList;
                Action RefreshDocList = this.RefreshDocList;

                var copyObject = new GenSmartflowItem
                {
                    Type = EditObject.ChapterObject.Type,
                    Name = EditObject.ChapterObject.Name,
                    EntityType = EditObject.ChapterObject.EntityType,
                    SeqNo = EditObject.ChapterObject.SeqNo,
                    SuppressStep = EditObject.ChapterObject.SuppressStep,
                    CompleteName = EditObject.ChapterObject.CompleteName,
                    AsName = EditObject.ChapterObject.AsName,
                    RescheduleDays = EditObject.ChapterObject.RescheduleDays,
                    AltDisplayName = EditObject.ChapterObject.AltDisplayName,
                    UserMessage = EditObject.ChapterObject.UserMessage,
                    PopupAlert = EditObject.ChapterObject.PopupAlert,
                    NextStatus = EditObject.ChapterObject.NextStatus,
                    Action = EditObject.ChapterObject.Action,
                    TrackingMethod = EditObject.ChapterObject.TrackingMethod,
                    ChaserDesc = EditObject.ChapterObject.ChaserDesc,
                    RescheduleDataItem = EditObject.ChapterObject.RescheduleDataItem,
                    MilestoneStatus = EditObject.ChapterObject.MilestoneStatus,
                    OptionalDocument = EditObject.ChapterObject.OptionalDocument,
                    Agenda = EditObject.ChapterObject.Agenda,
                    CustomItem = EditObject.ChapterObject.CustomItem
            };

                var parameters = new ModalParameters();
                parameters.Add("TaskObject", EditObject.ChapterObject);
                parameters.Add("RefreshDocList", RefreshDocList);
                parameters.Add("CopyObject", copyObject);
                parameters.Add("DataChanged", action);
                parameters.Add("SelectedList", SelectedList);
                parameters.Add("DropDownChapterList", DropDownChapterList);
                parameters.Add("TableDates", TableDates);
                parameters.Add("CaseTypeGroups", PartnerCaseTypeGroups);
                parameters.Add("ListOfStatus", LstStatus);
                parameters.Add("ListOfAgenda", LstAgendas);
                parameters.Add("SelectedChapter", SelectedChapter);
                parameters.Add("SelectedChapterObject", SelectedChapterObject);
                parameters.Add("Option", option);

                string className = "modal-chapter-item";

                if (SelectedList == "Steps and Documents")
                {
                    className = "modal-chapter-doc";
                }
                var options = new ModalOptions()
                {
                    Class = "blazored-custom-modal " + className
                };

                Modal.Show<ChapterDetail>(SelectedList, parameters, options);
            }
            catch(Exception e)
            {
                await GenericErrorLog(false,e, "ShowChapterDetailModal", e.Message);
            }

        }

        protected void ShowChapterImportModal()
        {

            SetSmartflowFilePath();

            while (!(ListFilesForBackups.Where(F => F.FilePath.Contains(".xlsx")).FirstOrDefault() is null))
            {
                ChapterFileUpload.DeleteFile(ListFilesForBackups.Where(F => F.FilePath.Contains(".xlsx")).FirstOrDefault().FilePath);
                GetSeletedChapterFileList();
            }


            Action WriteBackUp = WriteChapterJSONToFile;

            Action SelectedAction = RefreshJson;
            var parameters = new ModalParameters();
            parameters.Add("TaskObject", SelectedChapterObject);
            parameters.Add("ListFileDescriptions", ListFilesForBackups);
            parameters.Add("DataChanged", SelectedAction);
            parameters.Add("WriteBackUp", WriteBackUp);
            parameters.Add("OriginalDataViews", ListVmDataViews);

            var options = new ModalOptions()
            {
                Class = "blazored-custom-modal modal-chapter-import"
            };

            Modal.Show<ChapterImport>("Excel Import", parameters, options);
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

#endregion

#region Chapter Comparrisons

        private async void ToggleComparison()
        {
           
            try
            {
                CompareSystems = !CompareSystems;

                if (CompareSystems)
                {
                    await CompareSelectedChapterToAltSystem();
                }
            }
            catch (Exception e)
            {
                await GenericErrorLog(false,e, "ToggleComparison", $"Comparing Smartflows: {e.Message}");
            }
            
        }

        private async Task<bool> RefreshAltSystemChaptersList()
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

                var LstAllChapterItemsChapters = await CompanyDbAccess.GetAllSmartflowRecords(UserSession);

                if(!(LstAllChapterItemsChapters is null))
                {
                    LstAltSystemChapters = LstAllChapterItemsChapters.Select(A => new VmUsrOrsfSmartflows { SmartflowObject = Mapper.Map(A, new UsrOrsfSmartflows()) }).ToList();
                }
                


                await UserSession.ResetSelectedSystem();

                return true;
            }
            catch (Exception e)
            {
                await GenericErrorLog(false,e, "RefreshAltSystemChaptersList", $"{e.Message}");

                return false;
            }
        }

        private async Task<bool> CompareSelectedChapterToAltSystem()
        {

            try
            {
                var test = await RefreshChapterItems(NavDisplay);

                await RefreshAltSystemChaptersList();

                AltChapterObject = LstAltSystemChapters
                                        .Where(A => A.SmartflowObject.SmartflowName == SelectedChapterObject.SmartflowName)
                                        .Where(A => A.SmartflowObject.CaseType == SelectedChapterObject.CaseType)
                                        .Where(A => A.SmartflowObject.CaseTypeGroup == SelectedChapterObject.CaseTypeGroup)
                                        .Select(A => A.SmartflowObject)
                                        .FirstOrDefault();

                CreateNewSmartflow = false;

                if (!(AltChapterObject is null))
                {
                    altChapter = JsonConvert.DeserializeObject<VmChapter>(AltChapterObject.SmartflowData);

                    var cItems = altChapter.Items;


                    LstAltSystemChapterItems = cItems.Select(T => new VmUsrOrDefChapterManagement { ChapterObject = T, Compared = false }).ToList();

                    //Compare header items
                    CompareChapterToAltSytem();

                    foreach (var item in LstAgendas)
                    {
                        CompareChapterItemsToAltSytem(item);
                    }

                    foreach (var item in LstStatus)
                    {
                        CompareChapterItemsToAltSytem(item);
                    }

                    foreach (var item in LstDocs)
                    {
                        CompareChapterItemsToAltSytem(item);
                    }

                    var fItems = altChapter.Fees is null ? new List<Fee>() : altChapter.Fees;

                    LstAltSystemFeeItems = fItems.Select(T => new VmFee { FeeObject = T, Compared = false }).ToList();

                    foreach (var item in LstFees)
                    {
                        CompareFeeItemsToAltSytem(item);
                    }

                    var dItems = altChapter.DataViews is null ? new List<DataViews>() : altChapter.DataViews;

                    LstAltSystemDataViews = dItems.Select(T => new VmDataViews { DataView = T, Compared = false }).ToList();

                    foreach (var item in ListVmDataViews)
                    {
                        CompareDataViewsToAltSytem(item);
                    }

                    var tItems = altChapter.TickerMessages is null ? new List<TickerMessages>() : altChapter.TickerMessages;

                    LstAltSystemTickerMessages = tItems.Select(T => new VmTickerMessages { Message = T, Compared = false }).ToList();

                    foreach (var item in ListVmTickerMessages)
                    {
                        CompareTickerMessagesToAltSytem(item);
                    }

                }
                else
                {
                    CreateNewSmartflow = true;

                    ListVmTickerMessages = ListVmTickerMessages.Select(T => { T.ComparisonIcon = "times"; T.ComparisonResult = "No match"; return T; }).ToList();
                    ListVmDataViews = ListVmDataViews.Select(T => { T.ComparisonIcon = "times"; T.ComparisonResult = "No match"; return T; }).ToList();
                    LstFees = LstFees.Select(T => { T.ComparisonIcon = "times"; T.ComparisonResult = "No match"; return T; }).ToList();
                    LstDocs = LstDocs.Select(T => { T.ComparisonIcon = "times"; T.ComparisonResult = "No match"; return T; }).ToList();
                    LstStatus = LstStatus.Select(T => { T.ComparisonIcon = "times"; T.ComparisonResult = "No match"; return T; }).ToList();
                    LstAgendas = LstAgendas.Select(T => { T.ComparisonIcon = "times"; T.ComparisonResult = "No match"; return T; }).ToList();
                }
            }
            catch(Exception e)
            {
                await GenericErrorLog(true,e, "CompareSelectedChapterToAltSystem", $"Comparing systems: {e.Message}");
                
                return false;
            }

            await InvokeAsync(() =>
            {
                StateHasChanged();
            });


            return true;
        }

        private async void CompareAllChapters()
        {
            try
            {
                LstSelectedChapters.Select(C => { C.ComparisonIcon = null; C.ComparisonResult = null; return C; }).ToList();

                //var test = new string(SelectedChapterObject.SmartflowData);

                LstAltSystemChapters = new List<VmUsrOrsfSmartflows>();
                AltChapterObject = new UsrOrsfSmartflows();
                await RefreshCompararisonAllChapters();
            }
            catch (Exception e)
            {
                await GenericErrorLog(false,e, "CompareAllChapters", $"{e.Message}");
            }

            await InvokeAsync(() =>
            {
                StateHasChanged();
            });

        }

        private async Task<bool> RefreshCompararisonAllChapters()
        {
            try
            {
                if (CompareSystems)
                {
                    await RefreshAltSystemChaptersList();

                    /*
                    * for every chapter get list of chapter items from both current system and alt system
                    * if any result returns false
                    * 
                    * 
                    */
                    if(!(LstAltSystemChapters is null) && LstAltSystemChapters.Count > 0)
                    {
                        foreach (var chapter in LstSelectedChapters)
                        {
                            var chapterItems = JsonConvert.DeserializeObject<VmChapter>(chapter.SmartflowObject.SmartflowData);

                            AltChapterObject = LstAltSystemChapters
                                                .Where(A => A.SmartflowObject.SmartflowName == chapter.SmartflowObject.SmartflowName)
                                                .Where(A => A.SmartflowObject.CaseType == chapter.SmartflowObject.CaseType)
                                                .Where(A => A.SmartflowObject.CaseTypeGroup == chapter.SmartflowObject.CaseTypeGroup)
                                                .Select(C => C.SmartflowObject)
                                                .FirstOrDefault(); //get the first just in case there are 2 Smartflows with same name

                            if (AltChapterObject is null)
                            {
                                chapter.ComparisonResult = "No match";
                                chapter.ComparisonIcon = "times";
                            }
                            else
                            {
                                altChapter = JsonConvert.DeserializeObject<VmChapter>(AltChapterObject.SmartflowData);
                                
                                
                                if (!(altChapter.Items is null)!)
                                {
                                    LstAltSystemChapterItems = altChapter.Items.Select(T => new VmUsrOrDefChapterManagement { ChapterObject = T }).ToList();

                                    var vmChapterItems = chapterItems.Items is null 
                                                        ? new List<VmUsrOrDefChapterManagement>() 
                                                        : chapterItems.Items.Select(C => new VmUsrOrDefChapterManagement { ChapterObject = C }).ToList();

                                    foreach (var item in vmChapterItems)
                                    {
                                        CompareChapterItemsToAltSytem(item);
                                    }


                                    var fItems = altChapter.Fees is null ? new List<Fee>() : altChapter.Fees;

                                    LstAltSystemFeeItems = fItems.Select(T => new VmFee { FeeObject = T, Compared = false }).ToList();

                                    LstFees = chapterItems.Fees is null
                                                        ? new List<VmFee>()
                                                        : chapterItems.Fees.Select(C => new VmFee { FeeObject = C }).ToList();

                                    foreach (var item in LstFees)
                                    {
                                        CompareFeeItemsToAltSytem(item);
                                    }

                                    var dItems = altChapter.DataViews is null ? new List<DataViews>() : altChapter.DataViews;

                                    LstAltSystemDataViews = dItems.Select(T => new VmDataViews { DataView = T, Compared = false }).ToList();

                                    ListVmDataViews = chapterItems.DataViews is null
                                                        ? new List<VmDataViews>()
                                                        : chapterItems.DataViews.Select(C => new VmDataViews { DataView = C }).ToList();

                                    foreach (var item in ListVmDataViews)
                                    {
                                        CompareDataViewsToAltSytem(item);
                                    }

                                    var tItems = altChapter.TickerMessages is null ? new List<TickerMessages>() : altChapter.TickerMessages;

                                    LstAltSystemTickerMessages = tItems.Select(T => new VmTickerMessages { Message = T, Compared = false }).ToList();

                                    ListVmTickerMessages = chapterItems.TickerMessages is null
                                                        ? new List<VmTickerMessages>()
                                                        : chapterItems.TickerMessages.Select(C => new VmTickerMessages { Message = C }).ToList();

                                    foreach (var item in ListVmTickerMessages)
                                    {
                                        CompareTickerMessagesToAltSytem(item);
                                    }

                                    if (vmChapterItems.Where(C => C.ComparisonResult == "No match" | C.ComparisonResult == "Partial match").Count() > 0 | vmChapterItems.Count() != LstAltSystemChapterItems.Count())
                                    {
                                        chapter.ComparisonResult = "Partial match";
                                        chapter.ComparisonIcon = "exclamation";
                                    }
                                    else if (LstFees.Where(C => C.ComparisonResult == "No match" | C.ComparisonResult == "Partial match").Count() > 0 | LstFees.Count() != LstAltSystemFeeItems.Count())
                                    {
                                        chapter.ComparisonResult = "Partial match";
                                        chapter.ComparisonIcon = "exclamation";
                                    }
                                    else if (ListVmDataViews.Where(C => C.ComparisonResult == "No match" | C.ComparisonResult == "Partial match").Count() > 0 | ListVmDataViews.Count() != LstAltSystemDataViews.Count())
                                    {
                                        chapter.ComparisonResult = "Partial match";
                                        chapter.ComparisonIcon = "exclamation";
                                    }
                                    else if (ListVmTickerMessages.Where(C => C.ComparisonResult == "No match" | C.ComparisonResult == "Partial match").Count() > 0 | ListVmTickerMessages.Count() != LstAltSystemTickerMessages.Count())
                                    {
                                        chapter.ComparisonResult = "Partial match";
                                        chapter.ComparisonIcon = "exclamation";
                                    }
                                    else
                                    {
                                        chapter.ComparisonResult = "Exact match";
                                        chapter.ComparisonIcon = "check";
                                    }

                                }
                                else
                                {
                                    chapter.ComparisonResult = "Partial match";
                                    chapter.ComparisonIcon = "exclamation";
                                }


                                
                                
                            }
                        }
                    }
                    else
                    {
                        LstSelectedChapters.Select(T => { T.ComparisonIcon = "times"; T.ComparisonResult = "No match"; return T; }).ToList();
                    }

                
                }
            }
            catch(Exception e)
            {
                await GenericErrorLog(true,e, "RefreshCompararisonAllChapters", $"Comparing all Smartflows from current listing: {e.Message}");

                return false;
            }

            return true;
        }

        private VmChapterComparison CompareChapterToAltSytem()
        {
            EditChapterComparison = new VmChapterComparison { CurrentChapter = SelectedChapter };

            if (altChapter is null)
            {
                EditChapterComparison.ComparisonResult = "No match";
                EditChapterComparison.ComparisonIcon = "times";
            }
            else
            {
                if (EditChapterComparison.IsChapterMatch(altChapter))
                {
                    EditChapterComparison.ComparisonResult = "Exact match";
                    EditChapterComparison.ComparisonIcon = "check";

                }
                else
                {
                    EditChapterComparison.ComparisonResult = "Partial match";
                    EditChapterComparison.ComparisonIcon = "exclamation";

                }

            }

            return EditChapterComparison;
        }

        private async Task<VmUsrOrDefChapterManagement> CompareChapterItemsToAltSytem(VmUsrOrDefChapterManagement chapterItem)
        {
            try
            {
                var altObject = LstAltSystemChapterItems
                                    .Where(A => A.ChapterObject.Type == chapterItem.ChapterObject.Type)
                                    .Where(A => A.ChapterObject.Name == chapterItem.ChapterObject.Name)
                                    .Where(A => !A.Compared)
                                    .FirstOrDefault();

                if (altObject is null)
                {
                    chapterItem.ComparisonResult = "No match";
                    chapterItem.ComparisonIcon = "times";
                }
                else
                {
                    if (chapterItem.IsChapterItemMatch(altObject))
                    {
                        chapterItem.ComparisonResult = "Exact match";
                        chapterItem.ComparisonIcon = "check";

                    }
                    else
                    {
                        chapterItem.ComparisonResult = "Partial match";
                        chapterItem.ComparisonIcon = "exclamation";

                    }

                }

            }
            catch (Exception e)
            {
                await GenericErrorLog(false,e, "CompareChapterItemsToAltSytem", $"{e.Message}");

                return null;
            }

            return chapterItem;

        }

        private async Task<VmDataViews> CompareDataViewsToAltSytem(VmDataViews dataView)
        {
            try
            {
                
                var altObject = LstAltSystemDataViews
                                    .Where(A => A.DataView.ViewName == dataView.DataView.ViewName)
                                    .Where(A => !A.Compared)
                                    .SingleOrDefault();

                if (altObject is null)
                {
                    dataView.ComparisonResult = "No match";
                    dataView.ComparisonIcon = "times";
                }
                else
                {
                    if (dataView.IsDataViewMatch(altObject))
                    {
                        dataView.ComparisonResult = "Exact match";
                        dataView.ComparisonIcon = "check";
                    }
                    else
                    {
                        dataView.ComparisonResult = "Partial match";
                        dataView.ComparisonIcon = "exclamation";
                    }

                }
            }
            catch (Exception e)
            {
                await GenericErrorLog(false,e, "CompareDataViewsToAltSytem", $"{e.Message}");

                return null;
            }

            return dataView;
        }

        private async Task<VmTickerMessages> CompareTickerMessagesToAltSytem(VmTickerMessages tickerMessage)
        {
            try
            {
                var altObject = LstAltSystemTickerMessages
                                    .Where(A => A.Message.Message == tickerMessage.Message.Message)
                                    .Where(A => !A.Compared)
                                    .FirstOrDefault();

                if (altObject is null)
                {
                    tickerMessage.ComparisonResult = "No match";
                    tickerMessage.ComparisonIcon = "times";
                }
                else
                {
                    if (tickerMessage.IsTickerMessageMatch(altObject))
                    {
                        tickerMessage.ComparisonResult = "Exact match";
                        tickerMessage.ComparisonIcon = "check";
                    }
                    else
                    {
                        tickerMessage.ComparisonResult = "Partial match";
                        tickerMessage.ComparisonIcon = "exclamation";
                    }

                }
            }
            catch (Exception e)
            {
                await GenericErrorLog(false,e, "CompareTickerMessagesToAltSytem", $"{e.Message}");

                return null;
            }

            return tickerMessage;
        }

        private async Task<VmFee> CompareFeeItemsToAltSytem(VmFee chapterItem)
        {
            try
            {
                var altObject = LstAltSystemFeeItems
                                    .Where(A => A.FeeObject.FeeName == chapterItem.FeeObject.FeeName)
                                    .Where(A => !A.Compared)
                                    .SingleOrDefault();

                if (altObject is null)
                {
                    chapterItem.ComparisonResult = "No match";
                    chapterItem.ComparisonIcon = "times";
                }
                else
                {
                    if (chapterItem.IsChapterItemMatch(altObject))
                    {
                        chapterItem.ComparisonResult = "Exact match";
                        chapterItem.ComparisonIcon = "check";

                    }
                    else
                    {
                        chapterItem.ComparisonResult = "Partial match";
                        chapterItem.ComparisonIcon = "exclamation";

                    }

                }
            }
            catch (Exception e)
            {
                await GenericErrorLog(false,e, "CompareFeeItemsToAltSytem", $"{e.Message}");

                return null;
            }

            return chapterItem;
        }

        public async void CompareChapterItemsToAltSytemAction()
        {
            await CompareSelectedChapterToAltSystem();
        }

        private async void PrepareHeaderForComparison()
        {
            await CompareSelectedChapterToAltSystem();
            
            await ShowHeaderComparisonModal();
        }

        protected async Task ShowHeaderComparisonModal()
        {
            try
            {
                Action Compare = CompareChapterItemsToAltSytemAction;

                var parameters = new ModalParameters();
                parameters.Add("Object", EditChapterComparison);
                parameters.Add("ComparisonRefresh", Compare);
                parameters.Add("UserSession", UserSession);
                parameters.Add("CurrentSysParentId", SelectedChapterId);
                parameters.Add("AlternateSysParentId", altSysSelectedChapterId);
                parameters.Add("CurrentChapter", SelectedChapter);
                parameters.Add("AltChapter", altChapter);
                parameters.Add("CurrentChapterRow", SelectedChapterObject);
                parameters.Add("AltChapterRow", AltChapterObject);
                parameters.Add("CompanyDbAccess", CompanyDbAccess);

                var options = new ModalOptions()
                {
                    Class = "blazored-custom-modal modal-chapter-comparison"
                };

                Modal.Show<ChapterHeaderComparison>("Synchronise Smartflow Item", parameters, options);
            }
            catch(Exception e)
            {
                await GenericErrorLog(false,e, "ShowHeaderComparisonModal", e.Message);
            }
        }

        protected async Task PrepareDeleteAltObject(VmUsrOrDefChapterManagement selectedItem)
        {
            try
            {
                EditObject = selectedItem;

                Action SelectedDeleteAction = HandleAltDetailDelete;
                var parameters = new ModalParameters();
                parameters.Add("ItemName", EditObject.ChapterObject.Name);
                parameters.Add("ModalHeight", "300px");
                parameters.Add("ModalWidth", "500px");
                parameters.Add("DeleteAction", SelectedDeleteAction);
                parameters.Add("InfoText", $"Are you sure you wish to delete the '{EditObject.ChapterObject.Name}' {EditObject.ChapterObject.Type}?");

                var options = new ModalOptions()
                {
                    Class = "blazored-custom-modal"
                };

                Modal.Show<ModalDelete>($"Delete {EditObject.ChapterObject.Type}", parameters, options);
            }
            catch(Exception e)
            {
                await GenericErrorLog(false,e, "PrepareDeleteAltObject", e.Message);
            }

        }

        protected async Task PrepareDeleteAltDataView(VmDataViews selectedItem)
        {
            try
            {
                EditDataViewObject = selectedItem;

                Action SelectedDeleteAction = HandleAltDataViewDelete;
                var parameters = new ModalParameters();
                parameters.Add("ItemName", EditObject.ChapterObject.Name);
                parameters.Add("ModalHeight", "300px");
                parameters.Add("ModalWidth", "500px");
                parameters.Add("DeleteAction", SelectedDeleteAction);
                parameters.Add("InfoText", $"Are you sure you wish to delete the '{EditDataViewObject.DataView.ViewName}' View?");

                var options = new ModalOptions()
                {
                    Class = "blazored-custom-modal"
                };

                Modal.Show<ModalDelete>($"Delete Data View", parameters, options);
            }
            catch(Exception e)
            {
                await GenericErrorLog(false,e, "PrepareDeleteAltDataView", e.Message);
            }
        }

        protected async Task PrepareDeleteAltFee(VmFee selectedItem)
        {
            try
            {
                EditFeeObject = selectedItem;

                Action SelectedDeleteAction = HandleAltFeeDelete;
                var parameters = new ModalParameters();
                parameters.Add("ItemName", EditObject.ChapterObject.Name);
                parameters.Add("ModalHeight", "300px");
                parameters.Add("ModalWidth", "500px");
                parameters.Add("DeleteAction", SelectedDeleteAction);
                parameters.Add("InfoText", $"Are you sure you wish to delete the '{EditFeeObject.FeeObject.FeeName}' Fee?");

                var options = new ModalOptions()
                {
                    Class = "blazored-custom-modal"
                };

                Modal.Show<ModalDelete>($"Delete Fee", parameters, options);
            }
            catch(Exception e)
            {
                await GenericErrorLog(false,e, "PrepareDeleteAltFee", e.Message);
            }

        }

        protected async Task PrepareDeleteAltMessage(VmTickerMessages selectedItem)
        {
            try
            {
                EditTickerMessageObject = selectedItem;

                Action SelectedDeleteAction = HandleAltMessageDelete;
                var parameters = new ModalParameters();
                parameters.Add("ItemName", EditObject.ChapterObject.Name);
                parameters.Add("ModalHeight", "300px");
                parameters.Add("ModalWidth", "500px");
                parameters.Add("DeleteAction", SelectedDeleteAction);
                parameters.Add("InfoText", $"Are you sure you wish to delete this message?");

                var options = new ModalOptions()
                {
                    Class = "blazored-custom-modal"
                };

                Modal.Show<ModalDelete>($"Delete Message", parameters, options);
            }
            catch(Exception e)
            {
                await GenericErrorLog(false,e, "PrepareDeleteAltMessage", e.Message);
            }
        }

        private async void HandleAltDetailDelete()
        {
            await UserSession.SwitchSelectedSystem();
            altChapter.Items.Remove(EditObject.ChapterObject);
            AltChapterObject.SmartflowData = JsonConvert.SerializeObject(altChapter);
            await ChapterManagementService.Update(AltChapterObject);

            await UserSession.ResetSelectedSystem();

            await CompareSelectedChapterToAltSystem();
        }

        private async void HandleAltFeeDelete()
        {
            await UserSession.SwitchSelectedSystem();
            altChapter.Fees.Remove(EditFeeObject.FeeObject);
            AltChapterObject.SmartflowData = JsonConvert.SerializeObject(altChapter);
            await ChapterManagementService.Update(AltChapterObject);

            await UserSession.ResetSelectedSystem();

            await CompareSelectedChapterToAltSystem();
        }

        private async void HandleAltDataViewDelete()
        {
            await UserSession.SwitchSelectedSystem();
            altChapter.DataViews.Remove(EditDataViewObject.DataView);
            AltChapterObject.SmartflowData = JsonConvert.SerializeObject(altChapter);
            await ChapterManagementService.Update(AltChapterObject);

            await UserSession.ResetSelectedSystem();

            await CompareSelectedChapterToAltSystem();
        }

        private async void HandleAltMessageDelete()
        {
            await UserSession.SwitchSelectedSystem();
            altChapter.TickerMessages.Remove(EditTickerMessageObject.Message);
            AltChapterObject.SmartflowData = JsonConvert.SerializeObject(altChapter);
            await ChapterManagementService.Update(AltChapterObject);

            await UserSession.ResetSelectedSystem();

            await CompareSelectedChapterToAltSystem();
        }

        private void PrepareForComparison(VmUsrOrDefChapterManagement selectedItem)
        {
            EditObject = selectedItem;

            ShowChapterComparisonModal();
        }

        protected void ShowChapterComparisonModal()
        {
            Action Compare = CompareChapterItemsToAltSytemAction;

            var parameters = new ModalParameters();
            parameters.Add("Object", EditObject);
            parameters.Add("ComparisonRefresh", Compare);
            parameters.Add("UserSession", UserSession);
            parameters.Add("CurrentSysParentId", SelectedChapterId);
            parameters.Add("AlternateSysParentId", altSysSelectedChapterId);
            parameters.Add("CurrentChapter", SelectedChapter);
            parameters.Add("AltChapter", altChapter);
            parameters.Add("CurrentChapterRow", SelectedChapterObject);
            parameters.Add("AltChapterRow", AltChapterObject);
            parameters.Add("CompanyDbAccess", CompanyDbAccess);
            parameters.Add("CreateNewSmartflow", CreateNewSmartflow);

            var options = new ModalOptions()
            {
                Class = "blazored-custom-modal modal-chapter-comparison"
            };

            Modal.Show<ChapterItemComparison>("Synchronise Smartflow Item", parameters, options);
        }

        private void PrepareFeeForComparison(VmFee selectedItem)
        {
            EditFeeObject = selectedItem;

            ShowFeeComparisonModal();
        }

        protected void ShowFeeComparisonModal()
        {
            Action Compare = CompareChapterItemsToAltSytemAction;

            var parameters = new ModalParameters();
            parameters.Add("Object", EditFeeObject);
            parameters.Add("ComparisonRefresh", Compare);
            parameters.Add("UserSession", UserSession);
            parameters.Add("CurrentSysParentId", SelectedChapterId);
            parameters.Add("AlternateSysParentId", altSysSelectedChapterId);
            parameters.Add("CurrentChapter", SelectedChapter);
            parameters.Add("AltChapter", altChapter);
            parameters.Add("CurrentChapterRow", SelectedChapterObject);
            parameters.Add("AltChapterRow", AltChapterObject);
            parameters.Add("CompanyDbAccess", CompanyDbAccess);
            parameters.Add("CreateNewSmartflow", CreateNewSmartflow);

            var options = new ModalOptions()
            {
                Class = "blazored-custom-modal modal-chapter-comparison"
            };

            Modal.Show<ChapterFeeComparison>("Synchronise Smartflow Item", parameters, options);
        }

        private void PrepareDataViewForComparison(VmDataViews selectedItem)
        {
            EditDataViewObject = selectedItem;

            ShowDataViewComparisonModal();
        }

        private void PrepareTickerMessageForComparison(VmTickerMessages selectedItem)
        {
            EditTickerMessageObject = selectedItem;

            ShowTickerMessageComparisonModal();
        }

        protected void ShowDataViewComparisonModal()
        {
            Action Compare = CompareChapterItemsToAltSytemAction;

            var parameters = new ModalParameters();
            parameters.Add("Object", EditDataViewObject);
            parameters.Add("ComparisonRefresh", Compare);
            parameters.Add("UserSession", UserSession);
            parameters.Add("CurrentSysParentId", SelectedChapterId);
            parameters.Add("AlternateSysParentId", altSysSelectedChapterId);
            parameters.Add("CurrentChapter", SelectedChapter);
            parameters.Add("AltChapter", altChapter);
            parameters.Add("CurrentChapterRow", SelectedChapterObject);
            parameters.Add("AltChapterRow", AltChapterObject);
            parameters.Add("CompanyDbAccess", CompanyDbAccess);
            parameters.Add("CreateNewSmartflow", CreateNewSmartflow);

            var options = new ModalOptions()
            {
                Class = "blazored-custom-modal modal-chapter-comparison"
            };

            Modal.Show<ChapterDataViewComparison>("Synchronise Smartflow Item", parameters, options);
        }

        protected void ShowTickerMessageComparisonModal()
        {
            Action Compare = CompareChapterItemsToAltSytemAction;

            var parameters = new ModalParameters();
            parameters.Add("Object", EditTickerMessageObject);
            parameters.Add("ComparisonRefresh", Compare);
            parameters.Add("UserSession", UserSession);
            parameters.Add("CurrentSysParentId", SelectedChapterId);
            parameters.Add("AlternateSysParentId", altSysSelectedChapterId);
            parameters.Add("CurrentChapter", SelectedChapter);
            parameters.Add("AltChapter", altChapter);
            parameters.Add("CurrentChapterRow", SelectedChapterObject);
            parameters.Add("AltChapterRow", AltChapterObject);
            parameters.Add("CompanyDbAccess", CompanyDbAccess);
            parameters.Add("CreateNewSmartflow",CreateNewSmartflow);

            var options = new ModalOptions()
            {
                Class = "blazored-custom-modal modal-chapter-comparison"
            };

            Modal.Show<TickerMessageComparison>("Synchronise Smartflow Item", parameters, options);
        }

        private async void CreateSelectedChapterOnAlt()
        {
            try
            {
                await UserSession.SwitchSelectedSystem();

                AltChapterObject = LstAltSystemChapters
                                            .Where(A => A.SmartflowObject.SmartflowName == SelectedChapterObject.SmartflowName)
                                            .Where(A => A.SmartflowObject.CaseType == SelectedChapterObject.CaseType)
                                            .Where(A => A.SmartflowObject.CaseTypeGroup == SelectedChapterObject.CaseTypeGroup)
                                            .Select(C => C.SmartflowObject)
                                            .SingleOrDefault();

                if (AltChapterObject is null)
                {
                    var newAltChapterObject = new UsrOrsfSmartflows
                    {
                        SeqNo = SelectedChapterObject.SeqNo,
                        CaseTypeGroup = SelectedChapterObject.CaseTypeGroup,
                        CaseType = SelectedChapterObject.CaseType,
                        SmartflowName = SelectedChapterObject.SmartflowName,
                        SmartflowData = SelectedChapterObject.SmartflowData,
                        VariantName = SelectedChapterObject.VariantName,
                        VariantNo = SelectedChapterObject.VariantNo
                    };

                    var returnObject = await ChapterManagementService.Add(newAltChapterObject);
                    newAltChapterObject.Id = returnObject.Id;

                    bool gotLock = CompanyDbAccess.Lock;
                    while (gotLock)
                    {
                        await Task.Yield();
                        gotLock = CompanyDbAccess.Lock;
                    }

                    await CompanyDbAccess.SaveSmartFlowRecord(newAltChapterObject, UserSession);
                }
                else
                {
                    AltChapterObject.SmartflowData = SelectedChapterObject.SmartflowData;

                    await ChapterManagementService.Update(AltChapterObject);
                }


                
                await UserSession.ResetSelectedSystem();

                await RefreshCompararisonAllChapters();

                await InvokeAsync(() =>
                {
                    StateHasChanged();
                });
            }
            catch(HttpRequestException)
            {
                //do nothing, already dealt with
            }
            catch(Exception e)
            {
                await GenericErrorLog(false,e, "CreateSelectedChapterOnAlt", e.Message);
            }

        }

        protected async Task PrepareChapterCreateOnAlt(UsrOrsfSmartflows SelectedChapter)
        {
            SelectedChapterObject = SelectedChapter;

            string infoText = $"Do you wish to sync this smartflow to {(UserSession.SelectedSystem == "Live" ? "Dev" : "Live")}.";

            Action SelectedAction = CreateSelectedChapterOnAlt;
            var parameters = new ModalParameters();
            parameters.Add("InfoHeader", "Confirm Action");
            parameters.Add("InfoText", infoText);
            parameters.Add("ConfirmAction", SelectedAction);

            var options = new ModalOptions()
            {
                Class = "blazored-custom-modal modal-confirm"
            };

            Modal.Show<ModalConfirm>("Confirm", parameters, options);
        }

#endregion

#region Sync Systems
        private void HandleChapterSync()
        {
            if (NavDisplay.ToLower() == "chapter")
            {
                SyncAll();
            }
            else
            {
                SyncToAltSystem(NavDisplay);
            }

        }

        private async void SyncAll()
        {
            CompareSystems = true;
            await CompareSelectedChapterToAltSystem();
            CompareSystems = false;
            SyncToAltSystem("All");
        }


        private async void SyncToAltSystem(string option)
        {
            try
            {
                var selectedCopyItems = new VmChapter { Items = new List<GenSmartflowItem>()
                                                        , Fees = new List<Fee>()
                                                        , DataViews = new List<DataViews>()
                                                        , TickerMessages = new List<TickerMessages>()};

                if (!(AltChapterObject is null))
                {
                    if (!string.IsNullOrEmpty(AltChapterObject.SmartflowData))
                    {
                        selectedCopyItems = JsonConvert.DeserializeObject<VmChapter>(AltChapterObject.SmartflowData);
                    }
                }
                else
                {
                    AltChapterObject = new UsrOrsfSmartflows 
                                        { 
                                            CaseType = SelectedChapterObject.CaseType
                                            ,CaseTypeGroup = SelectedChapterObject.CaseTypeGroup
                                            ,SeqNo = SelectedChapterObject.SeqNo
                                            ,SmartflowName = SelectedChapterObject.SmartflowName
                                            ,VariantName = SelectedChapterObject.VariantName
                                            ,VariantNo = SelectedChapterObject.VariantNo
                                        };
                }


                if (option == "Agenda" | option == "All")
                {
                    foreach (var item in selectedCopyItems.Items.Where(C => C.Type == "Agenda").ToList())
                    {
                        selectedCopyItems.Items.Remove(item);
                    }

                    selectedCopyItems.Items.AddRange(SelectedChapter.Items.Where(C => C.Type == "Agenda").ToList());
                }



                if (option == "Status" | option == "All")
                {
                    foreach (var item in selectedCopyItems.Items.Where(C => C.Type == "Status").ToList())
                    {
                        selectedCopyItems.Items.Remove(item);
                    }

                    selectedCopyItems.Items.AddRange(SelectedChapter.Items.Where(C => C.Type == "Status").ToList());
                }



                if (option == "Docs" | option == "All")
                {
                    foreach (var item in selectedCopyItems.Items.Where(C => C.Type == "Doc").ToList())
                    {
                        selectedCopyItems.Items.Remove(item);
                    }

                    selectedCopyItems.Items.AddRange(SelectedChapter.Items.Where(C => C.Type == "Doc").ToList());
                }


                if (option == "Fees" | option == "All")
                {
                    foreach (var item in selectedCopyItems.Fees.ToList())
                    {
                        selectedCopyItems.Fees.Remove(item);
                    }

                    selectedCopyItems.Fees.AddRange(SelectedChapter.Fees.ToList());
                }


                if (option == "DataViews" | option == "All")
                {
                    foreach (var item in selectedCopyItems.DataViews.ToList())
                    {
                        selectedCopyItems.DataViews.Remove(item);
                    }

                    selectedCopyItems.DataViews.AddRange(SelectedChapter.DataViews.ToList());
                }

                if (option == "TickerMessages" | option == "All")
                {
                    foreach (var item in selectedCopyItems.TickerMessages.ToList())
                    {
                        selectedCopyItems.TickerMessages.Remove(item);
                    }

                    selectedCopyItems.TickerMessages.AddRange(SelectedChapter.TickerMessages.ToList());
                }

                AltChapterObject.SmartflowData = JsonConvert.SerializeObject(new VmChapter
                {
                    CaseTypeGroup = SelectedChapterObject.CaseTypeGroup,
                    CaseType = SelectedChapterObject.CaseType,
                    Name = SelectedChapterObject.SmartflowName,
                    SeqNo = SelectedChapterObject.SeqNo.GetValueOrDefault(),
                    Items = selectedCopyItems.Items,
                    DataViews = SelectedChapter.DataViews,
                    Fees = selectedCopyItems.Fees,
                    TickerMessages = selectedCopyItems.TickerMessages
                });

                await UserSession.SwitchSelectedSystem();

                if (AltChapterObject.Id == 0)
                {
                    var returnObject = await ChapterManagementService.Add(AltChapterObject);
                    AltChapterObject.Id = returnObject.Id;

                    bool gotLock = CompanyDbAccess.Lock;
                    while (gotLock)
                    {
                        await Task.Yield();
                        gotLock = CompanyDbAccess.Lock;
                    }

                    await CompanyDbAccess.SaveSmartFlowRecord(AltChapterObject, UserSession);
                }
                else
                {
                    await ChapterManagementService.Update(AltChapterObject);
                }

                await UserSession.ResetSelectedSystem();

                await CompareSelectedChapterToAltSystem();
                StateHasChanged();
            }
            catch(HttpRequestException)
            {
                //do nothing, already dealt with
            }
            catch(Exception e)
            {
                await GenericErrorLog(false,e, "SyncToAltSystem", e.Message).ConfigureAwait(false);
            }
        }

       
        public async void SyncSmartFlowSystems()
        {
            try
            {
                var lstC = await ChapterManagementService.GetAllChapters();

                await CompanyDbAccess.SyncAdminSysToClient(lstC, UserSession);

                RefreshChapters();

                var parameters = new ModalParameters();
                parameters.Add("InfoText", "Systems synced successfully");

                var options = new ModalOptions()
                {
                    Class = "blazored-custom-modal modal-chapter-chapter"
                };
                string title = $"Systems Synced";
                Modal.Show<ModalInfo>(title, parameters, options);
            }
            catch(HttpRequestException)
            {
                //do nothing, already dealt with
            }
            catch(Exception e)
            {
                await GenericErrorLog(true,e, "SyncSmartFlowSystems", e.Message);
            }
                
            
        }

#endregion

#region File Handling

        public void GetFile(FileDesc fileDesc)
        {
            NavigationManager.NavigateTo(fileDesc.FilePath + "//" + fileDesc.FileName, true);
        }

        public async void WriteChapterJSONToFile()
        {
            try
            {
                SetSmartflowFilePath();

                var fileName = SelectedChapter.Name + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".txt";

                ChapterFileUpload.WriteChapterToFile(SelectedChapterObject.SmartflowData, fileName);

                GetSeletedChapterFileList();
                StateHasChanged();
            }
            catch(Exception e)
            {
                await GenericErrorLog(false,e, "WriteChapterJSONToFile", e.Message);
            }
            
        }

        private async void HandleBackupFileSelection(IFileListEntry[] entryFiles)
        {
            try
            {
                SetSmartflowFilePath();
                var files = new List<IFileListEntry>();
                IList<string> fileErrorDescs = new List<string>();

                foreach (var file in entryFiles)
                {
                    if (file != null)
                    {
                        if(!(file.Name.Contains(".txt") || file.Name.Contains(".JSON")))
                        {
                            fileErrorDescs.Add($"The file: {file.Name} is not the correct type for a backup. Backup files must be either .txt or .JSON");
                        }
                        else
                        {
                            if(ListFilesForBackups.Where(F => F.FileName == file.Name).FirstOrDefault() is null)
                            {
                                await ChapterFileUpload.UploadChapterFiles(file);
                                files.Add(file);
                            }
                            else
                            {
                                fileErrorDescs.Add($"The file: {file.Name} already exists on the system");
                            }

                            
                        }
                        
                    }

                }
                if (files != null && files.Count > 0)
                {
                    StateHasChanged();
                }

                if(fileErrorDescs.Count > 0)
                {
                    ShowErrorModal("File Upload Error", "The following errors occured during the upload:", fileErrorDescs);
                }

                GetSeletedChapterFileList();

                StateHasChanged();
            }
            catch(Exception e)
            {
                await GenericErrorLog(false,e, "HandleBackupFileSelection", e.Message);
            }
        }

        private void GetSeletedChapterFileList()
        {
            ListFilesForBackups = ChapterFileUpload.GetFileListForChapter();
        }

        private void ReadBackUpFile(string filePath)
        {
            var readJSON = ChapterFileUpload.readJson(filePath);
            SaveJson(readJSON);
        }

        private async void DownloadFile(FileDesc file)
        {
            var data = ChapterFileUpload.ReadFileToByteArray(file.FilePath);

            await JSRuntime.InvokeAsync<object>(
                 "DownloadTextFile",
                 file.FileName,
                 Convert.ToBase64String(data));


        }

        private void HandleDeleteBackupFile()
        {
            DeleteBackupFile(SelectedFileDescription);
        }

        private void DeleteBackupFile(FileDesc file)
        {
            ChapterFileUpload.DeleteFile(file.FilePath);
            GetSeletedChapterFileList();
            StateHasChanged();
        }


        private async void SaveJson(string Json)
        {
            JsonErrors = new List<string>();
            JsonErrors = ChapterFileUpload.ValidateChapterJSON(Json);

            if (JsonErrors.Count == 0)
            {
                try
                {
                    var chapterData = JsonConvert.DeserializeObject<VmChapter>(Json);
                    SelectedChapter.Items = chapterData.Items;
                    SelectedChapter.DataViews = chapterData.DataViews;
                    SelectedChapter.Fees = chapterData.Fees;
                    SelectedChapter.TickerMessages = chapterData.TickerMessages;
                    SelectedChapter.P4WCaseTypeGroup = chapterData.P4WCaseTypeGroup;
                    SelectedChapter.SelectedStep = chapterData.SelectedStep;
                    SelectedChapter.SelectedView = chapterData.SelectedView;
                    SelectedChapter.ShowPartnerNotes = chapterData.ShowPartnerNotes;
                    SelectedChapter.ShowDocumentTracking = chapterData.ShowDocumentTracking;
                    SelectedChapterObject.SmartflowData = JsonConvert.SerializeObject(SelectedChapter);
                    
                    await ChapterManagementService.Update(SelectedChapterObject);


                    //keep track of time last updated ready for comparison by other sessions checking for updates
                    AppChapterState.SetLastUpdated(UserSession, SelectedChapter);

                    SelectChapter(SelectedChapterObject);

                    ShowJSON = false;

                    await InvokeAsync(() =>
                    {
                        StateHasChanged();
                    });
                }
                catch(Exception e)
                {
                    JsonErrors.Add("Error processing data");
                    ShowErrorModal("Backup Restore Error", "The following errors occured during the restore:", JsonErrors);
                }

            }
            else
            {
                ShowErrorModal("Backup Restore Error", "The following errors occured during the restore:", JsonErrors);
            }
        }

        private async void ExportSmartflowToExcel()
        {
            WriteChapterJSONToFile();
            await ChapterFileUpload.WriteChapterDataToExcel(SelectedChapter, DropDownChapterList, PartnerCaseTypeGroups);

        }

        private void RefreshJson()
        {
            SaveJson(SelectedChapterObject.SmartflowData);
        }

        /// <summary>
        /// TODO: Need to work out what this method does
        /// </summary>
        private async Task SetSmartflowFilePath()
        {
            try
            {
                ChapterFileOption = new ChapterFileOptions
                {
                    Company = UserSession.Company.CompanyName,
                    CaseTypeGroup = SelectedChapter.CaseTypeGroup,
                    CaseType = SelectedChapter.CaseType,
                    Chapter = SelectedChapter.Name
                };

                ChapterFileUpload.SetChapterOptions(ChapterFileOption);
            }
            catch (Exception ex)
            {
                await GenericErrorLog(true,ex, "SetSmartflowFilePath", $"Setting Smartflow file path: {ex.Message}");
            }
        }

#endregion 

#region Error Handling

        private void DisplaySmartflowLoadError(string Error)
        {
            var parameters = new ModalParameters();
            parameters.Add("ErrorDesc", $"Error loading smartflow for the following reason: ");
            parameters.Add("ErrorDetails", new List<string> {Error});

            var options = new ModalOptions()
            {
                Class = "blazored-custom-modal modal-chapter-chapter"
            };
            string title = $"Smartflow Error";
            Modal.Show<ModalErrorInfo>(title, parameters, options);
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

        /****************************************/
        /* ERROR HANDLING */
        /****************************************/
        private async Task GenericErrorLog(bool showNotificationMsg, Exception e, string _method, string _message)
        {
            using (LogContext.PushProperty("SourceSystem", UserSession.SelectedSystem))
            using (LogContext.PushProperty("SourceCompanyId", UserSession.Company.Id))
            using (LogContext.PushProperty("SourceUserId", UserSession.User.Id))
            using (LogContext.PushProperty("SourceContext", nameof(ChapterList)))
            {
                Logger.LogError(e,"Error - Method: {0}, Message: {1}",_method, _message);
            }

            if(showNotificationMsg)
            {
                await NotificationManager.ShowNotification("Danger", $"Oops! Something went wrong.");
            }
        }


#endregion

    }
}
