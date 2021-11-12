using GadjIT.AppContext.GadjIT_App;
using GadjIT.AppContext.GadjIT_App.Custom;
using GadjIT_V1_02.Data.Admin;
using GadjIT_V1_02.Services.SessionState;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GadjIT_V1_02.Pages.Admin.CompanyWorkTypeManagement
{
    public partial class CompanyWorkTypeManagementDetails
    {
        [Inject]
        ICompanyDbAccess companyDbAccess { get; set; }

        [Inject]
        NavigationManager NavigationManager { get; set; }

        [Inject]
        IMappingSessionState mappingSessionState { get; set; }

        private List<WorkTypeGroupItem> workTypeGroups { get; set; }
        public List<AppWorkTypeGroups> groups { get; set; }
        public List<WorkTypeGroupItem> refreshedWorkTypeGroups { get; set; }

        public List<WorkTypeAssignment> groupAssignments { get; set; }

        public WorkTypeGroupItem editGroup = new WorkTypeGroupItem();
        public WorkTypeItem editType = new WorkTypeItem();
        public WorkTypeGroupItem selectedGroup;

        public bool blockGroupDisplay = false;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                refreshedWorkTypeGroups = await companyDbAccess.GetGroupsWithWorkTypes();

                if (refreshedWorkTypeGroups.Count > 0)
                {
                    workTypeGroups = refreshedWorkTypeGroups
                                        .Select(G => new WorkTypeGroupItem
                                        {
                                            group = G.group,
                                            workTypes = G.workTypes,
                                            showWorkType = false
                                        })
                                        .ToList();

                    groupAssignments = refreshedWorkTypeGroups
                                .Select(G => new WorkTypeAssignment
                                {
                                    WorkTypeGroup = G.group,
                                    IsAssigned = false
                                })
                                .ToList();
                }
            }
            catch (Exception)
            {
                NavigationManager.NavigateTo($"/", true);
            }


        }

        protected void ToggleGroupWorkTypes(WorkTypeGroupItem workTypeGroup)
        {
            if (blockGroupDisplay)
            {
                blockGroupDisplay = false;
            }
            else
            {
                workTypeGroup.showWorkType = !workTypeGroup.showWorkType;
            }
        }

        protected void OpenWorkTypeMappingSelection(AppWorkTypes selectedWorkType)
        {
            mappingSessionState.SetSelectedWorkType(selectedWorkType);

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
