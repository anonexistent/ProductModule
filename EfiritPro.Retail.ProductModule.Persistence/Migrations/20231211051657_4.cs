using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EfiritPro.Retail.ProductModule.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class _4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "current_product_price_id",
                table: "products",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "price_should_be_set_in_time",
                table: "products",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_products_current_product_price_id_owner_id_organization_id_",
                table: "products",
                columns: new[] { "current_product_price_id", "owner_id", "organization_id", "id" });

            migrationBuilder.AddForeignKey(
                name: "fk_products_product_prices_current_product_price_id1",
                table: "products",
                columns: new[] { "current_product_price_id", "owner_id", "organization_id", "id" },
                principalTable: "product_prices",
                principalColumns: new[] { "id", "owner_id", "organization_id", "product_id" },
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_products_product_prices_current_product_price_id1",
                table: "products");

            migrationBuilder.DropIndex(
                name: "ix_products_current_product_price_id_owner_id_organization_id_",
                table: "products");

            migrationBuilder.DropColumn(
                name: "current_product_price_id",
                table: "products");

            migrationBuilder.DropColumn(
                name: "price_should_be_set_in_time",
                table: "products");
        }
    }
}
