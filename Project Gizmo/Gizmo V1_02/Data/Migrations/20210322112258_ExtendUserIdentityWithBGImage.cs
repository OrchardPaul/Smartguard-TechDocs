using Microsoft.EntityFrameworkCore.Migrations;

namespace Gizmo_V1_02.Data.Migrations
{
    public partial class ExtendUserIdentityWithBGImage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MainBackgroundImage",
                table: "AspNetUsers",
                maxLength: 500,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MainBackgroundImage",
                table: "AspNetUsers");
        }
    }
}
