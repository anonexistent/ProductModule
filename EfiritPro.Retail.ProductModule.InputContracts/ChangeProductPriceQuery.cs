using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace EfiritPro.Retail.ProductModule.InputContracts;

public record ChangeProductPriceQuery : ProductIdQuery
{
    [FromQuery(Name = "createdByPostingId")]
    public string? CreatedByPostingId { get; set; } = null;
};