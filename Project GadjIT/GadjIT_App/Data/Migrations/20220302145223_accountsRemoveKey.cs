using Microsoft.EntityFrameworkCore.Migrations;

namespace GadjIT_App.Data.Migrations
{
    public partial class accountsRemoveKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppCompanyAccountsSmartflowDetails_SmartflowRecords_SmartflowRecordId",
                table: "AppCompanyAccountsSmartflowDetails");

            migrationBuilder.DropIndex(
                name: "IX_AppCompanyAccountsSmartflowDetails_SmartflowRecordId",
                table: "AppCompanyAccountsSmartflowDetails");

            migrationBuilder.DropColumn(
                name: "SmartflowRecordId",
                table: "AppCompanyAccountsSmartflowDetails");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SmartflowRecordId",
                table: "AppCompanyAccountsSmartflowDetails",
                type: "int",
                nullable: true);

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
                onDelete: ReferentialAction.Restrict);
        }
    }
}
