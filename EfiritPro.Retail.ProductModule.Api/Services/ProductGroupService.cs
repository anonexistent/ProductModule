using EfiritPro.Retail.Packages.Errors.Models;
using EfiritPro.Retail.ProductModule.Models;
using EfiritPro.Retail.ProductModule.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EfiritPro.Retail.ProductModule.Api.Services;

public class ProductGroupService
{
    private readonly ProductDbContext _db;
    private readonly ProductGroupTreeService _productGroupTreeService;

    public ProductGroupService(ProductDbContext db, ProductGroupTreeService productGroupTreeService)
    {
        _db = db;
        _productGroupTreeService = productGroupTreeService;
    }

    public async Task<ServiceAnswer<ProductGroup>> Create(string ownerId, string organizationId, string name, string? parentGroupId)
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

        if (parentGroupId is not null && !Guid.TryParse(parentGroupId, out var parentGroupGuid))
            errors.Add(new ServiceFieldError()
            {
                Fields = new[] { "parentGroupId" },
                Message = "parentGroupId не соответствует формату."
            });

        if (errors.Count > 0)
            return new ServiceAnswer<ProductGroup>()
            {
                Ok = false,
                Errors = errors,
            };

        return await Create(ownerGuid, organizationGuid, name,
            parentGroupId is not null ? Guid.Parse(parentGroupId) : null);
    }

    public async Task<ServiceAnswer<ProductGroup>> Create(Guid ownerId, Guid organizationId, string name, Guid? parentGroupId)
    {
        var group = new ProductGroup()
        {
            OwnerId = ownerId,
            OrganizationId = organizationId,
            Name = name,
            ParentGroupId = null,
            ParentGroup = null,
        };

        if (parentGroupId is not null)
        {
            var parentGroup = (await Get(parentGroupId.Value, ownerId, organizationId)).Answer;

            if (parentGroup is null)
            {
                return new ServiceAnswer<ProductGroup>()
                {
                    Ok = false,
                    Errors = new[]
                    {
                        new ServiceFieldError()
                        {
                            Fields = new[] {"parentGroupId"},
                            Message = "Родительская группа продуктов не найдена."
                        }
                    }
                };
            }

            if (!(await _productGroupTreeService.CheckGroupLimitations(group.Id, parentGroup.Id,
                    ownerId, organizationId)))
            {
                return new ServiceAnswer<ProductGroup>()
                {
                    Ok = false,
                    Errors = new[]
                    {
                        new ServiceFieldError()
                        {
                            Fields = new[] {"parentGroupId"},
                            Message = "Превышены лимиты по глубине групп или появилась циклическая зависимость."
                        }
                    }
                };
            }

            group.ParentGroup = parentGroup;
            group.ParentGroupId = parentGroup.Id;
        }

        await _db.ProductGroups.AddAsync(group);
        await _db.SaveChangesAsync();

        return new ServiceAnswer<ProductGroup>()
        {
            Ok = true,
            Answer = group,
        };
    }

    public async Task<ServiceAnswer<ProductGroup>> Get(string id, string ownerId, string organizationId)
    {
        var errors = new List<object>();

        if (!Guid.TryParse(id, out var guid))
            errors.Add(new ServiceFieldError()
            {
                Fields = new[] { "productGroupId" },
                Message = "productGroupId не соответствует формату."
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
            return new ServiceAnswer<ProductGroup>()
            {
                Ok = false,
                Errors = errors,
            };

        return await Get(guid, ownerGuid, organizationGuid);
    }

    public async Task<ServiceAnswer<ProductGroup>> Get(Guid id, Guid ownerId, Guid organizationId)
    {
        await CheckOldItemsAsync(_db);

        var group = await _db.ProductGroups
            .Include(pg => pg.Products)
            .Include(pg => pg.ChildGroups)
            .FirstOrDefaultAsync(pg => pg.Id == id &&
                                       pg.OwnerId == ownerId &&
                                       pg.OrganizationId == organizationId);

        return group is not null
            ? new ServiceAnswer<ProductGroup>()
            {
                Ok = true,
                Answer = group
            }
            : new ServiceAnswer<ProductGroup>()
            {
                Ok = false,
                Errors = new[]
                {
                    new ServiceFieldError()
                    {
                        Fields = new[] { "productGroupId", "ownerId", "organizationId" },
                        Message = "Группа продуктов не найдена."
                    }
                }
            };
    }

    public async Task CheckOldItemsAsync(ProductDbContext c)
    {
        var products = await c.Products
            .Where(p => p.Hidden)
            .ToListAsync();

        foreach (var product in products)
        {
            product.ProductGroup = null;
            product.ProductGroupId = null;
        }

        await c.SaveChangesAsync();
    }

    public async Task<ServiceAnswer<ICollection<ProductGroup>>> GetList(string ownerId, string organizationId, bool justRoots)
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
            return new ServiceAnswer<ICollection<ProductGroup>>()
            {
                Ok = false,
                Errors = errors,
            };

        return await GetList(ownerGuid, organizationGuid, justRoots);
    }

    public async Task<ServiceAnswer<ICollection<ProductGroup>>> GetList(Guid ownerId, Guid organizationId, bool justRoots)
    {
        return new ServiceAnswer<ICollection<ProductGroup>>()
        {
            Ok = true,
            Answer = justRoots
                ? await _db.ProductGroups
                    .Where(pg => pg.OwnerId == ownerId &&
                                 pg.OrganizationId == organizationId &&
                                 pg.ParentGroupId == null)
                    .ToArrayAsync()
                : await _db.ProductGroups
                    .Where(pg => pg.OwnerId == ownerId &&
                                 pg.OrganizationId == organizationId)
                    .ToArrayAsync(),
        };
    }

    public async Task<ServiceAnswer<ProductGroup>> Update(string id, string ownerId, string organizationId, string name,
        string? parentGroupId)
    {
        var errors = new List<object>();

        if (!Guid.TryParse(id, out var guid))
            errors.Add(new ServiceFieldError()
            {
                Fields = new[] { "productGroupId" },
                Message = "productGroupId не соответствует формату."
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

        if (parentGroupId is not null && !Guid.TryParse(parentGroupId, out var parentGroupGuid))
            errors.Add(new ServiceFieldError()
            {
                Fields = new[] { "parentGroupId" },
                Message = "parentGroupId не соответствует формату."
            });

        if (errors.Count > 0)
            return new ServiceAnswer<ProductGroup>()
            {
                Ok = false,
                Errors = errors,
            };

        return await Update(guid, ownerGuid, organizationGuid, name,
            parentGroupId is not null ? Guid.Parse(parentGroupId) : null);
    }

    public async Task<ServiceAnswer<ProductGroup>> Update(Guid id, Guid ownerId, Guid organizationId, string name,
        Guid? parentGroupId)
    {
        var getGroup = await Get(id, ownerId, organizationId);
        if (!getGroup.Ok || getGroup.Answer is null) return getGroup;

        if (parentGroupId is not null)
        {
            var parentGroup = (await Get(parentGroupId.Value, ownerId, organizationId)).Answer;

            if (parentGroup is null)
            {
                return new ServiceAnswer<ProductGroup>()
                {
                    Ok = false,
                    Errors = new[]
                    {
                        new ServiceFieldError()
                        {
                            Fields = new[] {"parentGroupId"},
                            Message = "Родительская группа продуктов не найдена."
                        }
                    }
                };
            }

            if (!(await _productGroupTreeService.CheckGroupLimitations(getGroup.Answer.Id, parentGroup.Id, ownerId, organizationId)))
            {
                return new ServiceAnswer<ProductGroup>()
                {
                    Ok = false,
                    Errors = new[]
                    {
                        new ServiceFieldError()
                        {
                            Fields = new[] {"parentGroupId"},
                            Message = "Превышены лимиты по глубине групп или появилась циклическая зависимость."
                        }
                    }
                };
            }

            getGroup.Answer.ParentGroup = parentGroup;
            getGroup.Answer.ParentGroupId = parentGroup.Id;
        }
        else
        {
            getGroup.Answer.ParentGroup = null;
            getGroup.Answer.ParentGroupId = null;
        }

        getGroup.Answer.Name = name;

        _db.ProductGroups.Update(getGroup.Answer);
        await _db.SaveChangesAsync();

        return getGroup;
    }

    public async Task<ServiceAnswer<ProductGroup>> Remove(string id, string ownerId, string organizationId)
    {
        var errors = new List<object>();

        if (!Guid.TryParse(id, out var guid))
            errors.Add(new ServiceFieldError()
            {
                Fields = new[] { "productGroupId" },
                Message = "productGroupId не соответствует формату."
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
            return new ServiceAnswer<ProductGroup>()
            {
                Ok = false,
                Errors = errors
            };

        return await Remove(guid, ownerGuid, organizationGuid);
    }

    public async Task<ServiceAnswer<ProductGroup>> Remove(Guid id, Guid ownerId, Guid organizationId)
    {
        var getGroup = await Get(id, ownerId, organizationId);
        if (!getGroup.Ok || getGroup.Answer is null) return getGroup;

        var group = getGroup.Answer;

        var productsChildren = group.Products.Where(x => x.Hidden).ToList();

        if (group.Products.Count != 0)
        {
            //if (productsChildren.Count != 0 & productsChildren.Count==group.Products.Count)
            //{
            //    //productsChildren.Where(x => x.Hidden).ToList().ForEach(x =>
            //    //{
            //    //    x.ProductGroup = null;
            //    //    x.ProductGroupId = null;
            //    //});

            //    return new ServiceAnswer<ProductGroup>()
            //    {
            //        Ok = false,
            //        Errors = new[]
            //        {
            //        new ServiceFieldError()
            //        {
            //            Fields = new[] { "productGroupId", "ownerId", "organizationId" },
            //            Message = "Нельзя удалить группу, если к ней относятся скрытые товары."
            //        }
            //    }
            //    };
            //}
            return new ServiceAnswer<ProductGroup>()
            {
                Ok = false,
                Errors = new[]
                {
                    new ServiceFieldError()
                    {
                        Fields = new[] { "productGroupId", "ownerId", "organizationId" },
                        Message = "Нельзя удалить группу, если в ней есть товары."
                    }
                }
            };
        }


        if (group.ChildGroups.Count != 0)
            return new ServiceAnswer<ProductGroup>()
            {
                Ok = false,
                Errors = new[]
                {
                    new ServiceFieldError()
                    {
                        Fields = new[] { "productGroupId", "ownerId", "organizationId" },
                        Message = "Нельзя удалить группу, если в ней есть дочерние группы."
                    }
                }
            };

        _db.ProductGroups.Remove(group);
        await _db.SaveChangesAsync();

        return getGroup;
    }
}