using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HackBack.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RequestEntityPomenyal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "TestGenerationRequestEntity",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "TestGenerationRequestEntity",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "TestAccess",
                table: "TestGenerationRequestEntity",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "TestGenerationRequestEntity");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "TestGenerationRequestEntity");

            migrationBuilder.DropColumn(
                name: "TestAccess",
                table: "TestGenerationRequestEntity");
        }
    }
}
