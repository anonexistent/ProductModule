using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace EfiritPro.Retail.ProductModule.InputContracts;

public record ProductIdQuery : OwnerAndOrganizationIdQuery
{
    [FromQuery(Name = "productId")]
    [Required]
    public string ProductId { get; set; }
};