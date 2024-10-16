using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EfiritPro.Retail.ProductModule.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class correctSets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_product_sets_products_product_id_product_owner_id_product_o",
                table: "product_sets");

            migrationBuilder.AddForeignKey(
                name: "fk_product_sets_products_product_id_product_owner_id_product_o",
                table: "product_sets",
                columns: new[] { "product_id", "owner_id", "organization_id" },
                principalTable: "products",
                principalColumns: new[] { "id", "owner_id", "organization_id" },
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_product_sets_products_product_id_product_owner_id_product_o",
                table: "product_sets");

            migrationBuilder.AddForeignKey(
                name: "fk_product_sets_products_product_id_product_owner_id_product_o",
                table: "product_sets",
                columns: new[] { "product_id", "owner_id", "organization_id" },
                principalTable: "products",
                principalColumns: new[] { "id", "owner_id", "organization_id" });
        }
    }
}
