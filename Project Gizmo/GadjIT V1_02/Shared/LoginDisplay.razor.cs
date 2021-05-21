using Blazored.Modal;
using Blazored.Modal.Services;
using GadjIT_V1_02.Data;
using GadjIT_V1_02.Data.Admin;
using GadjIT_V1_02.Pages.Shared.Modals;
using GadjIT_V1_02.Services.SessionState;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GadjIT_V1_02.Shared
{
    public partial class LoginDisplay
    {
        [Inject]
        IModalService Modal { get; set; }

        [Parameter]
        public string userFullName { get; set; }

        [Inject]
        public IUserSessionState userSession { get; set; }

        [Inject]
        public NavigationManager navigationManager { get; set; }

        public string ModalInfoHeader { get; set; }
        public string ModalHeight { get; set; }
        public string ModalWidth { get; set; }

        public string SessionCompany { get; set; }

        public void NavigateToUserProfile()
        {
            RefreshBackGround();
            //set Return URI
            userSession.SetUserProfileReturnURI(navigationManager.Uri);
            navigationManager.NavigateTo("/userprofile");
        }



        private void RefreshBackGround()
        {
            if (!string.IsNullOrEmpty(userSession.TempBackGroundImage))
            {
                userSession.TempBackGroundImage = "";
                userSession.RefreshHome?.Invoke();
            }

        }


        protected override async Task OnInitializedAsync()
        {
            bool gotLock = userSession.Lock;
            while (gotLock)
            {
                await Task.Yield();
                gotLock = userSession.Lock;
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
            parameters.Add("currentUser", userSession.User);

            var options = new ModalOptions()
            {
                Class = "blazored-custom-modal"
            };


            Modal.Show<ModalSystemSelect>("System Select", parameters, options);
        }


    }
}
