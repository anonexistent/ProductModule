using EfiritPro.Retail.Packages.Errors.Models;
using EfiritPro.Retail.ProductModule.Models;
using EfiritPro.Retail.ProductModule.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EfiritPro.Retail.ProductModule.Api.Services;

public class ProductTypeService
{
    private readonly ProductDbContext _productDbContext;

    public ProductTypeService(ProductDbContext productDbContext)
    {
        _productDbContext = productDbContext;
    }

    public async Task<ServiceAnswer<ProductType>> CreateProductType(string name)
    {
        var productType = new ProductType()
        {
            Name = name
        };

        await _productDbContext.ProductTypes.AddAsync(productType);
        await _productDbContext.SaveChangesAsync();

        return new ServiceAnswer<ProductType>()
        {
            Ok = true,
            Answer = productType,
        };
    }

    public async Task<ServiceAnswer<ProductType>> GetProductTypeById(string id)
    {
        if (Guid.TryParse(id, out var guid))
            return await GetProductTypeById(guid);
        return new ServiceAnswer<ProductType>()
        {
            Ok = false,
            Errors = new[]
            {
                new ServiceFieldError()
                {
                    Fields = new[] { "productTypeId" },
                    Message = "productTypeId не соответствует формату."
                }
            }
        };
    }

    public async Task<ServiceAnswer<ProductType>> GetProductTypeById(Guid id)
    {
        var pt = await _productDbContext.ProductTypes.FirstOrDefaultAsync(pt => pt.Id == id);

        return pt is not null
            ? new ServiceAnswer<ProductType>()
            {
                Ok = true,
                Answer = pt
            }
            : new ServiceAnswer<ProductType>()
            {
                Ok = false,
                Errors = new[]
                {
                    new ServiceFieldError()
                    {
                        Fields = new[] { "productTypeId" },
                        Message = "Тип продукта не найден."
                    }
                }
            };
    }

    public async Task<ServiceAnswer<ICollection<ProductType>>> GetProductTypeList()
    {
        return new ServiceAnswer<ICollection<ProductType>>()
        {
            Ok = true,
            Answer = await _productDbContext.ProductTypes.ToArrayAsync()
        };
    }

    public async Task<ServiceAnswer<ProductType>> UpdateProductType(string id, string name)
    {
        if (Guid.TryParse(id, out var guid))
            return await UpdateProductType(guid, name);
        return new ServiceAnswer<ProductType>()
        {
            Ok = false,
            Errors = new[]
            {
                new ServiceFieldError()
                {
                    Fields = new[] { "productTypeId" },
                    Message = "productTypeId не соответствует формату."
                }
            }
        };
    }

    public async Task<ServiceAnswer<ProductType>> UpdateProductType(Guid id, string name)
    {
        var getProductType = await GetProductTypeById(id);
        if (!getProductType.Ok || getProductType.Answer is null) return getProductType;
        var productType = getProductType.Answer;

        productType.Name = name;
        _productDbContext.ProductTypes.Update(productType);
        await _productDbContext.SaveChangesAsync();

        return new ServiceAnswer<ProductType>()
        {
            Ok = true,
            Answer = productType,
        };
    }

    public async Task RemoveProductType(string id)
    {
        if (Guid.TryParse(id, out var guid)) 
            await RemoveProductType(guid);
    }

    public async Task RemoveProductType(Guid id)
    {
        var productType = (await GetProductTypeById(id)).Answer;

        if (productType is not null)
        {
            _productDbContext.ProductTypes.Remove(productType);
            await _productDbContext.SaveChangesAsync();
        }
    }
}