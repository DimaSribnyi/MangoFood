using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MangoFood.Services.ShoppingCartAPI.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ShoppingCartHeaders",
                columns: table => new
                {
                    CartHeaderId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CouponCode = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShoppingCartHeaders", x => x.CartHeaderId);
                });

            migrationBuilder.CreateTable(
                name: "ShoppingCartDetails",
                columns: table => new
                {
                    CartDetailsId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CartHeaderId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Count = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShoppingCartDetails", x => x.CartDetailsId);
                    table.ForeignKey(
                        name: "FK_ShoppingCartDetails_ShoppingCartHeaders_CartHeaderId",
                        column: x => x.CartHeaderId,
                        principalTable: "ShoppingCartHeaders",
                        principalColumn: "CartHeaderId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingCartDetails_CartHeaderId",
                table: "ShoppingCartDetails",
                column: "CartHeaderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShoppingCartDetails");

            migrationBuilder.DropTable(
                name: "ShoppingCartHeaders");
        }
    }
}
