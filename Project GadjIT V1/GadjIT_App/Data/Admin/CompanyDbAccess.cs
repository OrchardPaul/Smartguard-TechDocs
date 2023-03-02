using AutoMapper;
using GadjIT_AppContext.GadjIT_App;
using GadjIT_AppContext.GadjIT_App.Custom;
using GadjIT_ClientContext.P4W;
using GadjIT_App.Services.SessionState;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace GadjIT_App.Data.Admin
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
        Task<SmartflowRecords> SaveSmartFlowRecord(UsrOrsfSmartflows chapter, IUserSessionState sessionState);
        Task<SmartflowRecords> SaveSmartFlowRecordData(UsrOrsfSmartflows chapter, IUserSessionState sessionState);
        Task<SmartflowRecords> RemoveSmartFlowRecord(int id, IUserSessionState sessionState);
        Task<List<SmartflowRecords>> SyncAdminSysToClient(List<UsrOrsfSmartflows> clientObjects, IUserSessionState sessionState);
        Task<List<SmartflowRecords>> GetAllSmartflowRecords(IUserSessionState sessionState);
        Task<List<SmartflowRecords>> GetAllSmartflowRecordsForAllCompanies();
        Task<SmartflowRecords> GetSmartflow(IUserSessionState _sessionState, string _caseTypeGroup, string _caseType, string _smartflowName);
        bool Lock { get; set; }

        Task<List<AppCompanyAccountsSmartflowDetails>> GetCompanyAccountDetailsByAccountId(int accountId);
        Task<List<AppCompanyAccountsSmartflow>> GetCompanyAccounts();
        Task<bool> RefreshAccounts();
        Task<AppCompanyAccountsSmartflowDetails> UpdateSmartflowAccountDetails(AppCompanyAccountsSmartflowDetails appCompanyAccountsSmartflow);
        Task<List<BillingItem>> BillCompany(CompanyAccountObject companyAccount, List<SmartflowRecords> smartflowRecords, bool draft);
    }

    public class CompanyDbAccess : ICompanyDbAccess
    {

        private readonly IDbContextFactory<ApplicationDbContext> contextFactory;
        private readonly AuthenticationStateProvider authenticationStateProvider;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IIdentityUserAccess identityUserAccess;
        private readonly IMapper mapper;
        private readonly ILogger<CompanyDbAccess> logger;

        public CompanyDbAccess(IDbContextFactory<ApplicationDbContext> contextFactory
                                , AuthenticationStateProvider authenticationStateProvider
                                , UserManager<ApplicationUser> userManager
                                , IIdentityUserAccess identityUserAccess
                                , IMapper mapper
                                , ILogger<CompanyDbAccess> logger)
        {
            this.contextFactory = contextFactory;
            this.authenticationStateProvider = authenticationStateProvider;
            this.userManager = userManager;
            this.identityUserAccess = identityUserAccess;
            this.mapper = mapper;
            this.logger = logger;
        }


        public bool Lock { get; set; } = false;

        /*
         * 
         * Work Types
         * 
         */

        public async Task<List<AppDepartments>> GetDepartments()
        {
            using (var context = contextFactory.CreateDbContext())
            {
                return await context.AppDepartments.ToListAsync();
            }
        }

        public async Task<List<AppWorkTypeGroups>> GetWorkTypeGroups()
        {
            using (var context = contextFactory.CreateDbContext())
            {
                return await context.AppWorkTypeGroups.ToListAsync();
            }
            
        }

        public async Task<List<AppWorkTypes>> GetWorkTypes()
        {
            using (var context = contextFactory.CreateDbContext())
            {
                return await context.AppWorkTypes.ToListAsync();
            }
        }

        public async Task<WorkTypeMapping> GetWorkTypeMappingsByCompany(AppCompanyDetails company
                                                                                , List<CaseTypes> allCaseTypes
                                                                                , AppWorkTypes workType
                                                                                , string system)
        {
            using (var context = contextFactory.CreateDbContext())
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
            
        }

        public async Task<List<AppCompanyWorkTypeMapping>> UpdateWorkTypeMapping(WorkTypeMapping typeMapping
                                                                                , AppCompanyDetails company
                                                                                , string system)
        {
            using (var context = contextFactory.CreateDbContext())
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


        }

        public async Task<List<WorkTypeGroupItem>> GetGroupsWithWorkTypes()
        {

            using (var context = contextFactory.CreateDbContext())
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

        }

        public async Task<List<AppWorkTypeGroups>> GetWorkTypeGroupsByCompany(int companyId)
        {
            using (var context = contextFactory.CreateDbContext())
            {
                var assignedWorkTypeGroups = await context.AppCompanyWorkTypeGroups
                                                        .Where(A => A.CompanyId == companyId)
                                                        .Select(A => A.WorkTypeGroupId)
                                                        .ToListAsync();

                return await context.AppWorkTypeGroups
                    .Where(A => assignedWorkTypeGroups.Contains(A.Id))
                    .ToListAsync();
            }


        }

        public async Task<WorkTypeGroupItem> AssignWorkTypeToGroup(WorkTypeGroupItem workTypeGroup, List<WorkTypeGroupAssignment> assignments)
        {
            using (var context = contextFactory.CreateDbContext())
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



        }



        public async Task<AppDepartments> SubmitDepartment(AppDepartments department)
        {
            using (var context = contextFactory.CreateDbContext())
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

        }

        public async Task<AppWorkTypeGroups> SubmitWorkTypeGroup(AppWorkTypeGroups workTypeGroup)
        {
            using (var context = contextFactory.CreateDbContext())
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

        }

        public async Task<AppWorkTypes> SubmitWorkType(AppWorkTypes workType)
        {
            using (var context = contextFactory.CreateDbContext())
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

        }

        public async Task<AppDepartments> DeleteDepartment(AppDepartments department)
        {
            using (var context = contextFactory.CreateDbContext())
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

        }

        public async Task<AppWorkTypeGroups> DeleteWorkTypeGroup(AppWorkTypeGroups group)
        {
            using (var context = contextFactory.CreateDbContext())
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

        }

        public async Task<AppWorkTypes> DeleteWorkType(AppWorkTypes workType)
        {
            using (var context = contextFactory.CreateDbContext())
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


        }

        public async Task<AppWorkTypeGroupsTypeAssignments> DeleteAssignment(AppWorkTypeGroupsTypeAssignments assignment)
        {
            using (var context = contextFactory.CreateDbContext())
            {
                var selectedAssignment = await context.AppWorkTypeGroupsTypeAssignments
                                                    .SingleOrDefaultAsync(C => C.Id == assignment.Id);

                context.AppWorkTypeGroupsTypeAssignments.Remove(selectedAssignment);
                await context.SaveChangesAsync();

                return selectedAssignment;
            }

        }


        /*
         * 
         * Companies
         * 
         */

        public async Task<List<AppCompanyDetails>> GetCompanies()
        {
            using (var context = contextFactory.CreateDbContext())
            {
                var signedInUserState = await authenticationStateProvider.GetAuthenticationStateAsync();
                var signedInUserAsp = await userManager.FindByNameAsync(signedInUserState.User.Identity.Name);
                var signedInUser = mapper.Map(signedInUserAsp, new AspNetUsers());

                if (signedInUserState.User.IsInRole("Super User"))
                {
                    return await context.AppCompanyDetails
                                        .OrderBy(A => A.CompanyName)
                                        .ToListAsync();
                }
                else
                {
                    var signedInUsersCompany = await identityUserAccess.GetCompanyClaims(signedInUser);
                    var signedInUsersCompanyIds = signedInUsersCompany.Select(C => C.Value).ToList();

                    return await context.AppCompanyDetails
                                        .Where(A => signedInUsersCompanyIds.Contains(A.Id.ToString()))
                                        .OrderBy(A => A.CompanyName)
                                        .ToListAsync();
                }
            }
        }


        public async Task<AppCompanyDetails> GetCompanyById(int id)
        {
            using (var context = contextFactory.CreateDbContext())
            {
                return await context.AppCompanyDetails.SingleAsync(C => C.Id == id);
            }
            
        }

        public async Task<AppCompanyDetails> GetSelectedCompanyOfUser(AspNetUsers user)
        {
            using (var context = contextFactory.CreateDbContext())
            {
                return await context.AppCompanyDetails.SingleOrDefaultAsync(C => C.Id == user.SelectedCompanyId);
            }
            
        }

        public async Task<string> GetCompanyBaseUri(int id, string selectedServer)
        {
            using (var context = contextFactory.CreateDbContext())
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
            
        }

        public async Task<AppCompanyDetails> SubmitChanges(AppCompanyDetails company)
        {
            using (var context = contextFactory.CreateDbContext())
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
                    selectedCompany.CompanyDesc = company.CompanyDesc;
                    selectedCompany.DevUri = company.DevUri;
                    selectedCompany.LiveUri = company.LiveUri;
                    selectedCompany.CompCol1 = company.CompCol1;
                    selectedCompany.CompCol2 = company.CompCol2;
                    selectedCompany.CompCol3 = company.CompCol3;
                    selectedCompany.CompCol4 = company.CompCol4;
                    selectedCompany.CurrentVersion = company.CurrentVersion;

                    await context.SaveChangesAsync();
                    return selectedCompany;
                }
            }
            
        }

        public async Task<AppCompanyDetails> AssignWorkTypeGroupToCompany(AppCompanyDetails company, AppWorkTypeGroups workTypeGroup)
        {
            using (var context = contextFactory.CreateDbContext())
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

        }

        public async Task<AppCompanyDetails> RemoveWorkTypeGroupFromCompany(AppCompanyDetails company, AppWorkTypeGroups workTypeGroup)
        {
            using (var context = contextFactory.CreateDbContext())
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

        }


        public async Task<AppCompanyDetails> DeleteCompany(AppCompanyDetails company)
        {
            using (var context = contextFactory.CreateDbContext())
            {
                var selectedCompany = await context.AppCompanyDetails.SingleOrDefaultAsync(C => C.Id == company.Id);

                context.AppCompanyDetails.Remove(selectedCompany);
                await context.SaveChangesAsync();

                return selectedCompany;
            }

        }


        public async Task<List<SmartflowRecords>> SyncAdminSysToClient(List<UsrOrsfSmartflows> clientObjects, IUserSessionState sessionState)
        {
            using (var context = contextFactory.CreateDbContext())
            {
                var currentRecords = await context
                                                .SmartflowRecords
                                                .Where(R => R.CompanyId == sessionState.Company.Id)
                                                .Where(R => R.System == sessionState.SelectedSystem)
                                                .ToListAsync();

                var accountsDeleted = await context.AppCompanyAccountsSmartflowDetails
                                            .Where(A => A.CompanyId == sessionState.Company.Id
                                                        && A.System == sessionState.SelectedSystem)
                                            .Where(A => !clientObjects.Select(C => C.Id).Contains(A.ClientRowId))
                                            .ToListAsync();            

                foreach(var account in accountsDeleted)
                {
                    account.Status = "Deleted";
                    context.AppCompanyAccountsSmartflowDetails.Update(account);
                }

                foreach(var record in currentRecords)
                {
                    context.SmartflowRecords.Remove(record);
                }

                await context.SaveChangesAsync();

                foreach (var clientRow in clientObjects)
                {
                    var record = new SmartflowRecords { CompanyId = sessionState.Company.Id };
                    mapper.Map(clientRow, record);

                    record.System = sessionState.SelectedSystem;
                    record.CreatedByUserId = sessionState.User.Id;
                    record.CreatedDate = DateTime.Now;
                    record.LastModifiedByUserId = sessionState.User.Id;
                    record.LastModifiedDate = DateTime.Now;

                    context.SmartflowRecords.Add(record);

                    await context.SaveChangesAsync();
                }

                return currentRecords;
            }

        }


        public async Task<List<SmartflowRecords>> GetAllSmartflowRecords(IUserSessionState sessionState)
        {
            using (var context = contextFactory.CreateDbContext())
            {
                var returnValues = new List<SmartflowRecords>();

                try
                {
                    Lock = true;

                    returnValues = await context.SmartflowRecords.Where(S => S.CompanyId == sessionState.Company.Id)
                                    .Where(S => S.System == sessionState.SelectedSystem)
                                    .ToListAsync();

                }
                finally
                {
                    Lock = false;
                }

                return returnValues;
            }

        }

        public async Task<List<SmartflowRecords>> GetAllSmartflowRecordsForAllCompanies()
        {
            using (var context = contextFactory.CreateDbContext())
            {
                var returnValues = new List<SmartflowRecords>();

                try
                {
                    Lock = true;

                    returnValues = await context.SmartflowRecords
                                    .ToListAsync();

                }
                finally
                {
                    Lock = false;
                }

                return returnValues;
            }

        }

        public async Task<SmartflowRecords> GetSmartflow(IUserSessionState _sessionState, string _caseTypeGroup, string _caseType, string _smartflowName)
        {
            using (var context = contextFactory.CreateDbContext())
            {
                var returnValue = new SmartflowRecords();

                try
                {
                    Lock = true;

                    returnValue = await context.SmartflowRecords.Where(S => S.CompanyId == _sessionState.Company.Id)
                                    .Where(S => S.System == _sessionState.SelectedSystem)
                                    .Where(S => S.CaseTypeGroup == _caseTypeGroup)
                                    .Where(S => S.CaseType == _caseType)
                                    .Where(S => S.SmartflowName == _smartflowName)
                                    .FirstOrDefaultAsync();

                }
                finally
                {
                    Lock = false;
                }

                return returnValue;
            }

        }


        public async Task<SmartflowRecords> SaveSmartFlowRecord(UsrOrsfSmartflows chapter, IUserSessionState sessionState)
        {
            using (var context = contextFactory.CreateDbContext())
            {
                SmartflowRecords record = new SmartflowRecords { CompanyId = sessionState.Company.Id };

                
                var existingRecord = await context.SmartflowRecords
                                                    .Where(R => R.RowId == chapter.Id && R.CompanyId == sessionState.Company.Id)
                                                    .Where(R => R.System == sessionState.SelectedSystem)
                                                    .SingleOrDefaultAsync();

                if (existingRecord is null)
                {
                    mapper.Map(chapter, record);

                    record.System = sessionState.SelectedSystem;
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


        }

        public async Task<SmartflowRecords> SaveSmartFlowRecordData(UsrOrsfSmartflows chapter, IUserSessionState sessionState)
        {
            using (var context = contextFactory.CreateDbContext())
            {
                Lock = true;

                SmartflowRecords record = new SmartflowRecords { CompanyId = sessionState.Company.Id };


                var existingRecord = await context.SmartflowRecords
                                                    .Where(R => R.RowId == chapter.Id && R.CompanyId == sessionState.Company.Id)
                                                    .Where(R => R.System == sessionState.SelectedSystem)
                                                    .SingleOrDefaultAsync();

                if (existingRecord is null)
                {
                    record.SmartflowData = chapter.SmartflowData;

                    record.System = sessionState.SelectedSystem;
                    record.CreatedByUserId = sessionState.User.Id;
                    record.CreatedDate = DateTime.Now;
                    record.LastModifiedByUserId = sessionState.User.Id;
                    record.LastModifiedDate = DateTime.Now;

                    context.SmartflowRecords.Add(record);
                }
                else
                {
                    existingRecord.SmartflowData = chapter.SmartflowData;

                    existingRecord.LastModifiedByUserId = sessionState.User.Id;
                    existingRecord.LastModifiedDate = DateTime.Now;
                }

                await context.SaveChangesAsync();

                Lock = false;
                return record;
            }
        }

        public async Task<SmartflowRecords> RemoveSmartFlowRecord(int id, IUserSessionState sessionState)
        {
            using (var context = contextFactory.CreateDbContext())
            {
                var existingRecord = await context.SmartflowRecords
                                                    .Where(R => R.RowId == id && R.CompanyId == sessionState.Company.Id)
                                                    .Where(R => R.System == sessionState.SelectedSystem)
                                                    .FirstOrDefaultAsync();

                if (!(existingRecord is null))
                {
                    var existingAccount = await context
                                .AppCompanyAccountsSmartflowDetails
                                .Where(A => A.CompanyId == existingRecord.CompanyId 
                                            && A.ClientRowId == existingRecord.RowId
                                            && A.System == existingRecord.System)
                                .FirstOrDefaultAsync();

                    if(!(existingAccount is null))
                    {
                        existingAccount.Status = "Deleted";

                        context.AppCompanyAccountsSmartflowDetails.Update(existingAccount);
                        await context.SaveChangesAsync();
                    }


                    context.SmartflowRecords.Remove(existingRecord);
                    await context.SaveChangesAsync();
                }

                
                return existingRecord;
            }


        }



        public async Task<bool> RefreshAccounts()
        {
            try
            {
                using (var context = contextFactory.CreateDbContext())
                {
                    var existingAccounts = await context.AppCompanyAccountsSmartflow.ToListAsync();

                    var existingCompanies = await context.AppCompanyDetails.ToListAsync();

                    var newAccounts = existingCompanies
                                                    .Where(C => !existingAccounts.Select(A => A.CompanyId).ToList().Contains(C.Id))
                                                    .Select(C => new AppCompanyAccountsSmartflow
                                                    {
                                                        CompanyId = C.Id,
                                                        Subscription = 250,
                                                        TotalBilled = 0
                                                    }
                                                    ).ToList();

                    if (!(newAccounts is null) && newAccounts.Count > 0)
                    {
                        context.AppCompanyAccountsSmartflow.AddRange(newAccounts);
                        await context.SaveChangesAsync();
                    }


                    existingAccounts = await context.AppCompanyAccountsSmartflow.ToListAsync();
                    var existingAccountDetails = await context.AppCompanyAccountsSmartflowDetails.ToListAsync();
                    var existingSmartflowRecords = await context.SmartflowRecords.ToListAsync();

                    foreach(var companyId in existingAccounts.Select(A => A.CompanyId))
                    {
                        var newAccountDetails = existingSmartflowRecords
                                                                        .Where(R => R.CompanyId == companyId)
                                                                        .Where(R => !existingAccountDetails
                                                                                    .Select(D => D.ClientRowId)
                                                                                    .ToList()
                                                                                    .Contains(R.RowId))
                                                                        .Select(R => new AppCompanyAccountsSmartflowDetails
                                                                        {
                                                                            ClientRowId = R.RowId,
                                                                            CompanyId = companyId,
                                                                            StartDate = R.System.Trim() == "Live" ? R.CreatedDate.AddMonths(1) : R.CreatedDate,
                                                                            System = R.System.Trim(),
                                                                            SmartflowName = R.SmartflowName,
                                                                            CaseType = R.CaseType,
                                                                            CaseTypeGroup = R.CaseTypeGroup,
                                                                            Billable = true,
                                                                            Status = "Active",
                                                                            MonthlyCharge = R.System == "Live" ? 5 : 0,
                                                                            MonthsDuration = R.System == "Live" ? 12 : 0,
                                                                            MonthsRemaining = R.System == "Live" ? 12 : 0,
                                                                            Outstanding = R.System == "Live" ? 5 * 12 : 0,
                                                                            TotalBilled = 0,
                                                                            CreatedBy = R.CreatedByUserId,
                                                                            SmartflowAccountId = existingAccounts
                                                                                                            .Where(E => E.CompanyId == R.CompanyId)
                                                                                                            .Select(E => E.Id)
                                                                                                            .FirstOrDefault()
                                                                        }
                                                                        ).ToList();

                        newAccountDetails = newAccountDetails.Select(N => { N.System = N.System.Trim(); return N; } ).ToList();

                        context.AppCompanyAccountsSmartflowDetails.AddRange(newAccountDetails);
                    }



                    await context.SaveChangesAsync();
                    return true;
                }
                
            }
            catch(Exception e)
            {

                using (LogContext.PushProperty("SourceContext", nameof(CompanyDbAccess)))
                {
                    logger?.LogError(e, "Error retrieving smartflow Accounts");
                }

                return false;
            }

        }


        public async Task<List<AppCompanyAccountsSmartflow>> GetCompanyAccounts()
        {
            using (var context = contextFactory.CreateDbContext())
            {
                Lock = true;

                var returnValue = await context.AppCompanyAccountsSmartflow.ToListAsync();

                Lock = false;

                return returnValue;
            }

        }

        public async Task<List<AppCompanyAccountsSmartflowDetails>> GetCompanyAccountDetailsByAccountId(int accountId)
        {
            using (var context = contextFactory.CreateDbContext())
            {
                Lock = true;

                var returnValue = 
                    await context
                    .AppCompanyAccountsSmartflowDetails
                    .Where(D => D.SmartflowAccountId == accountId)
                    .ToListAsync();

                Lock = false;

                return returnValue;
            }

        }

        public async Task<AppCompanyAccountsSmartflowDetails> UpdateSmartflowAccountDetails(AppCompanyAccountsSmartflowDetails appCompanyAccountsSmartflow)
        {
            try
            {
                using (var context = contextFactory.CreateDbContext())
                {
                    Lock = true;

                    var updatingAccount = await context.AppCompanyAccountsSmartflowDetails.Where(D => D.Id == appCompanyAccountsSmartflow.Id).FirstOrDefaultAsync();

                    if (updatingAccount is null)
                    {
                        context.AppCompanyAccountsSmartflowDetails.Add(appCompanyAccountsSmartflow);

                        updatingAccount = appCompanyAccountsSmartflow;
                    }
                    else
                    {
                        updatingAccount.StartDate = appCompanyAccountsSmartflow.StartDate;
                        updatingAccount.Status = appCompanyAccountsSmartflow.Status;
                        updatingAccount.Billable = appCompanyAccountsSmartflow.Billable;
                        updatingAccount.BillingDescription = appCompanyAccountsSmartflow.BillingDescription;
                        updatingAccount.CreatedBy = appCompanyAccountsSmartflow.CreatedBy;
                        updatingAccount.System = appCompanyAccountsSmartflow.System.Trim();
                        updatingAccount.DeletedDate = appCompanyAccountsSmartflow.DeletedDate;
                        updatingAccount.MonthlyCharge = appCompanyAccountsSmartflow.MonthlyCharge;
                        updatingAccount.MonthsDuration = appCompanyAccountsSmartflow.MonthsDuration;
                        updatingAccount.MonthsRemaining = appCompanyAccountsSmartflow.MonthsRemaining;
                        updatingAccount.TotalBilled = appCompanyAccountsSmartflow.TotalBilled;
                        updatingAccount.Outstanding = appCompanyAccountsSmartflow.Outstanding;

                        context.AppCompanyAccountsSmartflowDetails.Update(updatingAccount);
                    }

                    await context.SaveChangesAsync();

                    Lock = false;

                    return updatingAccount;
                }
                
            }
            catch(Exception e)
            {

                using (LogContext.PushProperty("SourceContext", nameof(CompanyDbAccess)))
                {
                    logger?.LogError(e, $"Error updating account {appCompanyAccountsSmartflow.Id}");
                }

                return new AppCompanyAccountsSmartflowDetails();
            }

        }

        public class BillThing
        {
            public AppCompanyAccountsSmartflowDetails AccountObject { get; set; }

            public SmartflowRecords RecordObject { get; set; }

        }

        public async Task<List<BillingItem>> BillCompany(CompanyAccountObject companyAccount
                                                        , List<SmartflowRecords> smartflowRecords
                                                        , bool draft)
        {
            try
            {
                using (var context = contextFactory.CreateDbContext())
                {
                    var companyBillingSmartflowAccounts =
                        context.AppCompanyAccountsSmartflowDetails
                                .Where(C => C.SmartflowAccountId == companyAccount.AccountObject.Id)
                                .Where(C => C.System == "Live")
                                .Where(C => C.Billable)
                                .Where(C => C.StartDate < DateTime.Now)
                                .Where(C => C.MonthsRemaining > 0)
                                .Where(C => C.Status == "Active")
                                .Select(C => new BillThing
                                {
                                    AccountObject = C
                                })
                                .ToList();

                    
                    companyBillingSmartflowAccounts = companyBillingSmartflowAccounts
                    .Select(A =>
                    {
                        A.RecordObject = smartflowRecords
                                        .Where(S => A.AccountObject.ClientRowId == S.RowId 
                                                    && A.AccountObject.CompanyId == S.CompanyId
                                                    && A.AccountObject.System == S.System)
                                        .FirstOrDefault(); 
                        return A;
                    })
                    .Where(A => A.AccountObject.System.Trim() == "Live")
                    .Where(A => A.AccountObject.Status != "Deleted")
                    .ToList();
                    

                    companyAccount.AccountObject.TotalBilled = companyAccount.AccountObject.TotalBilled
                                                                + companyBillingSmartflowAccounts.Select(A => A.AccountObject.MonthlyCharge).Sum()
                                                                + companyAccount.AccountObject.Subscription;

                    companyAccount.AccountObject.LastBilledDate = DateTime.Now;

                    var BilledItems = companyBillingSmartflowAccounts
                                                .Select(B => new BillingItem
                                                {
                                                    AccountName = companyAccount.CompanyObject.CompanyName,
                                                    BillDate = DateTime.Now,
                                                    MonthlyCharge = B.AccountObject.MonthlyCharge,
                                                    MonthsRemaing = B.AccountObject.MonthsRemaining - 1,
                                                    Outstanding = B.AccountObject.Outstanding - B.AccountObject.MonthlyCharge,
                                                    Billed = B.AccountObject.TotalBilled + B.AccountObject.MonthlyCharge,
                                                    SmartflowName = B.RecordObject.SmartflowName,
                                                    SmartflowCaseTypeGroup = B.RecordObject.CaseTypeGroup,
                                                    SmartflowCaseType = B.RecordObject.CaseType
                                                })
                                                .ToList();


                    companyBillingSmartflowAccounts = companyBillingSmartflowAccounts.Select(B =>
                    {
                        B.AccountObject.Outstanding = B.AccountObject.Outstanding - B.AccountObject.MonthlyCharge;
                        B.AccountObject.MonthsRemaining = B.AccountObject.MonthsRemaining - 1;
                        B.AccountObject.TotalBilled = B.AccountObject.TotalBilled + B.AccountObject.MonthlyCharge;
                        return B;
                    })
                    .ToList();

                    if(!draft)
                    {
                        await context.SaveChangesAsync();
                    }

                    return BilledItems;
                }

            }
            catch(Exception e)
            {

                using (LogContext.PushProperty("SourceContext", nameof(CompanyDbAccess)))
                {
                    logger?.LogError(e, $"Error billing company: {companyAccount.CompanyObject.CompanyName} - {companyAccount.CompanyObject.Id}");
                }

                return new List<BillingItem>();
            }


        }

    }
}
