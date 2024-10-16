using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace EfiritPro.Retail.ProductModule.InputContracts
{
    public record ProductSetIdQuery : OwnerAndOrganizationIdQuery
    {
        [FromQuery(Name = "productSetId")]
        [Required]
        public string ProductSetId { get; set; }
    };
}
