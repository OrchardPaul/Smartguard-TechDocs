using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GadjIT_App.Data.Migrations
{
    public partial class accountsCompanyId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "AppCompanyAccountsSmartflowDetails");

            migrationBuilder.AddColumn<int>(
                name: "ClientRowId",
                table: "AppCompanyAccountsSmartflowDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "AppCompanyAccountsSmartflowDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClientRowId",
                table: "AppCompanyAccountsSmartflowDetails");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "AppCompanyAccountsSmartflowDetails");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "AppCompanyAccountsSmartflowDetails",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
