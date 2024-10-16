using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace EfiritPro.Retail.ProductModule.InputContracts;

public record MarkingTypeIdQuery : OwnerIdQuery
{
    [FromQuery(Name = "markingTypeId")]
    [Required]
    public string MarkingTypeId { get; set; }
};