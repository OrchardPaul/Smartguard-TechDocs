using Gizmo.Context.Gizmo_Authentification;
using Gizmo.Context.Gizmo_Authentification.Custom;
using Gizmo_V1_02.Data.Admin;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gizmo_V1_02.Pages.Admin.WorkTypeManagement
{
    public partial class ManageWorkTypes
    {

        [Inject]
        ICompanyDbAccess companyDbAccess { get; set; }

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

        private async void DataChanged()
        {
            refreshedWorkTypeGroups = await companyDbAccess.GetGroupsWithWorkTypes();

            if (refreshedWorkTypeGroups.Count > 0)
            {
                workTypeGroups = refreshedWorkTypeGroups
                            .Join(workTypeGroups
                                    , RWT => RWT.group.Id
                                    , WT => WT.group.Id
                                    , (RWT, WT) => new { RWT, WT }
                                    )
                            .Select(combined => new WorkTypeGroupItem
                            {
                                group = combined.RWT.group,
                                showWorkType = combined.WT.showWorkType,
                                workTypes = combined.RWT.workTypes
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

            StateHasChanged();
        }

        protected void PrepareGroupForEdit(WorkTypeGroupItem seletedGroup)
        {
            editGroup = seletedGroup;

            blockGroupDisplay = true;
        }

        protected void PrepareTypeForEdit(WorkTypeItem seletedType, WorkTypeGroupItem selectedGroup)
        {
            editType = seletedType;
            this.selectedGroup = selectedGroup;

            var selectedAssignments = seletedType
                                            .assignment
                                            .ToList();

            var selectedAssignmentGroupIds = selectedAssignments
                                                        .Select(A => A.WorkTypeGroupId)
                                                        .ToList();

            if(selectedAssignments.Count > 0)
            {
                groupAssignments = groupAssignments
                                    .Select(G => new WorkTypeAssignment
                                    {
                                        WorkTypeGroup = G.WorkTypeGroup,
                                        IsAssigned = (selectedAssignmentGroupIds.Contains(G.WorkTypeGroup.Id)) ? true : false
                                    }).ToList();
            }

            
        }

        protected void PrepareGroupForInsert()
        {
            editGroup = new WorkTypeGroupItem();
        }

        protected void PrepareTypeForInsert(WorkTypeGroupItem selectedGroup)
        {
            this.selectedGroup = selectedGroup;

            editType = new WorkTypeItem();

            groupAssignments = refreshedWorkTypeGroups
                                .Select(G => new WorkTypeAssignment
                                {
                                    WorkTypeGroup = G.group,
                                    IsAssigned = false
                                })
                                .ToList();
        }


        protected void ToggleGroupWorkTypes(WorkTypeGroupItem workTypeGroup)
        {
            if(blockGroupDisplay)
            {
                blockGroupDisplay = false;
            }
            else
            {
                workTypeGroup.showWorkType = !workTypeGroup.showWorkType;
            }
        }
    }
}
