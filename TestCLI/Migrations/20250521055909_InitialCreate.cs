using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestCLI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Entries",
                columns: table => new
                {
                    EID = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Entries", x => x.EID);
                });

            migrationBuilder.CreateTable(
                name: "Relationships",
                columns: table => new
                {
                    PID = table.Column<int>(type: "int", nullable: false),
                    CID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Relationships", x => new { x.PID, x.CID });
                    table.ForeignKey(
                        name: "FK_Relationships_Entries_PID",
                        column: x => x.PID,
                        principalTable: "Entries",
                        principalColumn: "EID",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Relationships_Entries_CID",
                        column: x => x.CID,
                        principalTable: "Entries",
                        principalColumn: "EID",
                        onDelete: ReferentialAction.NoAction);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Entries");

            migrationBuilder.DropTable(
                name: "Relationships");
        }
    }
}
