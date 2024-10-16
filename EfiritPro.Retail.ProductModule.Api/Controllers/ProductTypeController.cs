using EfiritPro.Retail.ProductModule.Api.Services;
using EfiritPro.Retail.ProductModule.InputContracts;
using EfiritPro.Retail.ProductModule.OutputContracts;
using Microsoft.AspNetCore.Mvc;

namespace EfiritPro.Retail.ProductModule.Api.Controllers;

[ApiController]
[Route("/product/productType/")]
public class ProductTypeController : ControllerBase
{
    private readonly ProductTypeService _productTypeService;

    public ProductTypeController(ProductTypeService productTypeService)
    {
        _productTypeService = productTypeService;
    }

    [HttpGet("getProductTypeById")]
    public async Task<IActionResult> GetProductTypeById([FromQuery] ProductTypeIdQuery query)
    {
        var productType = await _productTypeService.GetProductTypeById(query.ProductTypeId);
        if (!productType.Ok || productType.Answer is null) return BadRequest(productType.Errors);
        return Ok(new OutputProductTypeItem(productType.Answer));
    }
    
    [HttpGet("getProductTypeList")]
    public async Task<IActionResult> GetProductTypeList([FromQuery] OwnerIdQuery query)
    {
        var productTypeList = await _productTypeService.GetProductTypeList();
        if (!productTypeList.Ok || productTypeList.Answer is null) return BadRequest(productTypeList.Errors);
        return Ok(new OutputProductTypeList(productTypeList.Answer));
    }
}