using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace EfiritPro.Retail.ProductModule.InputContracts;

public record ProductTypeIdQuery : OwnerIdQuery
{
    [FromQuery(Name = "productTypeId")]
    [Required]
    public string ProductTypeId { get; set; }
}