using EfiritPro.Retail.Packages.Errors.Models;
using EfiritPro.Retail.ProductModule.Models;
using EfiritPro.Retail.ProductModule.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EfiritPro.Retail.ProductModule.Api.Services;

public class ProductPriceService
{
    private readonly ProductDbContext _productDbContext;
    private readonly ProductService _productService;

    public ProductPriceService(ProductService productService, ProductDbContext productDbContext)
    {
        _productService = productService;
        _productDbContext = productDbContext;
    }

    public async Task<ServiceAnswer<ProductPrice>> Create(string ownerId, string organizationId, string productId, string? createdByPostingId,
        float purchasePrice, float sellingPrice, float promoPrice, DateTime? startTime)
    {
        var errors = new List<object>();
        
        if (!Guid.TryParse(ownerId, out var ownerGuid))
            errors.Add(new ServiceFieldError()
            {
                Fields = new[] { "ownerId" },
                Message = "ownerId не соответствует формату."
            });
        
        if (!Guid.TryParse(organizationId, out var organizationGuid))
            errors.Add(new ServiceFieldError()
            {
                Fields = new[] { "organizationId" },
                Message = "organizationId не соответствует формату."
            });
        
        if (!Guid.TryParse(productId, out var productGuid))
            errors.Add(new ServiceFieldError()
            {
                Fields = new[] { "productId" },
                Message = "productId не соответствует формату."
            });
        
        if (createdByPostingId is not null && !Guid.TryParse(createdByPostingId, out var createdByPostingGuid))
            errors.Add(new ServiceFieldError()
            {
                Fields = new[] { "createByPostingGuid" },
                Message = "createByPostingGuid не соответствует формату."
            });

        if (errors.Count > 0)
            return new ServiceAnswer<ProductPrice>()
            {
                Ok = false,
                Errors = errors,
            };
        
        return await Create(ownerGuid, organizationGuid, productGuid, 
            createdByPostingId is not null ? Guid.Parse(createdByPostingId) : null, 
            purchasePrice, sellingPrice, promoPrice, startTime);
    }

    public async Task<ServiceAnswer<ProductPrice>> Create(Guid ownerId, Guid organizationId, Guid productId, Guid? createdByPostingId,
        float purchasePrice, float sellingPrice, float promoPrice, DateTime? startTime)
    {
        var getProduct = await _productService.GetProductById(productId, ownerId, organizationId);
        if (!getProduct.Ok || getProduct.Answer is null) return new ServiceAnswer<ProductPrice>()
        {
            Ok = false,
            Errors = getProduct.Errors
        };
        if (startTime is not null && startTime < DateTime.UtcNow) return new ServiceAnswer<ProductPrice>()
        {
            Ok = false,
            Errors = new[]
            {
                new ServiceFieldError()
                {
                    Fields = new[] {"startTime"},
                    Message = "Время должно быть позднее, чем настоящее время."
                }
            }
        };
        var product = getProduct.Answer;
        var nowTime = DateTime.UtcNow;
        startTime = startTime?.ToUniversalTime() ?? nowTime.AddMinutes(-1);
        var productPrice = new ProductPrice()
        {
            OwnerId = product.OwnerId,
            OrganizationId = product.OrganizationId,
            ProductId = product.Id,
            
            CreateByPostingId = createdByPostingId,

            Product = product,

            PurchasePrice = purchasePrice,
            SellingPrice = sellingPrice,
            PromoPrice = promoPrice,

            StartTime = startTime.Value,
        };

        await _productDbContext.ProductPrices.AddAsync(productPrice);

        if (startTime.Value <= nowTime)
        {
            product.PurchasePrice = productPrice.PurchasePrice;
            product.SellingPrice = productPrice.SellingPrice;
            product.PromoPrice = productPrice.PromoPrice;
        }
        else
        {
            product.PriceShouldBeSetInTime = DateTime.UtcNow;
        }
        _productDbContext.Products.Update(product);
        
        await _productDbContext.SaveChangesAsync();
        _productDbContext.ChangeTracker.DetectChanges();

        return new ServiceAnswer<ProductPrice>()
        {
            Ok = true,
            Answer = productPrice,
        };
    }

    public async Task<ServiceAnswer<ProductPrice>> Get(string id, string ownerId, string organizationId, string productId)
    {
        var errors = new List<object>();
        
        if (!Guid.TryParse(id, out var guid))
            errors.Add(new ServiceFieldError()
            {
                Fields = new[] { "productPriceId" },
                Message = "productPriceId не соответствует формату."
            });
        
        if (!Guid.TryParse(ownerId, out var ownerGuid))
            errors.Add(new ServiceFieldError()
            {
                Fields = new[] { "ownerId" },
                Message = "ownerId не соответствует формату."
            });
        
        if (!Guid.TryParse(organizationId, out var organizationGuid))
            errors.Add(new ServiceFieldError()
            {
                Fields = new[] { "organizationId" },
                Message = "organizationId не соответствует формату."
            });
        
        if (!Guid.TryParse(productId, out var productGuid))
            errors.Add(new ServiceFieldError()
            {
                Fields = new[] { "productId" },
                Message = "productId не соответствует формату."
            });

        if (errors.Count > 0)
            return new ServiceAnswer<ProductPrice>()
            {
                Ok = false,
                Errors = errors,
            };
        
        return await Get(guid, ownerGuid, organizationGuid, productGuid);
    }

    public async Task<ServiceAnswer<ProductPrice>> Get(Guid id, Guid ownerId, Guid organizationId, Guid productId)
    {
        var productPrice = await _productDbContext.ProductPrices
            .FirstOrDefaultAsync(pp => pp.Id == id &&
                                       pp.OwnerId == ownerId &&
                                       pp.OrganizationId == organizationId &&
                                       pp.ProductId == productId);

        return productPrice is not null
            ? new ServiceAnswer<ProductPrice>()
            {
                Ok = true,
                Answer = productPrice
            }
            : new ServiceAnswer<ProductPrice>()
            {
                Ok = false,
                Errors = new[]
                {
                    new ServiceFieldError()
                    {
                        Fields = new[] { "productPriceId", "ownerId", "organizationId", "productId" },
                        Message = "Цена не найдена."
                    }
                }
            };
    }

    public async Task<ServiceAnswer<ICollection<ProductPrice>>> GetList(string ownerId, string organizationId, string productId)
    {
        var errors = new List<object>();
        
        if (!Guid.TryParse(ownerId, out var ownerGuid))
            errors.Add(new ServiceFieldError()
            {
                Fields = new[] { "ownerId" },
                Message = "ownerId не соответствует формату."
            });
        
        if (!Guid.TryParse(organizationId, out var organizationGuid))
            errors.Add(new ServiceFieldError()
            {
                Fields = new[] { "organizationId" },
                Message = "organizationId не соответствует формату."
            });
        
        if (!Guid.TryParse(productId, out var productGuid))
            errors.Add(new ServiceFieldError()
            {
                Fields = new[] { "productId" },
                Message = "productId не соответствует формату."
            });

        if (errors.Count > 0)
            return new ServiceAnswer<ICollection<ProductPrice>>()
            {
                Ok = false,
                Errors = errors,
            };
        return await GetList(ownerGuid, organizationGuid, productGuid);
    }

    public async Task<ServiceAnswer<ICollection<ProductPrice>>> GetList(Guid ownerId, Guid organizationId, Guid productId)
    {
        return new ServiceAnswer<ICollection<ProductPrice>>()
        {
            Ok = true,
            Answer = await _productDbContext.ProductPrices
                .Where(pp => pp.OwnerId == ownerId &&
                             pp.OrganizationId == organizationId &&
                             pp.ProductId == productId)
                .OrderBy(pp => pp.StartTime)
                .ToArrayAsync(),
        };
    }

    public async Task<ServiceAnswer<ProductPrice>> Update(string id, string ownerId, string organizationId, string productId,
        float purchasePrice, float sellingPrice, float promoPrice, DateTime? startTime)
    {
        
        var errors = new List<object>();
        
        if (!Guid.TryParse(id, out var guid))
            errors.Add(new ServiceFieldError()
            {
                Fields = new[] { "productPriceId" },
                Message = "productPriceId не соответствует формату."
            });
        
        if (!Guid.TryParse(ownerId, out var ownerGuid))
            errors.Add(new ServiceFieldError()
            {
                Fields = new[] { "ownerId" },
                Message = "ownerId не соответствует формату."
            });
        
        if (!Guid.TryParse(organizationId, out var organizationGuid))
            errors.Add(new ServiceFieldError()
            {
                Fields = new[] { "organizationId" },
                Message = "organizationId не соответствует формату."
            });
        
        if (!Guid.TryParse(productId, out var productGuid))
            errors.Add(new ServiceFieldError()
            {
                Fields = new[] { "productId" },
                Message = "productId не соответствует формату."
            });

        if (errors.Count > 0)
            return new ServiceAnswer<ProductPrice>()
            {
                Ok = false,
                Errors = errors,
            };

        return await Update(guid, ownerGuid, organizationGuid, productGuid, purchasePrice, sellingPrice, promoPrice, startTime);
    }

    public async Task<ServiceAnswer<ProductPrice>> Update(Guid id, Guid ownerId, Guid organizationId, Guid productId,
        float purchasePrice, float sellingPrice, float promoPrice, DateTime? startTime)
    {
        var getProductPrice = await Get(id, ownerId, organizationId, productId);
        if (!getProductPrice.Ok || getProductPrice.Answer is null) return getProductPrice;
        if (startTime is not null && startTime < DateTime.UtcNow) return new ServiceAnswer<ProductPrice>()
        {
            Ok = false,
            Errors = new[]
            {
                new ServiceFieldError()
                {
                    Fields = new[] {"startTime"},
                    Message = "Время должно быть позднее, чем настоящее время."
                }
            }
        };

        var productPrice = getProductPrice.Answer;

        productPrice.PurchasePrice = purchasePrice;
        productPrice.SellingPrice = sellingPrice;
        productPrice.PromoPrice = promoPrice;

        var nowTime = DateTime.UtcNow;
        startTime = startTime?.ToUniversalTime() ?? nowTime.AddMinutes(-1);
        productPrice.StartTime = startTime.Value;

        productPrice.Product.PriceShouldBeSetInTime = DateTime.UtcNow;
        
        if (startTime.Value <= nowTime)
        {
            productPrice.Product.PurchasePrice = productPrice.PurchasePrice;
            productPrice.Product.SellingPrice = productPrice.SellingPrice;
            productPrice.Product.PromoPrice = productPrice.PromoPrice;
        }
        _productDbContext.Products.Update(productPrice.Product);

        _productDbContext.ProductPrices.Update(productPrice);
        await _productDbContext.SaveChangesAsync();
        _productDbContext.ChangeTracker.DetectChanges();

        return new ServiceAnswer<ProductPrice>()
        {
            Ok = true,
            Answer = productPrice,
        };
    }

    public async Task<ServiceAnswer<ProductPrice>> Remove(string id, string ownerId, string organizationId, string productId)
    {
        var errors = new List<object>();
        
        if (!Guid.TryParse(id, out var guid))
            errors.Add(new ServiceFieldError()
            {
                Fields = new[] { "productPriceId" },
                Message = "productPriceId не соответствует формату."
            });
        
        if (!Guid.TryParse(ownerId, out var ownerGuid))
            errors.Add(new ServiceFieldError()
            {
                Fields = new[] { "ownerId" },
                Message = "ownerId не соответствует формату."
            });
        
        if (!Guid.TryParse(organizationId, out var organizationGuid))
            errors.Add(new ServiceFieldError()
            {
                Fields = new[] { "organizationId" },
                Message = "organizationId не соответствует формату."
            });
        
        if (!Guid.TryParse(productId, out var productGuid))
            errors.Add(new ServiceFieldError()
            {
                Fields = new[] { "productId" },
                Message = "productId не соответствует формату."
            });

        if (errors.Count > 0)
            return new ServiceAnswer<ProductPrice>()
            {
                Ok = false,
                Errors = errors,
            };
        
        return await Remove(guid, ownerGuid, organizationGuid, productGuid);
    }
    
    public async Task<ServiceAnswer<ProductPrice>> Remove(Guid id, Guid ownerId, Guid organizationId, Guid productId)
    {
        var getProductPrice = await Get(id, ownerId, organizationId, productId);
        if (!getProductPrice.Ok || getProductPrice.Answer is null) return getProductPrice;
        
        var productPrice = getProductPrice.Answer;
        
        _productDbContext.ProductPrices.Remove(productPrice);
        await _productDbContext.SaveChangesAsync();

        return getProductPrice;
    }

    public async Task RemoveByPostingId(string ownerId, string organizationId, string postingId)
    {
        if (Guid.TryParse(ownerId, out var ownerGuid) &&
            Guid.TryParse(organizationId, out var organizationGuid) &&
            Guid.TryParse(postingId, out var postingGuid))
        {
            await RemoveByPostingId(ownerGuid, organizationGuid, postingGuid);
            return;
        }
        
        throw new ArgumentException();
    }

    public async Task RemoveByPostingId(Guid ownerGuid, Guid organizationGuid, Guid postingGuid)
    {
        var priceList = await _productDbContext.ProductPrices
            .Where(pp => pp.OwnerId == ownerGuid &&
                         pp.OrganizationId == organizationGuid &&
                         pp.CreateByPostingId == postingGuid)
            .Include(pp => pp.Product)
            .ToArrayAsync();
        var products = priceList.Select(pp => pp.Product).ToArray();
        
        foreach (var product in products)
        {
            product.PriceShouldBeSetInTime = DateTime.UtcNow;
        }

        _productDbContext.Products.UpdateRange(products);
        _productDbContext.ProductPrices.RemoveRange(priceList);
        await _productDbContext.SaveChangesAsync();
    }
}