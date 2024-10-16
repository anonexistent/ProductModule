using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace EfiritPro.Retail.ProductModule.InputContracts;
public record ProductNameQuery : OwnerAndOrganizationIdQuery
{
    [FromQuery(Name = "productName")]
    [Required]
    public string ProductName { get; set; }
}
