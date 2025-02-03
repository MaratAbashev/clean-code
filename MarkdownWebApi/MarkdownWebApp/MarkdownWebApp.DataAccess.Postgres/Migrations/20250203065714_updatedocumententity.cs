using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarkdownWebApp.DataAccess.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class updatedocumententity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Path",
                table: "Documents");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Path",
                table: "Documents",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
