using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Gizmo_V1._00.Models;

namespace Gizmo_V1._00.Data
{
    public partial class P4W_OR_RESI_V5_DEVContext : DbContext
    {
        public P4W_OR_RESI_V5_DEVContext()
        {
        }

        public P4W_OR_RESI_V5_DEVContext(DbContextOptions<P4W_OR_RESI_V5_DEVContext> options)
            : base(options)
        {
        }

        public virtual DbSet<UsrOrResiMt> UsrOrResiMt { get; set; }
        public virtual DbSet<UsrOrResiMtAdmin> UsrOrResiMtAdmin { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Data Source=dev\\SQLEXPRESS;Initial Catalog=P4W_OR_RESI_V5_DEV;Integrated Security=True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UsrOrResiMt>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .IsClustered(false);

                entity.ToTable("Usr_OR_RESI_MT");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.AdditionalAccountNo)
                    .HasColumnName("Additional_Account_No")
                    .HasMaxLength(25)
                    .IsUnicode(false);

                entity.Property(e => e.AdditionalBranch)
                    .HasColumnName("Additional_Branch")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.AdditionalMoney)
                    .HasColumnName("Additional_Money")
                    .HasColumnType("money");

                entity.Property(e => e.AdditionalSortcode)
                    .HasColumnName("Additional_Sortcode")
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.AllowanceAmount)
                    .HasColumnName("Allowance_Amount")
                    .HasColumnType("money");

                entity.Property(e => e.AnticipatedCompletionDate)
                    .HasColumnName("Anticipated_Completion_Date")
                    .HasColumnType("datetime");

                entity.Property(e => e.BalanceDueToClient)
                    .HasColumnName("Balance_Due_To_Client")
                    .HasColumnType("money");

                entity.Property(e => e.BuyingThroughEstateAgent)
                    .HasColumnName("Buying_Through_Estate_Agent")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.CarePackStatus)
                    .HasColumnName("Care_Pack_Status")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.CaseOwner)
                    .HasColumnName("Case_Owner")
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.CompletionAlternativeAddress1)
                    .HasColumnName("Completion_Alternative_Address_1")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CompletionAlternativeAddress2)
                    .HasColumnName("Completion_Alternative_Address_2")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CompletionAlternativeAddress3)
                    .HasColumnName("Completion_Alternative_Address_3")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CompletionAlternativeAddress4)
                    .HasColumnName("Completion_Alternative_Address_4")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CompletionAlternativeAddress5)
                    .HasColumnName("Completion_Alternative_Address_5")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CompletionAlternativeAddressPc)
                    .HasColumnName("Completion_Alternative_Address_PC")
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.CompletionBalance)
                    .HasColumnName("Completion_Balance")
                    .HasColumnType("money");

                entity.Property(e => e.CompletionDate)
                    .HasColumnName("Completion_Date")
                    .HasColumnType("datetime");

                entity.Property(e => e.CompletionDisbs)
                    .HasColumnName("Completion_Disbs")
                    .HasColumnType("money");

                entity.Property(e => e.CompletionFees)
                    .HasColumnName("Completion_Fees")
                    .HasColumnType("money");

                entity.Property(e => e.CompletionFunds)
                    .HasColumnName("Completion_Funds")
                    .HasColumnType("money");

                entity.Property(e => e.CompletionOnNotice)
                    .HasColumnName("Completion_On_Notice")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.CompletionOtherCosts)
                    .HasColumnName("Completion_Other_Costs")
                    .HasColumnType("money");

                entity.Property(e => e.CompletionReceipts)
                    .HasColumnName("Completion_Receipts")
                    .HasColumnType("money");

                entity.Property(e => e.CompletionTime)
                    .HasColumnName("Completion_Time")
                    .HasColumnType("datetime");

                entity.Property(e => e.CompletionTotalCosts)
                    .HasColumnName("Completion_TotalCosts")
                    .HasColumnType("money");

                entity.Property(e => e.ContactRef)
                    .HasColumnName("Contact_Ref")
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.DataValidatedSuccessfully)
                    .HasColumnName("Data_Validated_Successfully")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.DateCarePackReceived)
                    .HasColumnName("Date_Care_Pack_Received")
                    .HasColumnType("datetime");

                entity.Property(e => e.DateFileClosed)
                    .HasColumnName("Date_File_Closed")
                    .HasColumnType("datetime");

                entity.Property(e => e.DefaultClient)
                    .HasColumnName("Default_Client")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.DepositAmount)
                    .HasColumnName("Deposit_Amount")
                    .HasColumnType("money");

                entity.Property(e => e.DepositIs10Percent)
                    .HasColumnName("Deposit_Is_10_Percent")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.DepositType)
                    .HasColumnName("Deposit_Type")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.DraftCompletionBalance)
                    .HasColumnName("Draft_Completion_Balance")
                    .HasColumnType("money");

                entity.Property(e => e.DraftCompletionFunds)
                    .HasColumnName("Draft_Completion_Funds")
                    .HasColumnType("money");

                entity.Property(e => e.DraftCompletionReceipts)
                    .HasColumnName("Draft_Completion_Receipts")
                    .HasColumnType("money");

                entity.Property(e => e.DraftCompletionTotalCosts)
                    .HasColumnName("Draft_Completion_TotalCosts")
                    .HasColumnType("money");

                entity.Property(e => e.EntityRef)
                    .IsRequired()
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.EstateAgentBillAmount)
                    .HasColumnName("Estate_Agent_Bill_Amount")
                    .HasColumnType("money");

                entity.Property(e => e.EstateAgentBillReceived)
                    .HasColumnName("Estate_Agent_Bill_Received")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.EstateAgentsRef)
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.ExchangeCallNotes)
                    .HasColumnName("Exchange_Call_Notes")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.ExchangeDate)
                    .HasColumnName("Exchange_Date")
                    .HasColumnType("datetime");

                entity.Property(e => e.ExchangeTime)
                    .HasColumnName("Exchange_Time")
                    .HasColumnType("datetime");

                entity.Property(e => e.Exchanged)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.JointClientSalutation)
                    .HasColumnName("Joint_Client_Salutation")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.JointOwnership)
                    .HasColumnName("Joint_Ownership")
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.LeaseholdPackChargeApplicable)
                    .HasColumnName("Leasehold_Pack_Charge_Applicable")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.LeaseholdPackFee)
                    .HasColumnName("Leasehold_Pack_Fee")
                    .HasColumnType("money");

                entity.Property(e => e.LeaseholdPackRequestedFrom)
                    .HasColumnName("Leasehold_Pack_Requested_From")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.LenderPurchase)
                    .HasColumnName("Lender_Purchase")
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.LenderPurchase1)
                    .HasColumnName("Lender_Purchase_1")
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.LenderSale)
                    .HasColumnName("Lender_Sale")
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.LenderSale1)
                    .HasColumnName("Lender_Sale_1")
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.LinkedCaseEntityRef)
                    .HasColumnName("Linked_Case_EntityRef")
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.LinkedCaseExists)
                    .HasColumnName("Linked_Case_Exists")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.LinkedCaseMatterNo).HasColumnName("Linked_Case_MatterNo");

                entity.Property(e => e.LinkedCasePurchasePrice)
                    .HasColumnName("Linked_Case_Purchase_Price")
                    .HasColumnType("money");

                entity.Property(e => e.LinkedCasePurchaseProceeds)
                    .HasColumnName("Linked_Case_Purchase_Proceeds")
                    .HasColumnType("money");

                entity.Property(e => e.ManagingAgentRef)
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.MortgageAdvanceAmount)
                    .HasColumnName("Mortgage_Advance_Amount")
                    .HasColumnType("money");

                entity.Property(e => e.MortgageInitialAmount)
                    .HasColumnName("Mortgage_Initial_Amount")
                    .HasColumnType("money");

                entity.Property(e => e.MortgageRedemption)
                    .HasColumnName("Mortgage_Redemption")
                    .HasColumnType("money");

                entity.Property(e => e.MortgageRedemptionDateFundsSentToLender)
                    .HasColumnName("Mortgage_Redemption_Date_Funds_Sent_to_Lender")
                    .HasColumnType("datetime");

                entity.Property(e => e.MortgageRedemptionExisitingMortgageToRedeem)
                    .HasColumnName("Mortgage_Redemption_Exisiting_Mortgage_to_Redeem")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.MortgageRedemptionPostingSlip)
                    .HasColumnName("Mortgage_Redemption_Posting_Slip")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.MortgageRequested)
                    .HasColumnName("Mortgage_Requested")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.NameOfPersonExchangedWith)
                    .HasColumnName("Name_of_Person_Exchanged_With")
                    .IsUnicode(false);

                entity.Property(e => e.Notes).IsUnicode(false);

                entity.Property(e => e.NotesCategory)
                    .HasColumnName("Notes_Category")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.OfficialCopyDate)
                    .HasColumnName("Official_Copy_Date")
                    .HasColumnType("datetime");

                entity.Property(e => e.OnHoldYn)
                    .HasColumnName("On_Hold_YN")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.PostingSlipMessage)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.PreciseTermsOfAgreedVariation)
                    .HasColumnName("Precise_Terms_of_Agreed_Variation")
                    .HasMaxLength(2000)
                    .IsUnicode(false);

                entity.Property(e => e.PreferredCompletionDate)
                    .HasColumnName("Preferred_Completion_Date")
                    .HasColumnType("datetime");

                entity.Property(e => e.PreferredContactMethodClient)
                    .HasColumnName("Preferred_Contact_Method_Client")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.PropertyRegistered)
                    .HasColumnName("Property_Registered")
                    .HasMaxLength(3)
                    .IsUnicode(false);

                entity.Property(e => e.PropertyToBeHeld)
                    .HasColumnName("Property_To_Be_Held")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.PurchasePrice)
                    .HasColumnName("Purchase_Price")
                    .HasColumnType("money");

                entity.Property(e => e.PurchaseProceeds)
                    .HasColumnName("Purchase_Proceeds")
                    .HasColumnType("money");

                entity.Property(e => e.QuestionnaireCompleted)
                    .HasColumnName("Questionnaire_Completed")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.RedStatNumberOutstanding).HasColumnName("RedStat_Number_Outstanding");

                entity.Property(e => e.RedStatNumberReceived).HasColumnName("RedStat_Number_Received");

                entity.Property(e => e.RedStatNumberRequested).HasColumnName("RedStat_Number_Requested");

                entity.Property(e => e.RedemptionNumberOutstanding).HasColumnName("Redemption_Number_Outstanding");

                entity.Property(e => e.RedemptionNumberReceived).HasColumnName("Redemption_Number_Received");

                entity.Property(e => e.RedemptionNumberRedeemed).HasColumnName("Redemption_Number_Redeemed");

                entity.Property(e => e.RedemptionNumberRequested).HasColumnName("Redemption_Number_Requested");

                entity.Property(e => e.RedemptionTotalNumberMortgagees).HasColumnName("Redemption_Total_Number_Mortgagees");

                entity.Property(e => e.RedemptionTotalValue)
                    .HasColumnName("Redemption_Total_Value")
                    .HasColumnType("money");

                entity.Property(e => e.RemainingTerm).HasColumnName("Remaining_Term");

                entity.Property(e => e.RestrictionHolder1)
                    .HasColumnName("Restriction_Holder_1")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.RestrictionHolder2)
                    .HasColumnName("Restriction_Holder_2")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.RestrictionHolderAddress1)
                    .HasColumnName("Restriction_Holder_Address_1")
                    .HasMaxLength(300)
                    .IsUnicode(false);

                entity.Property(e => e.RestrictionHolderAddress2)
                    .HasColumnName("Restriction_Holder_Address_2")
                    .HasMaxLength(300)
                    .IsUnicode(false);

                entity.Property(e => e.RetentionAmount)
                    .HasColumnName("Retention_Amount")
                    .HasColumnType("money");

                entity.Property(e => e.SalePrice)
                    .HasColumnName("Sale_Price")
                    .HasColumnType("money");

                entity.Property(e => e.SaleProceeds)
                    .HasColumnName("Sale_Proceeds")
                    .HasColumnType("money");

                entity.Property(e => e.SameDayCompletion)
                    .HasColumnName("Same_Day_Completion")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.SatisfactoryEvidenceReceived)
                    .HasColumnName("Satisfactory_Evidence_Received")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.SearchAgentEmailAddress)
                    .HasColumnName("Search_Agent_Email_Address")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.SearchProviderRef)
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.SellerRef)
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.SlipPayInMethodAdvance)
                    .HasColumnName("Slip_PayIn_Method_Advance")
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.SlipPayInMethodSaleProceeds)
                    .HasColumnName("Slip_PayIn_Method_SaleProceeds")
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.SlipPayOutMethodBalance)
                    .HasColumnName("Slip_PayOut_Method_Balance")
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.SlipPayOutMethodDeposit)
                    .HasColumnName("Slip_PayOut_Method_Deposit")
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.SlipPayOutMethodRedemption)
                    .HasColumnName("Slip_PayOut_Method_Redemption")
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.Solicitorsref)
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.SourceOfFunds)
                    .HasColumnName("Source_Of_Funds")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.SourceOfIntroductionRef)
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.StandardIndemnityCovenant)
                    .HasColumnName("Standard_Indemnity_Covenant")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.TenDayCoolingOffPeriodWaivered)
                    .HasColumnName("Ten_Day_Cooling_Off_Period_Waivered")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.TitleNumber)
                    .HasColumnName("Title_Number")
                    .HasMaxLength(300)
                    .IsUnicode(false);

                entity.Property(e => e.TitleNumberSecondTitle)
                    .HasColumnName("Title_Number_Second_Title")
                    .HasMaxLength(300)
                    .IsUnicode(false);

                entity.Property(e => e.TitleNumberThirdTitle)
                    .HasColumnName("Title_Number_Third_Title")
                    .HasMaxLength(300)
                    .IsUnicode(false);

                entity.Property(e => e.TotalApportionments)
                    .HasColumnName("Total_Apportionments")
                    .HasColumnType("money");

                entity.Property(e => e.TotalApportionmentsOverpaid)
                    .HasColumnName("Total_Apportionments_Overpaid")
                    .HasColumnType("money");

                entity.Property(e => e.TotalApportionmentsUnderpaid)
                    .HasColumnName("Total_Apportionments_Underpaid")
                    .HasColumnType("money");

                entity.Property(e => e.TotalChattels)
                    .HasColumnName("Total_Chattels")
                    .HasColumnType("money");

                entity.Property(e => e.TotalDisbs)
                    .HasColumnName("Total_Disbs")
                    .HasColumnType("money");

                entity.Property(e => e.TotalDisbsAnticipated)
                    .HasColumnName("Total_Disbs_Anticipated")
                    .HasColumnType("money");

                entity.Property(e => e.TotalDisbsPaid)
                    .HasColumnName("Total_Disbs_Paid")
                    .HasColumnType("money");

                entity.Property(e => e.TotalDisbsUnpaid)
                    .HasColumnName("Total_Disbs_Unpaid")
                    .HasColumnType("money");

                entity.Property(e => e.TotalFees)
                    .HasColumnName("Total_Fees")
                    .HasColumnType("money");

                entity.Property(e => e.TotalFeesPaid)
                    .HasColumnName("Total_Fees_Paid")
                    .HasColumnType("money");

                entity.Property(e => e.TotalFeesUnpaid)
                    .HasColumnName("Total_Fees_Unpaid")
                    .HasColumnType("money");

                entity.Property(e => e.TotalFundsAvailable)
                    .HasColumnName("Total_Funds_Available")
                    .HasColumnType("money");

                entity.Property(e => e.TotalFundsCleared)
                    .HasColumnName("Total_Funds_Cleared")
                    .HasColumnType("money");

                entity.Property(e => e.TotalFundsPending)
                    .HasColumnName("Total_Funds_Pending")
                    .HasColumnType("money");

                entity.Property(e => e.TotalFundsReceived)
                    .HasColumnName("Total_Funds_Received")
                    .HasColumnType("money");

                entity.Property(e => e.TotalFundsShortfall)
                    .HasColumnName("Total_Funds_Shortfall")
                    .HasColumnType("money");

                entity.Property(e => e.TotalMoa)
                    .HasColumnName("Total_MOA")
                    .HasColumnType("money");

                entity.Property(e => e.TotalMoaForDisbs)
                    .HasColumnName("Total_MOA_For_Disbs")
                    .HasColumnType("money");

                entity.Property(e => e.TotalSearches)
                    .HasColumnName("Total_Searches")
                    .HasColumnType("money");

                entity.Property(e => e.TotalSearchesPaid)
                    .HasColumnName("Total_Searches_Paid")
                    .HasColumnType("money");

                entity.Property(e => e.TotalSearchesUnpaid)
                    .HasColumnName("Total_Searches_Unpaid")
                    .HasColumnType("money");

                entity.Property(e => e.WriteToClient2Separately)
                    .HasColumnName("Write_To_Client_2_Separately")
                    .HasMaxLength(20)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<UsrOrResiMtAdmin>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .IsClustered(false);

                entity.ToTable("Usr_OR_RESI_MT_Admin");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CcpProcessingAgenda).HasColumnName("CCP_Processing_Agenda");

                entity.Property(e => e.ContactFilterExists)
                    .HasColumnName("Contact_Filter_Exists")
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.ContactRef)
                    .HasColumnName("Contact_Ref")
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.CorrespondenceAttachment)
                    .HasColumnName("Correspondence_Attachment")
                    .HasMaxLength(2000)
                    .IsUnicode(false);

                entity.Property(e => e.CorrespondenceName)
                    .HasColumnName("Correspondence_Name")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.CorrespondenceSubject)
                    .HasColumnName("Correspondence_Subject")
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.EntityRef)
                    .IsRequired()
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.FssRequestMoa)
                    .HasColumnName("FSS_Request_MOA")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.LenderTel)
                    .HasColumnName("Lender_Tel")
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.MoaDisbsList)
                    .HasColumnName("MOA_Disbs_List")
                    .HasMaxLength(300)
                    .IsUnicode(false);

                entity.Property(e => e.MoaNextAction)
                    .HasColumnName("MOA_Next_Action")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.MoaRequired)
                    .HasColumnName("MOA_Required")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.OceFundingAmount)
                    .HasColumnName("OCE_Funding_Amount")
                    .HasColumnType("money");

                entity.Property(e => e.OceFundingEntity)
                    .HasColumnName("OCE_Funding_Entity")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.PostingSlipAmount)
                    .HasColumnName("PostingSlip_Amount")
                    .HasColumnType("money");

                entity.Property(e => e.PostingSlipDateInserted)
                    .HasColumnName("PostingSlip_DateInserted")
                    .HasColumnType("datetime");

                entity.Property(e => e.PostingSlipFeeDescription)
                    .HasColumnName("PostingSlip_FeeDescription")
                    .HasMaxLength(300)
                    .IsUnicode(false);

                entity.Property(e => e.PostingSlipFeeType)
                    .HasColumnName("PostingSlip_FeeType")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.PostingSlipFeeTypeGroup)
                    .HasColumnName("PostingSlip_FeeTypeGroup")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.PostingSlipPayeePayer)
                    .HasColumnName("PostingSlip_PayeePayer")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.PostingSlipPaymentMethod)
                    .HasColumnName("PostingSlip_PaymentMethod")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.PostingSlipPaymentRef)
                    .HasColumnName("PostingSlip_PaymentRef")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.PostingSlipStatus)
                    .HasColumnName("PostingSlip_Status")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.PostingSlipSuffix)
                    .HasColumnName("PostingSlip_Suffix")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.PostingSlipTotal)
                    .HasColumnName("PostingSlip_Total")
                    .HasColumnType("money");

                entity.Property(e => e.PostingSlipTransactionType)
                    .HasColumnName("PostingSlip_TransactionType")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.PostingSlipVat)
                    .HasColumnName("PostingSlip_VAT")
                    .HasColumnType("money");

                entity.Property(e => e.PreExchViewForms)
                    .HasColumnName("PreExch_View_Forms")
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.ProcessStep)
                    .HasColumnName("Process_Step")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.RequestMoa)
                    .HasColumnName("Request_MOA")
                    .HasColumnType("money");

                entity.Property(e => e.SelectedFeeInfo)
                    .HasColumnName("Selected_Fee_Info")
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.SolicitorContractText)
                    .HasColumnName("Solicitor_Contract_Text")
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
