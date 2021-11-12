using Microsoft.EntityFrameworkCore.Migrations;

namespace GadjIT_V1_02.Data.Migrations
{
    public partial class RemoveObseleteBaseUri : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BaseUri",
                table: "AppCompanyDetails");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BaseUri",
                table: "AppCompanyDetails",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);
        }
    }
}
