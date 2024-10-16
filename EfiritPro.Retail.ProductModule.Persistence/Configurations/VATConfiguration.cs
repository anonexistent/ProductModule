using EfiritPro.Retail.ProductModule.Models;
using Microsoft.EntityFrameworkCore;

namespace EfiritPro.Retail.ProductModule.Persistence.Configurations;

public static class VATConfiguration
{
    public static ModelBuilder ConfigureVATTAble(this ModelBuilder builder)
    {
        builder.Entity<VAT>(entity =>
        {
            entity.ToTable("vats");
            entity.HasKey(v => v.Id);
            entity.Property(v => v.Id).ValueGeneratedOnAdd();

            entity
                .HasMany<Product>(v => v.Products)
                .WithOne(p => p.VAT)
                .HasForeignKey(p => p.VATId);
        });

        return builder;
    }
}