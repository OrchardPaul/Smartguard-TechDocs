using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Gizmo_V1_02.Data.Migrations
{
    public partial class SmartFlowRecordsAddSystemDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "SmartflowRecords",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "SmartflowRecords",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedByUserId",
                table: "SmartflowRecords",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedDate",
                table: "SmartflowRecords",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "System",
                table: "SmartflowRecords",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "SmartflowRecords");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "SmartflowRecords");

            migrationBuilder.DropColumn(
                name: "LastModifiedByUserId",
                table: "SmartflowRecords");

            migrationBuilder.DropColumn(
                name: "LastModifiedDate",
                table: "SmartflowRecords");

            migrationBuilder.DropColumn(
                name: "System",
                table: "SmartflowRecords");
        }
    }
}
