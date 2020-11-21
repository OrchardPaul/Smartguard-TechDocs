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

        [Inject]
        NavigationManager NavigationManager { get; set; }

        private List<WorkTypeGroupItem> workTypeGroups { get; set; }
        public List<AppWorkTypeGroups> groups { get; set; }
        public List<AppWorkTypes> workTypeItems { get; set; }

        public List<WorkTypeGroupItem> refreshedWorkTypeGroups { get; set; }

        public List<WorkTypeAssignment> groupAssignments { get; set; }

        public WorkTypeGroupItem editGroup = new WorkTypeGroupItem();
        public AppWorkTypes editType = new AppWorkTypes();
        public WorkTypeGroupItem selectedGroup;

        public bool blockGroupDisplay = false;

        public string navDisplay = "Group";

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

                    workTypeItems = await companyDbAccess.GetWorkTypes();

                }
            }
            catch(Exception)
            {
                NavigationManager.NavigateTo($"/", true);
            }


        }

        private async void DataChanged()
        {
            refreshedWorkTypeGroups = await companyDbAccess.GetGroupsWithWorkTypes();

            if (refreshedWorkTypeGroups.Count > 0)
            {
                workTypeGroups = refreshedWorkTypeGroups
                            .Select(RWT => new WorkTypeGroupItem
                            {
                                group = RWT.group,
                                showWorkType = (workTypeGroups
                                        .Where(WT => RWT.group.Id == WT.group.Id)
                                        .SingleOrDefault() is null) 
                                                ? false 
                                                : workTypeGroups
                                                    .Where(WT => RWT.group.Id == WT.group.Id)
                                                    .SingleOrDefault()
                                                    .showWorkType,
                                workTypes = RWT.workTypes
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

        //protected void PrepareTypeForEdit(WorkTypeItem seletedType, WorkTypeGroupItem selectedGroup)
        //{
        //    editType = seletedType;
        //    this.selectedGroup = selectedGroup;

        //    var selectedAssignments = seletedType
        //                                    .assignment
        //                                    .ToList();

        //    var selectedAssignmentGroupIds = selectedAssignments
        //                                                .Select(A => A.WorkTypeGroupId)
        //                                                .ToList();

        //    if(selectedAssignments.Count > 0)
        //    {
        //        groupAssignments = groupAssignments
        //                            .Select(G => new WorkTypeAssignment
        //                            {
        //                                WorkTypeGroup = G.WorkTypeGroup,
        //                                IsAssigned = (selectedAssignmentGroupIds.Contains(G.WorkTypeGroup.Id)) ? true : false
        //                            }).ToList();
        //    }


        //}

        protected void PrepareTypeForEdit(AppWorkTypes seletedType)
        {
            editType = seletedType;

            blockGroupDisplay = true;

        }

        protected void PrepareGroupForInsert()
        {
            editGroup = new WorkTypeGroupItem();
        }

        protected void PrepareTypeForInsert()
        {
            editType = new AppWorkTypes();
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

        protected void ShowNav(string displayChange)
        {
            navDisplay = displayChange;
        }
    }
}
