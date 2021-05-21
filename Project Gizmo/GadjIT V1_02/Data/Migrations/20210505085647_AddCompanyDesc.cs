using Microsoft.EntityFrameworkCore.Migrations;

namespace GadjIT_V1_02.Data.Migrations
{
    public partial class AddCompanyDesc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CompanyDesc",
                table: "AppCompanyDetails",
                maxLength: 256,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompanyDesc",
                table: "AppCompanyDetails");
        }
    }
}
