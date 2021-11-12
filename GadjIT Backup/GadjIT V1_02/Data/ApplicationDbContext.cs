using System;
using System.Collections.Generic;
using System.Text;
using GadjIT.AppContext.GadjIT_App;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GadjIT_V1_02.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser,ApplicationRole,string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
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
