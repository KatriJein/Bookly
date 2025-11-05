using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bookly.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class scraping_task_model : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ScrapingTaskStates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrentGenreIndex = table.Column<int>(type: "integer", nullable: false),
                    CurrentPublisherIndex = table.Column<int>(type: "integer", nullable: false),
                    CurrentIndex = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CooldownUntil = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScrapingTaskStates", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScrapingTaskStates");
        }
    }
}
