using Microsoft.EntityFrameworkCore.Migrations;

namespace Gizmo_V1_02.Data.Migrations
{
    public partial class addCompCol1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CompCol1",
                table: "AppCompanyDetails",
                maxLength: 50,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompCol1",
                table: "AppCompanyDetails");
        }
    }
}
