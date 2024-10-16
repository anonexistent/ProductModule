using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EfiritPro.Retail.ProductModule.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class _7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "create_by_posting_id",
                table: "product_prices",
                type: "uuid",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "create_by_posting_id",
                table: "product_prices");
        }
    }
}
