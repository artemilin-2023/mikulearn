using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HackBack.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Fix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AnswerOptions",
                table: "Questions");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "Questions",
                newName: "QuestType");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "QuestType",
                table: "Questions",
                newName: "Type");

            migrationBuilder.AddColumn<string[]>(
                name: "AnswerOptions",
                table: "Questions",
                type: "text[]",
                nullable: false,
                defaultValue: new string[0]);
        }
    }
}
