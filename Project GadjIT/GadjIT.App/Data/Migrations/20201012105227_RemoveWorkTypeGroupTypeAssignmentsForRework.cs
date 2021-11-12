using Microsoft.EntityFrameworkCore.Migrations;

namespace GadjIT_App.Data.Migrations
{
    public partial class RemoveWorkTypeGroupTypeAssignmentsForRework : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppWorkTypeGroupsTypeAssignments");

            migrationBuilder.AddColumn<int>(
                name: "parentId",
                table: "AppWorkTypeGroups",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "parentId",
                table: "AppWorkTypeGroups");

            migrationBuilder.CreateTable(
                name: "AppWorkTypeGroupsTypeAssignments",
                columns: table => new
                {
                    WorkTypeGroupId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkTypeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppWorkTypeGroupsTypeAssignments", x => x.WorkTypeGroupId);
                });
        }
    }
}
