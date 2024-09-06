using GadjIT_AppContext.GadjIT_App;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GadjIT_App.Data
{
    /*
     * Duplicate of ApplicationDbContext
     * To avoid concurrency errors on company managament screens with the setup of the session state on refresh
     *
     * Identity is safer with its own Context especially as it expects a standard Context inherited from IdentityDbContext, and not a factory
     */

    public class AuthorisationDbContext : IdentityDbContext<ApplicationUser,ApplicationRole,string>
    {
        public AuthorisationDbContext(DbContextOptions<AuthorisationDbContext> options)
            : base(options)
        {
        }
        

        public virtual DbSet<AppCompanyAccountsSmartflow> AppCompanyAccountsSmartflow { get; set; }
        public virtual DbSet<AppCompanyAccountsSmartflowDetails> AppCompanyAccountsSmartflowDetails { get; set; }
        public virtual DbSet<AppCompanyDetails> AppCompanyDetails { get; set; }
        public virtual DbSet<AppCompanyUserRoles> AppCompanyUserRoles { get; set; }
        public virtual DbSet<AppCompanyWorkTypeGroups> AppCompanyWorkTypeGroups { get; set; }
        public virtual DbSet<AppDepartments> AppDepartments { get; set; }
        public virtual DbSet<AppWorkTypeGroups> AppWorkTypeGroups { get; set; }
        public virtual DbSet<AppWorkTypeGroupsTypeAssignments> AppWorkTypeGroupsTypeAssignments { get; set; }
        public virtual DbSet<AppWorkTypes> AppWorkTypes { get; set; }
        public virtual DbSet<AppCompanyWorkTypeMapping> AppCompanyWorkTypeMapping { get; set; }
        public virtual DbSet<App_SmartflowRecord> App_SmartflowRecord { get; set; }
        public virtual DbSet<GadjITLog> GadjItLogs { get; set; }
    }
}
