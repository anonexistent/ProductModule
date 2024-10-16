using EfiritPro.Retail.Packages.Errors.Models;
using EfiritPro.Retail.Packages.Rabbit.Events;
using EfiritPro.Retail.Packages.Rabbit.Interfaces;
using EfiritPro.Retail.Packages.Rabbit.Models;
using EfiritPro.Retail.ProductModule.InputContracts;
using EfiritPro.Retail.ProductModule.Models;
using EfiritPro.Retail.ProductModule.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace EfiritPro.Retail.ProductModule.Api.Services;

public class ProductService
{
    private readonly ProductDbContext _productDbContext;
    private readonly IRabbitPublisherService<ProductDbContext> _publisherService;

    private readonly ProductGroupService _productGroupService;
    private readonly VATService _vatService;
    private readonly ProductTypeService _productTypeService;
    private readonly UnitService _unitService;
    private readonly MarkingTypeService _markingTypeService;

    private readonly ICollection<string> _removeProductEventQueueList = new[]
    {
        "prodMove/removeProduct",
    };

    public ProductService(ProductDbContext productDbContext, IRabbitPublisherService<ProductDbContext> publisherService,
        VATService vatService, ProductTypeService productTypeService, UnitService unitService,
        MarkingTypeService markingTypeService, ProductGroupService productGroupService)
    {
        _productDbContext = productDbContext;
        _publisherService = publisherService;
        _vatService = vatService;
        _productTypeService = productTypeService;
        _unitService = unitService;
        _markingTypeService = markingTypeService;
        _productGroupService = productGroupService;
    }

    public async Task<ServiceAnswer<ICollection<Product>>> CreateProductSome(string ownerId, string organizationId, ICollection<CreateProductBody> rawProducts)
    {
        var products = new List<Product>();
        var errors = new List<object>();

        foreach (var body in rawProducts)
        {
            var temp = await CreateProduct(ownerId, organizationId, body.Name, body.VendorCode, body.BarCode, body.Description, body.Excise, body.ProductGroupId,
                body.VATId, body.ProductTypeId, body.UnitId, body.MarkingTypeId,
                body.PriceMayChangeInCashReceipt, body.PriceMinInCashReceipt, body.PriceMaxInCashReceipt,
                body.IsAllowedToMove, body.CompositionList);

            if (!temp.Ok || temp.Answer is null) errors.Add(temp.Errors);
            else products.Add(temp.Answer);
        }

        if(errors.Count != rawProducts.Count)
        {
            return new ServiceAnswer<ICollection<Product>>()
            {
                Ok = true,
                Answer = products,
                Errors = errors
            };
        }

        return new ServiceAnswer<ICollection<Product>>()
        { 
            Ok = false, 
            Errors = new[]
            {
                new ServiceFieldError()
                {
                    Fields = new[] { "" },
                    Message = "Отмена."
                }
            }
        };
    }

    public async Task<ServiceAnswer<Product>> CreateProduct(string ownerId, string organizationId,
        string name, string vendorCode, string? barCode, string? description, bool excise, string? productGroupId,
        string vatId, string productTypeId, string unitId, string? markingTypeId,
        bool priceMayChangeInCashReceipt, float priceMinInCashReceipt, float priceMaxInCashReceipt,
        bool isAllowedToMove, List<string> compositionList)
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

        if (productGroupId is not null && !Guid.TryParse(productGroupId, out var productGroupGuid))
            errors.Add(new ServiceFieldError()
            {
                Fields = new[] { "productGroupId" },
                Message = "productGroupId не соответствует формату."
            });

        if (!Guid.TryParse(vatId, out var vatGuid))
            errors.Add(new ServiceFieldError()
            {
                Fields = new[] { "vatId" },
                Message = "vatId не соответствует формату."
            });

        if (!Guid.TryParse(productTypeId, out var productTypeGuid))
            errors.Add(new ServiceFieldError()
            {
                Fields = new[] { "productTypeId" },
                Message = "productTypeId не соответствует формату."
            });

        if (!Guid.TryParse(unitId, out var unitGuid))
            errors.Add(new ServiceFieldError()
            {
                Fields = new[] { "unitId" },
                Message = "unitId не соответствует формату."
            });

        if (markingTypeId is not null && !Guid.TryParse(markingTypeId, out var markingTypeGuid))
            errors.Add(new ServiceFieldError()
            {
                Fields = new[] { "markingTypeId" },
                Message = "markingTypeId не соответствует формату."
            });

        var compositionGuidList = new List<Guid>(compositionList.Count);

        foreach (var composition in compositionList)
        {
            if (Guid.TryParse(composition, out var compositionGuid))
            {
                compositionGuidList.Add(compositionGuid);
            }
            else
            {
                errors.Add(new ServiceFieldError()
                {
                    Fields = new[] { "compositionList" },
                    Message = "compositionList не соответствует формату."
                });
                break;
            }
        }

        if (errors.Count > 0)
            return new ServiceAnswer<Product>()
            {
                Ok = false,
                Errors = errors,
            };

        return await CreateProduct(ownerGuid, organizationGuid, name, vendorCode, barCode, description, excise,
            productGroupId is not null ? Guid.Parse(productGroupId) : null,
            vatGuid, productTypeGuid, unitGuid,
            markingTypeId is not null ? Guid.Parse(markingTypeId) : null,
            priceMayChangeInCashReceipt, priceMinInCashReceipt, priceMaxInCashReceipt,
            isAllowedToMove, compositionGuidList);
    }

    public async Task<ServiceAnswer<Product>> CreateProduct(Guid ownerId, Guid organizationId,
        string name, string vendorCode, string? barCode, string? description, bool excise,
        Guid? productGroupId, Guid vatId, Guid productTypeId, Guid unitId, Guid? markingTypeId,
        bool priceMayChangeInCashReceipt, float priceMinInCashReceipt, float priceMaxInCashReceipt,
        bool isAllowedToMove, List<Guid> compositionList)
    {
        var errors = new List<object>();
        ProductGroup? productGroup = null;
        VAT? vat = null;
        ProductType? productType = null;
        Unit? unit = null;
        MarkingType? markingType = null;

        if (productGroupId is not null)
        {
            var getProductGroup = await _productGroupService.Get(productGroupId.Value, ownerId, organizationId);
            if (!getProductGroup.Ok || getProductGroup.Answer is null)
            {
                errors.AddRange(getProductGroup.Errors);
            }
            else
            {
                productGroup = getProductGroup.Answer;
            }
        }

        if (barCode is not null)
        {
            var checkBar = await GetProductByBarCode(barCode, organizationId, ownerId);

            if(checkBar.Answer is not null)
            {
                errors.Add(
                    new ServiceFieldError()
                    {
                        Fields = new[] { "barCode" },
                        Message = "Штрих-код должен быть уникальным."
                    }
                );
            }
        }

        if(vendorCode is not null)
        {
            var checkVendor = await GetProductByVendorCode(vendorCode, organizationId, ownerId);

            if(checkVendor.Answer is not null)
            {
                errors.Add(
                    new ServiceFieldError()
                    {
                        Fields = new[] { "vendorCode" },
                        Message = $"Артикул должен быть уникальным. ({vendorCode})"
                    }
                );
            }
        }

        var getVAT = await _vatService.GetVATById(vatId);
        if (!getVAT.Ok || getVAT.Answer is null)
        {
            errors.AddRange(getVAT.Errors);
        }
        else
        {
            vat = getVAT.Answer;
        }

        var getProductType = await _productTypeService.GetProductTypeById(productTypeId);
        if (!getProductType.Ok || getProductType.Answer is null)
        {
            errors.AddRange(getProductType.Errors);
        }
        else
        {
            productType = getProductType.Answer;
        }

        var getUnit = await _unitService.GetUnitById(unitId);
        if (!getUnit.Ok || getUnit.Answer is null)
        {
            errors.AddRange(getUnit.Errors);
        }
        else
        {
            unit = getUnit.Answer;
        }

        if (markingTypeId is not null)
        {
            var getMarkingType = await _markingTypeService.GetMarkingTypeById(markingTypeId.Value);
            if (!getMarkingType.Ok || getProductType.Answer is null)
            {
                errors.AddRange(getMarkingType.Errors);
            }
            else
            {
                markingType = getMarkingType.Answer;
            }
        }

        if (errors.Count > 0 || vat is null || productType is null || unit is null)
            return new ServiceAnswer<Product>()
            {
                Ok = false,
                Errors = errors
            };

        var product = new Product()
        {
            Id = Guid.Empty,
            OwnerId = ownerId,
            OrganizationId = organizationId,

            Name = name,
            VendorCode = vendorCode,
            BarCode = barCode,
            Description = description,
            Excise = excise,
            Hidden = false,

            PriceMayChangeInCashReceipt = priceMayChangeInCashReceipt,
            PriceMinInCashReceipt = priceMinInCashReceipt,
            PriceMaxInCashReceipt = priceMaxInCashReceipt,

            CompositionList = compositionList,

            VATId = vatId,
            VAT = vat,

            ProductGroup = productGroup,
            ProductGroupId = productGroup?.Id,

            ProductTypeId = productTypeId,
            ProductType = productType,

            UnitId = unitId,
            Unit = unit,

            MarkingTypeId = markingTypeId,
            MarkingType = markingType,

            IsAllowedToMove = isAllowedToMove,

            //ParentProductId = parentProductId,
            //ParentProduct = parentProduct,
        };

        _productDbContext.Products.Add(product);
        await _productDbContext.SaveChangesAsync();

        return new ServiceAnswer<Product>()
        {
            Ok = true,
            Answer = product,
        };
    }

    public async Task<ServiceAnswer<Product>> GetProductByVendorCode(string vendorCode, Guid organizationId, Guid ownerId)
    {
        var product = await _productDbContext.Products.FirstOrDefaultAsync(x=>x.OwnerId==ownerId && 
        x.OrganizationId==organizationId && x.VendorCode == vendorCode && !x.Hidden);

        return product is not null ?
            new ServiceAnswer<Product>()
            {
                Ok = true,
                Answer = product
            }
            :
            new ServiceAnswer<Product>()
            {
                Ok = false,
                Errors = new[]
                {
                    new ServiceFieldError()
                    {
                        Fields = new[] {"vendorCode"},
                        Message = "Артикул не найден."
                    }
                }
            };
    }

    public async Task<ServiceAnswer<Product>> GetProductByBarCode(string barCode, Guid organizationId, Guid ownerId)
    {
        var product = await _productDbContext.Products.FirstOrDefaultAsync(x => x.OwnerId == ownerId 
        && x.OrganizationId == organizationId && x.BarCode == barCode && !x.Hidden);

        return product is not null ?
            new ServiceAnswer<Product>()
            {
                Ok = true,
                Answer = product
            }
            :
            new ServiceAnswer<Product>()
            {
                Ok = false,
                Errors = new[]
                {
                    new ServiceFieldError()
                    {
                        Fields = new[] {"vendorCode"},
                        Message = "Штрих-код не найден."
                    }
                }
            };
    }

    public async Task<ServiceAnswer<Product>> GetProductById(string id, string ownerId, string organizationId)
    {
        var errors = new List<object>();

        if (!Guid.TryParse(id, out var guid))
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

        if (errors.Count > 0)
            return new ServiceAnswer<Product>()
            {
                Ok = false,
                Errors = errors,
            };

        return await GetProductById(guid, ownerGuid, organizationGuid);
    }

    public async Task<ServiceAnswer<Product>> GetProductById(Guid id, Guid ownerId, Guid organizationId)
    {
        var product = await _productDbContext.Products
            .Include(x => x.ProductGroup)
            .Include(x => x.includedProducts)
            .FirstOrDefaultAsync(p => p.Id == id && p.OwnerId == ownerId && p.OrganizationId == organizationId);

        return product is not null
            ? new ServiceAnswer<Product>()
            {
                Ok = true,
                Answer = product
            }
            : new ServiceAnswer<Product>()
            {
                Ok = false,
                Errors = new[]
                {
                    new ServiceFieldError()
                    {
                        Fields = new[] { "productId", "ownerId", "organizationId" },
                        Message = $"Продукт не найден."
                    }
                }
            };
    }

    public async Task<ServiceAnswer<int>> GetProductsCount(string ownerId, string organizationId)
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

        if (errors.Count > 0)
            return new ServiceAnswer<int>()
            {
                Ok = false,
                Errors = errors,
            };

        return await GetProductsCount(ownerGuid, organizationGuid);
    }

    public async Task<ServiceAnswer<int>> GetProductsCount(Guid ownerId, Guid organizationId)
    {
        var produtcs = await _productDbContext.Products.Where(p => p.OwnerId == ownerId && p.OrganizationId == organizationId).ToArrayAsync();

        return new ServiceAnswer<int>()
        {
            Ok = true,
            Answer = produtcs.Length
            //.Where(p => p.OwnerId == ownerId && p.OrganizationId == organizationId && !p.Hidden)
            //.ToArrayAsync()
        };
    }

    public async Task<ServiceAnswer<int>> GetProductListCount(string ownerId, string organizationId, bool? isHidden = null)
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

        if (errors.Count > 0)
            return new ServiceAnswer<int>()
            {
                Ok = false,
                Errors = errors,
            };

        return await GetProductListCount(ownerGuid, organizationGuid, isHidden);
    }

    public async Task<ServiceAnswer<int>> GetProductListCount(Guid ownerId, Guid organizationId, bool? isHidden = null)
    {
        return new ServiceAnswer<int>()
        {
            Ok = true,
            Answer = _productDbContext.Products
                .Where(p => p.OwnerId == ownerId && p.OrganizationId == organizationId
                    && (isHidden == null ? p.Hidden == false : true))
                .Count()
        };
    }
    public async Task<ServiceAnswer<ICollection<Product>>> GetProductList(string ownerId, string organizationId, bool? isHidden = null)
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
        
        if (errors.Count > 0)
            return new ServiceAnswer<ICollection<Product>>()
            {
                Ok = false,
                Errors = errors,
            };
        
        return await GetProductList(ownerGuid, organizationGuid, isHidden);
    }

    public async Task<ServiceAnswer<ICollection<Product>>> GetProductList(Guid ownerId, Guid organizationId, bool? isHidden = null)
    {
        return new ServiceAnswer<ICollection<Product>>()
        {
            Ok = true,
            Answer = await _productDbContext.Products
                .Where(p => p.OwnerId == ownerId && p.OrganizationId == organizationId 
                    && (isHidden==null?p.Hidden==false:true))
                .ToArrayAsync()
        };
    }
    
    public async Task<ServiceAnswer<ICollection<Product>>> GetProductListByName(string productName, string ownerId, string organizationId, bool? isHidden = null)
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
        
        if (errors.Count > 0)
            return new ServiceAnswer<ICollection<Product>>()
            {
                Ok = false,
                Errors = errors,
            };
        
        return await GetProductListByName(productName, ownerGuid, organizationGuid, isHidden);
    }

    public async Task<ServiceAnswer<ICollection<Product>>> GetProductListByName(string productName, Guid ownerId, Guid organizationId, bool? isHidden = null)
    {
        //var allProducts = _productDbContext.Products.Where(p => p.OwnerId == ownerId && p.OrganizationId == organizationId
        //            && (isHidden == null ? p.Hidden == false : true) || p.Name.Contains(productName));
        var tempList = new List<Product>();

        //foreach (var item in allProducts)
        //{
        //    tempList.Add(item);
        //    var childList = item.includingProducts;
        //    foreach (var c in childList)
        //    {
        //        if(c is not null && !c.IncludedProduct.Hidden && c.IncludedProduct.Name.Contains(productName))
        //        {
        //            tempList.Add(c.IncludedProduct);
        //        }
        //    }            
        //}

        var x = await _productDbContext.Products.Where(p => p.OwnerId == ownerId
                        && p.OrganizationId == organizationId
                        && (isHidden == null ? p.Hidden == false : true)).ToListAsync();

        foreach (var item in x)
        {
            if (item.Name.Contains(productName)) tempList.Add(item);
            else
            {
                if (
                    item.includingProducts.Count > 0
                    && item.includingProducts.Any(x => x is not null)
                    && item.includingProducts.Any(y => y.IncludedProduct is not null)
                    )
                {
                    foreach (ProductSet c in item.includingProducts)
                    {
                        if (c is not null && c.IncludedProduct is not null && c.IncludedProduct.Name.Contains(productName)) tempList.Add(item);
                    }
                }
            }
        }

        var result = new HashSet<Product>();

        foreach (var product in x)
        {
            if (product.OwnerId == ownerId && product.OrganizationId == organizationId && !product.Hidden)
            {
                if (product.Name.Contains(productName, StringComparison.OrdinalIgnoreCase))
                {
                    result.Add(product);
                }

                if (product.includingProducts.Count > 0)
                {
                    if (SearchInIncludingProducts(product.includingProducts, productName))
                    {
                        result.Add(product);
                    }
                }
            }
        }

        return new ServiceAnswer<ICollection<Product>>()
        {
            Ok = true,
            Answer = 
            //await _productDbContext.Products
            //    .Where(p => p.OwnerId == ownerId && p.OrganizationId == organizationId 
            //        && p.Name.Contains(productName) && (isHidden == null ? p.Hidden == false : true)
            //        || (
            //            p.includingProducts!=null & p.includingProducts.Any(c=>c.IncludedProduct.Name.Contains(productName))
            //            )
            //        )
            //    .ToArrayAsync()
            result
        };
    }

    private static bool SearchInIncludingProducts(ICollection<ProductSet> includingProducts, string searchName)
    {
        foreach (var productSet in includingProducts)
        {
            if (productSet.IncludedProduct.Name.Contains(searchName, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (productSet.IncludedProduct.includingProducts.Count > 0)
            {
                if (SearchInIncludingProducts(productSet.IncludedProduct.includingProducts, searchName))
                {
                    return true;
                }
            }
        }

        return false;
    }

    public async Task<ServiceAnswer<Product>> UpdateProduct(string id, string ownerId, string organizationId,
        string name, string vendorCode, string? barCode, string? description, bool excise, bool hidden, 
        string? productGroupId, string vatId, string productTypeId, string unitId, string? markingTypeId,
        bool priceMayChangeInCashReceipt, float priceMinInCashReceipt, float priceMaxInCashReceipt, 
        List<string> compositionList)
    {
        var errors = new List<object>();
        
        if (!Guid.TryParse(id, out var guid))
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
        
        if (productGroupId is not null && !Guid.TryParse(productGroupId, out var productGroupGuid))
            errors.Add(new ServiceFieldError()
            {
                Fields = new[] { "productGroupId" },
                Message = "productGroupId не соответствует формату."
            });
        
        if (!Guid.TryParse(vatId, out var vatGuid))
            errors.Add(new ServiceFieldError()
            {
                Fields = new[] { "vatId" },
                Message = "vatId не соответствует формату."
            });
        
        if (!Guid.TryParse(productTypeId, out var productTypeGuid))
            errors.Add(new ServiceFieldError()
            {
                Fields = new[] { "productTypeId" },
                Message = "productTypeId не соответствует формату."
            });
        
        if (!Guid.TryParse(unitId, out var unitGuid))
            errors.Add(new ServiceFieldError()
            {
                Fields = new[] { "unitId" },
                Message = "unitId не соответствует формату."
            });
        
        if (markingTypeId is not null && !Guid.TryParse(markingTypeId, out var markingTypeGuid))
            errors.Add(new ServiceFieldError()
            {
                Fields = new[] { "markingTypeId" },
                Message = "markingTypeId не соответствует формату."
            });

        var compositionGuidList = new List<Guid>(compositionList.Count);
        
        foreach (var composition in compositionList)
        {
            if (Guid.TryParse(composition, out var compositionGuid))
            {
                compositionGuidList.Add(compositionGuid);
            }
            else
            {
                errors.Add(new ServiceFieldError()
                {
                    Fields = new[] { "compositionList" },
                    Message = "compositionList не соответствует формату."
                });
                break;
            }
        }
        

        if (errors.Count > 0)
            return new ServiceAnswer<Product>()
            {
                Ok = false,
                Errors = errors,
            };
        
        return await UpdateProduct(guid, ownerGuid, organizationGuid, name, vendorCode, barCode, description, excise, hidden,
            productGroupId is not null ? Guid.Parse(productGroupId) : null,
            vatGuid, productTypeGuid, unitGuid,
            markingTypeId is not null ? Guid.Parse(markingTypeId) : null,
            priceMayChangeInCashReceipt, priceMinInCashReceipt, priceMaxInCashReceipt,
            compositionGuidList);
    }

    public async Task<ServiceAnswer<Product>> UpdateProduct(Guid id, Guid ownerId, Guid organizationId, 
        string name, string vendorCode, string? barCode, string? description, bool excise, bool hidden,
        Guid? productGroupId, Guid vatId, Guid productTypeId, Guid unitId, Guid? markingTypeId,
        bool priceMayChangeInCashReceipt, float priceMinInCashReceipt, float priceMaxInCashReceipt,
        List<Guid> compositionList)
    {
        var getProduct = await GetProductById(id, ownerId, organizationId);
        if (!getProduct.Ok || getProduct.Answer is null) return getProduct;
        var product = getProduct.Answer;
        
        var errors = new List<object>();
        ProductGroup? productGroup = null;
        VAT? vat = null;
        ProductType? productType = null;
        Unit? unit = null;
        MarkingType? markingType = null;

        if (productGroupId is not null)
        {
            var getProductGroup = await _productGroupService.Get(productGroupId.Value, ownerId, organizationId);
            if (!getProductGroup.Ok || getProductGroup.Answer is null)
            {
                errors.AddRange(getProductGroup.Errors);
            }
            else
            {
                productGroup = getProductGroup.Answer;
            }
        }

        if (productGroupId is not null)
        {
            var getProductGroup = await _productGroupService.Get(productGroupId.Value, ownerId, organizationId);
            if (!getProductGroup.Ok || getProductGroup.Answer is null)
            {
                errors.AddRange(getProductGroup.Errors);
            }
            else
            {
                productGroup = getProductGroup.Answer;
            }
        }

        if (barCode is not null)
        {
            var checkBar = await GetProductByBarCode(barCode, organizationId, ownerId);

            if (checkBar.Answer is not null && checkBar.Answer.Id != id)
            {
                errors.Add(
                    new ServiceFieldError()
                    {
                        Fields = new[] { "barCode" },
                        Message = "Штрих-код должен быть уникальным."
                    }
                );
            }
        }

        if (vendorCode is not null)
        {
            var checkVendor = await GetProductByVendorCode(vendorCode, organizationId, ownerId);

            if (checkVendor.Answer is not null && checkVendor.Answer.Id != id)
            {
                errors.Add(
                    new ServiceFieldError()
                    {
                        Fields = new[] { "vendorCode" },
                        Message = $"Артикул должен быть уникальным. ({vendorCode})"
                    }
                );
            }
        }

        var getVAT = await _vatService.GetVATById(vatId);
        if (!getVAT.Ok || getVAT.Answer is null)
        {
            errors.AddRange(getVAT.Errors);
        }
        else
        {
            vat = getVAT.Answer;
        }
        
        var getProductType = await _productTypeService.GetProductTypeById(productTypeId);
        if (!getProductType.Ok || getProductType.Answer is null)
        {
            errors.AddRange(getProductType.Errors);
        }
        else
        {
            productType = getProductType.Answer;
        }
        
        var getUnit = await _unitService.GetUnitById(unitId);
        if (!getUnit.Ok || getUnit.Answer is null)
        {
            errors.AddRange(getUnit.Errors);
        }
        else
        {
            unit = getUnit.Answer;
        }

        if (markingTypeId is not null)
        {
            var getMarkingType = await _markingTypeService.GetMarkingTypeById(markingTypeId.Value);
            if (!getMarkingType.Ok || getProductType.Answer is null)
            {
                errors.AddRange(getMarkingType.Errors);
            }
            else
            {
                markingType = getMarkingType.Answer;
            }
        }
        
        if (vat is null || productType is null || unit is null || errors.Count > 0)
            return new ServiceAnswer<Product>()
            {
                Ok = false,
                Errors = errors
            };
        
        product.Name = name;
        product.VendorCode = vendorCode;
        product.BarCode = barCode;
        product.Description = description;
        product.Excise = excise;
        product.Hidden = hidden;

        product.PriceMayChangeInCashReceipt = priceMayChangeInCashReceipt;
        product.PriceMinInCashReceipt = priceMinInCashReceipt;
        product.PriceMaxInCashReceipt = priceMaxInCashReceipt;
        
        product.CompositionList = compositionList;

        product.VAT = vat;
        product.VATId = vatId;

        product.ProductGroupId = productGroup?.Id;
        product.ProductGroup = productGroup;
            
        product.ProductTypeId = productTypeId;
        product.ProductType = productType;
            
        product.UnitId = unitId;
        product.Unit = unit;
            
        product.MarkingTypeId = markingTypeId;
        product.MarkingType = markingType;

        _productDbContext.Products.Update(product);
        await _productDbContext.SaveChangesAsync();

        return new ServiceAnswer<Product>()
        {
            Ok = true,
            Answer = product,
        };
    }
    
    public async Task<ServiceAnswer<ICollection<Product>>> RelocateProductSome(string? newGroupId, string ownerId, string organizationId, ICollection<string> productIds)
    {
        var errors = new List<object>();
        var productGuids = new List<Guid>();

        if (!Guid.TryParse(newGroupId, out var newGroupGuid) && newGroupId is not null)
        {
            errors.Add(new ServiceFieldError()
            {
                Fields = new[] { "newGroupId" },
                Message = "newGroupId не соответствует формату."
            });
        }

        if(!Guid.TryParse(ownerId, out var ownerGuid))
        {
            errors.Add(new ServiceFieldError()
            {
                Fields = new[] { "ownerId" },
                Message = "ownerId не соответствует формату."
            });
        }

        if(!Guid.TryParse(organizationId, out var organizationGuid))
        {
            errors.Add(new ServiceFieldError()
            {
                Fields = new[] { "organizationId" },
                Message = "organizationId не соответствует формату."
            });
        }

        foreach (var item in productIds)
        {
            if(!Guid.TryParse(item, out var productGuid))
            {
                errors.Add(new ServiceFieldError()                    
                {
                    Fields = new[] {"productIds"},
                    Message = $"productId не соответсвует формату. ({productGuid})"
                });
            }
            else
            {
                productGuids.Add(productGuid);
            }
        }

        if(errors.Count > 0)
        {
            return new ServiceAnswer<ICollection<Product>>()
            {
                Ok = false,
                Errors = errors
            };
        }

        return await RelocateProductSome(newGroupId is null ? null : newGroupGuid, ownerGuid, organizationGuid, productGuids);
    }

    public async Task<ServiceAnswer<ICollection<Product>>> RelocateProductSome(Guid? newGroupId, Guid ownerId, Guid organizatonId, ICollection<Guid> productIds)
    {
        var errors = new List<object>();
        var prdoucts = new List<Product>();
        ProductGroup newGroup = null;

        if(newGroupId.HasValue)
        {
            var rawNewGroup = await _productGroupService.Get(newGroupId.Value, ownerId, organizatonId);

            if (!rawNewGroup.Ok || rawNewGroup.Answer is null)
            {
                return new ServiceAnswer<ICollection<Product>>()
                {
                    Ok = false,
                    Errors = new[]
                    {
                        new ServiceFieldError()
                        {
                            Fields = new[] { "newGroupId" },
                            Message = "Целевая папка не найдена."
                        }
                    }
                };
            }
            else
            {
                newGroup = rawNewGroup.Answer;
            }
        }

        foreach (var id in productIds)
        {
            var tempProduct = await GetProductById(id, ownerId, organizatonId);
            if(tempProduct.Ok && tempProduct.Answer is not null)
            {
                tempProduct.Answer.ProductGroup = newGroup;
                tempProduct.Answer.ProductGroupId = newGroupId;

                prdoucts.Add(tempProduct.Answer);

                _productDbContext.Products.Update(tempProduct.Answer);
            }
            else
            {
                errors.Add(tempProduct.Errors.Select(x=>(x as ServiceFieldError).Message += $"({id})"));
            }
        }

        await _productDbContext.SaveChangesAsync();

        if(errors.Count != productIds.Count)
        {
            return new ServiceAnswer<ICollection<Product>>()
            {
                Ok = true,
                Answer = prdoucts,
                Errors = errors
            };
        }

        return new ServiceAnswer<ICollection<Product>>()
        {
            Ok = false,
            Errors = new[] 
            { 
                new ServiceFieldError() 
                { 
                    Fields = new[] 
                    { 
                        "newGroupId", "ownerId", "organizatonId", "productIds" 
                    },
                    Message = "Отмена."
                } 
            }
        };
    }

    public async Task<ServiceAnswer<ICollection<Product>>> HideProduct(string id, string ownerId, string organizationId, bool sendEvent = false)
    {
        var errors = new List<object>();
        
        if (!Guid.TryParse(id, out var guid))
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

        if (errors.Count > 0)
            return new ServiceAnswer<ICollection<Product>>()
            {
                Ok = false,
                Errors = errors,
            };
        
        return await HideProduct(guid, ownerGuid, organizationGuid, sendEvent);
    }

    public async Task<ServiceAnswer<ICollection<Product>>> HideProduct(Guid id, Guid ownerId, Guid organizationId, bool sendEvent = false)
    {
        var rawProduct = await GetProductById(id, ownerId, organizationId);
        var product = rawProduct.Answer;
        // var events = new List<RabbitEvent>();

        if (product is not null)
        {
            product.Hidden = true;

            var group = _productDbContext.ProductGroups.FirstOrDefault(x => x.Name == "deletedProducts");
            product.ProductGroup = null;
                        
            _productDbContext.Products.Update(product);

            // if (sendEvent)
            // {
            //     foreach (var dest in _removeProductEventQueueList) events.Add(await _publisherService.CreateRabbitEvent(dest, new OutputMQProductEvent()
            //     {
            //         EventId = string.Empty,
            //         AckDestination = string.Empty,
            //         ProductId = product.Id.ToString(),
            //         ownerId = product.ownerId.ToString(),
            //         OrganizationId = product.OrganizationId.ToString(),
            //     }, false));
            // }

            await _productDbContext.SaveChangesAsync();
        }
        else
        {
            return new ServiceAnswer<ICollection<Product>>()
            { 
                Ok = false, 
                Errors = rawProduct.Errors 
            };
        }
        
        // foreach (var rabbitEvent in events) await _publisherService.SendEvent(rabbitEvent.Id, false);
        // await _productDbContext.SaveChangesAsync();

        return await GetProductList(ownerId, organizationId);
    }

    public async Task<ServiceAnswer<ICollection<Product>>> HideProductSome(string ownerId, string organizationId, List<string> productIds, bool sendEvent = false)
    {
        var errors = new List<object>();
        var products = new List<Product>();

        foreach (var item in productIds)
        {
            var temp = await HideProduct(item, ownerId, organizationId, sendEvent);
            var tempProduct = await GetProductById(item, ownerId, organizationId);

            if (tempProduct.Ok && tempProduct.Answer is not null)
            {
                products.Add(tempProduct.Answer);
            }

            if (!temp.Ok || !tempProduct.Ok)
            {
                errors.Add(temp.Errors);
            }
        }

        if(errors.Count != productIds.Count)
        {
            return new ServiceAnswer<ICollection<Product>>()
            {
                Ok = true,
                Answer = products,
                Errors = errors,
            };  
        }

        return new ServiceAnswer<ICollection<Product>>()
        {
            Ok = false,
            Errors = errors
        };
    }

    public async Task<ServiceAnswer<Product>> RemoveProduct(string id, string ownerId, string organizationId, bool sendEvent = false)
    {
        var errors = new List<object>();
        
        if (!Guid.TryParse(id, out var guid))
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

        if (errors.Count > 0) 
            return new ServiceAnswer<Product>()
            {
                Ok = false,
                Errors = errors,
            };
        
        return await RemoveProduct(guid, ownerGuid, organizationGuid, sendEvent);
    }

    public async Task<ServiceAnswer<Product>> RemoveProduct(Guid id, Guid ownerId, Guid organizationId, bool sendEvent = false)
    {
        var getProduct = await GetProductById(id, ownerId, organizationId);
        if (!getProduct.Ok || getProduct.Answer is null) return getProduct;

        var product = getProduct.Answer;
        var events = new List<RabbitEvent>();

        _productDbContext.Products.Remove(product);

        if (sendEvent)
        {
            foreach (var dest in _removeProductEventQueueList)
                events.Add(await _publisherService.CreateRabbitEvent(_productDbContext, dest, new ProductEvent()
                {
                    EventId = string.Empty,
                    AckDestination = string.Empty,
                    ProductId = product.Id.ToString(),
                    OwnerId = product.OwnerId.ToString(),
                    OrganizationId = product.OrganizationId.ToString(),
                }));
        }
        
        await _productDbContext.SaveChangesAsync();
        
        foreach (var rabbitEvent in events) await _publisherService.SendEvent(_productDbContext, rabbitEvent.Id);
        await _productDbContext.SaveChangesAsync();

        return getProduct;
    }

    public async Task<ServiceAnswer<ICollection<Product>>> RemoveAllOrganizationProducts(string ownerId, string organizationId, bool sendEvent = false)
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

        if (errors.Count > 0) 
            return new ServiceAnswer<ICollection<Product>>()
            {
                Ok = false,
                Errors = errors,
            };
        
        return await RemoveAllOrganizationProducts(ownerGuid, organizationGuid, sendEvent);
    }

    public async Task<ServiceAnswer<ICollection<Product>>> RemoveAllOrganizationProducts(Guid ownerId, Guid organizationId, bool sendEvent = false )
    {
        var getProductList = await GetProductList(ownerId, organizationId);
        if (!getProductList.Ok || getProductList.Answer is null || getProductList.Answer.Count == 0) return getProductList;

        var productList = getProductList.Answer;
        var events = new List<RabbitEvent>();
        
        foreach (var product in productList)
        {
            _productDbContext.Products.Remove(product);
            if (!sendEvent) continue;
            foreach (var dest in _removeProductEventQueueList) 
                events.Add(await _publisherService.CreateRabbitEvent(_productDbContext, dest, new ProductEvent
                {
                    EventId = string.Empty,
                    AckDestination = string.Empty,
                    ProductId = product.Id.ToString(),
                    OwnerId = product.OwnerId.ToString(),
                    OrganizationId = product.OrganizationId.ToString(),
                }));
        }

        await _productDbContext.SaveChangesAsync();
        
        foreach (var rabbitEvent in events) await _publisherService.SendEvent(_productDbContext, rabbitEvent.Id);
        await _productDbContext.SaveChangesAsync();

        return getProductList;
    }
}