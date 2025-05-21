using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestCLI.Migrations
{
    /// <inheritdoc />
    public partial class SetIdentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Relationships_Entries_PID",
                table: "Relationships");
            migrationBuilder.DropForeignKey(
                name: "FK_Relationships_Entries_CID",
                table: "Relationships");
            migrationBuilder.DropPrimaryKey(
                name: "PK_Entries",
                table: "Entries");
            migrationBuilder.DropColumn(
                name: "EID", 
                table: "Entries");

            migrationBuilder.AddColumn<int>(
                name: "EID",
                table: "Entries",
                type: "int",
                nullable: false)
                .Annotation("SqlServer:Identity", "0, 1");
            migrationBuilder.AddPrimaryKey(
                name: "PK_Entries",
                table: "Entries",
                column: "EID");
            migrationBuilder.AddForeignKey(
                        name: "FK_Relationships_Entries_PID",
                        table: "Relationships",
                        column: "PID",
                        principalTable: "Entries",
                        principalColumn: "EID",
                        onDelete: ReferentialAction.NoAction);
            migrationBuilder.AddForeignKey(
                        name: "FK_Relationships_Entries_CID",
                        table: "Relationships",
                        column: "CID",
                        principalTable: "Entries",
                        principalColumn: "EID",
                        onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop foreign keys
            migrationBuilder.DropForeignKey(
                name: "FK_Relationships_Entries_PID",
                table: "Relationships");

            migrationBuilder.DropForeignKey(
                name: "FK_Relationships_Entries_CID",
                table: "Relationships");

            // Drop the primary key
            migrationBuilder.DropPrimaryKey(
                name: "PK_Entries",
                table: "Entries");

            // Drop the identity column
            migrationBuilder.DropColumn(
                name: "EID",
                table: "Entries");

            // Re-add the original column without identity (assuming it was previously configured)
            migrationBuilder.AddColumn<int>(
                name: "EID",
                table: "Entries",
                type: "int",
                nullable: false); // Adjust nullable as necessary

            // Re-add the primary key
            migrationBuilder.AddPrimaryKey(
                name: "PK_Entries",
                table: "Entries",
                column: "EID");

            // Re-add the foreign keys
            migrationBuilder.AddForeignKey(
                name: "FK_Relationships_Entries_PID",
                table: "Relationships",
                column: "PID",
                principalTable: "Entries",
                principalColumn: "EID",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Relationships_Entries_CID",
                table: "Relationships",
                column: "CID",
                principalTable: "Entries",
                principalColumn: "EID",
                onDelete: ReferentialAction.NoAction);
        }
    }
}
