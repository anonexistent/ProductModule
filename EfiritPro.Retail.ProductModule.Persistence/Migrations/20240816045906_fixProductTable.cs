using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EfiritPro.Retail.ProductModule.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class fixProductTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "fk_products_products_parent_product_id_parent_product_owner_id",
            //    table: "products");

            //migrationBuilder.DropIndex(
            //    name: "ix_products_parent_product_id_parent_product_owner_id_parent_p",
            //    table: "products");

            //migrationBuilder.DropColumn(
            //    name: "parent_product_id",
            //    table: "products");

            //migrationBuilder.DropColumn(
            //    name: "parent_product_organization_id",
            //    table: "products");

            //migrationBuilder.DropColumn(
            //    name: "parent_product_owner_id",
            //    table: "products");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.AddColumn<Guid>(
            //    name: "parent_product_id",
            //    table: "products",
            //    type: "uuid",
            //    nullable: true);

            //migrationBuilder.AddColumn<Guid>(
            //    name: "parent_product_organization_id",
            //    table: "products",
            //    type: "uuid",
            //    nullable: true);

            //migrationBuilder.AddColumn<Guid>(
            //    name: "parent_product_owner_id",
            //    table: "products",
            //    type: "uuid",
            //    nullable: true);

            //migrationBuilder.CreateIndex(
            //    name: "ix_products_parent_product_id_parent_product_owner_id_parent_p",
            //    table: "products",
            //    columns: new[] { "parent_product_id", "parent_product_owner_id", "parent_product_organization_id" });

            //migrationBuilder.AddForeignKey(
            //    name: "fk_products_products_parent_product_id_parent_product_owner_id",
            //    table: "products",
            //    columns: new[] { "parent_product_id", "parent_product_owner_id", "parent_product_organization_id" },
            //    principalTable: "products",
            //    principalColumns: new[] { "id", "owner_id", "organization_id" });
        }
    }
}
