using GadjIT.AppContext.GadjIT_App;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GadjIT_V1_02.Data
{
    /*
     * Duplicate of ApplicationDbContext
     * To avoid concurrency errors on company managament screens with the setup of the session state on refresh
     */

    public class AuthorisationDBContext : DbContext
    {
        
        public AuthorisationDBContext(DbContextOptions<AuthorisationDBContext> options)
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
        public virtual DbSet<SmartflowRecords> SmartflowRecords { get; set; }
    }
}
