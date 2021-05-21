using Microsoft.EntityFrameworkCore.Migrations;

namespace GadjIT_V1_02.Data.Migrations
{
    public partial class addCompanyIdToMapping : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "AppCompanyWorkTypeMapping",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "AppCompanyWorkTypeMapping");
        }
    }
}
