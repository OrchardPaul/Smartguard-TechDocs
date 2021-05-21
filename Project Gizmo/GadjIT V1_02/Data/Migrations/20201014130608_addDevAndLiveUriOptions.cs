using Microsoft.EntityFrameworkCore.Migrations;

namespace GadjIT_V1_02.Data.Migrations
{
    public partial class addDevAndLiveUriOptions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DevUri",
                table: "AppCompanyDetails",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LiveUri",
                table: "AppCompanyDetails",
                maxLength: 256,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DevUri",
                table: "AppCompanyDetails");

            migrationBuilder.DropColumn(
                name: "LiveUri",
                table: "AppCompanyDetails");
        }
    }
}
