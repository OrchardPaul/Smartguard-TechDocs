using GadjIT_App.Services.SessionState;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GadjIT_App.Pages.Admin.CompanyDetails
{
    public partial class CompanyDetails
    {
        [Inject]
        public IMappingSessionState mappingSessionState { get; set; }

        public bool showMappingPage { get; set; } = false;

        protected override void OnInitialized()
        {
            mappingSessionState.SetToggleMappingOverviewAction(ToggleMappingPage);
        }


        public void ToggleMappingPage()
        {
            showMappingPage = !showMappingPage;
            StateHasChanged();
        }
    }
}
