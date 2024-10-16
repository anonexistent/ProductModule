using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace EfiritPro.Retail.ProductModule.InputContracts;

public record UnitIdQuery : OwnerIdQuery
{
    [FromQuery(Name = "unitId")]
    [Required]
    public string UnitId { get; set; }
};