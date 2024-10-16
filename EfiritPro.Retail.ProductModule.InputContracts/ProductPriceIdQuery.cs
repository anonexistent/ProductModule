using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace EfiritPro.Retail.ProductModule.InputContracts;

public record ProductPriceIdQuery : ProductIdQuery
{
    [FromQuery(Name = "productPriceId")]
    [Required]
    public string ProductPriceId { get; set; }
};