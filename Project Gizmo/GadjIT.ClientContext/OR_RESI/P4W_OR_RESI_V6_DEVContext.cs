using System;
using System.Linq;
using GadjIT.ClientContext.OR_RESI.Functions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace GadjIT.ClientContext.OR_RESI
{
    public partial class P4W_OR_RESI_V6_DEVContext : DbContext
    {
        public P4W_OR_RESI_V6_DEVContext()
        {
        }

        public P4W_OR_RESI_V6_DEVContext(DbContextOptions<P4W_OR_RESI_V6_DEVContext> options)
            : base(options)
        {
        }

        public virtual DbSet<CaseTypeGroups> CaseTypeGroups { get; set; }
        public virtual DbSet<CaseTypes> CaseTypes { get; set; }
        public virtual DbSet<DmDocuments> DmDocuments { get; set; }
        public virtual DbSet<DmDocumentsPermissions> DmDocumentsPermissions { get; set; }
        public virtual DbSet<UsrOrDefChapterManagement> UsrOrDefChapterManagement { get; set; }
        public virtual DbSet<UsrOrResiMtChapterControl> UsrOrResiMtChapterControl { get; set; }
        public virtual DbSet<UsrOrResiMtFees> UsrOrResiMtFees { get; set; }

        //public DbSet<fnORCHAGetFeeDefinitions> fnORCHAGetFeeDefinitions { get; set; }

       
        public IQueryable<fnORCHAGetFeeDefinitions> fnORCHAGetFeeDefinitions(string Group, string CaseType) =>
                Set<fnORCHAGetFeeDefinitions>().FromSqlInterpolated($"select * from fn_OR_CHA_GetFeeDefinitions({Group},{CaseType})");
       
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
            modelBuilder.Entity<fnORCHAGetFeeDefinitions>().HasNoKey().ToView(null);

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


            modelBuilder.Entity<UsrOrDefChapterManagement>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .IsClustered(false);

                entity.Property(e => e.AltDisplayName).IsUnicode(false);

                entity.Property(e => e.AsName).IsUnicode(false);

                entity.Property(e => e.CaseType).IsUnicode(false);

                entity.Property(e => e.CaseTypeGroup).IsUnicode(false);

                entity.Property(e => e.CompleteName).IsUnicode(false);

                entity.Property(e => e.EntityType).IsUnicode(false);

                entity.Property(e => e.Name).IsUnicode(false);

                entity.Property(e => e.SuppressStep).IsUnicode(false);

                entity.Property(e => e.Type).IsUnicode(false);
            });

            modelBuilder.Entity<UsrOrResiMtChapterControl>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .IsClustered(false);

                entity.HasIndex(e => new { e.EntityRef, e.MatterNo })
                    .HasName("Usr_OR_RESI_MT_Chapter_Control_MPIndex");

                entity.Property(e => e.CompleteAsName).IsUnicode(false);

                entity.Property(e => e.CurrentChapter).IsUnicode(false);

                entity.Property(e => e.DefaultStep).IsUnicode(false);

                entity.Property(e => e.DefaultStepAsName).IsUnicode(false);

                entity.Property(e => e.DoNotReschedule).IsUnicode(false);

                entity.Property(e => e.EntityRef).IsUnicode(false);

                entity.Property(e => e.ScheduleAsName).IsUnicode(false);

                entity.Property(e => e.StepsToRun).IsUnicode(false);

                entity.Property(e => e.SubViewName).IsUnicode(false);
            });

            modelBuilder.Entity<UsrOrResiMtFees>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("PK_Usr_OR_RESIFeeList")
                    .IsClustered(false);

                entity.Property(e => e.Account).IsUnicode(false);

                entity.Property(e => e.Anticipated).IsUnicode(false);

                entity.Property(e => e.Category).IsUnicode(false);

                entity.Property(e => e.EntityRef).IsUnicode(false);

                entity.Property(e => e.FeeDescription).IsUnicode(false);

                entity.Property(e => e.FeeType).IsUnicode(false);

                entity.Property(e => e.FeeTypeGroup).IsUnicode(false);

                entity.Property(e => e.Funded).IsUnicode(false);

                entity.Property(e => e.Payee).IsUnicode(false);

                entity.Property(e => e.PayeePayer).IsUnicode(false);

                entity.Property(e => e.PaymentMethod).IsUnicode(false);

                entity.Property(e => e.PostingType).IsUnicode(false);

                entity.Property(e => e.PrintSlipYn).IsUnicode(false);

                entity.Property(e => e.ReconcileTable).IsUnicode(false);

                entity.Property(e => e.SlipPrintStatus).IsUnicode(false);

                entity.Property(e => e.Status).IsUnicode(false);

                entity.Property(e => e.Suffix).IsUnicode(false);

                entity.Property(e => e.TransactionType).IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
