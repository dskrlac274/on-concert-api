using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnConcert.DAL.Migrations
{
    /// <inheritdoc />
    public partial class VisitorValidation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Visitor");

            migrationBuilder.DropColumn(
                name: "GoogleId",
                table: "Visitor");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "Visitor");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Visitor",
                type: "nvarchar(70)",
                maxLength: 70,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Visitor",
                type: "nvarchar(70)",
                maxLength: 70,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Visitor_Email",
                table: "Visitor",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Visitor_Email",
                table: "Visitor");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Visitor");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Visitor",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(70)",
                oldMaxLength: 70);

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Visitor",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GoogleId",
                table: "Visitor",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Visitor",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
