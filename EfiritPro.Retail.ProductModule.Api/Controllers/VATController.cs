using EfiritPro.Retail.ProductModule.Api.Services;
using EfiritPro.Retail.ProductModule.InputContracts;
using EfiritPro.Retail.ProductModule.OutputContracts;
using Microsoft.AspNetCore.Mvc;

namespace EfiritPro.Retail.ProductModule.Api.Controllers;

[ApiController]
[Route("/product/vat/")]
public class VATController : ControllerBase
{
    private readonly VATService _vatService;

    public VATController(VATService vatService)
    {
        _vatService = vatService;
    }

    [HttpGet("getVATById")]
    public async Task<IActionResult> GetVATById([FromQuery] VATIdQuery query)
    {
        var vat = await _vatService.GetVATById(query.VATId);
        if (!vat.Ok || vat.Answer is null) return BadRequest(vat.Errors);
        return Ok(new OutputVATItem(vat.Answer));
    }
    
    [HttpGet("getVATList")]
    public async Task<IActionResult> GetVATList([FromQuery] OwnerIdQuery query)
    {
        var vatList = await _vatService.GetVATList();
        if (!vatList.Ok || vatList.Answer is null) return BadRequest(vatList.Errors);
        return Ok(new OutputVATList(vatList.Answer));
    }
}