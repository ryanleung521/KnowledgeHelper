using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestCLI.Migrations
{
    /// <inheritdoc />
    public partial class TagUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Taggings",
                columns: table => new
                {
                    EID = table.Column<int>(type: "int", nullable: false),
                    TID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Taggings", x => new { x.EID, x.TID });
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    TID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "0, 1"),
                    TName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.TID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Taggings");

            migrationBuilder.DropTable(
                name: "Tags");
        }
    }
}
