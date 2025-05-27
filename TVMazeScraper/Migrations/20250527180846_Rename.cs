using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TVMazeScraper.Migrations
{
    /// <inheritdoc />
    public partial class Rename : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShowActors");

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    ShowId = table.Column<int>(type: "INTEGER", nullable: false),
                    ActorId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => new { x.ShowId, x.ActorId });
                    table.ForeignKey(
                        name: "FK_Roles_Actors_ActorId",
                        column: x => x.ActorId,
                        principalTable: "Actors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Roles_Shows_ShowId",
                        column: x => x.ShowId,
                        principalTable: "Shows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Roles_ActorId",
                table: "Roles",
                column: "ActorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.CreateTable(
                name: "ShowActors",
                columns: table => new
                {
                    ShowID = table.Column<int>(type: "INTEGER", nullable: false),
                    ActorID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShowActors", x => new { x.ShowID, x.ActorID });
                    table.ForeignKey(
                        name: "FK_ShowActors_Actors_ActorID",
                        column: x => x.ActorID,
                        principalTable: "Actors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShowActors_Shows_ShowID",
                        column: x => x.ShowID,
                        principalTable: "Shows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShowActors_ActorID",
                table: "ShowActors",
                column: "ActorID");
        }
    }
}
