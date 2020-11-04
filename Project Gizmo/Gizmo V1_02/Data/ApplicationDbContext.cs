using System;
using System.Collections.Generic;
using System.Text;
using Gizmo.Context.Gizmo_Authentification;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Gizmo_V1_02.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser,ApplicationRole,string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AppCompanyDetails> AppCompanyDetails { get; set; }
        public virtual DbSet<AppCompanyWorkTypeGroups> AppCompanyWorkTypeGroups { get; set; }
        public virtual DbSet<AppWorkTypeGroups> AppWorkTypeGroups { get; set; }
        public virtual DbSet<AppWorkTypeGroupsTypeAssignments> AppWorkTypeGroupsTypeAssignments { get; set; }
        public virtual DbSet<AppWorkTypes> AppWorkTypes { get; set; }
        public virtual DbSet<AppCompanyWorkTypeMapping> AppCompanyWorkTypeMapping { get; set; }

    }
}
