using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HackBack.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTestGenerationRequestEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TestGenerationRequestEntity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    FileName = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestGenerationRequestEntity", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TestGenerationRequestEntity_CreatedBy",
                table: "TestGenerationRequestEntity",
                column: "CreatedBy");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TestGenerationRequestEntity");
        }
    }
}
