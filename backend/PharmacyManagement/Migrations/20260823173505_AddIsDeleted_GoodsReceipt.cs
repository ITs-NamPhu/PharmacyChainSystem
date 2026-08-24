using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PharmacyManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddIsDeleted_GoodsReceipt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "GoodsReceipt",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "GoodsReceipt");
        }
    }
}
