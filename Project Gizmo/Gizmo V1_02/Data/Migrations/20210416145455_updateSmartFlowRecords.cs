using Microsoft.EntityFrameworkCore.Migrations;

namespace Gizmo_V1_02.Data.Migrations
{
    public partial class updateSmartFlowRecords : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "SmartflowRecords",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RowId",
                table: "SmartflowRecords",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "SmartflowRecords");

            migrationBuilder.DropColumn(
                name: "RowId",
                table: "SmartflowRecords");
        }
    }
}
