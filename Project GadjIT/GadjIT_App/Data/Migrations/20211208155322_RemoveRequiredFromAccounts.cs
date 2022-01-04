using Microsoft.EntityFrameworkCore.Migrations;

namespace GadjIT_App.Data.Migrations
{
    public partial class RemoveRequiredFromAccounts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppCompanyAccountsSmartflowDetails_SmartflowRecords_SmartflowRecordId",
                table: "AppCompanyAccountsSmartflowDetails");

            migrationBuilder.AlterColumn<int>(
                name: "SmartflowRecordId",
                table: "AppCompanyAccountsSmartflowDetails",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_AppCompanyAccountsSmartflowDetails_SmartflowRecords_SmartflowRecordId",
                table: "AppCompanyAccountsSmartflowDetails",
                column: "SmartflowRecordId",
                principalTable: "SmartflowRecords",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppCompanyAccountsSmartflowDetails_SmartflowRecords_SmartflowRecordId",
                table: "AppCompanyAccountsSmartflowDetails");

            migrationBuilder.AlterColumn<int>(
                name: "SmartflowRecordId",
                table: "AppCompanyAccountsSmartflowDetails",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AppCompanyAccountsSmartflowDetails_SmartflowRecords_SmartflowRecordId",
                table: "AppCompanyAccountsSmartflowDetails",
                column: "SmartflowRecordId",
                principalTable: "SmartflowRecords",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
