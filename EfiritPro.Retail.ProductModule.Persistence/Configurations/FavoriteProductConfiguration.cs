using EfiritPro.Retail.ProductModule.Models;
using Microsoft.EntityFrameworkCore;

namespace EfiritPro.Retail.ProductModule.Persistence.Configurations;

public static class FavoriteProductConfiguration
{
    public static ModelBuilder ConfigureFavoriteProductTable(this ModelBuilder builder)
    {
        builder.Entity<FavoriteProduct>(entity =>
        {
            entity.ToTable("favorite_products");
            entity.HasKey(fp => new { fp.Id, fp.ProductId, fp.OwnerId, fp.OrganizationId });
            entity.Property(fp => fp.Id).ValueGeneratedOnAdd();

            entity
                .HasOne<Product>(fp => fp.Product)
                .WithMany()
                .HasForeignKey(fp => new { fp.ProductId, fp.OwnerId, fp.OrganizationId });
        });
        
        return builder;
    }
}