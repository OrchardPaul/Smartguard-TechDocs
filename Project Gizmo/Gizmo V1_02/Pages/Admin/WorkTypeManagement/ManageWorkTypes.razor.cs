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
        public List<WorkTypeAssignment> groupAssignments { get; set; }

        public WorkTypeGroupItem editGroup = new WorkTypeGroupItem();
        public WorkTypeItem editType = new WorkTypeItem();

        protected override async Task OnInitializedAsync()
        {
            groups = await companyDbAccess.GetWorkTypeGroups();
            if(groups.Count > 0)
            {
                workTypeGroups = groups
                                    .Select(G => new WorkTypeGroupItem
                                    {
                                        group = G,
                                        showWorkType = false
                                    })
                                    .ToList();

                groupAssignments = groups
                            .Select(G => new WorkTypeAssignment
                            {
                                WorkTypeGroup = G,
                                IsAssigned = false
                            })
                            .ToList();
            }

        }

        private async void DataChanged()
        {
            groups = await companyDbAccess.GetWorkTypeGroups();


            if (groups.Count > 0)
            {
                workTypeGroups = groups
                    .Select(G => new WorkTypeGroupItem
                    {
                        group = G,
                        showWorkType = (editGroup.group == G) ? true : false,
                        workTypes = (editGroup.group == G) ? editGroup.workTypes : null
                    })
                    .ToList();

                groupAssignments = groups
                            .Select(G => new WorkTypeAssignment
                            {
                                WorkTypeGroup = G,
                                IsAssigned = false
                            })
                            .ToList();
            }

            StateHasChanged();
        }

        protected void PrepareGroupForEdit(WorkTypeGroupItem seletedGroup)
        {
            editGroup = seletedGroup;
        }

        protected void PrepareTypeForEdit(WorkTypeItem seletedType)
        {
            editType = seletedType;

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

        protected void PrepareTypeForInsert()
        {
            editType = new WorkTypeItem();

            groupAssignments = groups
                                .Select(G => new WorkTypeAssignment
                                {
                                    WorkTypeGroup = G,
                                    IsAssigned = false
                                })
                                .ToList();
        }


        protected async Task<WorkTypeGroupItem> ToggleGroupWorkTypes(WorkTypeGroupItem workTypeGroup)
        {
            workTypeGroup.showWorkType = !workTypeGroup.showWorkType;

            if (workTypeGroup.showWorkType)
            {
                workTypeGroup.workTypes = await companyDbAccess.GetWorkTypesAssignedToGroup(workTypeGroup.group);
            }
                
            return workTypeGroup;
        }
    }
}
