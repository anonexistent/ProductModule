using EfiritPro.Retail.Packages.Errors.Models;
using EfiritPro.Retail.ProductModule.Api.Services;
using EfiritPro.Retail.ProductModule.InputContracts;
using EfiritPro.Retail.ProductModule.Models;
using EfiritPro.Retail.ProductModule.OutputContracts;
using Microsoft.AspNetCore.Mvc;

namespace EfiritPro.Retail.ProductModule.Api.Controllers;

[ApiController]
[Route("/product/product/")]
public class ProductController : ControllerBase
{
    private readonly ProductService _productService;
    private readonly ProductPriceService _productPriceService;

    public ProductController(ProductService productService, ProductPriceService productPriceService)
    {
        _productService = productService;
        _productPriceService = productPriceService;
    }

    [HttpPost("createProductSome")]
    public async Task<IActionResult> CreateProductSome([FromQuery] OwnerAndOrganizationIdQuery query,
        [FromBody] List<CreateProductBody> bodyList)
    {
        var products = new List<ServiceAnswer<Product>>();

        var product2 = await _productService.CreateProductSome(query.OwnerId, query.OrganizationId, bodyList);

        //Parallel.ForEach(bodyList, async body=>
        //    {
        //
        //    }
        //);

        //foreach (var body in bodyList)
        //{
        //    products.Add(await _productService.CreateProduct(query.OwnerId, query.OrganizationId,
        //        body.Name, body.VendorCode, body.BarCode, body.Description, body.Excise, body.ProductGroupId,
        //        body.VATId, body.ProductTypeId, body.UnitId, body.MarkingTypeId,
        //        body.PriceMayChangeInCashReceipt, body.PriceMinInCashReceipt, body.PriceMaxInCashReceipt,
        //        body.IsAllowedToMove, body.CompositionList)
        //    );
        //}
        //if (products.Count <= 0) return BadRequest(products.Select(x=>x.Errors).ToList());

        //return Ok(new OutputProductList(products.Select(x=>x.Answer).Where(x=>x is not null).ToList()));

        if (!product2.Ok || product2.Answer is null) return BadRequest(product2.Errors);

        return Ok(new OutputProductListWithErrorList(product2.Answer, product2.Errors));
    }

    [HttpPost("createProduct")]
    public async Task<IActionResult> CreateProduct([FromQuery] OwnerAndOrganizationIdQuery query,
        [FromBody] CreateProductBody body)
    {
        var product = await _productService.CreateProduct(query.OwnerId, query.OrganizationId,
            body.Name, body.VendorCode, body.BarCode, body.Description, body.Excise, body.ProductGroupId,
            body.VATId, body.ProductTypeId, body.UnitId, body.MarkingTypeId,
            body.PriceMayChangeInCashReceipt, body.PriceMinInCashReceipt, body.PriceMaxInCashReceipt,
            body.IsAllowedToMove, body.CompositionList);
        if (!product.Ok || product.Answer is null) return BadRequest(product.Errors);

        var productPrice = body.Price is not null
            ? await _productPriceService.Create(product.Answer.OwnerId, product.Answer.OrganizationId, product.Answer.Id, null,
                body.Price.PurchasePrice, body.Price.SellingPrice, body.Price.PromoPrice, body.Price.StartTime)
            : null;
        if (productPrice is { Ok: true, Answer: not null }) product.Answer.ProductPrices.Add(productPrice.Answer);   
        
        return Ok(new OutputProductItem(product.Answer));
    }

    [HttpGet("getProductById")]
    public async Task<IActionResult> GetProductById([FromQuery] ProductIdQuery query)
    {
        var product = await _productService.GetProductById(query.ProductId, query.OwnerId, query.OrganizationId);
        if (!product.Ok || product.Answer is null) return BadRequest(product.Errors);
        return Ok(new OutputProductItem(product.Answer));
    }

    [HttpGet("getProductTotalList")]
    public async Task<IActionResult> GetProductTotalList([FromQuery] OwnerAndOrganizationIdQuery query)
    {
        var products = await _productService.GetProductList(query.OwnerId, query.OrganizationId, true);
        if (!products.Ok || products.Answer is null) return BadRequest(products.Errors);
        return Ok(new OutputProductList(products.Answer));
    }

    [HttpGet("getProductTotalListCount")]
    public async Task<IActionResult> GetProductTotalListCount([FromQuery] OwnerAndOrganizationIdQuery query)
    {
        var products = await _productService.GetProductListCount(query.OwnerId, query.OrganizationId, true);
        if (!products.Ok) return BadRequest(products.Errors);
        return Ok(new OutputProductListCount(products.Answer));
    }

    [HttpGet("getProductList")]
    public async Task<IActionResult> GetProductList([FromQuery] OwnerAndOrganizationIdQuery query)
    {
        var productList = await _productService.GetProductList(query.OwnerId, query.OrganizationId);
        if (!productList.Ok || productList.Answer is null) return BadRequest(productList.Errors);
        return Ok(new OutputProductList(productList.Answer));
    }

    [HttpGet("getProductListByName")]
    public async Task<IActionResult> GetProductListByName([FromQuery] ProductNameQuery query)
    {
        var productList = await _productService.GetProductListByName(query.ProductName, query.OwnerId, query.OrganizationId);
        if (!productList.Ok || productList.Answer is null) return BadRequest(productList.Errors);
        return Ok(new OutputProductList(productList.Answer));
    }

    [HttpPatch("updateProduct")]
    public async Task<IActionResult> UpdateProduct([FromQuery] ProductIdQuery query, [FromBody] CreateProductBody body)
    {
        var product = await _productService.UpdateProduct(query.ProductId, query.OwnerId, query.OrganizationId, 
            body.Name, body.VendorCode, body.BarCode, body.Description, body.Excise, body.Hidden, body.ProductGroupId,
            body.VATId, body.ProductTypeId, body.UnitId, body.MarkingTypeId,
            body.PriceMayChangeInCashReceipt, body.PriceMinInCashReceipt, body.PriceMaxInCashReceipt,
            body.CompositionList);
        if (!product.Ok || product.Answer is null) return BadRequest(product.Errors);
        return Ok(new OutputProductItem(product.Answer));
    }

    [HttpPatch("relocateProductSome")]
    public async Task<IActionResult> RelocateProductSome([FromQuery] TargetGroupQuery q, [FromBody] ICollection<string> productIds)
    {
        var products = await _productService.RelocateProductSome(q.NewGroupId, q.OwnerId, q.OrganizationId, productIds);
        if (!products.Ok) return BadRequest(products.Errors);

        return Ok(new OutputProductListWithErrorList(products.Answer, products.Errors));
    }

    [HttpDelete("removeProduct")]
    public async Task<IActionResult> RemoveProduct([FromQuery] ProductIdQuery query)
    {
        var product = await _productService.HideProduct(query.ProductId, query.OwnerId, query.OrganizationId, true);
        if (!product.Ok || product.Answer is null) return BadRequest(product.Errors);
        return Ok();
    }

    [HttpDelete("removeProductSome")]
    public async Task<IActionResult> RemoveProductSome([FromQuery] OwnerAndOrganizationIdQuery q, [FromBody] List<string> productIds)
    {
        //var product = await _productService.HideProduct(query.ProductId, query.OwnerId, query.OrganizationId, true);
        var products = await _productService.HideProductSome(q.OwnerId, q.OrganizationId, productIds, true);

        if (!products.Ok) return BadRequest(products.Errors);
        return Ok(new OutputProductListWithErrorList(products.Answer, products.Errors));
    }
}