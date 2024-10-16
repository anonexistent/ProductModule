using EfiritPro.Retail.ProductModule.Models;
using Microsoft.EntityFrameworkCore;

namespace EfiritPro.Retail.ProductModule.Persistence.Configurations;

public static class ProductTypeConfiguration
{
    public static ModelBuilder ConfigureProductTypeTable(this ModelBuilder builder)
    {
        builder.Entity<ProductType>(entity =>
        {
            entity.ToTable("product_types");
            entity.HasKey(pt => pt.Id);
            entity.Property(pt => pt.Id).ValueGeneratedOnAdd();

            entity
                .HasMany<Product>(pt => pt.Products)
                .WithOne(p => p.ProductType)
                .HasForeignKey(p => p.ProductTypeId);
        });

        return builder;
    }
}