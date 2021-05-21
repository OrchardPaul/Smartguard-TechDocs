using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Gizmo_V1_02.Data.Migrations
{
    public partial class AddAccountsTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ExcludeFromAccounts",
                table: "AppCompanyDetails",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "AppCompanyAccountsSmartflow",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(nullable: false),
                    SmartflowCost = table.Column<decimal>(nullable: false),
                    Subscription = table.Column<decimal>(nullable: false),
                    TotalBilled = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppCompanyAccountsSmartflow", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppCompanyAccountsSmartflowDetails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SmartflowAccountId = table.Column<int>(nullable: false),
                    SmartflowRecordId = table.Column<int>(nullable: false),
                    Status = table.Column<string>(maxLength: 50, nullable: true),
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: false),
                    Duration = table.Column<int>(nullable: false),
                    MonthsRemaining = table.Column<int>(nullable: false),
                    MonthlyCharge = table.Column<decimal>(nullable: false),
                    TotalBilled = table.Column<decimal>(nullable: false),
                    Outstanding = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppCompanyAccountsSmartflowDetails", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppCompanyAccountsSmartflow");

            migrationBuilder.DropTable(
                name: "AppCompanyAccountsSmartflowDetails");

            migrationBuilder.DropColumn(
                name: "ExcludeFromAccounts",
                table: "AppCompanyDetails");
        }
    }
}
