using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gizmo.Context.OR_RESI
{
    [Table("Usr_OR_RESI_MT_Admin")]
    public partial class UsrOrResiMtAdmin
    {
        [Required]
        [StringLength(15)]
        public string EntityRef { get; set; }
        public int MatterNo { get; set; }
        [Column("Contact_Filter_Exists")]
        [StringLength(5)]
        public string ContactFilterExists { get; set; }
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Column("FSS_Request_MOA")]
        [StringLength(200)]
        public string FssRequestMoa { get; set; }
        [Column("Request_MOA", TypeName = "money")]
        public decimal? RequestMoa { get; set; }
        [Column("MOA_Next_Action")]
        [StringLength(30)]
        public string MoaNextAction { get; set; }
        [Column("MOA_Required")]
        [StringLength(200)]
        public string MoaRequired { get; set; }
        [Column("MOA_Disbs_List")]
        [StringLength(300)]
        public string MoaDisbsList { get; set; }
        [Column("OCE_Funding_Entity")]
        [StringLength(30)]
        public string OceFundingEntity { get; set; }
        [Column("OCE_Funding_Amount", TypeName = "money")]
        public decimal? OceFundingAmount { get; set; }
        [Column("PreExch_View_Forms")]
        [StringLength(500)]
        public string PreExchViewForms { get; set; }
        [Column("Selected_Fee_Info")]
        [StringLength(500)]
        public string SelectedFeeInfo { get; set; }
        [Column("CCP_Processing_Agenda")]
        public int? CcpProcessingAgenda { get; set; }
        [Column("PostingSlip_Amount", TypeName = "money")]
        public decimal? PostingSlipAmount { get; set; }
        [Column("PostingSlip_VAT", TypeName = "money")]
        public decimal? PostingSlipVat { get; set; }
        [Column("PostingSlip_Total", TypeName = "money")]
        public decimal? PostingSlipTotal { get; set; }
        [Column("PostingSlip_DateInserted", TypeName = "datetime")]
        public DateTime? PostingSlipDateInserted { get; set; }
        [Column("PostingSlip_FeeDescription")]
        [StringLength(300)]
        public string PostingSlipFeeDescription { get; set; }
        [Column("PostingSlip_Suffix")]
        [StringLength(200)]
        public string PostingSlipSuffix { get; set; }
        [Column("PostingSlip_FeeType")]
        [StringLength(50)]
        public string PostingSlipFeeType { get; set; }
        [Column("PostingSlip_PaymentMethod")]
        [StringLength(50)]
        public string PostingSlipPaymentMethod { get; set; }
        [Column("PostingSlip_Status")]
        [StringLength(50)]
        public string PostingSlipStatus { get; set; }
        [Column("PostingSlip_TransactionType")]
        [StringLength(30)]
        public string PostingSlipTransactionType { get; set; }
        [Column("PostingSlip_FeeTypeGroup")]
        [StringLength(30)]
        public string PostingSlipFeeTypeGroup { get; set; }
        [Column("PostingSlip_PaymentRef")]
        [StringLength(100)]
        public string PostingSlipPaymentRef { get; set; }
        [Column("PostingSlip_PayeePayer")]
        [StringLength(200)]
        public string PostingSlipPayeePayer { get; set; }
        [Column("Contact_Ref")]
        [StringLength(15)]
        public string ContactRef { get; set; }
        [Column("Lender_Tel")]
        [StringLength(15)]
        public string LenderTel { get; set; }
        [Column("Solicitor_Contract_Text")]
        public string SolicitorContractText { get; set; }
        [Column("Correspondence_Name")]
        [StringLength(200)]
        public string CorrespondenceName { get; set; }
        [Column("Correspondence_Subject")]
        [StringLength(500)]
        public string CorrespondenceSubject { get; set; }
        [Column("Correspondence_Attachment")]
        [StringLength(2000)]
        public string CorrespondenceAttachment { get; set; }
        [Column("Process_Step")]
        [StringLength(1)]
        public string ProcessStep { get; set; }
    }
}
