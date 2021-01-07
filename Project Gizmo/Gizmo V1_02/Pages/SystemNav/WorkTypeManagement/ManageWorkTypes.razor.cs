using Gizmo.Context.Gizmo_Authentification;
using Gizmo.Context.Gizmo_Authentification.Custom;
using Gizmo_V1_02.Data.Admin;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gizmo_V1_02.Pages.SystemNav.WorkTypeManagement
{
    public partial class ManageWorkTypes
    {

        [Inject]
        ICompanyDbAccess companyDbAccess { get; set; }

        [Inject]
        NavigationManager NavigationManager { get; set; }

        private List<VmDepartments> departments { get; set; } = new List<VmDepartments>();
        private List<WorkTypeGroupItem> workTypeGroups { get; set; }
        public List<AppWorkTypes> workTypeItems { get; set; }
        public List<WorkTypeWithDepartment> workTypeWithDepartmentItems { get; set; }
        public List<WorkTypeGroupItem> refreshedWorkTypeGroups { get; set; }
        public List<WorkTypeAssignment> groupAssignments { get; set; }

        public AppDepartments editDepartment = new AppDepartments();
        public WorkTypeGroupItem editGroup = new WorkTypeGroupItem();
        public WorkTypeGroupItem editGrouping = new WorkTypeGroupItem();

        public List<WorkTypeGroupAssignment> editAssignments = new List<WorkTypeGroupAssignment>();
        public AppWorkTypes editType = new AppWorkTypes();
        public WorkTypeGroupItem selectedGroup;

        public string navDisplay = "Department";
        public string showGrouping = "Hide";
        public string groupingDept = "";

        public Action SelectedDeleteAction { get; set; } 

        protected override async Task OnInitializedAsync()
        {
            try
            {
                refreshedWorkTypeGroups = await companyDbAccess.GetGroupsWithWorkTypes();

                if (refreshedWorkTypeGroups.Count > 0)
                {
                    var lstAppDepartments = await companyDbAccess.GetDepartments();
                    departments = lstAppDepartments.Select(A => new VmDepartments { department = A }).ToList();

                    departments = departments.OrderBy(D => D.department.DepartmentName).ToList();

                    workTypeGroups = refreshedWorkTypeGroups
                                        .Select(G => new WorkTypeGroupItem
                                        {
                                            group = G.group,
                                            department =
                                                        (departments
                                                                .Where(D => D.department.Id == G.group.parentId)
                                                                .SingleOrDefault() is null)
                                                                ?
                                                                new AppDepartments { DepartmentName = "" }
                                                                :
                                                                departments
                                                                .Where(D => D.department.Id == G.group.parentId)
                                                                .Select(D => D.department)
                                                                .SingleOrDefault(),
                                            workTypes = G.workTypes,
                                            showWorkType = false
                                        })
                                        .OrderBy(C => C.group.GroupName)
                                        .OrderBy(C => C.department.DepartmentName)
                                        .ToList();

                    groupAssignments = refreshedWorkTypeGroups
                                .Select(G => new WorkTypeAssignment
                                {
                                    WorkTypeGroup = G.group,
                                    IsAssigned = false
                                })
                                .ToList();

                    workTypeItems = await companyDbAccess.GetWorkTypes();

                    workTypeWithDepartmentItems = workTypeItems
                                                        .Select(W => new WorkTypeWithDepartment
                                                        {
                                                            workType = W,
                                                            department = (departments
                                                                                                .Where(D => D.department.Id == W.DepartmentId)
                                                                                                .SingleOrDefault()
                                                                                                is null)

                                                                            ? new AppDepartments { DepartmentName = "" }

                                                                            : departments
                                                                                    .Where(D => D.department.Id == W.DepartmentId)
                                                                                    .Select(D => D.department)
                                                                                    .SingleOrDefault()
                                                        })
                                                        .OrderBy(C => C.workType.TypeName)
                                                        .OrderBy(C => C.department.DepartmentName)
                                                        .ToList();

                    editAssignments = workTypeItems
                                            .Select(W => new WorkTypeGroupAssignment
                                            {
                                                WorkType = W,
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

        private async void DataChanged()
        {
            refreshedWorkTypeGroups = await companyDbAccess.GetGroupsWithWorkTypes();

            if (refreshedWorkTypeGroups.Count > 0)
            {
                var lstAppDepartments = await companyDbAccess.GetDepartments();
                departments = lstAppDepartments.Select(A => new VmDepartments { department = A }).ToList();

                departments = departments.OrderBy(D => D.department.DepartmentName).ToList();

                workTypeGroups = refreshedWorkTypeGroups
                            .Select(RWT => new WorkTypeGroupItem
                            {
                                group = RWT.group,
                                department =
                                        (departments
                                                .Where(D => D.department.Id == RWT.group.parentId)
                                                .SingleOrDefault() is null)
                                                ?
                                                new AppDepartments { DepartmentName = "" }
                                                :
                                                departments
                                                .Where(D => D.department.Id == RWT.group.parentId)
                                                .Select(D => D.department)
                                                .SingleOrDefault(),

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
                            .OrderBy(C => C.group.GroupName)
                            .OrderBy(C => C.department.DepartmentName)
                            .ToList();



                groupAssignments = refreshedWorkTypeGroups
                            .Select(G => new WorkTypeAssignment
                            {
                                WorkTypeGroup = G.group,
                                IsAssigned = false
                            })
                            .ToList();

                workTypeItems = await companyDbAccess.GetWorkTypes();
                workTypeWithDepartmentItems = workTypeItems
                                                    .Select(W => new WorkTypeWithDepartment
                                                    {
                                                        workType = W,
                                                        department = (departments
                                                                                            .Where(D => D.department.Id == W.DepartmentId)
                                                                                            .SingleOrDefault()
                                                                                            is null)

                                                                        ? new AppDepartments { DepartmentName = "" }

                                                                        : departments
                                                                                .Where(D => D.department.Id == W.DepartmentId)
                                                                                .Select(D => D.department)
                                                                                .SingleOrDefault()
                                                    })
                                                    .OrderBy(C => C.workType.TypeName)
                                                    .OrderBy(C => C.department.DepartmentName)
                                                    .ToList();


                editAssignments = workTypeItems
                                .Select(W => new WorkTypeGroupAssignment
                                {
                                    WorkType = W,
                                    IsAssigned = false
                                })
                                .ToList();
            }

            StateHasChanged();
        }

        protected void PrepareDepartmentForEdit(AppDepartments selectedDepartment)
        {
            editDepartment = selectedDepartment;
        }

        protected void PrepareDepartmentForDelete(AppDepartments selectedDepartment)
        {
            editDepartment = selectedDepartment;
            SelectedDeleteAction = HandleDepartmentDelete;
        }

        protected void PrepareGroupForEdit(WorkTypeGroupItem seletedGroup)
        {
            editGroup = seletedGroup;
        }

        protected void PrepareGroupForDelete(WorkTypeGroupItem seletedGroup)
        {
            editGroup = seletedGroup;
            SelectedDeleteAction = HandleGroupDelete;
        }

        protected void PrepareTypeForEdit(AppWorkTypes seletedType)
        {
            editType = seletedType;
        }

        protected void PrepareTypeForDelete(AppWorkTypes seletedType)
        {
            editType = seletedType;
            SelectedDeleteAction = HandleWorkTypeDelete;
        }

        protected void PrepareDepartmentForInsert()
        {
            editDepartment = new AppDepartments();
        }

        protected void PrepareGroupForInsert()
        {
            editGroup = new WorkTypeGroupItem();
        }

        protected void PrepareTypeForInsert()
        {
            editType = new AppWorkTypes();
        }

        protected void PrepareGroupingForEdit(WorkTypeGroupItem selectedGroup)
        {
            var workTypeIds = selectedGroup
            .workTypes
            .Select(W => W.workType.Id)
            .ToList();

            editGrouping = selectedGroup;

            editAssignments = workTypeItems
           .Select(W => new WorkTypeGroupAssignment
           {
               WorkType = W,
               IsAssigned = workTypeIds.Contains(W.Id) ? true : false
           })
           .ToList();

        }


        protected void ShowNav(string displayChange)
        {
            navDisplay = displayChange;
            ShowGroupingDept("Hide", "");
        }

        protected void ShowGroupingDept(string displayChange, string displayDept)
        {
            showGrouping = displayChange;
            groupingDept = displayDept;
        }

        private async void HandleDepartmentDelete()
        {
            await companyDbAccess.DeleteDepartment(editDepartment);

            DataChanged();
        }

        private async void HandleWorkTypeDelete()
        {
            await companyDbAccess.DeleteWorkType(editType);

            DataChanged();
        }

        private async void HandleGroupDelete()
        {
            await companyDbAccess.DeleteWorkTypeGroup(editGroup.group);

            DataChanged();
        }

    }
}
