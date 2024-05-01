using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MangoFood.Services.OrderAPI.Migrations
{
    /// <inheritdoc />
    public partial class FixingCols : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LatName",
                table: "OrderHeaders",
                newName: "LastName");

            migrationBuilder.RenameColumn(
                name: "OrdertHeaderId",
                table: "OrderHeaders",
                newName: "OrderHeaderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LastName",
                table: "OrderHeaders",
                newName: "LatName");

            migrationBuilder.RenameColumn(
                name: "OrderHeaderId",
                table: "OrderHeaders",
                newName: "OrdertHeaderId");
        }
    }
}
