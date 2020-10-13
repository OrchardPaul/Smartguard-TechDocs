using Microsoft.EntityFrameworkCore.Migrations;

namespace Gizmo_V1_02.Data.Migrations
{
    public partial class InitWorkTypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppCompanyWorkTypeGroups",
                columns: table => new
                {
                    CompanyID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkTypeGroupId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppCompanyWorkTypeGroups", x => x.CompanyID);
                });

            migrationBuilder.CreateTable(
                name: "AppWorkTypeGroups",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GroupName = table.Column<string>(maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppWorkTypeGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppWorkTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TypeName = table.Column<string>(maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppWorkTypes", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppCompanyWorkTypeGroups");

            migrationBuilder.DropTable(
                name: "AppWorkTypeGroups");

            migrationBuilder.DropTable(
                name: "AppWorkTypes");
        }
    }
}
