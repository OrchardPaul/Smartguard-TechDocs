using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GadjIT_V1_02.Data.Migrations
{
    public partial class UpdateAccounts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Duration",
                table: "AppCompanyAccountsSmartflowDetails");

            migrationBuilder.AddColumn<bool>(
                name: "Billable",
                table: "AppCompanyAccountsSmartflowDetails",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "BillingDescription",
                table: "AppCompanyAccountsSmartflowDetails",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "AppCompanyAccountsSmartflowDetails",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDate",
                table: "AppCompanyAccountsSmartflowDetails",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "MonthsDuration",
                table: "AppCompanyAccountsSmartflowDetails",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Billable",
                table: "AppCompanyAccountsSmartflowDetails");

            migrationBuilder.DropColumn(
                name: "BillingDescription",
                table: "AppCompanyAccountsSmartflowDetails");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "AppCompanyAccountsSmartflowDetails");

            migrationBuilder.DropColumn(
                name: "DeletedDate",
                table: "AppCompanyAccountsSmartflowDetails");

            migrationBuilder.DropColumn(
                name: "MonthsDuration",
                table: "AppCompanyAccountsSmartflowDetails");

            migrationBuilder.AddColumn<int>(
                name: "Duration",
                table: "AppCompanyAccountsSmartflowDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
