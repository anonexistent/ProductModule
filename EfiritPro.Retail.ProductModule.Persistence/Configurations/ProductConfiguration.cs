using System.Text.Json;
using EfiritPro.Retail.ProductModule.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EfiritPro.Retail.ProductModule.Persistence.Configurations;

public static class ProductConfiguration
{
    public static ModelBuilder ConfigureProductTable(this ModelBuilder builder)
    {
        builder.Entity<Product>(entity =>
        {
            entity.ToTable("products");
            entity.HasKey(p => new {p.Id, p.OwnerId, p.OrganizationId});
            entity.Property(p => p.Id).ValueGeneratedOnAdd();


            entity.HasMany(p => p.includingProducts)
                .WithOne(ps => ps.Product)
                .HasForeignKey(ps => new { ps.ProductId, ps.OwnerId, ps.OrganizationId })
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(p => p.includedProducts)
                  .WithOne(ps => ps.IncludedProduct)
                  .HasForeignKey(ps => new { ps.IncludedProductId, ps.OwnerId, ps.OrganizationId })
                  .OnDelete(DeleteBehavior.Restrict);

            entity.Property(p => p.CompositionList)
                .HasConversion(v => JsonSerializer.Serialize(v, new JsonSerializerOptions()),
                    v => JsonSerializer.Deserialize<List<Guid>>(v, new JsonSerializerOptions()) ??
                         new List<Guid>(),
                    new ValueComparer<List<Guid>>(
                        (p1, p2) => p2 != null && p1 != null && p1.SequenceEqual(p2),
                        p => p.GetHashCode(),
                        p => p.ToList()))
                .IsRequired(true)
                .HasDefaultValue(new List<Guid>());

            entity
                .HasOne<VAT>(p => p.VAT)
                .WithMany(v => v.Products)
                .HasForeignKey(p => p.VATId);

            entity
                .HasOne<ProductType>(p => p.ProductType)
                .WithMany(pt => pt.Products)
                .HasForeignKey(p => p.ProductTypeId);

            entity
                .HasOne<Unit>(p => p.Unit)
                .WithMany(u => u.Products)
                .HasForeignKey(p => p.UnitId);

            entity
                .HasOne<MarkingType>(p => p.MarkingType)
                .WithMany(mt => mt.Products)
                .HasForeignKey(p => p.MarkingTypeId);

            entity
                .HasOne<ProductGroup>(p => p.ProductGroup)
                .WithMany(pg => pg.Products)
                .HasForeignKey(p => new { p.ProductGroupId, p.OwnerId, p.OrganizationId })
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            entity
                .HasMany<ProductPrice>(p => p.ProductPrices)
                .WithOne(pp => pp.Product)
                .HasForeignKey(pp => new { pp.ProductId, pp.OwnerId, pp.OrganizationId });
        });

        return builder;
    }
}