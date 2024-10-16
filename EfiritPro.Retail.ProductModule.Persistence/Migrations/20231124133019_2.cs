using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EfiritPro.Retail.ProductModule.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class _2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "product_group_id",
                table: "products",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "product_groups",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    owner_id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    parent_group_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_product_groups", x => new { x.id, x.owner_id, x.organization_id });
                    table.ForeignKey(
                        name: "fk_product_groups_product_groups_parent_group_id_parent_group_",
                        columns: x => new { x.parent_group_id, x.owner_id, x.organization_id },
                        principalTable: "product_groups",
                        principalColumns: new[] { "id", "owner_id", "organization_id" },
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "ix_products_product_group_id_owner_id_organization_id",
                table: "products",
                columns: new[] { "product_group_id", "owner_id", "organization_id" });

            migrationBuilder.CreateIndex(
                name: "ix_product_groups_parent_group_id_owner_id_organization_id",
                table: "product_groups",
                columns: new[] { "parent_group_id", "owner_id", "organization_id" });

            migrationBuilder.AddForeignKey(
                name: "fk_products_product_groups_product_group_id",
                table: "products",
                columns: new[] { "product_group_id", "owner_id", "organization_id" },
                principalTable: "product_groups",
                principalColumns: new[] { "id", "owner_id", "organization_id" },
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_products_product_groups_product_group_id",
                table: "products");

            migrationBuilder.DropTable(
                name: "product_groups");

            migrationBuilder.DropIndex(
                name: "ix_products_product_group_id_owner_id_organization_id",
                table: "products");

            migrationBuilder.DropColumn(
                name: "product_group_id",
                table: "products");
        }
    }
}
