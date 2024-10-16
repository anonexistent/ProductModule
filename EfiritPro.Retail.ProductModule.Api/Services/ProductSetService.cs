using EfiritPro.Retail.Packages.Errors.Models;
using EfiritPro.Retail.Packages.Rabbit.Events;
using EfiritPro.Retail.Packages.Rabbit.Interfaces;
using EfiritPro.Retail.Packages.Rabbit.Models;
using EfiritPro.Retail.ProductModule.Models;
using EfiritPro.Retail.ProductModule.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace EfiritPro.Retail.ProductModule.Api.Services
{
    /// <summary>
    ///     Отвечает исключительно за работу с связками продукт(набор) - продукт(составляющее).
    /// 
    /// Вся дейтельность по выводу, вводу, редактированию, телепортации - вопросы к сервису продутка. ☺
    /// Здесь мы, словно, купидоны, создаем парочки для связи многие-ко-многим.
    /// 
    /// Пример:
    /// Имеется банановый смузи. Для того, чтобы реализовать его существование на торговой точке, необходимо туда-сюда сделать несколько записей о том,
    /// кто входит (included) и куда входит (product). В том числе, если речь заходит о вложенности наборов, - за это отвечает продуктовый сервис
    /// по причине:
    ///         "Набор = продукт*".
    ///  
    /// * - в отличии от простого продукта, набор отмечается в поле productType как таковой. Также, набор умеет входит в состав других наборов,
    /// что делает логику этой сущности немного отличающейся от рядовых товаров
    /// </summary>
    public class ProductSetService
    {
        private readonly ProductDbContext _productDbContext;
        private readonly IRabbitPublisherService<ProductDbContext> _publisherService;

        private readonly ProductService _productService;

        private readonly ICollection<string> _removeProductSetEventQueueList = new[]
        {
        "prodSetMove/removeProductSet",
        };

        public ProductSetService(ProductDbContext productDbContext, IRabbitPublisherService<ProductDbContext> publisherService, 
            ProductService productService)
        {
            _productDbContext = productDbContext;
            _publisherService = publisherService;
            _productService = productService;
        }

        public async Task<ServiceAnswer<ProductSet>> CreateProductSet(string ownerId, string organizationId, string rootProductId, string includedProductId)
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

            if (rootProductId is not null && !Guid.TryParse(rootProductId, out var productGuid))
                errors.Add(new ServiceFieldError()
                {
                    Fields = new[] { "productId" },
                    Message = "productId не соответствует формату."
                });

            if (!Guid.TryParse(includedProductId, out var includedGuid))
            {
                errors.Add(new ServiceFieldError()
                {
                    Fields = new[] { "includedProductId" },
                    Message = "includedProductId не соответствует формату."
                });
            }

            if (errors.Count > 0)
                return new ServiceAnswer<ProductSet>()
                {
                    Ok = false,
                    Errors = errors,
                };

            return await CreateProductSet(ownerGuid, organizationGuid, rootProductId is not null ? Guid.Parse(rootProductId) : null, includedGuid);
        }

        public async Task<ServiceAnswer<ProductSet>> CreateProductSet(Guid ownerId, Guid organizationId, Guid? rootProductId, Guid? includedProductId)
        {
            var errors = new List<object>();
            Product? rootProduct = null;
            Product? includedProduct = null;

            if (rootProductId == includedProductId)
            {
                return new ServiceAnswer<ProductSet>()
                {
                    Ok = false,
                    Errors = new[]
                    {
                        new ServiceFieldError()
                        {
                            Fields = new[] { "rootProductId, includedProductId" },
                            Message = "Набор не должен включать себя в себя."
                        }
                    }
                };
            }

            if (rootProductId is not null)
            {
                var getProduct = await _productService.GetProductById(rootProductId.Value, ownerId, organizationId);
                if (!getProduct.Ok || getProduct.Answer is null)
                {
                    errors.AddRange(getProduct.Errors);
                }
                else
                {
                    rootProduct = getProduct.Answer;
                }
            }

            if (includedProductId is not null)
            {
                var getInProduct = await _productService.GetProductById(includedProductId.Value, ownerId, organizationId);
                if (!getInProduct.Ok || getInProduct.Answer is null)
                {
                    errors.AddRange(getInProduct.Errors);
                }
                else
                {
                    includedProduct = getInProduct.Answer;
                }
            }

            if(includedProduct is null || rootProduct is null)
            {
                return new ServiceAnswer<ProductSet>()
                {
                    Ok = false,
                    Errors = new[]
    {
                        new ServiceFieldError()
                        {
                            Fields = new[] { "rootProductId, includedProductId" },
                            Message = "Продукт не найден"
                        }
                    }
                };
            }

            var productSet = new ProductSet()
            {
                Id = Guid.Empty,
                OwnerId = ownerId,
                OrganizationId = organizationId,

                ProductId=rootProduct.Id,
                Product = rootProduct,
                IncludedProductId = includedProduct.Id,
                IncludedProduct = includedProduct
            };

            //productSet.IncludedProduct.ParentProduct = rootProduct;
            //rootProduct.includedProducts.Add(productSet);

            if (productSet is null || errors.Count > 0)
            {
                return new ServiceAnswer<ProductSet>()
                {
                    Ok = false,
                    Errors = errors
                };
            }

            await _productDbContext.ProductSets.AddAsync(productSet);
            await _productDbContext.SaveChangesAsync();

            return new ServiceAnswer<ProductSet>()
            {
                Ok = true,
                Answer = productSet
            };
        }

        public async Task<ServiceAnswer<ProductSet>> GetProductSetById(string setId, string ownerId, string organizationId)
        {
            var errors = new List<object>();

            if (!Guid.TryParse(setId, out var setGuid))
                errors.Add(new ServiceFieldError()
                {
                    Fields = new[] { "productSetId" },
                    Message = "productSetId не соответствует формату."
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
            {
                return new ServiceAnswer<ProductSet>()
                {
                    Ok = false,
                    Errors = errors,
                };
            }


            return await GetProductSetById(setGuid, ownerGuid, organizationGuid);

        }

        public async Task<ServiceAnswer<ProductSet>> GetProductSetById(Guid setId, Guid ownerId, Guid organizationId)
        {
            var productSet = await _productDbContext.ProductSets.Include(x=>x.Product).Include(x=>x.IncludedProduct).FirstOrDefaultAsync(x => x.Id == setId &&
            x.OwnerId == ownerId && x.OrganizationId == organizationId);

            if (productSet is null)
            {
                return new ServiceAnswer<ProductSet>()
                {
                    Ok = false,
                    Errors = new[]
                    {
                        new ServiceFieldError()
                        {
                            Fields = new[] { "productSetId", "ownerId", "organizationId" },
                            Message = "Набор не найден."
                        }
                    }
                };
            }
            return new ServiceAnswer<ProductSet>() { Ok = true, Answer = productSet };
        }

        public async Task<ServiceAnswer<ProductSet>> GetProductSetByProduct(string rootProductId, string ownerId, string organizationId)
        {
            var errors = new List<object>();

            if (!Guid.TryParse(rootProductId, out var productGuid))
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
                return new ServiceAnswer<ProductSet>()
                {
                    Ok = false,
                    Errors = errors,
                };

            return await GetProductSetByProduct(productGuid, ownerGuid, organizationGuid);
        }

        //  
        public async Task<ServiceAnswer<ProductSet>> GetProductSetByProduct(Guid rootProductId, Guid ownerId, Guid organizationId)
        {
            var productSet = await _productDbContext.ProductSets
                .Include(x=>x.Product)
                .Include(x=>x.IncludedProduct)
                .FirstOrDefaultAsync(x => x.ProductId == rootProductId &&
            x.OwnerId == ownerId && x.OrganizationId == organizationId);

            var rawProductSet = _productDbContext.ProductSets.Entry(productSet);
            var a = rawProductSet;

            if (productSet is null)
            {
                return new ServiceAnswer<ProductSet>()
                {
                    Ok = false,
                    Errors = new[]
                    {
                        new ServiceFieldError()
                        {
                            Fields = new[] { "rootProductId", "ownerId", "organizationId" },
                            Message = "Набор не найден."
                        }
                    }
                };
            }

            ////Console.WriteLine(a.OriginalValues);
            ////Console.WriteLine(a.CurrentValues);
            ////Console.WriteLine(a.Metadata);
            ////Console.WriteLine(a.Context);
            ////Console.WriteLine(a.Collections);
            //Console.WriteLine(a.DebugView.LongView);
            ////Console.WriteLine(a.Entity);
            ////Console.WriteLine(a.IsKeySet);
            ////Console.WriteLine(a.Members);
            ////Console.WriteLine(a.Navigations);
            ////Console.WriteLine(a.Properties);
            ////Console.WriteLine(a.References);
            //Console.WriteLine(a.State);
            //Console.WriteLine(a.ToString());

            ////  кол-во эл-в в составе продукта
            //Console.WriteLine("ing: "+productSet.Product.includingProducts.Count);
            ////  здесь должно быть кол-во эл, которые ВКЛЮЧАЮТ В СВОЙ СОСТАВ ДАННЫЙ ПРОДУКТИК (ну, уже не включают)
            //Console.WriteLine("ed: "+productSet.Product.includedProducts.Count);

            return productSet is not null 
                ?
                new ServiceAnswer<ProductSet>()
                {
                    Ok = true,
                    Answer = productSet
                }
                :
                new ServiceAnswer<ProductSet>()
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
        }

        public async Task<ServiceAnswer<ProductSet>> RemoveProductSet(string setId, string orgId, string ownerId)
        {
            var errors = new List<object>();

            if (!Guid.TryParse(setId, out var setGuid))
                errors.Add(new ServiceFieldError()
                {
                    Fields = new[] { "ProductSetId" },
                    Message = "productSetId не соответствует формату."
                });

            if (!Guid.TryParse(ownerId, out var ownerGuid))
                errors.Add(new ServiceFieldError()
                {
                    Fields = new[] { "ownerId" },
                    Message = "ownerId не соответствует формату."
                });

            if (!Guid.TryParse(orgId, out var orgGuid))
                errors.Add(new ServiceFieldError()
                {
                    Fields = new[] { "organizationId" },
                    Message = "organizationId не соответствует формату."
                });

            if (errors.Count > 0)
            {
                return new ServiceAnswer<ProductSet>()
                {
                    Ok = false,
                    Errors = errors,
                };
            }

            return await RemoveProductSet(setGuid, orgGuid, ownerGuid);
        }

        public async Task<ServiceAnswer<ProductSet>> RemoveProductSet(Guid setId, Guid orgId, Guid ownerId)
        {
            //var currentSet = await _productDbContext.ProductSets.FirstOrDefaultAsync(x=>x.Id == setId & x.OrganizationId == orgId & x.OwnerId == ownerId);
            var currentSet = await GetProductSetById(setId, ownerId, orgId);
            //if (!getFavoriteProduct.Ok || getFavoriteProduct.Answer is null) return getFavoriteProduct;

            if (!currentSet.Ok || currentSet.Answer is null) return currentSet;

            _productDbContext.ProductSets.Remove(currentSet.Answer);

            //await _productDbContext.SaveChangesAsync();

            var a = _productDbContext.ProductSets.Remove(currentSet.Answer);

            _productDbContext.SaveChanges();

            return currentSet;
        }
    }
}
