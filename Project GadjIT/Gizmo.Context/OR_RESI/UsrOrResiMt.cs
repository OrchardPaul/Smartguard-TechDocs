using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gizmo.Context.OR_RESI
{
    [Table("Usr_OR_RESI_MT")]
    public partial class UsrOrResiMt
    {
        [Required]
        [StringLength(15)]
        public string EntityRef { get; set; }
        public int MatterNo { get; set; }
        [Column("Data_Validated_Successfully")]
        [StringLength(1)]
        public string DataValidatedSuccessfully { get; set; }
        [Column("Joint_Client_Salutation")]
        [StringLength(200)]
        public string JointClientSalutation { get; set; }
        [Column("Joint_Ownership")]
        [StringLength(5)]
        public string JointOwnership { get; set; }
        [Column("Care_Pack_Status")]
        [StringLength(30)]
        public string CarePackStatus { get; set; }
        [Column("Date_Care_Pack_Received", TypeName = "datetime")]
        public DateTime? DateCarePackReceived { get; set; }
        [Column("Write_To_Client_2_Separately")]
        [StringLength(20)]
        public string WriteToClient2Separately { get; set; }
        [Column("Search_Agent_Email_Address")]
        [StringLength(200)]
        public string SearchAgentEmailAddress { get; set; }
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [StringLength(15)]
        public string EstateAgentsRef { get; set; }
        [StringLength(15)]
        public string Solicitorsref { get; set; }
        [StringLength(15)]
        public string SellerRef { get; set; }
        [StringLength(15)]
        public string SearchProviderRef { get; set; }
        [StringLength(15)]
        public string ManagingAgentRef { get; set; }
        [StringLength(15)]
        public string SourceOfIntroductionRef { get; set; }
        [Column("Total_Fees", TypeName = "money")]
        public decimal? TotalFees { get; set; }
        [Column("Total_Fees_Paid", TypeName = "money")]
        public decimal? TotalFeesPaid { get; set; }
        [Column("Total_Fees_Unpaid", TypeName = "money")]
        public decimal? TotalFeesUnpaid { get; set; }
        [Column("Anticipated_Completion_Date", TypeName = "datetime")]
        public DateTime? AnticipatedCompletionDate { get; set; }
        [Column("Exchange_Date", TypeName = "datetime")]
        public DateTime? ExchangeDate { get; set; }
        [Column("Name_of_Person_Exchanged_With")]
        public string NameOfPersonExchangedWith { get; set; }
        [Column("Completion_On_Notice")]
        [StringLength(1)]
        public string CompletionOnNotice { get; set; }
        [StringLength(1)]
        public string Exchanged { get; set; }
        [Column("Exchange_Call_Notes")]
        [StringLength(250)]
        public string ExchangeCallNotes { get; set; }
        [Column("Preferred_Completion_Date", TypeName = "datetime")]
        public DateTime? PreferredCompletionDate { get; set; }
        [Column("Total_Chattels", TypeName = "money")]
        public decimal? TotalChattels { get; set; }
        [Column("Total_MOA", TypeName = "money")]
        public decimal? TotalMoa { get; set; }
        [Column("Total_Apportionments", TypeName = "money")]
        public decimal? TotalApportionments { get; set; }
        [Column("Total_Disbs", TypeName = "money")]
        public decimal? TotalDisbs { get; set; }
        [Column("Total_Disbs_Paid", TypeName = "money")]
        public decimal? TotalDisbsPaid { get; set; }
        [Column("Total_Disbs_Unpaid", TypeName = "money")]
        public decimal? TotalDisbsUnpaid { get; set; }
        [Column("Total_Searches", TypeName = "money")]
        public decimal? TotalSearches { get; set; }
        [Column("Total_Searches_Paid", TypeName = "money")]
        public decimal? TotalSearchesPaid { get; set; }
        [Column("Total_Searches_Unpaid", TypeName = "money")]
        public decimal? TotalSearchesUnpaid { get; set; }
        [StringLength(200)]
        public string PostingSlipMessage { get; set; }
        [Column("Linked_Case_Exists")]
        [StringLength(1)]
        public string LinkedCaseExists { get; set; }
        [Column("Total_Apportionments_Overpaid", TypeName = "money")]
        public decimal? TotalApportionmentsOverpaid { get; set; }
        [Column("Total_Apportionments_Underpaid", TypeName = "money")]
        public decimal? TotalApportionmentsUnderpaid { get; set; }
        [Column("Completion_Date", TypeName = "datetime")]
        public DateTime? CompletionDate { get; set; }
        [Column("Completion_Fees", TypeName = "money")]
        public decimal? CompletionFees { get; set; }
        [Column("Completion_Disbs", TypeName = "money")]
        public decimal? CompletionDisbs { get; set; }
        [Column("Completion_Other_Costs", TypeName = "money")]
        public decimal? CompletionOtherCosts { get; set; }
        [Column("Completion_Receipts", TypeName = "money")]
        public decimal? CompletionReceipts { get; set; }
        [Column("Completion_Balance", TypeName = "money")]
        public decimal? CompletionBalance { get; set; }
        [Column("Total_Funds_Available", TypeName = "money")]
        public decimal? TotalFundsAvailable { get; set; }
        [Column("Total_MOA_For_Disbs", TypeName = "money")]
        public decimal? TotalMoaForDisbs { get; set; }
        [Column("Total_Funds_Shortfall", TypeName = "money")]
        public decimal? TotalFundsShortfall { get; set; }
        [Column("Completion_TotalCosts", TypeName = "money")]
        public decimal? CompletionTotalCosts { get; set; }
        [Column("Mortgage_Advance_Amount", TypeName = "money")]
        public decimal? MortgageAdvanceAmount { get; set; }
        [Column("Mortgage_Redemption", TypeName = "money")]
        public decimal? MortgageRedemption { get; set; }
        [Column("Sale_Proceeds", TypeName = "money")]
        public decimal? SaleProceeds { get; set; }
        [Column("Deposit_Amount", TypeName = "money")]
        public decimal? DepositAmount { get; set; }
        [Column("Mortgage_Redemption_Posting_Slip")]
        [StringLength(1)]
        public string MortgageRedemptionPostingSlip { get; set; }
        [Column("Mortgage_Redemption_Date_Funds_Sent_to_Lender", TypeName = "datetime")]
        public DateTime? MortgageRedemptionDateFundsSentToLender { get; set; }
        [Column("Purchase_Price", TypeName = "money")]
        public decimal? PurchasePrice { get; set; }
        [Column("Sale_Price", TypeName = "money")]
        public decimal? SalePrice { get; set; }
        [Column("Retention_Amount", TypeName = "money")]
        public decimal? RetentionAmount { get; set; }
        [Column("Mortgage_Redemption_Exisiting_Mortgage_to_Redeem")]
        [StringLength(1)]
        public string MortgageRedemptionExisitingMortgageToRedeem { get; set; }
        [Column("Lender_Purchase")]
        [StringLength(15)]
        public string LenderPurchase { get; set; }
        [Column("Lender_Sale")]
        [StringLength(15)]
        public string LenderSale { get; set; }
        [Column("Lender_Sale_1")]
        [StringLength(15)]
        public string LenderSale1 { get; set; }
        [Column("Lender_Purchase_1")]
        [StringLength(15)]
        public string LenderPurchase1 { get; set; }
        [Column("Contact_Ref")]
        [StringLength(15)]
        public string ContactRef { get; set; }
        [Column("Deposit_Is_10_Percent")]
        [StringLength(1)]
        public string DepositIs10Percent { get; set; }
        [Column("Deposit_Type")]
        [StringLength(20)]
        public string DepositType { get; set; }
        [Column("Slip_PayOut_Method_Deposit")]
        [StringLength(15)]
        public string SlipPayOutMethodDeposit { get; set; }
        [Column("Slip_PayOut_Method_Balance")]
        [StringLength(15)]
        public string SlipPayOutMethodBalance { get; set; }
        [Column("Slip_PayOut_Method_Redemption")]
        [StringLength(15)]
        public string SlipPayOutMethodRedemption { get; set; }
        [Column("Slip_PayIn_Method_Advance")]
        [StringLength(15)]
        public string SlipPayInMethodAdvance { get; set; }
        [Column("Slip_PayIn_Method_SaleProceeds")]
        [StringLength(15)]
        public string SlipPayInMethodSaleProceeds { get; set; }
        [Column("Completion_Funds", TypeName = "money")]
        public decimal? CompletionFunds { get; set; }
        [Column("On_Hold_YN")]
        [StringLength(1)]
        public string OnHoldYn { get; set; }
        [Column("Total_Funds_Pending", TypeName = "money")]
        public decimal? TotalFundsPending { get; set; }
        [Column("Total_Funds_Received", TypeName = "money")]
        public decimal? TotalFundsReceived { get; set; }
        [Column("Total_Funds_Cleared", TypeName = "money")]
        public decimal? TotalFundsCleared { get; set; }
        [Column("Total_Disbs_Anticipated", TypeName = "money")]
        public decimal? TotalDisbsAnticipated { get; set; }
        [Column("Ten_Day_Cooling_Off_Period_Waivered")]
        [StringLength(1)]
        public string TenDayCoolingOffPeriodWaivered { get; set; }
        [Column("Estate_Agent_Bill_Amount", TypeName = "money")]
        public decimal? EstateAgentBillAmount { get; set; }
        [Column("Estate_Agent_Bill_Received")]
        [StringLength(1)]
        public string EstateAgentBillReceived { get; set; }
        [Column("Leasehold_Pack_Fee", TypeName = "money")]
        public decimal? LeaseholdPackFee { get; set; }
        [Column("Leasehold_Pack_Charge_Applicable")]
        [StringLength(20)]
        public string LeaseholdPackChargeApplicable { get; set; }
        [Column("Leasehold_Pack_Requested_From")]
        [StringLength(20)]
        public string LeaseholdPackRequestedFrom { get; set; }
        [Column("Redemption_Total_Number_Mortgagees")]
        public int? RedemptionTotalNumberMortgagees { get; set; }
        [Column("Redemption_Number_Requested")]
        public int? RedemptionNumberRequested { get; set; }
        [Column("Redemption_Number_Received")]
        public int? RedemptionNumberReceived { get; set; }
        [Column("Redemption_Number_Outstanding")]
        public int? RedemptionNumberOutstanding { get; set; }
        [Column("Redemption_Total_Value", TypeName = "money")]
        public decimal? RedemptionTotalValue { get; set; }
        [Column("RedStat_Number_Requested")]
        public int? RedStatNumberRequested { get; set; }
        [Column("RedStat_Number_Received")]
        public int? RedStatNumberReceived { get; set; }
        [Column("RedStat_Number_Outstanding")]
        public int? RedStatNumberOutstanding { get; set; }
        [Column("Redemption_Number_Redeemed")]
        public int? RedemptionNumberRedeemed { get; set; }
        [Column("Completion_Alternative_Address_1")]
        [StringLength(50)]
        public string CompletionAlternativeAddress1 { get; set; }
        [Column("Completion_Alternative_Address_2")]
        [StringLength(50)]
        public string CompletionAlternativeAddress2 { get; set; }
        [Column("Completion_Alternative_Address_3")]
        [StringLength(50)]
        public string CompletionAlternativeAddress3 { get; set; }
        [Column("Completion_Alternative_Address_4")]
        [StringLength(50)]
        public string CompletionAlternativeAddress4 { get; set; }
        [Column("Completion_Alternative_Address_PC")]
        [StringLength(10)]
        public string CompletionAlternativeAddressPc { get; set; }
        [Column("Default_Client")]
        [StringLength(1)]
        public string DefaultClient { get; set; }
        [Column("Linked_Case_EntityRef")]
        [StringLength(15)]
        public string LinkedCaseEntityRef { get; set; }
        [Column("Linked_Case_MatterNo")]
        public int? LinkedCaseMatterNo { get; set; }
        [Column("Preferred_Contact_Method_Client")]
        [StringLength(20)]
        public string PreferredContactMethodClient { get; set; }
        [Column("Remaining_Term")]
        public int? RemainingTerm { get; set; }
        public string Notes { get; set; }
        [Column("Notes_Category")]
        [StringLength(50)]
        public string NotesCategory { get; set; }
        [Column("Date_File_Closed", TypeName = "datetime")]
        public DateTime? DateFileClosed { get; set; }
        [Column("Completion_Time", TypeName = "datetime")]
        public DateTime? CompletionTime { get; set; }
        [Column("Linked_Case_Purchase_Price", TypeName = "money")]
        public decimal? LinkedCasePurchasePrice { get; set; }
        [Column("Purchase_Proceeds", TypeName = "money")]
        public decimal? PurchaseProceeds { get; set; }
        [Column("Balance_Due_To_Client", TypeName = "money")]
        public decimal? BalanceDueToClient { get; set; }
        [Column("Linked_Case_Purchase_Proceeds", TypeName = "money")]
        public decimal? LinkedCasePurchaseProceeds { get; set; }
        [Column("Title_Number")]
        [StringLength(300)]
        public string TitleNumber { get; set; }
        [Column("Case_Owner")]
        [StringLength(10)]
        public string CaseOwner { get; set; }
        [Column("Property_Registered")]
        [StringLength(3)]
        public string PropertyRegistered { get; set; }
        [Column("Title_Number_Third_Title")]
        [StringLength(300)]
        public string TitleNumberThirdTitle { get; set; }
        [Column("Title_Number_Second_Title")]
        [StringLength(300)]
        public string TitleNumberSecondTitle { get; set; }
        [Column("Restriction_Holder_1")]
        [StringLength(100)]
        public string RestrictionHolder1 { get; set; }
        [Column("Restriction_Holder_Address_1")]
        [StringLength(300)]
        public string RestrictionHolderAddress1 { get; set; }
        [Column("Restriction_Holder_Address_2")]
        [StringLength(300)]
        public string RestrictionHolderAddress2 { get; set; }
        [Column("Restriction_Holder_2")]
        [StringLength(100)]
        public string RestrictionHolder2 { get; set; }
        [Column("Additional_Account_No")]
        [StringLength(25)]
        public string AdditionalAccountNo { get; set; }
        [Column("Additional_Money", TypeName = "money")]
        public decimal? AdditionalMoney { get; set; }
        [Column("Additional_Sortcode")]
        [StringLength(10)]
        public string AdditionalSortcode { get; set; }
        [Column("Additional_Branch")]
        [StringLength(100)]
        public string AdditionalBranch { get; set; }
        [Column("Questionnaire_Completed")]
        [StringLength(1)]
        public string QuestionnaireCompleted { get; set; }
        [Column("Same_Day_Completion")]
        [StringLength(1)]
        public string SameDayCompletion { get; set; }
        [Column("Mortgage_Initial_Amount", TypeName = "money")]
        public decimal? MortgageInitialAmount { get; set; }
        [Column("Source_Of_Funds")]
        [StringLength(100)]
        public string SourceOfFunds { get; set; }
        [Column("Satisfactory_Evidence_Received")]
        [StringLength(1)]
        public string SatisfactoryEvidenceReceived { get; set; }
        [Column("Mortgage_Requested")]
        [StringLength(1)]
        public string MortgageRequested { get; set; }
        [Column("Property_To_Be_Held")]
        [StringLength(100)]
        public string PropertyToBeHeld { get; set; }
        [Column("Standard_Indemnity_Covenant")]
        [StringLength(1)]
        public string StandardIndemnityCovenant { get; set; }
        [Column("Buying_Through_Estate_Agent")]
        [StringLength(1)]
        public string BuyingThroughEstateAgent { get; set; }
        [Column("Draft_Completion_Receipts", TypeName = "money")]
        public decimal? DraftCompletionReceipts { get; set; }
        [Column("Draft_Completion_Funds", TypeName = "money")]
        public decimal? DraftCompletionFunds { get; set; }
        [Column("Draft_Completion_TotalCosts", TypeName = "money")]
        public decimal? DraftCompletionTotalCosts { get; set; }
        [Column("Draft_Completion_Balance", TypeName = "money")]
        public decimal? DraftCompletionBalance { get; set; }
        [Column("Exchange_Time", TypeName = "datetime")]
        public DateTime? ExchangeTime { get; set; }
        [Column("Precise_Terms_of_Agreed_Variation")]
        [StringLength(2000)]
        public string PreciseTermsOfAgreedVariation { get; set; }
        [Column("Completion_Alternative_Address_5")]
        [StringLength(50)]
        public string CompletionAlternativeAddress5 { get; set; }
        [Column("Official_Copy_Date", TypeName = "datetime")]
        public DateTime? OfficialCopyDate { get; set; }
        [Column("Allowance_Amount", TypeName = "money")]
        public decimal? AllowanceAmount { get; set; }
    }
}
