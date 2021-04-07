using GadjIT.GadjitContext.GadjIT_App;
using Gizmo_V1_02.Data;
using Gizmo_V1_02.Services.SessionState;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Server.HttpSys;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;

namespace Gizmo_V1_02.Shared
{
    public partial class MainLayout
    {

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

            if(currentUser is null)
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

                if(dExport != null)
                {
                    hideTopbar = true;
                    hideSidebar = true;
                }
            }



            StateHasChanged();
        }

        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                setParallax();
            }
        }

        public void setParallax()
        {
            if (!(sessionState.User is null))
            {
                if (!string.IsNullOrEmpty(sessionState.TempBackGroundImage) && sessionState.User.DisplaySmartflowPreviewImage)
                {
                    if (sessionState.TempBackGroundImage.Contains("#"))
                    {
                        parallax = ".parallax {  background-color: " + sessionState.TempBackGroundImage + " }";
                    }

                    else
                    {
                        parallax = ".parallax { background-image: url('" + sessionState.GetBackgroundImage() + "');  } .inner-content{ background-image: none;}";
                    }
                }
                else if (!(sessionState.User is null) && !string.IsNullOrEmpty(sessionState.User.MainBackgroundImage))
                {
                    if (sessionState.User.MainBackgroundImage.Contains("#"))
                    {
                        parallax = ".parallax {  background-color: " + sessionState.User.MainBackgroundImage + " }";
                    }

                    else
                    {
                        parallax = ".parallax { background-image: url('" + sessionState.User.MainBackgroundImage + "');  } .inner-content{ background-image: none;}";
                    }
                }
                else
                {
                    parallax = ".parallax {  background-color: #666666 }";
                }
                StateHasChanged();
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
            setParallax();
        }


    }
}
