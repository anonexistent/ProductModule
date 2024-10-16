using EfiritPro.Retail.Packages.Rabbit.Interfaces;
using EfiritPro.Retail.Packages.Rabbit.Models;
using EfiritPro.Retail.ProductModule.Models;
using EfiritPro.Retail.ProductModule.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace EfiritPro.Retail.ProductModule.Persistence;

public class ProductDbContext : DbContext, IRabbitDbContext
{
    public required DbSet<Product> Products { get; set; }
    public required DbSet<ProductSet> ProductSets { get; set; }
    public required DbSet<FavoriteProduct> FavoriteProducts { get; set; }
    public required DbSet<ProductGroup> ProductGroups { get; set; }
    public required DbSet<ProductPrice> ProductPrices { get; set; }
    public required DbSet<VAT> VATs { get; set; }
    public required DbSet<ProductType> ProductTypes { get; set; }
    public required DbSet<Unit> Units { get; set; }
    public required DbSet<MarkingType> MarkingTypes { get; set; }

    public required DbSet<RabbitEvent> RabbitEvents { get; set; }

    public ProductDbContext(DbContextOptions options) : base(options) {}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder
            .ConfigureProductTable()
            .ConfigureProductSetTable()
            .ConfigureFavoriteProductTable()
            .ConfigureProductPriceTable()
            .ConfigureProductGroupTable()
            .ConfigureVATTAble()
            .ConfigureProductTypeTable()
            .ConfigureUnitTable()
            .ConfigureMarkingTypeTable();
        
        RabbitDbContextConfiguration.ConfigureRabbitDbContext(modelBuilder);
    }

}