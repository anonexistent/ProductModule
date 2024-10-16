using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EfiritPro.Retail.ProductModule.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class datatypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropIndex(
            //    name: "ix_products_bar_code",
            //    table: "products");

            //migrationBuilder.DropIndex(
            //    name: "ix_products_vendor_code",
            //    table: "products");

            migrationBuilder.AlterColumn<float>(
                name: "selling_price",
                table: "products",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(20,0)");

            migrationBuilder.AlterColumn<float>(
                name: "purchase_price",
                table: "products",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(20,0)");

            migrationBuilder.AlterColumn<float>(
                name: "promo_price",
                table: "products",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(20,0)");

            migrationBuilder.AlterColumn<float>(
                name: "price_min_in_cash_receipt",
                table: "products",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(20,0)");

            migrationBuilder.AlterColumn<float>(
                name: "price_max_in_cash_receipt",
                table: "products",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(20,0)");

            migrationBuilder.AlterColumn<float>(
                name: "selling_price",
                table: "product_prices",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(20,0)");

            migrationBuilder.AlterColumn<float>(
                name: "purchase_price",
                table: "product_prices",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(20,0)");

            migrationBuilder.AlterColumn<float>(
                name: "promo_price",
                table: "product_prices",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(20,0)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "selling_price",
                table: "products",
                type: "numeric(20,0)",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "numeric(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "purchase_price",
                table: "products",
                type: "numeric(20,0)",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "numeric(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "promo_price",
                table: "products",
                type: "numeric(20,0)",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "numeric(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "price_min_in_cash_receipt",
                table: "products",
                type: "numeric(20,0)",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "numeric(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "price_max_in_cash_receipt",
                table: "products",
                type: "numeric(20,0)",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "numeric(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "selling_price",
                table: "product_prices",
                type: "numeric(20,0)",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "numeric(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "purchase_price",
                table: "product_prices",
                type: "numeric(20,0)",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "numeric(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "promo_price",
                table: "product_prices",
                type: "numeric(20,0)",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "numeric(18,2)");

            //migrationBuilder.CreateIndex(
            //    name: "ix_products_bar_code",
            //    table: "products",
            //    column: "bar_code",
            //    unique: false);

            //migrationBuilder.CreateIndex(
            //    name: "ix_products_vendor_code",
            //    table: "products",
            //    column: "vendor_code",
            //    unique: false);
        }
    }
}
