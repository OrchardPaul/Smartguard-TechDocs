using Gizmo_V1_02.Services.SessionState;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gizmo_V1_02.Pages.Admin.CompanyWorkTypeManagement
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
