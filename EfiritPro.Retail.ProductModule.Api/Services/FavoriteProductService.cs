using EfiritPro.Retail.Packages.Errors.Models;
using EfiritPro.Retail.ProductModule.Models;
using EfiritPro.Retail.ProductModule.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EfiritPro.Retail.ProductModule.Api.Services;

public class FavoriteProductService
{
    private readonly ProductDbContext _db;
    private readonly ProductService _productService;

    public FavoriteProductService(ProductDbContext db, ProductService productService)
    {
        _db = db;
        _productService = productService;
    }

    public async Task<ServiceAnswer<FavoriteProduct>> Get(string productId, string ownerId, string organizationId,
        string? workerId)
    {
        var errors = new List<object>();

        if (!Guid.TryParse(productId, out var productGuid))
            errors.Add(new ServiceFieldError()
            {
                Fields = new[] { "productId" },
                Message = "productId не соответствует формату."
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
        
        if (workerId is not null && !Guid.TryParse(workerId, out var workerGuid))
            errors.Add(new ServiceFieldError()
            {
                Fields = new[] { "workerId" },
                Message = "workerId не соответствует формату."
            });
        
        if (errors.Count > 0)
            return new ServiceAnswer<FavoriteProduct>()
            {
                Ok = false,
                Errors = errors,
            };

        return await Get(productGuid, ownerGuid, organizationGuid,
            workerId is not null ? Guid.Parse(workerId) : null);
    }

    private async Task<ServiceAnswer<FavoriteProduct>> Get(Guid productId, Guid ownerId, Guid organizationId, Guid? workerId)
    {
        var favoriteProduct = await _db.FavoriteProducts
            .FirstOrDefaultAsync(fp => fp.ProductId == productId &&
                                       fp.OwnerId == ownerId &&
                                       fp.OrganizationId == organizationId &&
                                       fp.WorkerId == workerId);
        
        return favoriteProduct is not null
            ? new ServiceAnswer<FavoriteProduct>()
            {
                Ok = true,
                Answer = favoriteProduct
            }
            : new ServiceAnswer<FavoriteProduct>()
            {
                Ok = false,
                Errors = new[]
                {
                    new ServiceFieldError()
                    {
                        Fields = new[] { "productId", "ownerId", "organizationId", "workerId" },
                        Message = "Избранный продукт не найден."
                    }
                }
            };
    }

    public async Task<ServiceAnswer<ICollection<FavoriteProduct>>> GetList(string ownerId, string organizationId, string? workerId)
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
        
        if (workerId is not null && !Guid.TryParse(workerId, out var workerGuid))
            errors.Add(new ServiceFieldError()
            {
                Fields = new[] { "workerId" },
                Message = "workerId не соответствует формату."
            });
        
        if (errors.Count > 0)
            return new ServiceAnswer<ICollection<FavoriteProduct>>()
            {
                Ok = false,
                Errors = errors,
            };

        return await GetList(ownerGuid, organizationGuid, workerId is not null ? Guid.Parse(workerId) : null);
    }

    public async Task<ServiceAnswer<ICollection<FavoriteProduct>>> GetList(Guid ownerId, Guid organizationId, Guid? workerId)
    {
        var favoriteProducts = await _db.FavoriteProducts
            .Where(fp => fp.OwnerId == ownerId &&
                         fp.OrganizationId == organizationId &&
                         (fp.WorkerId == workerId || fp.WorkerId == null))
            .ToArrayAsync();

        var favoriteProductsIdList = favoriteProducts
            .Select(fp => fp.ProductId)
            .ToArray();

        var productsIdList = (await _db.Products
            .Where(p => p.OwnerId == ownerId &&
                        p.OrganizationId == organizationId &&
                        !p.Hidden &&
                        favoriteProductsIdList.Contains(p.Id))
            .ToArrayAsync())
            .Select(p => p.Id)
            .ToHashSet();
        
        return new ServiceAnswer<ICollection<FavoriteProduct>>()
        {
            Ok = true,
            Answer = favoriteProducts
                .Where(fp => productsIdList.Contains(fp.ProductId))
                .ToArray()
        };
    }

    public async Task<ServiceAnswer<FavoriteProduct>> Create(string productId, string ownerId, string organizationId, string? workerId)
    {
        var errors = new List<object>();

        if (!Guid.TryParse(productId, out var productGuid))
            errors.Add(new ServiceFieldError()
            {
                Fields = new[] { "productId" },
                Message = "productId не соответствует формату."
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
        
        if (workerId is not null && !Guid.TryParse(workerId, out var workerGuid))
            errors.Add(new ServiceFieldError()
            {
                Fields = new[] { "workerId" },
                Message = "workerId не соответствует формату."
            });
        
        if (errors.Count > 0)
            return new ServiceAnswer<FavoriteProduct>()
            {
                Ok = false,
                Errors = errors,
            };

        return await Create(productGuid, ownerGuid, organizationGuid,
            workerId is not null ? Guid.Parse(workerId) : null);
    }

    private async Task<ServiceAnswer<FavoriteProduct>> Create(Guid productId, Guid ownerId, Guid organizationId, 
        Guid? workerId)
    {
        var getFavoriteProduct = await Get(productId, ownerId, organizationId, workerId);
        if (getFavoriteProduct.Ok) return getFavoriteProduct;
        var getProduct = await _productService.GetProductById(productId, ownerId, organizationId);
        if (!getProduct.Ok || getProduct.Answer is null)
            return new ServiceAnswer<FavoriteProduct>()
            {
                Ok = false,
                Errors = new[]
                {
                    new ServiceFieldError()
                    {
                        Fields = new[] { "productId", "ownerId", "organizationId" },
                        Message = "Продукт не найден."
                    }
                }
            };

        var favoriteProduct = new FavoriteProduct()
        {
            ProductId = productId,
            OwnerId = ownerId,
            OrganizationId = organizationId,

            WorkerId = workerId,

            Product = getProduct.Answer,
        };

        await _db.FavoriteProducts.AddAsync(favoriteProduct);
        await _db.SaveChangesAsync();

        getFavoriteProduct.Ok = true;
        getFavoriteProduct.Errors = Array.Empty<object>();
        getFavoriteProduct.Answer = favoriteProduct;

        return getFavoriteProduct;
    }
    
    public async Task<ServiceAnswer<FavoriteProduct>> Remove(string productId, string ownerId, string organizationId, string? workerId)
    {
        var errors = new List<object>();

        if (!Guid.TryParse(productId, out var productGuid))
            errors.Add(new ServiceFieldError()
            {
                Fields = new[] { "productId" },
                Message = "productId не соответствует формату."
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
        
        if (workerId is not null && !Guid.TryParse(workerId, out var workerGuid))
            errors.Add(new ServiceFieldError()
            {
                Fields = new[] { "workerId" },
                Message = "workerId не соответствует формату."
            });
        
        if (errors.Count > 0)
            return new ServiceAnswer<FavoriteProduct>()
            {
                Ok = false,
                Errors = errors,
            };

        return await Remove(productGuid, ownerGuid, organizationGuid,
            workerId is not null ? Guid.Parse(workerId) : null);
    }

    private async Task<ServiceAnswer<FavoriteProduct>> Remove(Guid productId, Guid ownerId, Guid organizationId, 
        Guid? workerId)
    {
        var getFavoriteProduct = await Get(productId, ownerId, organizationId, workerId);
        if (!getFavoriteProduct.Ok || getFavoriteProduct.Answer is null) return getFavoriteProduct;

        _db.FavoriteProducts.Remove(getFavoriteProduct.Answer);
        await _db.SaveChangesAsync();

        return getFavoriteProduct;
    }
}