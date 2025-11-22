using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bookly.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class added_book_collections_to_db_context : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookBookCollection_BookCollection_BookCollectionsId",
                table: "BookBookCollection");

            migrationBuilder.DropForeignKey(
                name: "FK_BookCollection_Users_UserId",
                table: "BookCollection");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BookCollection",
                table: "BookCollection");

            migrationBuilder.RenameTable(
                name: "BookCollection",
                newName: "BookCollections");

            migrationBuilder.RenameIndex(
                name: "IX_BookCollection_UserId",
                table: "BookCollections",
                newName: "IX_BookCollections_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BookCollections",
                table: "BookCollections",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BookBookCollection_BookCollections_BookCollectionsId",
                table: "BookBookCollection",
                column: "BookCollectionsId",
                principalTable: "BookCollections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BookCollections_Users_UserId",
                table: "BookCollections",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookBookCollection_BookCollections_BookCollectionsId",
                table: "BookBookCollection");

            migrationBuilder.DropForeignKey(
                name: "FK_BookCollections_Users_UserId",
                table: "BookCollections");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BookCollections",
                table: "BookCollections");

            migrationBuilder.RenameTable(
                name: "BookCollections",
                newName: "BookCollection");

            migrationBuilder.RenameIndex(
                name: "IX_BookCollections_UserId",
                table: "BookCollection",
                newName: "IX_BookCollection_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BookCollection",
                table: "BookCollection",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BookBookCollection_BookCollection_BookCollectionsId",
                table: "BookBookCollection",
                column: "BookCollectionsId",
                principalTable: "BookCollection",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BookCollection_Users_UserId",
                table: "BookCollection",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
