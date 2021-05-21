using Microsoft.EntityFrameworkCore.Migrations;

namespace GadjIT_V1_02.Data.Migrations
{
    public partial class AddAdditionalCompFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CompCol2",
                table: "AppCompanyDetails",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompCol3",
                table: "AppCompanyDetails",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompCol4",
                table: "AppCompanyDetails",
                maxLength: 50,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompCol2",
                table: "AppCompanyDetails");

            migrationBuilder.DropColumn(
                name: "CompCol3",
                table: "AppCompanyDetails");

            migrationBuilder.DropColumn(
                name: "CompCol4",
                table: "AppCompanyDetails");
        }
    }
}
