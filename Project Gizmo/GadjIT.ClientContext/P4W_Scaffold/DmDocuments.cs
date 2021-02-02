using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace GadjIT.ClientContext.P4W_Scaffold
{
    [Table("Dm_Documents")]
    public partial class DmDocuments
    {
        [Key]
        public int Code { get; set; }
        public int? CaseTypeGroupRef { get; set; }
        public int? DocumentType { get; set; }
        [StringLength(100)]
        public string Name { get; set; }
        [StringLength(100)]
        public string Notes { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? Created { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? DateAmended { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? DateReviewed { get; set; }
        [Column(TypeName = "money")]
        public decimal? FlatFee { get; set; }
        [Required]
        public bool? AlwaysSpecial { get; set; }
        [Required]
        public bool? Deleteble { get; set; }
        public int? IntsDiaryDays { get; set; }
        public int? FeeDiaryDays { get; set; }
        public int? PartDiaryDays { get; set; }
        public int? SupDiaryDays { get; set; }
        [StringLength(50)]
        public string InstDiaryText { get; set; }
        [StringLength(50)]
        public string FeeDiaryText { get; set; }
        [StringLength(50)]
        public string PartDiaryText { get; set; }
        [StringLength(50)]
        public string SupDiaryText { get; set; }
        public int? ReminderDocumentRef { get; set; }
        [StringLength(12)]
        public string CreatedByRef { get; set; }
        [StringLength(255)]
        public string Location { get; set; }
        [StringLength(3)]
        public string CanTake { get; set; }
        [Required]
        public bool? Actioned { get; set; }
        public int? DefaultMacro { get; set; }
        public int? Replicate { get; set; }
        public int? NoOfCopies { get; set; }
        [StringLength(6)]
        public string StepCategory { get; set; }
        public int? ChequeReq { get; set; }
        public int? Flags { get; set; }
        public int Duration { get; set; }
        public int DurationType { get; set; }
        public int? NoUnassignedGroupEntries { get; set; }
        public int? Critical { get; set; }
        public int? OutputType { get; set; }
        public byte? Mandatory { get; set; }
        public byte? MultiMatter { get; set; }
        public int? ActionOnComplete { get; set; }
        public int? ActionOnOverdue { get; set; }
        public byte? AutoTime { get; set; }
        public int? VersionControl { get; set; }
        [StringLength(255)]
        public string EmailTemplate { get; set; }
        public short? AutoEmail { get; set; }
        public int? ConsequenceRating { get; set; }
        [Column("SAMAction")]
        public int? Samaction { get; set; }
        [Column("SAMMacro")]
        public int? Sammacro { get; set; }
        public int? WebPublish { get; set; }
        public byte? TemplateEmail { get; set; }
        [Required]
        [StringLength(100)]
        public string UserHelp { get; set; }
        public byte DisableAutoPrint { get; set; }
        [StringLength(100)]
        public string DefaultProgressFile { get; set; }
        public byte ActionOnTake { get; set; }
        [Column("PDFStationerySetID")]
        public int PdfstationerySetId { get; set; }
        [Column("PDFHTMLEmailTemplate")]
        public int PdfhtmlemailTemplate { get; set; }
        public byte ShowTakePrintEmailPrompt { get; set; }
        [Required]
        [StringLength(1000)]
        public string RenameDesc { get; set; }
        public byte RenamePromptUser { get; set; }
        [Column("SAMPriority")]
        public int Sampriority { get; set; }
        [Column("MainClassID")]
        public int MainClassId { get; set; }
        [Column("SubClassID")]
        public int SubClassId { get; set; }
        public byte Locked { get; set; }
        public byte MandatoryDefaultMacro { get; set; }
        [Column("TakeToPDF")]
        public byte TakeToPdf { get; set; }
        public bool TheLinkApp { get; set; }
    }
}
