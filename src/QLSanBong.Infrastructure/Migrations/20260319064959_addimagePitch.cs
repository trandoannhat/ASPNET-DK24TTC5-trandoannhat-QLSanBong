using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLSanBong.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addimagePitch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Pitches",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Pitches");
        }
    }
}
