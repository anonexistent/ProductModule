using Microsoft.AspNetCore.Mvc;

namespace EfiritPro.Retail.ProductModule.InputContracts;

public record FavoriteProductListQuery : OwnerAndOrganizationIdQuery
{
    [FromQuery(Name = "workerId")]
    public string? WorkerId { get; set; }
}