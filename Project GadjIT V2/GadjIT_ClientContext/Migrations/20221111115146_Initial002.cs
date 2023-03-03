using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GadjIT_ClientContext_Proj.Migrations
{
    public partial class Initial002 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // migrationBuilder.CreateTable(
            //     name: "CaseTypeGroups",
            //     columns: table => new
            //     {
            //         ID = table.Column<int>(type: "int", nullable: false)
            //             .Annotation("SqlServer:Identity", "1, 1"),
            //         Name = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true)
            //     },
            //     constraints: table =>
            //     {
            //         table.PrimaryKey("PK_CaseTypeGroups", x => x.ID)
            //             .Annotation("SqlServer:Clustered", false);
            //     });

        //     migrationBuilder.CreateTable(
        //         name: "CaseTypes",
        //         columns: table => new
        //         {
        //             Code = table.Column<int>(type: "int", nullable: false)
        //                 .Annotation("SqlServer:Identity", "1, 1"),
        //             CodeName = table.Column<string>(type: "varchar(5)", unicode: false, maxLength: 5, nullable: true),
        //             Description = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
        //             DefaultDestroyYears = table.Column<short>(type: "smallint", nullable: true, defaultValueSql: "(0)"),
        //             CaseTypeGroupRef = table.Column<int>(type: "int", nullable: true, defaultValueSql: "(0)"),
        //             NextFileNumber = table.Column<int>(type: "int", nullable: true, defaultValueSql: "(1)"),
        //             ArchiveLocation = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true, defaultValueSql: "(null)"),
        //             ArchiveMode = table.Column<int>(type: "int", nullable: true, defaultValueSql: "(0)"),
        //             Inactive = table.Column<byte>(type: "tinyint", nullable: true, defaultValueSql: "(0)"),
        //             WebPublish = table.Column<int>(type: "int", nullable: true),
        //             SAMAllowed = table.Column<byte>(type: "tinyint", nullable: true, defaultValueSql: "(0)"),
        //             InitialAgendaName = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
        //             IsokonCaseType = table.Column<string>(type: "varchar(25)", unicode: false, maxLength: 25, nullable: true),
        //             max_consequence_rating = table.Column<int>(type: "int", nullable: true, defaultValueSql: "(0)"),
        //             ManFieldsMore = table.Column<int>(type: "int", nullable: false),
        //             ComplianceHighValue = table.Column<decimal>(type: "money", nullable: false, defaultValueSql: "((0.00))"),
        //             ArchiveDays = table.Column<int>(type: "int", nullable: false),
        //             EnableUTBMSCoding = table.Column<byte>(type: "tinyint", nullable: false),
        //             CMPortalType = table.Column<int>(type: "int", nullable: false),
        //             EnableLitigation = table.Column<bool>(type: "bit", nullable: false),
        //             MPView = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
        //             DefaultPASAgenda = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
        //             NetDocsSync = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "((1))"),
        //             ComplianceCentre = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "((1))"),
        //             TheLinkApp = table.Column<bool>(type: "bit", nullable: false),
        //             DefaultUTBMSTaskTemplateID = table.Column<int>(type: "int", nullable: false),
        //             DefaultUTBMSPhaseTemplateID = table.Column<int>(type: "int", nullable: false),
        //             DefaultUTBMSBudgetLevel = table.Column<byte>(type: "tinyint", nullable: false),
        //             DefaultUTBMSCodeSet = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false, defaultValueSql: "('')"),
        //             RetentionMonths = table.Column<int>(type: "int", nullable: true),
        //             AutoCreateCaseTheLinkApp = table.Column<bool>(type: "bit", nullable: false),
        //             EnableSDLT = table.Column<bool>(type: "bit", nullable: false)
        //         },
        //         constraints: table =>
        //         {
        //             table.PrimaryKey("PK_CaseTypes", x => x.Code)
        //                 .Annotation("SqlServer:Clustered", false);
        //         });

        //     migrationBuilder.CreateTable(
        //         name: "Dm_Documents",
        //         columns: table => new
        //         {
        //             Code = table.Column<int>(type: "int", nullable: false)
        //                 .Annotation("SqlServer:Identity", "1, 1"),
        //             CaseTypeGroupRef = table.Column<int>(type: "int", nullable: true, defaultValueSql: "(0)"),
        //             DocumentType = table.Column<int>(type: "int", nullable: true, defaultValueSql: "(0)"),
        //             Name = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
        //             Notes = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
        //             Created = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
        //             DateAmended = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
        //             DateReviewed = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
        //             FlatFee = table.Column<decimal>(type: "money", nullable: true, defaultValueSql: "(0)"),
        //             AlwaysSpecial = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "(0)"),
        //             Deleteble = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "(1)"),
        //             IntsDiaryDays = table.Column<int>(type: "int", nullable: true, defaultValueSql: "(0)"),
        //             FeeDiaryDays = table.Column<int>(type: "int", nullable: true, defaultValueSql: "(0)"),
        //             PartDiaryDays = table.Column<int>(type: "int", nullable: true, defaultValueSql: "(0)"),
        //             SupDiaryDays = table.Column<int>(type: "int", nullable: true, defaultValueSql: "(0)"),
        //             InstDiaryText = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
        //             FeeDiaryText = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
        //             PartDiaryText = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
        //             SupDiaryText = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
        //             ReminderDocumentRef = table.Column<int>(type: "int", nullable: true, defaultValueSql: "(0)"),
        //             CreatedByRef = table.Column<string>(type: "varchar(12)", unicode: false, maxLength: 12, nullable: true),
        //             Location = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
        //             CanTake = table.Column<string>(type: "varchar(3)", unicode: false, maxLength: 3, nullable: true),
        //             Actioned = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "(0)"),
        //             DefaultMacro = table.Column<int>(type: "int", nullable: true, defaultValueSql: "(0)"),
        //             Replicate = table.Column<int>(type: "int", nullable: true, defaultValueSql: "(1)"),
        //             NoOfCopies = table.Column<int>(type: "int", nullable: true, defaultValueSql: "(1)"),
        //             StepCategory = table.Column<string>(type: "varchar(6)", unicode: false, maxLength: 6, nullable: true),
        //             ChequeReq = table.Column<int>(type: "int", nullable: true, defaultValueSql: "(0)"),
        //             Flags = table.Column<int>(type: "int", nullable: true, defaultValueSql: "(0)"),
        //             Duration = table.Column<int>(type: "int", nullable: false, defaultValueSql: "(0)"),
        //             DurationType = table.Column<int>(type: "int", nullable: false, defaultValueSql: "(0)"),
        //             NoUnassignedGroupEntries = table.Column<int>(type: "int", nullable: true, defaultValueSql: "(1)"),
        //             Critical = table.Column<int>(type: "int", nullable: true, defaultValueSql: "(0)"),
        //             OutputType = table.Column<int>(type: "int", nullable: true, defaultValueSql: "(0)"),
        //             Mandatory = table.Column<byte>(type: "tinyint", nullable: true, defaultValueSql: "(0)"),
        //             MultiMatter = table.Column<byte>(type: "tinyint", nullable: true, defaultValueSql: "(0)"),
        //             ActionOnComplete = table.Column<int>(type: "int", nullable: true),
        //             ActionOnOverdue = table.Column<int>(type: "int", nullable: true),
        //             AutoTime = table.Column<byte>(type: "tinyint", nullable: true, defaultValueSql: "(0)"),
        //             VersionControl = table.Column<int>(type: "int", nullable: true),
        //             EmailTemplate = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
        //             AutoEmail = table.Column<short>(type: "smallint", nullable: true),
        //             ConsequenceRating = table.Column<int>(type: "int", nullable: true, defaultValueSql: "(0)"),
        //             SAMAction = table.Column<int>(type: "int", nullable: true, defaultValueSql: "(0)"),
        //             SAMMacro = table.Column<int>(type: "int", nullable: true, defaultValueSql: "(0)"),
        //             WebPublish = table.Column<int>(type: "int", nullable: true, defaultValueSql: "(0)"),
        //             TemplateEmail = table.Column<byte>(type: "tinyint", nullable: true, defaultValueSql: "(0)"),
        //             UserHelp = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false, defaultValueSql: "('')"),
        //             DisableAutoPrint = table.Column<byte>(type: "tinyint", nullable: false),
        //             DefaultProgressFile = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
        //             ActionOnTake = table.Column<byte>(type: "tinyint", nullable: false),
        //             PDFStationerySetID = table.Column<int>(type: "int", nullable: false, defaultValueSql: "((-1))"),
        //             PDFHTMLEmailTemplate = table.Column<int>(type: "int", nullable: false, defaultValueSql: "((-1))"),
        //             ShowTakePrintEmailPrompt = table.Column<byte>(type: "tinyint", nullable: false),
        //             RenameDesc = table.Column<string>(type: "varchar(1000)", unicode: false, maxLength: 1000, nullable: false, defaultValueSql: "('')"),
        //             RenamePromptUser = table.Column<byte>(type: "tinyint", nullable: false),
        //             SAMPriority = table.Column<int>(type: "int", nullable: false, defaultValueSql: "((1))"),
        //             MainClassID = table.Column<int>(type: "int", nullable: false),
        //             SubClassID = table.Column<int>(type: "int", nullable: false),
        //             Locked = table.Column<byte>(type: "tinyint", nullable: false),
        //             MandatoryDefaultMacro = table.Column<byte>(type: "tinyint", nullable: false),
        //             TakeToPDF = table.Column<byte>(type: "tinyint", nullable: false),
        //             TheLinkApp = table.Column<bool>(type: "bit", nullable: false)
        //         },
        //         constraints: table =>
        //         {
        //             table.PrimaryKey("PK_Dm_Documents", x => x.Code)
        //                 .Annotation("SqlServer:Clustered", false);
        //         });

        //     migrationBuilder.CreateTable(
        //         name: "DM_DocumentsPermissions",
        //         columns: table => new
        //         {
        //             doccode = table.Column<int>(type: "int", nullable: false),
        //             casetype = table.Column<int>(type: "int", nullable: false)
        //         },
        //         constraints: table =>
        //         {
        //             table.PrimaryKey("PK__DM_DocumentsPerm__1F7A4DDE", x => new { x.doccode, x.casetype });
        //         });

        //     migrationBuilder.CreateTable(
        //         name: "Mp_Sys_Views",
        //         columns: table => new
        //         {
        //             ID = table.Column<int>(type: "int", nullable: false),
        //             Name = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
        //             CaseGroupRef = table.Column<int>(type: "int", nullable: true, defaultValueSql: "(0)"),
        //             ReadLock = table.Column<int>(type: "int", nullable: true, defaultValueSql: "(0)"),
        //             WriteLock = table.Column<int>(type: "int", nullable: true, defaultValueSql: "(0)"),
        //             NextItem = table.Column<int>(type: "int", nullable: true, defaultValueSql: "(0)"),
        //             Flags = table.Column<int>(type: "int", nullable: true, defaultValueSql: "(0)"),
        //             Type = table.Column<int>(type: "int", nullable: true, defaultValueSql: "(0)"),
        //             Form = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true, defaultValueSql: "('')"),
        //             Icon = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true, defaultValueSql: "('')"),
        //             DescriptionField = table.Column<int>(type: "int", nullable: true, defaultValueSql: "(0)"),
        //             System = table.Column<byte>(type: "tinyint", nullable: true, defaultValueSql: "(0)"),
        //             SystemName = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true, defaultValueSql: "('')"),
        //             Designable = table.Column<byte>(type: "tinyint", nullable: true, defaultValueSql: "(1)"),
        //             InternalName = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
        //             visibility = table.Column<int>(type: "int", nullable: true, defaultValueSql: "(1)"),
        //             WebPublish = table.Column<byte>(type: "tinyint", nullable: false),
        //             CRM_Linked = table.Column<byte>(type: "tinyint", nullable: false),
        //             InternalNote = table.Column<string>(type: "varchar(3000)", unicode: false, maxLength: 3000, nullable: true)
        //         },
        //         constraints: table =>
        //         {
        //             table.PrimaryKey("PK_Mp_Sys_Views", x => x.ID)
        //                 .Annotation("SqlServer:Clustered", false);
        //         });
               

        //     migrationBuilder.CreateTable(
        //         name: "Usr_ORSF_Smartflows",
        //         columns: table => new
        //         {
        //             ID = table.Column<int>(type: "int", nullable: false)
        //                 .Annotation("SqlServer:Identity", "1, 1"),
        //             CaseTypeGroup = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
        //             CaseType = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
        //             SmartflowName = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: false),
        //             SeqNo = table.Column<int>(type: "int", nullable: true),
        //             SmartflowData = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
        //             VariantNo = table.Column<int>(type: "int", nullable: true),
        //             VariantName = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: true)
        //         },
        //         constraints: table =>
        //         {
        //             table.PrimaryKey("PK_Usr_ORSF_Smartflows", x => x.ID)
        //                 .Annotation("SqlServer:Clustered", false);
        //         });

        //     migrationBuilder.CreateIndex(
        //         name: "UC_CaseTypeGroups_Name",
        //         table: "CaseTypeGroups",
        //         column: "Name",
        //         unique: true,
        //         filter: "[Name] IS NOT NULL");

        //     migrationBuilder.CreateIndex(
        //         name: "UC_CaseTypes_CodeName",
        //         table: "CaseTypes",
        //         column: "CodeName",
        //         unique: true,
        //         filter: "[CodeName] IS NOT NULL");
        // }

        // protected override void Down(MigrationBuilder migrationBuilder)
        // {
        //     migrationBuilder.DropTable(
        //         name: "CaseTypeGroups");

        //     migrationBuilder.DropTable(
        //         name: "CaseTypes");

        //     migrationBuilder.DropTable(
        //         name: "Dm_Documents");

        //     migrationBuilder.DropTable(
        //         name: "DM_DocumentsPermissions");

        //     migrationBuilder.DropTable(
        //         name: "Mp_Sys_Views");

        //     migrationBuilder.DropTable(
        //         name: "Usr_ORSF_Smartflows");
         }
    }
}
