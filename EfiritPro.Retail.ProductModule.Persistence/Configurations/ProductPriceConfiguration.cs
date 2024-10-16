using EfiritPro.Retail.ProductModule.Models;
using Microsoft.EntityFrameworkCore;

namespace EfiritPro.Retail.ProductModule.Persistence.Configurations;

public static class ProductPriceConfiguration
{
    public static ModelBuilder ConfigureProductPriceTable(this ModelBuilder builder)
    {
        builder.Entity<ProductPrice>(entity =>
        {
            entity.ToTable("product_prices");
            entity.HasKey(pp => new { pp.Id, pp.OwnerId, pp.OrganizationId, pp.ProductId });
            entity.Property(pp => pp.Id).ValueGeneratedOnAdd();

            entity
                .HasOne<Product>(pp => pp.Product)
                .WithMany(p => p.ProductPrices)
                .HasForeignKey(pp => new { pp.ProductId, pp.OwnerId, pp.OrganizationId });
        });

        return builder;
    }
}