using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace EfiritPro.Retail.ProductModule.InputContracts;

public record OwnerAndOrganizationIdQuery : OwnerIdQuery
{
    [FromQuery(Name = "organizationId")]
    [Required]
    public string OrganizationId { get; set; }
};