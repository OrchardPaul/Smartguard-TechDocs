using System;
using System.Linq;
using GadjIT.ClientContext.P4W.Functions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace GadjIT.ClientContext.P4W
{
    public partial class P4W_Context : DbContext
    {
        public P4W_Context()
        {
        }

        public P4W_Context(DbContextOptions<P4W_Context> options)
            : base(options)
        {
        }

        public virtual DbSet<CaseTypeGroups> CaseTypeGroups { get; set; }
        public virtual DbSet<CaseTypes> CaseTypes { get; set; }
        public virtual DbSet<DmDocuments> DmDocuments { get; set; }
        public virtual DbSet<DmDocumentsPermissions> DmDocumentsPermissions { get; set; }
        public virtual DbSet<MpSysViews> MpSysViews { get; set; }
        public virtual DbSet<UsrOrsfSmartflows> UsrOrsfSmartflows { get; set; }

        //public IQueryable<fnORCHAGetFeeDefinitions> fnORCHAGetFeeDefinitions(string Group, string CaseType) =>
        //        Set<fnORCHAGetFeeDefinitions>().FromSqlInterpolated($"select * from fn_OR_CHA_GetFeeDefinitions({Group},{CaseType})");


        public IQueryable<TableDate> GetTableDates() =>
                Set<TableDate>().FromSqlInterpolated($"SELECT t.InternalName + '.' + f.InternalFieldName FROM Mp_Sys_FieldDef f INNER JOIN Mp_Sys_TableDef t on f.TableRef = t.ID WHERE SQLTypeRef = 91");



        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {


            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Data Source=dev\\SQLEXPRESS;Initial Catalog=P4W_OR_RESI_V6_DEV;Integrated Security=True");
            }

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<fnORCHAGetFeeDefinitions>().HasNoKey().ToView(null);

            modelBuilder.Entity<CaseTypeGroups>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .IsClustered(false);

                entity.HasIndex(e => e.Name)
                    .HasName("UC_CaseTypeGroups_Name")
                    .IsUnique();

                entity.Property(e => e.Name).IsUnicode(false);
            });

            modelBuilder.Entity<CaseTypes>(entity =>
            {
                entity.HasKey(e => e.Code)
                    .IsClustered(false);

                entity.HasIndex(e => e.CodeName)
                    .HasName("UC_CaseTypes_CodeName")
                    .IsUnique();

                entity.Property(e => e.ArchiveLocation)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(null)");

                entity.Property(e => e.ArchiveMode).HasDefaultValueSql("(0)");

                entity.Property(e => e.CaseTypeGroupRef).HasDefaultValueSql("(0)");

                entity.Property(e => e.CodeName).IsUnicode(false);

                entity.Property(e => e.ComplianceCentre).HasDefaultValueSql("((1))");

                entity.Property(e => e.ComplianceHighValue).HasDefaultValueSql("((0.00))");

                entity.Property(e => e.DefaultDestroyYears).HasDefaultValueSql("(0)");

                entity.Property(e => e.DefaultPasagenda).IsUnicode(false);

                entity.Property(e => e.DefaultUtbmscodeSet)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Description).IsUnicode(false);

                entity.Property(e => e.Inactive).HasDefaultValueSql("(0)");

                entity.Property(e => e.InitialAgendaName).IsUnicode(false);

                entity.Property(e => e.IsokonCaseType).IsUnicode(false);

                entity.Property(e => e.MaxConsequenceRating).HasDefaultValueSql("(0)");

                entity.Property(e => e.Mpview).IsUnicode(false);

                entity.Property(e => e.NetDocsSync).HasDefaultValueSql("((1))");

                entity.Property(e => e.NextFileNumber).HasDefaultValueSql("(1)");

                entity.Property(e => e.Samallowed).HasDefaultValueSql("(0)");
            });

            modelBuilder.Entity<DmDocuments>(entity =>
            {
                entity.HasKey(e => e.Code)
                    .IsClustered(false);

                entity.Property(e => e.Actioned).HasDefaultValueSql("(0)");

                entity.Property(e => e.AlwaysSpecial).HasDefaultValueSql("(0)");

                entity.Property(e => e.AutoTime).HasDefaultValueSql("(0)");

                entity.Property(e => e.CanTake).IsUnicode(false);

                entity.Property(e => e.CaseTypeGroupRef).HasDefaultValueSql("(0)");

                entity.Property(e => e.ChequeReq).HasDefaultValueSql("(0)");

                entity.Property(e => e.ConsequenceRating).HasDefaultValueSql("(0)");

                entity.Property(e => e.Created).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.CreatedByRef).IsUnicode(false);

                entity.Property(e => e.Critical).HasDefaultValueSql("(0)");

                entity.Property(e => e.DateAmended).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DateReviewed).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DefaultMacro).HasDefaultValueSql("(0)");

                entity.Property(e => e.DefaultProgressFile).IsUnicode(false);

                entity.Property(e => e.Deleteble).HasDefaultValueSql("(1)");

                entity.Property(e => e.DocumentType).HasDefaultValueSql("(0)");

                entity.Property(e => e.Duration).HasDefaultValueSql("(0)");

                entity.Property(e => e.DurationType).HasDefaultValueSql("(0)");

                entity.Property(e => e.EmailTemplate).IsUnicode(false);

                entity.Property(e => e.FeeDiaryDays).HasDefaultValueSql("(0)");

                entity.Property(e => e.FeeDiaryText).IsUnicode(false);

                entity.Property(e => e.Flags).HasDefaultValueSql("(0)");

                entity.Property(e => e.FlatFee).HasDefaultValueSql("(0)");

                entity.Property(e => e.InstDiaryText).IsUnicode(false);

                entity.Property(e => e.IntsDiaryDays).HasDefaultValueSql("(0)");

                entity.Property(e => e.Location).IsUnicode(false);

                entity.Property(e => e.Mandatory).HasDefaultValueSql("(0)");

                entity.Property(e => e.MultiMatter).HasDefaultValueSql("(0)");

                entity.Property(e => e.Name).IsUnicode(false);

                entity.Property(e => e.NoOfCopies).HasDefaultValueSql("(1)");

                entity.Property(e => e.NoUnassignedGroupEntries).HasDefaultValueSql("(1)");

                entity.Property(e => e.Notes).IsUnicode(false);

                entity.Property(e => e.OutputType).HasDefaultValueSql("(0)");

                entity.Property(e => e.PartDiaryDays).HasDefaultValueSql("(0)");

                entity.Property(e => e.PartDiaryText).IsUnicode(false);

                entity.Property(e => e.PdfhtmlemailTemplate).HasDefaultValueSql("((-1))");

                entity.Property(e => e.PdfstationerySetId).HasDefaultValueSql("((-1))");

                entity.Property(e => e.ReminderDocumentRef).HasDefaultValueSql("(0)");

                entity.Property(e => e.RenameDesc)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Replicate).HasDefaultValueSql("(1)");

                entity.Property(e => e.Samaction).HasDefaultValueSql("(0)");

                entity.Property(e => e.Sammacro).HasDefaultValueSql("(0)");

                entity.Property(e => e.Sampriority).HasDefaultValueSql("((1))");

                entity.Property(e => e.StepCategory).IsUnicode(false);

                entity.Property(e => e.SupDiaryDays).HasDefaultValueSql("(0)");

                entity.Property(e => e.SupDiaryText).IsUnicode(false);

                entity.Property(e => e.TemplateEmail).HasDefaultValueSql("(0)");

                entity.Property(e => e.UserHelp)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.WebPublish).HasDefaultValueSql("(0)");
            });

            modelBuilder.Entity<DmDocumentsPermissions>(entity =>
            {
                entity.HasKey(e => new { e.Doccode, e.Casetype })
                    .HasName("PK__DM_DocumentsPerm__1F7A4DDE");
            });

            modelBuilder.Entity<MpSysViews>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .IsClustered(false);

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.CaseGroupRef).HasDefaultValueSql("(0)");

                entity.Property(e => e.DescriptionField).HasDefaultValueSql("(0)");

                entity.Property(e => e.Designable).HasDefaultValueSql("(1)");

                entity.Property(e => e.Flags).HasDefaultValueSql("(0)");

                entity.Property(e => e.Form)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Icon)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.InternalName).IsUnicode(false);

                entity.Property(e => e.InternalNote).IsUnicode(false);

                entity.Property(e => e.Name).IsUnicode(false);

                entity.Property(e => e.NextItem).HasDefaultValueSql("(0)");

                entity.Property(e => e.ReadLock).HasDefaultValueSql("(0)");

                entity.Property(e => e.System).HasDefaultValueSql("(0)");

                entity.Property(e => e.SystemName)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Type).HasDefaultValueSql("(0)");

                entity.Property(e => e.Visibility).HasDefaultValueSql("(1)");

                entity.Property(e => e.WriteLock).HasDefaultValueSql("(0)");
            });


            modelBuilder.Entity<UsrOrsfSmartflows>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .IsClustered(false);

                entity.Property(e => e.CaseType).IsUnicode(false);

                entity.Property(e => e.CaseTypeGroup).IsUnicode(false);

                entity.Property(e => e.SmartflowData).IsUnicode(false);

                entity.Property(e => e.SmartflowName).IsUnicode(false);

                entity.Property(e => e.VariantName).IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
