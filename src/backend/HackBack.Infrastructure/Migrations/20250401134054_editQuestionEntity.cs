using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HackBack.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class editQuestionEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AnswerOptionsSerialized",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "CorrectAnswersSerialized",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "OptionsSerialized",
                table: "Questions");

            migrationBuilder.AddColumn<string[]>(
                name: "AnswerOptions",
                table: "Questions",
                type: "text[]",
                nullable: false,
                defaultValue: new string[0]);

            migrationBuilder.AddColumn<string[]>(
                name: "CorrectAnswers",
                table: "Questions",
                type: "text[]",
                nullable: false,
                defaultValue: new string[0]);

            migrationBuilder.AddColumn<string[]>(
                name: "Options",
                table: "Questions",
                type: "text[]",
                nullable: false,
                defaultValue: new string[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AnswerOptions",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "CorrectAnswers",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "Options",
                table: "Questions");

            migrationBuilder.AddColumn<string>(
                name: "AnswerOptionsSerialized",
                table: "Questions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CorrectAnswersSerialized",
                table: "Questions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OptionsSerialized",
                table: "Questions",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
