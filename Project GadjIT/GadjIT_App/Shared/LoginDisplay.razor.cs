using Blazored.Modal;
using Blazored.Modal.Services;
using GadjIT_App.Data;
using GadjIT_App.Data.Admin;
using GadjIT_App.Pages._Shared.Modals;
using GadjIT_App.Services.SessionState;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GadjIT_App.Shared
{
    public partial class LoginDisplay
    {
        [Inject]
        IModalService Modal { get; set; }

        [Parameter]
        public string UserFullName { get; set; }

        [Inject]
        public IUserSessionState UserSession { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        public string ModalInfoHeader { get; set; }
        public string ModalHeight { get; set; }
        public string ModalWidth { get; set; }

        public string SessionCompany { get; set; }

        public void NavigateToUserProfile()
        {
            RefreshBackGround();
            //set Return URI
            UserSession.SetUserProfileReturnURI(NavigationManager.Uri);
            NavigationManager.NavigateTo("/userprofile");
        }



        private void RefreshBackGround()
        {
            if (!string.IsNullOrEmpty(UserSession.TempBackGroundImage))
            {
                UserSession.TempBackGroundImage = "";
                UserSession.RefreshHome?.Invoke();
            }

        }


        protected override async Task OnInitializedAsync()
        {
            bool gotLock = UserSession.Lock;
            while (gotLock)
            {
                await Task.Yield();
                gotLock = UserSession.Lock;
            }

            StateHasChanged();
        }

        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                StateHasChanged();
            }
        }


        protected void ShowSystemSelectModel()
        {
            var parameters = new ModalParameters();
            parameters.Add("currentUser", UserSession.User);

            var options = new ModalOptions()
            {
                Class = "blazored-custom-modal"
            };


            Modal.Show<ModalSystemSelect>("System Select", parameters, options);
        }


    }
}
