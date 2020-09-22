using Microsoft.EntityFrameworkCore.Migrations;

namespace Gizmo_V1_02.Data.Migrations
{
    public partial class SeedUserRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"INSERT INTO dbo.AspNetRoles (id, name) VALUES (1,'Admin')");
            migrationBuilder.Sql(@"INSERT INTO dbo.AspNetRoles (id, name) VALUES (2,'StandardUser')");

            /*
            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    {1,"Admin"}
                    ,{2,"StandardUser"}
                }
            );
            */
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
