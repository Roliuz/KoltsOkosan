using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UjGyakorlas.Migrations
{
    /// <inheritdoc />
    public partial class AddKategoriaColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Tipus",
                table: "Kiadasok",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Tipus",
                table: "Kiadasok");
        }
    }
}
