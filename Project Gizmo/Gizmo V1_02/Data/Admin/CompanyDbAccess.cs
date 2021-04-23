using AutoMapper;
using GadjIT.GadjitContext.GadjIT_App;
using GadjIT.GadjitContext.GadjIT_App.Custom;
using GadjIT.ClientContext.P4W;
using Gizmo_V1_02.Services;
using Gizmo_V1_02.Services.SessionState;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using GadjIT.ClientContext.P4W.Custom;
using Newtonsoft.Json;

namespace Gizmo_V1_02.Data.Admin
{
    public interface ICompanyDbAccess
    {
        Task<List<AppDepartments>> GetDepartments();
        Task<List<AppWorkTypeGroups>> GetWorkTypeGroups();
        Task<List<AppWorkTypes>> GetWorkTypes();
        Task<WorkTypeMapping> GetWorkTypeMappingsByCompany(AppCompanyDetails company, List<CaseTypes> allCaseTypes, AppWorkTypes workType, string system);
        Task<List<AppCompanyWorkTypeMapping>> UpdateWorkTypeMapping(WorkTypeMapping typeMapping, AppCompanyDetails company, string system);
        Task<List<WorkTypeGroupItem>> GetGroupsWithWorkTypes();
        Task<List<AppWorkTypeGroups>> GetWorkTypeGroupsByCompany(int companyId);
        Task<WorkTypeGroupItem> AssignWorkTypeToGroup(WorkTypeGroupItem workTypeGroup, List<WorkTypeGroupAssignment> assignments);
        Task<AppDepartments> SubmitDepartment(AppDepartments department);
        Task<AppWorkTypeGroups> SubmitWorkTypeGroup(AppWorkTypeGroups workTypeGroup);
        Task<AppWorkTypes> SubmitWorkType(AppWorkTypes workType);
        Task<AppDepartments> DeleteDepartment(AppDepartments department);
        Task<AppWorkTypeGroups> DeleteWorkTypeGroup(AppWorkTypeGroups group);
        Task<AppWorkTypes> DeleteWorkType(AppWorkTypes workType);
        Task<AppWorkTypeGroupsTypeAssignments> DeleteAssignment(AppWorkTypeGroupsTypeAssignments assignment);



        Task<List<AppCompanyDetails>> GetCompanies();
        Task<AppCompanyDetails> GetCompanyById(int id);
        Task<AppCompanyDetails> GetSelectedCompanyOfUser(AspNetUsers user);
        Task<string> GetCompanyBaseUri(int id, string selectedUri);
        Task<AppCompanyDetails> SubmitChanges(AppCompanyDetails company);
        Task<AppCompanyDetails> DeleteCompany(AppCompanyDetails company);
        Task<AppCompanyDetails> AssignWorkTypeGroupToCompany(AppCompanyDetails company, AppWorkTypeGroups workTypeGroup);
        Task<AppCompanyDetails> RemoveWorkTypeGroupFromCompany(AppCompanyDetails company, AppWorkTypeGroups workTypeGroup);
        Task<SmartflowRecords> SaveSmartFlowRecord(UsrOrDefChapterManagement chapter, IUserSessionState sessionState);
        Task<SmartflowRecords> SaveSmartFlowRecordData(UsrOrDefChapterManagement chapter, IUserSessionState sessionState);
        Task<SmartflowRecords> RemoveSmartFlowRecord(int id, IUserSessionState sessionState);
        Task<List<SmartflowRecords>> SyncAdminSysToClient(List<UsrOrDefChapterManagement> clientObjects, IUserSessionState sessionState);
        Task<List<SmartflowRecords>> GetAllSmartflowRecords(IUserSessionState sessionState);
    }

    public class CompanyDbAccess : ICompanyDbAccess
    {
        private readonly AuthorisationDBContext context;
        private readonly AuthenticationStateProvider authenticationStateProvider;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IIdentityUserAccess identityUserAccess;
        private readonly IMapper mapper;

        public CompanyDbAccess(AuthorisationDBContext context
                                , AuthenticationStateProvider authenticationStateProvider
                                , UserManager<ApplicationUser> userManager
                                , IIdentityUserAccess identityUserAccess
                                , IMapper mapper)
        {
            this.context = context;
            this.authenticationStateProvider = authenticationStateProvider;
            this.userManager = userManager;
            this.identityUserAccess = identityUserAccess;
            this.mapper = mapper;
        }

        /*
         * 
         * Work Types
         * 
         */

        public async Task<List<AppDepartments>> GetDepartments()
        {
            return await context.AppDepartments.ToListAsync();
        }

        public async Task<List<AppWorkTypeGroups>> GetWorkTypeGroups()
        {
            return await context.AppWorkTypeGroups.ToListAsync();
        }

        public async Task<List<AppWorkTypes>> GetWorkTypes()
        {
            return await context.AppWorkTypes.ToListAsync();
        }

        public async Task<WorkTypeMapping> GetWorkTypeMappingsByCompany(AppCompanyDetails company
                                                                                , List<CaseTypes> allCaseTypes
                                                                                , AppWorkTypes workType
                                                                                , string system)
        {

            var caseTypeAssignments = allCaseTypes
                                            .Select(C => new CaseTypeAssignment
                                            {
                                                CaseType = C
                                                ,
                                                IsAssigned = context.AppCompanyWorkTypeMapping
                                                                .Where(M => M.WorkTypeId == workType.Id)
                                                                .Where(M => M.CaseTypeCode == C.Code)
                                                                .Where(M => M.CompanyId == company.Id)
                                                                .Where(M => M.System == system)
                                                                .SingleOrDefault() != null
                                            })
                                            .ToList();


            return await context.AppWorkTypes
                .Where(T => T.Id == workType.Id)
                .Select(T => new WorkTypeMapping
                {
                    workType = T
                    ,
                    caseTypeAssignments = caseTypeAssignments
                })
                .SingleOrDefaultAsync();
        }

        public async Task<List<AppCompanyWorkTypeMapping>> UpdateWorkTypeMapping(WorkTypeMapping typeMapping
                                                                                , AppCompanyDetails company
                                                                                , string system)
        {
            var addMappings = typeMapping.caseTypeAssignments
                                .Where(C => C.IsAssigned)
                                .Select(C => new AppCompanyWorkTypeMapping
                                {
                                    WorkTypeId = typeMapping.workType.Id
                                    ,
                                    CaseTypeCode = C.CaseType.Code
                                    ,
                                    CompanyId = company.Id
                                    ,
                                    System = system
                                })
                                .ToList();



            if (addMappings.Count > 0)
            {
                addMappings = addMappings
                                .Where(A => A.CaseTypeCode != context.AppCompanyWorkTypeMapping
                                                                .Where(M => M.WorkTypeId == A.WorkTypeId)
                                                                .Where(M => M.CaseTypeCode == A.CaseTypeCode)
                                                                .Where(M => M.CompanyId == A.CompanyId)
                                                                .Where(M => M.System == A.System)
                                                                .Select(M => M.CaseTypeCode)
                                                                .SingleOrDefault())
                                .ToList();

                context.AppCompanyWorkTypeMapping.AddRange(addMappings);
                await context.SaveChangesAsync();
            }

            var removeMappings = typeMapping.caseTypeAssignments
                                .Where(C => !C.IsAssigned)
                                .Select(C => new AppCompanyWorkTypeMapping
                                {
                                    WorkTypeId = typeMapping.workType.Id
                                    ,
                                    CaseTypeCode = C.CaseType.Code
                                    ,
                                    CompanyId = company.Id
                                    ,
                                    System = system
                                })
                                .ToList();

            if (removeMappings.Count > 0)
            {
                removeMappings = removeMappings
                                .Select(A => context.AppCompanyWorkTypeMapping
                                                                                .Where(M => M.WorkTypeId == A.WorkTypeId)
                                                                                .Where(M => M.CaseTypeCode == A.CaseTypeCode)
                                                                                .Where(M => M.CompanyId == A.CompanyId)
                                                                                .Where(M => M.System == A.System)
                                                                                .SingleOrDefault())
                                .Where(A => !(A is null))
                                .ToList();

                context.AppCompanyWorkTypeMapping.RemoveRange(removeMappings);
                await context.SaveChangesAsync();
            }

            addMappings.AddRange(removeMappings);
            return addMappings;

        }

        public async Task<List<WorkTypeGroupItem>> GetGroupsWithWorkTypes()
        {
            var cp = await context.AppCompanyUserRoles.ToListAsync();

            return await context.AppWorkTypeGroups
                                .Select(G => new WorkTypeGroupItem
                                {
                                    group = G,
                                    workTypes = context.AppWorkTypes
                                                            .Join(context.AppWorkTypeGroupsTypeAssignments,
                                                                workType => workType.Id,
                                                                assignment => assignment.WorkTypeId,
                                                                (workType, assignment) => new { workType, assignment })
                                                            .Where(combined => combined.assignment.WorkTypeGroupId == G.Id)
                                                            .Select(combined => new WorkTypeItem
                                                            {
                                                                workType = combined.workType,
                                                                assignment = context.AppWorkTypeGroupsTypeAssignments
                                                                                    .Where(A => A.WorkTypeId == combined.assignment.WorkTypeId)
                                                                                    .ToList()
                                                            })
                                                            .ToList()
                                })
                                .ToListAsync();
        }

        public async Task<List<AppWorkTypeGroups>> GetWorkTypeGroupsByCompany(int companyId)
        {
            var assignedWorkTypeGroups = await context.AppCompanyWorkTypeGroups
                                                    .Where(A => A.CompanyId == companyId)
                                                    .Select(A => A.WorkTypeGroupId)
                                                    .ToListAsync();

            return await context.AppWorkTypeGroups
                .Where(A => assignedWorkTypeGroups.Contains(A.Id))
                .ToListAsync();
        }

        public async Task<WorkTypeGroupItem> AssignWorkTypeToGroup(WorkTypeGroupItem workTypeGroup, List<WorkTypeGroupAssignment> assignments)
        {
            bool changed = false;

            var AssignmentsToRemove = await context
                                                .AppWorkTypeGroupsTypeAssignments
                                                .Where(A => A.WorkTypeGroupId == workTypeGroup.group.Id)
                                                .ToListAsync();

            if (AssignmentsToRemove.Count > 0)
            {
                context.AppWorkTypeGroupsTypeAssignments.RemoveRange(AssignmentsToRemove);

                changed = true;
            }


            if (assignments
                    .Where(A => A.IsAssigned)
                    .ToList()
                    .Count > 0)
            {
                var newAssignments = assignments
                                        .Where(A => A.IsAssigned)
                                        .Select(A => new AppWorkTypeGroupsTypeAssignments
                                        {
                                            WorkTypeGroupId = workTypeGroup.group.Id,
                                            WorkTypeId = A.WorkType.Id
                                        })
                                        .ToList();

                context.AppWorkTypeGroupsTypeAssignments.AddRange(newAssignments);

                changed = true;
            }

            if (changed)
            {
                await context.SaveChangesAsync();
            }

            return workTypeGroup;

        }



        public async Task<AppDepartments> SubmitDepartment(AppDepartments department)
        {
            var selectedDepartment = await context.AppDepartments.SingleOrDefaultAsync(A => A.Id == department.Id);

            if (selectedDepartment is null)
            {
                context.AppDepartments.Add(department);
                await context.SaveChangesAsync();
                return department;
            }
            else
            {
                selectedDepartment = department;
                await context.SaveChangesAsync();
                return selectedDepartment;
            }
        }

        public async Task<AppWorkTypeGroups> SubmitWorkTypeGroup(AppWorkTypeGroups workTypeGroup)
        {
            var selectedWorkTypeGroup = await context.AppWorkTypeGroups.SingleOrDefaultAsync(A => A.Id == workTypeGroup.Id);

            if (selectedWorkTypeGroup is null)
            {
                context.AppWorkTypeGroups.Add(workTypeGroup);
                await context.SaveChangesAsync();
                return selectedWorkTypeGroup;
            }
            else
            {
                selectedWorkTypeGroup = workTypeGroup;
                await context.SaveChangesAsync();
                return selectedWorkTypeGroup;
            }
        }

        public async Task<AppWorkTypes> SubmitWorkType(AppWorkTypes workType)
        {
            var selectedWorkType = await context.AppWorkTypes.SingleOrDefaultAsync(A => A.Id == workType.Id);
            var selectedWorkTypeWithNoTracking = await context.AppWorkTypes
                                                                .AsNoTracking()
                                                                .SingleOrDefaultAsync(A => A.Id == workType.Id);

            if (selectedWorkType is null)
            {
                context.AppWorkTypes.Add(workType);
                await context.SaveChangesAsync();
                return workType;
            }
            else
            {
                if (selectedWorkTypeWithNoTracking.DepartmentId != workType.DepartmentId)
                {
                    var assignedGroups = await context
                                                .AppWorkTypeGroupsTypeAssignments
                                                .Where(A => A.WorkTypeId == selectedWorkTypeWithNoTracking.Id)
                                                .Join(context.AppWorkTypeGroups,
                                                            A => A.WorkTypeGroupId,
                                                            AWG => AWG.Id,
                                                            (A, AWG) => new { A, AWG })
                                                .Where(C => C.AWG.ParentId == selectedWorkTypeWithNoTracking.DepartmentId)
                                                .Select(C => C.A)
                                                .ToListAsync();

                    if (assignedGroups.Count() > 0)
                    {
                        context.AppWorkTypeGroupsTypeAssignments.RemoveRange(assignedGroups);
                    }
                }





                selectedWorkType = workType;
                await context.SaveChangesAsync();
                return selectedWorkType;
            }
        }

        public async Task<AppDepartments> DeleteDepartment(AppDepartments department)
        {
            var selectedDepartment = await context.AppDepartments.SingleOrDefaultAsync(C => C.Id == department.Id);

            if (!(selectedDepartment is null))
            {
                var groups = await context.AppWorkTypeGroups
                                        .Where(A => A.ParentId == department.Id)
                                        .ToListAsync();

                if (!(groups.Count == 0))
                {
                    groups = groups
                                .Select(g => {
                                    g.ParentId = 0;
                                    return g;
                                })
                                .ToList();

                    context.AppWorkTypeGroups.UpdateRange(groups);
                }

                var workTypes = await context.AppWorkTypes
                        .Where(A => A.DepartmentId == department.Id)
                        .ToListAsync();

                if (!(workTypes.Count == 0))
                {
                    workTypes = workTypes
                                .Select(d => {
                                    d.DepartmentId = 0;
                                    return d;
                                })
                                .ToList();

                    context.AppWorkTypes.UpdateRange(workTypes);
                }

                context.AppDepartments.Remove(selectedDepartment);
                await context.SaveChangesAsync();
                return selectedDepartment;
            }
            else
            {
                return department;
            }
        }

        public async Task<AppWorkTypeGroups> DeleteWorkTypeGroup(AppWorkTypeGroups group)
        {
            var selectedGroup = await context.AppWorkTypeGroups.SingleOrDefaultAsync(C => C.Id == group.Id);

            var groupAssignments = await context.AppWorkTypeGroupsTypeAssignments
                .Where(A => A.WorkTypeGroupId == group.Id)
                .ToListAsync();

            if (!(groupAssignments is null))
                context.AppWorkTypeGroupsTypeAssignments.RemoveRange(groupAssignments);

            context.AppWorkTypeGroups.Remove(selectedGroup);
            await context.SaveChangesAsync();
            return selectedGroup;
        }

        public async Task<AppWorkTypes> DeleteWorkType(AppWorkTypes workType)
        {
            var selectedType = await context.AppWorkTypes.SingleOrDefaultAsync(C => C.Id == workType.Id);

            var TypeAssignments = await context.AppWorkTypeGroupsTypeAssignments
                .Where(A => A.WorkTypeId == selectedType.Id)
                .ToListAsync();

            if (!(TypeAssignments is null))
                context.AppWorkTypeGroupsTypeAssignments.RemoveRange(TypeAssignments);

            context.AppWorkTypes.Remove(selectedType);
            await context.SaveChangesAsync();
            return selectedType;
        }

        public async Task<AppWorkTypeGroupsTypeAssignments> DeleteAssignment(AppWorkTypeGroupsTypeAssignments assignment)
        {
            var selectedAssignment = await context.AppWorkTypeGroupsTypeAssignments
                                                  .SingleOrDefaultAsync(C => C.Id == assignment.Id);

            context.AppWorkTypeGroupsTypeAssignments.Remove(selectedAssignment);
            await context.SaveChangesAsync();

            return selectedAssignment;
        }


        /*
         * 
         * Companies
         * 
         */

        public async Task<List<AppCompanyDetails>> GetCompanies()
        {
            var signedInUserState = await authenticationStateProvider.GetAuthenticationStateAsync();
            var signedInUserAsp = await userManager.FindByNameAsync(signedInUserState.User.Identity.Name);
            var signedInUser = mapper.Map(signedInUserAsp, new AspNetUsers());

            if (signedInUserState.User.IsInRole("Super User"))
            {
                return await context.AppCompanyDetails.ToListAsync();
            }
            else
            {
                var signedInUsersCompany = await identityUserAccess.GetCompanyClaims(signedInUser);
                var signedInUsersCompanyIds = signedInUsersCompany.Select(C => C.Value).ToList();

                return await context.AppCompanyDetails
                                    .Where(A => signedInUsersCompanyIds.Contains(A.Id.ToString()))
                                    .ToListAsync();
            }

        }


        public async Task<AppCompanyDetails> GetCompanyById(int id)
        {
            return await context.AppCompanyDetails.SingleAsync(C => C.Id == id);
        }

        public async Task<AppCompanyDetails> GetSelectedCompanyOfUser(AspNetUsers user)
        {
            return await context.AppCompanyDetails.SingleOrDefaultAsync(C => C.Id == user.SelectedCompanyId);
        }

        public async Task<string> GetCompanyBaseUri(int id, string selectedServer)
        {
            var selectedCompany = await context.AppCompanyDetails.SingleOrDefaultAsync(C => C.Id == id);
            var selectedUri = "";

            if (!(selectedCompany is null))
            {
                selectedUri = (selectedServer == "Live") ? selectedCompany.LiveUri : selectedCompany.DevUri;
                if (!selectedUri.EndsWith("/"))
                {
                    selectedUri = selectedUri + "/";
                }
            }

            return (selectedCompany is null) ? null : selectedUri;
        }

        public async Task<AppCompanyDetails> SubmitChanges(AppCompanyDetails company)
        {
            var selectedCompany = await context.AppCompanyDetails.SingleOrDefaultAsync(C => C.Id == company.Id);

            if (selectedCompany is null)
            {
                context.AppCompanyDetails.Add(company);
                await context.SaveChangesAsync();
                return company;
            }
            else
            {
                selectedCompany.CompanyName = company.CompanyName;
                selectedCompany.DevUri = company.DevUri;
                selectedCompany.LiveUri = company.LiveUri;
                selectedCompany.CompCol1 = company.CompCol1;
                selectedCompany.CompCol2 = company.CompCol2;
                selectedCompany.CompCol3 = company.CompCol3;
                selectedCompany.CompCol4 = company.CompCol4;

                await context.SaveChangesAsync();
                return selectedCompany;
            }
        }

        public async Task<AppCompanyDetails> AssignWorkTypeGroupToCompany(AppCompanyDetails company, AppWorkTypeGroups workTypeGroup)
        {
            var selectedCompany = await context.AppCompanyDetails.SingleOrDefaultAsync(C => C.Id == company.Id);

            if (!(selectedCompany is null))
            {
                var newAssignment = new AppCompanyWorkTypeGroups
                {
                    CompanyId = selectedCompany.Id
                                                                    ,
                    WorkTypeGroupId = workTypeGroup.Id
                };

                var existingAssignment = await context.AppCompanyWorkTypeGroups
                                                    .Where(A => A.CompanyId == newAssignment.CompanyId)
                                                    .Where(A => A.WorkTypeGroupId == newAssignment.WorkTypeGroupId)
                                                    .SingleOrDefaultAsync();

                if (existingAssignment is null)
                {
                    context.AppCompanyWorkTypeGroups.Add(newAssignment);
                    await context.SaveChangesAsync();
                }

                return selectedCompany;
            }
            else
            {
                return selectedCompany;
            }
        }

        public async Task<AppCompanyDetails> RemoveWorkTypeGroupFromCompany(AppCompanyDetails company, AppWorkTypeGroups workTypeGroup)
        {
            var selectedCompany = await context.AppCompanyDetails.SingleOrDefaultAsync(C => C.Id == company.Id);

            if (!(selectedCompany is null))
            {

                var existingAssignment = await context.AppCompanyWorkTypeGroups
                                                    .Where(A => A.CompanyId == selectedCompany.Id)
                                                    .Where(A => A.WorkTypeGroupId == workTypeGroup.Id)
                                                    .SingleOrDefaultAsync();

                if (!(existingAssignment is null))
                {
                    context.AppCompanyWorkTypeGroups.Remove(existingAssignment);
                    await context.SaveChangesAsync();
                }

                return selectedCompany;
            }
            else
            {
                return selectedCompany;
            }
        }


        public async Task<AppCompanyDetails> DeleteCompany(AppCompanyDetails company)
        {
            var selectedCompany = await context.AppCompanyDetails.SingleOrDefaultAsync(C => C.Id == company.Id);

            context.AppCompanyDetails.Remove(selectedCompany);
            await context.SaveChangesAsync();

            return selectedCompany;
        }


        public async Task<List<SmartflowRecords>> SyncAdminSysToClient(List<UsrOrDefChapterManagement> clientObjects, IUserSessionState sessionState)
        {
            var currentRecords = await context
                                            .SmartflowRecords
                                            .Where(R => R.CompanyId == sessionState.Company.Id)
                                            .Where(R => R.System == sessionState.selectedSystem)
                                            .Where(R => clientObjects
                                                            .Select(C => C.Id)
                                                            .ToList()
                                                            .Contains(R.RowId))
                                            .ToListAsync();


            //loop though each object contained in the list
            foreach(var clientRow in clientObjects
                                        .Where(O => currentRecords
                                                        .Select(R => R.RowId)
                                                        .ToList()
                                                        .Contains(O.Id))
                                        .ToList())
            {
                var record = currentRecords.Where(R => R.RowId == clientRow.Id).SingleOrDefault();
                mapper.Map(clientRow, record);

                record.LastModifiedByUserId = sessionState.User.Id;
                record.LastModifiedDate = DateTime.Now;                

                await context.SaveChangesAsync();
            }

            foreach (var clientRow in clientObjects
                                        .Where(O => !currentRecords
                                                        .Select(R => R.RowId)
                                                        .ToList()
                                                        .Contains(O.Id))
                                        .ToList())
            {
                var record = new SmartflowRecords { CompanyId = sessionState.Company.Id };
                mapper.Map(clientRow, record);

                record.System = sessionState.selectedSystem;
                record.CreatedByUserId = sessionState.User.Id;
                record.CreatedDate = DateTime.Now;
                record.LastModifiedByUserId = sessionState.User.Id;
                record.LastModifiedDate = DateTime.Now;

                context.SmartflowRecords.Add(record);

                await context.SaveChangesAsync();
            }

            return currentRecords;
        }


        public async Task<List<SmartflowRecords>> GetAllSmartflowRecords(IUserSessionState sessionState)
        {
            return await context.SmartflowRecords.Where(S => S.CompanyId == sessionState.Company.Id)
                                            .Where(S => S.System == sessionState.selectedSystem)
                                            .ToListAsync();
        }

        public async Task<SmartflowRecords> SaveSmartFlowRecord(UsrOrDefChapterManagement chapter, IUserSessionState sessionState)
        {
            SmartflowRecords record = new SmartflowRecords { CompanyId = sessionState.Company.Id };

            
            var existingRecord = await context.SmartflowRecords
                                                .Where(R => R.RowId == chapter.Id && R.CompanyId == sessionState.Company.Id)
                                                .Where(R => R.System == sessionState.selectedSystem)
                                                .SingleOrDefaultAsync();

            if (existingRecord is null)
            {
                mapper.Map(chapter, record);

                record.System = sessionState.selectedSystem;
                record.CreatedByUserId = sessionState.User.Id;
                record.CreatedDate = DateTime.Now;
                record.LastModifiedByUserId = sessionState.User.Id;
                record.LastModifiedDate = DateTime.Now;

                context.SmartflowRecords.Add(record);            
            }
            else
            {
                mapper.Map(chapter, existingRecord);

                existingRecord.LastModifiedByUserId = sessionState.User.Id;
                existingRecord.LastModifiedDate = DateTime.Now;
            }

            await context.SaveChangesAsync();
            return record;

        }

        public async Task<SmartflowRecords> SaveSmartFlowRecordData(UsrOrDefChapterManagement chapter, IUserSessionState sessionState)
        {
            SmartflowRecords record = new SmartflowRecords { CompanyId = sessionState.Company.Id };


            var existingRecord = await context.SmartflowRecords
                                                .Where(R => R.RowId == chapter.Id && R.CompanyId == sessionState.Company.Id)
                                                .Where(R => R.System == sessionState.selectedSystem)
                                                .SingleOrDefaultAsync();

            if (existingRecord is null)
            {
                record.ChapterData = chapter.ChapterData;

                record.System = sessionState.selectedSystem;
                record.CreatedByUserId = sessionState.User.Id;
                record.CreatedDate = DateTime.Now;
                record.LastModifiedByUserId = sessionState.User.Id;
                record.LastModifiedDate = DateTime.Now;

                context.SmartflowRecords.Add(record);
            }
            else
            {
                existingRecord.ChapterData = chapter.ChapterData;

                existingRecord.LastModifiedByUserId = sessionState.User.Id;
                existingRecord.LastModifiedDate = DateTime.Now;
            }

            await context.SaveChangesAsync();
            return record;

        }

        public async Task<SmartflowRecords> RemoveSmartFlowRecord(int id, IUserSessionState sessionState)
        {
            var existingRecord = await context.SmartflowRecords
                                                .Where(R => R.RowId == id && R.CompanyId == sessionState.Company.Id)
                                                .Where(R => R.System == sessionState.selectedSystem)
                                                .SingleOrDefaultAsync();

            if (!(existingRecord is null))
            {
                context.SmartflowRecords.Remove(existingRecord);
                await context.SaveChangesAsync();
            }

            
            return existingRecord;

        }


    }
}
