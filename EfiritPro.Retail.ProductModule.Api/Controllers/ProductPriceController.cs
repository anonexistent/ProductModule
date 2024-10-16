using EfiritPro.Retail.ProductModule.Api.Services;
using EfiritPro.Retail.ProductModule.InputContracts;
using EfiritPro.Retail.ProductModule.Models;
using EfiritPro.Retail.ProductModule.OutputContracts;
using Microsoft.AspNetCore.Mvc;

namespace EfiritPro.Retail.ProductModule.Api.Controllers;

[ApiController]
[Route("/product/productPrice/")]
public class ProductPriceController: ControllerBase
{
    private readonly ProductPriceService _productPriceService;

    public ProductPriceController(ProductPriceService productPriceService)
    {
        _productPriceService = productPriceService;
    }

    [HttpPost("createProductPrice")]
    public async Task<IActionResult> CreateProductPrice([FromQuery] ChangeProductPriceQuery query,
        [FromBody] CreateProductPriceBody body)
    {
        var productPrice = await _productPriceService.Create(query.OwnerId, query.OrganizationId, query.ProductId, query.CreatedByPostingId,
            body.PurchasePrice, body.SellingPrice, body.PromoPrice, body.StartTime);
        if (!productPrice.Ok || productPrice.Answer is null) return BadRequest(productPrice.Errors);
        return Ok(new OutputProductPriceItem(productPrice.Answer));
    }
    
    
    [HttpGet("getProductPriceById")]
    public async Task<IActionResult> GetProductPriceById([FromQuery] ProductPriceIdQuery query)
    {
        var productPrice = await _productPriceService.Get(query.ProductPriceId, query.OwnerId, query.OrganizationId, query.ProductId);
        if (!productPrice.Ok || productPrice.Answer is null) return BadRequest(productPrice.Errors);
        return Ok(new OutputProductPriceItem(productPrice.Answer));
    }

    [HttpGet("getProductPriceList")]
    public async Task<IActionResult> GetProductPriceList([FromQuery] ProductIdQuery query)
    {
        var productPriceList = await _productPriceService.GetList(query.OwnerId, query.OrganizationId, query.ProductId);
        if (!productPriceList.Ok || productPriceList.Answer is null) return BadRequest(productPriceList.Errors);
        return Ok(new OutputProductPriceList(productPriceList.Answer ?? Array.Empty<ProductPrice>()));
    }

    [HttpPatch("updateProductPrice")]
    public async Task<IActionResult> UpdateProductPrice([FromQuery] ProductPriceIdQuery query, [FromBody] CreateProductPriceBody body)
    {
        var productPrice = await _productPriceService.Update(query.ProductPriceId,  query.OwnerId, query.OrganizationId, query.ProductId, 
            body.PurchasePrice, body.SellingPrice, body.PromoPrice, body.StartTime);
        if (!productPrice.Ok || productPrice.Answer is null) return BadRequest(productPrice.Errors);
        return Ok(new OutputProductPriceItem(productPrice.Answer));
    }

    [HttpDelete("removeProductPrice")]
    public async Task<IActionResult> RemoveProductPrice([FromQuery] ProductPriceIdQuery query)
    {
        var productPrice = await _productPriceService.Remove(query.ProductPriceId, query.OwnerId, query.OrganizationId, query.ProductId);
        if (!productPrice.Ok || productPrice.Answer is null) return BadRequest(productPrice.Errors);
        return Ok();
    }
}