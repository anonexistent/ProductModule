using EfiritPro.Retail.ProductModule.Api.Services;
using EfiritPro.Retail.ProductModule.InputContracts;
using EfiritPro.Retail.ProductModule.OutputContracts;
using Microsoft.AspNetCore.Mvc;

namespace EfiritPro.Retail.ProductModule.Api.Controllers;

[ApiController]
[Route("/product/favoriteProduct/")]
public class FavoriteProductController : ControllerBase
{
    private readonly FavoriteProductService _favoriteProductService;

    public FavoriteProductController(FavoriteProductService favoriteProductService)
    {
        _favoriteProductService = favoriteProductService;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateFavoriteProduct([FromQuery] FavoriteProductQuery query)
    {
        var favoriteProduct = await _favoriteProductService.Create(query.ProductId, query.OwnerId, 
            query.OrganizationId, query.WorkerId);
        if (!favoriteProduct.Ok || favoriteProduct.Answer is null) return BadRequest(favoriteProduct.Errors);
        
        var favoriteProductList = await _favoriteProductService.GetList(favoriteProduct.Answer.OwnerId, 
            favoriteProduct.Answer.OrganizationId, favoriteProduct.Answer.WorkerId);
        if (!favoriteProductList.Ok || favoriteProductList.Answer is null) return BadRequest(favoriteProductList.Errors);
        return Ok(new OutputFavoriteProductList(favoriteProductList.Answer));
    }

    [HttpGet("getList")]
    public async Task<IActionResult> GetFavoriteProductList([FromQuery] FavoriteProductListQuery query)
    {
        var favoriteProductList = await _favoriteProductService.GetList(query.OwnerId, 
            query.OrganizationId, query.WorkerId);
        if (!favoriteProductList.Ok || favoriteProductList.Answer is null) return BadRequest(favoriteProductList.Errors);
        return Ok(new OutputFavoriteProductList(favoriteProductList.Answer));
    }

    [HttpDelete("remove")]
    public async Task<IActionResult> RemoveProduct([FromQuery] FavoriteProductQuery query)
    {
        var favoriteProduct = await _favoriteProductService.Remove(query.ProductId, 
            query.OwnerId, query.OrganizationId, query.WorkerId);
        if (!favoriteProduct.Ok || favoriteProduct.Answer is null) return BadRequest(favoriteProduct.Errors);
        var favoriteProductList = await _favoriteProductService.GetList(favoriteProduct.Answer.OwnerId, 
            favoriteProduct.Answer.OrganizationId, favoriteProduct.Answer.WorkerId);
        if (!favoriteProductList.Ok || favoriteProductList.Answer is null) return BadRequest(favoriteProductList.Errors);
        return Ok(new OutputFavoriteProductList(favoriteProductList.Answer));
    }
}