using Microsoft.EntityFrameworkCore.Migrations;

namespace GadjIT_App.Data.Migrations
{
    public partial class addSelectedUriFlagForUsers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SelectedUri",
                table: "AspNetUsers",
                maxLength: 256,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SelectedUri",
                table: "AspNetUsers");
        }
    }
}
