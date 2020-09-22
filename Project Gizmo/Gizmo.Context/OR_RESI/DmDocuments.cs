using System;
using System.Collections.Generic;

namespace Gizmo.Context.OR_RESI
{
    public partial class DmDocuments
    {
        public int Code { get; set; }
        public int? CaseTypeGroupRef { get; set; }
        public int? DocumentType { get; set; }
        public string Name { get; set; }
        public string Notes { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? DateAmended { get; set; }
        public DateTime? DateReviewed { get; set; }
        public decimal? FlatFee { get; set; }
        public bool? AlwaysSpecial { get; set; }
        public bool? Deleteble { get; set; }
        public int? IntsDiaryDays { get; set; }
        public int? FeeDiaryDays { get; set; }
        public int? PartDiaryDays { get; set; }
        public int? SupDiaryDays { get; set; }
        public string InstDiaryText { get; set; }
        public string FeeDiaryText { get; set; }
        public string PartDiaryText { get; set; }
        public string SupDiaryText { get; set; }
        public int? ReminderDocumentRef { get; set; }
        public string CreatedByRef { get; set; }
        public string Location { get; set; }
        public string CanTake { get; set; }
        public bool? Actioned { get; set; }
        public int? DefaultMacro { get; set; }
        public int? Replicate { get; set; }
        public int? NoOfCopies { get; set; }
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
        public string EmailTemplate { get; set; }
        public short? AutoEmail { get; set; }
        public int? ConsequenceRating { get; set; }
        public int? Samaction { get; set; }
        public int? Sammacro { get; set; }
        public int? WebPublish { get; set; }
        public byte? TemplateEmail { get; set; }
        public string UserHelp { get; set; }
        public byte DisableAutoPrint { get; set; }
        public string DefaultProgressFile { get; set; }
        public byte ActionOnTake { get; set; }
        public int PdfstationerySetId { get; set; }
        public int PdfhtmlemailTemplate { get; set; }
        public byte ShowTakePrintEmailPrompt { get; set; }
        public string RenameDesc { get; set; }
        public byte RenamePromptUser { get; set; }
        public int Sampriority { get; set; }
        public int MainClassId { get; set; }
        public int SubClassId { get; set; }
        public byte Locked { get; set; }
        public byte MandatoryDefaultMacro { get; set; }
        public byte TakeToPdf { get; set; }
        public bool TheLinkApp { get; set; }
    }
}
