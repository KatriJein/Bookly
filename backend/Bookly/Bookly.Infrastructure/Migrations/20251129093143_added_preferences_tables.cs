using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bookly.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class added_preferences_tables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserAuthorPreference_Authors_AuthorId",
                table: "UserAuthorPreference");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAuthorPreference_Users_UserId",
                table: "UserAuthorPreference");

            migrationBuilder.DropForeignKey(
                name: "FK_UserGenrePreference_Genres_GenreId",
                table: "UserGenrePreference");

            migrationBuilder.DropForeignKey(
                name: "FK_UserGenrePreference_Users_UserId",
                table: "UserGenrePreference");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserGenrePreference",
                table: "UserGenrePreference");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserAuthorPreference",
                table: "UserAuthorPreference");

            migrationBuilder.RenameTable(
                name: "UserGenrePreference",
                newName: "UserGenrePreferences");

            migrationBuilder.RenameTable(
                name: "UserAuthorPreference",
                newName: "UserAuthorPreferences");

            migrationBuilder.RenameIndex(
                name: "IX_UserGenrePreference_UserId",
                table: "UserGenrePreferences",
                newName: "IX_UserGenrePreferences_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserGenrePreference_GenreId",
                table: "UserGenrePreferences",
                newName: "IX_UserGenrePreferences_GenreId");

            migrationBuilder.RenameIndex(
                name: "IX_UserAuthorPreference_UserId",
                table: "UserAuthorPreferences",
                newName: "IX_UserAuthorPreferences_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserAuthorPreference_AuthorId",
                table: "UserAuthorPreferences",
                newName: "IX_UserAuthorPreferences_AuthorId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserGenrePreferences",
                table: "UserGenrePreferences",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserAuthorPreferences",
                table: "UserAuthorPreferences",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserAuthorPreferences_Authors_AuthorId",
                table: "UserAuthorPreferences",
                column: "AuthorId",
                principalTable: "Authors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAuthorPreferences_Users_UserId",
                table: "UserAuthorPreferences",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserGenrePreferences_Genres_GenreId",
                table: "UserGenrePreferences",
                column: "GenreId",
                principalTable: "Genres",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserGenrePreferences_Users_UserId",
                table: "UserGenrePreferences",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserAuthorPreferences_Authors_AuthorId",
                table: "UserAuthorPreferences");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAuthorPreferences_Users_UserId",
                table: "UserAuthorPreferences");

            migrationBuilder.DropForeignKey(
                name: "FK_UserGenrePreferences_Genres_GenreId",
                table: "UserGenrePreferences");

            migrationBuilder.DropForeignKey(
                name: "FK_UserGenrePreferences_Users_UserId",
                table: "UserGenrePreferences");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserGenrePreferences",
                table: "UserGenrePreferences");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserAuthorPreferences",
                table: "UserAuthorPreferences");

            migrationBuilder.RenameTable(
                name: "UserGenrePreferences",
                newName: "UserGenrePreference");

            migrationBuilder.RenameTable(
                name: "UserAuthorPreferences",
                newName: "UserAuthorPreference");

            migrationBuilder.RenameIndex(
                name: "IX_UserGenrePreferences_UserId",
                table: "UserGenrePreference",
                newName: "IX_UserGenrePreference_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserGenrePreferences_GenreId",
                table: "UserGenrePreference",
                newName: "IX_UserGenrePreference_GenreId");

            migrationBuilder.RenameIndex(
                name: "IX_UserAuthorPreferences_UserId",
                table: "UserAuthorPreference",
                newName: "IX_UserAuthorPreference_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserAuthorPreferences_AuthorId",
                table: "UserAuthorPreference",
                newName: "IX_UserAuthorPreference_AuthorId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserGenrePreference",
                table: "UserGenrePreference",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserAuthorPreference",
                table: "UserAuthorPreference",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserAuthorPreference_Authors_AuthorId",
                table: "UserAuthorPreference",
                column: "AuthorId",
                principalTable: "Authors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAuthorPreference_Users_UserId",
                table: "UserAuthorPreference",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserGenrePreference_Genres_GenreId",
                table: "UserGenrePreference",
                column: "GenreId",
                principalTable: "Genres",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserGenrePreference_Users_UserId",
                table: "UserGenrePreference",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
