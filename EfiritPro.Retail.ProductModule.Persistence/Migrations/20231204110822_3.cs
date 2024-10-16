using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EfiritPro.Retail.ProductModule.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class _3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "promo_price",
                table: "products");

            migrationBuilder.DropColumn(
                name: "purchase_price",
                table: "products");

            migrationBuilder.DropColumn(
                name: "selling_price",
                table: "products");

            migrationBuilder.CreateTable(
                name: "product_prices",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    owner_id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    product_id = table.Column<Guid>(type: "uuid", nullable: false),
                    start_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    purchase_price = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    selling_price = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    promo_price = table.Column<decimal>(type: "numeric(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_product_prices", x => new { x.id, x.owner_id, x.organization_id, x.product_id });
                    table.ForeignKey(
                        name: "fk_product_prices_products_product_id_product_owner_id_product",
                        columns: x => new { x.product_id, x.owner_id, x.organization_id },
                        principalTable: "products",
                        principalColumns: new[] { "id", "owner_id", "organization_id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_product_prices_product_id_owner_id_organization_id",
                table: "product_prices",
                columns: new[] { "product_id", "owner_id", "organization_id" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "product_prices");

            migrationBuilder.AddColumn<decimal>(
                name: "promo_price",
                table: "products",
                type: "numeric(20,0)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "purchase_price",
                table: "products",
                type: "numeric(20,0)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "selling_price",
                table: "products",
                type: "numeric(20,0)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
