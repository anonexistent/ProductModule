using EfiritPro.Retail.ProductModule.Api.Services;
using EfiritPro.Retail.ProductModule.InputContracts;
using EfiritPro.Retail.ProductModule.OutputContracts;
using Microsoft.AspNetCore.Mvc;

namespace EfiritPro.Retail.ProductModule.Api.Controllers;

[ApiController]
[Route("/product/unit/")]
public class UnitController : ControllerBase
{
    private readonly UnitService _unitService;

    public UnitController(UnitService unitService)
    {
        _unitService = unitService;
    }

    [HttpGet("getUnitById")]
    public async Task<IActionResult> GetUnitById([FromQuery] UnitIdQuery query)
    {
        var unit = await _unitService.GetUnitById(query.UnitId);
        if (!unit.Ok || unit.Answer is null) return BadRequest(unit.Errors);
        return Ok(new OutputUnitItem(unit.Answer));
    }
    
    [HttpGet("getUnitList")]
    public async Task<IActionResult> GetUnitList([FromQuery] OwnerIdQuery query)
    {
        var unitList = await _unitService.GetUnitList();
        if (!unitList.Ok || unitList.Answer is null) return BadRequest(unitList.Errors);
        return Ok(new OutputUnitList(unitList.Answer));
    }
}