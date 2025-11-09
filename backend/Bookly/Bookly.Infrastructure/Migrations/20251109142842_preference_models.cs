using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bookly.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class preference_models : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AgeCategory",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "TookEntrySurvey",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "VolumeSizePreference",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "UserAuthorPreference",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: false),
                    PreferenceType = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAuthorPreference", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserAuthorPreference_Authors_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Authors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserAuthorPreference_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserGenrePreference",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    GenreId = table.Column<Guid>(type: "uuid", nullable: false),
                    PreferenceType = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserGenrePreference", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserGenrePreference_Genres_GenreId",
                        column: x => x.GenreId,
                        principalTable: "Genres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserGenrePreference_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserAuthorPreference_AuthorId",
                table: "UserAuthorPreference",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAuthorPreference_UserId",
                table: "UserAuthorPreference",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserGenrePreference_GenreId",
                table: "UserGenrePreference",
                column: "GenreId");

            migrationBuilder.CreateIndex(
                name: "IX_UserGenrePreference_UserId",
                table: "UserGenrePreference",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserAuthorPreference");

            migrationBuilder.DropTable(
                name: "UserGenrePreference");

            migrationBuilder.DropColumn(
                name: "AgeCategory",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TookEntrySurvey",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "VolumeSizePreference",
                table: "Users");
        }
    }
}
