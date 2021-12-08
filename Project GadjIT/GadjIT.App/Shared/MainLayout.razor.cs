using GadjIT.AppContext.GadjIT_App;
using GadjIT_App.Data;
using GadjIT_App.Services.SessionState;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Server.HttpSys;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;

namespace GadjIT_App.Shared
{
    public partial class MainLayout
    {
        private readonly ILogger<MainLayout> logger;

        public MainLayout()
        {

        }

        public MainLayout(ILogger<MainLayout> logger)
        {
            this.logger = logger;
        }

        [Inject]
        protected IUserSessionState sessionState { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        public AspNetUsers currentUser { get; set; }

        public int selectedCompanyId { get; set; }

        public bool hideTopbar { get; set; } = false;
        public bool hideSidebar { get; set; } = false;

        public string parallax { get; set; } = "";

        protected override async Task OnInitializedAsync()
        {
            try
            {
                bool gotLock = sessionState.Lock;
                while (gotLock)
                {
                    await Task.Yield();
                    gotLock = sessionState.Lock;
                }

                if (sessionState.User is null)
                {
                    await sessionState.SetSessionState();
                    currentUser = sessionState.User;
                }

                sessionState.RefreshHome = Refresh;

                if (currentUser is null)
                {
                    string returnUrl = HttpUtility.UrlEncode("/" + HttpUtility.UrlDecode(NavigationManager.Uri.Replace(NavigationManager.BaseUri, "")));
                    NavigationManager.NavigateTo($"Identity/Account/Login?returnUrl={returnUrl}", true);
                }

                //Check if sidebar and topbar is not required, i.e. when exporting data via DataTables

                var dExport = "";

                var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);

                if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("dxport", out var param))
                {
                    dExport = param.FirstOrDefault();

                    if (dExport != null)
                    {
                        hideTopbar = true;
                        hideSidebar = true;
                    }
                }

                StateHasChanged();
            }
            catch (Exception e)
            {
                logger.LogError(e,$"Error caught on main layout");
            }


        }

        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                try
                {
                    setParallax();
                }
                catch(Exception e)
                {
                    logger.LogError(e, "Error setting parallax");
                }
            }
        }

        public async void setParallax()
        {
            if (!(sessionState.User is null))
            {
                if (!string.IsNullOrEmpty(sessionState.TempBackGroundImage) && sessionState.User.DisplaySmartflowPreviewImage)
                {
                    //if normal bg is to be overridden with a temp image i.e. Smartflow preview image
                    parallax = ".parallax { background-image: url('" + sessionState.TempBackGroundImage + "');  } .inner-content{ background-image: none;}";
                }
                else if (!(sessionState.User is null) && !string.IsNullOrEmpty(sessionState.User.MainBackgroundImage))
                {
                    if (sessionState.selectedSystem == "Live")
                    {
                        parallax = ".parallax {  background-color: #DDDDDD }";
                    }
                    else
                    {
                        parallax = ".parallax {  background-color: #555555 }";
                    }
                    //if (sessionState.User.MainBackgroundImage.Contains("#"))
                    //{

                    //}

                    //else
                    //{
                    //    parallax = ".parallax { background-image: url('" + sessionState.User.MainBackgroundImage + "');  } .inner-content{ background-image: none;}";
                    //}
                }
                else
                {
                    parallax = ".parallax {  background-color: #666666 }";
                }
                await InvokeAsync(() =>
                {
                    StateHasChanged();
                });
            }
        }


        private void ToggleCompany(int companyId)
        {
            currentUser.SelectedCompanyId = companyId;
            //await sessionState.switchSelectedCompany(companyId);

            /*
             * Need to redirect to Identity Section
             * Can't use sign in manager from main blazor section
             * something to do with authentication state cookies
             */

            NavigationManager.NavigateTo("/Identity/Account/LogOutOnGet", true);
        }


        public void Refresh()
        {
            try
            {
                setParallax();
            }
            catch(Exception e)
            {
                logger.LogError(e, "Error refreshing parallax");
            }
        }


    }
}
