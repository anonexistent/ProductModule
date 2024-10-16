using EfiritPro.Retail.ProductModule.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EfiritPro.Retail.ProductModule.Api.Services;

public class ProductPriceSetterService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private const int Delay = 1000 * 30;

    public ProductPriceSetterService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Task.Run(async () =>
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(Delay, cancellationToken);
                using var scope = _serviceProvider.CreateScope();
                var services = scope.ServiceProvider;

                try
                {
                    var db = services.GetRequiredService<ProductDbContext>();
                    var curTime = DateTime.UtcNow;
                    var products = await db.Products
                        .Where(p => p.PriceShouldBeSetInTime != null && p.PriceShouldBeSetInTime > curTime)
                        .Include(product => product.ProductPrices)
                        .ToArrayAsync(cancellationToken);
                    foreach (var product in products)
                    {
                        if (product.ProductPrices.Count == 0)
                        {
                            product.PriceShouldBeSetInTime = null;
                        }
                        else
                        {
                            var priceByTime = product.ProductPrices
                                .OrderByDescending(pp => pp.StartTime)
                                .ToArray();
                            var curProductPrice = priceByTime.FirstOrDefault(pp => pp.StartTime <= curTime);
                            var nextProductPrice = priceByTime.FirstOrDefault(pp => pp.StartTime > curTime);

                            if (curProductPrice is not null)
                            {
                                product.PurchasePrice = curProductPrice.PurchasePrice;
                                product.SellingPrice = curProductPrice.SellingPrice;
                                product.PromoPrice = curProductPrice.PromoPrice;
                            }

                            product.PriceShouldBeSetInTime = nextProductPrice?.StartTime;
                        }

                        db.Products.Update(product);
                    }

                    await db.SaveChangesAsync(cancellationToken);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }, cancellationToken);
        
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}