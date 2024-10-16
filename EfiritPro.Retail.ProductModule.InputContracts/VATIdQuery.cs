using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace EfiritPro.Retail.ProductModule.InputContracts;

public record VATIdQuery : OwnerIdQuery
{
    [FromQuery(Name = "vatId")]
    [Required]
    public string VATId { get; set; }
}