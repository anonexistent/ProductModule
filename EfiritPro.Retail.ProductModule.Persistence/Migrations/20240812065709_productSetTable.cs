using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EfiritPro.Retail.ProductModule.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class productSetTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "product_sets",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    owner_id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    product_id = table.Column<Guid>(type: "uuid", nullable: false),
                    included_product_id = table.Column<Guid>(type: "uuid", nullable: false),
                    //product_owner_id = table.Column<Guid>(type: "uuid", nullable: true),
                    //product_organization_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_product_sets", x => new { x.id, x.owner_id, x.organization_id });
                    table.ForeignKey(
                        name: "fk_product_sets_products_included_product_id_included_product_",
                        columns: x => new { x.included_product_id, x.owner_id, x.organization_id },
                        principalTable: "products",
                        principalColumns: new[] { "id", "owner_id", "organization_id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_product_sets_products_product_id_product_owner_id_product_o",
                        columns: x => new { x.product_id, x.owner_id, x.organization_id },
                        principalTable: "products",
                        principalColumns: new[] { "id", "owner_id", "organization_id" });
                });

            migrationBuilder.CreateIndex(
                name: "ix_product_sets_included_product_id_owner_id_organization_id",
                table: "product_sets",
                columns: new[] { "included_product_id", "owner_id", "organization_id" });

            migrationBuilder.CreateIndex(
                name: "ix_product_sets_product_id_product_owner_id_product_organizati",
                table: "product_sets",
                columns: new[] { "product_id", "owner_id", "organization_id" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "product_sets");
        }
    }
}
