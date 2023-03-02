using Microsoft.EntityFrameworkCore.Migrations;

namespace GadjIT_ClientContext_Proj.Migrations
{
    public partial class Initial005 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropTable(
            //    name: "TableDate");

            migrationBuilder.AddColumn<string>(
                name: "VariantName2",
                table: "Usr_ORSF_Smartflows",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VariantNo2",
                table: "Usr_ORSF_Smartflows",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VariantName2",
                table: "Usr_ORSF_Smartflows");

            migrationBuilder.DropColumn(
                name: "VariantNo2",
                table: "Usr_ORSF_Smartflows");

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
