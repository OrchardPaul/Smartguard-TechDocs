using System;
using System.Collections.Generic;

namespace Gizmo_V1._00.Models
{
    public partial class UsrOrResiMtAdmin
    {
        public string EntityRef { get; set; }
        public int MatterNo { get; set; }
        public string ContactFilterExists { get; set; }
        public int Id { get; set; }
        public string FssRequestMoa { get; set; }
        public decimal? RequestMoa { get; set; }
        public string MoaNextAction { get; set; }
        public string MoaRequired { get; set; }
        public string MoaDisbsList { get; set; }
        public string OceFundingEntity { get; set; }
        public decimal? OceFundingAmount { get; set; }
        public string PreExchViewForms { get; set; }
        public string SelectedFeeInfo { get; set; }
        public int? CcpProcessingAgenda { get; set; }
        public decimal? PostingSlipAmount { get; set; }
        public decimal? PostingSlipVat { get; set; }
        public decimal? PostingSlipTotal { get; set; }
        public DateTime? PostingSlipDateInserted { get; set; }
        public string PostingSlipFeeDescription { get; set; }
        public string PostingSlipSuffix { get; set; }
        public string PostingSlipFeeType { get; set; }
        public string PostingSlipPaymentMethod { get; set; }
        public string PostingSlipStatus { get; set; }
        public string PostingSlipTransactionType { get; set; }
        public string PostingSlipFeeTypeGroup { get; set; }
        public string PostingSlipPaymentRef { get; set; }
        public string PostingSlipPayeePayer { get; set; }
        public string ContactRef { get; set; }
        public string LenderTel { get; set; }
        public string SolicitorContractText { get; set; }
        public string CorrespondenceName { get; set; }
        public string CorrespondenceSubject { get; set; }
        public string CorrespondenceAttachment { get; set; }
        public string ProcessStep { get; set; }
    }
}
