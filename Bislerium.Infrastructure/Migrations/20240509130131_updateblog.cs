using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bislerium.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateblog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ResetToken",
                table: "Blogs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Token",
                table: "Blogs",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResetToken",
                table: "Blogs");

            migrationBuilder.DropColumn(
                name: "Token",
                table: "Blogs");
        }
    }
}
