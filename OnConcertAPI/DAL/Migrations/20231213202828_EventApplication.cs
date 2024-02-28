using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnConcert.DAL.Migrations
{
    /// <inheritdoc />
    public partial class EventApplication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EventApplication",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventId = table.Column<int>(type: "int", nullable: false),
                    VisitorId = table.Column<int>(type: "int", nullable: true),
                    BandId = table.Column<int>(type: "int", nullable: true),
                    BandApplicationStatus = table.Column<string>(type: "nvarchar(30)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventApplication", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventApplication_Band_BandId",
                        column: x => x.BandId,
                        principalTable: "Band",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EventApplication_Event_EventId",
                        column: x => x.EventId,
                        principalTable: "Event",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventApplication_Visitor_VisitorId",
                        column: x => x.VisitorId,
                        principalTable: "Visitor",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventApplication_BandId",
                table: "EventApplication",
                column: "BandId");

            migrationBuilder.CreateIndex(
                name: "IX_EventApplication_EventId",
                table: "EventApplication",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_EventApplication_VisitorId",
                table: "EventApplication",
                column: "VisitorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventApplication");
        }
    }
}
