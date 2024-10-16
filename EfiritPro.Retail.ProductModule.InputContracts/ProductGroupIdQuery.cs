using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace EfiritPro.Retail.ProductModule.InputContracts;

public record ProductGroupIdQuery : OwnerAndOrganizationIdQuery
{
    [FromQuery(Name = "productGroupId")]
    [Required]
    public string ProductGroupId { get; set; }
    [FromQuery(Name = "withProducts")]
    [Required]
    public bool WithProducts { get; set; }
};