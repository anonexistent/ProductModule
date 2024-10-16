using EfiritPro.Retail.ProductModule.Api.Services;
using EfiritPro.Retail.ProductModule.InputContracts;
using EfiritPro.Retail.ProductModule.OutputContracts;
using Microsoft.AspNetCore.Mvc;

namespace EfiritPro.Retail.ProductModule.Api.Controllers;

[ApiController]
[Route("/product/productGroup/")]
public class ProductGroupController : ControllerBase
{
    private readonly ProductGroupService _productGroupService;

    public ProductGroupController(ProductGroupService productGroupService)
    {
        _productGroupService = productGroupService;
    }

    [HttpPost("createGroup")]
    public async Task<IActionResult> CreateGroup([FromQuery] OwnerAndOrganizationIdQuery query,
        [FromBody] CreateProductGroupBody body)
    {
        var productGroup = await _productGroupService.Create(query.OwnerId, query.OrganizationId,
            body.Name, body.ParentGroupId);
        if (!productGroup.Ok || productGroup.Answer is null) return BadRequest(productGroup.Errors);
        return Ok(new OutputProductGroup(productGroup.Answer, false));
    }

    [HttpGet("getGroup")]
    public async Task<IActionResult> GetGroup([FromQuery] ProductGroupIdQuery query)
    {
        var productGroup = await _productGroupService.Get(query.ProductGroupId, query.OwnerId, query.OrganizationId);
        if (!productGroup.Ok || productGroup.Answer is null) return BadRequest(productGroup.Errors);
        return Ok(new OutputProductGroup(productGroup.Answer, query.WithProducts));
    }

    [HttpGet("getGroupList")]
    public async Task<IActionResult> GetGroupList([FromQuery] ProductGroupListQuery query)
    {
        var productGroups = await _productGroupService.GetList(query.OwnerId, query.OrganizationId, query.JustRoots);
        if (!productGroups.Ok || productGroups.Answer is null) return BadRequest(productGroups.Errors);
        return Ok(new OutputProductGroupList(productGroups.Answer, query.WithProducts));
    }

    [HttpPatch("updateGroup")]
    public async Task<IActionResult> UpdateGroup([FromQuery] ProductGroupIdQuery query,
        [FromBody] CreateProductGroupBody body)
    {
        var productGroup = await _productGroupService.Update(query.ProductGroupId, query.OwnerId,
            query.OrganizationId, body.Name, body.ParentGroupId);
        if (!productGroup.Ok || productGroup.Answer is null) return BadRequest(productGroup.Errors);
        return Ok(new OutputProductGroup(productGroup.Answer, query.WithProducts));
    }

    [HttpDelete("removeGroup")]
    public async Task<IActionResult> RemoveGroup([FromQuery] ProductGroupIdQuery query)
    {
        var group = await _productGroupService.Remove(query.ProductGroupId, query.OwnerId, query.OrganizationId);
        if (!group.Ok || group.Answer is null) return BadRequest(group);
        return Ok(new OutputProductGroup(group.Answer, query.WithProducts));
    }
}