using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Gizmo.Context.OR_RESI
{
    public partial class P4W_OR_RESI_V6_DEVContext : DbContext
    {

        public P4W_OR_RESI_V6_DEVContext(DbContextOptions<P4W_OR_RESI_V6_DEVContext> options)
            : base(options)
        {
        }

        public virtual DbSet<CaseTypes> CaseTypes { get; set; }
        public virtual DbSet<DmDocuments> DmDocuments { get; set; }
        public virtual DbSet<DmDocumentsPermissions> DmDocumentsPermissions { get; set; }
        public virtual DbSet<TblEmployee> TblEmployee { get; set; }
        public virtual DbSet<TblToDo> TblToDo { get; set; }
        public virtual DbSet<UsrOrDefChapterManagement> UsrOrDefChapterManagement { get; set; }
        public virtual DbSet<UsrOrResiMt> UsrOrResiMt { get; set; }
        public virtual DbSet<UsrOrResiMtAdmin> UsrOrResiMtAdmin { get; set; }
        public virtual DbSet<UsrOrResiMtChapterControl> UsrOrResiMtChapterControl { get; set; }
        public virtual DbSet<UsrOrResiMtFees> UsrOrResiMtFees { get; set; }

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

            modelBuilder.Entity<TblEmployee>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.City).IsUnicode(false);

                entity.Property(e => e.Department).IsUnicode(false);

                entity.Property(e => e.EmployeeId).ValueGeneratedOnAdd();

                entity.Property(e => e.Gender).IsUnicode(false);

                entity.Property(e => e.Name).IsUnicode(false);
            });

            modelBuilder.Entity<TblToDo>(entity =>
            {
                entity.Property(e => e.Name).IsUnicode(false);

                entity.Property(e => e.Status).IsUnicode(false);
            });

            modelBuilder.Entity<UsrOrDefChapterManagement>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .IsClustered(false);

                entity.Property(e => e.AsName).IsUnicode(false);

                entity.Property(e => e.CaseType).IsUnicode(false);

                entity.Property(e => e.CaseTypeGroup).IsUnicode(false);

                entity.Property(e => e.CompleteName).IsUnicode(false);

                entity.Property(e => e.EntityType).IsUnicode(false);

                entity.Property(e => e.Name).IsUnicode(false);

                entity.Property(e => e.SuppressStep).IsUnicode(false);

                entity.Property(e => e.Type).IsUnicode(false);
            });

            modelBuilder.Entity<UsrOrResiMt>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .IsClustered(false);

                entity.Property(e => e.AdditionalAccountNo).IsUnicode(false);

                entity.Property(e => e.AdditionalBranch).IsUnicode(false);

                entity.Property(e => e.AdditionalSortcode).IsUnicode(false);

                entity.Property(e => e.BuyingThroughEstateAgent).IsUnicode(false);

                entity.Property(e => e.CarePackStatus).IsUnicode(false);

                entity.Property(e => e.CaseOwner).IsUnicode(false);

                entity.Property(e => e.CompletionAlternativeAddress1).IsUnicode(false);

                entity.Property(e => e.CompletionAlternativeAddress2).IsUnicode(false);

                entity.Property(e => e.CompletionAlternativeAddress3).IsUnicode(false);

                entity.Property(e => e.CompletionAlternativeAddress4).IsUnicode(false);

                entity.Property(e => e.CompletionAlternativeAddress5).IsUnicode(false);

                entity.Property(e => e.CompletionAlternativeAddressPc).IsUnicode(false);

                entity.Property(e => e.CompletionOnNotice).IsUnicode(false);

                entity.Property(e => e.ContactRef).IsUnicode(false);

                entity.Property(e => e.DataValidatedSuccessfully).IsUnicode(false);

                entity.Property(e => e.DefaultClient).IsUnicode(false);

                entity.Property(e => e.DepositIs10Percent).IsUnicode(false);

                entity.Property(e => e.DepositType).IsUnicode(false);

                entity.Property(e => e.EntityRef).IsUnicode(false);

                entity.Property(e => e.EstateAgentBillReceived).IsUnicode(false);

                entity.Property(e => e.EstateAgentsRef).IsUnicode(false);

                entity.Property(e => e.ExchangeCallNotes).IsUnicode(false);

                entity.Property(e => e.Exchanged).IsUnicode(false);

                entity.Property(e => e.JointClientSalutation).IsUnicode(false);

                entity.Property(e => e.JointOwnership).IsUnicode(false);

                entity.Property(e => e.LeaseholdPackChargeApplicable).IsUnicode(false);

                entity.Property(e => e.LeaseholdPackRequestedFrom).IsUnicode(false);

                entity.Property(e => e.LenderPurchase).IsUnicode(false);

                entity.Property(e => e.LenderPurchase1).IsUnicode(false);

                entity.Property(e => e.LenderSale).IsUnicode(false);

                entity.Property(e => e.LenderSale1).IsUnicode(false);

                entity.Property(e => e.LinkedCaseEntityRef).IsUnicode(false);

                entity.Property(e => e.LinkedCaseExists).IsUnicode(false);

                entity.Property(e => e.ManagingAgentRef).IsUnicode(false);

                entity.Property(e => e.MortgageRedemptionExisitingMortgageToRedeem).IsUnicode(false);

                entity.Property(e => e.MortgageRedemptionPostingSlip).IsUnicode(false);

                entity.Property(e => e.MortgageRequested).IsUnicode(false);

                entity.Property(e => e.NameOfPersonExchangedWith).IsUnicode(false);

                entity.Property(e => e.Notes).IsUnicode(false);

                entity.Property(e => e.NotesCategory).IsUnicode(false);

                entity.Property(e => e.OnHoldYn).IsUnicode(false);

                entity.Property(e => e.PostingSlipMessage).IsUnicode(false);

                entity.Property(e => e.PreciseTermsOfAgreedVariation).IsUnicode(false);

                entity.Property(e => e.PreferredContactMethodClient).IsUnicode(false);

                entity.Property(e => e.PropertyRegistered).IsUnicode(false);

                entity.Property(e => e.PropertyToBeHeld).IsUnicode(false);

                entity.Property(e => e.QuestionnaireCompleted).IsUnicode(false);

                entity.Property(e => e.RestrictionHolder1).IsUnicode(false);

                entity.Property(e => e.RestrictionHolder2).IsUnicode(false);

                entity.Property(e => e.RestrictionHolderAddress1).IsUnicode(false);

                entity.Property(e => e.RestrictionHolderAddress2).IsUnicode(false);

                entity.Property(e => e.SameDayCompletion).IsUnicode(false);

                entity.Property(e => e.SatisfactoryEvidenceReceived).IsUnicode(false);

                entity.Property(e => e.SearchAgentEmailAddress).IsUnicode(false);

                entity.Property(e => e.SearchProviderRef).IsUnicode(false);

                entity.Property(e => e.SellerRef).IsUnicode(false);

                entity.Property(e => e.SlipPayInMethodAdvance).IsUnicode(false);

                entity.Property(e => e.SlipPayInMethodSaleProceeds).IsUnicode(false);

                entity.Property(e => e.SlipPayOutMethodBalance).IsUnicode(false);

                entity.Property(e => e.SlipPayOutMethodDeposit).IsUnicode(false);

                entity.Property(e => e.SlipPayOutMethodRedemption).IsUnicode(false);

                entity.Property(e => e.Solicitorsref).IsUnicode(false);

                entity.Property(e => e.SourceOfFunds).IsUnicode(false);

                entity.Property(e => e.SourceOfIntroductionRef).IsUnicode(false);

                entity.Property(e => e.StandardIndemnityCovenant).IsUnicode(false);

                entity.Property(e => e.TenDayCoolingOffPeriodWaivered).IsUnicode(false);

                entity.Property(e => e.TitleNumber).IsUnicode(false);

                entity.Property(e => e.TitleNumberSecondTitle).IsUnicode(false);

                entity.Property(e => e.TitleNumberThirdTitle).IsUnicode(false);

                entity.Property(e => e.WriteToClient2Separately).IsUnicode(false);
            });

            modelBuilder.Entity<UsrOrResiMtAdmin>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .IsClustered(false);

                entity.Property(e => e.ContactFilterExists).IsUnicode(false);

                entity.Property(e => e.ContactRef).IsUnicode(false);

                entity.Property(e => e.CorrespondenceAttachment).IsUnicode(false);

                entity.Property(e => e.CorrespondenceName).IsUnicode(false);

                entity.Property(e => e.CorrespondenceSubject).IsUnicode(false);

                entity.Property(e => e.EntityRef).IsUnicode(false);

                entity.Property(e => e.FssRequestMoa).IsUnicode(false);

                entity.Property(e => e.LenderTel).IsUnicode(false);

                entity.Property(e => e.MoaDisbsList).IsUnicode(false);

                entity.Property(e => e.MoaNextAction).IsUnicode(false);

                entity.Property(e => e.MoaRequired).IsUnicode(false);

                entity.Property(e => e.OceFundingEntity).IsUnicode(false);

                entity.Property(e => e.PostingSlipFeeDescription).IsUnicode(false);

                entity.Property(e => e.PostingSlipFeeType).IsUnicode(false);

                entity.Property(e => e.PostingSlipFeeTypeGroup).IsUnicode(false);

                entity.Property(e => e.PostingSlipPayeePayer).IsUnicode(false);

                entity.Property(e => e.PostingSlipPaymentMethod).IsUnicode(false);

                entity.Property(e => e.PostingSlipPaymentRef).IsUnicode(false);

                entity.Property(e => e.PostingSlipStatus).IsUnicode(false);

                entity.Property(e => e.PostingSlipSuffix).IsUnicode(false);

                entity.Property(e => e.PostingSlipTransactionType).IsUnicode(false);

                entity.Property(e => e.PreExchViewForms).IsUnicode(false);

                entity.Property(e => e.ProcessStep).IsUnicode(false);

                entity.Property(e => e.SelectedFeeInfo).IsUnicode(false);

                entity.Property(e => e.SolicitorContractText).IsUnicode(false);
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
