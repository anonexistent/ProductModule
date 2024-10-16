using EfiritPro.Retail.ProductModule.Models;
using Microsoft.EntityFrameworkCore;

namespace EfiritPro.Retail.ProductModule.Persistence.Configurations
{
    public static class ProductSetConfiguration
    {
        public static ModelBuilder ConfigureProductSetTable(this ModelBuilder builder)
        {
            builder.Entity<ProductSet>(entity =>
            {
                entity
                    .HasKey(ps => new { ps.Id, ps.OwnerId,ps.OrganizationId });
                entity.Property(p => p.Id).ValueGeneratedOnAdd();

                entity.HasOne(e => e.Product)
                       .WithMany(p => p.includingProducts)
                       .HasForeignKey(e => new { e.ProductId, e.OwnerId, e.OrganizationId })
                       .OnDelete(DeleteBehavior.Restrict);

                // Настройка внешнего ключа для IncludedProduct
                entity.HasOne(e => e.IncludedProduct)
                       .WithMany(p => p.includedProducts)
                       .HasForeignKey(e => new { e.IncludedProductId, e.OwnerId, e.OrganizationId })
                       .OnDelete(DeleteBehavior.Restrict);
            });

            return builder;
        }
    }
}
