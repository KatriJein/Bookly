using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bookly.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class preference_weight : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HatedGenresStrictRestriction",
                table: "Users");

            migrationBuilder.AddColumn<double>(
                name: "Weight",
                table: "UserGenrePreference",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Weight",
                table: "UserAuthorPreference",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Weight",
                table: "UserGenrePreference");

            migrationBuilder.DropColumn(
                name: "Weight",
                table: "UserAuthorPreference");

            migrationBuilder.AddColumn<bool>(
                name: "HatedGenresStrictRestriction",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
