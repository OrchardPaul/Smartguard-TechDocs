using Microsoft.EntityFrameworkCore.Migrations;

namespace GadjIT_ClientContext_Proj.Migrations
{
    public partial class Initial004 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TableDate");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TableDate",
                columns: table => new
                {
                    TableField = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TableType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                });
        }
    }
}
