using Microsoft.EntityFrameworkCore.Migrations;

namespace GadjIT_V1_02.Data.Migrations
{
    public partial class changeCompanyWorkTypeGroupKeyPart1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AppCompanyWorkTypeGroups",
                table: "AppCompanyWorkTypeGroups");

            migrationBuilder.DropColumn(
                name: "CompanyID",
                table: "AppCompanyWorkTypeGroups");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "AppCompanyWorkTypeGroups",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AppCompanyWorkTypeGroups",
                table: "AppCompanyWorkTypeGroups",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AppCompanyWorkTypeGroups",
                table: "AppCompanyWorkTypeGroups");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "AppCompanyWorkTypeGroups");

            migrationBuilder.AddColumn<int>(
                name: "CompanyID",
                table: "AppCompanyWorkTypeGroups",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AppCompanyWorkTypeGroups",
                table: "AppCompanyWorkTypeGroups",
                column: "CompanyID");
        }
    }
}
