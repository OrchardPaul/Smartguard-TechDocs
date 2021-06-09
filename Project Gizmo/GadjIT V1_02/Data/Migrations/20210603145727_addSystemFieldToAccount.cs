using Microsoft.EntityFrameworkCore.Migrations;

namespace GadjIT_V1_02.Data.Migrations
{
    public partial class addSystemFieldToAccount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "System",
                table: "AppCompanyAccountsSmartflowDetails",
                maxLength: 50,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "System",
                table: "AppCompanyAccountsSmartflowDetails");
        }
    }
}
