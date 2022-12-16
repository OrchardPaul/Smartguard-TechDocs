using GadjIT_AppContext.GadjIT_App;
using GadjIT_AppContext.GadjIT_App.Custom;
using GadjIT_ClientContext.P4W;
using GadjIT_App.Data.Admin;
using GadjIT_App.Services;
using GadjIT_App.Services.SessionState;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GadjIT_App.Pages.Admin.CompanyWorkTypeManagement
{
    public partial class CompanyWorkTypeManagementMapping
    {
        [Inject]
        private IPartnerAccessService ApiConnection { get; set; }

        [Inject]
        private IUserSessionState SessionState { get; set; }

        [Inject]
        private ICompanyDbAccess DbAccess { get; set; }

        [Inject]
        private IMappingSessionState MappingSessionState { get; set; }

        private WorkTypeMapping WorkTypeMapping { get; set; } = new WorkTypeMapping();

        private AppWorkTypes SelectedWorkType { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await setWorkTypeMapping();
        }

        public async Task<WorkTypeMapping> setWorkTypeMapping()
        {
            WorkTypeMapping.workType = MappingSessionState.selectedWorkType;

            var allCaseTypes = await ApiConnection.GetPartnerCaseTypes();
            WorkTypeMapping = await DbAccess.GetWorkTypeMappingsByCompany(SessionState.Company
                                                                               , allCaseTypes
                                                                               , WorkTypeMapping.workType
                                                                               , SessionState.SelectedSystem);

            return WorkTypeMapping;
        }

        protected async Task<WorkTypeMapping> UpdateMapping(CaseTypeAssignment selectedAssignment)
        {
            selectedAssignment.IsAssigned = !selectedAssignment.IsAssigned;

            await DbAccess.UpdateWorkTypeMapping(WorkTypeMapping
                                                    , SessionState.Company
                                                    , SessionState.SelectedSystem);
            return WorkTypeMapping;
        }

        protected void OpenWorkTypeMappingOverview()
        {
            var action = MappingSessionState.ToggleMapping;
            action?.Invoke();
        }

        protected void ReturnToDetailsScreen()
        {
            var action = MappingSessionState.ToggleMappingOverviewScreen;
            action?.Invoke();
        }


    }
}
