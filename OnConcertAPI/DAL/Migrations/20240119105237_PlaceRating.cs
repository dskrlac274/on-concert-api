using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnConcert.DAL.Migrations
{
    /// <inheritdoc />
    public partial class PlaceRating : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PlaceRating",
                columns: table => new
                {
                    RatingId = table.Column<int>(type: "int", nullable: false),
                    BandId = table.Column<int>(type: "int", nullable: true),
                    VisitorId = table.Column<int>(type: "int", nullable: true),
                    PlaceId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlaceRating", x => x.RatingId);
                    table.ForeignKey(
                        name: "FK_PlaceRating_Band_BandId",
                        column: x => x.BandId,
                        principalTable: "Band",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PlaceRating_Place_PlaceId",
                        column: x => x.PlaceId,
                        principalTable: "Place",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlaceRating_Rating_RatingId",
                        column: x => x.RatingId,
                        principalTable: "Rating",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlaceRating_Visitor_VisitorId",
                        column: x => x.VisitorId,
                        principalTable: "Visitor",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlaceRating_BandId_VisitorId_PlaceId",
                table: "PlaceRating",
                columns: new[] { "BandId", "VisitorId", "PlaceId" },
                unique: true,
                filter: "[BandId] IS NOT NULL AND [VisitorId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_PlaceRating_PlaceId",
                table: "PlaceRating",
                column: "PlaceId");

            migrationBuilder.CreateIndex(
                name: "IX_PlaceRating_VisitorId",
                table: "PlaceRating",
                column: "VisitorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlaceRating");
        }
    }
}
