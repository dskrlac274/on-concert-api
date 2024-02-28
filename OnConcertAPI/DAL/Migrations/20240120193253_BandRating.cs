using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnConcert.DAL.Migrations
{
    /// <inheritdoc />
    public partial class BandRating : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BandRating",
                columns: table => new
                {
                    RatingId = table.Column<int>(type: "int", nullable: false),
                    BandId = table.Column<int>(type: "int", nullable: false),
                    OrganizerId = table.Column<int>(type: "int", nullable: true),
                    VisitorId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BandRating", x => x.RatingId);
                    table.ForeignKey(
                        name: "FK_BandRating_Band_BandId",
                        column: x => x.BandId,
                        principalTable: "Band",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BandRating_Organizer_OrganizerId",
                        column: x => x.OrganizerId,
                        principalTable: "Organizer",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BandRating_Rating_RatingId",
                        column: x => x.RatingId,
                        principalTable: "Rating",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BandRating_Visitor_VisitorId",
                        column: x => x.VisitorId,
                        principalTable: "Visitor",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_BandRating_BandId_OrganizerId_VisitorId",
                table: "BandRating",
                columns: new[] { "BandId", "OrganizerId", "VisitorId" },
                unique: true,
                filter: "[OrganizerId] IS NOT NULL AND [VisitorId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_BandRating_OrganizerId",
                table: "BandRating",
                column: "OrganizerId");

            migrationBuilder.CreateIndex(
                name: "IX_BandRating_VisitorId",
                table: "BandRating",
                column: "VisitorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BandRating");
        }
    }
}
