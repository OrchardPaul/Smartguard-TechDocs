using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GadjIT_App.Data.Migrations
{
    public partial class AddRecordForignKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GadjIT_Log",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MessageTemplate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Level = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TimeStamp = table.Column<DateTime>(type: "datetime", nullable: true),
                    Exception = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Properties = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SourceContext = table.Column<string>(type: "nvarchar(75)", maxLength: 75, nullable: true),
                    SourceUserId = table.Column<string>(type: "nvarchar(75)", maxLength: 75, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GadjIT_Log", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppCompanyAccountsSmartflowDetails_SmartflowRecordId",
                table: "AppCompanyAccountsSmartflowDetails",
                column: "SmartflowRecordId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppCompanyAccountsSmartflowDetails_SmartflowRecords_SmartflowRecordId",
                table: "AppCompanyAccountsSmartflowDetails",
                column: "SmartflowRecordId",
                principalTable: "SmartflowRecords",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppCompanyAccountsSmartflowDetails_SmartflowRecords_SmartflowRecordId",
                table: "AppCompanyAccountsSmartflowDetails");

            migrationBuilder.DropTable(
                name: "GadjIT_Log");

            migrationBuilder.DropIndex(
                name: "IX_AppCompanyAccountsSmartflowDetails_SmartflowRecordId",
                table: "AppCompanyAccountsSmartflowDetails");
        }
    }
}
