using Microsoft.EntityFrameworkCore.Migrations;

namespace GadjIT_App.Data.Migrations
{
    public partial class AddMoreDetailsToAccounts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CaseType",
                table: "AppCompanyAccountsSmartflowDetails",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CaseTypeGroup",
                table: "AppCompanyAccountsSmartflowDetails",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SmartflowName",
                table: "AppCompanyAccountsSmartflowDetails",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CaseType",
                table: "AppCompanyAccountsSmartflowDetails");

            migrationBuilder.DropColumn(
                name: "CaseTypeGroup",
                table: "AppCompanyAccountsSmartflowDetails");

            migrationBuilder.DropColumn(
                name: "SmartflowName",
                table: "AppCompanyAccountsSmartflowDetails");
        }
    }
}
