using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace EfiritPro.Retail.ProductModule.InputContracts;

public record OwnerIdQuery
{
    [FromQuery(Name = "ownerId")]
    [Required]
    public string OwnerId { get; set; }
}