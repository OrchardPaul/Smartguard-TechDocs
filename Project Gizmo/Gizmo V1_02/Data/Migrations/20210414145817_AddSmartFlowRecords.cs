using Microsoft.EntityFrameworkCore.Migrations;

namespace Gizmo_V1_02.Data.Migrations
{
    public partial class AddSmartFlowRecords : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SmartflowRecords",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParentID = table.Column<int>(nullable: true),
                    CaseTypeGroup = table.Column<string>(maxLength: 100, nullable: true),
                    CaseType = table.Column<string>(maxLength: 100, nullable: true),
                    Type = table.Column<string>(maxLength: 100, nullable: false),
                    Name = table.Column<string>(maxLength: 250, nullable: false),
                    SeqNo = table.Column<int>(nullable: false),
                    Suppress_Step = table.Column<string>(maxLength: 1, nullable: true),
                    Entity_Type = table.Column<string>(maxLength: 100, nullable: true),
                    AsName = table.Column<string>(maxLength: 250, nullable: true),
                    CompleteName = table.Column<string>(maxLength: 250, nullable: true),
                    RescheduleDays = table.Column<int>(nullable: true),
                    Alt_Display_Name = table.Column<string>(maxLength: 300, nullable: true),
                    Next_Status = table.Column<string>(maxLength: 100, nullable: true),
                    ChapterData = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmartflowRecords", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SmartflowRecords");
        }
    }
}
