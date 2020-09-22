using System;
using System.Collections.Generic;

namespace Gizmo.Context.OR_RESI
{
    public partial class CaseTypes
    {
        public int Code { get; set; }
        public string CodeName { get; set; }
        public string Description { get; set; }
        public short? DefaultDestroyYears { get; set; }
        public int? CaseTypeGroupRef { get; set; }
        public int? NextFileNumber { get; set; }
        public string ArchiveLocation { get; set; }
        public int? ArchiveMode { get; set; }
        public byte? Inactive { get; set; }
        public int? WebPublish { get; set; }
        public byte? Samallowed { get; set; }
        public string InitialAgendaName { get; set; }
        public string IsokonCaseType { get; set; }
        public int? MaxConsequenceRating { get; set; }
        public int ManFieldsMore { get; set; }
        public decimal ComplianceHighValue { get; set; }
        public int ArchiveDays { get; set; }
        public byte EnableUtbmscoding { get; set; }
        public int CmportalType { get; set; }
        public bool EnableLitigation { get; set; }
        public string Mpview { get; set; }
        public string DefaultPasagenda { get; set; }
        public bool? NetDocsSync { get; set; }
        public bool? ComplianceCentre { get; set; }
        public bool TheLinkApp { get; set; }
        public int DefaultUtbmstaskTemplateId { get; set; }
        public int DefaultUtbmsphaseTemplateId { get; set; }
        public byte DefaultUtbmsbudgetLevel { get; set; }
        public string DefaultUtbmscodeSet { get; set; }
        public int? RetentionMonths { get; set; }
        public bool AutoCreateCaseTheLinkApp { get; set; }
        public bool EnableSdlt { get; set; }
    }
}
