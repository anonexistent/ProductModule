using EfiritPro.Retail.ProductModule.Models;
using Microsoft.EntityFrameworkCore;

namespace EfiritPro.Retail.ProductModule.Persistence.Configurations;

public static class UnitConfiguration
{
    public static ModelBuilder ConfigureUnitTable(this ModelBuilder builder)
    {
        builder.Entity<Unit>(entity =>
        {
            entity.ToTable("units");
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Id).ValueGeneratedOnAdd();

            entity
                .HasMany<Product>(u => u.Products)
                .WithOne(p => p.Unit)
                .HasForeignKey(p => p.UnitId);
        });

        return builder;
    }
}