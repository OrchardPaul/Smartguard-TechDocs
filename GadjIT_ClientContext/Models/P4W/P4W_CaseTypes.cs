using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace GadjIT_ClientContext.Models.P4W
{
    public class P4W_CaseTypes
    {
        [Key]
        public int Code { get; set; }
        [StringLength(5)]
        public string CodeName { get; set; }
        [StringLength(50)]
        public string Description { get; set; }
        public short? DefaultDestroyYears { get; set; }
        public int? CaseTypeGroupRef { get; set; }
        public int? NextFileNumber { get; set; }
        [StringLength(255)]
        public string ArchiveLocation { get; set; }
        public int? ArchiveMode { get; set; }
        public byte? Inactive { get; set; }
        public int? WebPublish { get; set; }
        [Column("SAMAllowed")]
        public byte? Samallowed { get; set; }
        [StringLength(50)]
        public string InitialAgendaName { get; set; }
        [StringLength(25)]
        public string IsokonCaseType { get; set; }
        [Column("max_consequence_rating")]
        public int? MaxConsequenceRating { get; set; }
        public int ManFieldsMore { get; set; }
        [Column(TypeName = "money")]
        public decimal ComplianceHighValue { get; set; }
        public int ArchiveDays { get; set; }
        [Column("EnableUTBMSCoding")]
        public byte EnableUtbmscoding { get; set; }
        [Column("CMPortalType")]
        public int CmportalType { get; set; }
        public bool EnableLitigation { get; set; }
        [Column("MPView")]
        [StringLength(255)]
        public string Mpview { get; set; }
        [Column("DefaultPASAgenda")]
        [StringLength(50)]
        public string DefaultPasagenda { get; set; }
        [Required]
        public bool? NetDocsSync { get; set; }
        [Required]
        public bool? ComplianceCentre { get; set; }
        public bool TheLinkApp { get; set; }
        [Column("DefaultUTBMSTaskTemplateID")]
        public int DefaultUtbmstaskTemplateId { get; set; }
        [Column("DefaultUTBMSPhaseTemplateID")]
        public int DefaultUtbmsphaseTemplateId { get; set; }
        [Column("DefaultUTBMSBudgetLevel")]
        public byte DefaultUtbmsbudgetLevel { get; set; }
        [Required]
        [Column("DefaultUTBMSCodeSet")]
        [StringLength(10)]
        public string DefaultUtbmscodeSet { get; set; }
        public int? RetentionMonths { get; set; }
        public bool AutoCreateCaseTheLinkApp { get; set; }
        [Column("EnableSDLT")]
        public bool EnableSdlt { get; set; }
    }
}
