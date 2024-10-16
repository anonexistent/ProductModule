using EfiritPro.Retail.ProductModule.Models;
using Microsoft.EntityFrameworkCore;

namespace EfiritPro.Retail.ProductModule.Persistence.Configurations;

public static class MarkingTypeConfiguration
{
    public static ModelBuilder ConfigureMarkingTypeTable(this ModelBuilder builder)
    {
        builder.Entity<MarkingType>(entity =>
        {
            entity.ToTable("marking_types");
            entity.HasKey(mt => mt.Id);
            entity.Property(mt => mt.Id).ValueGeneratedOnAdd();

            entity
                .HasMany<Product>(mt => mt.Products)
                .WithOne(p => p.MarkingType)
                .HasForeignKey(p => p.MarkingTypeId);
        });

        return builder;
    }
}