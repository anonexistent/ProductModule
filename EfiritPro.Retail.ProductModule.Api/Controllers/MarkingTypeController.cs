using EfiritPro.Retail.ProductModule.Api.Services;
using EfiritPro.Retail.ProductModule.InputContracts;
using EfiritPro.Retail.ProductModule.OutputContracts;
using Microsoft.AspNetCore.Mvc;

namespace EfiritPro.Retail.ProductModule.Api.Controllers;

[ApiController]
[Route("/product/markingType/")]
public class MarkingTypeController : ControllerBase
{
    private readonly MarkingTypeService _markingTypeService;

    public MarkingTypeController(MarkingTypeService markingTypeService)
    {
        _markingTypeService = markingTypeService;
    }

    [HttpGet("getMarkingTypeById")]
    public async Task<IActionResult> GetMarkingTypeById([FromQuery] MarkingTypeIdQuery query)
    {
        var markingType = await _markingTypeService.GetMarkingTypeById(query.MarkingTypeId);
        if (!markingType.Ok || markingType.Answer is null) return BadRequest(markingType.Errors);
        return Ok(new OutputMarkingTypeItem(markingType.Answer));
    }
    
    [HttpGet("getMarkingTypeList")]
    public async Task<IActionResult> GetMarkingTypeList([FromQuery] OwnerIdQuery query)
    {
        var markingTypeList = await _markingTypeService.GetMarkingTypeList();
        if (!markingTypeList.Ok || markingTypeList.Answer is null) return BadRequest(markingTypeList.Errors);

        return Ok(new OutputMarkingTypeList(markingTypeList.Answer));
    }
}