using Microsoft.EntityFrameworkCore.Migrations;

namespace Gizmo_V1_02.Data.Migrations
{
    public partial class ReworkSmartflowRecords : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Alt_Display_Name",
                table: "SmartflowRecords");

            migrationBuilder.DropColumn(
                name: "AsName",
                table: "SmartflowRecords");

            migrationBuilder.DropColumn(
                name: "ChapterData",
                table: "SmartflowRecords");

            migrationBuilder.DropColumn(
                name: "CompleteName",
                table: "SmartflowRecords");

            migrationBuilder.DropColumn(
                name: "Entity_Type",
                table: "SmartflowRecords");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "SmartflowRecords");

            migrationBuilder.DropColumn(
                name: "Next_Status",
                table: "SmartflowRecords");

            migrationBuilder.DropColumn(
                name: "ParentID",
                table: "SmartflowRecords");

            migrationBuilder.DropColumn(
                name: "RescheduleDays",
                table: "SmartflowRecords");

            migrationBuilder.DropColumn(
                name: "Suppress_Step",
                table: "SmartflowRecords");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "SmartflowRecords");

            migrationBuilder.AlterColumn<int>(
                name: "SeqNo",
                table: "SmartflowRecords",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "SmartflowData",
                table: "SmartflowRecords",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SmartflowName",
                table: "SmartflowRecords",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VariantName",
                table: "SmartflowRecords",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VariantNo",
                table: "SmartflowRecords",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SmartflowData",
                table: "SmartflowRecords");

            migrationBuilder.DropColumn(
                name: "SmartflowName",
                table: "SmartflowRecords");

            migrationBuilder.DropColumn(
                name: "VariantName",
                table: "SmartflowRecords");

            migrationBuilder.DropColumn(
                name: "VariantNo",
                table: "SmartflowRecords");

            migrationBuilder.AlterColumn<int>(
                name: "SeqNo",
                table: "SmartflowRecords",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Alt_Display_Name",
                table: "SmartflowRecords",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AsName",
                table: "SmartflowRecords",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ChapterData",
                table: "SmartflowRecords",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompleteName",
                table: "SmartflowRecords",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Entity_Type",
                table: "SmartflowRecords",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "SmartflowRecords",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Next_Status",
                table: "SmartflowRecords",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ParentID",
                table: "SmartflowRecords",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RescheduleDays",
                table: "SmartflowRecords",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Suppress_Step",
                table: "SmartflowRecords",
                type: "nvarchar(1)",
                maxLength: 1,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "SmartflowRecords",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }
    }
}
