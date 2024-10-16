using EfiritPro.Retail.ProductModule.Api.Services;
using EfiritPro.Retail.ProductModule.InputContracts;
using EfiritPro.Retail.ProductModule.OutputContracts;
using Microsoft.AspNetCore.Mvc;

namespace EfiritPro.Retail.ProductModule.Api.Controllers
{
    [ApiController]
    [Route("/product/productSet/")]
    public class ProductSetController : ControllerBase
    {
        private readonly ProductSetService _productSetService;
        private readonly ProductService _productService;
        //private readonly ProductPriceService _productPriceService;

        public ProductSetController(ProductSetService productSetService, ProductService productService, ProductPriceService productPriceService)
        {
            _productSetService = productSetService;
            _productService = productService;
            //_productPriceService = productPriceService;
        }

        [HttpPost("createProductSet")]
        public async Task<IActionResult> CreateProductSet([FromQuery] OwnerAndOrganizationIdQuery query,
            [FromBody] CreateProductSetBody body)
        {
            var set = await _productSetService.CreateProductSet(query.OwnerId, query.OrganizationId, body.productId, body.includedProductId);

            if (!set.Ok || set.Answer is null) return BadRequest(set.Errors);

            return Ok(new OutputProductSetItem(set.Answer));
        }

        [HttpGet("getProductSetById")]
        public async Task<IActionResult> GetProductSetById([FromQuery] ProductSetIdQuery query)
        {
            var productSet = await _productSetService.GetProductSetById(query.ProductSetId, query.OwnerId, query.OrganizationId);
            if (!productSet.Ok || productSet.Answer is null) return BadRequest(productSet.Errors);
            return Ok(new OutputProductSetItem(productSet.Answer));
        }

        [HttpGet("getProductSetByProduct")]
        public async Task<IActionResult> GetProductSetByProduct([FromQuery] ProductIdQuery query)
        {
            var product = await _productSetService.GetProductSetByProduct(query.ProductId, query.OwnerId, query.OrganizationId);
            if (!product.Ok || product.Answer is null) return BadRequest(product.Errors);
            return Ok(new OutputProductSet(product.Answer.Product));
        }

        [HttpDelete("removeProductSet")]
        public async Task<IActionResult> RemoveProductSet([FromQuery] ProductSetIdQuery q)
        {
            var productSet = await _productSetService.RemoveProductSet(q.ProductSetId, q.OrganizationId, q.OwnerId);
            if (!productSet.Ok || productSet.Answer is null) return BadRequest(productSet.Errors);
            return Ok($"Запись {productSet.Answer.Id} успешно удалена. ☺");
        }

        ////[HttpGet("getProductsCount")]
        ////public async Task<IActionResult> GetProductsCount([FromQuery] OwnerAndOrganizationIdQuery query)
        ////{
        ////    var products = await _productService.GetProductsCount(query.OwnerId, query.OrganizationId);
        ////    if (!products.Ok || products.Answer == -1) return BadRequest(products.Errors);
        ////    return Ok(new(products.Answer));
        ////}

        //[HttpGet("getProductSetList")]
        //public async Task<IActionResult> GetProductList([FromQuery] OwnerAndOrganizationIdQuery query)
        //{
        //    var productList = await _productService.GetProductList(query.OwnerId, query.OrganizationId);
        //    if (!productList.Ok || productList.Answer is null) return BadRequest(productList.Errors);
        //    return Ok(new OutputProductList(productList.Answer));
        //}

        //[HttpPatch("updateProductSet")]
        //public async Task<IActionResult> UpdateProduct([FromQuery] ProductIdQuery query, [FromBody] CreateProductBody body)
        //{
        //    var product = await _productService.UpdateProduct(query.ProductId, query.OwnerId, query.OrganizationId,
        //        body.Name, body.VendorCode, body.BarCode, body.Description, body.Excise, body.Hidden, body.ProductGroupId,
        //        body.VATId, body.ProductTypeId, body.UnitId, body.MarkingTypeId,
        //        body.PriceMayChangeInCashReceipt, body.PriceMinInCashReceipt, body.PriceMaxInCashReceipt,
        //        body.CompositionList);
        //    if (!product.Ok || product.Answer is null) return BadRequest(product.Errors);
        //    return Ok(new OutputProductItem(product.Answer));
        //}

        //[HttpDelete("removeProductSet")]
        //public async Task<IActionResult> RemoveProduct([FromQuery] ProductIdQuery query)
        //{
        //    var product = await _productService.HideProduct(query.ProductId, query.OwnerId, query.OrganizationId, true);
        //    if (!product.Ok || product.Answer is null) return BadRequest(product.Errors);
        //    return Ok();
        //}
    }
}
