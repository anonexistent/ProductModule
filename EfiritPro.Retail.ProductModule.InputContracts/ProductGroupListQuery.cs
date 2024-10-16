using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace EfiritPro.Retail.ProductModule.InputContracts;

public record ProductGroupListQuery : OwnerAndOrganizationIdQuery
{
    [FromQuery(Name = "justRoots")]
    [Required]
    public bool JustRoots { get; set; }
    [FromQuery(Name = "withProducts")]
    [Required]
    public bool WithProducts { get; set; }
};