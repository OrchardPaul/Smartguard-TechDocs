using Microsoft.EntityFrameworkCore.Migrations;

namespace GadjIT_ClientContext_Proj.Migrations
{
    public partial class Initial007 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VariantName2",
                table: "Usr_ORSF_Smartflows");

            migrationBuilder.DropColumn(
                name: "VariantNo2",
                table: "Usr_ORSF_Smartflows");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
    }
}
