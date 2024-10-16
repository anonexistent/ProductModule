using System.Text.Json;
using EfiritPro.Retail.Packages.Rabbit.Events;
using EfiritPro.Retail.Packages.Rabbit.Interfaces;
using EfiritPro.Retail.ProductModule.Persistence;

namespace EfiritPro.Retail.ProductModule.Api.Services;

public class RabbitEventHandler : IRabbitEventHandler
{
    private readonly ProductService _productService;
    private readonly ProductPriceService _productPriceService;
    private readonly IRabbitPublisherService<ProductDbContext> _publisherService;

    public RabbitEventHandler(ProductService productService, 
        ProductPriceService productPriceService, 
        IRabbitPublisherService<ProductDbContext> publisherService)
    {
        _productService = productService;
        _productPriceService = productPriceService;
        _publisherService = publisherService;
    }

    public async Task HandleEvent(string queue, string eventBody)
    {
        OrganizationEvent? organizationEvent;
        ProductEvent? productEvent;
        
        switch (queue)
        {
            case "product/removeOrganization":
                organizationEvent = JsonSerializer.Deserialize<OrganizationEvent>(eventBody);
                if (organizationEvent is null) throw new NullReferenceException();
                await _productService.RemoveAllOrganizationProducts(organizationEvent.OwnerId,
                    organizationEvent.OrganizationId);
                await _publisherService.SendAck(organizationEvent.EventId, organizationEvent.AckDestination);
                break;
            case "product/removePosting":
                productEvent = JsonSerializer.Deserialize<ProductEvent>(eventBody);
                if (productEvent is null) throw new NullReferenceException();
                await _productPriceService.RemoveByPostingId(productEvent.OwnerId, productEvent.OrganizationId,
                    productEvent.ProductId);
                await _publisherService.SendAck(productEvent.EventId, productEvent.AckDestination);
                break;
        }
    }
}