using GadjIT_AppContext.GadjIT_App;
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

        [Inject]
        private ILogger<MainLayout> Logger { get; set; }



        [Inject]
        protected IUserSessionState SessionState { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        public AspNetUsers CurrentUser { get; set; }

        public int SelectedCompanyId { get; set; }

        public bool HideTopbar { get; set; } = false;
        public bool HideSidebar { get; set; } = false;

        public string Parallax { get; set; } = "";

        protected override async Task OnInitializedAsync()
        {
            try
            {
                bool gotLock = SessionState.Lock;
                while (gotLock)
                {
                    await Task.Yield();
                    gotLock = SessionState.Lock;
                }

                if (SessionState.User is null)
                {
                    await SessionState.SetSessionState();
                    CurrentUser = SessionState.User;
                }

                SessionState.RefreshHome = Refresh;

                if (CurrentUser is null)
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
                        HideTopbar = true;
                        HideSidebar = true;
                    }
                }

                StateHasChanged();
            }
            catch (Exception e)
            {
                Logger.LogError(e,$"Error caught on main layout");
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
                    Logger.LogError(e, "Error setting parallax");
                }
            }
        }

        public async void setParallax()
        {
            if (!(SessionState.User is null))
            {
                if (!string.IsNullOrEmpty(SessionState.TempBackGroundImage) && SessionState.User.DisplaySmartflowPreviewImage)
                {
                    //if normal bg is to be overridden with a temp image i.e. Smartflow preview image
                    Parallax = ".parallax { background-image: url('" + SessionState.TempBackGroundImage + "');  } .inner-content{ background-image: none;}";
                }
                else if (!(SessionState.User is null) && !string.IsNullOrEmpty(SessionState.User.MainBackgroundImage))
                {
                    if (SessionState.SelectedSystem == "Live")
                    {
                        Parallax = ".parallax {  background-color: #DDDDDD }";
                    }
                    else
                    {
                        Parallax = ".parallax {  background-color: #555555 }";
                    }
                    //if (SessionState.User.MainBackgroundImage.Contains("#"))
                    //{

                    //}

                    //else
                    //{
                    //    parallax = ".parallax { background-image: url('" + SessionState.User.MainBackgroundImage + "');  } .inner-content{ background-image: none;}";
                    //}
                }
                else
                {
                    Parallax = ".parallax {  background-color: #666666 }";
                }
                await InvokeAsync(() =>
                {
                    StateHasChanged();
                });
            }
        }


        private void ToggleCompany(int companyId)
        {
            CurrentUser.SelectedCompanyId = companyId;
            //await SessionState.switchSelectedCompany(companyId);

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
                Logger.LogError(e, "Error refreshing parallax");
            }
        }


    }
}
