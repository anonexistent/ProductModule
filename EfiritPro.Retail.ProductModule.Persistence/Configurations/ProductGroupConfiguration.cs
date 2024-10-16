using EfiritPro.Retail.ProductModule.Models;
using Microsoft.EntityFrameworkCore;

namespace EfiritPro.Retail.ProductModule.Persistence.Configurations;

public static class ProductGroupConfiguration
{
    public static ModelBuilder ConfigureProductGroupTable(this ModelBuilder builder)
    {
        builder.Entity<ProductGroup>(entity =>
        {
            entity.ToTable("product_groups");
            entity.HasKey(pg => new { pg.Id, pg.OwnerId, pg.OrganizationId });
            entity.Property(pg => pg.Id).ValueGeneratedOnAdd();

            entity
                .HasMany<Product>(pg => pg.Products)
                .WithOne(p => p.ProductGroup)
                .HasForeignKey(p => new { p.ProductGroupId, p.OwnerId, p.OrganizationId })
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            entity
                .HasOne<ProductGroup>(pg => pg.ParentGroup)
                .WithMany(pg => pg.ChildGroups)
                .HasForeignKey(pg => new { pg.ParentGroupId, pg.OwnerId, pg.OrganizationId })
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);
            
        });

        return builder;
    }
}