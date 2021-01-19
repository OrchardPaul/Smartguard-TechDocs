using GadjIT.GadjitContext.GadjIT_App;
using GadjIT.GadjitContext.GadjIT_App.Custom;
using GadjIT.ClientContext.OR_RESI;
using Gizmo_V1_02.Data.Admin;
using Gizmo_V1_02.Services;
using Gizmo_V1_02.Services.SessionState;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gizmo_V1_02.Pages.Admin.CompanyWorkTypeManagement
{
    public partial class CompanyWorkTypeManagementMapping
    {
        [Inject]
        private IPartnerAccessService apiConnection { get; set; }

        [Inject]
        private IUserSessionState sessionState { get; set; }

        [Inject]
        private ICompanyDbAccess dbAccess { get; set; }

        [Inject]
        private IMappingSessionState mappingSessionState { get; set; }

        private WorkTypeMapping workTypeMapping { get; set; } = new WorkTypeMapping();

        private AppWorkTypes selectedWorkType { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await setWorkTypeMapping();
        }

        public async Task<WorkTypeMapping> setWorkTypeMapping()
        {
            workTypeMapping.workType = mappingSessionState.selectedWorkType;

            var allCaseTypes = await apiConnection.GetPartnerCaseTypes();
            workTypeMapping = await dbAccess.GetWorkTypeMappingsByCompany(sessionState.Company
                                                                               , allCaseTypes
                                                                               , workTypeMapping.workType
                                                                               , sessionState.selectedSystem);

            return workTypeMapping;
        }

        protected async Task<WorkTypeMapping> UpdateMapping(CaseTypeAssignment selectedAssignment)
        {
            selectedAssignment.IsAssigned = !selectedAssignment.IsAssigned;

            await dbAccess.UpdateWorkTypeMapping(workTypeMapping
                                                    , sessionState.Company
                                                    , sessionState.selectedSystem);
            return workTypeMapping;
        }

        protected void OpenWorkTypeMappingOverview()
        {
            var action = mappingSessionState.ToggleMapping;
            action?.Invoke();
        }

        protected void ReturnToDetailsScreen()
        {
            var action = mappingSessionState.ToggleMappingOverviewScreen;
            action?.Invoke();
        }


    }
}
