using GadjIT_App.Services.SessionState;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GadjIT_App.Pages.Admin.CompanyWorkTypeManagement
{
    public partial class CompanyWorkTypeManagementOverview
    {
        [Inject]
        public IMappingSessionState mappingSessionState { get; set; }

        public bool showMapping { get; set; } = false;

        protected override void OnInitialized()
        {
            showMapping = false;
            mappingSessionState.SetToggleMappingAction(OpenWorkTypeMappingSelectionScreen);

        }

        protected void OpenWorkTypeMappingSelectionScreen()
        {
            mappingSessionState.ToggleMappingScreen();
            showMapping = mappingSessionState.showMapping;

            StateHasChanged();
        }



    }
}
